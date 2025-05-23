﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TvProgViewer.Core;
using TvProgViewer.Core.Domain.Catalog;
using TvProgViewer.Core.Domain.Users;
using TvProgViewer.Services.Catalog;
using TvProgViewer.Services.Users;
using TvProgViewer.Services.Seo;
using TvProgViewer.WebUI.Areas.Admin.Infrastructure.Mapper.Extensions;
using TvProgViewer.WebUI.Areas.Admin.Models.Catalog;
using TvProgViewer.WebUI.Areas.Admin.Models.Users;
using TvProgViewer.Web.Framework.Models.Extensions;

namespace TvProgViewer.WebUI.Areas.Admin.Factories
{
    /// <summary>
    /// Represents the user role model factory implementation
    /// </summary>
    public partial class UserRoleModelFactory : IUserRoleModelFactory
    {
        #region Fields

        private readonly IBaseAdminModelFactory _baseAdminModelFactory;
        private readonly IUserService _userService;
        private readonly ITvChannelService _tvChannelService;
        private readonly IUrlRecordService _urlRecordService;
        private readonly IWorkContext _workContext;

        #endregion

        #region Ctor

        public UserRoleModelFactory(IBaseAdminModelFactory baseAdminModelFactory,
            IUserService userService,
            ITvChannelService tvChannelService,
            IUrlRecordService urlRecordService,
            IWorkContext workContext)
        {
            _baseAdminModelFactory = baseAdminModelFactory;
            _userService = userService;
            _tvChannelService = tvChannelService;
            _urlRecordService = urlRecordService;
            _workContext = workContext;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Prepare user role search model
        /// </summary>
        /// <param name="searchModel">User role search model</param>
        /// <returns>
        /// Задача представляет асинхронную операцию
        /// The task result contains the user role search model
        /// </returns>
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
        /// <returns>
        /// Задача представляет асинхронную операцию
        /// The task result contains the user role list model
        /// </returns>
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
                    userRoleModel.PurchasedWithTvChannelName = (await _tvChannelService.GetTvChannelByIdAsync(role.PurchasedWithTvChannelId))?.Name;

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
        /// <returns>
        /// Задача представляет асинхронную операцию
        /// The task result contains the user role model
        /// </returns>
        public virtual async Task<UserRoleModel> PrepareUserRoleModelAsync(UserRoleModel model, UserRole userRole, bool excludeProperties = false)
        {
            if (userRole != null)
            {
                //fill in model values from the entity
                model ??= userRole.ToModel<UserRoleModel>();
                model.PurchasedWithTvChannelName = (await _tvChannelService.GetTvChannelByIdAsync(userRole.PurchasedWithTvChannelId))?.Name;
            }

            //set default values for the new model
            if (userRole == null)
                model.Active = true;

            //prepare available tax display types
            await _baseAdminModelFactory.PrepareTaxDisplayTypesAsync(model.TaxDisplayTypeValues, false);

            return model;
        }

        /// <summary>
        /// Prepare user role tvChannel search model
        /// </summary>
        /// <param name="searchModel">User role tvChannel search model</param>
        /// <returns>
        /// Задача представляет асинхронную операцию
        /// The task result contains the user role tvChannel search model
        /// </returns>
        public virtual async Task<UserRoleTvChannelSearchModel> PrepareUserRoleTvChannelSearchModelAsync(UserRoleTvChannelSearchModel searchModel)
        {
            if (searchModel == null)
                throw new ArgumentNullException(nameof(searchModel));

            //a vendor should have access only to his tvChannels
            searchModel.IsLoggedInAsVendor = await _workContext.GetCurrentVendorAsync() != null;

            //prepare available categories
            await _baseAdminModelFactory.PrepareCategoriesAsync(searchModel.AvailableCategories);

            //prepare available manufacturers
            await _baseAdminModelFactory.PrepareManufacturersAsync(searchModel.AvailableManufacturers);

            //prepare available stores
            await _baseAdminModelFactory.PrepareStoresAsync(searchModel.AvailableStores);

            //prepare available vendors
            await _baseAdminModelFactory.PrepareVendorsAsync(searchModel.AvailableVendors);

            //prepare available tvChannel types
            await _baseAdminModelFactory.PrepareTvChannelTypesAsync(searchModel.AvailableTvChannelTypes);

            //prepare page parameters
            searchModel.SetPopupGridPageSize();

            return searchModel;
        }

        /// <summary>
        /// Prepare paged user role tvChannel list model
        /// </summary>
        /// <param name="searchModel">User role tvChannel search model</param>
        /// <returns>
        /// Задача представляет асинхронную операцию
        /// The task result contains the user role tvChannel list model
        /// </returns>
        public virtual async Task<UserRoleTvChannelListModel> PrepareUserRoleTvChannelListModelAsync(UserRoleTvChannelSearchModel searchModel)
        {
            if (searchModel == null)
                throw new ArgumentNullException(nameof(searchModel));

            //a vendor should have access only to his tvChannels
            var currentVendor = await _workContext.GetCurrentVendorAsync();
            if (currentVendor != null)
                searchModel.SearchVendorId = currentVendor.Id;

            //get tvChannels
            var tvChannels = await _tvChannelService.SearchTvChannelsAsync(showHidden: true,
                categoryIds: new List<int> { searchModel.SearchCategoryId },
                manufacturerIds: new List<int> { searchModel.SearchManufacturerId },
                storeId: searchModel.SearchStoreId,
                vendorId: searchModel.SearchVendorId,
                tvChannelType: searchModel.SearchTvChannelTypeId > 0 ? (TvChannelType?)searchModel.SearchTvChannelTypeId : null,
                keywords: searchModel.SearchTvChannelName,
                pageIndex: searchModel.Page - 1, pageSize: searchModel.PageSize);

            //prepare grid model
            var model = await new UserRoleTvChannelListModel().PrepareToGridAsync(searchModel, tvChannels, () =>
            {
                return tvChannels.SelectAwait(async tvChannel =>
                {
                    var tvChannelModel = tvChannel.ToModel<TvChannelModel>();

                    tvChannelModel.SeName = await _urlRecordService.GetSeNameAsync(tvChannel, 0, true, false);

                    return tvChannelModel;
                });
            });

            return model;
        }

        #endregion
    }
}