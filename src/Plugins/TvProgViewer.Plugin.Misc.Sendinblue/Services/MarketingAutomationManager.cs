using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.Routing;
using TvProgViewer.Core;
using TvProgViewer.Core.Domain.Catalog;
using TvProgViewer.Core.Domain.Users;
using TvProgViewer.Core.Domain.Directory;
using TvProgViewer.Core.Domain.Orders;
using TvProgViewer.Core.Domain.Tax;
using TvProgViewer.Plugin.Misc.Sendinblue.MarketingAutomation;
using TvProgViewer.Services.Catalog;
using TvProgViewer.Services.Common;
using TvProgViewer.Services.Users;
using TvProgViewer.Services.Directory;
using TvProgViewer.Services.Logging;
using TvProgViewer.Services.Media;
using TvProgViewer.Services.Orders;
using TvProgViewer.Services.Seo;
using TvProgViewer.Web.Framework.Mvc.Routing;

namespace TvProgViewer.Plugin.Misc.Sendinblue.Services
{
    /// <summary>
    /// Represents Sendinblue marketing automation manager
    /// </summary>
    public class MarketingAutomationManager
    {
        #region Fields

        private readonly CurrencySettings _currencySettings;
        private readonly IActionContextAccessor _actionContextAccessor;
        private readonly IAddressService _addressService;
        private readonly ICategoryService _categoryService;
        private readonly ICountryService _countryService;
        private readonly ICurrencyService _currencyService;
        private readonly IUserService _userService;
        private readonly IGenericAttributeService _genericAttributeService;
        private readonly ILogger _logger;
        private readonly ITvProgUrlHelper _nopUrlHelper;
        private readonly IOrderService _orderService;
        private readonly IOrderTotalCalculationService _orderTotalCalculationService;
        private readonly IPictureService _pictureService;
        private readonly ITvChannelAttributeParser _tvChannelAttributeParser;
        private readonly ITvChannelService _tvChannelService;
        private readonly IShoppingCartService _shoppingCartService;
        private readonly IStateProvinceService _stateProvinceService;
        private readonly IStoreContext _storeContext;
        private readonly IUrlHelperFactory _urlHelperFactory;
        private readonly IUrlRecordService _urlRecordService;
        private readonly IWebHelper _webHelper;
        private readonly IWorkContext _workContext;
        private readonly MarketingAutomationHttpClient _marketingAutomationHttpClient;
        private readonly SendinblueSettings _sendinblueSettings;

        #endregion

        #region Ctor

        public MarketingAutomationManager(CurrencySettings currencySettings,
            IActionContextAccessor actionContextAccessor,
            IAddressService addressService,
            ICategoryService categoryService,
            ICountryService countryService,
            ICurrencyService currencyService,
            IUserService userService,
            IGenericAttributeService genericAttributeService,
            ILogger logger,
            ITvProgUrlHelper nopUrlHelper,
            IOrderService orderService,
            IOrderTotalCalculationService orderTotalCalculationService,
            IPictureService pictureService,
            ITvChannelAttributeParser tvChannelAttributeParser,
            ITvChannelService tvChannelService,
            IShoppingCartService shoppingCartService,
            IStateProvinceService stateProvinceService,
            IStoreContext storeContext,
            IUrlHelperFactory urlHelperFactory,
            IUrlRecordService urlRecordService,
            IWebHelper webHelper,
            IWorkContext workContext,
            MarketingAutomationHttpClient marketingAutomationHttpClient,
            SendinblueSettings sendinblueSettings)
        {
            _currencySettings = currencySettings;
            _actionContextAccessor = actionContextAccessor;
            _addressService = addressService;
            _categoryService = categoryService;
            _countryService = countryService;
            _currencyService = currencyService;
            _userService = userService;
            _genericAttributeService = genericAttributeService;
            _logger = logger;
            _nopUrlHelper = nopUrlHelper;
            _orderService = orderService;
            _orderTotalCalculationService = orderTotalCalculationService;
            _pictureService = pictureService;
            _tvChannelAttributeParser = tvChannelAttributeParser;
            _tvChannelService = tvChannelService;
            _shoppingCartService = shoppingCartService;
            _stateProvinceService = stateProvinceService;
            _storeContext = storeContext;
            _urlHelperFactory = urlHelperFactory;
            _urlRecordService = urlRecordService;
            _webHelper = webHelper;
            _workContext = workContext;
            _marketingAutomationHttpClient = marketingAutomationHttpClient;
            _sendinblueSettings = sendinblueSettings;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Handle shopping cart changed event
        /// </summary>
        /// <param name="cartItem">Shopping cart item</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        public async Task HandleShoppingCartChangedEventAsync(ShoppingCartItem cartItem)
        {
            //whether marketing automation is enabled
            if (!_sendinblueSettings.UseMarketingAutomation)
                return;

            var user = await _userService.GetUserByIdAsync(cartItem.UserId);

            try
            {
                //first, try to identify current user
                await _marketingAutomationHttpClient.RequestAsync(new IdentifyRequest { Email = user.Email });

                //get shopping cart GUID
                var shoppingCartGuid = await _genericAttributeService
                    .GetAttributeAsync<Guid?>(user, SendinblueDefaults.ShoppingCartGuidAttribute);

                //create track event object
                var trackEvent = new TrackEventRequest { Email = user.Email };

                //get current user's shopping cart
                var store = await _storeContext.GetCurrentStoreAsync();
                var cart = await _shoppingCartService
                    .GetShoppingCartAsync(user, ShoppingCartType.ShoppingCart, store.Id);

                if (cart.Any())
                {
                    //get URL helper
                    var urlHelper = _urlHelperFactory.GetUrlHelper(_actionContextAccessor.ActionContext);

                    //get shopping cart amounts
                    var (_, cartDiscount, _, cartSubtotal, _) = await _orderTotalCalculationService.GetShoppingCartSubTotalAsync(cart,
                        await _workContext.GetTaxDisplayTypeAsync() == TaxDisplayType.IncludingTax);
                    var cartTax = await _orderTotalCalculationService.GetTaxTotalAsync(cart, false);
                    var cartShipping = await _orderTotalCalculationService.GetShoppingCartShippingTotalAsync(cart);
                    var (cartTotal, _, _, _, _, _) = await _orderTotalCalculationService.GetShoppingCartTotalAsync(cart, false, false);

                    //get tvChannels data by shopping cart items
                    var itemsData = await cart.Where(item => item.TvChannelId != 0).SelectAwait(async item =>
                    {
                        var tvChannel = await _tvChannelService.GetTvChannelByIdAsync(item.TvChannelId);

                        //try to get tvChannel attribute combination
                        var combination = await _tvChannelAttributeParser.FindTvChannelAttributeCombinationAsync(tvChannel, item.AttributesXml);

                        //get default tvChannel picture
                        var picture = await _pictureService.GetTvChannelPictureAsync(tvChannel, item.AttributesXml);

                        //get tvChannel SEO slug name
                        var seName = await _urlRecordService.GetSeNameAsync(tvChannel);

                        //create tvChannel data
                        return new
                        {
                            id = tvChannel.Id,
                            name = tvChannel.Name,
                            variant_id = combination?.Id ?? tvChannel.Id,
                            variant_name = combination?.Sku ?? tvChannel.Name,
                            sku = combination?.Sku ?? tvChannel.Sku,
                            category = await (await _categoryService.GetTvChannelCategoriesByTvChannelIdAsync(item.TvChannelId)).AggregateAwaitAsync(",", async (all, pc) =>
                            {
                                var res = (await _categoryService.GetCategoryByIdAsync(pc.CategoryId)).Name;
                                res = all == "," ? res : all + ", " + res;
                                return res;
                            }),
                            url = await _nopUrlHelper.RouteGenericUrlAsync<TvChannel>(new { SeName = seName }, _webHelper.GetCurrentRequestProtocol()),
                            image = (await _pictureService.GetPictureUrlAsync(picture)).Url,
                            quantity = item.Quantity,
                            price = (await _shoppingCartService.GetSubTotalAsync(item, true)).subTotal
                        };
                    }).ToArrayAsync();

                    //prepare cart data
                    var cartData = new
                    {
                        affiliation = store.Name,
                        subtotal = cartSubtotal,
                        shipping = cartShipping ?? decimal.Zero,
                        total_before_tax = cartSubtotal + cartShipping ?? decimal.Zero,
                        tax = cartTax,
                        discount = cartDiscount,
                        revenue = cartTotal ?? decimal.Zero,
                        url = urlHelper.RouteUrl("ShoppingCart", null, _webHelper.GetCurrentRequestProtocol()),
                        currency = (await _currencyService.GetCurrencyByIdAsync(_currencySettings.PrimaryStoreCurrencyId))?.CurrencyCode,
                        //gift_wrapping = string.Empty, //currently we can't get this value
                        items = itemsData
                    };

                    //if there is a single item in the cart, so the cart is just created
                    if (cart.Count == 1)
                    {
                        shoppingCartGuid = Guid.NewGuid();
                    }
                    else
                    {
                        //otherwise cart is updated
                        shoppingCartGuid ??= Guid.NewGuid();
                    }
                    trackEvent.EventName = SendinblueDefaults.CartUpdatedEventName;
                    trackEvent.EventData = new { id = $"cart:{shoppingCartGuid}", data = cartData };
                }
                else
                {
                    //there are no items in the cart, so the cart is deleted
                    shoppingCartGuid ??= Guid.NewGuid();
                    trackEvent.EventName = SendinblueDefaults.CartDeletedEventName;
                    trackEvent.EventData = new { id = $"cart:{shoppingCartGuid}" };
                }

                //track event
                await _marketingAutomationHttpClient.RequestAsync(trackEvent);

                //update GUID for the current user's shopping cart
                await _genericAttributeService.SaveAttributeAsync(user, SendinblueDefaults.ShoppingCartGuidAttribute, shoppingCartGuid);
            }
            catch (Exception exception)
            {
                //log full error
                await _logger.ErrorAsync($"Sendinblue Marketing Automation error: {exception.Message}.", exception, user);
            }
        }

        /// <summary>
        /// Handle order completed event
        /// </summary>
        /// <param name="order">Order</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        public async Task HandleOrderCompletedEventAsync(Order order)
        {
            //whether marketing automation is enabled
            if (!_sendinblueSettings.UseMarketingAutomation)
                return;

            if (order is null)
                throw new ArgumentNullException(nameof(order));

            var user = await _userService.GetUserByIdAsync(order.UserId);

            try
            {
                //first, try to identify current user
                await _marketingAutomationHttpClient.RequestAsync(new IdentifyRequest { Email = user.Email });

                //get URL helper
                var urlHelper = _urlHelperFactory.GetUrlHelper(_actionContextAccessor.ActionContext);

                //get tvChannels data by order items
                var itemsData = await (await _orderService.GetOrderItemsAsync(order.Id)).SelectAwait(async item =>
                {
                    var tvChannel = await _tvChannelService.GetTvChannelByIdAsync(item.TvChannelId);

                    //try to get tvChannel attribute combination
                    var combination = await _tvChannelAttributeParser.FindTvChannelAttributeCombinationAsync(tvChannel, item.AttributesXml);

                    //get default tvChannel picture
                    var picture = await _pictureService.GetTvChannelPictureAsync(tvChannel, item.AttributesXml);

                    //get tvChannel SEO slug name
                    var seName = await _urlRecordService.GetSeNameAsync(tvChannel);

                    //create tvChannel data
                    return new
                    {
                        id = tvChannel.Id,
                        name = tvChannel.Name,
                        variant_id = combination?.Id ?? tvChannel.Id,
                        variant_name = combination?.Sku ?? tvChannel.Name,
                        sku = combination?.Sku ?? tvChannel.Sku,
                        category = await (await _categoryService.GetTvChannelCategoriesByTvChannelIdAsync(item.TvChannelId)).AggregateAwaitAsync(",", async (all, pc) =>
                        {
                            var res = (await _categoryService.GetCategoryByIdAsync(pc.CategoryId)).Name;
                            res = all == "," ? res : all + ", " + res;
                            return res;
                        }),
                        url = await _nopUrlHelper.RouteGenericUrlAsync<TvChannel>(new { SeName = seName }, _webHelper.GetCurrentRequestProtocol()),
                        image = (await _pictureService.GetPictureUrlAsync(picture)).Url,
                        quantity = item.Quantity,
                        price = item.PriceInclTax,
                    };
                }).ToArrayAsync();

                var shippingAddress = await _addressService.GetAddressByIdAsync(order.ShippingAddressId ?? 0);
                var billingAddress = await _addressService.GetAddressByIdAsync(order.BillingAddressId);

                var shippingAddressData = new
                {
                    firstname = shippingAddress?.FirstName,
                    lastname = shippingAddress?.LastName,
                    company = shippingAddress?.Company,
                    phone = shippingAddress?.PhoneNumber,
                    address1 = shippingAddress?.Address1,
                    address2 = shippingAddress?.Address2,
                    city = shippingAddress?.City,
                    country = (await _countryService.GetCountryByAddressAsync(shippingAddress))?.Name,
                    state = (await _stateProvinceService.GetStateProvinceByAddressAsync(shippingAddress))?.Name,
                    zipcode = shippingAddress?.ZipPostalCode
                };

                var billingAddressData = new
                {
                    firstname = billingAddress?.FirstName,
                    lastname = billingAddress?.LastName,
                    company = billingAddress?.Company,
                    phone = billingAddress?.PhoneNumber,
                    address1 = billingAddress?.Address1,
                    address2 = billingAddress?.Address2,
                    city = billingAddress?.City,
                    country = (await _countryService.GetCountryByAddressAsync(billingAddress))?.Name,
                    state = (await _stateProvinceService.GetStateProvinceByAddressAsync(billingAddress))?.Name,
                    zipcode = billingAddress?.ZipPostalCode
                };

                var store = await _storeContext.GetCurrentStoreAsync();

                //prepare cart data
                var cartData = new
                {
                    id = order.Id,
                    affiliation = user.AffiliateId > 0 ? user.AffiliateId.ToString() : store.Name,
                    date = order.PaidDateUtc?.ToString("yyyy-MM-dd"),
                    subtotal = order.OrderSubtotalInclTax,
                    shipping = order.OrderShippingInclTax,
                    total_before_tax = order.OrderSubtotalInclTax + order.OrderShippingInclTax,
                    tax = order.OrderTax,
                    discount = order.OrderDiscount,
                    revenue = order.OrderTotal,
                    url = urlHelper.RouteUrl("OrderDetails", new { orderId = order.Id }, _webHelper.GetCurrentRequestProtocol()),
                    currency = order.UserCurrencyCode,
                    //gift_wrapping = string.Empty, //currently we can't get this value
                    items = itemsData,
                    shipping_address = shippingAddressData,
                    billing_address = billingAddressData
                };

                //get shopping cart GUID
                var shoppingCartGuid = await _genericAttributeService.GetAttributeAsync<Guid?>(order,
                    SendinblueDefaults.ShoppingCartGuidAttribute) ?? Guid.NewGuid();

                //create track event object
                var trackEvent = new TrackEventRequest
                {
                    Email = user.Email,
                    EventName = SendinblueDefaults.OrderCompletedEventName,
                    EventData = new { id = $"cart:{shoppingCartGuid}", data = cartData }
                };

                //track event
                await _marketingAutomationHttpClient.RequestAsync(trackEvent);

                //update GUID for the current user's shopping cart
                await _genericAttributeService.SaveAttributeAsync<Guid?>(order, SendinblueDefaults.ShoppingCartGuidAttribute, null);
            }
            catch (Exception exception)
            {
                //log full error
                await _logger.ErrorAsync($"Sendinblue Marketing Automation error: {exception.Message}.", exception, user);
            }
        }

        /// <summary>
        /// Handle order placed event
        /// </summary>
        /// <param name="order">Order</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        public async Task HandleOrderPlacedEventAsync(Order order)
        {
            //whether marketing automation is enabled
            if (!_sendinblueSettings.UseMarketingAutomation)
                return;

            //copy shopping cart GUID to order
            var shoppingCartGuid = await _genericAttributeService.GetAttributeAsync<User, Guid?>(order.UserId, SendinblueDefaults.ShoppingCartGuidAttribute);
            await _genericAttributeService.SaveAttributeAsync(order, SendinblueDefaults.ShoppingCartGuidAttribute, shoppingCartGuid);
        }

        #endregion
    }
}