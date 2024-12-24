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
using System.Globalization;

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
        private readonly ITvChannelAttributeParser _tvChannelAttributeParser;
        private readonly ITvChannelModelFactory _tvChannelModelFactory;
        private readonly ITvChannelService _tvChannelService;
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
            ITvChannelAttributeParser tvChannelAttributeParser,
            ITvChannelModelFactory tvChannelModelFactory,
            ITvChannelService tvChannelService,
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
            _tvChannelAttributeParser = tvChannelAttributeParser;
            _tvChannelModelFactory = tvChannelModelFactory;
            _tvChannelService = tvChannelService;
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

        protected virtual async Task ValidateTvChannelReviewAvailabilityAsync(TvChannel tvChannel)
        {
            var user = await _workContext.GetCurrentUserAsync();
            if (await _userService.IsGuestAsync(user) && !_catalogSettings.AllowAnonymousUsersToReviewTvChannel)
                ModelState.AddModelError(string.Empty, await _localizationService.GetResourceAsync("Reviews.OnlyRegisteredUsersCanWriteReviews"));

            if (!_catalogSettings.TvChannelReviewPossibleOnlyAfterPurchasing)
                return;

            var hasCompletedOrders = tvChannel.TvChannelType == TvChannelType.SimpleTvChannel
                ? await HasCompletedOrdersAsync(tvChannel)
                : await (await _tvChannelService.GetAssociatedTvChannelsAsync(tvChannel.Id)).AnyAwaitAsync(HasCompletedOrdersAsync);

            if (!hasCompletedOrders)
                ModelState.AddModelError(string.Empty, await _localizationService.GetResourceAsync("Reviews.TvChannelReviewPossibleOnlyAfterPurchasing"));
        }

        protected virtual async ValueTask<bool> HasCompletedOrdersAsync(TvChannel tvChannel)
        {
            var user = await _workContext.GetCurrentUserAsync();
            return (await _orderService.SearchOrdersAsync(userId: user.Id,
                tvChannelId: tvChannel.Id,
                osIds: new List<int> { (int)OrderStatus.Complete },
                pageSize: 1)).Any();
        }

        #endregion

        #region TvChannel details page

        public virtual async Task<IActionResult> TvChannelDetails(int tvChannelId, int updatecartitemid = 0)
        {
            var tvChannel = await _tvChannelService.GetTvChannelByIdAsync(tvChannelId);
            if (tvChannel == null || tvChannel.Deleted)
                return InvokeHttp404();

            var notAvailable =
                //published?
                (!tvChannel.Published && !_catalogSettings.AllowViewUnpublishedTvChannelPage) ||
                //ACL (access control list) 
                !await _aclService.AuthorizeAsync(tvChannel) ||
                //Store mapping
                !await _storeMappingService.AuthorizeAsync(tvChannel) ||
                //availability dates
                !_tvChannelService.TvChannelIsAvailable(tvChannel);
            //Check whether the current user has a "Manage tvChannels" permission (usually a store owner)
            //We should allows him (her) to use "Preview" functionality
            var hasAdminAccess = await _permissionService.AuthorizeAsync(StandardPermissionProvider.AccessAdminPanel) && await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageTvChannels);
            if (notAvailable && !hasAdminAccess)
                return InvokeHttp404();

            //visible individually?
            if (!tvChannel.VisibleIndividually)
            {
                //is this one an associated tvChannels?
                var parentGroupedTvChannel = await _tvChannelService.GetTvChannelByIdAsync(tvChannel.ParentGroupedTvChannelId);
                if (parentGroupedTvChannel == null)
                    return RedirectToRoute("Homepage");

                var seName = await _urlRecordService.GetSeNameAsync(parentGroupedTvChannel);
                var tvChannelUrl = await _nopUrlHelper.RouteGenericUrlAsync<TvChannel>(new { SeName = seName });
                return LocalRedirectPermanent(tvChannelUrl);
            }

            //update existing shopping cart or wishlist  item?
            ShoppingCartItem updatecartitem = null;
            if (_shoppingCartSettings.AllowCartItemEditing && updatecartitemid > 0)
            {
                var seName = await _urlRecordService.GetSeNameAsync(tvChannel);
                var tvChannelUrl = await _nopUrlHelper.RouteGenericUrlAsync<TvChannel>(new { SeName = seName });
                var store = await _storeContext.GetCurrentStoreAsync();
                var cart = await _shoppingCartService.GetShoppingCartAsync(await _workContext.GetCurrentUserAsync(), storeId: store.Id);
                updatecartitem = cart.FirstOrDefault(x => x.Id == updatecartitemid);
                
                //not found?
                if (updatecartitem == null)
                    return LocalRedirect(tvChannelUrl);

                //is it this tvChannel?
                if (tvChannel.Id != updatecartitem.TvChannelId)
                    return LocalRedirect(tvChannelUrl);
            }

            //save as recently viewed
            await _recentlyViewedTvChannelsService.AddTvChannelToRecentlyViewedListAsync(tvChannel.Id);

            //display "edit" (manage) link
            if (await _permissionService.AuthorizeAsync(StandardPermissionProvider.AccessAdminPanel) &&
                await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageTvChannels))
            {
                //a vendor should have access only to his tvChannels
                var currentVendor = await _workContext.GetCurrentVendorAsync();
                if (currentVendor == null || currentVendor.Id == tvChannel.VendorId)
                {
                    DisplayEditLink(Url.Action("Edit", "TvChannel", new { id = tvChannel.Id, area = AreaNames.Admin }));
                }
            }

            //activity log
            await _userActivityService.InsertActivityAsync("PublicStore.ViewTvChannel",
                string.Format(await _localizationService.GetResourceAsync("ActivityLog.PublicStore.ViewTvChannel"), tvChannel.Name), tvChannel);

            //model
            var model = await _tvChannelModelFactory.PrepareTvChannelDetailsModelAsync(tvChannel, updatecartitem, false);
            //template
            var tvChannelTemplateViewPath = await _tvChannelModelFactory.PrepareTvChannelTemplateViewPathAsync(tvChannel);

            return View(tvChannelTemplateViewPath, model);
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

            var tvChannel = await _tvChannelService.GetTvChannelByIdAsync(model.TvChannelId);
            if (tvChannel == null || tvChannel.Deleted)
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
                TvChannelId = tvChannel.Id,
                CreatedOnUtc = DateTime.UtcNow
            };

            var addToCartWarnings = new List<string>();
            //user entered price
            wrappedTvChannel.UserEnteredPrice = await _tvChannelAttributeParser.ParseUserEnteredPriceAsync(tvChannel, form);

            //entered quantity
            wrappedTvChannel.Quantity = _tvChannelAttributeParser.ParseEnteredQuantity(tvChannel, form);

            //tvChannel and gift card attributes
            wrappedTvChannel.AttributesXml = await _tvChannelAttributeParser.ParseTvChannelAttributesAsync(tvChannel, form, addToCartWarnings);

            //rental attributes
            _tvChannelAttributeParser.ParseRentalDates(tvChannel, form, out var rentalStartDate, out var rentalEndDate);
            wrappedTvChannel.RentalStartDateUtc = rentalStartDate;
            wrappedTvChannel.RentalEndDateUtc = rentalEndDate;

            var result = await _shoppingCartModelFactory.PrepareEstimateShippingResultModelAsync(new[] { wrappedTvChannel }, model, false);

            return Json(result);
        }

        //ignore SEO friendly URLs checks
        [CheckLanguageSeoCode(ignore: true)]
        public virtual async Task<IActionResult> GetTvChannelCombinations(int tvChannelId)
        {
            var tvChannel = await _tvChannelService.GetTvChannelByIdAsync(tvChannelId);
            if (tvChannel == null)
                return NotFound();

            var model = await _tvChannelModelFactory.PrepareTvChannelCombinationModelsAsync(tvChannel);
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
            IFormatProvider provider = CultureInfo.GetCultureInfo("ru-RU");
            DateTime dtStart = Convert.ToDateTime(tsDate, provider).AddHours(5).AddMinutes(45);
            DateTime dtEnd = Convert.ToDateTime(tsDate, provider).AddDays(1).AddHours(5).AddMinutes(45);
            if (User.Identity.IsAuthenticated)
            {
                if (await _workContext.GetCurrentUserFullYearsOldAsync() >= 18)
                {
                    return Json(await _programmeService.GetUserAdultProgrammesOfDayListAsync(progTypeID, channelId.Value, dtStart, dtEnd,
                                   (category != "Все категории") ? category : null));
                }
                else
                {
                    return Json(await _programmeService.GetUserProgrammesOfDayListAsync(progTypeID, channelId.Value, dtStart, dtEnd,
                                   (category != "Все категории") ? category : null));
                }
            }
            else
            {
                return Json(await _programmeService.GetUserProgrammesOfDayListAsync(progTypeID, channelId.Value, dtStart, dtEnd,
                (category != "Все категории") ? category : null));
            }
        }

        #endregion

        #region Recently viewed tvChannels

        public virtual async Task<IActionResult> RecentlyViewedTvChannels()
        {
            if (!_catalogSettings.RecentlyViewedTvChannelsEnabled)
                return Content("");

            var tvChannels = await _recentlyViewedTvChannelsService.GetRecentlyViewedTvChannelsAsync(_catalogSettings.RecentlyViewedTvChannelsNumber);

            var model = new List<TvChannelOverviewModel>();
            model.AddRange(await _tvChannelModelFactory.PrepareTvChannelOverviewModelsAsync(tvChannels));

            return View(model);
        }

        #endregion

        #region TvChannel reviews

        public virtual async Task<IActionResult> TvChannelReviews(int tvChannelId)
        {
            var tvChannel = await _tvChannelService.GetTvChannelByIdAsync(tvChannelId);
            if (tvChannel == null || tvChannel.Deleted || !tvChannel.Published || !tvChannel.AllowUserReviews)
                return RedirectToRoute("Homepage");

            var model = new TvChannelReviewsModel();
            model = await _tvChannelModelFactory.PrepareTvChannelReviewsModelAsync(model, tvChannel);

            await ValidateTvChannelReviewAvailabilityAsync(tvChannel);

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
        public virtual async Task<IActionResult> TvChannelReviewsAdd(int tvChannelId, TvChannelReviewsModel model, bool captchaValid)
        {
            var tvChannel = await _tvChannelService.GetTvChannelByIdAsync(tvChannelId);
            var currentStore = await _storeContext.GetCurrentStoreAsync();

            if (tvChannel == null || tvChannel.Deleted || !tvChannel.Published || !tvChannel.AllowUserReviews ||
                !await _tvChannelService.CanAddReviewAsync(tvChannel.Id, _catalogSettings.ShowTvChannelReviewsPerStore ? currentStore.Id : 0))
                return RedirectToRoute("Homepage");

            //validate CAPTCHA
            if (_captchaSettings.Enabled && _captchaSettings.ShowOnTvChannelReviewPage && !captchaValid)
            {
                ModelState.AddModelError("", await _localizationService.GetResourceAsync("Common.WrongCaptchaMessage"));
            }

            await ValidateTvChannelReviewAvailabilityAsync(tvChannel);

            if (ModelState.IsValid)
            {
                //save review
                var rating = model.AddTvChannelReview.Rating;
                if (rating < 1 || rating > 5)
                    rating = _catalogSettings.DefaultTvChannelRatingValue;
                var isApproved = !_catalogSettings.TvChannelReviewsMustBeApproved;
                var user = await _workContext.GetCurrentUserAsync();

                var tvChannelReview = new TvChannelReview
                {
                    TvChannelId = tvChannel.Id,
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

                await _tvChannelService.InsertTvChannelReviewAsync(tvChannelReview);

                //add tvChannel review and review type mapping                
                foreach (var additionalReview in model.AddAdditionalTvChannelReviewList)
                {
                    var additionalTvChannelReview = new TvChannelReviewReviewTypeMapping
                    {
                        TvChannelReviewId = tvChannelReview.Id,
                        ReviewTypeId = additionalReview.ReviewTypeId,
                        Rating = additionalReview.Rating
                    };

                    await _reviewTypeService.InsertTvChannelReviewReviewTypeMappingsAsync(additionalTvChannelReview);
                }

                //update tvChannel totals
                await _tvChannelService.UpdateTvChannelReviewTotalsAsync(tvChannel);

                //notify store owner
                if (_catalogSettings.NotifyStoreOwnerAboutNewTvChannelReviews)
                    await _workflowMessageService.SendTvChannelReviewStoreOwnerNotificationMessageAsync(tvChannelReview, _localizationSettings.DefaultAdminLanguageId);

                //activity log
                await _userActivityService.InsertActivityAsync("PublicStore.AddTvChannelReview",
                    string.Format(await _localizationService.GetResourceAsync("ActivityLog.PublicStore.AddTvChannelReview"), tvChannel.Name), tvChannel);

                //raise event
                if (tvChannelReview.IsApproved)
                    await _eventPublisher.PublishAsync(new TvChannelReviewApprovedEvent(tvChannelReview));

                model = await _tvChannelModelFactory.PrepareTvChannelReviewsModelAsync(model, tvChannel);
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
            model = await _tvChannelModelFactory.PrepareTvChannelReviewsModelAsync(model, tvChannel);
            return View(model);
        }

        [HttpPost]
        public virtual async Task<IActionResult> SetTvChannelReviewHelpfulness(int tvChannelReviewId, bool washelpful)
        {
            var tvChannelReview = await _tvChannelService.GetTvChannelReviewByIdAsync(tvChannelReviewId);
            if (tvChannelReview == null)
                throw new ArgumentException("No tvChannel review found with the specified id");

            var user = await _workContext.GetCurrentUserAsync();
            if (await _userService.IsGuestAsync(user) && !_catalogSettings.AllowAnonymousUsersToReviewTvChannel)
            {
                return Json(new
                {
                    Result = await _localizationService.GetResourceAsync("Reviews.Helpfulness.OnlyRegistered"),
                    TotalYes = tvChannelReview.HelpfulYesTotal,
                    TotalNo = tvChannelReview.HelpfulNoTotal
                });
            }

            //users aren't allowed to vote for their own reviews
            if (tvChannelReview.UserId == user.Id)
            {
                return Json(new
                {
                    Result = await _localizationService.GetResourceAsync("Reviews.Helpfulness.YourOwnReview"),
                    TotalYes = tvChannelReview.HelpfulYesTotal,
                    TotalNo = tvChannelReview.HelpfulNoTotal
                });
            }

            await _tvChannelService.SetTvChannelReviewHelpfulnessAsync(tvChannelReview, washelpful);

            //new totals
            await _tvChannelService.UpdateTvChannelReviewHelpfulnessTotalsAsync(tvChannelReview);

            return Json(new
            {
                Result = await _localizationService.GetResourceAsync("Reviews.Helpfulness.SuccessfullyVoted"),
                TotalYes = tvChannelReview.HelpfulYesTotal,
                TotalNo = tvChannelReview.HelpfulNoTotal
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

            var model = await _tvChannelModelFactory.PrepareUserTvChannelReviewsModelAsync(pageNumber);

            return View(model);
        }

        #endregion

        #region Email a friend

        public virtual async Task<IActionResult> TvChannelEmailAFriend(int tvChannelId)
        {
            var tvChannel = await _tvChannelService.GetTvChannelByIdAsync(tvChannelId);
            if (tvChannel == null || tvChannel.Deleted || !tvChannel.Published || !_catalogSettings.EmailAFriendEnabled)
                return RedirectToRoute("Homepage");

            var model = new TvChannelEmailAFriendModel();
            model = await _tvChannelModelFactory.PrepareTvChannelEmailAFriendModelAsync(model, tvChannel, false);
            return View(model);
        }

        [HttpPost, ActionName("TvChannelEmailAFriend")]
        [FormValueRequired("send-email")]
        [ValidateCaptcha]
        public virtual async Task<IActionResult> TvChannelEmailAFriendSend(TvChannelEmailAFriendModel model, bool captchaValid)
        {
            var tvChannel = await _tvChannelService.GetTvChannelByIdAsync(model.TvChannelId);
            if (tvChannel == null || tvChannel.Deleted || !tvChannel.Published || !_catalogSettings.EmailAFriendEnabled)
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
                        (await _workContext.GetWorkingLanguageAsync()).Id, tvChannel,
                        model.YourEmailAddress, model.FriendEmail,
                        _htmlFormatter.FormatText(model.PersonalMessage, false, true, false, false, false, false));

                model = await _tvChannelModelFactory.PrepareTvChannelEmailAFriendModelAsync(model, tvChannel, true);
                model.SuccessfullySent = true;
                model.Result = await _localizationService.GetResourceAsync("TvChannels.EmailAFriend.SuccessfullySent");

                return View(model);
            }

            //If we got this far, something failed, redisplay form
            model = await _tvChannelModelFactory.PrepareTvChannelEmailAFriendModelAsync(model, tvChannel, true);
            return View(model);
        }

        #endregion

        #region Comparing tvChannels

        [HttpPost]
        public virtual async Task<IActionResult> AddTvChannelToCompareList(int tvChannelId)
        {
            var tvChannel = await _tvChannelService.GetTvChannelByIdAsync(tvChannelId);
            if (tvChannel == null || tvChannel.Deleted || !tvChannel.Published)
                return Json(new
                {
                    success = false,
                    message = "No tvChannel found with the specified ID"
                });

            if (!_catalogSettings.CompareTvChannelsEnabled)
                return Json(new
                {
                    success = false,
                    message = "TvChannel comparison is disabled"
                });

            await _compareTvChannelsService.AddTvChannelToCompareListAsync(tvChannelId);

            //activity log
            await _userActivityService.InsertActivityAsync("PublicStore.AddToCompareList",
                string.Format(await _localizationService.GetResourceAsync("ActivityLog.PublicStore.AddToCompareList"), tvChannel.Name), tvChannel);

            return Json(new
            {
                success = true,
                message = string.Format(await _localizationService.GetResourceAsync("TvChannels.TvChannelHasBeenAddedToCompareList.Link"), Url.RouteUrl("CompareTvChannels"))
                //use the code below (commented) if you want a user to be automatically redirected to the compare tvChannels page
                //redirect = Url.RouteUrl("CompareTvChannels"),
            });
        }

        public virtual async Task<IActionResult> RemoveTvChannelFromCompareList(int tvChannelId)
        {
            var tvChannel = await _tvChannelService.GetTvChannelByIdAsync(tvChannelId);
            if (tvChannel == null)
                return RedirectToRoute("Homepage");

            if (!_catalogSettings.CompareTvChannelsEnabled)
                return RedirectToRoute("Homepage");

            await _compareTvChannelsService.RemoveTvChannelFromCompareListAsync(tvChannelId);

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

            var tvChannels = await (await _compareTvChannelsService.GetComparedTvChannelsAsync())
            //ACL and store mapping
            .WhereAwait(async p => await _aclService.AuthorizeAsync(p) && await _storeMappingService.AuthorizeAsync(p))
            //availability dates
            .Where(p => _tvChannelService.TvChannelIsAvailable(p)).ToListAsync();

            //prepare model
            var poModels = (await _tvChannelModelFactory.PrepareTvChannelOverviewModelsAsync(tvChannels, prepareSpecificationAttributes: true))
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