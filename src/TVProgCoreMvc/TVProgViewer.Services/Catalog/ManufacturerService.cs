﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TVProgViewer.Core;
using TVProgViewer.Core.Caching;
using TVProgViewer.Core.Domain.Catalog;
using TVProgViewer.Core.Domain.Users;
using TVProgViewer.Core.Domain.Discounts;
using TVProgViewer.Data;
using TVProgViewer.Services.Users;
using TVProgViewer.Services.Discounts;
using TVProgViewer.Services.Security;
using TVProgViewer.Services.Stores;

namespace TVProgViewer.Services.Catalog
{
    /// <summary>
    /// Manufacturer service
    /// </summary>
    public partial class ManufacturerService : IManufacturerService
    {
        #region Fields

        private readonly CatalogSettings _catalogSettings;
        private readonly IAclService _aclService;
        private readonly IUserService _userService;
        private readonly IRepository<DiscountManufacturerMapping> _discountManufacturerMappingRepository;
        private readonly IRepository<Manufacturer> _manufacturerRepository;
        private readonly IRepository<Product> _productRepository;
        private readonly IRepository<ProductManufacturer> _productManufacturerRepository;
        private readonly IStaticCacheManager _staticCacheManager;
        private readonly IStoreContext _storeContext;
        private readonly IStoreMappingService _storeMappingService;
        private readonly IWorkContext _workContext;

        #endregion

        #region Ctor

        public ManufacturerService(CatalogSettings catalogSettings,
            IAclService aclService,
            IUserService userService,
            IRepository<DiscountManufacturerMapping> discountManufacturerMappingRepository,
            IRepository<Manufacturer> manufacturerRepository,
            IRepository<Product> productRepository,
            IRepository<ProductManufacturer> productManufacturerRepository,
            IStaticCacheManager staticCacheManager,
            IStoreContext storeContext,
            IStoreMappingService storeMappingService,
            IWorkContext workContext)
        {
            _catalogSettings = catalogSettings;
            _aclService = aclService;
            _userService = userService;
            _discountManufacturerMappingRepository = discountManufacturerMappingRepository;
            _manufacturerRepository = manufacturerRepository;
            _productRepository = productRepository;
            _productManufacturerRepository = productManufacturerRepository;
            _staticCacheManager = staticCacheManager;
            _storeContext = storeContext;
            _storeMappingService = storeMappingService;
            _workContext = workContext;
        }

        #endregion

        #region Utilities

        /// <summary>
        /// Filter hidden entries according to constraints if any
        /// </summary>
        /// <param name="query">Query to filter</param>
        /// <param name="storeId">A store identifier</param>
        /// <param name="userRolesIds">Identifiers of user's roles</param>
        /// <returns>Filtered query</returns>
        protected virtual async Task<IQueryable<TEntity>> FilterHiddenEntriesAsync<TEntity>(IQueryable<TEntity> query,
            int storeId, int[] userRolesIds)
            where TEntity : Manufacturer
        {
            //filter unpublished entries
            query = query.Where(entry => entry.Published);

            //apply store mapping constraints
            if (!_catalogSettings.IgnoreStoreLimitations && await _storeMappingService.IsEntityMappingExistsAsync<TEntity>(storeId))
                query = query.Where(_storeMappingService.ApplyStoreMapping<TEntity>(storeId));

            //apply ACL constraints
            if (!_catalogSettings.IgnoreAcl && await _aclService.IsEntityAclMappingExistAsync<TEntity>(userRolesIds))
                query = query.Where(_aclService.ApplyAcl<TEntity>(userRolesIds));

            return query;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Clean up manufacturer references for a specified discount
        /// </summary>
        /// <param name="discount">Discount</param>
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
        public virtual async Task DeleteManufacturerAsync(Manufacturer manufacturer)
        {
            await _manufacturerRepository.DeleteAsync(manufacturer);
        }

        /// <summary>
        /// Delete manufacturers
        /// </summary>
        /// <param name="manufacturers">Manufacturers</param>
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
        /// true - load only "Published" products
        /// false - load only "Unpublished" products
        /// </param>
        /// <returns>Manufacturers</returns>
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
                {
                    var user = await _workContext.GetCurrentUserAsync();
                    var userRolesIds = await _userService.GetUserRoleIdsAsync(user);
                    query = await FilterHiddenEntriesAsync(query, storeId, userRolesIds);
                }
                else if (overridePublished.HasValue)
                    query = query.Where(m => m.Published == overridePublished.Value);

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
        /// <returns>Manufacturer identifiers</returns>
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
        /// <returns>Manufacturer</returns>
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
        /// <returns>List of manufacturers</returns>
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
        /// Gets manufacturers by identifier
        /// </summary>
        /// <param name="manufacturerIds">manufacturer identifiers</param>
        /// <returns>Manufacturers</returns>
        public virtual async Task<IList<Manufacturer>> GetManufacturersByIdsAsync(int[] manufacturerIds)
        {
            return await _manufacturerRepository.GetByIdsAsync(manufacturerIds);
        }

        /// <summary>
        /// Inserts a manufacturer
        /// </summary>
        /// <param name="manufacturer">Manufacturer</param>
        public virtual async Task InsertManufacturerAsync(Manufacturer manufacturer)
        {
            await _manufacturerRepository.InsertAsync(manufacturer);
        }

        /// <summary>
        /// Updates the manufacturer
        /// </summary>
        /// <param name="manufacturer">Manufacturer</param>
        public virtual async Task UpdateManufacturerAsync(Manufacturer manufacturer)
        {
            await _manufacturerRepository.UpdateAsync(manufacturer);
        }

        /// <summary>
        /// Deletes a product manufacturer mapping
        /// </summary>
        /// <param name="productManufacturer">Product manufacturer mapping</param>
        public virtual async Task DeleteProductManufacturerAsync(ProductManufacturer productManufacturer)
        {
            await _productManufacturerRepository.DeleteAsync(productManufacturer);
        }

        /// <summary>
        /// Gets product manufacturer collection
        /// </summary>
        /// <param name="manufacturerId">Manufacturer identifier</param>
        /// <param name="pageIndex">Page index</param>
        /// <param name="pageSize">Page size</param>
        /// <param name="showHidden">A value indicating whether to show hidden records</param>
        /// <returns>Product manufacturer collection</returns>
        public virtual async Task<IPagedList<ProductManufacturer>> GetProductManufacturersByManufacturerIdAsync(int manufacturerId,
            int pageIndex = 0, int pageSize = int.MaxValue, bool showHidden = false)
        {
            if (manufacturerId == 0)
                return new PagedList<ProductManufacturer>(new List<ProductManufacturer>(), pageIndex, pageSize);

            var query = from pm in _productManufacturerRepository.Table
                        join p in _productRepository.Table on pm.ProductId equals p.Id
                        where pm.ManufacturerId == manufacturerId && !p.Deleted
                        orderby pm.DisplayOrder, pm.Id
                        select pm;

            if (!showHidden)
            {
                var storeId = (await _storeContext.GetCurrentStoreAsync()).Id;
                var user = await _workContext.GetCurrentUserAsync();
                var userRolesIds = await _userService.GetUserRoleIdsAsync(user);
                var manufacturersQuery = await FilterHiddenEntriesAsync(_manufacturerRepository.Table, storeId, userRolesIds);
                query = query.Where(pm => manufacturersQuery.Any(m => m.Id == pm.ManufacturerId));
            }

            return await query.ToPagedListAsync(pageIndex, pageSize);
        }

        /// <summary>
        /// Gets a product manufacturer mapping collection
        /// </summary>
        /// <param name="productId">Product identifier</param>
        /// <param name="showHidden">A value indicating whether to show hidden records</param>
        /// <returns>Product manufacturer mapping collection</returns>
        public virtual async Task<IList<ProductManufacturer>> GetProductManufacturersByProductIdAsync(int productId,
            bool showHidden = false)
        {
            if (productId == 0)
                return new List<ProductManufacturer>();

            var storeId = (await _storeContext.GetCurrentStoreAsync()).Id;
            var user = await _workContext.GetCurrentUserAsync();
            var userRolesIds = await _userService.GetUserRoleIdsAsync(user);

            var key = _staticCacheManager
                .PrepareKeyForDefaultCache(TvProgCatalogDefaults.ProductManufacturersByProductCacheKey, productId, showHidden, user, storeId);

            var query = from pm in _productManufacturerRepository.Table
                        join m in _manufacturerRepository.Table on pm.ManufacturerId equals m.Id
                        where pm.ProductId == productId && !m.Deleted
                        orderby pm.DisplayOrder, pm.Id
                        select pm;

            if (!showHidden)
            {
                var manufacturersQuery = await FilterHiddenEntriesAsync(_manufacturerRepository.Table, storeId, userRolesIds);
                query = query.Where(pm => manufacturersQuery.Any(m => m.Id == pm.ManufacturerId));
            }

            return await _staticCacheManager.GetAsync(key, query.ToList);
        }

        /// <summary>
        /// Gets a product manufacturer mapping 
        /// </summary>
        /// <param name="productManufacturerId">Product manufacturer mapping identifier</param>
        /// <returns>Product manufacturer mapping</returns>
        public virtual async Task<ProductManufacturer> GetProductManufacturerByIdAsync(int productManufacturerId)
        {
            return await _productManufacturerRepository.GetByIdAsync(productManufacturerId, cache => default);
        }

        /// <summary>
        /// Inserts a product manufacturer mapping
        /// </summary>
        /// <param name="productManufacturer">Product manufacturer mapping</param>
        public virtual async Task InsertProductManufacturerAsync(ProductManufacturer productManufacturer)
        {
            await _productManufacturerRepository.InsertAsync(productManufacturer);
        }

        /// <summary>
        /// Updates the product manufacturer mapping
        /// </summary>
        /// <param name="productManufacturer">Product manufacturer mapping</param>
        public virtual async Task UpdateProductManufacturerAsync(ProductManufacturer productManufacturer)
        {
            await _productManufacturerRepository.UpdateAsync(productManufacturer);
        }

        /// <summary>
        /// Get manufacturer IDs for products
        /// </summary>
        /// <param name="productIds">Products IDs</param>
        /// <returns>Manufacturer IDs for products</returns>
        public virtual async Task<IDictionary<int, int[]>> GetProductManufacturerIdsAsync(int[] productIds)
        {
            var query = _productManufacturerRepository.Table;

            return (await query.Where(p => productIds.Contains(p.ProductId))
                .Select(p => new { p.ProductId, p.ManufacturerId })
                .ToListAsync())
                .GroupBy(a => a.ProductId)
                .ToDictionary(items => items.Key, items => items.Select(a => a.ManufacturerId).ToArray());
        }

        /// <summary>
        /// Returns a list of names of not existing manufacturers
        /// </summary>
        /// <param name="manufacturerIdsNames">The names and/or IDs of the manufacturers to check</param>
        /// <returns>List of names and/or IDs not existing manufacturers</returns>
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
        /// Returns a ProductManufacturer that has the specified values
        /// </summary>
        /// <param name="source">Source</param>
        /// <param name="productId">Product identifier</param>
        /// <param name="manufacturerId">Manufacturer identifier</param>
        /// <returns>A ProductManufacturer that has the specified values; otherwise null</returns>
        public virtual ProductManufacturer FindProductManufacturer(IList<ProductManufacturer> source, int productId, int manufacturerId)
        {
            foreach (var productManufacturer in source)
                if (productManufacturer.ProductId == productId && productManufacturer.ManufacturerId == manufacturerId)
                    return productManufacturer;

            return null;
        }

        /// <summary>
        /// Get a discount-manufacturer mapping record
        /// </summary>
        /// <param name="manufacturerId">Manufacturer identifier</param>
        /// <param name="discountId">Discount identifier</param>
        /// <returns>Result</returns>
        public async Task<DiscountManufacturerMapping> GetDiscountAppliedToManufacturerAsync(int manufacturerId, int discountId)
        {
            return await _discountManufacturerMappingRepository.Table
                .FirstOrDefaultAsync(dcm => dcm.EntityId == manufacturerId && dcm.DiscountId == discountId);
        }

        /// <summary>
        /// Inserts a discount-manufacturer mapping record
        /// </summary>
        /// <param name="discountManufacturerMapping">Discount-manufacturer mapping</param>
        public async Task InsertDiscountManufacturerMappingAsync(DiscountManufacturerMapping discountManufacturerMapping)
        {
            await _discountManufacturerMappingRepository.InsertAsync(discountManufacturerMapping);
        }

        /// <summary>
        /// Deletes a discount-manufacturer mapping record
        /// </summary>
        /// <param name="discountManufacturerMapping">Discount-manufacturer mapping</param>
        public async Task DeleteDiscountManufacturerMappingAsync(DiscountManufacturerMapping discountManufacturerMapping)
        {
            await _discountManufacturerMappingRepository.DeleteAsync(discountManufacturerMapping);
        }

        #endregion
    }
}