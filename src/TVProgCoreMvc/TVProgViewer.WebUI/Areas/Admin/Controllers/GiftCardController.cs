using System;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using TVProgViewer.Core;
using TVProgViewer.Core.Domain.Directory;
using TVProgViewer.Core.Domain.Localization;
using TVProgViewer.Core.Domain.Orders;
using TVProgViewer.Services.Catalog;
using TVProgViewer.Services.Directory;
using TVProgViewer.Services.Helpers;
using TVProgViewer.Services.Localization;
using TVProgViewer.Services.Logging;
using TVProgViewer.Services.Messages;
using TVProgViewer.Services.Orders;
using TVProgViewer.Services.Security;
using TVProgViewer.WebUI.Areas.Admin.Factories;
using TVProgViewer.WebUI.Areas.Admin.Infrastructure.Mapper.Extensions;
using TVProgViewer.WebUI.Areas.Admin.Models.Orders;
using TVProgViewer.Web.Framework.Controllers;
using TVProgViewer.Web.Framework.Mvc.Filters;
using System.Threading.Tasks;

namespace TVProgViewer.WebUI.Areas.Admin.Controllers
{
    public partial class GiftCardController : BaseAdminController
    {
        #region Fields

        private readonly CurrencySettings _currencySettings;
        private readonly ICurrencyService _currencyService;
        private readonly IUserActivityService _userActivityService;
        private readonly IDateTimeHelper _dateTimeHelper;
        private readonly IGiftCardModelFactory _giftCardModelFactory;
        private readonly IGiftCardService _giftCardService;
        private readonly ILanguageService _languageService;
        private readonly ILocalizationService _localizationService;
        private readonly INotificationService _notificationService;
        private readonly IOrderService _orderService;
        private readonly IPermissionService _permissionService;
        private readonly IPriceFormatter _priceFormatter;
        private readonly IWorkflowMessageService _workflowMessageService;
        private readonly LocalizationSettings _localizationSettings;

        #endregion

        #region Ctor

        public GiftCardController(CurrencySettings currencySettings,
            ICurrencyService currencyService,
            IUserActivityService userActivityService,
            IDateTimeHelper dateTimeHelper,
            IGiftCardModelFactory giftCardModelFactory,
            IGiftCardService giftCardService,
            ILanguageService languageService,
            ILocalizationService localizationService,
            INotificationService notificationService,
            IOrderService orderService,
            IPermissionService permissionService,
            IPriceFormatter priceFormatter,
            IWorkflowMessageService workflowMessageService,
            LocalizationSettings localizationSettings)
        {
            _currencySettings = currencySettings;
            _currencyService = currencyService;
            _userActivityService = userActivityService;
            _dateTimeHelper = dateTimeHelper;
            _giftCardModelFactory = giftCardModelFactory;
            _giftCardService = giftCardService;
            _languageService = languageService;
            _localizationService = localizationService;
            _notificationService = notificationService;
            _orderService = orderService;
            _permissionService = permissionService;
            _priceFormatter = priceFormatter;
            _workflowMessageService = workflowMessageService;
            _localizationSettings = localizationSettings;
        }

        #endregion

        #region Methods

        public virtual IActionResult Index()
        {
            return RedirectToAction("List");
        }

        public virtual async Task<IActionResult> List()
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageGiftCards))
                return AccessDeniedView();

            //prepare model
            var model = await _giftCardModelFactory.PrepareGiftCardSearchModelAsync(new GiftCardSearchModel());

            return View(model);
        }

        [HttpPost]
        public virtual async Task<IActionResult> GiftCardList(GiftCardSearchModel searchModel)
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageGiftCards))
                return await AccessDeniedDataTablesJson();

            //prepare model
            var model = await _giftCardModelFactory.PrepareGiftCardListModelAsync(searchModel);

            return Json(model);
        }

        public virtual async Task<IActionResult> Create()
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageGiftCards))
                return AccessDeniedView();

            //prepare model
            var model = await _giftCardModelFactory.PrepareGiftCardModelAsync(new GiftCardModel(), null);

            return View(model);
        }

        [HttpPost, ParameterBasedOnFormName("save-continue", "continueEditing")]
        public virtual async Task<IActionResult> Create(GiftCardModel model, bool continueEditing)
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageGiftCards))
                return AccessDeniedView();

            if (ModelState.IsValid)
            {
                var giftCard = model.ToEntity<GiftCard>();
                giftCard.CreatedOnUtc = DateTime.UtcNow;
                await _giftCardService.InsertGiftCardAsync(giftCard);

                //activity log
                await _userActivityService.InsertActivityAsync("AddNewGiftCard",
                    string.Format(await _localizationService.GetResourceAsync("ActivityLog.AddNewGiftCard"), giftCard.GiftCardCouponCode), giftCard);

                _notificationService.SuccessNotification(await _localizationService.GetResourceAsync("Admin.GiftCards.Added"));

                return continueEditing ? RedirectToAction("Edit", new { id = giftCard.Id }) : RedirectToAction("List");
            }

            //prepare model
            model = await _giftCardModelFactory.PrepareGiftCardModelAsync(model, null, true);

            //if we got this far, something failed, redisplay form
            return View(model);
        }

        public virtual async Task<IActionResult> Edit(int id)
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageGiftCards))
                return AccessDeniedView();

            //try to get a gift card with the specified id
            var giftCard = await _giftCardService.GetGiftCardByIdAsync(id);
            if (giftCard == null)
                return RedirectToAction("List");

            //prepare model
            var model = await _giftCardModelFactory.PrepareGiftCardModelAsync(null, giftCard);

            return View(model);
        }

        [HttpPost, ParameterBasedOnFormName("save-continue", "continueEditing")]
        [FormValueRequired("save", "save-continue")]
        public virtual async Task<IActionResult> Edit(GiftCardModel model, bool continueEditing)
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageGiftCards))
                return AccessDeniedView();

            //try to get a gift card with the specified id
            var giftCard = await _giftCardService.GetGiftCardByIdAsync(model.Id);
            if (giftCard == null)
                return RedirectToAction("List");

            var order = await _orderService.GetOrderByOrderItemAsync(giftCard.PurchasedWithOrderItemId ?? 0);

            model.PurchasedWithOrderId = order?.Id;
            model.RemainingAmountStr = await _priceFormatter.FormatPriceAsync(await _giftCardService.GetGiftCardRemainingAmountAsync(giftCard), true, false);
            model.AmountStr = await _priceFormatter.FormatPriceAsync(giftCard.Amount, true, false);
            model.CreatedOn = await _dateTimeHelper.ConvertToUserTimeAsync(giftCard.CreatedOnUtc, DateTimeKind.Utc);
            model.PrimaryStoreCurrencyCode = (await _currencyService.GetCurrencyByIdAsync(_currencySettings.PrimaryStoreCurrencyId)).CurrencyCode;
            model.PurchasedWithOrderNumber = order?.CustomOrderNumber;

            if (ModelState.IsValid)
            {
                giftCard = model.ToEntity(giftCard);
                await _giftCardService.UpdateGiftCardAsync(giftCard);

                //activity log
                await _userActivityService.InsertActivityAsync("EditGiftCard",
                    string.Format(await _localizationService.GetResourceAsync("ActivityLog.EditGiftCard"), giftCard.GiftCardCouponCode), giftCard);

                _notificationService.SuccessNotification(await _localizationService.GetResourceAsync("Admin.GiftCards.Updated"));

                if (!continueEditing)
                    return RedirectToAction("List");

                return RedirectToAction("Edit", new { id = giftCard.Id });
            }

            //prepare model
            model = await _giftCardModelFactory.PrepareGiftCardModelAsync(model, giftCard, true);

            //if we got this far, something failed, redisplay form
            return View(model);
        }

        [HttpPost]
        public virtual IActionResult GenerateCouponCode()
        {
            return Json(new { CouponCode = _giftCardService.GenerateGiftCardCode() });
        }

        [HttpPost, ActionName("Edit")]
        [FormValueRequired("notifyRecipient")]
        public virtual async Task<IActionResult> NotifyRecipient(GiftCardModel model)
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageGiftCards))
                return AccessDeniedView();

            //try to get a gift card with the specified id
            var giftCard = await _giftCardService.GetGiftCardByIdAsync(model.Id);
            if (giftCard == null)
                return RedirectToAction("List");

            try
            {
                if (!CommonHelper.IsValidEmail(giftCard.RecipientEmail))
                    throw new TvProgException("Recipient email is not valid");

                if (!CommonHelper.IsValidEmail(giftCard.SenderEmail))
                    throw new TvProgException("Sender email is not valid");

                var languageId = 0;
                var order = await _orderService.GetOrderByOrderItemAsync(giftCard.PurchasedWithOrderItemId ?? 0);

                if (order != null)
                {
                    var userLang = await _languageService.GetLanguageByIdAsync(order.UserLanguageId);
                    if (userLang == null)
                        userLang = (await _languageService.GetAllLanguagesAsync()).FirstOrDefault();
                    if (userLang != null)
                        languageId = userLang.Id;
                }
                else
                {
                    languageId = _localizationSettings.DefaultAdminLanguageId;
                }

                var queuedEmailIds = await _workflowMessageService.SendGiftCardNotificationAsync(giftCard, languageId);
                if (queuedEmailIds.Any())
                {
                    giftCard.IsRecipientNotified = true;
                    await _giftCardService.UpdateGiftCardAsync(giftCard);
                    model.IsRecipientNotified = true;
                }
            }
            catch (Exception exc)
            {
                await _notificationService.ErrorNotificationAsync(exc);
            }

            //prepare model
            model = await _giftCardModelFactory.PrepareGiftCardModelAsync(model, giftCard);

            return View(model);
        }

        [HttpPost]
        public virtual async Task<IActionResult> Delete(int id)
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageGiftCards))
                return AccessDeniedView();

            //try to get a gift card with the specified id
            var giftCard = await _giftCardService.GetGiftCardByIdAsync(id);
            if (giftCard == null)
                return RedirectToAction("List");

            await _giftCardService.DeleteGiftCardAsync(giftCard);

            //activity log
            await _userActivityService.InsertActivityAsync("DeleteGiftCard",
                string.Format(await _localizationService.GetResourceAsync("ActivityLog.DeleteGiftCard"), giftCard.GiftCardCouponCode), giftCard);

            _notificationService.SuccessNotification(await _localizationService.GetResourceAsync("Admin.GiftCards.Deleted"));

            return RedirectToAction("List");
        }

        [HttpPost]
        public virtual async Task<IActionResult> UsageHistoryList(GiftCardUsageHistorySearchModel searchModel)
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageGiftCards))
                return await AccessDeniedDataTablesJson();

            //try to get a gift card with the specified id
            var giftCard = await _giftCardService.GetGiftCardByIdAsync(searchModel.GiftCardId)
                ?? throw new ArgumentException("No gift card found with the specified id");

            //prepare model
            var model = await _giftCardModelFactory.PrepareGiftCardUsageHistoryListModelAsync(searchModel, giftCard);

            return Json(model);
        }

        #endregion
    }
}