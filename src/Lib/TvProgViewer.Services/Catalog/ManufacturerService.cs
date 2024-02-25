using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TvProgViewer.Core;
using TvProgViewer.Core.Caching;
using TvProgViewer.Core.Domain.Catalog;
using TvProgViewer.Core.Domain.Users;
using TvProgViewer.Core.Domain.Discounts;
using TvProgViewer.Data;
using TvProgViewer.Services.Users;
using TvProgViewer.Services.Discounts;
using TvProgViewer.Services.Security;
using TvProgViewer.Services.Stores;

namespace TvProgViewer.Services.Catalog
{
    /// <summary>
    /// Manufacturer service
    /// </summary>
    public partial class ManufacturerService : IManufacturerService
    {
        #region Fields

        private readonly CatalogSettings _catalogSettings;
        private readonly IAclService _aclService;
        private readonly ICategoryService _categoryService;
        private readonly IUserService _userService;
        private readonly IRepository<DiscountManufacturerMapping> _discountManufacturerMappingRepository;
        private readonly IRepository<Manufacturer> _manufacturerRepository;
        private readonly IRepository<TvChannel> _tvchannelRepository;
        private readonly IRepository<TvChannelManufacturer> _tvchannelManufacturerRepository;
        private readonly IRepository<TvChannelCategory> _tvchannelCategoryRepository;
        private readonly IStaticCacheManager _staticCacheManager;
        private readonly IStoreContext _storeContext;
        private readonly IStoreMappingService _storeMappingService;
        private readonly IWorkContext _workContext;

        #endregion

        #region Ctor

        public ManufacturerService(CatalogSettings catalogSettings,
            IAclService aclService,
            ICategoryService categoryService,
            IUserService userService,
            IRepository<DiscountManufacturerMapping> discountManufacturerMappingRepository,
            IRepository<Manufacturer> manufacturerRepository,
            IRepository<TvChannel> tvchannelRepository,
            IRepository<TvChannelManufacturer> tvchannelManufacturerRepository,
            IRepository<TvChannelCategory> tvchannelCategoryRepository,
            IStaticCacheManager staticCacheManager,
            IStoreContext storeContext,
            IStoreMappingService storeMappingService,
            IWorkContext workContext)
        {
            _catalogSettings = catalogSettings;
            _aclService = aclService;
            _categoryService = categoryService;
            _userService = userService;
            _discountManufacturerMappingRepository = discountManufacturerMappingRepository;
            _manufacturerRepository = manufacturerRepository;
            _tvchannelRepository = tvchannelRepository;
            _tvchannelManufacturerRepository = tvchannelManufacturerRepository;
            _tvchannelCategoryRepository = tvchannelCategoryRepository;
            _staticCacheManager = staticCacheManager;
            _storeContext = storeContext;
            _storeMappingService = storeMappingService;
            _workContext = workContext;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Clean up manufacturer references for a specified discount
        /// </summary>
        /// <param name="discount">Discount</param>
        /// <returns>Задача представляет асинхронную операцию</returns>
        public virtual async Task ClearDiscountManufacturerMappingAsync(Discount discount)
        {
            if (discount is null)
                throw new ArgumentNullException(nameof(discount));

            var mappings = _discountManufacturerMappingRepository.Table.Where(dcm => dcm.DiscountId == discount.Id);

            await _discountManufacturerMappingRepository.DeleteAsync(mappings.ToList());
        }

        /// <summary>
        /// Deletes a manufacturer
        /// </summary>
        /// <param name="manufacturer">Manufacturer</param>
        /// <returns>Задача представляет асинхронную операцию</returns>
        public virtual async Task DeleteManufacturerAsync(Manufacturer manufacturer)
        {
            await _manufacturerRepository.DeleteAsync(manufacturer);
        }

        /// <summary>
        /// Delete manufacturers
        /// </summary>
        /// <param name="manufacturers">Manufacturers</param>
        /// <returns>Задача представляет асинхронную операцию</returns>
        public virtual async Task DeleteManufacturersAsync(IList<Manufacturer> manufacturers)
        {
            await _manufacturerRepository.DeleteAsync(manufacturers);
        }

        /// <summary>
        /// Gets all manufacturers
        /// </summary>
        /// <param name="manufacturerName">Manufacturer name</param>
        /// <param name="storeId">Store identifier; 0 if you want to get all records</param>
        /// <param name="pageIndex">Page index</param>
        /// <param name="pageSize">Page size</param>
        /// <param name="showHidden">A value indicating whether to show hidden records</param>
        /// <param name="overridePublished">
        /// null - process "Published" property according to "showHidden" parameter
        /// true - load only "Published" tvchannels
        /// false - load only "Unpublished" tvchannels
        /// </param>
        /// <returns>
        /// Задача представляет асинхронную операцию
        /// The task result contains the manufacturers
        /// </returns>
        public virtual async Task<IPagedList<Manufacturer>> GetAllManufacturersAsync(string manufacturerName = "",
            int storeId = 0,
            int pageIndex = 0,
            int pageSize = int.MaxValue,
            bool showHidden = false,
            bool? overridePublished = null)
        {
            return await _manufacturerRepository.GetAllPagedAsync(async query =>
            {
                if (!showHidden)
                    query = query.Where(m => m.Published);
                else if (overridePublished.HasValue)
                    query = query.Where(m => m.Published == overridePublished.Value);

                if (!showHidden)
                {
                    //apply store mapping constraints
                    query = await _storeMappingService.ApplyStoreMapping(query, storeId);

                    //apply ACL constraints
                    var user = await _workContext.GetCurrentUserAsync();
                    query = await _aclService.ApplyAcl(query, user);
                }

                query = query.Where(m => !m.Deleted);

                if (!string.IsNullOrWhiteSpace(manufacturerName))
                    query = query.Where(m => m.Name.Contains(manufacturerName));

                return query.OrderBy(m => m.DisplayOrder).ThenBy(m => m.Id);
            }, pageIndex, pageSize);
        }

        /// <summary>
        /// Get manufacturer identifiers to which a discount is applied
        /// </summary>
        /// <param name="discount">Discount</param>
        /// <param name="user">User</param>
        /// <returns>
        /// Задача представляет асинхронную операцию
        /// The task result contains the manufacturer identifiers
        /// </returns>
        public virtual async Task<IList<int>> GetAppliedManufacturerIdsAsync(Discount discount, User user)
        {
            if (discount == null)
                throw new ArgumentNullException(nameof(discount));

            var cacheKey = _staticCacheManager.PrepareKeyForDefaultCache(TvProgDiscountDefaults.ManufacturerIdsByDiscountCacheKey,
                discount,
                await _userService.GetUserRoleIdsAsync(user),
                await _storeContext.GetCurrentStoreAsync());

            var query = _discountManufacturerMappingRepository.Table.Where(dmm => dmm.DiscountId == discount.Id)
                .Select(dmm => dmm.EntityId);

            var result = await _staticCacheManager.GetAsync(cacheKey, async () => await query.ToListAsync());

            return result;
        }

        /// <summary>
        /// Gets a manufacturer
        /// </summary>
        /// <param name="manufacturerId">Manufacturer identifier</param>
        /// <returns>
        /// Задача представляет асинхронную операцию
        /// The task result contains the manufacturer
        /// </returns>
        public virtual async Task<Manufacturer> GetManufacturerByIdAsync(int manufacturerId)
        {
            return await _manufacturerRepository.GetByIdAsync(manufacturerId, cache => default);
        }

        /// <summary>
        /// Get manufacturers for which a discount is applied
        /// </summary>
        /// <param name="discountId">Discount identifier; pass null to load all records</param>
        /// <param name="showHidden">A value indicating whether to load deleted manufacturers</param>
        /// <param name="pageIndex">Page index</param>
        /// <param name="pageSize">Page size</param>
        /// <returns>
        /// Задача представляет асинхронную операцию
        /// The task result contains the list of manufacturers
        /// </returns>
        public virtual async Task<IPagedList<Manufacturer>> GetManufacturersWithAppliedDiscountAsync(int? discountId = null,
            bool showHidden = false, int pageIndex = 0, int pageSize = int.MaxValue)
        {
            var manufacturers = _manufacturerRepository.Table;

            if (discountId.HasValue)
                manufacturers = from manufacturer in manufacturers
                                join dmm in _discountManufacturerMappingRepository.Table on manufacturer.Id equals dmm.EntityId
                                where dmm.DiscountId == discountId.Value
                                select manufacturer;

            if (!showHidden)
                manufacturers = manufacturers.Where(manufacturer => !manufacturer.Deleted);

            manufacturers = manufacturers.OrderBy(manufacturer => manufacturer.DisplayOrder).ThenBy(manufacturer => manufacturer.Id);

            return await manufacturers.ToPagedListAsync(pageIndex, pageSize);
        }

        /// <summary>
        /// Gets the manufacturers by category identifier
        /// </summary>
        /// <param name="categoryId">Cateogry identifier</param>
        /// <returns>
        /// Задача представляет асинхронную операцию
        /// The task result contains the manufacturers
        /// </returns>
        public virtual async Task<IList<Manufacturer>> GetManufacturersByCategoryIdAsync(int categoryId)
        {
            if (categoryId <= 0)
                return new List<Manufacturer>();

            // get available tvchannels in category
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

            var subCategoryIds = _catalogSettings.ShowTvChannelsFromSubcategories
                ? await _categoryService.GetChildCategoryIdsAsync(categoryId, store.Id)
                : null;

            var tvchannelCategoryQuery =
                from pc in _tvchannelCategoryRepository.Table
                where (pc.CategoryId == categoryId || (_catalogSettings.ShowTvChannelsFromSubcategories && subCategoryIds.Contains(pc.CategoryId))) &&
                      (_catalogSettings.IncludeFeaturedTvChannelsInNormalLists || !pc.IsFeaturedTvChannel)
                select pc;

            // get manufacturers of the tvchannels
            var manufacturersQuery =
                from m in _manufacturerRepository.Table
                join pm in _tvchannelManufacturerRepository.Table on m.Id equals pm.ManufacturerId
                join p in tvchannelsQuery on pm.TvChannelId equals p.Id
                join pc in tvchannelCategoryQuery on p.Id equals pc.TvChannelId
                where !m.Deleted
                orderby
                   m.DisplayOrder, m.Name
                select m;

            var key = _staticCacheManager
                .PrepareKeyForDefaultCache(TvProgCatalogDefaults.ManufacturersByCategoryCacheKey, categoryId.ToString());

            return await _staticCacheManager.GetAsync(key, async () => await manufacturersQuery.Distinct().ToListAsync());
        }

        /// <summary>
        /// Gets manufacturers by identifier
        /// </summary>
        /// <param name="manufacturerIds">manufacturer identifiers</param>
        /// <returns>
        /// Задача представляет асинхронную операцию
        /// The task result contains the manufacturers
        /// </returns>
        public virtual async Task<IList<Manufacturer>> GetManufacturersByIdsAsync(int[] manufacturerIds)
        {
            return await _manufacturerRepository.GetByIdsAsync(manufacturerIds, includeDeleted: false);
        }

        /// <summary>
        /// Inserts a manufacturer
        /// </summary>
        /// <param name="manufacturer">Manufacturer</param>
        /// <returns>Задача представляет асинхронную операцию</returns>
        public virtual async Task InsertManufacturerAsync(Manufacturer manufacturer)
        {
            await _manufacturerRepository.InsertAsync(manufacturer);
        }

        /// <summary>
        /// Updates the manufacturer
        /// </summary>
        /// <param name="manufacturer">Manufacturer</param>
        /// <returns>Задача представляет асинхронную операцию</returns>
        public virtual async Task UpdateManufacturerAsync(Manufacturer manufacturer)
        {
            await _manufacturerRepository.UpdateAsync(manufacturer);
        }

        /// <summary>
        /// Deletes a tvchannel manufacturer mapping
        /// </summary>
        /// <param name="tvchannelManufacturer">TvChannel manufacturer mapping</param>
        /// <returns>Задача представляет асинхронную операцию</returns>
        public virtual async Task DeleteTvChannelManufacturerAsync(TvChannelManufacturer tvchannelManufacturer)
        {
            await _tvchannelManufacturerRepository.DeleteAsync(tvchannelManufacturer);
        }

        /// <summary>
        /// Gets tvchannel manufacturer collection
        /// </summary>
        /// <param name="manufacturerId">Manufacturer identifier</param>
        /// <param name="pageIndex">Page index</param>
        /// <param name="pageSize">Page size</param>
        /// <param name="showHidden">A value indicating whether to show hidden records</param>
        /// <returns>
        /// Задача представляет асинхронную операцию
        /// The task result contains the tvchannel manufacturer collection
        /// </returns>
        public virtual async Task<IPagedList<TvChannelManufacturer>> GetTvChannelManufacturersByManufacturerIdAsync(int manufacturerId,
            int pageIndex = 0, int pageSize = int.MaxValue, bool showHidden = false)
        {
            if (manufacturerId == 0)
                return new PagedList<TvChannelManufacturer>(new List<TvChannelManufacturer>(), pageIndex, pageSize);

            var query = from pm in _tvchannelManufacturerRepository.Table
                        join p in _tvchannelRepository.Table on pm.TvChannelId equals p.Id
                        where pm.ManufacturerId == manufacturerId && !p.Deleted
                        orderby pm.DisplayOrder, pm.Id
                        select pm;

            if (!showHidden)
            {
                var manufacturersQuery = _manufacturerRepository.Table.Where(m => m.Published);

                //apply store mapping constraints
                var store = await _storeContext.GetCurrentStoreAsync();
                manufacturersQuery = await _storeMappingService.ApplyStoreMapping(manufacturersQuery, store.Id);

                //apply ACL constraints
                var user = await _workContext.GetCurrentUserAsync();
                manufacturersQuery = await _aclService.ApplyAcl(manufacturersQuery, user);

                query = query.Where(pm => manufacturersQuery.Any(m => m.Id == pm.ManufacturerId));
            }

            return await query.ToPagedListAsync(pageIndex, pageSize);
        }

        /// <summary>
        /// Gets a tvchannel manufacturer mapping collection
        /// </summary>
        /// <param name="tvchannelId">TvChannel identifier</param>
        /// <param name="showHidden">A value indicating whether to show hidden records</param>
        /// <returns>
        /// Задача представляет асинхронную операцию
        /// The task result contains the tvchannel manufacturer mapping collection
        /// </returns>
        public virtual async Task<IList<TvChannelManufacturer>> GetTvChannelManufacturersByTvChannelIdAsync(int tvchannelId,
            bool showHidden = false)
        {
            if (tvchannelId == 0)
                return new List<TvChannelManufacturer>();

            var store = await _storeContext.GetCurrentStoreAsync();
            var user = await _workContext.GetCurrentUserAsync();

            var key = _staticCacheManager
                .PrepareKeyForDefaultCache(TvProgCatalogDefaults.TvChannelManufacturersByTvChannelCacheKey, tvchannelId, showHidden, user, store);

            var query = from pm in _tvchannelManufacturerRepository.Table
                        join m in _manufacturerRepository.Table on pm.ManufacturerId equals m.Id
                        where pm.TvChannelId == tvchannelId && !m.Deleted
                        orderby pm.DisplayOrder, pm.Id
                        select pm;

            if (!showHidden)
            {
                var manufacturersQuery = _manufacturerRepository.Table.Where(m => m.Published);

                //apply store mapping constraints
                manufacturersQuery = await _storeMappingService.ApplyStoreMapping(manufacturersQuery, store.Id);

                //apply ACL constraints
                manufacturersQuery = await _aclService.ApplyAcl(manufacturersQuery, user);

                query = query.Where(pm => manufacturersQuery.Any(m => m.Id == pm.ManufacturerId));
            }

            return await _staticCacheManager.GetAsync(key, query.ToList);
        }

        /// <summary>
        /// Gets a tvchannel manufacturer mapping 
        /// </summary>
        /// <param name="tvchannelManufacturerId">TvChannel manufacturer mapping identifier</param>
        /// <returns>
        /// Задача представляет асинхронную операцию
        /// The task result contains the tvchannel manufacturer mapping
        /// </returns>
        public virtual async Task<TvChannelManufacturer> GetTvChannelManufacturerByIdAsync(int tvchannelManufacturerId)
        {
            return await _tvchannelManufacturerRepository.GetByIdAsync(tvchannelManufacturerId, cache => default);
        }

        /// <summary>
        /// Inserts a tvchannel manufacturer mapping
        /// </summary>
        /// <param name="tvchannelManufacturer">TvChannel manufacturer mapping</param>
        /// <returns>Задача представляет асинхронную операцию</returns>
        public virtual async Task InsertTvChannelManufacturerAsync(TvChannelManufacturer tvchannelManufacturer)
        {
            await _tvchannelManufacturerRepository.InsertAsync(tvchannelManufacturer);
        }

        /// <summary>
        /// Updates the tvchannel manufacturer mapping
        /// </summary>
        /// <param name="tvchannelManufacturer">TvChannel manufacturer mapping</param>
        /// <returns>Задача представляет асинхронную операцию</returns>
        public virtual async Task UpdateTvChannelManufacturerAsync(TvChannelManufacturer tvchannelManufacturer)
        {
            await _tvchannelManufacturerRepository.UpdateAsync(tvchannelManufacturer);
        }

        /// <summary>
        /// Get manufacturer IDs for tvchannels
        /// </summary>
        /// <param name="tvchannelIds">TvChannels IDs</param>
        /// <returns>
        /// Задача представляет асинхронную операцию
        /// The task result contains the manufacturer IDs for tvchannels
        /// </returns>
        public virtual async Task<IDictionary<int, int[]>> GetTvChannelManufacturerIdsAsync(int[] tvchannelIds)
        {
            var query = _tvchannelManufacturerRepository.Table;

            return (await query.Where(p => tvchannelIds.Contains(p.TvChannelId))
                .Select(p => new { p.TvChannelId, p.ManufacturerId })
                .ToListAsync())
                .GroupBy(a => a.TvChannelId)
                .ToDictionary(items => items.Key, items => items.Select(a => a.ManufacturerId).ToArray());
        }

        /// <summary>
        /// Returns a list of names of not existing manufacturers
        /// </summary>
        /// <param name="manufacturerIdsNames">The names and/or IDs of the manufacturers to check</param>
        /// <returns>
        /// Задача представляет асинхронную операцию
        /// The task result contains the list of names and/or IDs not existing manufacturers
        /// </returns>
        public virtual async Task<string[]> GetNotExistingManufacturersAsync(string[] manufacturerIdsNames)
        {
            if (manufacturerIdsNames == null)
                throw new ArgumentNullException(nameof(manufacturerIdsNames));

            var query = _manufacturerRepository.Table;
            var queryFilter = manufacturerIdsNames.Distinct().ToArray();
            //filtering by name
            var filter = query.Select(m => m.Name).Where(m => queryFilter.Contains(m)).ToList();
            queryFilter = queryFilter.Except(filter).ToArray();

            //if some names not found
            if (!queryFilter.Any())
                return queryFilter.ToArray();

            //filtering by IDs
            filter = await query.Select(c => c.Id.ToString())
                .Where(c => queryFilter.Contains(c))
                .ToListAsync();

            return queryFilter.Except(filter).ToArray();
        }

        /// <summary>
        /// Returns a TvChannelManufacturer that has the specified values
        /// </summary>
        /// <param name="source">Source</param>
        /// <param name="tvchannelId">TvChannel identifier</param>
        /// <param name="manufacturerId">Manufacturer identifier</param>
        /// <returns>A TvChannelManufacturer that has the specified values; otherwise null</returns>
        public virtual TvChannelManufacturer FindTvChannelManufacturer(IList<TvChannelManufacturer> source, int tvchannelId, int manufacturerId)
        {
            foreach (var tvchannelManufacturer in source)
                if (tvchannelManufacturer.TvChannelId == tvchannelId && tvchannelManufacturer.ManufacturerId == manufacturerId)
                    return tvchannelManufacturer;

            return null;
        }

        /// <summary>
        /// Get a discount-manufacturer mapping record
        /// </summary>
        /// <param name="manufacturerId">Manufacturer identifier</param>
        /// <param name="discountId">Discount identifier</param>
        /// <returns>
        /// Задача представляет асинхронную операцию
        /// The task result contains the result
        /// </returns>
        public async Task<DiscountManufacturerMapping> GetDiscountAppliedToManufacturerAsync(int manufacturerId, int discountId)
        {
            return await _discountManufacturerMappingRepository.Table
                .FirstOrDefaultAsync(dcm => dcm.EntityId == manufacturerId && dcm.DiscountId == discountId);
        }

        /// <summary>
        /// Inserts a discount-manufacturer mapping record
        /// </summary>
        /// <param name="discountManufacturerMapping">Discount-manufacturer mapping</param>
        /// <returns>Задача представляет асинхронную операцию</returns>
        public async Task InsertDiscountManufacturerMappingAsync(DiscountManufacturerMapping discountManufacturerMapping)
        {
            await _discountManufacturerMappingRepository.InsertAsync(discountManufacturerMapping);
        }

        /// <summary>
        /// Deletes a discount-manufacturer mapping record
        /// </summary>
        /// <param name="discountManufacturerMapping">Discount-manufacturer mapping</param>
        /// <returns>Задача представляет асинхронную операцию</returns>
        public async Task DeleteDiscountManufacturerMappingAsync(DiscountManufacturerMapping discountManufacturerMapping)
        {
            await _discountManufacturerMappingRepository.DeleteAsync(discountManufacturerMapping);
        }

        #endregion
    }
}