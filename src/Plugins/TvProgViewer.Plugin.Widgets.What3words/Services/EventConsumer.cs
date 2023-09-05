using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Primitives;
using TvProgViewer.Core;
using TvProgViewer.Core.Domain.Common;
using TvProgViewer.Core.Domain.Users;
using TvProgViewer.Core.Domain.Orders;
using TvProgViewer.Core.Events;
using TvProgViewer.Services.Cms;
using TvProgViewer.Services.Common;
using TvProgViewer.Services.Users;
using TvProgViewer.Services.Events;
using TvProgViewer.Web.Framework.Events;
using TvProgViewer.Web.Framework.Models;
using TvProgViewer.WebUI.Models.Checkout;

namespace TvProgViewer.Plugin.Widgets.What3words.Services
{
    /// <summary>
    /// Represents plugin event consumer
    /// </summary>
    public class EventConsumer :
        IConsumer<EntityDeletedEvent<Address>>,
        IConsumer<EntityInsertedEvent<UserAddressMapping>>,
        IConsumer<ModelReceivedEvent<BaseTvProgModel>>,
        IConsumer<OrderPlacedEvent>
    {
        #region Fields

        private readonly IAddressService _addressService;
        private readonly IUserService _userService;
        private readonly IGenericAttributeService _genericAttributeService;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IWidgetPluginManager _widgetPluginManager;
        private readonly IWorkContext _workContext;
        private readonly What3wordsSettings _what3WordsSettings;

        #endregion

        #region Ctor

        public EventConsumer(IAddressService addressService,
            IUserService userService,
            IGenericAttributeService genericAttributeService,
            IHttpContextAccessor httpContextAccessor,
            IWidgetPluginManager widgetPluginManager,
            IWorkContext workContext,
            What3wordsSettings what3WordsSettings)
        {
            _addressService = addressService;
            _userService = userService;
            _genericAttributeService = genericAttributeService;
            _httpContextAccessor = httpContextAccessor;
            _widgetPluginManager = widgetPluginManager;
            _workContext = workContext;
            _what3WordsSettings = what3WordsSettings;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Handle entity deleted event
        /// </summary>
        /// <param name="eventMessage">Event message</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        public async Task HandleEventAsync(EntityDeletedEvent<Address> eventMessage)
        {
            if (eventMessage.Entity is not Address address)
                return;

            await _genericAttributeService.SaveAttributeAsync<string>(address, What3wordsDefaults.AddressValueAttribute, null);
        }

        /// <summary>
        /// Handle entity inserted event
        /// </summary>
        /// <param name="eventMessage">Event message</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        public async Task HandleEventAsync(EntityInsertedEvent<UserAddressMapping> eventMessage)
        {
            if (eventMessage.Entity is not UserAddressMapping mapping)
                return;

            //move previously cached value to the address
            if (_httpContextAccessor.HttpContext.Items.TryGetValue(What3wordsDefaults.AddressValueAttribute, out var addressValue))
            {
                var address = await _addressService.GetAddressByIdAsync(mapping.AddressId);
                if (address is not null)
                    await _genericAttributeService.SaveAttributeAsync(address, What3wordsDefaults.AddressValueAttribute, addressValue);
            }
        }

        /// <summary>
        /// Handle model received event
        /// </summary>
        /// <param name="eventMessage">Event message</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        public async Task HandleEventAsync(ModelReceivedEvent<BaseTvProgModel> eventMessage)
        {
            if (eventMessage.Model is not CheckoutBillingAddressModel &&
                eventMessage.Model is not CheckoutShippingAddressModel)
                return;

            var user = await _workContext.GetCurrentUserAsync();
            if (!await _widgetPluginManager.IsPluginActiveAsync(What3wordsDefaults.SystemName, user))
                return;

            if (!_what3WordsSettings.Enabled)
                return;

            //cache the value within the request, we save it to the address later
            var form = _httpContextAccessor.HttpContext.Request.Form;
            if (form.TryGetValue(What3wordsDefaults.ComponentName, out var addressValue) && !StringValues.IsNullOrEmpty(addressValue))
                _httpContextAccessor.HttpContext.Items[What3wordsDefaults.AddressValueAttribute] = addressValue.ToString().TrimStart('/');
        }

        /// <summary>
        /// Handle order placed event
        /// </summary>
        /// <param name="eventMessage">Event message</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        public async Task HandleEventAsync(OrderPlacedEvent eventMessage)
        {
            if (eventMessage.Order is null)
                return;

            var user = await _userService.GetUserByIdAsync(eventMessage.Order.UserId);
            if (!await _widgetPluginManager.IsPluginActiveAsync(What3wordsDefaults.SystemName, user))
                return;

            if (!_what3WordsSettings.Enabled)
                return;

            async Task copyAddressValueAsync(int? userAddressId, int? orderAddressId)
            {
                var userAddress = await _addressService.GetAddressByIdAsync(userAddressId ?? 0);
                var addressValue = userAddress is not null
                    ? await _genericAttributeService.GetAttributeAsync<string>(userAddress, What3wordsDefaults.AddressValueAttribute)
                    : null;
                if (!string.IsNullOrEmpty(addressValue))
                {
                    var orderAddress = await _addressService.GetAddressByIdAsync(orderAddressId ?? 0);
                    if (orderAddress is not null)
                        await _genericAttributeService.SaveAttributeAsync(orderAddress, What3wordsDefaults.AddressValueAttribute, addressValue);
                }
            }

            //copy values from user addresses to order addresses for next use
            await copyAddressValueAsync(user.BillingAddressId, eventMessage.Order.BillingAddressId);
            await copyAddressValueAsync(user.ShippingAddressId, eventMessage.Order.ShippingAddressId);
        }

        #endregion

    }
}