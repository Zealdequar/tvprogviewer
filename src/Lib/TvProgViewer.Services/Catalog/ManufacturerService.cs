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
        private readonly IRepository<TvChannel> _tvChannelRepository;
        private readonly IRepository<TvChannelManufacturer> _tvChannelManufacturerRepository;
        private readonly IRepository<TvChannelCategory> _tvChannelCategoryRepository;
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
            IRepository<TvChannel> tvChannelRepository,
            IRepository<TvChannelManufacturer> tvChannelManufacturerRepository,
            IRepository<TvChannelCategory> tvChannelCategoryRepository,
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
            _tvChannelRepository = tvChannelRepository;
            _tvChannelManufacturerRepository = tvChannelManufacturerRepository;
            _tvChannelCategoryRepository = tvChannelCategoryRepository;
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
        /// true - load only "Published" tvChannels
        /// false - load only "Unpublished" tvChannels
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

            // get available tvChannels in category
            var tvChannelsQuery =
                from p in _tvChannelRepository.Table
                where !p.Deleted && p.Published &&
                      (p.ParentGroupedTvChannelId == 0 || p.VisibleIndividually) &&
                      (!p.AvailableStartDateTimeUtc.HasValue || p.AvailableStartDateTimeUtc <= DateTime.UtcNow) &&
                      (!p.AvailableEndDateTimeUtc.HasValue || p.AvailableEndDateTimeUtc >= DateTime.UtcNow)
                select p;

            var store = await _storeContext.GetCurrentStoreAsync();
            var currentUser = await _workContext.GetCurrentUserAsync();

            //apply store mapping constraints
            tvChannelsQuery = await _storeMappingService.ApplyStoreMapping(tvChannelsQuery, store.Id);

            //apply ACL constraints
            tvChannelsQuery = await _aclService.ApplyAcl(tvChannelsQuery, currentUser);

            var subCategoryIds = _catalogSettings.ShowTvChannelsFromSubcategories
                ? await _categoryService.GetChildCategoryIdsAsync(categoryId, store.Id)
                : null;

            var tvChannelCategoryQuery =
                from pc in _tvChannelCategoryRepository.Table
                where (pc.CategoryId == categoryId || (_catalogSettings.ShowTvChannelsFromSubcategories && subCategoryIds.Contains(pc.CategoryId))) &&
                      (_catalogSettings.IncludeFeaturedTvChannelsInNormalLists || !pc.IsFeaturedTvChannel)
                select pc;

            // get manufacturers of the tvChannels
            var manufacturersQuery =
                from m in _manufacturerRepository.Table
                join pm in _tvChannelManufacturerRepository.Table on m.Id equals pm.ManufacturerId
                join p in tvChannelsQuery on pm.TvChannelId equals p.Id
                join pc in tvChannelCategoryQuery on p.Id equals pc.TvChannelId
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
        /// Deletes a tvChannel manufacturer mapping
        /// </summary>
        /// <param name="tvChannelManufacturer">TvChannel manufacturer mapping</param>
        /// <returns>Задача представляет асинхронную операцию</returns>
        public virtual async Task DeleteTvChannelManufacturerAsync(TvChannelManufacturer tvChannelManufacturer)
        {
            await _tvChannelManufacturerRepository.DeleteAsync(tvChannelManufacturer);
        }

        /// <summary>
        /// Gets tvChannel manufacturer collection
        /// </summary>
        /// <param name="manufacturerId">Manufacturer identifier</param>
        /// <param name="pageIndex">Page index</param>
        /// <param name="pageSize">Page size</param>
        /// <param name="showHidden">A value indicating whether to show hidden records</param>
        /// <returns>
        /// Задача представляет асинхронную операцию
        /// The task result contains the tvChannel manufacturer collection
        /// </returns>
        public virtual async Task<IPagedList<TvChannelManufacturer>> GetTvChannelManufacturersByManufacturerIdAsync(int manufacturerId,
            int pageIndex = 0, int pageSize = int.MaxValue, bool showHidden = false)
        {
            if (manufacturerId == 0)
                return new PagedList<TvChannelManufacturer>(new List<TvChannelManufacturer>(), pageIndex, pageSize);

            var query = from pm in _tvChannelManufacturerRepository.Table
                        join p in _tvChannelRepository.Table on pm.TvChannelId equals p.Id
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
        /// Gets a tvChannel manufacturer mapping collection
        /// </summary>
        /// <param name="tvChannelId">TvChannel identifier</param>
        /// <param name="showHidden">A value indicating whether to show hidden records</param>
        /// <returns>
        /// Задача представляет асинхронную операцию
        /// The task result contains the tvChannel manufacturer mapping collection
        /// </returns>
        public virtual async Task<IList<TvChannelManufacturer>> GetTvChannelManufacturersByTvChannelIdAsync(int tvChannelId,
            bool showHidden = false)
        {
            if (tvChannelId == 0)
                return new List<TvChannelManufacturer>();

            var store = await _storeContext.GetCurrentStoreAsync();
            var user = await _workContext.GetCurrentUserAsync();

            var key = _staticCacheManager
                .PrepareKeyForDefaultCache(TvProgCatalogDefaults.TvChannelManufacturersByTvChannelCacheKey, tvChannelId, showHidden, user, store);

            var query = from pm in _tvChannelManufacturerRepository.Table
                        join m in _manufacturerRepository.Table on pm.ManufacturerId equals m.Id
                        where pm.TvChannelId == tvChannelId && !m.Deleted
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
        /// Gets a tvChannel manufacturer mapping 
        /// </summary>
        /// <param name="tvChannelManufacturerId">TvChannel manufacturer mapping identifier</param>
        /// <returns>
        /// Задача представляет асинхронную операцию
        /// The task result contains the tvChannel manufacturer mapping
        /// </returns>
        public virtual async Task<TvChannelManufacturer> GetTvChannelManufacturerByIdAsync(int tvChannelManufacturerId)
        {
            return await _tvChannelManufacturerRepository.GetByIdAsync(tvChannelManufacturerId, cache => default);
        }

        /// <summary>
        /// Inserts a tvChannel manufacturer mapping
        /// </summary>
        /// <param name="tvChannelManufacturer">TvChannel manufacturer mapping</param>
        /// <returns>Задача представляет асинхронную операцию</returns>
        public virtual async Task InsertTvChannelManufacturerAsync(TvChannelManufacturer tvChannelManufacturer)
        {
            await _tvChannelManufacturerRepository.InsertAsync(tvChannelManufacturer);
        }

        /// <summary>
        /// Updates the tvChannel manufacturer mapping
        /// </summary>
        /// <param name="tvChannelManufacturer">TvChannel manufacturer mapping</param>
        /// <returns>Задача представляет асинхронную операцию</returns>
        public virtual async Task UpdateTvChannelManufacturerAsync(TvChannelManufacturer tvChannelManufacturer)
        {
            await _tvChannelManufacturerRepository.UpdateAsync(tvChannelManufacturer);
        }

        /// <summary>
        /// Get manufacturer IDs for tvChannels
        /// </summary>
        /// <param name="tvChannelIds">TvChannels IDs</param>
        /// <returns>
        /// Задача представляет асинхронную операцию
        /// The task result contains the manufacturer IDs for tvChannels
        /// </returns>
        public virtual async Task<IDictionary<int, int[]>> GetTvChannelManufacturerIdsAsync(int[] tvChannelIds)
        {
            var query = _tvChannelManufacturerRepository.Table;

            return (await query.Where(p => tvChannelIds.Contains(p.TvChannelId))
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
        /// <param name="tvChannelId">TvChannel identifier</param>
        /// <param name="manufacturerId">Manufacturer identifier</param>
        /// <returns>A TvChannelManufacturer that has the specified values; otherwise null</returns>
        public virtual TvChannelManufacturer FindTvChannelManufacturer(IList<TvChannelManufacturer> source, int tvChannelId, int manufacturerId)
        {
            foreach (var tvChannelManufacturer in source)
                if (tvChannelManufacturer.TvChannelId == tvChannelId && tvChannelManufacturer.ManufacturerId == manufacturerId)
                    return tvChannelManufacturer;

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