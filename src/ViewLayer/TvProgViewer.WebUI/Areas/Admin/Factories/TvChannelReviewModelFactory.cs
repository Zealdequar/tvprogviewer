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
    /// Represents the tvchannel review model factory implementation
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
        private readonly ITvChannelService _tvchannelService;
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
            ITvChannelService tvchannelService,
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
            _tvchannelService = tvchannelService;
            _reviewTypeService = reviewTypeService;
            _storeService = storeService;
            _workContext = workContext;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Prepare tvchannel review search model
        /// </summary>
        /// <param name="searchModel">TvChannel review search model</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the tvchannel review search model
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
        /// Prepare paged tvchannel review list model
        /// </summary>
        /// <param name="searchModel">TvChannel review search model</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the tvchannel review list model
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

            //get tvchannel reviews
            var tvchannelReviews = await _tvchannelService.GetAllTvChannelReviewsAsync(showHidden: true,
                userId: 0,
                approved: isApprovedOnly,
                fromUtc: createdOnFromValue,
                toUtc: createdToFromValue,
                message: searchModel.SearchText,
                storeId: searchModel.SearchStoreId,
                tvchannelId: searchModel.SearchTvChannelId,
                vendorId: vendorId,
                pageIndex: searchModel.Page - 1, pageSize: searchModel.PageSize);

            //prepare list model
            var model = await new TvChannelReviewListModel().PrepareToGridAsync(searchModel, tvchannelReviews, () =>
            {
                return tvchannelReviews.SelectAwait(async tvchannelReview =>
                {
                    //fill in model values from the entity
                    var tvchannelReviewModel = tvchannelReview.ToModel<TvChannelReviewModel>();

                    //convert dates to the user time
                    tvchannelReviewModel.CreatedOn = await _dateTimeHelper.ConvertToUserTimeAsync(tvchannelReview.CreatedOnUtc, DateTimeKind.Utc);

                    //fill in additional values (not existing in the entity)
                    tvchannelReviewModel.StoreName = (await _storeService.GetStoreByIdAsync(tvchannelReview.StoreId))?.Name;
                    tvchannelReviewModel.TvChannelName = (await _tvchannelService.GetTvChannelByIdAsync(tvchannelReview.TvChannelId))?.Name;
                    tvchannelReviewModel.UserInfo = (await _userService.GetUserByIdAsync(tvchannelReview.UserId)) is User user && (await _userService.IsRegisteredAsync(user))
                        ? user.Email
                        : await _localizationService.GetResourceAsync("Admin.Users.Guest");

                    tvchannelReviewModel.ReviewText = _htmlFormatter.FormatText(tvchannelReview.ReviewText, false, true, false, false, false, false);
                    tvchannelReviewModel.ReplyText = _htmlFormatter.FormatText(tvchannelReview.ReplyText, false, true, false, false, false, false);

                    return tvchannelReviewModel;
                });
            });

            return model;
        }

        /// <summary>
        /// Prepare tvchannel review model
        /// </summary>
        /// <param name="model">TvChannel review model</param>
        /// <param name="tvchannelReview">TvChannel review</param>
        /// <param name="excludeProperties">Whether to exclude populating of some properties of model</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the tvchannel review model
        /// </returns>
        public virtual async Task<TvChannelReviewModel> PrepareTvChannelReviewModelAsync(TvChannelReviewModel model,
            TvChannelReview tvchannelReview, bool excludeProperties = false)
        {
            if (tvchannelReview != null)
            {
                var showStoreName = (await _storeService.GetAllStoresAsync()).Count > 1;

                //fill in model values from the entity
                model ??= new TvChannelReviewModel
                {
                    Id = tvchannelReview.Id,
                    StoreName = showStoreName ? (await _storeService.GetStoreByIdAsync(tvchannelReview.StoreId))?.Name : string.Empty,
                    TvChannelId = tvchannelReview.TvChannelId,
                    TvChannelName = (await _tvchannelService.GetTvChannelByIdAsync(tvchannelReview.TvChannelId))?.Name,
                    UserId = tvchannelReview.UserId,
                    Rating = tvchannelReview.Rating
                };

                model.ShowStoreName = showStoreName;

                model.UserInfo = await _userService.GetUserByIdAsync(tvchannelReview.UserId) is User user && await _userService.IsRegisteredAsync(user)
                    ? user.Email : await _localizationService.GetResourceAsync("Admin.Users.Guest");

                model.CreatedOn = await _dateTimeHelper.ConvertToUserTimeAsync(tvchannelReview.CreatedOnUtc, DateTimeKind.Utc);

                if (!excludeProperties)
                {
                    model.Title = tvchannelReview.Title;
                    model.ReviewText = tvchannelReview.ReviewText;
                    model.ReplyText = tvchannelReview.ReplyText;
                    model.IsApproved = tvchannelReview.IsApproved;
                }

                //prepare nested search model
                await PrepareTvChannelReviewReviewTypeMappingSearchModelAsync(model.TvChannelReviewReviewTypeMappingSearchModel, tvchannelReview);
            }

            model.IsLoggedInAsVendor = await _workContext.GetCurrentVendorAsync() != null;

            return model;
        }

        /// <summary>
        /// Prepare tvchannel review mapping search model
        /// </summary>
        /// <param name="searchModel">TvChannel review mapping search model</param>
        /// <param name="tvchannelReview">TvChannel</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the tvchannel review mapping search model
        /// </returns>
        public virtual async Task<TvChannelReviewReviewTypeMappingSearchModel> PrepareTvChannelReviewReviewTypeMappingSearchModelAsync(TvChannelReviewReviewTypeMappingSearchModel searchModel,
            TvChannelReview tvchannelReview)
        {
            if (searchModel == null)
                throw new ArgumentNullException(nameof(searchModel));

            if (tvchannelReview == null)
                throw new ArgumentNullException(nameof(tvchannelReview));

            searchModel.TvChannelReviewId = tvchannelReview.Id;

            searchModel.IsAnyReviewTypes = (await _reviewTypeService.GetTvChannelReviewReviewTypeMappingsByTvChannelReviewIdAsync(tvchannelReview.Id)).Any();

            //prepare page parameters
            searchModel.SetGridPageSize();

            return searchModel;
        }

        /// <summary>
        /// Prepare paged tvchannel reviews mapping list model
        /// </summary>
        /// <param name="searchModel">TvChannel review and review type mapping search model</param>
        /// <param name="tvchannelReview">TvChannel review</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the tvchannel review and review type mapping list model
        /// </returns>
        public virtual async Task<TvChannelReviewReviewTypeMappingListModel> PrepareTvChannelReviewReviewTypeMappingListModelAsync(TvChannelReviewReviewTypeMappingSearchModel searchModel, TvChannelReview tvchannelReview)
        {
            if (searchModel == null)
                throw new ArgumentNullException(nameof(searchModel));

            if (tvchannelReview == null)
                throw new ArgumentNullException(nameof(tvchannelReview));

            //get tvchannel review and review type mappings
            var tvchannelReviewReviewTypeMappings = (await _reviewTypeService
                .GetTvChannelReviewReviewTypeMappingsByTvChannelReviewIdAsync(tvchannelReview.Id)).ToPagedList(searchModel);

            //prepare grid model
            var model = await new TvChannelReviewReviewTypeMappingListModel().PrepareToGridAsync(searchModel, tvchannelReviewReviewTypeMappings, () =>
            {
                return tvchannelReviewReviewTypeMappings.SelectAwait(async tvchannelReviewReviewTypeMapping =>
                {
                    //fill in model values from the entity
                    var tvchannelReviewReviewTypeMappingModel = tvchannelReviewReviewTypeMapping
                        .ToModel<TvChannelReviewReviewTypeMappingModel>();

                    //fill in additional values (not existing in the entity)
                    var reviewType = await _reviewTypeService.GetReviewTypeByIdAsync(tvchannelReviewReviewTypeMapping.ReviewTypeId);

                    tvchannelReviewReviewTypeMappingModel.Name = await _localizationService.GetLocalizedAsync(reviewType, entity => entity.Name);
                    tvchannelReviewReviewTypeMappingModel.Description = await _localizationService.GetLocalizedAsync(reviewType, entity => entity.Description);
                    tvchannelReviewReviewTypeMappingModel.VisibleToAllUsers = reviewType.VisibleToAllUsers;

                    return tvchannelReviewReviewTypeMappingModel;
                });
            });

            return model;
        }

        #endregion
    }
}