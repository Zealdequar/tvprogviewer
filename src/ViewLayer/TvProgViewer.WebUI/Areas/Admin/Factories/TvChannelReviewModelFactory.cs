using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Rendering;
using TvProgViewer.Core;
using TvProgViewer.Core.Domain.Catalog;
using TvProgViewer.Core.Domain.Users;
using TvProgViewer.Services.Catalog;
using TvProgViewer.Services.Users;
using TvProgViewer.Services.Helpers;
using TvProgViewer.Services.Html;
using TvProgViewer.Services.Localization;
using TvProgViewer.Services.Stores;
using TvProgViewer.WebUI.Areas.Admin.Infrastructure.Mapper.Extensions;
using TvProgViewer.WebUI.Areas.Admin.Models.Catalog;
using TvProgViewer.Web.Framework.Extensions;
using TvProgViewer.Web.Framework.Models.Extensions;

namespace TvProgViewer.WebUI.Areas.Admin.Factories
{
    /// <summary>
    /// Represents the tvChannel review model factory implementation
    /// </summary>
    public partial class TvChannelReviewModelFactory : ITvChannelReviewModelFactory
    {
        #region Fields

        private readonly CatalogSettings _catalogSettings;
        private readonly IBaseAdminModelFactory _baseAdminModelFactory;
        private readonly IUserService _userService;
        private readonly IDateTimeHelper _dateTimeHelper;
        private readonly IHtmlFormatter _htmlFormatter;
        private readonly ILocalizationService _localizationService;
        private readonly ITvChannelService _tvChannelService;
        private readonly IReviewTypeService _reviewTypeService;
        private readonly IStoreService _storeService;
        private readonly IWorkContext _workContext;

        #endregion

        #region Ctor

        public TvChannelReviewModelFactory(CatalogSettings catalogSettings,
            IBaseAdminModelFactory baseAdminModelFactory,
            IUserService userService,
            IDateTimeHelper dateTimeHelper,
            IHtmlFormatter htmlFormatter,
            ILocalizationService localizationService,
            ITvChannelService tvChannelService,
            IReviewTypeService reviewTypeService,
            IStoreService storeService,
            IWorkContext workContext)
        {
            _catalogSettings = catalogSettings;
            _baseAdminModelFactory = baseAdminModelFactory;
            _userService = userService;
            _dateTimeHelper = dateTimeHelper;
            _htmlFormatter = htmlFormatter;
            _localizationService = localizationService;
            _tvChannelService = tvChannelService;
            _reviewTypeService = reviewTypeService;
            _storeService = storeService;
            _workContext = workContext;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Prepare tvChannel review search model
        /// </summary>
        /// <param name="searchModel">TvChannel review search model</param>
        /// <returns>
        /// Задача представляет асинхронную операцию
        /// The task result contains the tvChannel review search model
        /// </returns>
        public virtual async Task<TvChannelReviewSearchModel> PrepareTvChannelReviewSearchModelAsync(TvChannelReviewSearchModel searchModel)
        {
            if (searchModel == null)
                throw new ArgumentNullException(nameof(searchModel));

            searchModel.IsLoggedInAsVendor = await _workContext.GetCurrentVendorAsync() != null;

            //prepare available stores
            await _baseAdminModelFactory.PrepareStoresAsync(searchModel.AvailableStores);

            //prepare "approved" property (0 - all; 1 - approved only; 2 - disapproved only)
            searchModel.AvailableApprovedOptions.Add(new SelectListItem
            {
                Text = await _localizationService.GetResourceAsync("Admin.Catalog.TvChannelReviews.List.SearchApproved.All"),
                Value = "0"
            });
            searchModel.AvailableApprovedOptions.Add(new SelectListItem
            {
                Text = await _localizationService.GetResourceAsync("Admin.Catalog.TvChannelReviews.List.SearchApproved.ApprovedOnly"),
                Value = "1"
            });
            searchModel.AvailableApprovedOptions.Add(new SelectListItem
            {
                Text = await _localizationService.GetResourceAsync("Admin.Catalog.TvChannelReviews.List.SearchApproved.DisapprovedOnly"),
                Value = "2"
            });

            searchModel.HideStoresList = _catalogSettings.IgnoreStoreLimitations || searchModel.AvailableStores.SelectionIsNotPossible();

            //prepare page parameters
            searchModel.SetGridPageSize();

            return searchModel;
        }

        /// <summary>
        /// Prepare paged tvChannel review list model
        /// </summary>
        /// <param name="searchModel">TvChannel review search model</param>
        /// <returns>
        /// Задача представляет асинхронную операцию
        /// The task result contains the tvChannel review list model
        /// </returns>
        public virtual async Task<TvChannelReviewListModel> PrepareTvChannelReviewListModelAsync(TvChannelReviewSearchModel searchModel)
        {
            if (searchModel == null)
                throw new ArgumentNullException(nameof(searchModel));

            //get parameters to filter reviews
            var createdOnFromValue = !searchModel.CreatedOnFrom.HasValue ? null
                : (DateTime?)_dateTimeHelper.ConvertToUtcTime(searchModel.CreatedOnFrom.Value, await _dateTimeHelper.GetCurrentTimeZoneAsync());
            var createdToFromValue = !searchModel.CreatedOnTo.HasValue ? null
                : (DateTime?)_dateTimeHelper.ConvertToUtcTime(searchModel.CreatedOnTo.Value, await _dateTimeHelper.GetCurrentTimeZoneAsync()).AddDays(1);
            var isApprovedOnly = searchModel.SearchApprovedId == 0 ? null : searchModel.SearchApprovedId == 1 ? true : (bool?)false;
            var vendor = await _workContext.GetCurrentVendorAsync();
            var vendorId = vendor?.Id ?? 0;

            //get tvChannel reviews
            var tvChannelReviews = await _tvChannelService.GetAllTvChannelReviewsAsync(showHidden: true,
                userId: 0,
                approved: isApprovedOnly,
                fromUtc: createdOnFromValue,
                toUtc: createdToFromValue,
                message: searchModel.SearchText,
                storeId: searchModel.SearchStoreId,
                tvChannelId: searchModel.SearchTvChannelId,
                vendorId: vendorId,
                pageIndex: searchModel.Page - 1, pageSize: searchModel.PageSize);

            //prepare list model
            var model = await new TvChannelReviewListModel().PrepareToGridAsync(searchModel, tvChannelReviews, () =>
            {
                return tvChannelReviews.SelectAwait(async tvChannelReview =>
                {
                    //fill in model values from the entity
                    var tvChannelReviewModel = tvChannelReview.ToModel<TvChannelReviewModel>();

                    //convert dates to the user time
                    tvChannelReviewModel.CreatedOn = await _dateTimeHelper.ConvertToUserTimeAsync(tvChannelReview.CreatedOnUtc, DateTimeKind.Utc);

                    //fill in additional values (not existing in the entity)
                    tvChannelReviewModel.StoreName = (await _storeService.GetStoreByIdAsync(tvChannelReview.StoreId))?.Name;
                    tvChannelReviewModel.TvChannelName = (await _tvChannelService.GetTvChannelByIdAsync(tvChannelReview.TvChannelId))?.Name;
                    tvChannelReviewModel.UserInfo = (await _userService.GetUserByIdAsync(tvChannelReview.UserId)) is User user && (await _userService.IsRegisteredAsync(user))
                        ? user.Email
                        : await _localizationService.GetResourceAsync("Admin.Users.Guest");

                    tvChannelReviewModel.ReviewText = _htmlFormatter.FormatText(tvChannelReview.ReviewText, false, true, false, false, false, false);
                    tvChannelReviewModel.ReplyText = _htmlFormatter.FormatText(tvChannelReview.ReplyText, false, true, false, false, false, false);

                    return tvChannelReviewModel;
                });
            });

            return model;
        }

        /// <summary>
        /// Prepare tvChannel review model
        /// </summary>
        /// <param name="model">TvChannel review model</param>
        /// <param name="tvChannelReview">TvChannel review</param>
        /// <param name="excludeProperties">Whether to exclude populating of some properties of model</param>
        /// <returns>
        /// Задача представляет асинхронную операцию
        /// The task result contains the tvChannel review model
        /// </returns>
        public virtual async Task<TvChannelReviewModel> PrepareTvChannelReviewModelAsync(TvChannelReviewModel model,
            TvChannelReview tvChannelReview, bool excludeProperties = false)
        {
            if (tvChannelReview != null)
            {
                var showStoreName = (await _storeService.GetAllStoresAsync()).Count > 1;

                //fill in model values from the entity
                model ??= new TvChannelReviewModel
                {
                    Id = tvChannelReview.Id,
                    StoreName = showStoreName ? (await _storeService.GetStoreByIdAsync(tvChannelReview.StoreId))?.Name : string.Empty,
                    TvChannelId = tvChannelReview.TvChannelId,
                    TvChannelName = (await _tvChannelService.GetTvChannelByIdAsync(tvChannelReview.TvChannelId))?.Name,
                    UserId = tvChannelReview.UserId,
                    Rating = tvChannelReview.Rating
                };

                model.ShowStoreName = showStoreName;

                model.UserInfo = await _userService.GetUserByIdAsync(tvChannelReview.UserId) is User user && await _userService.IsRegisteredAsync(user)
                    ? user.Email : await _localizationService.GetResourceAsync("Admin.Users.Guest");

                model.CreatedOn = await _dateTimeHelper.ConvertToUserTimeAsync(tvChannelReview.CreatedOnUtc, DateTimeKind.Utc);

                if (!excludeProperties)
                {
                    model.Title = tvChannelReview.Title;
                    model.ReviewText = tvChannelReview.ReviewText;
                    model.ReplyText = tvChannelReview.ReplyText;
                    model.IsApproved = tvChannelReview.IsApproved;
                }

                //prepare nested search model
                await PrepareTvChannelReviewReviewTypeMappingSearchModelAsync(model.TvChannelReviewReviewTypeMappingSearchModel, tvChannelReview);
            }

            model.IsLoggedInAsVendor = await _workContext.GetCurrentVendorAsync() != null;

            return model;
        }

        /// <summary>
        /// Prepare tvChannel review mapping search model
        /// </summary>
        /// <param name="searchModel">TvChannel review mapping search model</param>
        /// <param name="tvChannelReview">TvChannel</param>
        /// <returns>
        /// Задача представляет асинхронную операцию
        /// The task result contains the tvChannel review mapping search model
        /// </returns>
        public virtual async Task<TvChannelReviewReviewTypeMappingSearchModel> PrepareTvChannelReviewReviewTypeMappingSearchModelAsync(TvChannelReviewReviewTypeMappingSearchModel searchModel,
            TvChannelReview tvChannelReview)
        {
            if (searchModel == null)
                throw new ArgumentNullException(nameof(searchModel));

            if (tvChannelReview == null)
                throw new ArgumentNullException(nameof(tvChannelReview));

            searchModel.TvChannelReviewId = tvChannelReview.Id;

            searchModel.IsAnyReviewTypes = (await _reviewTypeService.GetTvChannelReviewReviewTypeMappingsByTvChannelReviewIdAsync(tvChannelReview.Id)).Any();

            //prepare page parameters
            searchModel.SetGridPageSize();

            return searchModel;
        }

        /// <summary>
        /// Prepare paged tvChannel reviews mapping list model
        /// </summary>
        /// <param name="searchModel">TvChannel review and review type mapping search model</param>
        /// <param name="tvChannelReview">TvChannel review</param>
        /// <returns>
        /// Задача представляет асинхронную операцию
        /// The task result contains the tvChannel review and review type mapping list model
        /// </returns>
        public virtual async Task<TvChannelReviewReviewTypeMappingListModel> PrepareTvChannelReviewReviewTypeMappingListModelAsync(TvChannelReviewReviewTypeMappingSearchModel searchModel, TvChannelReview tvChannelReview)
        {
            if (searchModel == null)
                throw new ArgumentNullException(nameof(searchModel));

            if (tvChannelReview == null)
                throw new ArgumentNullException(nameof(tvChannelReview));

            //get tvChannel review and review type mappings
            var tvChannelReviewReviewTypeMappings = (await _reviewTypeService
                .GetTvChannelReviewReviewTypeMappingsByTvChannelReviewIdAsync(tvChannelReview.Id)).ToPagedList(searchModel);

            //prepare grid model
            var model = await new TvChannelReviewReviewTypeMappingListModel().PrepareToGridAsync(searchModel, tvChannelReviewReviewTypeMappings, () =>
            {
                return tvChannelReviewReviewTypeMappings.SelectAwait(async tvChannelReviewReviewTypeMapping =>
                {
                    //fill in model values from the entity
                    var tvChannelReviewReviewTypeMappingModel = tvChannelReviewReviewTypeMapping
                        .ToModel<TvChannelReviewReviewTypeMappingModel>();

                    //fill in additional values (not existing in the entity)
                    var reviewType = await _reviewTypeService.GetReviewTypeByIdAsync(tvChannelReviewReviewTypeMapping.ReviewTypeId);

                    tvChannelReviewReviewTypeMappingModel.Name = await _localizationService.GetLocalizedAsync(reviewType, entity => entity.Name);
                    tvChannelReviewReviewTypeMappingModel.Description = await _localizationService.GetLocalizedAsync(reviewType, entity => entity.Description);
                    tvChannelReviewReviewTypeMappingModel.VisibleToAllUsers = reviewType.VisibleToAllUsers;

                    return tvChannelReviewReviewTypeMappingModel;
                });
            });

            return model;
        }

        #endregion
    }
}