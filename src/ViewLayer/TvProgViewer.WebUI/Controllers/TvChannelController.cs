using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using TvProgViewer.Core;
using TvProgViewer.Core.Domain.Catalog;
using TvProgViewer.Core.Domain.Localization;
using TvProgViewer.Core.Domain.Orders;
using TvProgViewer.Core.Domain.Security;
using TvProgViewer.Core.Domain.Shipping;
using TvProgViewer.Core.Events;
using TvProgViewer.Services.Catalog;
using TvProgViewer.Services.Users;
using TvProgViewer.Services.Html;
using TvProgViewer.Services.Localization;
using TvProgViewer.Services.Logging;
using TvProgViewer.Services.Messages;
using TvProgViewer.Services.Orders;
using TvProgViewer.Services.Security;
using TvProgViewer.Services.Seo;
using TvProgViewer.Services.Stores;
using TvProgViewer.WebUI.Factories;
using TvProgViewer.Web.Framework;
using TvProgViewer.Web.Framework.Controllers;
using TvProgViewer.Web.Framework.Mvc.Filters;
using TvProgViewer.Web.Framework.Mvc.Routing;
using TvProgViewer.WebUI.Models.Catalog;
using TvProgViewer.Services.TvProgMain;

namespace TvProgViewer.WebUI.Controllers
{
    [AutoValidateAntiforgeryToken]
    public partial class TvChannelController : BasePublicController
    {
        #region Fields

        private readonly CaptchaSettings _captchaSettings;
        private readonly CatalogSettings _catalogSettings;
        private readonly IAclService _aclService;
        private readonly ICompareTvChannelsService _compareTvChannelsService;
        private readonly IUserActivityService _userActivityService;
        private readonly IUserService _userService;
        private readonly IEventPublisher _eventPublisher;
        private readonly IHtmlFormatter _htmlFormatter;
        private readonly ILocalizationService _localizationService;
        private readonly ITvProgUrlHelper _nopUrlHelper;
        private readonly IOrderService _orderService;
        private readonly IPermissionService _permissionService;
        private readonly ITvChannelAttributeParser _tvchannelAttributeParser;
        private readonly ITvChannelModelFactory _tvchannelModelFactory;
        private readonly ITvChannelService _tvchannelService;
        private readonly IRecentlyViewedTvChannelsService _recentlyViewedTvChannelsService;
        private readonly IReviewTypeService _reviewTypeService;
        private readonly IShoppingCartModelFactory _shoppingCartModelFactory;
        private readonly IShoppingCartService _shoppingCartService;
        private readonly IStoreContext _storeContext;
        private readonly IStoreMappingService _storeMappingService;
        private readonly IUrlRecordService _urlRecordService;
        private readonly IWorkContext _workContext;
        private readonly IWorkflowMessageService _workflowMessageService;
        private readonly LocalizationSettings _localizationSettings;
        private readonly ShoppingCartSettings _shoppingCartSettings;
        private readonly ShippingSettings _shippingSettings;
        private readonly IProgrammeService _programmeService;
        private readonly IChannelService _channelService;

        #endregion

        #region Ctor

        public TvChannelController(CaptchaSettings captchaSettings,
            CatalogSettings catalogSettings,
            IAclService aclService,
            ICompareTvChannelsService compareTvChannelsService,
            IUserActivityService userActivityService,
            IUserService userService,
            IEventPublisher eventPublisher,
            IHtmlFormatter htmlFormatter,
            ILocalizationService localizationService,
            ITvProgUrlHelper nopUrlHelper,
            IOrderService orderService,
            IPermissionService permissionService,
            ITvChannelAttributeParser tvchannelAttributeParser,
            ITvChannelModelFactory tvchannelModelFactory,
            ITvChannelService tvchannelService,
            IRecentlyViewedTvChannelsService recentlyViewedTvChannelsService,
            IReviewTypeService reviewTypeService,
            IShoppingCartModelFactory shoppingCartModelFactory,
            IShoppingCartService shoppingCartService,
            IStoreContext storeContext,
            IStoreMappingService storeMappingService,
            IUrlRecordService urlRecordService,
            IWorkContext workContext,
            IWorkflowMessageService workflowMessageService,
            LocalizationSettings localizationSettings,
            ShoppingCartSettings shoppingCartSettings,
            ShippingSettings shippingSettings,
            IProgrammeService programmeService,
            IChannelService channelService)
        {
            _captchaSettings = captchaSettings;
            _catalogSettings = catalogSettings;
            _aclService = aclService;
            _compareTvChannelsService = compareTvChannelsService;
            _userActivityService = userActivityService;
            _userService = userService;
            _eventPublisher = eventPublisher;
            _htmlFormatter = htmlFormatter;
            _localizationService = localizationService;
            _nopUrlHelper = nopUrlHelper;
            _orderService = orderService;
            _permissionService = permissionService;
            _tvchannelAttributeParser = tvchannelAttributeParser;
            _tvchannelModelFactory = tvchannelModelFactory;
            _tvchannelService = tvchannelService;
            _reviewTypeService = reviewTypeService;
            _recentlyViewedTvChannelsService = recentlyViewedTvChannelsService;
            _shoppingCartModelFactory = shoppingCartModelFactory;
            _shoppingCartService = shoppingCartService;
            _storeContext = storeContext;
            _storeMappingService = storeMappingService;
            _urlRecordService = urlRecordService;
            _workContext = workContext;
            _workflowMessageService = workflowMessageService;
            _localizationSettings = localizationSettings;
            _shoppingCartSettings = shoppingCartSettings;
            _shippingSettings = shippingSettings;
            _programmeService = programmeService;
            _channelService = channelService;
        }

        #endregion

        #region Utilities

        protected virtual async Task ValidateTvChannelReviewAvailabilityAsync(TvChannel tvchannel)
        {
            var user = await _workContext.GetCurrentUserAsync();
            if (await _userService.IsGuestAsync(user) && !_catalogSettings.AllowAnonymousUsersToReviewTvChannel)
                ModelState.AddModelError(string.Empty, await _localizationService.GetResourceAsync("Reviews.OnlyRegisteredUsersCanWriteReviews"));

            if (!_catalogSettings.TvChannelReviewPossibleOnlyAfterPurchasing)
                return;

            var hasCompletedOrders = tvchannel.TvChannelType == TvChannelType.SimpleTvChannel
                ? await HasCompletedOrdersAsync(tvchannel)
                : await (await _tvchannelService.GetAssociatedTvChannelsAsync(tvchannel.Id)).AnyAwaitAsync(HasCompletedOrdersAsync);

            if (!hasCompletedOrders)
                ModelState.AddModelError(string.Empty, await _localizationService.GetResourceAsync("Reviews.TvChannelReviewPossibleOnlyAfterPurchasing"));
        }

        protected virtual async ValueTask<bool> HasCompletedOrdersAsync(TvChannel tvchannel)
        {
            var user = await _workContext.GetCurrentUserAsync();
            return (await _orderService.SearchOrdersAsync(userId: user.Id,
                tvchannelId: tvchannel.Id,
                osIds: new List<int> { (int)OrderStatus.Complete },
                pageSize: 1)).Any();
        }

        #endregion

        #region TvChannel details page

        public virtual async Task<IActionResult> TvChannelDetails(int tvchannelId, int updatecartitemid = 0)
        {
            var tvchannel = await _tvchannelService.GetTvChannelByIdAsync(tvchannelId);
            if (tvchannel == null || tvchannel.Deleted)
                return InvokeHttp404();

            var notAvailable =
                //published?
                (!tvchannel.Published && !_catalogSettings.AllowViewUnpublishedTvChannelPage) ||
                //ACL (access control list) 
                !await _aclService.AuthorizeAsync(tvchannel) ||
                //Store mapping
                !await _storeMappingService.AuthorizeAsync(tvchannel) ||
                //availability dates
                !_tvchannelService.TvChannelIsAvailable(tvchannel);
            //Check whether the current user has a "Manage tvchannels" permission (usually a store owner)
            //We should allows him (her) to use "Preview" functionality
            var hasAdminAccess = await _permissionService.AuthorizeAsync(StandardPermissionProvider.AccessAdminPanel) && await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageTvChannels);
            if (notAvailable && !hasAdminAccess)
                return InvokeHttp404();

            //visible individually?
            if (!tvchannel.VisibleIndividually)
            {
                //is this one an associated tvchannels?
                var parentGroupedTvChannel = await _tvchannelService.GetTvChannelByIdAsync(tvchannel.ParentGroupedTvChannelId);
                if (parentGroupedTvChannel == null)
                    return RedirectToRoute("Homepage");

                var seName = await _urlRecordService.GetSeNameAsync(parentGroupedTvChannel);
                var tvchannelUrl = await _nopUrlHelper.RouteGenericUrlAsync<TvChannel>(new { SeName = seName });
                return LocalRedirectPermanent(tvchannelUrl);
            }

            //update existing shopping cart or wishlist  item?
            ShoppingCartItem updatecartitem = null;
            if (_shoppingCartSettings.AllowCartItemEditing && updatecartitemid > 0)
            {
                var seName = await _urlRecordService.GetSeNameAsync(tvchannel);
                var tvchannelUrl = await _nopUrlHelper.RouteGenericUrlAsync<TvChannel>(new { SeName = seName });
                var store = await _storeContext.GetCurrentStoreAsync();
                var cart = await _shoppingCartService.GetShoppingCartAsync(await _workContext.GetCurrentUserAsync(), storeId: store.Id);
                updatecartitem = cart.FirstOrDefault(x => x.Id == updatecartitemid);
                
                //not found?
                if (updatecartitem == null)
                    return LocalRedirect(tvchannelUrl);

                //is it this tvchannel?
                if (tvchannel.Id != updatecartitem.TvChannelId)
                    return LocalRedirect(tvchannelUrl);
            }

            //save as recently viewed
            await _recentlyViewedTvChannelsService.AddTvChannelToRecentlyViewedListAsync(tvchannel.Id);

            //display "edit" (manage) link
            if (await _permissionService.AuthorizeAsync(StandardPermissionProvider.AccessAdminPanel) &&
                await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageTvChannels))
            {
                //a vendor should have access only to his tvchannels
                var currentVendor = await _workContext.GetCurrentVendorAsync();
                if (currentVendor == null || currentVendor.Id == tvchannel.VendorId)
                {
                    DisplayEditLink(Url.Action("Edit", "TvChannel", new { id = tvchannel.Id, area = AreaNames.Admin }));
                }
            }

            //activity log
            await _userActivityService.InsertActivityAsync("PublicStore.ViewTvChannel",
                string.Format(await _localizationService.GetResourceAsync("ActivityLog.PublicStore.ViewTvChannel"), tvchannel.Name), tvchannel);

            //model
            var model = await _tvchannelModelFactory.PrepareTvChannelDetailsModelAsync(tvchannel, updatecartitem, false);
            //template
            var tvchannelTemplateViewPath = await _tvchannelModelFactory.PrepareTvChannelTemplateViewPathAsync(tvchannel);

            return View(tvchannelTemplateViewPath, model);
        }

        [HttpPost]
        public virtual async Task<IActionResult> EstimateShipping([FromQuery] TvChannelDetailsModel.TvChannelEstimateShippingModel model, IFormCollection form)
        {
            if (model == null)
                model = new TvChannelDetailsModel.TvChannelEstimateShippingModel();

            var errors = new List<string>();

            if (!_shippingSettings.EstimateShippingCityNameEnabled && string.IsNullOrEmpty(model.ZipPostalCode))
                errors.Add(await _localizationService.GetResourceAsync("Shipping.EstimateShipping.ZipPostalCode.Required"));

            if (_shippingSettings.EstimateShippingCityNameEnabled && string.IsNullOrEmpty(model.City))
                errors.Add(await _localizationService.GetResourceAsync("Shipping.EstimateShipping.City.Required"));

            if (model.CountryId == null || model.CountryId == 0)
                errors.Add(await _localizationService.GetResourceAsync("Shipping.EstimateShipping.Country.Required"));

            if (errors.Count > 0)
                return Json(new
                {
                    Success = false,
                    Errors = errors
                });

            var tvchannel = await _tvchannelService.GetTvChannelByIdAsync(model.TvChannelId);
            if (tvchannel == null || tvchannel.Deleted)
            {
                errors.Add(await _localizationService.GetResourceAsync("Shipping.EstimateShippingPopUp.TvChannel.IsNotFound"));
                return Json(new
                {
                    Success = false,
                    Errors = errors
                });
            }

            var store = await _storeContext.GetCurrentStoreAsync();
            var user = await _workContext.GetCurrentUserAsync();

            var wrappedTvChannel = new ShoppingCartItem()
            {
                StoreId = store.Id,
                ShoppingCartTypeId = (int)ShoppingCartType.ShoppingCart,
                UserId = user.Id,
                TvChannelId = tvchannel.Id,
                CreatedOnUtc = DateTime.UtcNow
            };

            var addToCartWarnings = new List<string>();
            //user entered price
            wrappedTvChannel.UserEnteredPrice = await _tvchannelAttributeParser.ParseUserEnteredPriceAsync(tvchannel, form);

            //entered quantity
            wrappedTvChannel.Quantity = _tvchannelAttributeParser.ParseEnteredQuantity(tvchannel, form);

            //tvchannel and gift card attributes
            wrappedTvChannel.AttributesXml = await _tvchannelAttributeParser.ParseTvChannelAttributesAsync(tvchannel, form, addToCartWarnings);

            //rental attributes
            _tvchannelAttributeParser.ParseRentalDates(tvchannel, form, out var rentalStartDate, out var rentalEndDate);
            wrappedTvChannel.RentalStartDateUtc = rentalStartDate;
            wrappedTvChannel.RentalEndDateUtc = rentalEndDate;

            var result = await _shoppingCartModelFactory.PrepareEstimateShippingResultModelAsync(new[] { wrappedTvChannel }, model, false);

            return Json(result);
        }

        //ignore SEO friendly URLs checks
        [CheckLanguageSeoCode(ignore: true)]
        public virtual async Task<IActionResult> GetTvChannelCombinations(int tvchannelId)
        {
            var tvchannel = await _tvchannelService.GetTvChannelByIdAsync(tvchannelId);
            if (tvchannel == null)
                return NotFound();

            var model = await _tvchannelModelFactory.PrepareTvChannelCombinationModelsAsync(tvchannel);
            return Ok(model);
        }

        /// <summary>
        /// Получение программы передач за день
        /// </summary>
        /// <param name="progTypeID">Тип телепрограммы</param>
        /// <param name="cid">Телеканал</param>
        /// <param name="tsDate">На дату</param>
        /// <param name="category">Категория</param>
        public async Task<JsonResult> GetUserProgrammeOfDay(int progTypeID, int cid, string tsDate, string category)
        {
            int? channelId = await _channelService.GetChannelIdByInternalIdAsync(cid);
            return Json(await _programmeService.GetUserProgrammesOfDayListAsync(null, progTypeID, channelId.Value,
                                Convert.ToDateTime(tsDate).AddHours(5).AddMinutes(45),
                                Convert.ToDateTime(tsDate).AddDays(1).AddHours(5).AddMinutes(45), (category != "null") ? category : null));
        }

        #endregion

        #region Recently viewed tvchannels

        public virtual async Task<IActionResult> RecentlyViewedTvChannels()
        {
            if (!_catalogSettings.RecentlyViewedTvChannelsEnabled)
                return Content("");

            var tvchannels = await _recentlyViewedTvChannelsService.GetRecentlyViewedTvChannelsAsync(_catalogSettings.RecentlyViewedTvChannelsNumber);

            var model = new List<TvChannelOverviewModel>();
            model.AddRange(await _tvchannelModelFactory.PrepareTvChannelOverviewModelsAsync(tvchannels));

            return View(model);
        }

        #endregion

        #region TvChannel reviews

        public virtual async Task<IActionResult> TvChannelReviews(int tvchannelId)
        {
            var tvchannel = await _tvchannelService.GetTvChannelByIdAsync(tvchannelId);
            if (tvchannel == null || tvchannel.Deleted || !tvchannel.Published || !tvchannel.AllowUserReviews)
                return RedirectToRoute("Homepage");

            var model = new TvChannelReviewsModel();
            model = await _tvchannelModelFactory.PrepareTvChannelReviewsModelAsync(model, tvchannel);

            await ValidateTvChannelReviewAvailabilityAsync(tvchannel);

            //default value
            model.AddTvChannelReview.Rating = _catalogSettings.DefaultTvChannelRatingValue;

            //default value for all additional review types
            if (model.ReviewTypeList.Count > 0)
                foreach (var additionalTvChannelReview in model.AddAdditionalTvChannelReviewList)
                {
                    additionalTvChannelReview.Rating = additionalTvChannelReview.IsRequired ? _catalogSettings.DefaultTvChannelRatingValue : 0;
                }

            return View(model);
        }

        [HttpPost, ActionName("TvChannelReviews")]
        [FormValueRequired("add-review")]
        [ValidateCaptcha]
        public virtual async Task<IActionResult> TvChannelReviewsAdd(int tvchannelId, TvChannelReviewsModel model, bool captchaValid)
        {
            var tvchannel = await _tvchannelService.GetTvChannelByIdAsync(tvchannelId);
            var currentStore = await _storeContext.GetCurrentStoreAsync();

            if (tvchannel == null || tvchannel.Deleted || !tvchannel.Published || !tvchannel.AllowUserReviews ||
                !await _tvchannelService.CanAddReviewAsync(tvchannel.Id, _catalogSettings.ShowTvChannelReviewsPerStore ? currentStore.Id : 0))
                return RedirectToRoute("Homepage");

            //validate CAPTCHA
            if (_captchaSettings.Enabled && _captchaSettings.ShowOnTvChannelReviewPage && !captchaValid)
            {
                ModelState.AddModelError("", await _localizationService.GetResourceAsync("Common.WrongCaptchaMessage"));
            }

            await ValidateTvChannelReviewAvailabilityAsync(tvchannel);

            if (ModelState.IsValid)
            {
                //save review
                var rating = model.AddTvChannelReview.Rating;
                if (rating < 1 || rating > 5)
                    rating = _catalogSettings.DefaultTvChannelRatingValue;
                var isApproved = !_catalogSettings.TvChannelReviewsMustBeApproved;
                var user = await _workContext.GetCurrentUserAsync();

                var tvchannelReview = new TvChannelReview
                {
                    TvChannelId = tvchannel.Id,
                    UserId = user.Id,
                    Title = model.AddTvChannelReview.Title,
                    ReviewText = model.AddTvChannelReview.ReviewText,
                    Rating = rating,
                    HelpfulYesTotal = 0,
                    HelpfulNoTotal = 0,
                    IsApproved = isApproved,
                    CreatedOnUtc = DateTime.UtcNow,
                    StoreId = currentStore.Id,
                };

                await _tvchannelService.InsertTvChannelReviewAsync(tvchannelReview);

                //add tvchannel review and review type mapping                
                foreach (var additionalReview in model.AddAdditionalTvChannelReviewList)
                {
                    var additionalTvChannelReview = new TvChannelReviewReviewTypeMapping
                    {
                        TvChannelReviewId = tvchannelReview.Id,
                        ReviewTypeId = additionalReview.ReviewTypeId,
                        Rating = additionalReview.Rating
                    };

                    await _reviewTypeService.InsertTvChannelReviewReviewTypeMappingsAsync(additionalTvChannelReview);
                }

                //update tvchannel totals
                await _tvchannelService.UpdateTvChannelReviewTotalsAsync(tvchannel);

                //notify store owner
                if (_catalogSettings.NotifyStoreOwnerAboutNewTvChannelReviews)
                    await _workflowMessageService.SendTvChannelReviewStoreOwnerNotificationMessageAsync(tvchannelReview, _localizationSettings.DefaultAdminLanguageId);

                //activity log
                await _userActivityService.InsertActivityAsync("PublicStore.AddTvChannelReview",
                    string.Format(await _localizationService.GetResourceAsync("ActivityLog.PublicStore.AddTvChannelReview"), tvchannel.Name), tvchannel);

                //raise event
                if (tvchannelReview.IsApproved)
                    await _eventPublisher.PublishAsync(new TvChannelReviewApprovedEvent(tvchannelReview));

                model = await _tvchannelModelFactory.PrepareTvChannelReviewsModelAsync(model, tvchannel);
                model.AddTvChannelReview.Title = null;
                model.AddTvChannelReview.ReviewText = null;

                model.AddTvChannelReview.SuccessfullyAdded = true;
                if (!isApproved)
                    model.AddTvChannelReview.Result = await _localizationService.GetResourceAsync("Reviews.SeeAfterApproving");
                else
                    model.AddTvChannelReview.Result = await _localizationService.GetResourceAsync("Reviews.SuccessfullyAdded");

                return View(model);
            }

            //if we got this far, something failed, redisplay form
            model = await _tvchannelModelFactory.PrepareTvChannelReviewsModelAsync(model, tvchannel);
            return View(model);
        }

        [HttpPost]
        public virtual async Task<IActionResult> SetTvChannelReviewHelpfulness(int tvchannelReviewId, bool washelpful)
        {
            var tvchannelReview = await _tvchannelService.GetTvChannelReviewByIdAsync(tvchannelReviewId);
            if (tvchannelReview == null)
                throw new ArgumentException("No tvchannel review found with the specified id");

            var user = await _workContext.GetCurrentUserAsync();
            if (await _userService.IsGuestAsync(user) && !_catalogSettings.AllowAnonymousUsersToReviewTvChannel)
            {
                return Json(new
                {
                    Result = await _localizationService.GetResourceAsync("Reviews.Helpfulness.OnlyRegistered"),
                    TotalYes = tvchannelReview.HelpfulYesTotal,
                    TotalNo = tvchannelReview.HelpfulNoTotal
                });
            }

            //users aren't allowed to vote for their own reviews
            if (tvchannelReview.UserId == user.Id)
            {
                return Json(new
                {
                    Result = await _localizationService.GetResourceAsync("Reviews.Helpfulness.YourOwnReview"),
                    TotalYes = tvchannelReview.HelpfulYesTotal,
                    TotalNo = tvchannelReview.HelpfulNoTotal
                });
            }

            await _tvchannelService.SetTvChannelReviewHelpfulnessAsync(tvchannelReview, washelpful);

            //new totals
            await _tvchannelService.UpdateTvChannelReviewHelpfulnessTotalsAsync(tvchannelReview);

            return Json(new
            {
                Result = await _localizationService.GetResourceAsync("Reviews.Helpfulness.SuccessfullyVoted"),
                TotalYes = tvchannelReview.HelpfulYesTotal,
                TotalNo = tvchannelReview.HelpfulNoTotal
            });
        }

        public virtual async Task<IActionResult> UserTvChannelReviews(int? pageNumber)
        {
            if (await _userService.IsGuestAsync(await _workContext.GetCurrentUserAsync()))
                return Challenge();

            if (!_catalogSettings.ShowTvChannelReviewsTabOnAccountPage)
            {
                return RedirectToRoute("UserInfo");
            }

            var model = await _tvchannelModelFactory.PrepareUserTvChannelReviewsModelAsync(pageNumber);

            return View(model);
        }

        #endregion

        #region Email a friend

        public virtual async Task<IActionResult> TvChannelEmailAFriend(int tvchannelId)
        {
            var tvchannel = await _tvchannelService.GetTvChannelByIdAsync(tvchannelId);
            if (tvchannel == null || tvchannel.Deleted || !tvchannel.Published || !_catalogSettings.EmailAFriendEnabled)
                return RedirectToRoute("Homepage");

            var model = new TvChannelEmailAFriendModel();
            model = await _tvchannelModelFactory.PrepareTvChannelEmailAFriendModelAsync(model, tvchannel, false);
            return View(model);
        }

        [HttpPost, ActionName("TvChannelEmailAFriend")]
        [FormValueRequired("send-email")]
        [ValidateCaptcha]
        public virtual async Task<IActionResult> TvChannelEmailAFriendSend(TvChannelEmailAFriendModel model, bool captchaValid)
        {
            var tvchannel = await _tvchannelService.GetTvChannelByIdAsync(model.TvChannelId);
            if (tvchannel == null || tvchannel.Deleted || !tvchannel.Published || !_catalogSettings.EmailAFriendEnabled)
                return RedirectToRoute("Homepage");

            //validate CAPTCHA
            if (_captchaSettings.Enabled && _captchaSettings.ShowOnEmailTvChannelToFriendPage && !captchaValid)
            {
                ModelState.AddModelError("", await _localizationService.GetResourceAsync("Common.WrongCaptchaMessage"));
            }

            //check whether the current user is guest and ia allowed to email a friend
            var user = await _workContext.GetCurrentUserAsync();
            if (await _userService.IsGuestAsync(user) && !_catalogSettings.AllowAnonymousUsersToEmailAFriend)
            {
                ModelState.AddModelError("", await _localizationService.GetResourceAsync("TvChannels.EmailAFriend.OnlyRegisteredUsers"));
            }

            if (ModelState.IsValid)
            {
                //email
                await _workflowMessageService.SendTvChannelEmailAFriendMessageAsync(user,
                        (await _workContext.GetWorkingLanguageAsync()).Id, tvchannel,
                        model.YourEmailAddress, model.FriendEmail,
                        _htmlFormatter.FormatText(model.PersonalMessage, false, true, false, false, false, false));

                model = await _tvchannelModelFactory.PrepareTvChannelEmailAFriendModelAsync(model, tvchannel, true);
                model.SuccessfullySent = true;
                model.Result = await _localizationService.GetResourceAsync("TvChannels.EmailAFriend.SuccessfullySent");

                return View(model);
            }

            //If we got this far, something failed, redisplay form
            model = await _tvchannelModelFactory.PrepareTvChannelEmailAFriendModelAsync(model, tvchannel, true);
            return View(model);
        }

        #endregion

        #region Comparing tvchannels

        [HttpPost]
        public virtual async Task<IActionResult> AddTvChannelToCompareList(int tvchannelId)
        {
            var tvchannel = await _tvchannelService.GetTvChannelByIdAsync(tvchannelId);
            if (tvchannel == null || tvchannel.Deleted || !tvchannel.Published)
                return Json(new
                {
                    success = false,
                    message = "No tvchannel found with the specified ID"
                });

            if (!_catalogSettings.CompareTvChannelsEnabled)
                return Json(new
                {
                    success = false,
                    message = "TvChannel comparison is disabled"
                });

            await _compareTvChannelsService.AddTvChannelToCompareListAsync(tvchannelId);

            //activity log
            await _userActivityService.InsertActivityAsync("PublicStore.AddToCompareList",
                string.Format(await _localizationService.GetResourceAsync("ActivityLog.PublicStore.AddToCompareList"), tvchannel.Name), tvchannel);

            return Json(new
            {
                success = true,
                message = string.Format(await _localizationService.GetResourceAsync("TvChannels.TvChannelHasBeenAddedToCompareList.Link"), Url.RouteUrl("CompareTvChannels"))
                //use the code below (commented) if you want a user to be automatically redirected to the compare tvchannels page
                //redirect = Url.RouteUrl("CompareTvChannels"),
            });
        }

        public virtual async Task<IActionResult> RemoveTvChannelFromCompareList(int tvchannelId)
        {
            var tvchannel = await _tvchannelService.GetTvChannelByIdAsync(tvchannelId);
            if (tvchannel == null)
                return RedirectToRoute("Homepage");

            if (!_catalogSettings.CompareTvChannelsEnabled)
                return RedirectToRoute("Homepage");

            await _compareTvChannelsService.RemoveTvChannelFromCompareListAsync(tvchannelId);

            return RedirectToRoute("CompareTvChannels");
        }

        public virtual async Task<IActionResult> CompareTvChannels()
        {
            if (!_catalogSettings.CompareTvChannelsEnabled)
                return RedirectToRoute("Homepage");

            var model = new CompareTvChannelsModel
            {
                IncludeShortDescriptionInCompareTvChannels = _catalogSettings.IncludeShortDescriptionInCompareTvChannels,
                IncludeFullDescriptionInCompareTvChannels = _catalogSettings.IncludeFullDescriptionInCompareTvChannels,
            };

            var tvchannels = await (await _compareTvChannelsService.GetComparedTvChannelsAsync())
            //ACL and store mapping
            .WhereAwait(async p => await _aclService.AuthorizeAsync(p) && await _storeMappingService.AuthorizeAsync(p))
            //availability dates
            .Where(p => _tvchannelService.TvChannelIsAvailable(p)).ToListAsync();

            //prepare model
            var poModels = (await _tvchannelModelFactory.PrepareTvChannelOverviewModelsAsync(tvchannels, prepareSpecificationAttributes: true))
                .ToList();
            foreach(var poModel in poModels)
            {
                model.TvChannels.Add(poModel);
            }

            return View(model);
        }

        public virtual IActionResult ClearCompareList()
        {
            if (!_catalogSettings.CompareTvChannelsEnabled)
                return RedirectToRoute("Homepage");

            _compareTvChannelsService.ClearCompareTvChannels();

            return RedirectToRoute("CompareTvChannels");
        }

        #endregion

    }
}