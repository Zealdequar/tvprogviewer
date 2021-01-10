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
        public virtual UserRoleSearchModel PrepareUserRoleSearchModel(UserRoleSearchModel searchModel)
        {
            if (searchModel == null)
                throw new ArgumentNullException(nameof(searchModel));

            //prepare page parameters
            searchModel.SetGridPageSize();

            return searchModel;
        }

        /// <summary>
        /// Prepare paged user role list model
        /// </summary>
        /// <param name="searchModel">User role search model</param>
        /// <returns>User role list model</returns>
        public virtual UserRoleListModel PrepareUserRoleListModel(UserRoleSearchModel searchModel)
        {
            if (searchModel == null)
                throw new ArgumentNullException(nameof(searchModel));

            //get user roles
            var userRoles = _userService.GetAllUserRoles(true).ToPagedList(searchModel);

            //prepare grid model
            var model = new UserRoleListModel().PrepareToGrid(searchModel, userRoles, () =>
            {
                return userRoles.Select(role =>
                {
                    //fill in model values from the entity
                    var userRoleModel = role.ToModel<UserRoleModel>();

                    //fill in additional values (not existing in the entity)
                    userRoleModel.PurchasedWithProductName = _productService.GetProductById(role.PurchasedWithProductId)?.Name;

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
        public virtual UserRoleModel PrepareUserRoleModel(UserRoleModel model, UserRole userRole, bool excludeProperties = false)
        {
            if (userRole != null)
            {
                //fill in model values from the entity
                model ??= userRole.ToModel<UserRoleModel>();
                model.PurchasedWithProductName = _productService.GetProductById(userRole.PurchasedWithProductId)?.Name;
            }

            //set default values for the new model
            if (userRole == null)
                model.Active = true;

            //prepare available tax display types
            _baseAdminModelFactory.PrepareTaxDisplayTypes(model.TaxDisplayTypeValues, false);

            return model;
        }

        /// <summary>
        /// Prepare user role product search model
        /// </summary>
        /// <param name="searchModel">User role product search model</param>
        /// <returns>User role product search model</returns>
        public virtual UserRoleProductSearchModel PrepareUserRoleProductSearchModel(UserRoleProductSearchModel searchModel)
        {
            if (searchModel == null)
                throw new ArgumentNullException(nameof(searchModel));

            //a vendor should have access only to his products
            searchModel.IsLoggedInAsVendor = _workContext.CurrentVendor != null;

            //prepare available categories
            _baseAdminModelFactory.PrepareCategories(searchModel.AvailableCategories);

            //prepare available manufacturers
            _baseAdminModelFactory.PrepareManufacturers(searchModel.AvailableManufacturers);

            //prepare available stores
            _baseAdminModelFactory.PrepareStores(searchModel.AvailableStores);

            //prepare available vendors
            _baseAdminModelFactory.PrepareVendors(searchModel.AvailableVendors);

            //prepare available product types
            _baseAdminModelFactory.PrepareProductTypes(searchModel.AvailableProductTypes);

            //prepare page parameters
            searchModel.SetPopupGridPageSize();

            return searchModel;
        }

        /// <summary>
        /// Prepare paged user role product list model
        /// </summary>
        /// <param name="searchModel">User role product search model</param>
        /// <returns>User role product list model</returns>
        public virtual UserRoleProductListModel PrepareUserRoleProductListModel(UserRoleProductSearchModel searchModel)
        {
            if (searchModel == null)
                throw new ArgumentNullException(nameof(searchModel));

            //a vendor should have access only to his products
            if (_workContext.CurrentVendor != null)
                searchModel.SearchVendorId = _workContext.CurrentVendor.Id;

            //get products
            var products = _productService.SearchProducts(showHidden: true,
                categoryIds: new List<int> { searchModel.SearchCategoryId },
                manufacturerId: searchModel.SearchManufacturerId,
                storeId: searchModel.SearchStoreId,
                vendorId: searchModel.SearchVendorId,
                productType: searchModel.SearchProductTypeId > 0 ? (ProductType?)searchModel.SearchProductTypeId : null,
                keywords: searchModel.SearchProductName,
                pageIndex: searchModel.Page - 1, pageSize: searchModel.PageSize);

            //prepare grid model
            var model = new UserRoleProductListModel().PrepareToGrid(searchModel, products, () =>
            {
                return products.Select(product =>
                {
                    var productModel = product.ToModel<ProductModel>();
                    productModel.SeName = _urlRecordService.GetSeName(product, 0, true, false);

                    return productModel;
                });
            });

            return model;
        }

        #endregion
    }
}