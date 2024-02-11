using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Primitives;
using TvProgViewer.Core;
using TvProgViewer.Core.Caching;
using TvProgViewer.Core.Domain.Catalog;
using TvProgViewer.Core.Domain.Common;
using TvProgViewer.Core.Domain.Users;
using TvProgViewer.Core.Domain.Media;
using TvProgViewer.Core.Domain.Orders;
using TvProgViewer.Core.Domain.Security;
using TvProgViewer.Core.Domain.Shipping;
using TvProgViewer.Core.Infrastructure;
using TvProgViewer.Services.Catalog;
using TvProgViewer.Services.Common;
using TvProgViewer.Services.Users;
using TvProgViewer.Services.Directory;
using TvProgViewer.Services.Discounts;
using TvProgViewer.Services.Html;
using TvProgViewer.Services.Localization;
using TvProgViewer.Services.Logging;
using TvProgViewer.Services.Media;
using TvProgViewer.Services.Messages;
using TvProgViewer.Services.Orders;
using TvProgViewer.Services.Security;
using TvProgViewer.Services.Seo;
using TvProgViewer.Services.Shipping;
using TvProgViewer.Services.Tax;
using TvProgViewer.WebUI.Components;
using TvProgViewer.WebUI.Factories;
using TvProgViewer.Web.Framework.Controllers;
using TvProgViewer.Web.Framework.Mvc;
using TvProgViewer.Web.Framework.Mvc.Filters;
using TvProgViewer.Web.Framework.Mvc.Routing;
using TvProgViewer.WebUI.Infrastructure.Cache;
using TvProgViewer.WebUI.Models.Media;
using TvProgViewer.WebUI.Models.ShoppingCart;

namespace TvProgViewer.WebUI.Controllers
{
    [AutoValidateAntiforgeryToken]
    public partial class ShoppingCartController : BasePublicController
    {
        #region Fields

        private readonly CaptchaSettings _captchaSettings;
        private readonly UserSettings _userSettings;
        private readonly ICheckoutAttributeParser _checkoutAttributeParser;
        private readonly ICheckoutAttributeService _checkoutAttributeService;
        private readonly ICurrencyService _currencyService;
        private readonly IUserActivityService _userActivityService;
        private readonly IUserService _userService;
        private readonly IDiscountService _discountService;
        private readonly IDownloadService _downloadService;
        private readonly IGenericAttributeService _genericAttributeService;
        private readonly IGiftCardService _giftCardService;
        private readonly IHtmlFormatter _htmlFormatter;
        private readonly ILocalizationService _localizationService;
        private readonly ITvProgFileProvider _fileProvider;
        private readonly ITvProgUrlHelper _nopUrlHelper;
        private readonly INotificationService _notificationService;
        private readonly IPermissionService _permissionService;
        private readonly IPictureService _pictureService;
        private readonly IPriceFormatter _priceFormatter;
        private readonly ITvChannelAttributeParser _tvchannelAttributeParser;
        private readonly ITvChannelAttributeService _tvchannelAttributeService;
        private readonly ITvChannelService _tvchannelService;
        private readonly IShippingService _shippingService;
        private readonly IShoppingCartModelFactory _shoppingCartModelFactory;
        private readonly IShoppingCartService _shoppingCartService;
        private readonly IStaticCacheManager _staticCacheManager;
        private readonly IStoreContext _storeContext;
        private readonly ITaxService _taxService;
        private readonly IUrlRecordService _urlRecordService;
        private readonly IWebHelper _webHelper;
        private readonly IWorkContext _workContext;
        private readonly IWorkflowMessageService _workflowMessageService;
        private readonly MediaSettings _mediaSettings;
        private readonly OrderSettings _orderSettings;
        private readonly ShoppingCartSettings _shoppingCartSettings;
        private readonly ShippingSettings _shippingSettings;

        #endregion

        #region Ctor

        public ShoppingCartController(CaptchaSettings captchaSettings,
            UserSettings userSettings,
            ICheckoutAttributeParser checkoutAttributeParser,
            ICheckoutAttributeService checkoutAttributeService,
            ICurrencyService currencyService,
            IUserActivityService userActivityService,
            IUserService userService,
            IDiscountService discountService,
            IDownloadService downloadService,
            IGenericAttributeService genericAttributeService,
            IGiftCardService giftCardService,
            IHtmlFormatter htmlFormatter,
            ILocalizationService localizationService,
            ITvProgFileProvider fileProvider,
            ITvProgUrlHelper nopUrlHelper,
            INotificationService notificationService,
            IPermissionService permissionService,
            IPictureService pictureService,
            IPriceFormatter priceFormatter,
            ITvChannelAttributeParser tvchannelAttributeParser,
            ITvChannelAttributeService tvchannelAttributeService,
            ITvChannelService tvchannelService,
            IShippingService shippingService,
            IShoppingCartModelFactory shoppingCartModelFactory,
            IShoppingCartService shoppingCartService,
            IStaticCacheManager staticCacheManager,
            IStoreContext storeContext,
            ITaxService taxService,
            IUrlRecordService urlRecordService,
            IWebHelper webHelper,
            IWorkContext workContext,
            IWorkflowMessageService workflowMessageService,
            MediaSettings mediaSettings,
            OrderSettings orderSettings,
            ShoppingCartSettings shoppingCartSettings,
            ShippingSettings shippingSettings)
        {
            _captchaSettings = captchaSettings;
            _userSettings = userSettings;
            _checkoutAttributeParser = checkoutAttributeParser;
            _checkoutAttributeService = checkoutAttributeService;
            _currencyService = currencyService;
            _userActivityService = userActivityService;
            _userService = userService;
            _discountService = discountService;
            _downloadService = downloadService;
            _genericAttributeService = genericAttributeService;
            _giftCardService = giftCardService;
            _htmlFormatter = htmlFormatter;
            _localizationService = localizationService;
            _fileProvider = fileProvider;
            _nopUrlHelper = nopUrlHelper;
            _notificationService = notificationService;
            _permissionService = permissionService;
            _pictureService = pictureService;
            _priceFormatter = priceFormatter;
            _tvchannelAttributeParser = tvchannelAttributeParser;
            _tvchannelAttributeService = tvchannelAttributeService;
            _tvchannelService = tvchannelService;
            _shippingService = shippingService;
            _shoppingCartModelFactory = shoppingCartModelFactory;
            _shoppingCartService = shoppingCartService;
            _staticCacheManager = staticCacheManager;
            _storeContext = storeContext;
            _taxService = taxService;
            _urlRecordService = urlRecordService;
            _webHelper = webHelper;
            _workContext = workContext;
            _workflowMessageService = workflowMessageService;
            _mediaSettings = mediaSettings;
            _orderSettings = orderSettings;
            _shoppingCartSettings = shoppingCartSettings;
            _shippingSettings = shippingSettings;
        }

        #endregion

        #region Utilities

        protected virtual async Task ParseAndSaveCheckoutAttributesAsync(IList<ShoppingCartItem> cart, IFormCollection form)
        {
            if (cart == null)
                throw new ArgumentNullException(nameof(cart));

            if (form == null)
                throw new ArgumentNullException(nameof(form));

            var attributesXml = string.Empty;
            var excludeShippableAttributes = !await _shoppingCartService.ShoppingCartRequiresShippingAsync(cart);
            var store = await _storeContext.GetCurrentStoreAsync();
            var checkoutAttributes = await _checkoutAttributeService.GetAllCheckoutAttributesAsync(store.Id, excludeShippableAttributes);
            foreach (var attribute in checkoutAttributes)
            {
                var controlId = $"checkout_attribute_{attribute.Id}";
                switch (attribute.AttributeControlType)
                {
                    case AttributeControlType.DropdownList:
                    case AttributeControlType.RadioList:
                    case AttributeControlType.ColorSquares:
                    case AttributeControlType.ImageSquares:
                        {
                            var ctrlAttributes = form[controlId];
                            if (!StringValues.IsNullOrEmpty(ctrlAttributes))
                            {
                                var selectedAttributeId = int.Parse(ctrlAttributes);
                                if (selectedAttributeId > 0)
                                    attributesXml = _checkoutAttributeParser.AddCheckoutAttribute(attributesXml,
                                        attribute, selectedAttributeId.ToString());
                            }
                        }

                        break;
                    case AttributeControlType.Checkboxes:
                        {
                            var cblAttributes = form[controlId];
                            if (!StringValues.IsNullOrEmpty(cblAttributes))
                            {
                                foreach (var item in cblAttributes.ToString().Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
                                {
                                    var selectedAttributeId = int.Parse(item);
                                    if (selectedAttributeId > 0)
                                        attributesXml = _checkoutAttributeParser.AddCheckoutAttribute(attributesXml,
                                            attribute, selectedAttributeId.ToString());
                                }
                            }
                        }

                        break;
                    case AttributeControlType.ReadonlyCheckboxes:
                        {
                            //load read-only (already server-side selected) values
                            var attributeValues = await _checkoutAttributeService.GetCheckoutAttributeValuesAsync(attribute.Id);
                            foreach (var selectedAttributeId in attributeValues
                                .Where(v => v.IsPreSelected)
                                .Select(v => v.Id)
                                .ToList())
                            {
                                attributesXml = _checkoutAttributeParser.AddCheckoutAttribute(attributesXml,
                                            attribute, selectedAttributeId.ToString());
                            }
                        }

                        break;
                    case AttributeControlType.TextBox:
                    case AttributeControlType.MultilineTextbox:
                        {
                            var ctrlAttributes = form[controlId];
                            if (!StringValues.IsNullOrEmpty(ctrlAttributes))
                            {
                                var enteredText = ctrlAttributes.ToString().Trim();
                                attributesXml = _checkoutAttributeParser.AddCheckoutAttribute(attributesXml,
                                    attribute, enteredText);
                            }
                        }

                        break;
                    case AttributeControlType.Datepicker:
                        {
                            var date = form[controlId + "_day"];
                            var month = form[controlId + "_month"];
                            var year = form[controlId + "_year"];
                            DateTime? selectedDate = null;
                            try
                            {
                                selectedDate = new DateTime(int.Parse(year), int.Parse(month), int.Parse(date));
                            }
                            catch
                            {
                                // ignored
                            }

                            if (selectedDate.HasValue)
                                attributesXml = _checkoutAttributeParser.AddCheckoutAttribute(attributesXml,
                                    attribute, selectedDate.Value.ToString("D"));
                        }

                        break;
                    case AttributeControlType.FileUpload:
                        {
                            _ = Guid.TryParse(form[controlId], out var downloadGuid);
                            var download = await _downloadService.GetDownloadByGuidAsync(downloadGuid);
                            if (download != null)
                            {
                                attributesXml = _checkoutAttributeParser.AddCheckoutAttribute(attributesXml,
                                           attribute, download.DownloadGuid.ToString());
                            }
                        }

                        break;
                    default:
                        break;
                }
            }

            //validate conditional attributes (if specified)
            foreach (var attribute in checkoutAttributes)
            {
                var conditionMet = await _checkoutAttributeParser.IsConditionMetAsync(attribute, attributesXml);
                if (conditionMet.HasValue && !conditionMet.Value)
                    attributesXml = _checkoutAttributeParser.RemoveCheckoutAttribute(attributesXml, attribute);
            }

            //save checkout attributes
            await _genericAttributeService.SaveAttributeAsync(await _workContext.GetCurrentUserAsync(), TvProgUserDefaults.CheckoutAttributes, attributesXml, store.Id);
        }

        protected virtual async Task SaveItemAsync(ShoppingCartItem updatecartitem, List<string> addToCartWarnings, TvChannel tvchannel,
           ShoppingCartType cartType, string attributes, decimal userEnteredPriceConverted, DateTime? rentalStartDate,
           DateTime? rentalEndDate, int quantity)
        {
            var user = await _workContext.GetCurrentUserAsync();
            var store = await _storeContext.GetCurrentStoreAsync();
            if (updatecartitem == null)
            {
                //add to the cart
                addToCartWarnings.AddRange(await _shoppingCartService.AddToCartAsync(user,
                    tvchannel, cartType, store.Id,
                    attributes, userEnteredPriceConverted,
                    rentalStartDate, rentalEndDate, quantity, true));
            }
            else
            {
                var cart = await _shoppingCartService.GetShoppingCartAsync(user, updatecartitem.ShoppingCartType, store.Id);

                var otherCartItemWithSameParameters = await _shoppingCartService.FindShoppingCartItemInTheCartAsync(
                    cart, updatecartitem.ShoppingCartType, tvchannel, attributes, userEnteredPriceConverted,
                    rentalStartDate, rentalEndDate);
                if (otherCartItemWithSameParameters != null &&
                    otherCartItemWithSameParameters.Id == updatecartitem.Id)
                {
                    //ensure it's some other shopping cart item
                    otherCartItemWithSameParameters = null;
                }
                //update existing item
                addToCartWarnings.AddRange(await _shoppingCartService.UpdateShoppingCartItemAsync(user,
                    updatecartitem.Id, attributes, userEnteredPriceConverted,
                    rentalStartDate, rentalEndDate, quantity + (otherCartItemWithSameParameters?.Quantity ?? 0), true));
                if (otherCartItemWithSameParameters != null && !addToCartWarnings.Any())
                {
                    //delete the same shopping cart item (the other one)
                    await _shoppingCartService.DeleteShoppingCartItemAsync(otherCartItemWithSameParameters);
                }
            }
        }

        protected virtual async Task<IActionResult> GetTvChannelToCartDetailsAsync(List<string> addToCartWarnings, ShoppingCartType cartType,
            TvChannel tvchannel)
        {
            if (addToCartWarnings.Any())
            {
                //cannot be added to the cart/wishlist
                //let's display warnings
                return Json(new
                {
                    success = false,
                    message = addToCartWarnings.ToArray()
                });
            }

            //added to the cart/wishlist
            var user = await _workContext.GetCurrentUserAsync();
            var store = await _storeContext.GetCurrentStoreAsync();
            switch (cartType)
            {
                case ShoppingCartType.Wishlist:
                    {
                        //activity log
                        await _userActivityService.InsertActivityAsync("PublicStore.AddToWishlist",
                            string.Format(await _localizationService.GetResourceAsync("ActivityLog.PublicStore.AddToWishlist"), tvchannel.Name), tvchannel);

                        if (_shoppingCartSettings.DisplayWishlistAfterAddingTvChannel)
                        {
                            //redirect to the wishlist page
                            return Json(new
                            {
                                redirect = Url.RouteUrl("Wishlist")
                            });
                        }

                        //display notification message and update appropriate blocks
                        var shoppingCarts = await _shoppingCartService.GetShoppingCartAsync(user, ShoppingCartType.Wishlist, store.Id);

                        var updateTopWishlistSectionHtml = string.Format(
                            await _localizationService.GetResourceAsync("Wishlist.HeaderQuantity"),
                            shoppingCarts.Sum(item => item.Quantity));

                        return Json(new
                        {
                            success = true,
                            message = string.Format(
                                await _localizationService.GetResourceAsync("TvChannels.TvChannelHasBeenAddedToTheWishlist.Link"),
                                Url.RouteUrl("Wishlist")),
                            updatetopwishlistsectionhtml = updateTopWishlistSectionHtml
                        });
                    }

                case ShoppingCartType.ShoppingCart:
                default:
                    {
                        //activity log
                        await _userActivityService.InsertActivityAsync("PublicStore.AddToShoppingCart",
                            string.Format(await _localizationService.GetResourceAsync("ActivityLog.PublicStore.AddToShoppingCart"), tvchannel.Name), tvchannel);

                        if (_shoppingCartSettings.DisplayCartAfterAddingTvChannel)
                        {
                            //redirect to the shopping cart page
                            return Json(new
                            {
                                redirect = Url.RouteUrl("ShoppingCart")
                            });
                        }

                        //display notification message and update appropriate blocks
                        var shoppingCarts = await _shoppingCartService.GetShoppingCartAsync(user, ShoppingCartType.ShoppingCart, store.Id);

                        var updateTopCartSectionHtml = string.Format(
                            await _localizationService.GetResourceAsync("ShoppingCart.HeaderQuantity"),
                            shoppingCarts.Sum(item => item.Quantity));

                        var updateFlyoutCartSectionHtml = _shoppingCartSettings.MiniShoppingCartEnabled
                            ? await RenderViewComponentToStringAsync(typeof(FlyoutShoppingCartViewComponent))
                            : string.Empty;

                        return Json(new
                        {
                            success = true,
                            message = string.Format(await _localizationService.GetResourceAsync("TvChannels.TvChannelHasBeenAddedToTheCart.Link"),
                                Url.RouteUrl("ShoppingCart")),
                            updatetopcartsectionhtml = updateTopCartSectionHtml,
                            updateflyoutcartsectionhtml = updateFlyoutCartSectionHtml
                        });
                    }
            }
        }

        #endregion

        #region Shopping cart

        [HttpPost]
        public virtual async Task<IActionResult> SelectShippingOption([FromQuery] string name, [FromQuery] EstimateShippingModel model, IFormCollection form)
        {
            if (model == null)
                model = new EstimateShippingModel();

            var errors = new List<string>();
            if (string.IsNullOrEmpty(model.ZipPostalCode) && !_shippingSettings.EstimateShippingCityNameEnabled)
                errors.Add(await _localizationService.GetResourceAsync("Shipping.EstimateShipping.ZipPostalCode.Required"));

            if (model.CountryId == null || model.CountryId == 0)
                errors.Add(await _localizationService.GetResourceAsync("Shipping.EstimateShipping.Country.Required"));

            if (errors.Count > 0)
                return Json(new
                {
                    success = false,
                    errors = errors
                });

            var user = await _workContext.GetCurrentUserAsync();
            var store = await _storeContext.GetCurrentStoreAsync();
            var cart = await _shoppingCartService.GetShoppingCartAsync(user, ShoppingCartType.ShoppingCart, store.Id);
            //parse and save checkout attributes
            await ParseAndSaveCheckoutAttributesAsync(cart, form);

            var shippingOptions = new List<ShippingOption>();
            ShippingOption selectedShippingOption = null;

            if (!string.IsNullOrWhiteSpace(name))
            {
                //find shipping options
                //performance optimization. try cache first
                shippingOptions = await _genericAttributeService.GetAttributeAsync<List<ShippingOption>>(user,
                    TvProgUserDefaults.OfferedShippingOptionsAttribute, store.Id);

                if (shippingOptions == null || !shippingOptions.Any())
                {
                    var address = new Address
                    {
                        CountryId = model.CountryId,
                        StateProvinceId = model.StateProvinceId,
                        ZipPostalCode = model.ZipPostalCode,
                    };

                    //not found? let's load them using shipping service
                    var getShippingOptionResponse = await _shippingService.GetShippingOptionsAsync(cart, address,
                        user, storeId: store.Id);

                    if (getShippingOptionResponse.Success)
                        shippingOptions = getShippingOptionResponse.ShippingOptions.ToList();
                    else
                        foreach (var error in getShippingOptionResponse.Errors)
                            errors.Add(error);
                }
            }

            selectedShippingOption = shippingOptions.Find(so => !string.IsNullOrEmpty(so.Name) && so.Name.Equals(name, StringComparison.InvariantCultureIgnoreCase));
            if (selectedShippingOption == null)
                errors.Add(await _localizationService.GetResourceAsync("Shipping.EstimateShippingPopUp.ShippingOption.IsNotFound"));

            if (errors.Count > 0)
                return Json(new
                {
                    success = false,
                    errors = errors
                });

            //reset pickup point
            await _genericAttributeService.SaveAttributeAsync<PickupPoint>(user,
                TvProgUserDefaults.SelectedPickupPointAttribute, null, store.Id);

            //cache shipping option
            await _genericAttributeService.SaveAttributeAsync(user,
                TvProgUserDefaults.SelectedShippingOptionAttribute, selectedShippingOption, store.Id);

            var orderTotalsSectionHtml = await RenderViewComponentToStringAsync(typeof(OrderTotalsViewComponent), new { isEditable = true });

            return Json(new
            {
                success = true,
                ordertotalssectionhtml = orderTotalsSectionHtml
            });
        }

        //add tvchannel to cart using AJAX
        //currently we use this method on catalog pages (category/manufacturer/etc)
        [HttpPost]
        public virtual async Task<IActionResult> AddTvChannelToCart_Catalog(int tvchannelId, int shoppingCartTypeId,
            int quantity, bool forceredirection = false)
        {
            var cartType = (ShoppingCartType)shoppingCartTypeId;

            var tvchannel = await _tvchannelService.GetTvChannelByIdAsync(tvchannelId);
            if (tvchannel == null)
                //no tvchannel found
                return Json(new
                {
                    success = false,
                    message = "No tvchannel found with the specified ID"
                });

            var redirectUrl = await _nopUrlHelper.RouteGenericUrlAsync<TvChannel>(new { SeName = await _urlRecordService.GetSeNameAsync(tvchannel) });

            //we can add only simple tvchannels
            if (tvchannel.TvChannelType != TvChannelType.SimpleTvChannel)
                return Json(new { redirect = redirectUrl });

            //tvchannels with "minimum order quantity" more than a specified qty
            if (tvchannel.OrderMinimumQuantity > quantity)
            {
                //we cannot add to the cart such tvchannels from category pages
                //it can confuse users. That's why we redirect users to the tvchannel details page
                return Json(new { redirect = redirectUrl });
            }

            if (tvchannel.UserEntersPrice)
            {
                //cannot be added to the cart (requires a user to enter price)
                return Json(new { redirect = redirectUrl });
            }

            if (tvchannel.IsRental)
            {
                //rental tvchannels require start/end dates to be entered
                return Json(new { redirect = redirectUrl });
            }

            var allowedQuantities = _tvchannelService.ParseAllowedQuantities(tvchannel);
            if (allowedQuantities.Length > 0)
            {
                //cannot be added to the cart (requires a user to select a quantity from dropdownlist)
                return Json(new { redirect = redirectUrl });
            }

            //allow a tvchannel to be added to the cart when all attributes are with "read-only checkboxes" type
            var tvchannelAttributes = await _tvchannelAttributeService.GetTvChannelAttributeMappingsByTvChannelIdAsync(tvchannel.Id);
            if (tvchannelAttributes.Any(pam => pam.AttributeControlType != AttributeControlType.ReadonlyCheckboxes))
            {
                //tvchannel has some attributes. let a user see them
                return Json(new { redirect = redirectUrl });
            }

            //creating XML for "read-only checkboxes" attributes
            var attXml = await tvchannelAttributes.AggregateAwaitAsync(string.Empty, async (attributesXml, attribute) =>
            {
                var attributeValues = await _tvchannelAttributeService.GetTvChannelAttributeValuesAsync(attribute.Id);
                foreach (var selectedAttributeId in attributeValues
                    .Where(v => v.IsPreSelected)
                    .Select(v => v.Id)
                    .ToList())
                {
                    attributesXml = _tvchannelAttributeParser.AddTvChannelAttribute(attributesXml,
                        attribute, selectedAttributeId.ToString());
                }

                return attributesXml;
            });

            //get standard warnings without attribute validations
            //first, try to find existing shopping cart item
            var user = await _workContext.GetCurrentUserAsync();
            var store = await _storeContext.GetCurrentStoreAsync();
            var cart = await _shoppingCartService.GetShoppingCartAsync(user, cartType, store.Id);
            var shoppingCartItem = await _shoppingCartService.FindShoppingCartItemInTheCartAsync(cart, cartType, tvchannel);
            //if we already have the same tvchannel in the cart, then use the total quantity to validate
            var quantityToValidate = shoppingCartItem != null ? shoppingCartItem.Quantity + quantity : quantity;
            var addToCartWarnings = await _shoppingCartService
                .GetShoppingCartItemWarningsAsync(user, cartType,
                tvchannel, store.Id, string.Empty,
                decimal.Zero, null, null, quantityToValidate, false, shoppingCartItem?.Id ?? 0, true, false, false, false);
            if (addToCartWarnings.Any())
            {
                //cannot be added to the cart
                //let's display standard warnings
                return Json(new
                {
                    success = false,
                    message = addToCartWarnings.ToArray()
                });
            }

            //now let's try adding tvchannel to the cart (now including tvchannel attribute validation, etc)
            addToCartWarnings = await _shoppingCartService.AddToCartAsync(user: user,
                tvchannel: tvchannel,
                shoppingCartType: cartType,
                storeId: store.Id,
                attributesXml: attXml,
                quantity: quantity);
            if (addToCartWarnings.Any())
            {
                //cannot be added to the cart
                //but we do not display attribute and gift card warnings here. let's do it on the tvchannel details page
                return Json(new { redirect = redirectUrl });
            }

            //added to the cart/wishlist
            switch (cartType)
            {
                case ShoppingCartType.Wishlist:
                    {
                        //activity log
                        await _userActivityService.InsertActivityAsync("PublicStore.AddToWishlist",
                            string.Format(await _localizationService.GetResourceAsync("ActivityLog.PublicStore.AddToWishlist"), tvchannel.Name), tvchannel);

                        if (_shoppingCartSettings.DisplayWishlistAfterAddingTvChannel || forceredirection)
                        {
                            //redirect to the wishlist page
                            return Json(new
                            {
                                redirect = Url.RouteUrl("Wishlist")
                            });
                        }

                        //display notification message and update appropriate blocks
                        var shoppingCarts = await _shoppingCartService.GetShoppingCartAsync(user, ShoppingCartType.Wishlist, store.Id);

                        var updatetopwishlistsectionhtml = string.Format(await _localizationService.GetResourceAsync("Wishlist.HeaderQuantity"),
                            shoppingCarts.Sum(item => item.Quantity));
                        return Json(new
                        {
                            success = true,
                            message = string.Format(await _localizationService.GetResourceAsync("TvChannels.TvChannelHasBeenAddedToTheWishlist.Link"), Url.RouteUrl("Wishlist")),
                            updatetopwishlistsectionhtml
                        });
                    }

                case ShoppingCartType.ShoppingCart:
                default:
                    {
                        //activity log
                        await _userActivityService.InsertActivityAsync("PublicStore.AddToShoppingCart",
                            string.Format(await _localizationService.GetResourceAsync("ActivityLog.PublicStore.AddToShoppingCart"), tvchannel.Name), tvchannel);

                        if (_shoppingCartSettings.DisplayCartAfterAddingTvChannel || forceredirection)
                        {
                            //redirect to the shopping cart page
                            return Json(new
                            {
                                redirect = Url.RouteUrl("ShoppingCart")
                            });
                        }

                        //display notification message and update appropriate blocks
                        var shoppingCarts = await _shoppingCartService.GetShoppingCartAsync(user, ShoppingCartType.ShoppingCart, store.Id);

                        var updatetopcartsectionhtml = string.Format(await _localizationService.GetResourceAsync("ShoppingCart.HeaderQuantity"),
                            shoppingCarts.Sum(item => item.Quantity));

                        var updateflyoutcartsectionhtml = _shoppingCartSettings.MiniShoppingCartEnabled
                            ? await RenderViewComponentToStringAsync(typeof(FlyoutShoppingCartViewComponent))
                            : string.Empty;

                        return Json(new
                        {
                            success = true,
                            message = string.Format(await _localizationService.GetResourceAsync("TvChannels.TvChannelHasBeenAddedToTheCart.Link"), Url.RouteUrl("ShoppingCart")),
                            updatetopcartsectionhtml,
                            updateflyoutcartsectionhtml
                        });
                    }
            }
        }

        //add tvchannel to cart using AJAX
        //currently we use this method on the tvchannel details pages
        [HttpPost]
        public virtual async Task<IActionResult> AddTvChannelToCart_Details(int tvchannelId, int shoppingCartTypeId, IFormCollection form)
        {
            var tvchannel = await _tvchannelService.GetTvChannelByIdAsync(tvchannelId);
            if (tvchannel == null)
            {
                return Json(new
                {
                    redirect = Url.RouteUrl("Homepage")
                });
            }

            //we can add only simple tvchannels
            if (tvchannel.TvChannelType != TvChannelType.SimpleTvChannel)
            {
                return Json(new
                {
                    success = false,
                    message = "Only simple tvchannels could be added to the cart"
                });
            }

            //update existing shopping cart item
            var updatecartitemid = 0;
            foreach (var formKey in form.Keys)
                if (formKey.Equals($"addtocart_{tvchannelId}.UpdatedShoppingCartItemId", StringComparison.InvariantCultureIgnoreCase))
                {
                    _ = int.TryParse(form[formKey], out updatecartitemid);
                    break;
                }

            ShoppingCartItem updatecartitem = null;
            if (_shoppingCartSettings.AllowCartItemEditing && updatecartitemid > 0)
            {
                var store = await _storeContext.GetCurrentStoreAsync();
                //search with the same cart type as specified
                var cart = await _shoppingCartService.GetShoppingCartAsync(await _workContext.GetCurrentUserAsync(), (ShoppingCartType)shoppingCartTypeId, store.Id);

                updatecartitem = cart.FirstOrDefault(x => x.Id == updatecartitemid);
                //not found? let's ignore it. in this case we'll add a new item
                //if (updatecartitem == null)
                //{
                //    return Json(new
                //    {
                //        success = false,
                //        message = "No shopping cart item found to update"
                //    });
                //}
                //is it this tvchannel?
                if (updatecartitem != null && tvchannel.Id != updatecartitem.TvChannelId)
                {
                    return Json(new
                    {
                        success = false,
                        message = "This tvchannel does not match a passed shopping cart item identifier"
                    });
                }
            }

            var addToCartWarnings = new List<string>();

            //user entered price
            var userEnteredPriceConverted = await _tvchannelAttributeParser.ParseUserEnteredPriceAsync(tvchannel, form);

            //entered quantity
            var quantity = _tvchannelAttributeParser.ParseEnteredQuantity(tvchannel, form);

            //tvchannel and gift card attributes
            var attributes = await _tvchannelAttributeParser.ParseTvChannelAttributesAsync(tvchannel, form, addToCartWarnings);

            //rental attributes
            _tvchannelAttributeParser.ParseRentalDates(tvchannel, form, out var rentalStartDate, out var rentalEndDate);

            var cartType = updatecartitem == null ? (ShoppingCartType)shoppingCartTypeId :
                //if the item to update is found, then we ignore the specified "shoppingCartTypeId" parameter
                updatecartitem.ShoppingCartType;

            await SaveItemAsync(updatecartitem, addToCartWarnings, tvchannel, cartType, attributes, userEnteredPriceConverted, rentalStartDate, rentalEndDate, quantity);

            //return result
            return await GetTvChannelToCartDetailsAsync(addToCartWarnings, cartType, tvchannel);
        }

        //handle tvchannel attribute selection event. this way we return new price, overridden gtin/sku/mpn
        //currently we use this method on the tvchannel details pages
        [HttpPost]
        public virtual async Task<IActionResult> TvChannelDetails_AttributeChange(int tvchannelId, bool validateAttributeConditions,
            bool loadPicture, IFormCollection form)
        {
            var tvchannel = await _tvchannelService.GetTvChannelByIdAsync(tvchannelId);
            if (tvchannel == null)
                return new NullJsonResult();

            var errors = new List<string>();
            var attributeXml = await _tvchannelAttributeParser.ParseTvChannelAttributesAsync(tvchannel, form, errors);

            //rental attributes
            DateTime? rentalStartDate = null;
            DateTime? rentalEndDate = null;
            if (tvchannel.IsRental)
            {
                _tvchannelAttributeParser.ParseRentalDates(tvchannel, form, out rentalStartDate, out rentalEndDate);
            }

            //sku, mpn, gtin
            var sku = await _tvchannelService.FormatSkuAsync(tvchannel, attributeXml);
            var mpn = await _tvchannelService.FormatMpnAsync(tvchannel, attributeXml);
            var gtin = await _tvchannelService.FormatGtinAsync(tvchannel, attributeXml);

            // calculating weight adjustment
            var attributeValues = await _tvchannelAttributeParser.ParseTvChannelAttributeValuesAsync(attributeXml);
            var totalWeight = tvchannel.BasepriceAmount;

            foreach (var attributeValue in attributeValues)
            {
                switch (attributeValue.AttributeValueType)
                {
                    case AttributeValueType.Simple:
                        //simple attribute
                        totalWeight += attributeValue.WeightAdjustment;
                        break;
                    case AttributeValueType.AssociatedToTvChannel:
                        //bundled tvchannel
                        var associatedTvChannel = await _tvchannelService.GetTvChannelByIdAsync(attributeValue.AssociatedTvChannelId);
                        if (associatedTvChannel != null)
                            totalWeight += associatedTvChannel.BasepriceAmount * attributeValue.Quantity;
                        break;
                }
            }

            //price
            var price = string.Empty;
            //base price
            var basepricepangv = string.Empty;
            if (!tvchannel.UserEntersPrice && await _permissionService.AuthorizeAsync(StandardPermissionProvider.DisplayPrices))
            {
                var currentStore = await _storeContext.GetCurrentStoreAsync();
                var currentUser = await _workContext.GetCurrentUserAsync();
                
                //we do not calculate price of "user enters price" option is enabled
                var (finalPrice, _, _) = await _shoppingCartService.GetUnitPriceAsync(tvchannel,
                    currentUser,
                    currentStore,
                    ShoppingCartType.ShoppingCart,
                    1, attributeXml, 0,
                    rentalStartDate, rentalEndDate, true);
                var (finalPriceWithDiscountBase, _) = await _taxService.GetTvChannelPriceAsync(tvchannel, finalPrice);
                var finalPriceWithDiscount = await _currencyService.ConvertFromPrimaryStoreCurrencyAsync(finalPriceWithDiscountBase, await _workContext.GetWorkingCurrencyAsync());
                price = await _priceFormatter.FormatPriceAsync(finalPriceWithDiscount);
                basepricepangv = await _priceFormatter.FormatBasePriceAsync(tvchannel, finalPriceWithDiscountBase, totalWeight);
            }

            //stock
            var stockAvailability = await _tvchannelService.FormatStockMessageAsync(tvchannel, attributeXml);

            //conditional attributes
            var enabledAttributeMappingIds = new List<int>();
            var disabledAttributeMappingIds = new List<int>();
            if (validateAttributeConditions)
            {
                var attributes = await _tvchannelAttributeService.GetTvChannelAttributeMappingsByTvChannelIdAsync(tvchannel.Id);
                foreach (var attribute in attributes)
                {
                    var conditionMet = await _tvchannelAttributeParser.IsConditionMetAsync(attribute, attributeXml);
                    if (conditionMet.HasValue)
                    {
                        if (conditionMet.Value)
                            enabledAttributeMappingIds.Add(attribute.Id);
                        else
                            disabledAttributeMappingIds.Add(attribute.Id);
                    }
                }
            }

            //picture. used when we want to override a default tvchannel picture when some attribute is selected
            var pictureFullSizeUrl = string.Empty;
            var pictureDefaultSizeUrl = string.Empty;
            if (loadPicture)
            {
                //first, try to get tvchannel attribute combination picture
                var pictureId = (await _tvchannelAttributeParser.FindTvChannelAttributeCombinationAsync(tvchannel, attributeXml))?.PictureId ?? 0;

                //then, let's see whether we have attribute values with pictures
                if (pictureId == 0)
                {
                    pictureId = (await _tvchannelAttributeParser.ParseTvChannelAttributeValuesAsync(attributeXml))
                        .FirstOrDefault(attributeValue => attributeValue.PictureId > 0)?.PictureId ?? 0;
                }

                if (pictureId > 0)
                {
                    var tvchannelAttributePictureCacheKey = _staticCacheManager.PrepareKeyForDefaultCache(TvProgModelCacheDefaults.TvChannelAttributePictureModelKey,
                        pictureId, _webHelper.IsCurrentConnectionSecured(), await _storeContext.GetCurrentStoreAsync());
                    var pictureModel = await _staticCacheManager.GetAsync(tvchannelAttributePictureCacheKey, async () =>
                    {
                        var picture = await _pictureService.GetPictureByIdAsync(pictureId);
                        string fullSizeImageUrl, imageUrl;

                        (fullSizeImageUrl, picture) = await _pictureService.GetPictureUrlAsync(picture);
                        (imageUrl, picture) = await _pictureService.GetPictureUrlAsync(picture, _mediaSettings.TvChannelDetailsPictureSize);

                        return picture == null ? new PictureModel() : new PictureModel
                        {
                            FullSizeImageUrl = fullSizeImageUrl,
                            ImageUrl = imageUrl
                        };
                    });
                    pictureFullSizeUrl = pictureModel.FullSizeImageUrl;
                    pictureDefaultSizeUrl = pictureModel.ImageUrl;
                }
            }

            var isFreeShipping = tvchannel.IsFreeShipping;
            if (isFreeShipping && !string.IsNullOrEmpty(attributeXml))
            {
                isFreeShipping = await (await _tvchannelAttributeParser.ParseTvChannelAttributeValuesAsync(attributeXml))
                    .Where(attributeValue => attributeValue.AttributeValueType == AttributeValueType.AssociatedToTvChannel)
                    .SelectAwait(async attributeValue => await _tvchannelService.GetTvChannelByIdAsync(attributeValue.AssociatedTvChannelId))
                    .AllAsync(associatedTvChannel => associatedTvChannel == null || !associatedTvChannel.IsShipEnabled || associatedTvChannel.IsFreeShipping);
            }

            return Json(new
            {
                tvchannelId,
                gtin,
                mpn,
                sku,
                price,
                basepricepangv,
                stockAvailability,
                enabledattributemappingids = enabledAttributeMappingIds.ToArray(),
                disabledattributemappingids = disabledAttributeMappingIds.ToArray(),
                pictureFullSizeUrl,
                pictureDefaultSizeUrl,
                isFreeShipping,
                message = errors.Any() ? errors.ToArray() : null
            });
        }

        [HttpPost]
        public virtual async Task<IActionResult> CheckoutAttributeChange(IFormCollection form, bool isEditable)
        {
            var user = await _workContext.GetCurrentUserAsync();
            var store = await _storeContext.GetCurrentStoreAsync();
            var cart = await _shoppingCartService.GetShoppingCartAsync(user, ShoppingCartType.ShoppingCart, store.Id);

            //save selected attributes
            await ParseAndSaveCheckoutAttributesAsync(cart, form);
            var attributeXml = await _genericAttributeService.GetAttributeAsync<string>(user,
                TvProgUserDefaults.CheckoutAttributes, store.Id);

            //conditions
            var enabledAttributeIds = new List<int>();
            var disabledAttributeIds = new List<int>();
            var excludeShippableAttributes = !await _shoppingCartService.ShoppingCartRequiresShippingAsync(cart);
            var attributes = await _checkoutAttributeService.GetAllCheckoutAttributesAsync(store.Id, excludeShippableAttributes);
            foreach (var attribute in attributes)
            {
                var conditionMet = await _checkoutAttributeParser.IsConditionMetAsync(attribute, attributeXml);
                if (conditionMet.HasValue)
                {
                    if (conditionMet.Value)
                        enabledAttributeIds.Add(attribute.Id);
                    else
                        disabledAttributeIds.Add(attribute.Id);
                }
            }

            //update blocks
            var ordetotalssectionhtml = await RenderViewComponentToStringAsync(typeof(OrderTotalsViewComponent), new { isEditable });
            var selectedcheckoutattributesssectionhtml = await RenderViewComponentToStringAsync(typeof(SelectedCheckoutAttributesViewComponent));

            return Json(new
            {
                ordetotalssectionhtml,
                selectedcheckoutattributesssectionhtml,
                enabledattributeids = enabledAttributeIds.ToArray(),
                disabledattributeids = disabledAttributeIds.ToArray()
            });
        }

        [HttpPost]
        [IgnoreAntiforgeryToken]
        public virtual async Task<IActionResult> UploadFileTvChannelAttribute(int attributeId)
        {
            var attribute = await _tvchannelAttributeService.GetTvChannelAttributeMappingByIdAsync(attributeId);
            if (attribute == null || attribute.AttributeControlType != AttributeControlType.FileUpload)
            {
                return Json(new
                {
                    success = false,
                    downloadGuid = Guid.Empty
                });
            }

            var httpPostedFile = Request.Form.Files.FirstOrDefault();
            if (httpPostedFile == null)
            {
                return Json(new
                {
                    success = false,
                    message = "No file uploaded",
                    downloadGuid = Guid.Empty
                });
            }

            var fileBinary = await _downloadService.GetDownloadBitsAsync(httpPostedFile);

            var qqFileNameParameter = "qqfilename";
            var fileName = httpPostedFile.FileName;
            if (string.IsNullOrEmpty(fileName) && Request.Form.ContainsKey(qqFileNameParameter))
                fileName = Request.Form[qqFileNameParameter].ToString();
            //remove path (passed in IE)
            fileName = _fileProvider.GetFileName(fileName);

            var contentType = httpPostedFile.ContentType;

            var fileExtension = _fileProvider.GetFileExtension(fileName);
            if (!string.IsNullOrEmpty(fileExtension))
                fileExtension = fileExtension.ToLowerInvariant();

            if (attribute.ValidationFileMaximumSize.HasValue)
            {
                //compare in bytes
                var maxFileSizeBytes = attribute.ValidationFileMaximumSize.Value * 1024;
                if (fileBinary.Length > maxFileSizeBytes)
                {
                    //when returning JSON the mime-type must be set to text/plain
                    //otherwise some browsers will pop-up a "Save As" dialog.
                    return Json(new
                    {
                        success = false,
                        message = string.Format(await _localizationService.GetResourceAsync("ShoppingCart.MaximumUploadedFileSize"), attribute.ValidationFileMaximumSize.Value),
                        downloadGuid = Guid.Empty
                    });
                }
            }

            var download = new Download
            {
                DownloadGuid = Guid.NewGuid(),
                UseDownloadUrl = false,
                DownloadUrl = string.Empty,
                DownloadBinary = fileBinary,
                ContentType = contentType,
                //we store filename without extension for downloads
                Filename = _fileProvider.GetFileNameWithoutExtension(fileName),
                Extension = fileExtension,
                IsNew = true
            };
            await _downloadService.InsertDownloadAsync(download);

            //when returning JSON the mime-type must be set to text/plain
            //otherwise some browsers will pop-up a "Save As" dialog.
            return Json(new
            {
                success = true,
                message = await _localizationService.GetResourceAsync("ShoppingCart.FileUploaded"),
                downloadUrl = Url.Action("GetFileUpload", "Download", new { downloadId = download.DownloadGuid }),
                downloadGuid = download.DownloadGuid
            });
        }

        [HttpPost]
        [IgnoreAntiforgeryToken]
        public virtual async Task<IActionResult> UploadFileCheckoutAttribute(int attributeId)
        {
            var attribute = await _checkoutAttributeService.GetCheckoutAttributeByIdAsync(attributeId);
            if (attribute == null || attribute.AttributeControlType != AttributeControlType.FileUpload)
            {
                return Json(new
                {
                    success = false,
                    downloadGuid = Guid.Empty
                });
            }

            var httpPostedFile = Request.Form.Files.FirstOrDefault();
            if (httpPostedFile == null)
            {
                return Json(new
                {
                    success = false,
                    message = "No file uploaded",
                    downloadGuid = Guid.Empty
                });
            }

            var fileBinary = await _downloadService.GetDownloadBitsAsync(httpPostedFile);

            var qqFileNameParameter = "qqfilename";
            var fileName = httpPostedFile.FileName;
            if (string.IsNullOrEmpty(fileName) && Request.Form.ContainsKey(qqFileNameParameter))
                fileName = Request.Form[qqFileNameParameter].ToString();
            //remove path (passed in IE)
            fileName = _fileProvider.GetFileName(fileName);

            var contentType = httpPostedFile.ContentType;

            var fileExtension = _fileProvider.GetFileExtension(fileName);
            if (!string.IsNullOrEmpty(fileExtension))
                fileExtension = fileExtension.ToLowerInvariant();

            if (attribute.ValidationFileMaximumSize.HasValue)
            {
                //compare in bytes
                var maxFileSizeBytes = attribute.ValidationFileMaximumSize.Value * 1024;
                if (fileBinary.Length > maxFileSizeBytes)
                {
                    //when returning JSON the mime-type must be set to text/plain
                    //otherwise some browsers will pop-up a "Save As" dialog.
                    return Json(new
                    {
                        success = false,
                        message = string.Format(await _localizationService.GetResourceAsync("ShoppingCart.MaximumUploadedFileSize"), attribute.ValidationFileMaximumSize.Value),
                        downloadGuid = Guid.Empty
                    });
                }
            }

            var download = new Download
            {
                DownloadGuid = Guid.NewGuid(),
                UseDownloadUrl = false,
                DownloadUrl = string.Empty,
                DownloadBinary = fileBinary,
                ContentType = contentType,
                //we store filename without extension for downloads
                Filename = _fileProvider.GetFileNameWithoutExtension(fileName),
                Extension = fileExtension,
                IsNew = true
            };
            await _downloadService.InsertDownloadAsync(download);

            //when returning JSON the mime-type must be set to text/plain
            //otherwise some browsers will pop-up a "Save As" dialog.
            return Json(new
            {
                success = true,
                message = await _localizationService.GetResourceAsync("ShoppingCart.FileUploaded"),
                downloadUrl = Url.Action("GetFileUpload", "Download", new { downloadId = download.DownloadGuid }),
                downloadGuid = download.DownloadGuid
            });
        }

        public virtual async Task<IActionResult> Cart()
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.EnableShoppingCart))
                return RedirectToRoute("Homepage");

            var store = await _storeContext.GetCurrentStoreAsync();
            var cart = await _shoppingCartService.GetShoppingCartAsync(await _workContext.GetCurrentUserAsync(), ShoppingCartType.ShoppingCart, store.Id);
            var model = new ShoppingCartModel();
            model = await _shoppingCartModelFactory.PrepareShoppingCartModelAsync(model, cart);
            return View(model);
        }

        [HttpPost, ActionName("Cart")]
        [FormValueRequired("updatecart")]
        public virtual async Task<IActionResult> UpdateCart(IFormCollection form)
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.EnableShoppingCart))
                return RedirectToRoute("Homepage");

            var user = await _workContext.GetCurrentUserAsync();
            var store = await _storeContext.GetCurrentStoreAsync();
            var cart = await _shoppingCartService.GetShoppingCartAsync(user, ShoppingCartType.ShoppingCart, store.Id);

            //get identifiers of items to remove
            var itemIdsToRemove = form["removefromcart"]
                .SelectMany(value => value.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
                .Select(idString => int.TryParse(idString, out var id) ? id : 0)
                .Distinct().ToList();

            var tvchannels = (await _tvchannelService.GetTvChannelsByIdsAsync(cart.Select(item => item.TvChannelId).Distinct().ToArray()))
                .ToDictionary(item => item.Id, item => item);

            //get order items with changed quantity
            var itemsWithNewQuantity = cart.Select(item => new
            {
                //try to get a new quantity for the item, set 0 for items to remove
                NewQuantity = itemIdsToRemove.Contains(item.Id) ? 0 : int.TryParse(form[$"itemquantity{item.Id}"], out var quantity) ? quantity : item.Quantity,
                Item = item,
                TvChannel = tvchannels.ContainsKey(item.TvChannelId) ? tvchannels[item.TvChannelId] : null
            }).Where(item => item.NewQuantity != item.Item.Quantity);

            //order cart items
            //first should be items with a reduced quantity and that require other tvchannels; or items with an increased quantity and are required for other tvchannels
            var orderedCart = await itemsWithNewQuantity
                .OrderByDescendingAwait(async cartItem =>
                    (cartItem.NewQuantity < cartItem.Item.Quantity &&
                     (cartItem.TvChannel?.RequireOtherTvChannels ?? false)) ||
                    (cartItem.NewQuantity > cartItem.Item.Quantity && cartItem.TvChannel != null && (await _shoppingCartService
                         .GetTvChannelsRequiringTvChannelAsync(cart, cartItem.TvChannel)).Any()))
                .ToListAsync();

            //try to update cart items with new quantities and get warnings
            var warnings = await orderedCart.SelectAwait(async cartItem => new
            {
                ItemId = cartItem.Item.Id,
                Warnings = await _shoppingCartService.UpdateShoppingCartItemAsync(user,
                    cartItem.Item.Id, cartItem.Item.AttributesXml, cartItem.Item.UserEnteredPrice,
                    cartItem.Item.RentalStartDateUtc, cartItem.Item.RentalEndDateUtc, cartItem.NewQuantity, true)
            }).ToListAsync();

            //updated cart
            cart = await _shoppingCartService.GetShoppingCartAsync(user, ShoppingCartType.ShoppingCart, store.Id);

            //parse and save checkout attributes
            await ParseAndSaveCheckoutAttributesAsync(cart, form);

            //prepare model
            var model = new ShoppingCartModel();
            model = await _shoppingCartModelFactory.PrepareShoppingCartModelAsync(model, cart);

            //update current warnings
            foreach (var warningItem in warnings.Where(warningItem => warningItem.Warnings.Any()))
            {
                //find shopping cart item model to display appropriate warnings
                var itemModel = model.Items.FirstOrDefault(item => item.Id == warningItem.ItemId);
                if (itemModel != null)
                    itemModel.Warnings = warningItem.Warnings.Concat(itemModel.Warnings).Distinct().ToList();
            }

            return View(model);
        }

        [HttpPost, ActionName("Cart")]
        [FormValueRequired("continueshopping")]
        public virtual async Task<IActionResult> ContinueShopping()
        {
            var store = await _storeContext.GetCurrentStoreAsync();
            var returnUrl = await _genericAttributeService.GetAttributeAsync<string>(await _workContext.GetCurrentUserAsync(), TvProgUserDefaults.LastContinueShoppingPageAttribute, store.Id);

            if (!string.IsNullOrEmpty(returnUrl))
                return Redirect(returnUrl);

            return RedirectToRoute("Homepage");
        }

        [HttpPost, ActionName("Cart")]
        [FormValueRequired("checkout")]
        public virtual async Task<IActionResult> StartCheckout(IFormCollection form)
        {
            var user = await _workContext.GetCurrentUserAsync();
            var store = await _storeContext.GetCurrentStoreAsync();
            var cart = await _shoppingCartService.GetShoppingCartAsync(user, ShoppingCartType.ShoppingCart, store.Id);

            //parse and save checkout attributes
            await ParseAndSaveCheckoutAttributesAsync(cart, form);

            //validate attributes
            var checkoutAttributes = await _genericAttributeService.GetAttributeAsync<string>(user,
                TvProgUserDefaults.CheckoutAttributes, store.Id);
            var checkoutAttributeWarnings = await _shoppingCartService.GetShoppingCartWarningsAsync(cart, checkoutAttributes, true);
            if (checkoutAttributeWarnings.Any())
            {
                //something wrong, redisplay the page with warnings
                var model = new ShoppingCartModel();
                model = await _shoppingCartModelFactory.PrepareShoppingCartModelAsync(model, cart, validateCheckoutAttributes: true);
                return View(model);
            }

            var anonymousPermissed = _orderSettings.AnonymousCheckoutAllowed
                                     && _userSettings.UserRegistrationType == UserRegistrationType.Disabled;

            if (anonymousPermissed || !await _userService.IsGuestAsync(user))
                return RedirectToRoute("Checkout");

            var cartTvChannelIds = cart.Select(ci => ci.TvChannelId).ToArray();
            var downloadableTvChannelsRequireRegistration =
                _userSettings.RequireRegistrationForDownloadableTvChannels && await _tvchannelService.HasAnyDownloadableTvChannelAsync(cartTvChannelIds);

            if (!_orderSettings.AnonymousCheckoutAllowed || downloadableTvChannelsRequireRegistration)
            {
                //verify user identity (it may be facebook login page, or google, or local)
                return Challenge();
            }

            return RedirectToRoute("LoginCheckoutAsGuest", new { returnUrl = Url.RouteUrl("ShoppingCart") });
        }

        [HttpPost, ActionName("Cart")]
        [FormValueRequired("applydiscountcouponcode")]
        public virtual async Task<IActionResult> ApplyDiscountCoupon(string discountcouponcode, IFormCollection form)
        {
            //trim
            if (discountcouponcode != null)
                discountcouponcode = discountcouponcode.Trim();

            //cart
            var user = await _workContext.GetCurrentUserAsync();
            var store = await _storeContext.GetCurrentStoreAsync();
            var cart = await _shoppingCartService.GetShoppingCartAsync(user, ShoppingCartType.ShoppingCart, store.Id);

            //parse and save checkout attributes
            await ParseAndSaveCheckoutAttributesAsync(cart, form);

            var model = new ShoppingCartModel();
            if (!string.IsNullOrWhiteSpace(discountcouponcode))
            {
                //we find even hidden records here. this way we can display a user-friendly message if it's expired
                var discounts = (await _discountService.GetAllDiscountsAsync(couponCode: discountcouponcode, showHidden: true))
                    .Where(d => d.RequiresCouponCode)
                    .ToList();
                if (discounts.Any())
                {
                    var userErrors = new List<string>();
                    var anyValidDiscount = await discounts.AnyAwaitAsync(async discount =>
                    {
                        var validationResult = await _discountService.ValidateDiscountAsync(discount, user, new[] { discountcouponcode });
                        userErrors.AddRange(validationResult.Errors);

                        return validationResult.IsValid;
                    });

                    if (anyValidDiscount)
                    {
                        //valid
                        await _userService.ApplyDiscountCouponCodeAsync(user, discountcouponcode);
                        model.DiscountBox.Messages.Add(await _localizationService.GetResourceAsync("ShoppingCart.DiscountCouponCode.Applied"));
                        model.DiscountBox.IsApplied = true;
                    }
                    else
                    {
                        if (userErrors.Any())
                            //some user errors
                            model.DiscountBox.Messages = userErrors;
                        else
                            //general error text
                            model.DiscountBox.Messages.Add(await _localizationService.GetResourceAsync("ShoppingCart.DiscountCouponCode.WrongDiscount"));
                    }
                }
                else
                    //discount cannot be found
                    model.DiscountBox.Messages.Add(await _localizationService.GetResourceAsync("ShoppingCart.DiscountCouponCode.CannotBeFound"));
            }
            else
                //empty coupon code
                model.DiscountBox.Messages.Add(await _localizationService.GetResourceAsync("ShoppingCart.DiscountCouponCode.Empty"));

            model = await _shoppingCartModelFactory.PrepareShoppingCartModelAsync(model, cart);

            return View(model);
        }

        [HttpPost, ActionName("Cart")]
        [FormValueRequired("applygiftcardcouponcode")]
        public virtual async Task<IActionResult> ApplyGiftCard(string giftcardcouponcode, IFormCollection form)
        {
            //trim
            if (giftcardcouponcode != null)
                giftcardcouponcode = giftcardcouponcode.Trim();

            //cart
            var user = await _workContext.GetCurrentUserAsync();
            var store = await _storeContext.GetCurrentStoreAsync();
            var cart = await _shoppingCartService.GetShoppingCartAsync(user, ShoppingCartType.ShoppingCart, store.Id);

            //parse and save checkout attributes
            await ParseAndSaveCheckoutAttributesAsync(cart, form);

            var model = new ShoppingCartModel();
            if (!await _shoppingCartService.ShoppingCartIsRecurringAsync(cart))
            {
                if (!string.IsNullOrWhiteSpace(giftcardcouponcode))
                {
                    var giftCard = (await _giftCardService.GetAllGiftCardsAsync(giftCardCouponCode: giftcardcouponcode)).FirstOrDefault();
                    var isGiftCardValid = giftCard != null && await _giftCardService.IsGiftCardValidAsync(giftCard);
                    if (isGiftCardValid)
                    {
                        await _userService.ApplyGiftCardCouponCodeAsync(user, giftcardcouponcode);
                        model.GiftCardBox.Message = await _localizationService.GetResourceAsync("ShoppingCart.GiftCardCouponCode.Applied");
                        model.GiftCardBox.IsApplied = true;
                    }
                    else
                    {
                        model.GiftCardBox.Message = await _localizationService.GetResourceAsync("ShoppingCart.GiftCardCouponCode.WrongGiftCard");
                        model.GiftCardBox.IsApplied = false;
                    }
                }
                else
                {
                    model.GiftCardBox.Message = await _localizationService.GetResourceAsync("ShoppingCart.GiftCardCouponCode.WrongGiftCard");
                    model.GiftCardBox.IsApplied = false;
                }
            }
            else
            {
                model.GiftCardBox.Message = await _localizationService.GetResourceAsync("ShoppingCart.GiftCardCouponCode.DontWorkWithAutoshipTvChannels");
                model.GiftCardBox.IsApplied = false;
            }

            model = await _shoppingCartModelFactory.PrepareShoppingCartModelAsync(model, cart);
            return View(model);
        }

        [HttpPost]
        public virtual async Task<IActionResult> GetEstimateShipping(EstimateShippingModel model, IFormCollection form)
        {
            if (model == null)
                model = new EstimateShippingModel();

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

            var store = await _storeContext.GetCurrentStoreAsync();
            var cart = await _shoppingCartService.GetShoppingCartAsync(await _workContext.GetCurrentUserAsync(), ShoppingCartType.ShoppingCart, store.Id);
            //parse and save checkout attributes
            await ParseAndSaveCheckoutAttributesAsync(cart, form);

            var result = await _shoppingCartModelFactory.PrepareEstimateShippingResultModelAsync(cart, model, true);

            return Json(result);
        }

        [HttpPost, ActionName("Cart")]
        [FormValueRequired(FormValueRequirement.StartsWith, "removediscount-")]
        public virtual async Task<IActionResult> RemoveDiscountCoupon(IFormCollection form)
        {
            var model = new ShoppingCartModel();

            //get discount identifier
            var discountId = 0;
            foreach (var formValue in form.Keys)
                if (formValue.StartsWith("removediscount-", StringComparison.InvariantCultureIgnoreCase))
                    discountId = Convert.ToInt32(formValue["removediscount-".Length..]);
            var discount = await _discountService.GetDiscountByIdAsync(discountId);
            var user = await _workContext.GetCurrentUserAsync();
            if (discount != null)
                await _userService.RemoveDiscountCouponCodeAsync(user, discount.CouponCode);

            var store = await _storeContext.GetCurrentStoreAsync();
            var cart = await _shoppingCartService.GetShoppingCartAsync(user, ShoppingCartType.ShoppingCart, store.Id);

            model = await _shoppingCartModelFactory.PrepareShoppingCartModelAsync(model, cart);
            return View(model);
        }

        [HttpPost, ActionName("Cart")]
        [FormValueRequired(FormValueRequirement.StartsWith, "removegiftcard-")]
        public virtual async Task<IActionResult> RemoveGiftCardCode(IFormCollection form)
        {
            var model = new ShoppingCartModel();

            //get gift card identifier
            var giftCardId = 0;
            foreach (var formValue in form.Keys)
                if (formValue.StartsWith("removegiftcard-", StringComparison.InvariantCultureIgnoreCase))
                    giftCardId = Convert.ToInt32(formValue["removegiftcard-".Length..]);
            var gc = await _giftCardService.GetGiftCardByIdAsync(giftCardId);
            var user = await _workContext.GetCurrentUserAsync();
            if (gc != null)
                await _userService.RemoveGiftCardCouponCodeAsync(user, gc.GiftCardCouponCode);

            var store = await _storeContext.GetCurrentStoreAsync();
            var cart = await _shoppingCartService.GetShoppingCartAsync(user, ShoppingCartType.ShoppingCart, store.Id);

            model = await _shoppingCartModelFactory.PrepareShoppingCartModelAsync(model, cart);
            return View(model);
        }

        #endregion

        #region Wishlist

        public virtual async Task<IActionResult> Wishlist(Guid? userGuid)
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.EnableWishlist))
                return RedirectToRoute("Homepage");

            var user = userGuid.HasValue ?
                await _userService.GetUserByGuidAsync(userGuid.Value)
                : await _workContext.GetCurrentUserAsync();
            if (user == null)
                return RedirectToRoute("Homepage");

            var store = await _storeContext.GetCurrentStoreAsync();
            var cart = await _shoppingCartService.GetShoppingCartAsync(user, ShoppingCartType.Wishlist, store.Id);

            var model = new WishlistModel();
            model = await _shoppingCartModelFactory.PrepareWishlistModelAsync(model, cart, !userGuid.HasValue);
            return View(model);
        }

        [HttpPost, ActionName("Wishlist")]
        [FormValueRequired("updatecart")]
        public virtual async Task<IActionResult> UpdateWishlist(IFormCollection form)
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.EnableWishlist))
                return RedirectToRoute("Homepage");

            var user = await _workContext.GetCurrentUserAsync();
            var store = await _storeContext.GetCurrentStoreAsync();
            var cart = await _shoppingCartService.GetShoppingCartAsync(user, ShoppingCartType.Wishlist, store.Id);

            var allIdsToRemove = form.ContainsKey("removefromcart")
                ? form["removefromcart"].ToString().Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries)
                .Select(int.Parse)
                .ToList()
                : new List<int>();

            //current warnings <cart item identifier, warnings>
            var innerWarnings = new Dictionary<int, IList<string>>();
            foreach (var sci in cart)
            {
                var remove = allIdsToRemove.Contains(sci.Id);
                if (remove)
                    await _shoppingCartService.DeleteShoppingCartItemAsync(sci);
                else
                {
                    foreach (var formKey in form.Keys)
                        if (formKey.Equals($"itemquantity{sci.Id}", StringComparison.InvariantCultureIgnoreCase))
                        {
                            if (int.TryParse(form[formKey], out var newQuantity))
                            {
                                var currSciWarnings = await _shoppingCartService.UpdateShoppingCartItemAsync(user,
                                    sci.Id, sci.AttributesXml, sci.UserEnteredPrice,
                                    sci.RentalStartDateUtc, sci.RentalEndDateUtc,
                                    newQuantity, true);
                                innerWarnings.Add(sci.Id, currSciWarnings);
                            }

                            break;
                        }
                }
            }

            //updated wishlist
            cart = await _shoppingCartService.GetShoppingCartAsync(user, ShoppingCartType.Wishlist, store.Id);
            var model = new WishlistModel();
            model = await _shoppingCartModelFactory.PrepareWishlistModelAsync(model, cart);
            //update current warnings
            foreach (var kvp in innerWarnings)
            {
                //kvp = <cart item identifier, warnings>
                var sciId = kvp.Key;
                var warnings = kvp.Value;
                //find model
                var sciModel = model.Items.FirstOrDefault(x => x.Id == sciId);
                if (sciModel != null)
                    foreach (var w in warnings)
                        if (!sciModel.Warnings.Contains(w))
                            sciModel.Warnings.Add(w);
            }

            return View(model);
        }

        [HttpPost, ActionName("Wishlist")]
        [FormValueRequired("addtocartbutton")]
        public virtual async Task<IActionResult> AddItemsToCartFromWishlist(Guid? userGuid, IFormCollection form)
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.EnableShoppingCart))
                return RedirectToRoute("Homepage");

            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.EnableWishlist))
                return RedirectToRoute("Homepage");

            var user = await _workContext.GetCurrentUserAsync();
            var pageUser = userGuid.HasValue
                ? await _userService.GetUserByGuidAsync(userGuid.Value)
                : user;
            if (pageUser == null)
                return RedirectToRoute("Homepage");

            var store = await _storeContext.GetCurrentStoreAsync();
            var pageCart = await _shoppingCartService.GetShoppingCartAsync(pageUser, ShoppingCartType.Wishlist, store.Id);

            var allWarnings = new List<string>();
            var countOfAddedItems = 0;
            var allIdsToAdd = form.ContainsKey("addtocart")
                ? form["addtocart"].ToString().Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries).Select(int.Parse).ToList()
                : new List<int>();
            foreach (var sci in pageCart)
            {
                if (allIdsToAdd.Contains(sci.Id))
                {
                    var tvchannel = await _tvchannelService.GetTvChannelByIdAsync(sci.TvChannelId);

                    var warnings = await _shoppingCartService.AddToCartAsync(user,
                        tvchannel, ShoppingCartType.ShoppingCart,
                        store.Id,
                        sci.AttributesXml, sci.UserEnteredPrice,
                        sci.RentalStartDateUtc, sci.RentalEndDateUtc, sci.Quantity, true);
                    if (!warnings.Any())
                        countOfAddedItems++;
                    if (_shoppingCartSettings.MoveItemsFromWishlistToCart && //settings enabled
                        !userGuid.HasValue && //own wishlist
                        !warnings.Any()) //no warnings ( already in the cart)
                    {
                        //let's remove the item from wishlist
                        await _shoppingCartService.DeleteShoppingCartItemAsync(sci);
                    }

                    allWarnings.AddRange(warnings);
                }
            }

            if (countOfAddedItems > 0)
            {
                //redirect to the shopping cart page

                if (allWarnings.Any())
                {
                    _notificationService.ErrorNotification(await _localizationService.GetResourceAsync("Wishlist.AddToCart.Error"));
                }

                return RedirectToRoute("ShoppingCart");
            }
            else
            {
                _notificationService.WarningNotification(await _localizationService.GetResourceAsync("Wishlist.AddToCart.NoAddedItems"));
            }
            //no items added. redisplay the wishlist page

            if (allWarnings.Any())
            {
                _notificationService.ErrorNotification(await _localizationService.GetResourceAsync("Wishlist.AddToCart.Error"));
            }

            var cart = await _shoppingCartService.GetShoppingCartAsync(pageUser, ShoppingCartType.Wishlist, store.Id);

            var model = new WishlistModel();
            model = await _shoppingCartModelFactory.PrepareWishlistModelAsync(model, cart, !userGuid.HasValue);
            return View(model);
        }

        public virtual async Task<IActionResult> EmailWishlist()
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.EnableWishlist) || !_shoppingCartSettings.EmailWishlistEnabled)
                return RedirectToRoute("Homepage");

            var store = await _storeContext.GetCurrentStoreAsync();
            var cart = await _shoppingCartService.GetShoppingCartAsync(await _workContext.GetCurrentUserAsync(), ShoppingCartType.Wishlist, store.Id);

            if (!cart.Any())
                return RedirectToRoute("Homepage");

            var model = new WishlistEmailAFriendModel();
            model = await _shoppingCartModelFactory.PrepareWishlistEmailAFriendModelAsync(model, false);
            return View(model);
        }

        [HttpPost, ActionName("EmailWishlist")]
        [FormValueRequired("send-email")]
        [ValidateCaptcha]
        public virtual async Task<IActionResult> EmailWishlistSend(WishlistEmailAFriendModel model, bool captchaValid)
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.EnableWishlist) || !_shoppingCartSettings.EmailWishlistEnabled)
                return RedirectToRoute("Homepage");

            var user = await _workContext.GetCurrentUserAsync();
            var store = await _storeContext.GetCurrentStoreAsync();
            var cart = await _shoppingCartService.GetShoppingCartAsync(user, ShoppingCartType.Wishlist, store.Id);

            if (!cart.Any())
                return RedirectToRoute("Homepage");

            //validate CAPTCHA
            if (_captchaSettings.Enabled && _captchaSettings.ShowOnEmailWishlistToFriendPage && !captchaValid)
            {
                ModelState.AddModelError(string.Empty, await _localizationService.GetResourceAsync("Common.WrongCaptchaMessage"));
            }

            //check whether the current user is guest and ia allowed to email wishlist
            if (await _userService.IsGuestAsync(user) && !_shoppingCartSettings.AllowAnonymousUsersToEmailWishlist)
            {
                ModelState.AddModelError(string.Empty, await _localizationService.GetResourceAsync("Wishlist.EmailAFriend.OnlyRegisteredUsers"));
            }

            if (ModelState.IsValid)
            {
                //email
                await _workflowMessageService.SendWishlistEmailAFriendMessageAsync(user,
                        (await _workContext.GetWorkingLanguageAsync()).Id, model.YourEmailAddress,
                        model.FriendEmail, _htmlFormatter.FormatText(model.PersonalMessage, false, true, false, false, false, false));

                model.SuccessfullySent = true;
                model.Result = await _localizationService.GetResourceAsync("Wishlist.EmailAFriend.SuccessfullySent");

                return View(model);
            }

            //If we got this far, something failed, redisplay form
            model = await _shoppingCartModelFactory.PrepareWishlistEmailAFriendModelAsync(model, true);

            return View(model);
        }

        #endregion
    }
}