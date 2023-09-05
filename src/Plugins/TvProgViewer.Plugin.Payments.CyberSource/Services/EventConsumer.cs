using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using TvProgViewer.Core;
using TvProgViewer.Core.Domain.Gdpr;
using TvProgViewer.Core.Domain.Orders;
using TvProgViewer.Core.Domain.Payments;
using TvProgViewer.Core.Events;
using TvProgViewer.Core.Http.Extensions;
using TvProgViewer.Plugin.Payments.CyberSource.Domain;
using TvProgViewer.Services.Common;
using TvProgViewer.Services.Events;
using TvProgViewer.Services.Localization;
using TvProgViewer.Services.Orders;
using TvProgViewer.Services.Payments;
using TvProgViewer.Web.Framework.Events;
using TvProgViewer.Web.Framework.Models;
using TvProgViewer.Web.Framework.UI;
using TvProgViewer.WebUI.Models.User;

namespace TvProgViewer.Plugin.Payments.CyberSource.Services
{
    /// <summary>
    /// Represents plugin event consumer
    /// </summary>
    public class EventConsumer :
        IConsumer<UserPermanentlyDeleted>,
        IConsumer<EntityUpdatedEvent<Order>>,
        IConsumer<ModelPreparedEvent<BaseTvProgModel>>,
        IConsumer<OrderPlacedEvent>,
        IConsumer<PageRenderingEvent>
    {
        #region Fields

        private readonly UserTokenService _userTokenService;
        private readonly CyberSourceSettings _cyberSourceSettings;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IGenericAttributeService _genericAttributeService;
        private readonly ILocalizationService _localizationService;
        private readonly IOrderProcessingService _orderProcessingService;
        private readonly IOrderService _orderService;
        private readonly IPaymentPluginManager _paymentPluginManager;
        private readonly IPaymentService _paymentService;
        private readonly IStoreContext _storeContext;
        private readonly IWorkContext _workContext;

        #endregion

        #region Ctor

        public EventConsumer(UserTokenService userTokenService,
            CyberSourceSettings cyberSourceSettings,
            IHttpContextAccessor httpContextAccessor,
            IGenericAttributeService genericAttributeService,
            ILocalizationService localizationService,
            IOrderProcessingService orderProcessingService,
            IOrderService orderService,
            IPaymentPluginManager paymentPluginManager,
            IPaymentService paymentService,
            IStoreContext storeContext,
            IWorkContext workContext)
        {
            _userTokenService = userTokenService;
            _cyberSourceSettings = cyberSourceSettings;
            _httpContextAccessor = httpContextAccessor;
            _genericAttributeService = genericAttributeService;
            _localizationService = localizationService;
            _orderProcessingService = orderProcessingService;
            _orderService = orderService;
            _paymentPluginManager = paymentPluginManager;
            _paymentService = paymentService;
            _storeContext = storeContext;
            _workContext = workContext;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Handle user permanently deleted event
        /// </summary>
        /// <param name="eventMessage">Event message</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        public async Task HandleEventAsync(UserPermanentlyDeleted eventMessage)
        {
            //ensure that CyberSource payment method is active
            if (!await _paymentPluginManager.IsPluginActiveAsync(CyberSourceDefaults.SystemName))
                return;

            if (!_cyberSourceSettings.TokenizationEnabled)
                return;

            //delete user tokens
            var tokens = await _userTokenService.GetAllTokensAsync(eventMessage.UserId);
            foreach (var token in tokens)
            {
                await _userTokenService.DeleteAsync(token);
            }
        }

        /// <summary>
        /// Handle model prepared event
        /// </summary>
        /// <param name="eventMessage">Event message</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        public async Task HandleEventAsync(ModelPreparedEvent<BaseTvProgModel> eventMessage)
        {
            if (eventMessage.Model is not UserNavigationModel navigationModel)
                return;

            //ensure that CyberSource payment method is active for the current user, since it's the event from the public area
            var user = await _workContext.GetCurrentUserAsync();
            var store = await _storeContext.GetCurrentStoreAsync();
            if (!await _paymentPluginManager.IsPluginActiveAsync(CyberSourceDefaults.SystemName, user, store.Id))
                return;

            if (!_cyberSourceSettings.TokenizationEnabled)
                return;

            //add a new menu item
            var orderItem = navigationModel.UserNavigationItems.FirstOrDefault(item => item.Tab == (int)UserNavigationEnum.Orders);
            var position = navigationModel.UserNavigationItems.IndexOf(orderItem) + 1;
            navigationModel.UserNavigationItems.Insert(position, new UserNavigationItemModel
            {
                RouteName = CyberSourceDefaults.UserTokensRouteName,
                ItemClass = CyberSourceDefaults.UserTokensMenuClassName,
                Tab = CyberSourceDefaults.UserTokensMenuTab,
                Title = await _localizationService.GetResourceAsync("Plugins.Payments.CyberSource.PaymentTokens")
            });
        }

        /// <summary>
        /// Handle entity updated event
        /// </summary>
        /// <param name="eventMessage">Event message</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        public async Task HandleEventAsync(EntityUpdatedEvent<Order> eventMessage)
        {
            if (eventMessage.Entity is not Order order)
                return;

            //ensure that CyberSource payment method is active
            if (!await _paymentPluginManager.IsPluginActiveAsync(CyberSourceDefaults.SystemName))
                return;

            if (order.PaymentMethodSystemName != CyberSourceDefaults.SystemName)
                return;

            //save authorize and capture date
            var customValueChanged = false;
            var customValues = _paymentService.DeserializeCustomValues(order);
            if (!string.IsNullOrEmpty(order.AuthorizationTransactionId) &&
                !customValues.ContainsKey(CyberSourceDefaults.AuthorizationDateCustomValue))
            {
                customValues.Add(CyberSourceDefaults.AuthorizationDateCustomValue, DateTime.UtcNow);
                customValueChanged = true;
            }

            if (!string.IsNullOrEmpty(order.CaptureTransactionId) &&
                !customValues.ContainsKey(CyberSourceDefaults.CaptureDateCustomValue))
            {
                customValues.Add(CyberSourceDefaults.CaptureDateCustomValue, DateTime.UtcNow);
                customValueChanged = true;
            }

            if (customValueChanged)
            {
                order.CustomValuesXml = _paymentService.SerializeCustomValues(new ProcessPaymentRequest
                {
                    CustomValues = customValues
                });

                await _orderService.UpdateOrderAsync(order);
            }
        }

        /// <summary>
        /// Handle order placed event
        /// </summary>
        /// <param name="eventMessage">Event message</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        public async Task HandleEventAsync(OrderPlacedEvent eventMessage)
        {
            if (eventMessage.Order is not Order order)
                return;

            //ensure that CyberSource payment method is active
            if (!await _paymentPluginManager.IsPluginActiveAsync(CyberSourceDefaults.SystemName))
                return;

            if (order.PaymentMethodSystemName != CyberSourceDefaults.SystemName)
                return;

            var key = string.Format(CyberSourceDefaults.OrderStatusesSessionKey, order.OrderGuid);
            var (orderStatus, paymentStatus) = _httpContextAccessor.HttpContext.Session.Get<(OrderStatus?, PaymentStatus?)>(key);
            if (!orderStatus.HasValue || !paymentStatus.HasValue)
                return;

            //remove value from session
            _httpContextAccessor.HttpContext.Session.Remove(key);

            var note = $"Order status has been changed to {orderStatus.Value} by CyberSource AVS/CVN/decision profile results";
            await _orderService.InsertOrderNoteAsync(new OrderNote
            {
                OrderId = order.Id,
                Note = note,
                DisplayToUser = false,
                CreatedOnUtc = DateTime.UtcNow
            });

            //update order status for AVS or CVN decline
            if (orderStatus.Value == OrderStatus.Cancelled)
                await _orderProcessingService.CancelOrderAsync(order, false);

            if (orderStatus.Value == OrderStatus.Pending)
            {
                //set payment status for future use
                await _genericAttributeService.SaveAttributeAsync(order, CyberSourceDefaults.PaymentStatusAttributeName, paymentStatus);

                await _orderProcessingService.CheckOrderStatusAsync(order);
            }
        }

        /// <summary>
        /// Handle page rendering event
        /// </summary>
        /// <param name="eventMessage">Event message</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        public async Task HandleEventAsync(PageRenderingEvent eventMessage)
        {
            var routeName = eventMessage.GetRouteName();
            if (routeName is null || !routeName.Equals(CyberSourceDefaults.OnePageCheckoutRouteName))
                return;

            //ensure that CyberSource payment method is active for the current user, since it's the event from the public area
            var user = await _workContext.GetCurrentUserAsync();
            var store = await _storeContext.GetCurrentStoreAsync();
            if (!await _paymentPluginManager.IsPluginActiveAsync(CyberSourceDefaults.SystemName, user, store.Id))
                return;

            if (!CyberSourceService.IsConfigured(_cyberSourceSettings))
                return;

            if (_cyberSourceSettings.PaymentConnectionMethod != ConnectionMethodType.FlexMicroForm)
                return;

            //add sсript to the one page checkout
            eventMessage.Helper.AddScriptParts(ResourceLocation.Footer, CyberSourceDefaults.FlexMicroformScriptUrl, excludeFromBundle: true);
        }

        #endregion
    }
}