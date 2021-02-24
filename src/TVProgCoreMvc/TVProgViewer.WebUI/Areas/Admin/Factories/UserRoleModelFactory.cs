using System;
using System.Collections.Generic;
using System.Linq;
using TVProgViewer.Core;
using TVProgViewer.Core.Domain.Catalog;
using TVProgViewer.Core.Domain.Users;
using TVProgViewer.Services.Catalog;
using TVProgViewer.Services.Users;
using TVProgViewer.Services.Seo;
using TVProgViewer.WebUI.Areas.Admin.Infrastructure.Mapper.Extensions;
using TVProgViewer.WebUI.Areas.Admin.Models.Catalog;
using TVProgViewer.WebUI.Areas.Admin.Models.Users;
using TVProgViewer.Web.Framework.Models.Extensions;
using System.Threading.Tasks;

namespace TVProgViewer.WebUI.Areas.Admin.Factories
{
    /// <summary>
    /// Represents the user role model factory implementation
    /// </summary>
    public partial class UserRoleModelFactory : IUserRoleModelFactory
    {
        #region Fields

        private readonly IBaseAdminModelFactory _baseAdminModelFactory;
        private readonly IUserService _userService;
        private readonly IProductService _productService;
        private readonly IUrlRecordService _urlRecordService;
        private readonly IWorkContext _workContext;

        #endregion

        #region Ctor

        public UserRoleModelFactory(IBaseAdminModelFactory baseAdminModelFactory,
            IUserService userService,
            IProductService productService,
            IUrlRecordService urlRecordService,
            IWorkContext workContext)
        {
            _baseAdminModelFactory = baseAdminModelFactory;
            _userService = userService;
            _productService = productService;
            _urlRecordService = urlRecordService;
            _workContext = workContext;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Prepare user role search model
        /// </summary>
        /// <param name="searchModel">User role search model</param>
        /// <returns>User role search model</returns>
        public virtual Task<UserRoleSearchModel> PrepareUserRoleSearchModelAsync(UserRoleSearchModel searchModel)
        {
            if (searchModel == null)
                throw new ArgumentNullException(nameof(searchModel));

            //prepare page parameters
            searchModel.SetGridPageSize();

            return Task.FromResult(searchModel);
        }

        /// <summary>
        /// Prepare paged user role list model
        /// </summary>
        /// <param name="searchModel">User role search model</param>
        /// <returns>User role list model</returns>
        public virtual async Task<UserRoleListModel> PrepareUserRoleListModelAsync(UserRoleSearchModel searchModel)
        {
            if (searchModel == null)
                throw new ArgumentNullException(nameof(searchModel));

            //get user roles
            var userRoles = (await _userService.GetAllUserRolesAsync(true)).ToPagedList(searchModel);

            //prepare grid model
            var model = await new UserRoleListModel().PrepareToGridAsync(searchModel, userRoles, () =>
            {
                return userRoles.SelectAwait(async role =>
                {
                    //fill in model values from the entity
                    var userRoleModel = role.ToModel<UserRoleModel>();

                    //fill in additional values (not existing in the entity)
                    userRoleModel.PurchasedWithProductName = (await _productService.GetProductByIdAsync(role.PurchasedWithProductId))?.Name;

                    return userRoleModel;
                });
            });

            return model;
        }

        /// <summary>
        /// Prepare user role model
        /// </summary>
        /// <param name="model">User role model</param>
        /// <param name="userRole">User role</param>
        /// <param name="excludeProperties">Whether to exclude populating of some properties of model</param>
        /// <returns>User role model</returns>
        public virtual async Task<UserRoleModel> PrepareUserRoleModelAsync(UserRoleModel model, UserRole userRole, bool excludeProperties = false)
        {
            if (userRole != null)
            {
                //fill in model values from the entity
                model ??= userRole.ToModel<UserRoleModel>();
                model.PurchasedWithProductName = (await _productService.GetProductByIdAsync(userRole.PurchasedWithProductId))?.Name;
            }

            //set default values for the new model
            if (userRole == null)
                model.Active = true;

            //prepare available tax display types
            await _baseAdminModelFactory.PrepareTaxDisplayTypesAsync(model.TaxDisplayTypeValues, false);

            return model;
        }

        /// <summary>
        /// Prepare user role product search model
        /// </summary>
        /// <param name="searchModel">User role product search model</param>
        /// <returns>User role product search model</returns>
        public virtual async Task<UserRoleProductSearchModel> PrepareUserRoleProductSearchModelAsync(UserRoleProductSearchModel searchModel)
        {
            if (searchModel == null)
                throw new ArgumentNullException(nameof(searchModel));

            //a vendor should have access only to his products
            searchModel.IsLoggedInAsVendor = await _workContext.GetCurrentVendorAsync() != null;

            //prepare available categories
            await _baseAdminModelFactory.PrepareCategoriesAsync(searchModel.AvailableCategories);

            //prepare available manufacturers
            await _baseAdminModelFactory.PrepareManufacturersAsync(searchModel.AvailableManufacturers);

            //prepare available stores
            await _baseAdminModelFactory.PrepareStoresAsync(searchModel.AvailableStores);

            //prepare available vendors
            await _baseAdminModelFactory.PrepareVendorsAsync(searchModel.AvailableVendors);

            //prepare available product types
            await _baseAdminModelFactory.PrepareProductTypesAsync(searchModel.AvailableProductTypes);

            //prepare page parameters
            searchModel.SetPopupGridPageSize();

            return searchModel;
        }

        /// <summary>
        /// Prepare paged user role product list model
        /// </summary>
        /// <param name="searchModel">User role product search model</param>
        /// <returns>User role product list model</returns>
        public virtual async Task<UserRoleProductListModel> PrepareUserRoleProductListModelAsync(UserRoleProductSearchModel searchModel)
        {
            if (searchModel == null)
                throw new ArgumentNullException(nameof(searchModel));

            //a vendor should have access only to his products
            if (await _workContext.GetCurrentVendorAsync() != null)
                searchModel.SearchVendorId = (await _workContext.GetCurrentVendorAsync()).Id;

            //get products
            var (products, _) = await _productService.SearchProductsAsync(false, showHidden: true,
                categoryIds: new List<int> { searchModel.SearchCategoryId },
                manufacturerId: searchModel.SearchManufacturerId,
                storeId: searchModel.SearchStoreId,
                vendorId: searchModel.SearchVendorId,
                productType: searchModel.SearchProductTypeId > 0 ? (ProductType?)searchModel.SearchProductTypeId : null,
                keywords: searchModel.SearchProductName,
                pageIndex: searchModel.Page - 1, pageSize: searchModel.PageSize);

            //prepare grid model
            var model = await new UserRoleProductListModel().PrepareToGridAsync(searchModel, products, () =>
            {
                return products.SelectAwait(async product =>
                {
                    var productModel = product.ToModel<ProductModel>();

                    productModel.SeName = await _urlRecordService.GetSeNameAsync(product, 0, true, false);

                    return productModel;
                });
            });

            return model;
        }

        #endregion
    }
}