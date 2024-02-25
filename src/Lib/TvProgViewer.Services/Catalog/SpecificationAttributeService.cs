using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TvProgViewer.Core;
using TvProgViewer.Core.Caching;
using TvProgViewer.Core.Domain.Catalog;
using TvProgViewer.Data;
using TvProgViewer.Services.Security;
using TvProgViewer.Services.Stores;

namespace TvProgViewer.Services.Catalog
{
    /// <summary>
    /// Specification attribute service
    /// </summary>
    public partial class SpecificationAttributeService : ISpecificationAttributeService
    {
        #region Fields

        private readonly CatalogSettings _catalogSettings;
        private readonly IAclService _aclService;
        private readonly ICategoryService _categoryService;
        private readonly IRepository<TvChannel> _tvchannelRepository;
        private readonly IRepository<TvChannelCategory> _tvchannelCategoryRepository;
        private readonly IRepository<TvChannelManufacturer> _tvchannelManufacturerRepository;
        private readonly IRepository<TvChannelSpecificationAttribute> _tvchannelSpecificationAttributeRepository;
        private readonly IRepository<SpecificationAttribute> _specificationAttributeRepository;
        private readonly IRepository<SpecificationAttributeOption> _specificationAttributeOptionRepository;
        private readonly IRepository<SpecificationAttributeGroup> _specificationAttributeGroupRepository;
        private readonly IStoreContext _storeContext;
        private readonly IStoreMappingService _storeMappingService;
        private readonly IStaticCacheManager _staticCacheManager;
        private readonly IWorkContext _workContext;

        #endregion

        #region Ctor

        public SpecificationAttributeService(
            CatalogSettings catalogSettings,
            IAclService aclService,
            ICategoryService categoryService,
            IRepository<TvChannel> tvchannelRepository,
            IRepository<TvChannelCategory> tvchannelCategoryRepository,
            IRepository<TvChannelManufacturer> tvchannelManufacturerRepository,
            IRepository<TvChannelSpecificationAttribute> tvchannelSpecificationAttributeRepository,
            IRepository<SpecificationAttribute> specificationAttributeRepository,
            IRepository<SpecificationAttributeOption> specificationAttributeOptionRepository,
            IRepository<SpecificationAttributeGroup> specificationAttributeGroupRepository,
            IStoreContext storeContext,
            IStoreMappingService storeMappingService,
            IStaticCacheManager staticCacheManager,
            IWorkContext workContext)
        {
            _catalogSettings = catalogSettings;
            _aclService = aclService;
            _categoryService = categoryService;
            _tvchannelRepository = tvchannelRepository;
            _tvchannelCategoryRepository = tvchannelCategoryRepository;
            _tvchannelManufacturerRepository = tvchannelManufacturerRepository;
            _tvchannelSpecificationAttributeRepository = tvchannelSpecificationAttributeRepository;
            _specificationAttributeRepository = specificationAttributeRepository;
            _specificationAttributeOptionRepository = specificationAttributeOptionRepository;
            _specificationAttributeGroupRepository = specificationAttributeGroupRepository;
            _storeContext = storeContext;
            _storeMappingService = storeMappingService;
            _staticCacheManager = staticCacheManager;
            _workContext = workContext;
        }

        #endregion

        #region Utilities

        /// <returns>Задача представляет асинхронную операцию</returns>
        protected virtual async Task<IQueryable<TvChannel>> GetAvailableTvChannelsQueryAsync()
        {
            var tvchannelsQuery = 
                from p in _tvchannelRepository.Table
                where !p.Deleted && p.Published &&
                      (p.ParentGroupedTvChannelId == 0 || p.VisibleIndividually) &&
                      (!p.AvailableStartDateTimeUtc.HasValue || p.AvailableStartDateTimeUtc <= DateTime.UtcNow) &&
                      (!p.AvailableEndDateTimeUtc.HasValue || p.AvailableEndDateTimeUtc >= DateTime.UtcNow)
                select p;

            var store = await _storeContext.GetCurrentStoreAsync();
            var currentUser = await _workContext.GetCurrentUserAsync();

            //apply store mapping constraints
            tvchannelsQuery = await _storeMappingService.ApplyStoreMapping(tvchannelsQuery, store.Id);

            //apply ACL constraints
            tvchannelsQuery = await _aclService.ApplyAcl(tvchannelsQuery, currentUser);

            return tvchannelsQuery;
        }

        #endregion

        #region Methods

        #region Specification attribute group

        /// <summary>
        /// Gets a specification attribute group
        /// </summary>
        /// <param name="specificationAttributeGroupId">The specification attribute group identifier</param>
        /// <returns>
        /// Задача представляет асинхронную операцию
        /// The task result contains the specification attribute group
        /// </returns>
        public virtual async Task<SpecificationAttributeGroup> GetSpecificationAttributeGroupByIdAsync(int specificationAttributeGroupId)
        {
            return await _specificationAttributeGroupRepository.GetByIdAsync(specificationAttributeGroupId, cache => default);
        }

        /// <summary>
        /// Gets specification attribute groups
        /// </summary>
        /// <param name="pageIndex">Page index</param>
        /// <param name="pageSize">Page size</param>
        /// <returns>
        /// Задача представляет асинхронную операцию
        /// The task result contains the specification attribute groups
        /// </returns>
        public virtual async Task<IPagedList<SpecificationAttributeGroup>> GetSpecificationAttributeGroupsAsync(int pageIndex = 0, int pageSize = int.MaxValue)
        {
            var query = from sag in _specificationAttributeGroupRepository.Table
                        orderby sag.DisplayOrder, sag.Id
                        select sag;

            return await query.ToPagedListAsync(pageIndex, pageSize);
        }

        /// <summary>
        /// Gets tvchannel specification attribute groups
        /// </summary>
        /// <param name="tvchannelId">TvChannel identifier</param>
        /// <returns>
        /// Задача представляет асинхронную операцию
        /// The task result contains the specification attribute groups
        /// </returns>
        public virtual async Task<IList<SpecificationAttributeGroup>> GetTvChannelSpecificationAttributeGroupsAsync(int tvchannelId)
        {
            var tvchannelAttributesForGroupQuery =
                from sa in _specificationAttributeRepository.Table
                join sao in _specificationAttributeOptionRepository.Table
                    on sa.Id equals sao.SpecificationAttributeId
                join psa in _tvchannelSpecificationAttributeRepository.Table
                    on sao.Id equals psa.SpecificationAttributeOptionId
                where psa.TvChannelId == tvchannelId && psa.ShowOnTvChannelPage
                select sa.SpecificationAttributeGroupId;

            var availableGroupsQuery =
                from sag in _specificationAttributeGroupRepository.Table
                where tvchannelAttributesForGroupQuery.Any(groupId => groupId == sag.Id)
                select sag;

            var key = _staticCacheManager.PrepareKeyForDefaultCache(TvProgCatalogDefaults.SpecificationAttributeGroupByTvChannelCacheKey, tvchannelId);

            return await _staticCacheManager.GetAsync(key, async () => await availableGroupsQuery.ToListAsync());
        }

        /// <summary>
        /// Deletes a specification attribute group
        /// </summary>
        /// <param name="specificationAttributeGroup">The specification attribute group</param>
        /// <returns>Задача представляет асинхронную операцию</returns>
        public virtual async Task DeleteSpecificationAttributeGroupAsync(SpecificationAttributeGroup specificationAttributeGroup)
        {
            await _specificationAttributeGroupRepository.DeleteAsync(specificationAttributeGroup);
        }

        /// <summary>
        /// Inserts a specification attribute group
        /// </summary>
        /// <param name="specificationAttributeGroup">The specification attribute group</param>
        /// <returns>Задача представляет асинхронную операцию</returns>
        public virtual async Task InsertSpecificationAttributeGroupAsync(SpecificationAttributeGroup specificationAttributeGroup)
        {
            await _specificationAttributeGroupRepository.InsertAsync(specificationAttributeGroup);
        }

        /// <summary>
        /// Updates the specification attribute group
        /// </summary>
        /// <param name="specificationAttributeGroup">The specification attribute group</param>
        /// <returns>Задача представляет асинхронную операцию</returns>
        public virtual async Task UpdateSpecificationAttributeGroupAsync(SpecificationAttributeGroup specificationAttributeGroup)
        {
            await _specificationAttributeGroupRepository.UpdateAsync(specificationAttributeGroup);
        }

        #endregion

        #region Specification attribute

        /// <summary>
        /// Gets a specification attribute
        /// </summary>
        /// <param name="specificationAttributeId">The specification attribute identifier</param>
        /// <returns>
        /// Задача представляет асинхронную операцию
        /// The task result contains the specification attribute
        /// </returns>
        public virtual async Task<SpecificationAttribute> GetSpecificationAttributeByIdAsync(int specificationAttributeId)
        {
            return await _specificationAttributeRepository.GetByIdAsync(specificationAttributeId, cache => default);
        }

        /// <summary>
        /// Gets specification attributes
        /// </summary>
        /// <param name="specificationAttributeIds">The specification attribute identifiers</param>
        /// <returns>
        /// Задача представляет асинхронную операцию
        /// The task result contains the specification attributes
        /// </returns>
        public virtual async Task<IList<SpecificationAttribute>> GetSpecificationAttributeByIdsAsync(int[] specificationAttributeIds)
        {
            return await _specificationAttributeRepository.GetByIdsAsync(specificationAttributeIds);
        }

        /// <summary>
        /// Gets specification attributes
        /// </summary>
        /// <param name="pageIndex">Page index</param>
        /// <param name="pageSize">Page size</param>
        /// <returns>
        /// Задача представляет асинхронную операцию
        /// The task result contains the specification attributes
        /// </returns>
        public virtual async Task<IPagedList<SpecificationAttribute>> GetSpecificationAttributesAsync(int pageIndex = 0, int pageSize = int.MaxValue)
        {
            var query = from sa in _specificationAttributeRepository.Table
                        orderby sa.DisplayOrder, sa.Id
                        select sa;

            return await query.ToPagedListAsync(pageIndex, pageSize);
        }

        /// <summary>
        /// Gets specification attributes that have options
        /// </summary>
        /// <returns>
        /// Задача представляет асинхронную операцию
        /// The task result contains the specification attributes that have available options
        /// </returns>
        public virtual async Task<IList<SpecificationAttribute>> GetSpecificationAttributesWithOptionsAsync()
        {
            var query = from sa in _specificationAttributeRepository.Table
                        where _specificationAttributeOptionRepository.Table.Any(o => o.SpecificationAttributeId == sa.Id)
                        orderby sa.DisplayOrder, sa.Id
                        select sa;

            return await _staticCacheManager.GetAsync(_staticCacheManager.PrepareKeyForDefaultCache(TvProgCatalogDefaults.SpecificationAttributesWithOptionsCacheKey), async () => await query.ToListAsync());
        }

        /// <summary>
        /// Gets specification attributes by group identifier
        /// </summary>
        /// <param name="specificationAttributeGroupId">The specification attribute group identifier</param>
        /// <returns>
        /// Задача представляет асинхронную операцию
        /// The task result contains the specification attributes
        /// </returns>
        public virtual async Task<IList<SpecificationAttribute>> GetSpecificationAttributesByGroupIdAsync(int? specificationAttributeGroupId = null)
        {
            var query = _specificationAttributeRepository.Table;
            if (!specificationAttributeGroupId.HasValue || specificationAttributeGroupId > 0)
                query = query.Where(sa => sa.SpecificationAttributeGroupId == specificationAttributeGroupId);

            query = query.OrderBy(sa => sa.DisplayOrder).ThenBy(sa => sa.Id);

            return await query.ToListAsync();
        }

        /// <summary>
        /// Deletes a specification attribute
        /// </summary>
        /// <param name="specificationAttribute">The specification attribute</param>
        /// <returns>Задача представляет асинхронную операцию</returns>
        public virtual async Task DeleteSpecificationAttributeAsync(SpecificationAttribute specificationAttribute)
        {
            await _specificationAttributeRepository.DeleteAsync(specificationAttribute);
        }

        /// <summary>
        /// Deletes specifications attributes
        /// </summary>
        /// <param name="specificationAttributes">Specification attributes</param>
        /// <returns>Задача представляет асинхронную операцию</returns>
        public virtual async Task DeleteSpecificationAttributesAsync(IList<SpecificationAttribute> specificationAttributes)
        {
            if (specificationAttributes == null)
                throw new ArgumentNullException(nameof(specificationAttributes));

            foreach (var specificationAttribute in specificationAttributes)
                await DeleteSpecificationAttributeAsync(specificationAttribute);
        }

        /// <summary>
        /// Inserts a specification attribute
        /// </summary>
        /// <param name="specificationAttribute">The specification attribute</param>
        /// <returns>Задача представляет асинхронную операцию</returns>
        public virtual async Task InsertSpecificationAttributeAsync(SpecificationAttribute specificationAttribute)
        {
            await _specificationAttributeRepository.InsertAsync(specificationAttribute);
        }

        /// <summary>
        /// Updates the specification attribute
        /// </summary>
        /// <param name="specificationAttribute">The specification attribute</param>
        /// <returns>Задача представляет асинхронную операцию</returns>
        public virtual async Task UpdateSpecificationAttributeAsync(SpecificationAttribute specificationAttribute)
        {
            await _specificationAttributeRepository.UpdateAsync(specificationAttribute);
        }

        #endregion

        #region Specification attribute option

        /// <summary>
        /// Gets a specification attribute option
        /// </summary>
        /// <param name="specificationAttributeOptionId">The specification attribute option identifier</param>
        /// <returns>
        /// Задача представляет асинхронную операцию
        /// The task result contains the specification attribute option
        /// </returns>
        public virtual async Task<SpecificationAttributeOption> GetSpecificationAttributeOptionByIdAsync(int specificationAttributeOptionId)
        {
            return await _specificationAttributeOptionRepository.GetByIdAsync(specificationAttributeOptionId, cache => default);
        }

        /// <summary>
        /// Get specification attribute options by identifiers
        /// </summary>
        /// <param name="specificationAttributeOptionIds">Identifiers</param>
        /// <returns>
        /// Задача представляет асинхронную операцию
        /// The task result contains the specification attribute options
        /// </returns>
        public virtual async Task<IList<SpecificationAttributeOption>> GetSpecificationAttributeOptionsByIdsAsync(int[] specificationAttributeOptionIds)
        {
            return await _specificationAttributeOptionRepository.GetByIdsAsync(specificationAttributeOptionIds);
        }

        /// <summary>
        /// Gets a specification attribute option by specification attribute id
        /// </summary>
        /// <param name="specificationAttributeId">The specification attribute identifier</param>
        /// <returns>
        /// Задача представляет асинхронную операцию
        /// The task result contains the specification attribute option
        /// </returns>
        public virtual async Task<IList<SpecificationAttributeOption>> GetSpecificationAttributeOptionsBySpecificationAttributeAsync(int specificationAttributeId)
        {
            var query = from sao in _specificationAttributeOptionRepository.Table
                        orderby sao.DisplayOrder, sao.Id
                        where sao.SpecificationAttributeId == specificationAttributeId
                        select sao;

            var specificationAttributeOptions = await _staticCacheManager.GetAsync(_staticCacheManager.PrepareKeyForDefaultCache(TvProgCatalogDefaults.SpecificationAttributeOptionsCacheKey, specificationAttributeId), async () => await query.ToListAsync());

            return specificationAttributeOptions;
        }

        /// <summary>
        /// Deletes a specification attribute option
        /// </summary>
        /// <param name="specificationAttributeOption">The specification attribute option</param>
        /// <returns>Задача представляет асинхронную операцию</returns>
        public virtual async Task DeleteSpecificationAttributeOptionAsync(SpecificationAttributeOption specificationAttributeOption)
        {
            await _specificationAttributeOptionRepository.DeleteAsync(specificationAttributeOption);
        }

        /// <summary>
        /// Inserts a specification attribute option
        /// </summary>
        /// <param name="specificationAttributeOption">The specification attribute option</param>
        /// <returns>Задача представляет асинхронную операцию</returns>
        public virtual async Task InsertSpecificationAttributeOptionAsync(SpecificationAttributeOption specificationAttributeOption)
        {
            await _specificationAttributeOptionRepository.InsertAsync(specificationAttributeOption);
        }

        /// <summary>
        /// Updates the specification attribute
        /// </summary>
        /// <param name="specificationAttributeOption">The specification attribute option</param>
        /// <returns>Задача представляет асинхронную операцию</returns>
        public virtual async Task UpdateSpecificationAttributeOptionAsync(SpecificationAttributeOption specificationAttributeOption)
        {
            await _specificationAttributeOptionRepository.UpdateAsync(specificationAttributeOption);
        }

        /// <summary>
        /// Returns a list of IDs of not existing specification attribute options
        /// </summary>
        /// <param name="attributeOptionIds">The IDs of the attribute options to check</param>
        /// <returns>
        /// Задача представляет асинхронную операцию
        /// The task result contains the list of IDs not existing specification attribute options
        /// </returns>
        public virtual async Task<int[]> GetNotExistingSpecificationAttributeOptionsAsync(int[] attributeOptionIds)
        {
            if (attributeOptionIds == null)
                throw new ArgumentNullException(nameof(attributeOptionIds));

            var query = _specificationAttributeOptionRepository.Table;
            var queryFilter = attributeOptionIds.Distinct().ToArray();
            var filter = await query.Select(a => a.Id)
                .Where(m => queryFilter.Contains(m))
                .ToListAsync();
            return queryFilter.Except(filter).ToArray();
        }

        /// <summary>
        /// Gets the filtrable specification attribute options by category id
        /// </summary>
        /// <param name="categoryId">The category id</param>
        /// <returns>
        /// Задача представляет асинхронную операцию
        /// The task result contains the specification attribute options
        /// </returns>
        public virtual async Task<IList<SpecificationAttributeOption>> GetFiltrableSpecificationAttributeOptionsByCategoryIdAsync(int categoryId)
        {
            if (categoryId <= 0)
                return new List<SpecificationAttributeOption>();

            var tvchannelsQuery = await GetAvailableTvChannelsQueryAsync();

            IList<int> subCategoryIds = null;

            if (_catalogSettings.ShowTvChannelsFromSubcategories)
            {
                var store = await _storeContext.GetCurrentStoreAsync();
                subCategoryIds = await _categoryService.GetChildCategoryIdsAsync(categoryId, store.Id);
            }
            
            var tvchannelCategoryQuery = 
                from pc in _tvchannelCategoryRepository.Table
                where (pc.CategoryId == categoryId || (_catalogSettings.ShowTvChannelsFromSubcategories && subCategoryIds.Contains(pc.CategoryId))) &&
                      (_catalogSettings.IncludeFeaturedTvChannelsInNormalLists || !pc.IsFeaturedTvChannel)
                select pc;

            var result = 
                from sao in _specificationAttributeOptionRepository.Table
                join psa in _tvchannelSpecificationAttributeRepository.Table on sao.Id equals psa.SpecificationAttributeOptionId
                join p in tvchannelsQuery on psa.TvChannelId equals p.Id
                join pc in tvchannelCategoryQuery on p.Id equals pc.TvChannelId
                join sa in _specificationAttributeRepository.Table on sao.SpecificationAttributeId equals sa.Id
                where psa.AllowFiltering
                orderby
                    sa.DisplayOrder, sa.Name,
                    sao.DisplayOrder, sao.Name
                //linq2db don't specify 'sa' in 'SELECT' statement
                //see also https://github.com/nopSolutions/tvProgViewer/issues/5425
                select new { sa, sao };

            var cacheKey = _staticCacheManager.PrepareKeyForDefaultCache(
                TvProgCatalogDefaults.SpecificationAttributeOptionsByCategoryCacheKey, categoryId.ToString());

            return await _staticCacheManager.GetAsync(cacheKey, async () => (await result.Distinct().ToListAsync()).Select(query => query.sao).ToList());
        }

        /// <summary>
        /// Gets the filtrable specification attribute options by manufacturer id
        /// </summary>
        /// <param name="manufacturerId">The manufacturer id</param>
        /// <returns>
        /// Задача представляет асинхронную операцию
        /// The task result contains the specification attribute options
        /// </returns>
        public virtual async Task<IList<SpecificationAttributeOption>> GetFiltrableSpecificationAttributeOptionsByManufacturerIdAsync(int manufacturerId)
        {
            if (manufacturerId <= 0)
                return new List<SpecificationAttributeOption>();

            var tvchannelsQuery = await GetAvailableTvChannelsQueryAsync();

            var tvchannelManufacturerQuery = 
                from pm in _tvchannelManufacturerRepository.Table
                where pm.ManufacturerId == manufacturerId && 
                      (_catalogSettings.IncludeFeaturedTvChannelsInNormalLists || !pm.IsFeaturedTvChannel)
                select pm;

            var result = 
                from sao in _specificationAttributeOptionRepository.Table
                join psa in _tvchannelSpecificationAttributeRepository.Table on sao.Id equals psa.SpecificationAttributeOptionId
                join p in tvchannelsQuery on psa.TvChannelId equals p.Id
                join pm in tvchannelManufacturerQuery on p.Id equals pm.TvChannelId
                join sa in _specificationAttributeRepository.Table on sao.SpecificationAttributeId equals sa.Id
                where psa.AllowFiltering
                orderby
                   sa.DisplayOrder, sa.Name,
                   sao.DisplayOrder, sao.Name
                //linq2db don't specify 'sa' in 'SELECT' statement
                //see also https://github.com/nopSolutions/tvProgViewer/issues/5425
                select new { sa, sao };

            var cacheKey = _staticCacheManager.PrepareKeyForDefaultCache(
                TvProgCatalogDefaults.SpecificationAttributeOptionsByManufacturerCacheKey, manufacturerId.ToString());

            return await _staticCacheManager.GetAsync(cacheKey, async () => (await result.Distinct().ToListAsync()).Select(query => query.sao).ToList());
        }

        #endregion

        #region TvChannel specification attribute

        /// <summary>
        /// Deletes a tvchannel specification attribute mapping
        /// </summary>
        /// <param name="tvchannelSpecificationAttribute">TvChannel specification attribute</param>
        /// <returns>Задача представляет асинхронную операцию</returns>
        public virtual async Task DeleteTvChannelSpecificationAttributeAsync(TvChannelSpecificationAttribute tvchannelSpecificationAttribute)
        {
            await _tvchannelSpecificationAttributeRepository.DeleteAsync(tvchannelSpecificationAttribute);
        }

        /// <summary>
        /// Gets a tvchannel specification attribute mapping collection
        /// </summary>
        /// <param name="tvchannelId">TvChannel identifier; 0 to load all records</param>
        /// <param name="specificationAttributeOptionId">Specification attribute option identifier; 0 to load all records</param>
        /// <param name="allowFiltering">0 to load attributes with AllowFiltering set to false, 1 to load attributes with AllowFiltering set to true, null to load all attributes</param>
        /// <param name="showOnTvChannelPage">0 to load attributes with ShowOnTvChannelPage set to false, 1 to load attributes with ShowOnTvChannelPage set to true, null to load all attributes</param>
        /// <param name="specificationAttributeGroupId">Specification attribute group identifier; 0 to load all records; null to load attributes without group</param>
        /// <returns>
        /// Задача представляет асинхронную операцию
        /// The task result contains the tvchannel specification attribute mapping collection
        /// </returns>
        public virtual async Task<IList<TvChannelSpecificationAttribute>> GetTvChannelSpecificationAttributesAsync(int tvchannelId = 0,
            int specificationAttributeOptionId = 0, bool? allowFiltering = null, bool? showOnTvChannelPage = null, int? specificationAttributeGroupId = 0)
        {
            var allowFilteringCacheStr = allowFiltering.HasValue ? allowFiltering.ToString() : "null";
            var showOnTvChannelPageCacheStr = showOnTvChannelPage.HasValue ? showOnTvChannelPage.ToString() : "null";
            var specificationAttributeGroupIdCacheStr = specificationAttributeGroupId.HasValue ? specificationAttributeGroupId.ToString() : "null";

            var key = _staticCacheManager.PrepareKeyForDefaultCache(TvProgCatalogDefaults.TvChannelSpecificationAttributeByTvChannelCacheKey,
                tvchannelId, specificationAttributeOptionId, allowFilteringCacheStr, showOnTvChannelPageCacheStr, specificationAttributeGroupIdCacheStr);

            var query = _tvchannelSpecificationAttributeRepository.Table;
            if (tvchannelId > 0)
                query = query.Where(psa => psa.TvChannelId == tvchannelId);
            if (specificationAttributeOptionId > 0)
                query = query.Where(psa => psa.SpecificationAttributeOptionId == specificationAttributeOptionId);
            if (allowFiltering.HasValue)
                query = query.Where(psa => psa.AllowFiltering == allowFiltering.Value);
            if (!specificationAttributeGroupId.HasValue || specificationAttributeGroupId > 0)
            {
                query = from psa in query
                        join sao in _specificationAttributeOptionRepository.Table
                            on psa.SpecificationAttributeOptionId equals sao.Id
                        join sa in _specificationAttributeRepository.Table
                            on sao.SpecificationAttributeId equals sa.Id
                        where sa.SpecificationAttributeGroupId == specificationAttributeGroupId
                        select psa;
            }
            if (showOnTvChannelPage.HasValue)
                query = query.Where(psa => psa.ShowOnTvChannelPage == showOnTvChannelPage.Value);
            query = query.OrderBy(psa => psa.DisplayOrder).ThenBy(psa => psa.Id);

            var tvchannelSpecificationAttributes = await _staticCacheManager.GetAsync(key, async () => await query.ToListAsync());

            return tvchannelSpecificationAttributes;
        }

        /// <summary>
        /// Gets a tvchannel specification attribute mapping 
        /// </summary>
        /// <param name="tvchannelSpecificationAttributeId">TvChannel specification attribute mapping identifier</param>
        /// <returns>
        /// Задача представляет асинхронную операцию
        /// The task result contains the tvchannel specification attribute mapping
        /// </returns>
        public virtual async Task<TvChannelSpecificationAttribute> GetTvChannelSpecificationAttributeByIdAsync(int tvchannelSpecificationAttributeId)
        {
            return await _tvchannelSpecificationAttributeRepository.GetByIdAsync(tvchannelSpecificationAttributeId);
        }

        /// <summary>
        /// Inserts a tvchannel specification attribute mapping
        /// </summary>
        /// <param name="tvchannelSpecificationAttribute">TvChannel specification attribute mapping</param>
        /// <returns>Задача представляет асинхронную операцию</returns>
        public virtual async Task InsertTvChannelSpecificationAttributeAsync(TvChannelSpecificationAttribute tvchannelSpecificationAttribute)
        {
            await _tvchannelSpecificationAttributeRepository.InsertAsync(tvchannelSpecificationAttribute);
        }

        /// <summary>
        /// Updates the tvchannel specification attribute mapping
        /// </summary>
        /// <param name="tvchannelSpecificationAttribute">TvChannel specification attribute mapping</param>
        /// <returns>Задача представляет асинхронную операцию</returns>
        public virtual async Task UpdateTvChannelSpecificationAttributeAsync(TvChannelSpecificationAttribute tvchannelSpecificationAttribute)
        {
            await _tvchannelSpecificationAttributeRepository.UpdateAsync(tvchannelSpecificationAttribute);
        }

        /// <summary>
        /// Gets a count of tvchannel specification attribute mapping records
        /// </summary>
        /// <param name="tvchannelId">TvChannel identifier; 0 to load all records</param>
        /// <param name="specificationAttributeOptionId">The specification attribute option identifier; 0 to load all records</param>
        /// <returns>
        /// Задача представляет асинхронную операцию
        /// The task result contains the count
        /// </returns>
        public virtual async Task<int> GetTvChannelSpecificationAttributeCountAsync(int tvchannelId = 0, int specificationAttributeOptionId = 0)
        {
            var query = _tvchannelSpecificationAttributeRepository.Table;
            if (tvchannelId > 0)
                query = query.Where(psa => psa.TvChannelId == tvchannelId);
            if (specificationAttributeOptionId > 0)
                query = query.Where(psa => psa.SpecificationAttributeOptionId == specificationAttributeOptionId);

            return await query.CountAsync();
        }

        /// <summary>
        /// Get mapped tvchannels for specification attribute
        /// </summary>
        /// <param name="specificationAttributeId">The specification attribute identifier</param>
        /// <param name="pageIndex">Page index</param>
        /// <param name="pageSize">Page size</param>
        /// <returns>
        /// Задача представляет асинхронную операцию
        /// The task result contains the tvchannels
        /// </returns>
        public virtual async Task<IPagedList<TvChannel>> GetTvChannelsBySpecificationAttributeIdAsync(int specificationAttributeId, int pageIndex, int pageSize)
        {
            var query = from tvchannel in _tvchannelRepository.Table
                join psa in _tvchannelSpecificationAttributeRepository.Table on tvchannel.Id equals psa.TvChannelId
                join spao in _specificationAttributeOptionRepository.Table on psa.SpecificationAttributeOptionId equals spao.Id
                where spao.SpecificationAttributeId == specificationAttributeId
                orderby tvchannel.Name
                select tvchannel;

            return await query.ToPagedListAsync(pageIndex, pageSize);
        }

        #endregion

        #endregion
    }
}
