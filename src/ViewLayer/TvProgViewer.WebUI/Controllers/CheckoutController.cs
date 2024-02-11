using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using TvProgViewer.Core;
using TvProgViewer.Core.Domain.Common;
using TvProgViewer.Core.Domain.Users;
using TvProgViewer.Core.Domain.Orders;
using TvProgViewer.Core.Domain.Payments;
using TvProgViewer.Core.Domain.Security;
using TvProgViewer.Core.Domain.Shipping;
using TvProgViewer.Core.Domain.Tax;
using TvProgViewer.Core.Http.Extensions;
using TvProgViewer.Services.Catalog;
using TvProgViewer.Services.Common;
using TvProgViewer.Services.Users;
using TvProgViewer.Services.Directory;
using TvProgViewer.Services.Localization;
using TvProgViewer.Services.Logging;
using TvProgViewer.Services.Orders;
using TvProgViewer.Services.Payments;
using TvProgViewer.Services.Shipping;
using TvProgViewer.Services.Tax;
using TvProgViewer.WebUI.Extensions;
using TvProgViewer.WebUI.Factories;
using TvProgViewer.Web.Framework.Controllers;
using TvProgViewer.Web.Framework.Mvc.Filters;
using TvProgViewer.WebUI.Models.Checkout;
using TvProgViewer.WebUI.Models.Common;

namespace TvProgViewer.WebUI.Controllers
{
    [AutoValidateAntiforgeryToken]
    public partial class CheckoutController : BasePublicController
    {
        #region Fields

        private readonly AddressSettings _addressSettings;
        private readonly CaptchaSettings _captchaSettings;
        private readonly UserSettings _userSettings;
        private readonly IAddressAttributeParser _addressAttributeParser;        
        private readonly IAddressModelFactory _addressModelFactory;
        private readonly IAddressService _addressService;
        private readonly ICheckoutModelFactory _checkoutModelFactory;
        private readonly ICountryService _countryService;
        private readonly IUserService _userService;
        private readonly IGenericAttributeService _genericAttributeService;
        private readonly ILocalizationService _localizationService;
        private readonly ILogger _logger;
        private readonly IOrderProcessingService _orderProcessingService;
        private readonly IOrderService _orderService;
        private readonly IPaymentPluginManager _paymentPluginManager;
        private readonly IPaymentService _paymentService;
        private readonly ITvChannelService _tvchannelService;
        private readonly IShippingService _shippingService;
        private readonly IShoppingCartService _shoppingCartService;
        private readonly IStoreContext _storeContext;
        private readonly ITaxService _taxService;
        private readonly IWebHelper _webHelper;
        private readonly IWorkContext _workContext;
        private readonly OrderSettings _orderSettings;
        private readonly PaymentSettings _paymentSettings;
        private readonly RewardPointsSettings _rewardPointsSettings;
        private readonly ShippingSettings _shippingSettings;
        private readonly TaxSettings _taxSettings;

        #endregion

        #region Ctor

        public CheckoutController(AddressSettings addressSettings,
            CaptchaSettings captchaSettings,
            UserSettings userSettings,
            IAddressAttributeParser addressAttributeParser,
            IAddressModelFactory addressModelFactory,
            IAddressService addressService,
            ICheckoutModelFactory checkoutModelFactory,
            ICountryService countryService,
            IUserService userService,
            IGenericAttributeService genericAttributeService,
            ILocalizationService localizationService,
            ILogger logger,
            IOrderProcessingService orderProcessingService,
            IOrderService orderService,
            IPaymentPluginManager paymentPluginManager,
            IPaymentService paymentService,
            ITvChannelService tvchannelService,
            IShippingService shippingService,
            IShoppingCartService shoppingCartService,
            IStoreContext storeContext,
            ITaxService taxService,
            IWebHelper webHelper,
            IWorkContext workContext,
            OrderSettings orderSettings,
            PaymentSettings paymentSettings,
            RewardPointsSettings rewardPointsSettings,
            ShippingSettings shippingSettings,
            TaxSettings taxSettings)
        {
            _addressSettings = addressSettings;
            _captchaSettings = captchaSettings;
            _userSettings = userSettings;
            _addressAttributeParser = addressAttributeParser;
            _addressModelFactory = addressModelFactory;
            _addressService = addressService;
            _checkoutModelFactory = checkoutModelFactory;
            _countryService = countryService;
            _userService = userService;
            _genericAttributeService = genericAttributeService;
            _localizationService = localizationService;
            _logger = logger;
            _orderProcessingService = orderProcessingService;
            _orderService = orderService;
            _paymentPluginManager = paymentPluginManager;
            _paymentService = paymentService;
            _tvchannelService = tvchannelService;
            _shippingService = shippingService;
            _shoppingCartService = shoppingCartService;
            _storeContext = storeContext;
            _taxService = taxService;
            _webHelper = webHelper;
            _workContext = workContext;
            _orderSettings = orderSettings;
            _paymentSettings = paymentSettings;
            _rewardPointsSettings = rewardPointsSettings;
            _shippingSettings = shippingSettings;
            _taxSettings = taxSettings;
        }

        #endregion

        #region Utilities

        protected virtual async Task<bool> IsMinimumOrderPlacementIntervalValidAsync(User user)
        {
            //prevent 2 orders being placed within an X seconds time frame
            if (_orderSettings.MinimumOrderPlacementInterval == 0)
                return true;

            var store = await _storeContext.GetCurrentStoreAsync();

            var lastOrder = (await _orderService.SearchOrdersAsync(storeId: store.Id,
                userId: user.Id, pageSize: 1))
                .FirstOrDefault();
            if (lastOrder == null)
                return true;

            var interval = DateTime.UtcNow - lastOrder.CreatedOnUtc;
            return interval.TotalSeconds > _orderSettings.MinimumOrderPlacementInterval;
        }

        /// <summary>
        /// Parses the value indicating whether the "pickup in store" is allowed
        /// </summary>
        /// <param name="form">The form</param>
        /// <returns>The value indicating whether the "pickup in store" is allowed</returns>
        protected virtual bool ParsePickupInStore(IFormCollection form)
        {
            var pickupInStore = false;

            var pickupInStoreParameter = form["PickupInStore"].FirstOrDefault();
            if (!string.IsNullOrWhiteSpace(pickupInStoreParameter))
                _ = bool.TryParse(pickupInStoreParameter, out pickupInStore);

            return pickupInStore;
        }

        /// <summary>
        /// Parses the pickup option
        /// </summary>
        /// <param name="cart">Shopping Cart</param>
        /// <param name="form">The form</param>
        /// <returns>
        /// The task result contains the pickup option
        /// </returns>
        protected virtual async Task<PickupPoint> ParsePickupOptionAsync(IList<ShoppingCartItem> cart, IFormCollection form)
        {
            var pickupPoint = form["pickup-points-id"].ToString().Split(new[] { "___" }, StringSplitOptions.None);

            var user = await _workContext.GetCurrentUserAsync();
            var store = await _storeContext.GetCurrentStoreAsync();
            var address = user.BillingAddressId.HasValue
                ? await _addressService.GetAddressByIdAsync(user.BillingAddressId.Value)
                : null;

            var selectedPoint = (await _shippingService.GetPickupPointsAsync(cart, address,
                user, pickupPoint[1], store.Id)).PickupPoints.FirstOrDefault(x => x.Id.Equals(pickupPoint[0]));

            if (selectedPoint == null)
                throw new Exception("Pickup point is not allowed");

            return selectedPoint;
        }

        /// <summary>
        /// Saves the pickup option
        /// </summary>
        /// <param name="pickupPoint">The pickup option</param>
        protected virtual async Task SavePickupOptionAsync(PickupPoint pickupPoint)
        {
            var name = !string.IsNullOrEmpty(pickupPoint.Name) ?
                string.Format(await _localizationService.GetResourceAsync("Checkout.PickupPoints.Name"), pickupPoint.Name) :
                await _localizationService.GetResourceAsync("Checkout.PickupPoints.NullName");
            var pickUpInStoreShippingOption = new ShippingOption
            {
                Name = name,
                Rate = pickupPoint.PickupFee,
                Description = pickupPoint.Description,
                ShippingRateComputationMethodSystemName = pickupPoint.ProviderSystemName,
                IsPickupInStore = true
            };

            var user = await _workContext.GetCurrentUserAsync();
            var store = await _storeContext.GetCurrentStoreAsync();
            await _genericAttributeService.SaveAttributeAsync(user, TvProgUserDefaults.SelectedShippingOptionAttribute, pickUpInStoreShippingOption, store.Id);
            await _genericAttributeService.SaveAttributeAsync(user, TvProgUserDefaults.SelectedPickupPointAttribute, pickupPoint, store.Id);
        }

        /// <summary>
        /// Save user VAT number
        /// </summary>
        /// <param name="fullVatNumber">The full VAT number</param>
        /// <param name="user">The user</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the Vat number error if exists
        /// </returns>
        protected virtual async Task<string> SaveUserVatNumberAsync(string fullVatNumber, User user)
        {
            var (vatNumberStatus, _, _) = await _taxService.GetVatNumberStatusAsync(fullVatNumber);
            user.VatNumberStatus = vatNumberStatus;
            user.VatNumber = fullVatNumber;
            await _userService.UpdateUserAsync(user);

            if (vatNumberStatus != VatNumberStatus.Valid && !string.IsNullOrEmpty(fullVatNumber))
            {
                var warning = await _localizationService.GetResourceAsync("Checkout.VatNumber.Warning");
                return string.Format(warning, await _localizationService.GetLocalizedEnumAsync(vatNumberStatus));
            }

            return string.Empty;
        }

        protected async Task<JsonResult> EditAddressAsync(AddressModel addressModel, IFormCollection form, Func<User, IList<ShoppingCartItem>, Address, Task<JsonResult>> getResult)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    var errors = string.Join(", ", ModelState.Values.Where(p => p.Errors.Any()).SelectMany(p => p.Errors)
                        .Select(p => p.ErrorMessage));

                    throw new Exception(errors);
                }
                
                var user = await _workContext.GetCurrentUserAsync();
                var store = await _storeContext.GetCurrentStoreAsync();
                var cart = await _shoppingCartService.GetShoppingCartAsync(user, ShoppingCartType.ShoppingCart, store.Id);
                if (!cart.Any())
                    throw new Exception("Your cart is empty");

                //find address (ensure that it belongs to the current user)
                var address = await _userService.GetUserAddressAsync(user.Id, addressModel.Id);
                if (address == null)
                    throw new Exception("Address can't be loaded");

                //custom address attributes
                var customAttributes = await _addressAttributeParser.ParseCustomAddressAttributesAsync(form);
                var customAttributeWarnings = await _addressAttributeParser.GetAttributeWarningsAsync(customAttributes);

                if (customAttributeWarnings.Any()) 
                    return Json(new { error = 1, message = customAttributeWarnings });

                address = addressModel.ToEntity(address);
                address.CustomAttributes = customAttributes;

                await _addressService.UpdateAddressAsync(address);

                return await getResult(user, cart, address);
            }
            catch (Exception exc)
            {
                await _logger.WarningAsync(exc.Message, exc, await _workContext.GetCurrentUserAsync());
                return Json(new { error = 1, message = exc.Message });
            }
        }

        protected async Task<JsonResult> DeleteAddressAsync(int addressId, Func<IList<ShoppingCartItem>, Task<JsonResult>> getResult)
        {
            try
            {
                var user = await _workContext.GetCurrentUserAsync();
                var store = await _storeContext.GetCurrentStoreAsync();
                var cart = await _shoppingCartService.GetShoppingCartAsync(user, ShoppingCartType.ShoppingCart, store.Id);
                if (!cart.Any())
                    throw new Exception("Your cart is empty");

                var address = await _userService.GetUserAddressAsync(user.Id, addressId);
                if (address != null)
                {
                    await _userService.RemoveUserAddressAsync(user, address);
                    await _userService.UpdateUserAsync(user);
                    await _addressService.DeleteAddressAsync(address);
                }

                return await getResult(cart);
            }
            catch (Exception exc)
            {
                await _logger.WarningAsync(exc.Message, exc, await _workContext.GetCurrentUserAsync());
                return Json(new { error = 1, message = exc.Message });
            }
        }

        #endregion

        #region Methods (common)

        public virtual async Task<IActionResult> Index()
        {
            //validation
            if (_orderSettings.CheckoutDisabled)
                return RedirectToRoute("ShoppingCart");

            var user = await _workContext.GetCurrentUserAsync();
            var store = await _storeContext.GetCurrentStoreAsync();
            var cart = await _shoppingCartService.GetShoppingCartAsync(user, ShoppingCartType.ShoppingCart, store.Id);

            if (!cart.Any())
                return RedirectToRoute("ShoppingCart");

            var cartTvChannelIds = cart.Select(ci => ci.TvChannelId).ToArray();
            var downloadableTvChannelsRequireRegistration =
                _userSettings.RequireRegistrationForDownloadableTvChannels && await _tvchannelService.HasAnyDownloadableTvChannelAsync(cartTvChannelIds);

            if (await _userService.IsGuestAsync(user) && (!_orderSettings.AnonymousCheckoutAllowed || downloadableTvChannelsRequireRegistration))
                return Challenge();

            //if we have only "button" payment methods available (displayed on the shopping cart page, not during checkout),
            //then we should allow standard checkout
            //all payment methods (do not filter by country here as it could be not specified yet)
            var paymentMethods = await (await _paymentPluginManager
                .LoadActivePluginsAsync(user, store.Id))
                .WhereAwait(async pm => !await pm.HidePaymentMethodAsync(cart)).ToListAsync();
            //payment methods displayed during checkout (not with "Button" type)
            var nonButtonPaymentMethods = paymentMethods
                .Where(pm => pm.PaymentMethodType != PaymentMethodType.Button)
                .ToList();
            //"button" payment methods(*displayed on the shopping cart page)
            var buttonPaymentMethods = paymentMethods
                .Where(pm => pm.PaymentMethodType == PaymentMethodType.Button)
                .ToList();
            if (!nonButtonPaymentMethods.Any() && buttonPaymentMethods.Any())
                return RedirectToRoute("ShoppingCart");

            //reset checkout data
            await _userService.ResetCheckoutDataAsync(user, store.Id);

            //validation (cart)
            var checkoutAttributesXml = await _genericAttributeService.GetAttributeAsync<string>(user,
                TvProgUserDefaults.CheckoutAttributes, store.Id);
            var scWarnings = await _shoppingCartService.GetShoppingCartWarningsAsync(cart, checkoutAttributesXml, true);
            if (scWarnings.Any())
                return RedirectToRoute("ShoppingCart");
            //validation (each shopping cart item)
            foreach (var sci in cart)
            {
                var tvchannel = await _tvchannelService.GetTvChannelByIdAsync(sci.TvChannelId);

                var sciWarnings = await _shoppingCartService.GetShoppingCartItemWarningsAsync(user,
                    sci.ShoppingCartType,
                    tvchannel,
                    sci.StoreId,
                    sci.AttributesXml,
                    sci.UserEnteredPrice,
                    sci.RentalStartDateUtc,
                    sci.RentalEndDateUtc,
                    sci.Quantity,
                    false,
                    sci.Id);
                if (sciWarnings.Any())
                    return RedirectToRoute("ShoppingCart");
            }

            if (_orderSettings.OnePageCheckoutEnabled)
                return RedirectToRoute("CheckoutOnePage");

            return RedirectToRoute("CheckoutBillingAddress");
        }

        public virtual async Task<IActionResult> Completed(int? orderId)
        {
            //validation
            var user = await _workContext.GetCurrentUserAsync();
            if (await _userService.IsGuestAsync(user) && !_orderSettings.AnonymousCheckoutAllowed)
                return Challenge();

            Order order = null;
            if (orderId.HasValue)
            {
                //load order by identifier (if provided)
                order = await _orderService.GetOrderByIdAsync(orderId.Value);
            }
            if (order == null)
            {
                var store = await _storeContext.GetCurrentStoreAsync();
                order = (await _orderService.SearchOrdersAsync(storeId: store.Id,
                userId: user.Id, pageSize: 1))
                    .FirstOrDefault();
            }
            if (order == null || order.Deleted || user.Id != order.UserId)
            {
                return RedirectToRoute("Homepage");
            }

            //disable "order completed" page?
            if (_orderSettings.DisableOrderCompletedPage)
            {
                return RedirectToRoute("OrderDetails", new { orderId = order.Id });
            }

            //model
            var model = await _checkoutModelFactory.PrepareCheckoutCompletedModelAsync(order);
            return View(model);
        }

        /// <summary>
        /// Get specified Address by addresId
        /// </summary>
        /// <param name="addressId"></param>
        public virtual async Task<IActionResult> GetAddressById(int addressId)
        {
            var user = await _workContext.GetCurrentUserAsync();
            var address = await _userService.GetUserAddressAsync(user.Id, addressId);
            if (address == null)
                throw new ArgumentNullException(nameof(address));

            var addressModel = new AddressModel();

            await _addressModelFactory.PrepareAddressModelAsync(addressModel,
                address: address,
                excludeProperties: false,
                addressSettings: _addressSettings,
                prePopulateWithUserFields: true,
                user: user);

            var json = JsonConvert.SerializeObject(addressModel, Formatting.Indented,
                new JsonSerializerSettings
                {
                    ReferenceLoopHandling = ReferenceLoopHandling.Ignore
                });

            return Content(json, "application/json");
        }

        /// <summary>
        /// Save edited address
        /// </summary>
        /// <param name="model"></param>
        /// <param name="form"></param>
        /// <param name="opc"></param>
        /// <returns></returns>
        public virtual async Task<IActionResult> SaveEditBillingAddress(CheckoutBillingAddressModel model, IFormCollection form, bool opc = false)
        {
            return await EditAddressAsync(model.BillingNewAddress, form, async (user, cart, address) =>
            {
                user.BillingAddressId = address.Id;
                await _userService.UpdateUserAsync(user);

                if (!opc)
                    return Json(new { redirect = Url.RouteUrl("CheckoutBillingAddress") });

                var billingAddressModel =
                    await _checkoutModelFactory.PrepareBillingAddressModelAsync(cart, address.CountryId);

                return Json(new
                {
                    selected_id = model.BillingNewAddress.Id,
                    update_section = new UpdateSectionJsonModel
                    {
                        name = "billing",
                        html = await RenderPartialViewToStringAsync("OpcBillingAddress",
                            billingAddressModel)
                    }
                });
            });
        }

        /// <summary>
        /// Delete edited address
        /// </summary>
        /// <param name="addressId"></param>
        /// <param name="opc"></param>
        public virtual async Task<IActionResult> DeleteEditBillingAddress(int addressId, bool opc = false)
        {
            return await DeleteAddressAsync(addressId, async (cart) =>
            {
                if (!opc) 
                    return Json(new { redirect = Url.RouteUrl("CheckoutBillingAddress") });

                var billingAddressModel = await _checkoutModelFactory.PrepareBillingAddressModelAsync(cart);
                return Json(new
                {
                    update_section = new UpdateSectionJsonModel
                    {
                        name = "billing",
                        html = await RenderPartialViewToStringAsync("OpcBillingAddress", billingAddressModel)
                    }
                });
            });
        }

        /// <summary>
        /// Delete edited address
        /// </summary>
        /// <param name="addressId"></param>
        /// <param name="opc"></param>
        public virtual async Task<IActionResult> DeleteEditShippingAddress(int addressId, bool opc = false)
        {
            return await DeleteAddressAsync(addressId, async (cart) =>
            {
                if (!opc)
                    return Json(new { redirect = Url.RouteUrl("CheckoutShippingAddress") });

                var shippingAddressModel = await _checkoutModelFactory.PrepareShippingAddressModelAsync(cart);
                
                return Json(new
                {
                    update_section = new UpdateSectionJsonModel
                    {
                        name = "shipping",
                        html = await RenderPartialViewToStringAsync("OpcShippingAddress", shippingAddressModel)
                    }
                });
            });
        }

        /// <summary>
        /// Save edited address
        /// </summary>
        /// <param name="model"></param>
        /// <param name="opc"></param>
        /// <returns></returns>
        public virtual async Task<IActionResult> SaveEditShippingAddress(CheckoutShippingAddressModel model, IFormCollection form, bool opc = false)
        {
            return await EditAddressAsync(model.ShippingNewAddress, form, async (user, cart, address) =>
            {
                user.ShippingAddressId = address.Id;
                await _userService.UpdateUserAsync(user);

                if (!opc)
                    return Json(new
                    {
                        redirect = Url.RouteUrl("CheckoutShippingAddress")
                    });

                var shippingAddressModel = await _checkoutModelFactory.PrepareShippingAddressModelAsync(cart, address.CountryId);
                return Json(new
                {
                    selected_id = model.ShippingNewAddress.Id,
                    update_section = new UpdateSectionJsonModel
                    {
                        name = "shipping",
                        html = await RenderPartialViewToStringAsync("OpcShippingAddress", shippingAddressModel)
                    }
                });
            });
        }
        
        #endregion

        #region Methods (multistep checkout)

        public virtual async Task<IActionResult> BillingAddress(IFormCollection form)
        {
            //validation
            if (_orderSettings.CheckoutDisabled)
                return RedirectToRoute("ShoppingCart");

            var user = await _workContext.GetCurrentUserAsync();
            var store = await _storeContext.GetCurrentStoreAsync();
            var cart = await _shoppingCartService.GetShoppingCartAsync(user, ShoppingCartType.ShoppingCart, store.Id);

            if (!cart.Any())
                return RedirectToRoute("ShoppingCart");

            if (_orderSettings.OnePageCheckoutEnabled)
                return RedirectToRoute("CheckoutOnePage");

            if (await _userService.IsGuestAsync(user) && !_orderSettings.AnonymousCheckoutAllowed)
                return Challenge();

            //model
            var model = await _checkoutModelFactory.PrepareBillingAddressModelAsync(cart, prePopulateNewAddressWithUserFields: true);

            //check whether "billing address" step is enabled
            if (_orderSettings.DisableBillingAddressCheckoutStep && model.ExistingAddresses.Any())
            {
                if (model.ExistingAddresses.Any())
                {
                    //choose the first one
                    return await SelectBillingAddress(model.ExistingAddresses.First().Id);
                }

                TryValidateModel(model);
                TryValidateModel(model.BillingNewAddress);
                return await NewBillingAddress(model, form);
            }

            return View(model);
        }

        public virtual async Task<IActionResult> SelectBillingAddress(int addressId, bool shipToSameAddress = false)
        {
            //validation
            if (_orderSettings.CheckoutDisabled)
                return RedirectToRoute("ShoppingCart");

            var user = await _workContext.GetCurrentUserAsync();
            var address = await _userService.GetUserAddressAsync(user.Id, addressId);

            if (address == null)
                return RedirectToRoute("CheckoutBillingAddress");

            user.BillingAddressId = address.Id;
            await _userService.UpdateUserAsync(user);

            var store = await _storeContext.GetCurrentStoreAsync();
            var cart = await _shoppingCartService.GetShoppingCartAsync(user, ShoppingCartType.ShoppingCart, store.Id);

            //ship to the same address?
            //by default Shipping is available if the country is not specified
            var shippingAllowed = !_addressSettings.CountryEnabled || ((await _countryService.GetCountryByAddressAsync(address))?.AllowsShipping ?? false);
            if (_shippingSettings.ShipToSameAddress && shipToSameAddress && await _shoppingCartService.ShoppingCartRequiresShippingAsync(cart) && shippingAllowed)
            {
                user.ShippingAddressId = user.BillingAddressId;
                await _userService.UpdateUserAsync(user);
                //reset selected shipping method (in case if "pick up in store" was selected)
                await _genericAttributeService.SaveAttributeAsync<ShippingOption>(user, TvProgUserDefaults.SelectedShippingOptionAttribute, null, store.Id);
                await _genericAttributeService.SaveAttributeAsync<PickupPoint>(user, TvProgUserDefaults.SelectedPickupPointAttribute, null, store.Id);
                //limitation - "Ship to the same address" doesn't properly work in "pick up in store only" case (when no shipping plugins are available) 
                return RedirectToRoute("CheckoutShippingMethod");
            }

            return RedirectToRoute("CheckoutShippingAddress");
        }

        [HttpPost, ActionName("BillingAddress")]
        [FormValueRequired("nextstep")]
        public virtual async Task<IActionResult> NewBillingAddress(CheckoutBillingAddressModel model, IFormCollection form)
        {
            //validation
            if (_orderSettings.CheckoutDisabled)
                return RedirectToRoute("ShoppingCart");

            var user = await _workContext.GetCurrentUserAsync();
            var store = await _storeContext.GetCurrentStoreAsync();
            var cart = await _shoppingCartService.GetShoppingCartAsync(user, ShoppingCartType.ShoppingCart, store.Id);

            if (!cart.Any())
                return RedirectToRoute("ShoppingCart");

            if (_orderSettings.OnePageCheckoutEnabled)
                return RedirectToRoute("CheckoutOnePage");

            if (await _userService.IsGuestAsync(user) && !_orderSettings.AnonymousCheckoutAllowed)
                return Challenge();

            if (await _userService.IsGuestAsync(user) && _taxSettings.EuVatEnabled && _taxSettings.EuVatEnabledForGuests)
            {
                var warning = await SaveUserVatNumberAsync(model.VatNumber, user);
                if (!string.IsNullOrEmpty(warning))
                    ModelState.AddModelError("", warning);
            }

            //custom address attributes
            var customAttributes = await _addressAttributeParser.ParseCustomAddressAttributesAsync(form);
            var customAttributeWarnings = await _addressAttributeParser.GetAttributeWarningsAsync(customAttributes);
            foreach (var error in customAttributeWarnings)
            {
                ModelState.AddModelError("", error);
            }

            var newAddress = model.BillingNewAddress;

            if (ModelState.IsValid)
            {
                //try to find an address with the same values (don't duplicate records)
                var address = _addressService.FindAddress((await _userService.GetAddressesByUserIdAsync(user.Id)).ToList(),
                    newAddress.FirstName, newAddress.LastName, newAddress.MiddleName, newAddress.PhoneNumber,
                    newAddress.Email, newAddress.FaxNumber, newAddress.Company,
                    newAddress.Address1, newAddress.Address2, newAddress.City,
                    newAddress.County, newAddress.StateProvinceId, newAddress.ZipPostalCode,
                    newAddress.CountryId, customAttributes);

                if (address == null)
                {
                    //address is not found. let's create a new one
                    address = newAddress.ToEntity();
                    address.CustomAttributes = customAttributes;
                    address.CreatedOnUtc = DateTime.UtcNow;

                    //some validation
                    if (address.CountryId == 0)
                        address.CountryId = null;
                    if (address.StateProvinceId == 0)
                        address.StateProvinceId = null;

                    await _addressService.InsertAddressAsync(address);

                    await _userService.InsertUserAddressAsync(user, address);
                }

                user.BillingAddressId = address.Id;

                await _userService.UpdateUserAsync(user);

                //ship to the same address?
                if (_shippingSettings.ShipToSameAddress && model.ShipToSameAddress && await _shoppingCartService.ShoppingCartRequiresShippingAsync(cart))
                {
                    user.ShippingAddressId = user.BillingAddressId;
                    await _userService.UpdateUserAsync(user);

                    //reset selected shipping method (in case if "pick up in store" was selected)
                    await _genericAttributeService.SaveAttributeAsync<ShippingOption>(user, TvProgUserDefaults.SelectedShippingOptionAttribute, null, store.Id);
                    await _genericAttributeService.SaveAttributeAsync<PickupPoint>(user, TvProgUserDefaults.SelectedPickupPointAttribute, null, store.Id);

                    //limitation - "Ship to the same address" doesn't properly work in "pick up in store only" case (when no shipping plugins are available) 
                    return RedirectToRoute("CheckoutShippingMethod");
                }

                return RedirectToRoute("CheckoutShippingAddress");
            }

            //If we got this far, something failed, redisplay form
            model = await _checkoutModelFactory.PrepareBillingAddressModelAsync(cart,
                selectedCountryId: newAddress.CountryId,
                overrideAttributesXml: customAttributes);
            return View(model);
        }

        public virtual async Task<IActionResult> ShippingAddress()
        {
            //validation
            if (_orderSettings.CheckoutDisabled)
                return RedirectToRoute("ShoppingCart");

            var user = await _workContext.GetCurrentUserAsync();
            var store = await _storeContext.GetCurrentStoreAsync();
            var cart = await _shoppingCartService.GetShoppingCartAsync(user, ShoppingCartType.ShoppingCart, store.Id);

            if (!cart.Any())
                return RedirectToRoute("ShoppingCart");

            if (_orderSettings.OnePageCheckoutEnabled)
                return RedirectToRoute("CheckoutOnePage");

            if (await _userService.IsGuestAsync(user) && !_orderSettings.AnonymousCheckoutAllowed)
                return Challenge();

            if (!await _shoppingCartService.ShoppingCartRequiresShippingAsync(cart))
                return RedirectToRoute("CheckoutShippingMethod");

            //model
            var model = await _checkoutModelFactory.PrepareShippingAddressModelAsync(cart, prePopulateNewAddressWithUserFields: true);
            return View(model);
        }

        public virtual async Task<IActionResult> SelectShippingAddress(int addressId)
        {
            //validation
            if (_orderSettings.CheckoutDisabled)
                return RedirectToRoute("ShoppingCart");

            var user = await _workContext.GetCurrentUserAsync();
            var address = await _userService.GetUserAddressAsync(user.Id, addressId);

            if (address == null)
                return RedirectToRoute("CheckoutShippingAddress");

            user.ShippingAddressId = address.Id;
            await _userService.UpdateUserAsync(user);

            if (_shippingSettings.AllowPickupInStore)
            {
                var store = await _storeContext.GetCurrentStoreAsync();
                //set value indicating that "pick up in store" option has not been chosen
                await _genericAttributeService.SaveAttributeAsync<PickupPoint>(user, TvProgUserDefaults.SelectedPickupPointAttribute, null, store.Id);
            }

            return RedirectToRoute("CheckoutShippingMethod");
        }

        [HttpPost, ActionName("ShippingAddress")]
        [FormValueRequired("nextstep")]
        public virtual async Task<IActionResult> NewShippingAddress(CheckoutShippingAddressModel model, IFormCollection form)
        {
            //validation
            if (_orderSettings.CheckoutDisabled)
                return RedirectToRoute("ShoppingCart");

            var user = await _workContext.GetCurrentUserAsync();
            var store = await _storeContext.GetCurrentStoreAsync();
            var cart = await _shoppingCartService.GetShoppingCartAsync(user, ShoppingCartType.ShoppingCart, store.Id);

            if (!cart.Any())
                return RedirectToRoute("ShoppingCart");

            if (_orderSettings.OnePageCheckoutEnabled)
                return RedirectToRoute("CheckoutOnePage");

            if (await _userService.IsGuestAsync(user) && !_orderSettings.AnonymousCheckoutAllowed)
                return Challenge();

            if (!await _shoppingCartService.ShoppingCartRequiresShippingAsync(cart))
                return RedirectToRoute("CheckoutShippingMethod");

            //pickup point
            if (_shippingSettings.AllowPickupInStore && !_orderSettings.DisplayPickupInStoreOnShippingMethodPage)
            {
                var pickupInStore = ParsePickupInStore(form);
                if (pickupInStore)
                {
                    var pickupOption = await ParsePickupOptionAsync(cart, form);
                    await SavePickupOptionAsync(pickupOption);

                    return RedirectToRoute("CheckoutPaymentMethod");
                }

                //set value indicating that "pick up in store" option has not been chosen
                await _genericAttributeService.SaveAttributeAsync<PickupPoint>(user, TvProgUserDefaults.SelectedPickupPointAttribute, null, store.Id);
            }

            //custom address attributes
            var customAttributes = await _addressAttributeParser.ParseCustomAddressAttributesAsync(form);
            var customAttributeWarnings = await _addressAttributeParser.GetAttributeWarningsAsync(customAttributes);
            foreach (var error in customAttributeWarnings)
            {
                ModelState.AddModelError("", error);
            }

            var newAddress = model.ShippingNewAddress;

            if (ModelState.IsValid)
            {
                //try to find an address with the same values (don't duplicate records)
                var address = _addressService.FindAddress((await _userService.GetAddressesByUserIdAsync(user.Id)).ToList(),
                    newAddress.FirstName, newAddress.LastName, newAddress.MiddleName, newAddress.PhoneNumber,
                    newAddress.Email, newAddress.FaxNumber, newAddress.Company,
                    newAddress.Address1, newAddress.Address2, newAddress.City,
                    newAddress.County, newAddress.StateProvinceId, newAddress.ZipPostalCode,
                    newAddress.CountryId, customAttributes);

                if (address == null)
                {
                    address = newAddress.ToEntity();
                    address.CustomAttributes = customAttributes;
                    address.CreatedOnUtc = DateTime.UtcNow;
                    //some validation
                    if (address.CountryId == 0)
                        address.CountryId = null;
                    if (address.StateProvinceId == 0)
                        address.StateProvinceId = null;

                    await _addressService.InsertAddressAsync(address);

                    await _userService.InsertUserAddressAsync(user, address);

                }

                user.ShippingAddressId = address.Id;
                await _userService.UpdateUserAsync(user);

                return RedirectToRoute("CheckoutShippingMethod");
            }

            //If we got this far, something failed, redisplay form
            model = await _checkoutModelFactory.PrepareShippingAddressModelAsync(cart,
                selectedCountryId: newAddress.CountryId,
                overrideAttributesXml: customAttributes);
            return View(model);
        }

        public virtual async Task<IActionResult> ShippingMethod()
        {
            //validation
            if (_orderSettings.CheckoutDisabled)
                return RedirectToRoute("ShoppingCart");

            var user = await _workContext.GetCurrentUserAsync();
            var store = await _storeContext.GetCurrentStoreAsync();
            var cart = await _shoppingCartService.GetShoppingCartAsync(user, ShoppingCartType.ShoppingCart, store.Id);

            if (!cart.Any())
                return RedirectToRoute("ShoppingCart");

            if (_orderSettings.OnePageCheckoutEnabled)
                return RedirectToRoute("CheckoutOnePage");

            if (await _userService.IsGuestAsync(user) && !_orderSettings.AnonymousCheckoutAllowed)
                return Challenge();

            if (!await _shoppingCartService.ShoppingCartRequiresShippingAsync(cart))
            {
                await _genericAttributeService.SaveAttributeAsync<ShippingOption>(user, TvProgUserDefaults.SelectedShippingOptionAttribute, null, store.Id);
                return RedirectToRoute("CheckoutPaymentMethod");
            }

            //check if pickup point is selected on the shipping address step
            if (!_orderSettings.DisplayPickupInStoreOnShippingMethodPage)
            {
                var selectedPickUpPoint = await _genericAttributeService
                    .GetAttributeAsync<PickupPoint>(user, TvProgUserDefaults.SelectedPickupPointAttribute, store.Id);
                if (selectedPickUpPoint != null)
                    return RedirectToRoute("CheckoutPaymentMethod");
            }

            //model
            var model = await _checkoutModelFactory.PrepareShippingMethodModelAsync(cart, await _userService.GetUserShippingAddressAsync(user));

            if (_shippingSettings.BypassShippingMethodSelectionIfOnlyOne &&
                model.ShippingMethods.Count == 1)
            {
                //if we have only one shipping method, then a user doesn't have to choose a shipping method
                await _genericAttributeService.SaveAttributeAsync(user,
                    TvProgUserDefaults.SelectedShippingOptionAttribute,
                    model.ShippingMethods.First().ShippingOption,
                    store.Id);

                return RedirectToRoute("CheckoutPaymentMethod");
            }

            return View(model);
        }

        [HttpPost, ActionName("ShippingMethod")]
        [FormValueRequired("nextstep")]
        public virtual async Task<IActionResult> SelectShippingMethod(string shippingoption, IFormCollection form)
        {
            //validation
            if (_orderSettings.CheckoutDisabled)
                return RedirectToRoute("ShoppingCart");

            var user = await _workContext.GetCurrentUserAsync();
            var store = await _storeContext.GetCurrentStoreAsync();
            var cart = await _shoppingCartService.GetShoppingCartAsync(user, ShoppingCartType.ShoppingCart, store.Id);

            if (!cart.Any())
                return RedirectToRoute("ShoppingCart");

            if (_orderSettings.OnePageCheckoutEnabled)
                return RedirectToRoute("CheckoutOnePage");

            if (await _userService.IsGuestAsync(user) && !_orderSettings.AnonymousCheckoutAllowed)
                return Challenge();

            if (!await _shoppingCartService.ShoppingCartRequiresShippingAsync(cart))
            {
                await _genericAttributeService.SaveAttributeAsync<ShippingOption>(user,
                    TvProgUserDefaults.SelectedShippingOptionAttribute, null, store.Id);
                return RedirectToRoute("CheckoutPaymentMethod");
            }

            //pickup point
            if (_shippingSettings.AllowPickupInStore && _orderSettings.DisplayPickupInStoreOnShippingMethodPage)
            {
                var pickupInStore = ParsePickupInStore(form);
                if (pickupInStore)
                {
                    var pickupOption = await ParsePickupOptionAsync(cart, form);
                    await SavePickupOptionAsync(pickupOption);

                    return RedirectToRoute("CheckoutPaymentMethod");
                }

                //set value indicating that "pick up in store" option has not been chosen
                await _genericAttributeService.SaveAttributeAsync<PickupPoint>(user, TvProgUserDefaults.SelectedPickupPointAttribute, null, store.Id);
            }

            //parse selected method 
            if (string.IsNullOrEmpty(shippingoption))
                return await ShippingMethod();
            var splittedOption = shippingoption.Split(new[] { "___" }, StringSplitOptions.RemoveEmptyEntries);
            if (splittedOption.Length != 2)
                return await ShippingMethod();
            var selectedName = splittedOption[0];
            var shippingRateComputationMethodSystemName = splittedOption[1];

            //find it
            //performance optimization. try cache first
            var shippingOptions = await _genericAttributeService.GetAttributeAsync<List<ShippingOption>>(user,
                TvProgUserDefaults.OfferedShippingOptionsAttribute, store.Id);
            if (shippingOptions == null || !shippingOptions.Any())
            {
                //not found? let's load them using shipping service
                shippingOptions = (await _shippingService.GetShippingOptionsAsync(cart, await _userService.GetUserShippingAddressAsync(user),
                    user, shippingRateComputationMethodSystemName, store.Id)).ShippingOptions.ToList();
            }
            else
            {
                //loaded cached results. let's filter result by a chosen shipping rate computation method
                shippingOptions = shippingOptions.Where(so => so.ShippingRateComputationMethodSystemName.Equals(shippingRateComputationMethodSystemName, StringComparison.InvariantCultureIgnoreCase))
                    .ToList();
            }

            var shippingOption = shippingOptions
                .Find(so => !string.IsNullOrEmpty(so.Name) && so.Name.Equals(selectedName, StringComparison.InvariantCultureIgnoreCase));
            if (shippingOption == null)
                return await ShippingMethod();

            //save
            await _genericAttributeService.SaveAttributeAsync(user, TvProgUserDefaults.SelectedShippingOptionAttribute, shippingOption, store.Id);

            return RedirectToRoute("CheckoutPaymentMethod");
        }

        public virtual async Task<IActionResult> PaymentMethod()
        {
            //validation
            if (_orderSettings.CheckoutDisabled)
                return RedirectToRoute("ShoppingCart");

            var user = await _workContext.GetCurrentUserAsync();
            var store = await _storeContext.GetCurrentStoreAsync();
            var cart = await _shoppingCartService.GetShoppingCartAsync(user, ShoppingCartType.ShoppingCart, store.Id);

            if (!cart.Any())
                return RedirectToRoute("ShoppingCart");

            if (_orderSettings.OnePageCheckoutEnabled)
                return RedirectToRoute("CheckoutOnePage");

            if (await _userService.IsGuestAsync(user) && !_orderSettings.AnonymousCheckoutAllowed)
                return Challenge();

            //Check whether payment workflow is required
            //we ignore reward points during cart total calculation
            var isPaymentWorkflowRequired = await _orderProcessingService.IsPaymentWorkflowRequiredAsync(cart, false);
            if (!isPaymentWorkflowRequired)
            {
                await _genericAttributeService.SaveAttributeAsync<string>(user,
                    TvProgUserDefaults.SelectedPaymentMethodAttribute, null, store.Id);
                return RedirectToRoute("CheckoutPaymentInfo");
            }

            //filter by country
            var filterByCountryId = 0;
            if (_addressSettings.CountryEnabled)
            {
                filterByCountryId = (await _userService.GetUserBillingAddressAsync(user))?.CountryId ?? 0;
            }

            //model
            var paymentMethodModel = await _checkoutModelFactory.PreparePaymentMethodModelAsync(cart, filterByCountryId);

            if (_paymentSettings.BypassPaymentMethodSelectionIfOnlyOne &&
                paymentMethodModel.PaymentMethods.Count == 1 && !paymentMethodModel.DisplayRewardPoints)
            {
                //if we have only one payment method and reward points are disabled or the current user doesn't have any reward points
                //so user doesn't have to choose a payment method

                await _genericAttributeService.SaveAttributeAsync(user,
                    TvProgUserDefaults.SelectedPaymentMethodAttribute,
                    paymentMethodModel.PaymentMethods[0].PaymentMethodSystemName,
                    store.Id);
                return RedirectToRoute("CheckoutPaymentInfo");
            }

            return View(paymentMethodModel);
        }

        [HttpPost, ActionName("PaymentMethod")]
        [FormValueRequired("nextstep")]
        public virtual async Task<IActionResult> SelectPaymentMethod(string paymentmethod, CheckoutPaymentMethodModel model)
        {
            //validation
            if (_orderSettings.CheckoutDisabled)
                return RedirectToRoute("ShoppingCart");

            var user = await _workContext.GetCurrentUserAsync();
            var store = await _storeContext.GetCurrentStoreAsync();
            var cart = await _shoppingCartService.GetShoppingCartAsync(user, ShoppingCartType.ShoppingCart, store.Id);

            if (!cart.Any())
                return RedirectToRoute("ShoppingCart");

            if (_orderSettings.OnePageCheckoutEnabled)
                return RedirectToRoute("CheckoutOnePage");

            if (await _userService.IsGuestAsync(user) && !_orderSettings.AnonymousCheckoutAllowed)
                return Challenge();

            //reward points
            if (_rewardPointsSettings.Enabled)
            {
                await _genericAttributeService.SaveAttributeAsync(user,
                    TvProgUserDefaults.UseRewardPointsDuringCheckoutAttribute, model.UseRewardPoints,
                    store.Id);
            }

            //Check whether payment workflow is required
            var isPaymentWorkflowRequired = await _orderProcessingService.IsPaymentWorkflowRequiredAsync(cart);
            if (!isPaymentWorkflowRequired)
            {
                await _genericAttributeService.SaveAttributeAsync<string>(user,
                    TvProgUserDefaults.SelectedPaymentMethodAttribute, null, store.Id);
                return RedirectToRoute("CheckoutPaymentInfo");
            }
            //payment method 
            if (string.IsNullOrEmpty(paymentmethod))
                return await PaymentMethod();

            if (!await _paymentPluginManager.IsPluginActiveAsync(paymentmethod, user, store.Id))
                return await PaymentMethod();

            //save
            await _genericAttributeService.SaveAttributeAsync(user,
                TvProgUserDefaults.SelectedPaymentMethodAttribute, paymentmethod, store.Id);

            return RedirectToRoute("CheckoutPaymentInfo");
        }

        public virtual async Task<IActionResult> PaymentInfo()
        {
            //validation
            if (_orderSettings.CheckoutDisabled)
                return RedirectToRoute("ShoppingCart");

            var user = await _workContext.GetCurrentUserAsync();
            var store = await _storeContext.GetCurrentStoreAsync();
            var cart = await _shoppingCartService.GetShoppingCartAsync(user, ShoppingCartType.ShoppingCart, store.Id);

            if (!cart.Any())
                return RedirectToRoute("ShoppingCart");

            if (_orderSettings.OnePageCheckoutEnabled)
                return RedirectToRoute("CheckoutOnePage");

            if (await _userService.IsGuestAsync(user) && !_orderSettings.AnonymousCheckoutAllowed)
                return Challenge();

            //Check whether payment workflow is required
            var isPaymentWorkflowRequired = await _orderProcessingService.IsPaymentWorkflowRequiredAsync(cart);
            if (!isPaymentWorkflowRequired)
            {
                return RedirectToRoute("CheckoutConfirm");
            }

            //load payment method
            var paymentMethodSystemName = await _genericAttributeService.GetAttributeAsync<string>(user,
                TvProgUserDefaults.SelectedPaymentMethodAttribute, store.Id);
            var paymentMethod = await _paymentPluginManager
                .LoadPluginBySystemNameAsync(paymentMethodSystemName, user, store.Id);
            if (paymentMethod == null)
                return RedirectToRoute("CheckoutPaymentMethod");

            //Check whether payment info should be skipped
            if (paymentMethod.SkipPaymentInfo ||
                (paymentMethod.PaymentMethodType == PaymentMethodType.Redirection && _paymentSettings.SkipPaymentInfoStepForRedirectionPaymentMethods))
            {
                //skip payment info page
                var paymentInfo = new ProcessPaymentRequest();

                //session save
                HttpContext.Session.Set("OrderPaymentInfo", paymentInfo);

                return RedirectToRoute("CheckoutConfirm");
            }

            //model
            var model = await _checkoutModelFactory.PreparePaymentInfoModelAsync(paymentMethod);
            return View(model);
        }

        [HttpPost, ActionName("PaymentInfo")]
        [FormValueRequired("nextstep")]
        public virtual async Task<IActionResult> EnterPaymentInfo(IFormCollection form)
        {
            //validation
            if (_orderSettings.CheckoutDisabled)
                return RedirectToRoute("ShoppingCart");

            var user = await _workContext.GetCurrentUserAsync();
            var store = await _storeContext.GetCurrentStoreAsync();
            var cart = await _shoppingCartService.GetShoppingCartAsync(user, ShoppingCartType.ShoppingCart, store.Id);

            if (!cart.Any())
                return RedirectToRoute("ShoppingCart");

            if (_orderSettings.OnePageCheckoutEnabled)
                return RedirectToRoute("CheckoutOnePage");

            if (await _userService.IsGuestAsync(user) && !_orderSettings.AnonymousCheckoutAllowed)
                return Challenge();

            //Check whether payment workflow is required
            var isPaymentWorkflowRequired = await _orderProcessingService.IsPaymentWorkflowRequiredAsync(cart);
            if (!isPaymentWorkflowRequired)
            {
                return RedirectToRoute("CheckoutConfirm");
            }

            //load payment method
            var paymentMethodSystemName = await _genericAttributeService.GetAttributeAsync<string>(user,
                TvProgUserDefaults.SelectedPaymentMethodAttribute, store.Id);
            var paymentMethod = await _paymentPluginManager
                .LoadPluginBySystemNameAsync(paymentMethodSystemName, user, store.Id);
            if (paymentMethod == null)
                return RedirectToRoute("CheckoutPaymentMethod");

            var warnings = await paymentMethod.ValidatePaymentFormAsync(form);
            foreach (var warning in warnings)
                ModelState.AddModelError("", warning);
            if (ModelState.IsValid)
            {
                //get payment info
                var paymentInfo = await paymentMethod.GetPaymentInfoAsync(form);
                //set previous order GUID (if exists)
                _paymentService.GenerateOrderGuid(paymentInfo);

                //session save
                HttpContext.Session.Set("OrderPaymentInfo", paymentInfo);
                return RedirectToRoute("CheckoutConfirm");
            }

            //If we got this far, something failed, redisplay form
            //model
            var model = await _checkoutModelFactory.PreparePaymentInfoModelAsync(paymentMethod);
            return View(model);
        }

        public virtual async Task<IActionResult> Confirm()
        {
            //validation
            if (_orderSettings.CheckoutDisabled)
                return RedirectToRoute("ShoppingCart");

            var user = await _workContext.GetCurrentUserAsync();
            var store = await _storeContext.GetCurrentStoreAsync();
            var cart = await _shoppingCartService.GetShoppingCartAsync(user, ShoppingCartType.ShoppingCart, store.Id);

            if (!cart.Any())
                return RedirectToRoute("ShoppingCart");

            if (_orderSettings.OnePageCheckoutEnabled)
                return RedirectToRoute("CheckoutOnePage");

            if (await _userService.IsGuestAsync(user) && !_orderSettings.AnonymousCheckoutAllowed)
                return Challenge();

            //model
            var model = await _checkoutModelFactory.PrepareConfirmOrderModelAsync(cart);
            return View(model);
        }

        [ValidateCaptcha]
        [HttpPost, ActionName("Confirm")]
        public virtual async Task<IActionResult> ConfirmOrder(bool captchaValid)
        {
            //validation
            if (_orderSettings.CheckoutDisabled)
                return RedirectToRoute("ShoppingCart");

            var user = await _workContext.GetCurrentUserAsync();
            var store = await _storeContext.GetCurrentStoreAsync();
            var cart = await _shoppingCartService.GetShoppingCartAsync(user, ShoppingCartType.ShoppingCart, store.Id);

            if (!cart.Any())
                return RedirectToRoute("ShoppingCart");

            if (_orderSettings.OnePageCheckoutEnabled)
                return RedirectToRoute("CheckoutOnePage");

            if (await _userService.IsGuestAsync(user) && !_orderSettings.AnonymousCheckoutAllowed)
                return Challenge();

            //model
            var model = await _checkoutModelFactory.PrepareConfirmOrderModelAsync(cart);

            var isCaptchaSettingEnabled = await _userService.IsGuestAsync(user) &&
                _captchaSettings.Enabled && _captchaSettings.ShowOnCheckoutPageForGuests;

            //captcha validation for guest users
            if (isCaptchaSettingEnabled && !captchaValid)
            {
                model.Warnings.Add(await _localizationService.GetResourceAsync("Common.WrongCaptchaMessage"));
                return View(model);
            }

            try
            {
                //prevent 2 orders being placed within an X seconds time frame
                if (!await IsMinimumOrderPlacementIntervalValidAsync(user))
                    throw new Exception(await _localizationService.GetResourceAsync("Checkout.MinOrderPlacementInterval"));

                //place order
                var processPaymentRequest = HttpContext.Session.Get<ProcessPaymentRequest>("OrderPaymentInfo");
                if (processPaymentRequest == null)
                {
                    //Check whether payment workflow is required
                    if (await _orderProcessingService.IsPaymentWorkflowRequiredAsync(cart))
                        return RedirectToRoute("CheckoutPaymentInfo");

                    processPaymentRequest = new ProcessPaymentRequest();
                }
                _paymentService.GenerateOrderGuid(processPaymentRequest);
                processPaymentRequest.StoreId = store.Id;
                processPaymentRequest.UserId = user.Id;
                processPaymentRequest.PaymentMethodSystemName = await _genericAttributeService.GetAttributeAsync<string>(user,
                    TvProgUserDefaults.SelectedPaymentMethodAttribute, store.Id);
                HttpContext.Session.Set<ProcessPaymentRequest>("OrderPaymentInfo", processPaymentRequest);
                var placeOrderResult = await _orderProcessingService.PlaceOrderAsync(processPaymentRequest);
                if (placeOrderResult.Success)
                {
                    HttpContext.Session.Set<ProcessPaymentRequest>("OrderPaymentInfo", null);
                    var postProcessPaymentRequest = new PostProcessPaymentRequest
                    {
                        Order = placeOrderResult.PlacedOrder
                    };
                    await _paymentService.PostProcessPaymentAsync(postProcessPaymentRequest);

                    if (_webHelper.IsRequestBeingRedirected || _webHelper.IsPostBeingDone)
                    {
                        //redirection or POST has been done in PostProcessPayment
                        return Content(await _localizationService.GetResourceAsync("Checkout.RedirectMessage"));
                    }

                    return RedirectToRoute("CheckoutCompleted", new { orderId = placeOrderResult.PlacedOrder.Id });
                }

                foreach (var error in placeOrderResult.Errors)
                    model.Warnings.Add(error);
            }
            catch (Exception exc)
            {
                await _logger.WarningAsync(exc.Message, exc);
                model.Warnings.Add(exc.Message);
            }

            //If we got this far, something failed, redisplay form
            return View(model);
        }

        #endregion

        #region Methods (one page checkout)

        protected virtual async Task<JsonResult> OpcLoadStepAfterShippingAddress(IList<ShoppingCartItem> cart)
        {
            var user = await _workContext.GetCurrentUserAsync();
            var shippingMethodModel = await _checkoutModelFactory.PrepareShippingMethodModelAsync(cart, await _userService.GetUserShippingAddressAsync(user));
            if (_shippingSettings.BypassShippingMethodSelectionIfOnlyOne &&
                shippingMethodModel.ShippingMethods.Count == 1)
            {
                var store = await _storeContext.GetCurrentStoreAsync();
                //if we have only one shipping method, then a user doesn't have to choose a shipping method
                await _genericAttributeService.SaveAttributeAsync(user,
                    TvProgUserDefaults.SelectedShippingOptionAttribute,
                    shippingMethodModel.ShippingMethods.First().ShippingOption,
                    store.Id);

                //load next step
                return await OpcLoadStepAfterShippingMethod(cart);
            }

            return Json(new
            {
                update_section = new UpdateSectionJsonModel
                {
                    name = "shipping-method",
                    html = await RenderPartialViewToStringAsync("OpcShippingMethods", shippingMethodModel)
                },
                goto_section = "shipping_method"
            });
        }

        protected virtual async Task<JsonResult> OpcLoadStepAfterShippingMethod(IList<ShoppingCartItem> cart)
        {
            //Check whether payment workflow is required
            //we ignore reward points during cart total calculation
            var user = await _workContext.GetCurrentUserAsync();
            var store = await _storeContext.GetCurrentStoreAsync();
            var isPaymentWorkflowRequired = await _orderProcessingService.IsPaymentWorkflowRequiredAsync(cart, false);
            if (isPaymentWorkflowRequired)
            {
                //filter by country
                var filterByCountryId = 0;
                if (_addressSettings.CountryEnabled)
                {
                    filterByCountryId = (await _userService.GetUserBillingAddressAsync(user))?.CountryId ?? 0;
                }

                //payment is required
                var paymentMethodModel = await _checkoutModelFactory.PreparePaymentMethodModelAsync(cart, filterByCountryId);

                if (_paymentSettings.BypassPaymentMethodSelectionIfOnlyOne &&
                    paymentMethodModel.PaymentMethods.Count == 1 && !paymentMethodModel.DisplayRewardPoints)
                {
                    //if we have only one payment method and reward points are disabled or the current user doesn't have any reward points
                    //so user doesn't have to choose a payment method

                    var selectedPaymentMethodSystemName = paymentMethodModel.PaymentMethods[0].PaymentMethodSystemName;
                    await _genericAttributeService.SaveAttributeAsync(user,
                        TvProgUserDefaults.SelectedPaymentMethodAttribute,
                        selectedPaymentMethodSystemName, store.Id);

                    var paymentMethodInst = await _paymentPluginManager
                        .LoadPluginBySystemNameAsync(selectedPaymentMethodSystemName, user, store.Id);
                    if (!_paymentPluginManager.IsPluginActive(paymentMethodInst))
                        throw new Exception("Selected payment method can't be parsed");

                    return await OpcLoadStepAfterPaymentMethod(paymentMethodInst, cart);
                }

                //user have to choose a payment method
                return Json(new
                {
                    update_section = new UpdateSectionJsonModel
                    {
                        name = "payment-method",
                        html = await RenderPartialViewToStringAsync("OpcPaymentMethods", paymentMethodModel)
                    },
                    goto_section = "payment_method"
                });
            }

            //payment is not required
            await _genericAttributeService.SaveAttributeAsync<string>(user,
                TvProgUserDefaults.SelectedPaymentMethodAttribute, null, store.Id);

            var confirmOrderModel = await _checkoutModelFactory.PrepareConfirmOrderModelAsync(cart);
            return Json(new
            {
                update_section = new UpdateSectionJsonModel
                {
                    name = "confirm-order",
                    html = await RenderPartialViewToStringAsync("OpcConfirmOrder", confirmOrderModel)
                },
                goto_section = "confirm_order"
            });
        }

        protected virtual async Task<JsonResult> OpcLoadStepAfterPaymentMethod(IPaymentMethod paymentMethod, IList<ShoppingCartItem> cart)
        {
            if (paymentMethod.SkipPaymentInfo ||
                (paymentMethod.PaymentMethodType == PaymentMethodType.Redirection && _paymentSettings.SkipPaymentInfoStepForRedirectionPaymentMethods))
            {
                //skip payment info page
                var paymentInfo = new ProcessPaymentRequest();

                //session save
                HttpContext.Session.Set("OrderPaymentInfo", paymentInfo);

                var confirmOrderModel = await _checkoutModelFactory.PrepareConfirmOrderModelAsync(cart);
                return Json(new
                {
                    update_section = new UpdateSectionJsonModel
                    {
                        name = "confirm-order",
                        html = await RenderPartialViewToStringAsync("OpcConfirmOrder", confirmOrderModel)
                    },
                    goto_section = "confirm_order"
                });
            }

            //return payment info page
            var paymenInfoModel = await _checkoutModelFactory.PreparePaymentInfoModelAsync(paymentMethod);
            return Json(new
            {
                update_section = new UpdateSectionJsonModel
                {
                    name = "payment-info",
                    html = await RenderPartialViewToStringAsync("OpcPaymentInfo", paymenInfoModel)
                },
                goto_section = "payment_info"
            });
        }

        public virtual async Task<IActionResult> OnePageCheckout()
        {
            //validation
            if (_orderSettings.CheckoutDisabled)
                return RedirectToRoute("ShoppingCart");

            var user = await _workContext.GetCurrentUserAsync();
            var store = await _storeContext.GetCurrentStoreAsync();
            var cart = await _shoppingCartService.GetShoppingCartAsync(user, ShoppingCartType.ShoppingCart, store.Id);

            if (!cart.Any())
                return RedirectToRoute("ShoppingCart");

            if (!_orderSettings.OnePageCheckoutEnabled)
                return RedirectToRoute("Checkout");

            if (await _userService.IsGuestAsync(user) && !_orderSettings.AnonymousCheckoutAllowed)
                return Challenge();

            var model = await _checkoutModelFactory.PrepareOnePageCheckoutModelAsync(cart);
            return View(model);
        }

        [HttpPost]
        public virtual async Task<IActionResult> OpcSaveBilling(CheckoutBillingAddressModel model, IFormCollection form)
        {
            try
            {
                //validation
                if (_orderSettings.CheckoutDisabled)
                    throw new Exception(await _localizationService.GetResourceAsync("Checkout.Disabled"));

                var user = await _workContext.GetCurrentUserAsync();
                var store = await _storeContext.GetCurrentStoreAsync();
                var cart = await _shoppingCartService.GetShoppingCartAsync(user, ShoppingCartType.ShoppingCart, store.Id);

                if (!cart.Any())
                    throw new Exception("Your cart is empty");

                if (!_orderSettings.OnePageCheckoutEnabled)
                    throw new Exception("One page checkout is disabled");

                if (await _userService.IsGuestAsync(user) && !_orderSettings.AnonymousCheckoutAllowed)
                    throw new Exception("Anonymous checkout is not allowed");

                _ = int.TryParse(form["billing_address_id"], out var billingAddressId);

                if (billingAddressId > 0)
                {
                    //existing address
                    var address = await _userService.GetUserAddressAsync(user.Id, billingAddressId)
                        ?? throw new Exception(await _localizationService.GetResourceAsync("Checkout.Address.NotFound"));

                    user.BillingAddressId = address.Id;
                    await _userService.UpdateUserAsync(user);
                }
                else
                {
                    if (await _userService.IsGuestAsync(user) && _taxSettings.EuVatEnabled && _taxSettings.EuVatEnabledForGuests)
                    {
                        var warning = await SaveUserVatNumberAsync(model.VatNumber, user);
                        if (!string.IsNullOrEmpty(warning))
                            ModelState.AddModelError("", warning);
                    }

                    //new address
                    var newAddress = model.BillingNewAddress;

                    //custom address attributes
                    var customAttributes = await _addressAttributeParser.ParseCustomAddressAttributesAsync(form);
                    var customAttributeWarnings = await _addressAttributeParser.GetAttributeWarningsAsync(customAttributes);
                    foreach (var error in customAttributeWarnings)
                    {
                        ModelState.AddModelError("", error);
                    }

                    //validate model
                    if (!ModelState.IsValid)
                    {
                        //model is not valid. redisplay the form with errors
                        var billingAddressModel = await _checkoutModelFactory.PrepareBillingAddressModelAsync(cart,
                            selectedCountryId: newAddress.CountryId,
                            overrideAttributesXml: customAttributes);
                        billingAddressModel.NewAddressPreselected = true;
                        return Json(new
                        {
                            update_section = new UpdateSectionJsonModel
                            {
                                name = "billing",
                                html = await RenderPartialViewToStringAsync("OpcBillingAddress", billingAddressModel)
                            },
                            wrong_billing_address = true,
                        });
                    }

                    //try to find an address with the same values (don't duplicate records)
                    var address = _addressService.FindAddress((await _userService.GetAddressesByUserIdAsync(user.Id)).ToList(),
                        newAddress.FirstName, newAddress.LastName, newAddress.MiddleName, newAddress.PhoneNumber,
                        newAddress.Email, newAddress.FaxNumber, newAddress.Company,
                        newAddress.Address1, newAddress.Address2, newAddress.City,
                        newAddress.County, newAddress.StateProvinceId, newAddress.ZipPostalCode,
                        newAddress.CountryId, customAttributes);

                    if (address == null)
                    {
                        //address is not found. let's create a new one
                        address = newAddress.ToEntity();
                        address.CustomAttributes = customAttributes;
                        address.CreatedOnUtc = DateTime.UtcNow;

                        //some validation
                        if (address.CountryId == 0)
                            address.CountryId = null;

                        if (address.StateProvinceId == 0)
                            address.StateProvinceId = null;

                        await _addressService.InsertAddressAsync(address);

                        await _userService.InsertUserAddressAsync(user, address);
                    }

                    user.BillingAddressId = address.Id;

                    await _userService.UpdateUserAsync(user);
                }

                if (await _shoppingCartService.ShoppingCartRequiresShippingAsync(cart))
                {
                    //shipping is required
                    var address = await _userService.GetUserBillingAddressAsync(user);

                    //by default Shipping is available if the country is not specified
                    var shippingAllowed = !_addressSettings.CountryEnabled || ((await _countryService.GetCountryByAddressAsync(address))?.AllowsShipping ?? false);
                    if (_shippingSettings.ShipToSameAddress && model.ShipToSameAddress && shippingAllowed)
                    {
                        //ship to the same address
                        user.ShippingAddressId = address.Id;
                        await _userService.UpdateUserAsync(user);
                        //reset selected shipping method (in case if "pick up in store" was selected)
                        await _genericAttributeService.SaveAttributeAsync<ShippingOption>(user, TvProgUserDefaults.SelectedShippingOptionAttribute, null, store.Id);
                        await _genericAttributeService.SaveAttributeAsync<PickupPoint>(user, TvProgUserDefaults.SelectedPickupPointAttribute, null, store.Id);
                        //limitation - "Ship to the same address" doesn't properly work in "pick up in store only" case (when no shipping plugins are available) 
                        return await OpcLoadStepAfterShippingAddress(cart);
                    }

                    //do not ship to the same address
                    var shippingAddressModel = await _checkoutModelFactory.PrepareShippingAddressModelAsync(cart, prePopulateNewAddressWithUserFields: true);

                    return Json(new
                    {
                        update_section = new UpdateSectionJsonModel
                        {
                            name = "shipping",
                            html = await RenderPartialViewToStringAsync("OpcShippingAddress", shippingAddressModel)
                        },
                        goto_section = "shipping"
                    });
                }

                //shipping is not required
                await _genericAttributeService.SaveAttributeAsync<ShippingOption>(user, TvProgUserDefaults.SelectedShippingOptionAttribute, null, store.Id);

                //load next step
                return await OpcLoadStepAfterShippingMethod(cart);
            }
            catch (Exception exc)
            {
                await _logger.WarningAsync(exc.Message, exc, await _workContext.GetCurrentUserAsync());
                return Json(new { error = 1, message = exc.Message });
            }
        }

        [HttpPost]
        public virtual async Task<IActionResult> OpcSaveShipping(CheckoutShippingAddressModel model, IFormCollection form)
        {
            try
            {
                //validation
                if (_orderSettings.CheckoutDisabled)
                    throw new Exception(await _localizationService.GetResourceAsync("Checkout.Disabled"));

                var user = await _workContext.GetCurrentUserAsync();
                var store = await _storeContext.GetCurrentStoreAsync();
                var cart = await _shoppingCartService.GetShoppingCartAsync(user, ShoppingCartType.ShoppingCart, store.Id);

                if (!cart.Any())
                    throw new Exception("Your cart is empty");

                if (!_orderSettings.OnePageCheckoutEnabled)
                    throw new Exception("One page checkout is disabled");

                if (await _userService.IsGuestAsync(user) && !_orderSettings.AnonymousCheckoutAllowed)
                    throw new Exception("Anonymous checkout is not allowed");

                if (!await _shoppingCartService.ShoppingCartRequiresShippingAsync(cart))
                    throw new Exception("Shipping is not required");

                //pickup point
                if (_shippingSettings.AllowPickupInStore && !_orderSettings.DisplayPickupInStoreOnShippingMethodPage)
                {
                    var pickupInStore = ParsePickupInStore(form);
                    if (pickupInStore)
                    {
                        var pickupOption = await ParsePickupOptionAsync(cart, form);
                        await SavePickupOptionAsync(pickupOption);

                        return await OpcLoadStepAfterShippingMethod(cart);
                    }

                    //set value indicating that "pick up in store" option has not been chosen
                    await _genericAttributeService.SaveAttributeAsync<PickupPoint>(user, TvProgUserDefaults.SelectedPickupPointAttribute, null, store.Id);
                }

                _ = int.TryParse(form["shipping_address_id"], out var shippingAddressId);

                if (shippingAddressId > 0)
                {
                    //existing address
                    var address = await _userService.GetUserAddressAsync(user.Id, shippingAddressId)
                        ?? throw new Exception(await _localizationService.GetResourceAsync("Checkout.Address.NotFound"));

                    user.ShippingAddressId = address.Id;
                    await _userService.UpdateUserAsync(user);
                }
                else
                {
                    //new address
                    var newAddress = model.ShippingNewAddress;

                    //custom address attributes
                    var customAttributes = await _addressAttributeParser.ParseCustomAddressAttributesAsync(form);
                    var customAttributeWarnings = await _addressAttributeParser.GetAttributeWarningsAsync(customAttributes);
                    foreach (var error in customAttributeWarnings)
                    {
                        ModelState.AddModelError("", error);
                    }

                    //validate model
                    if (!ModelState.IsValid)
                    {
                        //model is not valid. redisplay the form with errors
                        var shippingAddressModel = await _checkoutModelFactory.PrepareShippingAddressModelAsync(cart,
                            selectedCountryId: newAddress.CountryId,
                            overrideAttributesXml: customAttributes);
                        shippingAddressModel.NewAddressPreselected = true;
                        return Json(new
                        {
                            update_section = new UpdateSectionJsonModel
                            {
                                name = "shipping",
                                html = await RenderPartialViewToStringAsync("OpcShippingAddress", shippingAddressModel)
                            }
                        });
                    }

                    //try to find an address with the same values (don't duplicate records)
                    var address = _addressService.FindAddress((await _userService.GetAddressesByUserIdAsync(user.Id)).ToList(),
                        newAddress.FirstName, newAddress.LastName, newAddress.MiddleName, newAddress.PhoneNumber,
                        newAddress.Email, newAddress.FaxNumber, newAddress.Company,
                        newAddress.Address1, newAddress.Address2, newAddress.City,
                        newAddress.County, newAddress.StateProvinceId, newAddress.ZipPostalCode,
                        newAddress.CountryId, customAttributes);

                    if (address == null)
                    {
                        address = newAddress.ToEntity();
                        address.CustomAttributes = customAttributes;
                        address.CreatedOnUtc = DateTime.UtcNow;

                        await _addressService.InsertAddressAsync(address);

                        await _userService.InsertUserAddressAsync(user, address);
                    }

                    user.ShippingAddressId = address.Id;

                    await _userService.UpdateUserAsync(user);
                }

                return await OpcLoadStepAfterShippingAddress(cart);
            }
            catch (Exception exc)
            {
                await _logger.WarningAsync(exc.Message, exc, await _workContext.GetCurrentUserAsync());
                return Json(new { error = 1, message = exc.Message });
            }
        }

        [HttpPost]
        public virtual async Task<IActionResult> OpcSaveShippingMethod(string shippingoption, IFormCollection form)
        {
            try
            {
                //validation
                if (_orderSettings.CheckoutDisabled)
                    throw new Exception(await _localizationService.GetResourceAsync("Checkout.Disabled"));

                var user = await _workContext.GetCurrentUserAsync();
                var store = await _storeContext.GetCurrentStoreAsync();
                var cart = await _shoppingCartService.GetShoppingCartAsync(user, ShoppingCartType.ShoppingCart, store.Id);

                if (!cart.Any())
                    throw new Exception("Your cart is empty");

                if (!_orderSettings.OnePageCheckoutEnabled)
                    throw new Exception("One page checkout is disabled");

                if (await _userService.IsGuestAsync(user) && !_orderSettings.AnonymousCheckoutAllowed)
                    throw new Exception("Anonymous checkout is not allowed");

                if (!await _shoppingCartService.ShoppingCartRequiresShippingAsync(cart))
                    throw new Exception("Shipping is not required");

                //pickup point
                if (_shippingSettings.AllowPickupInStore && _orderSettings.DisplayPickupInStoreOnShippingMethodPage)
                {
                    var pickupInStore = ParsePickupInStore(form);
                    if (pickupInStore)
                    {
                        var pickupOption = await ParsePickupOptionAsync(cart, form);
                        await SavePickupOptionAsync(pickupOption);

                        return await OpcLoadStepAfterShippingMethod(cart);
                    }

                    //set value indicating that "pick up in store" option has not been chosen
                    await _genericAttributeService.SaveAttributeAsync<PickupPoint>(user, TvProgUserDefaults.SelectedPickupPointAttribute, null, store.Id);
                }

                //parse selected method 
                if (string.IsNullOrEmpty(shippingoption))
                    throw new Exception("Selected shipping method can't be parsed");
                var splittedOption = shippingoption.Split(new[] { "___" }, StringSplitOptions.RemoveEmptyEntries);
                if (splittedOption.Length != 2)
                    throw new Exception("Selected shipping method can't be parsed");
                var selectedName = splittedOption[0];
                var shippingRateComputationMethodSystemName = splittedOption[1];

                //find it
                //performance optimization. try cache first
                var shippingOptions = await _genericAttributeService.GetAttributeAsync<List<ShippingOption>>(user,
                    TvProgUserDefaults.OfferedShippingOptionsAttribute, store.Id);
                if (shippingOptions == null || !shippingOptions.Any())
                {
                    //not found? let's load them using shipping service
                    shippingOptions = (await _shippingService.GetShippingOptionsAsync(cart, await _userService.GetUserShippingAddressAsync(user),
                        user, shippingRateComputationMethodSystemName, store.Id)).ShippingOptions.ToList();
                }
                else
                {
                    //loaded cached results. let's filter result by a chosen shipping rate computation method
                    shippingOptions = shippingOptions.Where(so => so.ShippingRateComputationMethodSystemName.Equals(shippingRateComputationMethodSystemName, StringComparison.InvariantCultureIgnoreCase))
                        .ToList();
                }

                var shippingOption = shippingOptions
                    .Find(so => !string.IsNullOrEmpty(so.Name) && so.Name.Equals(selectedName, StringComparison.InvariantCultureIgnoreCase));
                if (shippingOption == null)
                    throw new Exception("Selected shipping method can't be loaded");

                //save
                await _genericAttributeService.SaveAttributeAsync(user, TvProgUserDefaults.SelectedShippingOptionAttribute, shippingOption, store.Id);

                //load next step
                return await OpcLoadStepAfterShippingMethod(cart);
            }
            catch (Exception exc)
            {
                await _logger.WarningAsync(exc.Message, exc, await _workContext.GetCurrentUserAsync());
                return Json(new { error = 1, message = exc.Message });
            }
        }

        [HttpPost]
        public virtual async Task<IActionResult> OpcSavePaymentMethod(string paymentmethod, CheckoutPaymentMethodModel model)
        {
            try
            {
                //validation
                if (_orderSettings.CheckoutDisabled)
                    throw new Exception(await _localizationService.GetResourceAsync("Checkout.Disabled"));

                var user = await _workContext.GetCurrentUserAsync();
                var store = await _storeContext.GetCurrentStoreAsync();
                var cart = await _shoppingCartService.GetShoppingCartAsync(user, ShoppingCartType.ShoppingCart, store.Id);

                if (!cart.Any())
                    throw new Exception("Your cart is empty");

                if (!_orderSettings.OnePageCheckoutEnabled)
                    throw new Exception("One page checkout is disabled");

                if (await _userService.IsGuestAsync(user) && !_orderSettings.AnonymousCheckoutAllowed)
                    throw new Exception("Anonymous checkout is not allowed");

                //payment method 
                if (string.IsNullOrEmpty(paymentmethod))
                    throw new Exception("Selected payment method can't be parsed");

                //reward points
                if (_rewardPointsSettings.Enabled)
                {
                    await _genericAttributeService.SaveAttributeAsync(user,
                        TvProgUserDefaults.UseRewardPointsDuringCheckoutAttribute, model.UseRewardPoints,
                        store.Id);
                }

                //Check whether payment workflow is required
                var isPaymentWorkflowRequired = await _orderProcessingService.IsPaymentWorkflowRequiredAsync(cart);
                if (!isPaymentWorkflowRequired)
                {
                    //payment is not required
                    await _genericAttributeService.SaveAttributeAsync<string>(user,
                        TvProgUserDefaults.SelectedPaymentMethodAttribute, null, store.Id);

                    var confirmOrderModel = await _checkoutModelFactory.PrepareConfirmOrderModelAsync(cart);
                    return Json(new
                    {
                        update_section = new UpdateSectionJsonModel
                        {
                            name = "confirm-order",
                            html = await RenderPartialViewToStringAsync("OpcConfirmOrder", confirmOrderModel)
                        },
                        goto_section = "confirm_order"
                    });
                }

                var paymentMethodInst = await _paymentPluginManager
                    .LoadPluginBySystemNameAsync(paymentmethod, user, store.Id);
                if (!_paymentPluginManager.IsPluginActive(paymentMethodInst))
                    throw new Exception("Selected payment method can't be parsed");

                //save
                await _genericAttributeService.SaveAttributeAsync(user,
                    TvProgUserDefaults.SelectedPaymentMethodAttribute, paymentmethod, store.Id);

                return await OpcLoadStepAfterPaymentMethod(paymentMethodInst, cart);
            }
            catch (Exception exc)
            {
                await _logger.WarningAsync(exc.Message, exc, await _workContext.GetCurrentUserAsync());
                return Json(new { error = 1, message = exc.Message });
            }
        }

        [HttpPost]
        [IgnoreAntiforgeryToken]
        public virtual async Task<IActionResult> OpcSavePaymentInfo(IFormCollection form)
        {
            try
            {
                //validation
                if (_orderSettings.CheckoutDisabled)
                    throw new Exception(await _localizationService.GetResourceAsync("Checkout.Disabled"));

                var user = await _workContext.GetCurrentUserAsync();
                var store = await _storeContext.GetCurrentStoreAsync();
                var cart = await _shoppingCartService.GetShoppingCartAsync(user, ShoppingCartType.ShoppingCart, store.Id);

                if (!cart.Any())
                    throw new Exception("Your cart is empty");

                if (!_orderSettings.OnePageCheckoutEnabled)
                    throw new Exception("One page checkout is disabled");

                if (await _userService.IsGuestAsync(user) && !_orderSettings.AnonymousCheckoutAllowed)
                    throw new Exception("Anonymous checkout is not allowed");

                var paymentMethodSystemName = await _genericAttributeService.GetAttributeAsync<string>(user,
                    TvProgUserDefaults.SelectedPaymentMethodAttribute, store.Id);
                var paymentMethod = await _paymentPluginManager
                    .LoadPluginBySystemNameAsync(paymentMethodSystemName, user, store.Id)
                    ?? throw new Exception("Payment method is not selected");

                var warnings = await paymentMethod.ValidatePaymentFormAsync(form);
                foreach (var warning in warnings)
                    ModelState.AddModelError("", warning);
                if (ModelState.IsValid)
                {
                    //get payment info
                    var paymentInfo = await paymentMethod.GetPaymentInfoAsync(form);
                    //set previous order GUID (if exists)
                    _paymentService.GenerateOrderGuid(paymentInfo);

                    //session save
                    HttpContext.Session.Set("OrderPaymentInfo", paymentInfo);

                    var confirmOrderModel = await _checkoutModelFactory.PrepareConfirmOrderModelAsync(cart);
                    return Json(new
                    {
                        update_section = new UpdateSectionJsonModel
                        {
                            name = "confirm-order",
                            html = await RenderPartialViewToStringAsync("OpcConfirmOrder", confirmOrderModel)
                        },
                        goto_section = "confirm_order"
                    });
                }

                //If we got this far, something failed, redisplay form
                var paymenInfoModel = await _checkoutModelFactory.PreparePaymentInfoModelAsync(paymentMethod);
                return Json(new
                {
                    update_section = new UpdateSectionJsonModel
                    {
                        name = "payment-info",
                        html = await RenderPartialViewToStringAsync("OpcPaymentInfo", paymenInfoModel)
                    }
                });
            }
            catch (Exception exc)
            {
                await _logger.WarningAsync(exc.Message, exc, await _workContext.GetCurrentUserAsync());
                return Json(new { error = 1, message = exc.Message });
            }
        }

        [ValidateCaptcha]
        [HttpPost]
        public virtual async Task<IActionResult> OpcConfirmOrder(bool captchaValid)
        {
            try
            {
                var user = await _workContext.GetCurrentUserAsync();

                var isCaptchaSettingEnabled = await _userService.IsGuestAsync(user) && 
                    _captchaSettings.Enabled && _captchaSettings.ShowOnCheckoutPageForGuests;

                var confirmOrderModel = new CheckoutConfirmModel()
                { 
                    DisplayCaptcha = isCaptchaSettingEnabled
                };

                //captcha validation for guest users
                if (!isCaptchaSettingEnabled || (isCaptchaSettingEnabled && captchaValid))
                {
                    //validation
                    if (_orderSettings.CheckoutDisabled)
                        throw new Exception(await _localizationService.GetResourceAsync("Checkout.Disabled"));

                    var store = await _storeContext.GetCurrentStoreAsync();
                    var cart = await _shoppingCartService.GetShoppingCartAsync(user, ShoppingCartType.ShoppingCart, store.Id);

                    if (!cart.Any())
                        throw new Exception("Your cart is empty");

                    if (!_orderSettings.OnePageCheckoutEnabled)
                        throw new Exception("One page checkout is disabled");

                    if (await _userService.IsGuestAsync(user) && !_orderSettings.AnonymousCheckoutAllowed)
                        throw new Exception("Anonymous checkout is not allowed");

                    //prevent 2 orders being placed within an X seconds time frame
                    if (!await IsMinimumOrderPlacementIntervalValidAsync(user))
                        throw new Exception(await _localizationService.GetResourceAsync("Checkout.MinOrderPlacementInterval"));

                    //place order
                    var processPaymentRequest = HttpContext.Session.Get<ProcessPaymentRequest>("OrderPaymentInfo");
                    if (processPaymentRequest == null)
                    {
                        //Check whether payment workflow is required
                        if (await _orderProcessingService.IsPaymentWorkflowRequiredAsync(cart))
                        {
                            throw new Exception("Payment information is not entered");
                        }

                        processPaymentRequest = new ProcessPaymentRequest();
                    }
                    _paymentService.GenerateOrderGuid(processPaymentRequest);
                    processPaymentRequest.StoreId = store.Id;
                    processPaymentRequest.UserId = user.Id;
                    processPaymentRequest.PaymentMethodSystemName = await _genericAttributeService.GetAttributeAsync<string>(user,
                        TvProgUserDefaults.SelectedPaymentMethodAttribute, store.Id);
                    HttpContext.Session.Set<ProcessPaymentRequest>("OrderPaymentInfo", processPaymentRequest);
                    var placeOrderResult = await _orderProcessingService.PlaceOrderAsync(processPaymentRequest);
                    if (placeOrderResult.Success)
                    {
                        HttpContext.Session.Set<ProcessPaymentRequest>("OrderPaymentInfo", null);
                        var postProcessPaymentRequest = new PostProcessPaymentRequest
                        {
                            Order = placeOrderResult.PlacedOrder
                        };

                        var paymentMethod = await _paymentPluginManager
                            .LoadPluginBySystemNameAsync(placeOrderResult.PlacedOrder.PaymentMethodSystemName, user, store.Id);
                        if (paymentMethod == null)
                            //payment method could be null if order total is 0
                            //success
                            return Json(new { success = 1 });

                        if (paymentMethod.PaymentMethodType == PaymentMethodType.Redirection)
                        {
                            //Redirection will not work because it's AJAX request.
                            //That's why we don't process it here (we redirect a user to another page where he'll be redirected)

                            //redirect
                            return Json(new
                            {
                                redirect = $"{_webHelper.GetStoreLocation()}checkout/OpcCompleteRedirectionPayment"
                            });
                        }

                        await _paymentService.PostProcessPaymentAsync(postProcessPaymentRequest);
                        //success
                        return Json(new { success = 1 });
                    }

                    //error
                    foreach (var error in placeOrderResult.Errors)
                        confirmOrderModel.Warnings.Add(error);
                }
                else
                {
                    confirmOrderModel.Warnings.Add(await _localizationService.GetResourceAsync("Common.WrongCaptchaMessage"));
                }

                return Json(new
                {
                    update_section = new UpdateSectionJsonModel
                    {
                        name = "confirm-order",
                        html = await RenderPartialViewToStringAsync("OpcConfirmOrder", confirmOrderModel)
                    },
                    goto_section = "confirm_order"
                });
            }
            catch (Exception exc)
            {
                await _logger.WarningAsync(exc.Message, exc, await _workContext.GetCurrentUserAsync());
                return Json(new { error = 1, message = exc.Message });
            }
        }

        public virtual async Task<IActionResult> OpcCompleteRedirectionPayment()
        {
            try
            {
                //validation
                if (!_orderSettings.OnePageCheckoutEnabled)
                    return RedirectToRoute("Homepage");

                var user = await _workContext.GetCurrentUserAsync();
                if (await _userService.IsGuestAsync(user) && !_orderSettings.AnonymousCheckoutAllowed)
                    return Challenge();

                //get the order
                var store = await _storeContext.GetCurrentStoreAsync();
                var order = (await _orderService.SearchOrdersAsync(storeId: store.Id,
                userId: user.Id, pageSize: 1)).FirstOrDefault();
                if (order == null)
                    return RedirectToRoute("Homepage");

                var paymentMethod = await _paymentPluginManager
                    .LoadPluginBySystemNameAsync(order.PaymentMethodSystemName, user, store.Id);
                if (paymentMethod == null)
                    return RedirectToRoute("Homepage");
                if (paymentMethod.PaymentMethodType != PaymentMethodType.Redirection)
                    return RedirectToRoute("Homepage");

                //ensure that order has been just placed
                if ((DateTime.UtcNow - order.CreatedOnUtc).TotalMinutes > 3)
                    return RedirectToRoute("Homepage");

                //Redirection will not work on one page checkout page because it's AJAX request.
                //That's why we process it here
                var postProcessPaymentRequest = new PostProcessPaymentRequest
                {
                    Order = order
                };

                await _paymentService.PostProcessPaymentAsync(postProcessPaymentRequest);

                if (_webHelper.IsRequestBeingRedirected || _webHelper.IsPostBeingDone)
                {
                    //redirection or POST has been done in PostProcessPayment
                    return Content(await _localizationService.GetResourceAsync("Checkout.RedirectMessage"));
                }

                //if no redirection has been done (to a third-party payment page)
                //theoretically it's not possible
                return RedirectToRoute("CheckoutCompleted", new { orderId = order.Id });
            }
            catch (Exception exc)
            {
                await _logger.WarningAsync(exc.Message, exc, await _workContext.GetCurrentUserAsync());
                return Content(exc.Message);
            }
        }

        #endregion
    }
}
