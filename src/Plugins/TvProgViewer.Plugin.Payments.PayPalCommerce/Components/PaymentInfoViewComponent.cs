using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using TvProgViewer.Core.Domain.Orders;
using TvProgViewer.Core.Http.Extensions;
using TvProgViewer.Plugin.Payments.PayPalViewer.Models;
using TvProgViewer.Plugin.Payments.PayPalViewer.Services;
using TvProgViewer.Services.Localization;
using TvProgViewer.Services.Messages;
using TvProgViewer.Services.Payments;
using TvProgViewer.Web.Framework.Components;

namespace TvProgViewer.Plugin.Payments.PayPalViewer.Components
{
    /// <summary>
    /// Represents the view component to display payment info in public store
    /// </summary>
    public class PaymentInfoViewComponent : TvProgViewComponent
    {
        #region Fields

        private readonly ILocalizationService _localizationService;
        private readonly INotificationService _notificationService;
        private readonly IPaymentService _paymentService;
        private readonly OrderSettings _orderSettings;
        private readonly PayPalViewerSettings _settings;
        private readonly ServiceManager _serviceManager;

        #endregion

        #region Ctor

        public PaymentInfoViewComponent(ILocalizationService localizationService,
            INotificationService notificationService,
            IPaymentService paymentService,
            OrderSettings orderSettings,
            PayPalViewerSettings settings,
            ServiceManager serviceManager)
        {
            _localizationService = localizationService;
            _notificationService = notificationService;
            _paymentService = paymentService;
            _orderSettings = orderSettings;
            _settings = settings;
            _serviceManager = serviceManager;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Invoke view component
        /// </summary>
        /// <param name="widgetZone">Widget zone name</param>
        /// <param name="additionalData">Additional data</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the view component result
        /// </returns>
        public async Task<IViewComponentResult> InvokeAsync(string widgetZone, object additionalData)
        {
            var model = new PaymentInfoModel();

            //prepare order GUID
            var paymentRequest = new ProcessPaymentRequest();
            _paymentService.GenerateOrderGuid(paymentRequest);

            //try to create an order
            var (order, error) = await _serviceManager.CreateOrderAsync(_settings, paymentRequest.OrderGuid);
            if (order != null)
            {
                model.OrderId = order.Id;
                model.OrderTotal = order.PurchaseUnits.FirstOrDefault().AmountWithBreakdown.Value;

                //save order details for future using
                var key = await _localizationService.GetResourceAsync("Plugins.Payments.PayPalViewer.OrderId");
                paymentRequest.CustomValues.Add(key, order.Id);
            }
            else if (!string.IsNullOrEmpty(error))
            {
                model.Errors = error;
                if (_orderSettings.OnePageCheckoutEnabled)
                    ModelState.AddModelError(string.Empty, error);
                else
                    _notificationService.ErrorNotification(error);
            }

            HttpContext.Session.Set(PayPalViewerDefaults.PaymentRequestSessionKey, paymentRequest);

            return View("~/Plugins/Payments.PayPalViewer/Views/PaymentInfo.cshtml", model);
        }

        #endregion
    }
}