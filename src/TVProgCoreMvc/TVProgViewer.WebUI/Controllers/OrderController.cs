using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using TVProgViewer.Core;
using TVProgViewer.Core.Domain.Users;
using TVProgViewer.Core.Domain.Orders;
using TVProgViewer.Services.Common;
using TVProgViewer.Services.Users;
using TVProgViewer.Services.Orders;
using TVProgViewer.Services.Payments;
using TVProgViewer.Services.Shipping;
using TVProgViewer.WebUI.Factories;
using TVProgViewer.Web.Framework.Controllers;
using TVProgViewer.Web.Framework.Mvc.Filters;
using TVProgViewer.Web.Framework.Security;
using System.Threading.Tasks;

namespace TVProgViewer.WebUI.Controllers
{
    public partial class OrderController : BasePublicController
    {
        #region Fields

        private readonly IUserService _userService;
        private readonly IOrderModelFactory _orderModelFactory;
        private readonly IOrderProcessingService _orderProcessingService;
        private readonly IOrderService _orderService;
        private readonly IPaymentService _paymentService;
        private readonly IPdfService _pdfService;
        private readonly IShipmentService _shipmentService;
        private readonly IWebHelper _webHelper;
        private readonly IWorkContext _workContext;
        private readonly RewardPointsSettings _rewardPointsSettings;

        #endregion

        #region Ctor

        public OrderController(IUserService userService,
            IOrderModelFactory orderModelFactory,
            IOrderProcessingService orderProcessingService,
            IOrderService orderService,
            IPaymentService paymentService,
            IPdfService pdfService,
            IShipmentService shipmentService,
            IWebHelper webHelper,
            IWorkContext workContext,
            RewardPointsSettings rewardPointsSettings)
        {
            _userService = userService;
            _orderModelFactory = orderModelFactory;
            _orderProcessingService = orderProcessingService;
            _orderService = orderService;
            _paymentService = paymentService;
            _pdfService = pdfService;
            _shipmentService = shipmentService;
            _webHelper = webHelper;
            _workContext = workContext;
            _rewardPointsSettings = rewardPointsSettings;
        }

        #endregion

        #region Methods

        //My account / Orders
        public virtual async Task<IActionResult> UserOrders()
        {
            if (!await _userService.IsRegisteredAsync(await _workContext.GetCurrentUserAsync()))
                return Challenge();

            var model = await _orderModelFactory.PrepareUserOrderListModelAsync();
            return View(model);
        }

        //My account / Orders / Cancel recurring order
        [HttpPost, ActionName("UserOrders")]
        [AutoValidateAntiforgeryToken]
        [FormValueRequired(FormValueRequirement.StartsWith, "cancelRecurringPayment")]
        public virtual async Task<IActionResult> CancelRecurringPayment(IFormCollection form)
        {
            if (!await _userService.IsRegisteredAsync(await _workContext.GetCurrentUserAsync()))
                return Challenge();

            //get recurring payment identifier
            var recurringPaymentId = 0;
            foreach (var formValue in form.Keys)
                if (formValue.StartsWith("cancelRecurringPayment", StringComparison.InvariantCultureIgnoreCase))
                    recurringPaymentId = Convert.ToInt32(formValue["cancelRecurringPayment".Length..]);

            var recurringPayment = await _orderService.GetRecurringPaymentByIdAsync(recurringPaymentId);
            if (recurringPayment == null)
            {
                return RedirectToRoute("UserOrders");
            }

            if (await _orderProcessingService.CanCancelRecurringPaymentAsync(await _workContext.GetCurrentUserAsync(), recurringPayment))
            {
                var errors = await _orderProcessingService.CancelRecurringPaymentAsync(recurringPayment);

                var model = await _orderModelFactory.PrepareUserOrderListModelAsync();
                model.RecurringPaymentErrors = errors;

                return View(model);
            }

            return RedirectToRoute("UserOrders");
        }

        //My account / Orders / Retry last recurring order
        [HttpPost, ActionName("UserOrders")]
        [AutoValidateAntiforgeryToken]
        [FormValueRequired(FormValueRequirement.StartsWith, "retryLastPayment")]
        public virtual async Task<IActionResult> RetryLastRecurringPayment(IFormCollection form)
        {
            if (!await _userService.IsRegisteredAsync(await _workContext.GetCurrentUserAsync()))
                return Challenge();

            //get recurring payment identifier
            var recurringPaymentId = 0;
            if (!form.Keys.Any(formValue => formValue.StartsWith("retryLastPayment", StringComparison.InvariantCultureIgnoreCase) &&
                int.TryParse(formValue[(formValue.IndexOf('_') + 1)..], out recurringPaymentId)))
            {
                return RedirectToRoute("UserOrders");
            }

            var recurringPayment = await _orderService.GetRecurringPaymentByIdAsync(recurringPaymentId);
            if (recurringPayment == null)
                return RedirectToRoute("UserOrders");

            if (!await _orderProcessingService.CanRetryLastRecurringPaymentAsync(await _workContext.GetCurrentUserAsync(), recurringPayment))
                return RedirectToRoute("UserOrders");

            var errors = await _orderProcessingService.ProcessNextRecurringPaymentAsync(recurringPayment);
            var model = await _orderModelFactory.PrepareUserOrderListModelAsync();
            model.RecurringPaymentErrors = errors.ToList();

            return View(model);
        }

        //My account / Reward points
        public virtual async Task<IActionResult> UserRewardPoints(int? pageNumber)
        {
            if (!await _userService.IsRegisteredAsync(await _workContext.GetCurrentUserAsync()))
                return Challenge();

            if (!_rewardPointsSettings.Enabled)
                return RedirectToRoute("UserInfo");

            var model = await _orderModelFactory.PrepareUserRewardPointsAsync(pageNumber);
            return View(model);
        }

        //My account / Order details page
        public virtual async Task<IActionResult> Details(int orderId)
        {
            var order = await _orderService.GetOrderByIdAsync(orderId);
            if (order == null || order.Deleted || (await _workContext.GetCurrentUserAsync()).Id != order.UserId)
                return Challenge();

            var model = await _orderModelFactory.PrepareOrderDetailsModelAsync(order);
            return View(model);
        }

        //My account / Order details page / Print
        public virtual async Task<IActionResult> PrintOrderDetails(int orderId)
        {
            var order = await _orderService.GetOrderByIdAsync(orderId);
            if (order == null || order.Deleted || (await _workContext.GetCurrentUserAsync()).Id != order.UserId)
                return Challenge();

            var model = await _orderModelFactory.PrepareOrderDetailsModelAsync(order);
            model.PrintMode = true;

            return View("Details", model);
        }

        //My account / Order details page / PDF invoice
        public virtual async Task<IActionResult> GetPdfInvoice(int orderId)
        {
            var order = await _orderService.GetOrderByIdAsync(orderId);
            if (order == null || order.Deleted || (await _workContext.GetCurrentUserAsync()).Id != order.UserId)
                return Challenge();

            var orders = new List<Order>();
            orders.Add(order);
            byte[] bytes;
            await using (var stream = new MemoryStream())
            {
                await _pdfService.PrintOrdersToPdfAsync(stream, orders, (await _workContext.GetWorkingLanguageAsync()).Id);
                bytes = stream.ToArray();
            }
            return File(bytes, MimeTypes.ApplicationPdf, $"order_{order.Id}.pdf");
        }

        //My account / Order details page / re-order
        public virtual async Task<IActionResult> ReOrder(int orderId)
        {
            var order = await _orderService.GetOrderByIdAsync(orderId);
            if (order == null || order.Deleted || (await _workContext.GetCurrentUserAsync()).Id != order.UserId)
                return Challenge();

            await _orderProcessingService.ReOrderAsync(order);
            return RedirectToRoute("ShoppingCart");
        }

        //My account / Order details page / Complete payment
        [HttpPost, ActionName("Details")]
        [AutoValidateAntiforgeryToken]
        [FormValueRequired("repost-payment")]
        public virtual async Task<IActionResult> RePostPayment(int orderId)
        {
            var order = await _orderService.GetOrderByIdAsync(orderId);
            if (order == null || order.Deleted || (await _workContext.GetCurrentUserAsync()).Id != order.UserId)
                return Challenge();

            if (!await _paymentService.CanRePostProcessPaymentAsync(order))
                return RedirectToRoute("OrderDetails", new { orderId = orderId });

            var postProcessPaymentRequest = new PostProcessPaymentRequest
            {
                Order = order
            };
            await _paymentService.PostProcessPaymentAsync(postProcessPaymentRequest);

            if (_webHelper.IsRequestBeingRedirected || _webHelper.IsPostBeingDone)
            {
                //redirection or POST has been done in PostProcessPayment
                return Content("Redirected");
            }

            //if no redirection has been done (to a third-party payment page)
            //theoretically it's not possible
            return RedirectToRoute("OrderDetails", new { orderId = orderId });
        }

        //My account / Order details page / Shipment details page
        public virtual async Task<IActionResult> ShipmentDetails(int shipmentId)
        {
            var shipment = await _shipmentService.GetShipmentByIdAsync(shipmentId);
            if (shipment == null)
                return Challenge();

            var order = await _orderService.GetOrderByIdAsync(shipment.OrderId);

            if (order == null || order.Deleted || (await _workContext.GetCurrentUserAsync()).Id != order.UserId)
                return Challenge();

            var model = await _orderModelFactory.PrepareShipmentDetailsModelAsync(shipment);
            return View(model);
        }

        #endregion
    }
}