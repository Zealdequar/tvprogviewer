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

        public virtual IActionResult List()
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageGiftCards))
                return AccessDeniedView();

            //prepare model
            var model = _giftCardModelFactory.PrepareGiftCardSearchModel(new GiftCardSearchModel());

            return View(model);
        }

        [HttpPost]
        public virtual IActionResult GiftCardList(GiftCardSearchModel searchModel)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageGiftCards))
                return AccessDeniedDataTablesJson();

            //prepare model
            var model = _giftCardModelFactory.PrepareGiftCardListModel(searchModel);

            return Json(model);
        }

        public virtual IActionResult Create()
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageGiftCards))
                return AccessDeniedView();

            //prepare model
            var model = _giftCardModelFactory.PrepareGiftCardModel(new GiftCardModel(), null);

            return View(model);
        }

        [HttpPost, ParameterBasedOnFormName("save-continue", "continueEditing")]
        public virtual IActionResult Create(GiftCardModel model, bool continueEditing)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageGiftCards))
                return AccessDeniedView();

            if (ModelState.IsValid)
            {
                var giftCard = model.ToEntity<GiftCard>();
                giftCard.CreatedOnUtc = DateTime.UtcNow;
                _giftCardService.InsertGiftCard(giftCard);

                //activity log
                _userActivityService.InsertActivity("AddNewGiftCard",
                    string.Format(_localizationService.GetResource("ActivityLog.AddNewGiftCard"), giftCard.GiftCardCouponCode), giftCard);

                _notificationService.SuccessNotification(_localizationService.GetResource("Admin.GiftCards.Added"));

                return continueEditing ? RedirectToAction("Edit", new { id = giftCard.Id }) : RedirectToAction("List");
            }

            //prepare model
            model = _giftCardModelFactory.PrepareGiftCardModel(model, null, true);

            //if we got this far, something failed, redisplay form
            return View(model);
        }

        public virtual IActionResult Edit(int id)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageGiftCards))
                return AccessDeniedView();

            //try to get a gift card with the specified id
            var giftCard = _giftCardService.GetGiftCardById(id);
            if (giftCard == null)
                return RedirectToAction("List");

            //prepare model
            var model = _giftCardModelFactory.PrepareGiftCardModel(null, giftCard);

            return View(model);
        }

        [HttpPost, ParameterBasedOnFormName("save-continue", "continueEditing")]
        [FormValueRequired("save", "save-continue")]
        public virtual IActionResult Edit(GiftCardModel model, bool continueEditing)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageGiftCards))
                return AccessDeniedView();

            //try to get a gift card with the specified id
            var giftCard = _giftCardService.GetGiftCardById(model.Id);
            if (giftCard == null)
                return RedirectToAction("List");

            var order = _orderService.GetOrderByOrderItem(giftCard.PurchasedWithOrderItemId ?? 0);

            model.PurchasedWithOrderId = order?.Id;
            model.RemainingAmountStr = _priceFormatter.FormatPrice(_giftCardService.GetGiftCardRemainingAmount(giftCard), true, false);
            model.AmountStr = _priceFormatter.FormatPrice(giftCard.Amount, true, false);
            model.CreatedOn = _dateTimeHelper.ConvertToUserTime(giftCard.CreatedOnUtc, DateTimeKind.Utc);
            model.PrimaryStoreCurrencyCode = _currencyService.GetCurrencyById(_currencySettings.PrimaryStoreCurrencyId).CurrencyCode;
            model.PurchasedWithOrderNumber = order?.CustomOrderNumber;

            if (ModelState.IsValid)
            {
                giftCard = model.ToEntity(giftCard);
                _giftCardService.UpdateGiftCard(giftCard);

                //activity log
                _userActivityService.InsertActivity("EditGiftCard",
                    string.Format(_localizationService.GetResource("ActivityLog.EditGiftCard"), giftCard.GiftCardCouponCode), giftCard);

                _notificationService.SuccessNotification(_localizationService.GetResource("Admin.GiftCards.Updated"));

                if (!continueEditing)
                    return RedirectToAction("List");

                return RedirectToAction("Edit", new { id = giftCard.Id });
            }

            //prepare model
            model = _giftCardModelFactory.PrepareGiftCardModel(model, giftCard, true);

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
        public virtual IActionResult NotifyRecipient(GiftCardModel model)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageGiftCards))
                return AccessDeniedView();

            //try to get a gift card with the specified id
            var giftCard = _giftCardService.GetGiftCardById(model.Id);
            if (giftCard == null)
                return RedirectToAction("List");

            try
            {
                if (!CommonHelper.IsValidEmail(giftCard.RecipientEmail))
                    throw new TvProgException("Recipient email is not valid");

                if (!CommonHelper.IsValidEmail(giftCard.SenderEmail))
                    throw new TvProgException("Sender email is not valid");

                var languageId = 0;
                var order = _orderService.GetOrderByOrderItem(giftCard.PurchasedWithOrderItemId ?? 0);
                
                if (order != null)
                {
                    var userLang = _languageService.GetLanguageById(order.UserLanguageId);
                    if (userLang == null)
                        userLang = _languageService.GetAllLanguages().FirstOrDefault();
                    if (userLang != null)
                        languageId = userLang.Id;
                }
                else
                {
                    languageId = _localizationSettings.DefaultAdminLanguageId;
                }

                var queuedEmailIds = _workflowMessageService.SendGiftCardNotification(giftCard, languageId);
                if (queuedEmailIds.Any())
                {
                    giftCard.IsRecipientNotified = true;
                    _giftCardService.UpdateGiftCard(giftCard);
                    model.IsRecipientNotified = true;
                }
            }
            catch (Exception exc)
            {
                _notificationService.ErrorNotification(exc);
            }

            //prepare model
            model = _giftCardModelFactory.PrepareGiftCardModel(model, giftCard);

            return View(model);
        }

        [HttpPost]
        public virtual IActionResult Delete(int id)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageGiftCards))
                return AccessDeniedView();

            //try to get a gift card with the specified id
            var giftCard = _giftCardService.GetGiftCardById(id);
            if (giftCard == null)
                return RedirectToAction("List");

            _giftCardService.DeleteGiftCard(giftCard);

            //activity log
            _userActivityService.InsertActivity("DeleteGiftCard",
                string.Format(_localizationService.GetResource("ActivityLog.DeleteGiftCard"), giftCard.GiftCardCouponCode), giftCard);

            _notificationService.SuccessNotification(_localizationService.GetResource("Admin.GiftCards.Deleted"));

            return RedirectToAction("List");
        }

        [HttpPost]
        public virtual IActionResult UsageHistoryList(GiftCardUsageHistorySearchModel searchModel)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageGiftCards))
                return AccessDeniedDataTablesJson();

            //try to get a gift card with the specified id
            var giftCard = _giftCardService.GetGiftCardById(searchModel.GiftCardId)
                ?? throw new ArgumentException("No gift card found with the specified id");

            //prepare model
            var model = _giftCardModelFactory.PrepareGiftCardUsageHistoryListModel(searchModel, giftCard);

            return Json(model);
        }

        #endregion
    }
}