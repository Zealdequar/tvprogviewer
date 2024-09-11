using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.Routing;
using TvProgViewer.Core;
using TvProgViewer.Core.Domain;
using TvProgViewer.Core.Domain.Blogs;
using TvProgViewer.Core.Domain.Catalog;
using TvProgViewer.Core.Domain.Common;
using TvProgViewer.Core.Domain.Users;
using TvProgViewer.Core.Domain.Directory;
using TvProgViewer.Core.Domain.Forums;
using TvProgViewer.Core.Domain.Localization;
using TvProgViewer.Core.Domain.Messages;
using TvProgViewer.Core.Domain.News;
using TvProgViewer.Core.Domain.Orders;
using TvProgViewer.Core.Domain.Payments;
using TvProgViewer.Core.Domain.Shipping;
using TvProgViewer.Core.Domain.Stores;
using TvProgViewer.Core.Domain.Tax;
using TvProgViewer.Core.Domain.Vendors;
using TvProgViewer.Core.Events;
using TvProgViewer.Core.Infrastructure;
using TvProgViewer.Services.Blogs;
using TvProgViewer.Services.Catalog;
using TvProgViewer.Services.Common;
using TvProgViewer.Services.Users;
using TvProgViewer.Services.Directory;
using TvProgViewer.Services.Forums;
using TvProgViewer.Services.Helpers;
using TvProgViewer.Services.Html;
using TvProgViewer.Services.Localization;
using TvProgViewer.Services.News;
using TvProgViewer.Services.Orders;
using TvProgViewer.Services.Payments;
using TvProgViewer.Services.Seo;
using TvProgViewer.Services.Shipping;
using TvProgViewer.Services.Stores;
using TvProgViewer.Services.Vendors;

namespace TvProgViewer.Services.Messages
{
    /// <summary>
    /// Message token provider
    /// </summary>
    public partial class MessageTokenProvider : IMessageTokenProvider
    {
        #region Fields

        private readonly CatalogSettings _catalogSettings;
        private readonly CurrencySettings _currencySettings;
        private readonly IActionContextAccessor _actionContextAccessor;
        private readonly IAddressAttributeFormatter _addressAttributeFormatter;
        private readonly IAddressService _addressService;
        private readonly IBlogService _blogService;
        private readonly ICountryService _countryService;
        private readonly ICurrencyService _currencyService;
        private readonly IUserAttributeFormatter _userAttributeFormatter;
        private readonly IUserService _userService;
        private readonly IDateTimeHelper _dateTimeHelper;
        private readonly IEventPublisher _eventPublisher;
        private readonly IGenericAttributeService _genericAttributeService;
        private readonly IGiftCardService _giftCardService;
        private readonly IHtmlFormatter _htmlFormatter;
        private readonly ILanguageService _languageService;
        private readonly ILocalizationService _localizationService;
        private readonly INewsService _newsService;
        private readonly IOrderService _orderService;
        private readonly IPaymentPluginManager _paymentPluginManager;
        private readonly IPaymentService _paymentService;
        private readonly IPriceFormatter _priceFormatter;
        private readonly ITvChannelService _tvChannelService;
        private readonly IRewardPointService _rewardPointService;
        private readonly IShipmentService _shipmentService;
        private readonly IStateProvinceService _stateProvinceService;
        private readonly IStoreContext _storeContext;
        private readonly IStoreService _storeService;
        private readonly IUrlHelperFactory _urlHelperFactory;
        private readonly IUrlRecordService _urlRecordService;
        private readonly IVendorAttributeFormatter _vendorAttributeFormatter;
        private readonly IWorkContext _workContext;
        private readonly MessageTemplatesSettings _templatesSettings;
        private readonly PaymentSettings _paymentSettings;
        private readonly StoreInformationSettings _storeInformationSettings;
        private readonly TaxSettings _taxSettings;

        private Dictionary<string, IEnumerable<string>> _allowedTokens;

        #endregion

        #region Ctor

        public MessageTokenProvider(CatalogSettings catalogSettings,
            CurrencySettings currencySettings,
            IActionContextAccessor actionContextAccessor,
            IAddressAttributeFormatter addressAttributeFormatter,
            IAddressService addressService,
            IBlogService blogService,
            ICountryService countryService,
            ICurrencyService currencyService,
            IUserAttributeFormatter userAttributeFormatter,
            IUserService userService,
            IDateTimeHelper dateTimeHelper,
            IEventPublisher eventPublisher,
            IGenericAttributeService genericAttributeService,
            IGiftCardService giftCardService,
            IHtmlFormatter htmlFormatter,
            ILanguageService languageService,
            ILocalizationService localizationService,
            INewsService newsService,
            IOrderService orderService,
            IPaymentPluginManager paymentPluginManager,
            IPaymentService paymentService,
            IPriceFormatter priceFormatter,
            ITvChannelService tvChannelService,
            IRewardPointService rewardPointService,
            IShipmentService shipmentService,
            IStateProvinceService stateProvinceService,
            IStoreContext storeContext,
            IStoreService storeService,
            IUrlHelperFactory urlHelperFactory,
            IUrlRecordService urlRecordService,
            IVendorAttributeFormatter vendorAttributeFormatter,
            IWorkContext workContext,
            MessageTemplatesSettings templatesSettings,
            PaymentSettings paymentSettings,
            StoreInformationSettings storeInformationSettings,
            TaxSettings taxSettings)
        {
            _catalogSettings = catalogSettings;
            _currencySettings = currencySettings;
            _actionContextAccessor = actionContextAccessor;
            _addressAttributeFormatter = addressAttributeFormatter;
            _addressService = addressService;
            _blogService = blogService;
            _countryService = countryService;
            _currencyService = currencyService;
            _userAttributeFormatter = userAttributeFormatter;
            _userService = userService;
            _dateTimeHelper = dateTimeHelper;
            _eventPublisher = eventPublisher;
            _genericAttributeService = genericAttributeService;
            _giftCardService = giftCardService;
            _htmlFormatter = htmlFormatter;
            _languageService = languageService;
            _localizationService = localizationService;
            _newsService = newsService;
            _orderService = orderService;
            _paymentPluginManager = paymentPluginManager;
            _paymentService = paymentService;
            _priceFormatter = priceFormatter;
            _tvChannelService = tvChannelService;
            _rewardPointService = rewardPointService;
            _shipmentService = shipmentService;
            _stateProvinceService = stateProvinceService;
            _storeContext = storeContext;
            _storeService = storeService;
            _urlHelperFactory = urlHelperFactory;
            _urlRecordService = urlRecordService;
            _vendorAttributeFormatter = vendorAttributeFormatter;
            _workContext = workContext;
            _templatesSettings = templatesSettings;
            _paymentSettings = paymentSettings;
            _storeInformationSettings = storeInformationSettings;
            _taxSettings = taxSettings;
        }

        #endregion

        #region Allowed tokens

        /// <summary>
        /// Get all available tokens by token groups
        /// </summary>
        protected Dictionary<string, IEnumerable<string>> AllowedTokens
        {
            get
            {
                if (_allowedTokens != null)
                    return _allowedTokens;

                _allowedTokens = new Dictionary<string, IEnumerable<string>>();

                //store tokens
                _allowedTokens.Add(TokenGroupNames.StoreTokens, new[]
                {
                    "%Store.Name%",
                    "%Store.URL%",
                    "%Store.Email%",
                    "%Store.CompanyName%",
                    "%Store.CompanyAddress%",
                    "%Store.CompanyPhoneNumber%",
                    "%Store.CompanyVat%",
                    "%Facebook.URL%",
                    "%Twitter.URL%",
                    "%YouTube.URL%",
                    "%Instagram.URL%"
                });

                //user tokens
                _allowedTokens.Add(TokenGroupNames.UserTokens, new[]
                {
                    "%User.Email%",
                    "%User.Username%",
                    "%User.FullName%",
                    "%User.FirstName%",
                    "%User.LastName%",
                    "%User.MiddleName%",
                    "%User.VatNumber%",
                    "%User.VatNumberStatus%",
                    "%User.CustomAttributes%",
                    "%User.PasswordRecoveryURL%",
                    "%User.AccountActivationURL%",
                    "%User.EmailRevalidationURL%",
                    "%Wishlist.URLForUser%"
                });

                //order tokens
                _allowedTokens.Add(TokenGroupNames.OrderTokens, new[]
                {
                    "%Order.OrderNumber%",
                    "%Order.UserFullName%",
                    "%Order.UserEmail%",
                    "%Order.BillingFirstName%",
                    "%Order.BillingLastName%",
                    "%Order.BillingMiddleName%",
                    "%Order.BillingPhoneNumber%",
                    "%Order.BillingEmail%",
                    "%Order.BillingFaxNumber%",
                    "%Order.BillingCompany%",
                    "%Order.BillingAddress1%",
                    "%Order.BillingAddress2%",
                    "%Order.BillingCity%",
                    "%Order.BillingCounty%",
                    "%Order.BillingStateProvince%",
                    "%Order.BillingZipPostalCode%",
                    "%Order.BillingCountry%",
                    "%Order.BillingCustomAttributes%",
                    "%Order.Shippable%",
                    "%Order.ShippingMethod%",
                    "%Order.ShippingFirstName%",
                    "%Order.ShippingLastName%",
                    "%Order.ShippingMiddleName%",
                    "%Order.ShippingPhoneNumber%",
                    "%Order.ShippingEmail%",
                    "%Order.ShippingFaxNumber%",
                    "%Order.ShippingCompany%",
                    "%Order.ShippingAddress1%",
                    "%Order.ShippingAddress2%",
                    "%Order.ShippingCity%",
                    "%Order.ShippingCounty%",
                    "%Order.ShippingStateProvince%",
                    "%Order.ShippingZipPostalCode%",
                    "%Order.ShippingCountry%",
                    "%Order.ShippingCustomAttributes%",
                    "%Order.PaymentMethod%",
                    "%Order.VatNumber%",
                    "%Order.CustomValues%",
                    "%Order.TvChannel(s)%",
                    "%Order.CreatedOn%",
                    "%Order.OrderURLForUser%",
                    "%Order.PickupInStore%",
                    "%Order.OrderId%",
                    "%Order.IsCompletelyShipped%",
                    "%Order.IsCompletelyReadyForPickup%",
                    "%Order.IsCompletelyDelivered%"
                });

                //shipment tokens
                _allowedTokens.Add(TokenGroupNames.ShipmentTokens, new[]
                {
                    "%Shipment.ShipmentNumber%",
                    "%Shipment.TrackingNumber%",
                    "%Shipment.TrackingNumberURL%",
                    "%Shipment.TvChannel(s)%",
                    "%Shipment.URLForUser%"
                });

                //refunded order tokens
                _allowedTokens.Add(TokenGroupNames.RefundedOrderTokens, new[]
                {
                    "%Order.AmountRefunded%"
                });

                //order note tokens
                _allowedTokens.Add(TokenGroupNames.OrderNoteTokens, new[]
                {
                    "%Order.NewNoteText%",
                    "%Order.OrderNoteAttachmentUrl%"
                });

                //recurring payment tokens
                _allowedTokens.Add(TokenGroupNames.RecurringPaymentTokens, new[]
                {
                    "%RecurringPayment.ID%",
                    "%RecurringPayment.CancelAfterFailedPayment%",
                    "%RecurringPayment.RecurringPaymentType%"
                });

                //newsletter subscription tokens
                _allowedTokens.Add(TokenGroupNames.SubscriptionTokens, new[]
                {
                    "%NewsLetterSubscription.Email%",
                    "%NewsLetterSubscription.ActivationUrl%",
                    "%NewsLetterSubscription.DeactivationUrl%"
                });

                //tvChannel tokens
                _allowedTokens.Add(TokenGroupNames.TvChannelTokens, new[]
                {
                    "%TvChannel.ID%",
                    "%TvChannel.Name%",
                    "%TvChannel.ShortDescription%",
                    "%TvChannel.TvChannelURLForUser%",
                    "%TvChannel.SKU%",
                    "%TvChannel.StockQuantity%"
                });

                //return request tokens
                _allowedTokens.Add(TokenGroupNames.ReturnRequestTokens, new[]
                {
                    "%ReturnRequest.CustomNumber%",
                    "%ReturnRequest.OrderId%",
                    "%ReturnRequest.TvChannel.Quantity%",
                    "%ReturnRequest.TvChannel.Name%",
                    "%ReturnRequest.Reason%",
                    "%ReturnRequest.RequestedAction%",
                    "%ReturnRequest.UserComment%",
                    "%ReturnRequest.StaffNotes%",
                    "%ReturnRequest.Status%"
                });

                //forum tokens
                _allowedTokens.Add(TokenGroupNames.ForumTokens, new[]
                {
                    "%Forums.ForumURL%",
                    "%Forums.ForumName%"
                });

                //forum topic tokens
                _allowedTokens.Add(TokenGroupNames.ForumTopicTokens, new[]
                {
                    "%Forums.TopicURL%",
                    "%Forums.TopicName%"
                });

                //forum post tokens
                _allowedTokens.Add(TokenGroupNames.ForumPostTokens, new[]
                {
                    "%Forums.PostAuthor%",
                    "%Forums.PostBody%"
                });

                //private message tokens
                _allowedTokens.Add(TokenGroupNames.PrivateMessageTokens, new[]
                {
                    "%PrivateMessage.Subject%",
                    "%PrivateMessage.Text%"
                });

                //vendor tokens
                _allowedTokens.Add(TokenGroupNames.VendorTokens, new[]
                {
                    "%Vendor.Name%",
                    "%Vendor.Email%",
                    "%Vendor.VendorAttributes%"
                });

                //gift card tokens
                _allowedTokens.Add(TokenGroupNames.GiftCardTokens, new[]
                {
                    "%GiftCard.SenderName%",
                    "%GiftCard.SenderEmail%",
                    "%GiftCard.RecipientName%",
                    "%GiftCard.RecipientEmail%",
                    "%GiftCard.Amount%",
                    "%GiftCard.CouponCode%",
                    "%GiftCard.Message%"
                });

                //tvChannel review tokens
                _allowedTokens.Add(TokenGroupNames.TvChannelReviewTokens, new[]
                {
                    "%TvChannelReview.TvChannelName%",
                    "%TvChannelReview.Title%",
                    "%TvChannelReview.IsApproved%",
                    "%TvChannelReview.ReviewText%",
                    "%TvChannelReview.ReplyText%"
                });

                //attribute combination tokens
                _allowedTokens.Add(TokenGroupNames.AttributeCombinationTokens, new[]
                {
                    "%AttributeCombination.Formatted%",
                    "%AttributeCombination.SKU%",
                    "%AttributeCombination.StockQuantity%"
                });

                //blog comment tokens
                _allowedTokens.Add(TokenGroupNames.BlogCommentTokens, new[]
                {
                    "%BlogComment.BlogPostTitle%"
                });

                //news comment tokens
                _allowedTokens.Add(TokenGroupNames.NewsCommentTokens, new[]
                {
                    "%NewsComment.NewsTitle%"
                });

                //tvChannel back in stock tokens
                _allowedTokens.Add(TokenGroupNames.TvChannelBackInStockTokens, new[]
                {
                    "%BackInStockSubscription.TvChannelName%",
                    "%BackInStockSubscription.TvChannelUrl%"
                });

                //email a friend tokens
                _allowedTokens.Add(TokenGroupNames.EmailAFriendTokens, new[]
                {
                    "%EmailAFriend.PersonalMessage%",
                    "%EmailAFriend.Email%"
                });

                //wishlist to friend tokens
                _allowedTokens.Add(TokenGroupNames.WishlistToFriendTokens, new[]
                {
                    "%Wishlist.PersonalMessage%",
                    "%Wishlist.Email%"
                });

                //VAT validation tokens
                _allowedTokens.Add(TokenGroupNames.VatValidation, new[]
                {
                    "%VatValidationResult.Name%",
                    "%VatValidationResult.Address%"
                });

                //contact us tokens
                _allowedTokens.Add(TokenGroupNames.ContactUs, new[]
                {
                    "%ContactUs.SenderEmail%",
                    "%ContactUs.SenderName%",
                    "%ContactUs.Body%"
                });

                //contact vendor tokens
                _allowedTokens.Add(TokenGroupNames.ContactVendor, new[]
                {
                    "%ContactUs.SenderEmail%",
                    "%ContactUs.SenderName%",
                    "%ContactUs.Body%"
                });

                return _allowedTokens;
            }
        }

        #endregion

        #region Utilities

        /// <summary>
        /// Convert a collection to a HTML table
        /// </summary>
        /// <param name="order">Order</param>
        /// <param name="languageId">Language identifier</param>
        /// <param name="vendorId">Vendor identifier (used to limit tvChannels by vendor</param>
        /// <returns>
        /// Задача представляет асинхронную операцию
        /// The task result contains the hTML table of tvChannels
        /// </returns>
        protected virtual async Task<string> TvChannelListToHtmlTableAsync(Order order, int languageId, int vendorId)
        {
            var language = await _languageService.GetLanguageByIdAsync(languageId);

            var sb = new StringBuilder();
            sb.AppendLine("<table border=\"0\" style=\"width:100%;\">");

            sb.AppendLine($"<tr style=\"background-color:{_templatesSettings.Color1};text-align:center;\">");
            sb.AppendLine($"<th>{await _localizationService.GetResourceAsync("Messages.Order.TvChannel(s).Name", languageId)}</th>");
            sb.AppendLine($"<th>{await _localizationService.GetResourceAsync("Messages.Order.TvChannel(s).Price", languageId)}</th>");
            sb.AppendLine($"<th>{await _localizationService.GetResourceAsync("Messages.Order.TvChannel(s).Quantity", languageId)}</th>");
            sb.AppendLine($"<th>{await _localizationService.GetResourceAsync("Messages.Order.TvChannel(s).Total", languageId)}</th>");
            sb.AppendLine("</tr>");

            var table = await _orderService.GetOrderItemsAsync(order.Id, vendorId: vendorId);
            for (var i = 0; i <= table.Count - 1; i++)
            {
                var orderItem = table[i];

                var tvChannel = await _tvChannelService.GetTvChannelByIdAsync(orderItem.TvChannelId);

                if (tvChannel == null)
                    continue;

                sb.AppendLine($"<tr style=\"background-color: {_templatesSettings.Color2};text-align: center;\">");
                //tvChannel name
                var tvChannelName = await _localizationService.GetLocalizedAsync(tvChannel, x => x.Name, languageId);

                sb.AppendLine("<td style=\"padding: 0.6em 0.4em;text-align: left;\">" + WebUtility.HtmlEncode(tvChannelName));

                //add download link
                if (await _orderService.IsDownloadAllowedAsync(orderItem))
                {
                    var downloadUrl  = await RouteUrlAsync(order.StoreId, "GetDownload", new { orderItemId = orderItem.OrderItemGuid });
                    var downloadLink = $"<a class=\"link\" href=\"{downloadUrl}\">{await _localizationService.GetResourceAsync("Messages.Order.TvChannel(s).Download", languageId)}</a>";
                    sb.AppendLine("<br />");
                    sb.AppendLine(downloadLink);
                }
                //add download link
                if (await _orderService.IsLicenseDownloadAllowedAsync(orderItem))
                {
                    var licenseUrl  = await RouteUrlAsync(order.StoreId, "GetLicense", new { orderItemId = orderItem.OrderItemGuid });
                    var licenseLink = $"<a class=\"link\" href=\"{licenseUrl}\">{await _localizationService.GetResourceAsync("Messages.Order.TvChannel(s).License", languageId)}</a>";
                    sb.AppendLine("<br />");
                    sb.AppendLine(licenseLink);
                }
                //attributes
                if (!string.IsNullOrEmpty(orderItem.AttributeDescription))
                {
                    sb.AppendLine("<br />");
                    sb.AppendLine(orderItem.AttributeDescription);
                }
                //rental info
                if (tvChannel.IsRental)
                {
                    var rentalStartDate = orderItem.RentalStartDateUtc.HasValue
                        ? _tvChannelService.FormatRentalDate(tvChannel, orderItem.RentalStartDateUtc.Value) : string.Empty;
                    var rentalEndDate = orderItem.RentalEndDateUtc.HasValue
                        ? _tvChannelService.FormatRentalDate(tvChannel, orderItem.RentalEndDateUtc.Value) : string.Empty;
                    var rentalInfo = string.Format(await _localizationService.GetResourceAsync("Order.Rental.FormattedDate"),
                        rentalStartDate, rentalEndDate);
                    sb.AppendLine("<br />");
                    sb.AppendLine(rentalInfo);
                }
                //SKU
                if (_catalogSettings.ShowSkuOnTvChannelDetailsPage)
                {
                    var sku = await _tvChannelService.FormatSkuAsync(tvChannel, orderItem.AttributesXml);
                    if (!string.IsNullOrEmpty(sku))
                    {
                        sb.AppendLine("<br />");
                        sb.AppendLine(string.Format(await _localizationService.GetResourceAsync("Messages.Order.TvChannel(s).SKU", languageId), WebUtility.HtmlEncode(sku)));
                    }
                }

                sb.AppendLine("</td>");

                string unitPriceStr;
                if (order.UserTaxDisplayType == TaxDisplayType.IncludingTax)
                {
                    //including tax
                    var unitPriceInclTaxInUserCurrency = _currencyService.ConvertCurrency(orderItem.UnitPriceInclTax, order.CurrencyRate);
                    unitPriceStr = await _priceFormatter.FormatPriceAsync(unitPriceInclTaxInUserCurrency, true, order.UserCurrencyCode, languageId, true);
                }
                else
                {
                    //excluding tax
                    var unitPriceExclTaxInUserCurrency = _currencyService.ConvertCurrency(orderItem.UnitPriceExclTax, order.CurrencyRate);
                    unitPriceStr = await _priceFormatter.FormatPriceAsync(unitPriceExclTaxInUserCurrency, true, order.UserCurrencyCode, languageId, false);
                }

                sb.AppendLine($"<td style=\"padding: 0.6em 0.4em;text-align: right;\">{unitPriceStr}</td>");

                sb.AppendLine($"<td style=\"padding: 0.6em 0.4em;text-align: center;\">{orderItem.Quantity}</td>");

                string priceStr;
                if (order.UserTaxDisplayType == TaxDisplayType.IncludingTax)
                {
                    //including tax
                    var priceInclTaxInUserCurrency = _currencyService.ConvertCurrency(orderItem.PriceInclTax, order.CurrencyRate);
                    priceStr = await _priceFormatter.FormatPriceAsync(priceInclTaxInUserCurrency, true, order.UserCurrencyCode, languageId, true);
                }
                else
                {
                    //excluding tax
                    var priceExclTaxInUserCurrency = _currencyService.ConvertCurrency(orderItem.PriceExclTax, order.CurrencyRate);
                    priceStr = await _priceFormatter.FormatPriceAsync(priceExclTaxInUserCurrency, true, order.UserCurrencyCode, languageId, false);
                }

                sb.AppendLine($"<td style=\"padding: 0.6em 0.4em;text-align: right;\">{priceStr}</td>");

                sb.AppendLine("</tr>");
            }

            if (vendorId == 0)
            {
                //we render checkout attributes and totals only for store owners (hide for vendors)

                if (!string.IsNullOrEmpty(order.CheckoutAttributeDescription))
                {
                    sb.AppendLine("<tr><td style=\"text-align:right;\" colspan=\"1\">&nbsp;</td><td colspan=\"3\" style=\"text-align:right\">");
                    sb.AppendLine(order.CheckoutAttributeDescription);
                    sb.AppendLine("</td></tr>");
                }

                //totals
                await WriteTotalsAsync(order, language, sb);
            }

            sb.AppendLine("</table>");
            var result = sb.ToString();
            return result;
        }

        /// <summary>
        /// Write order totals
        /// </summary>
        /// <param name="order">Order</param>
        /// <param name="language">Language</param>
        /// <param name="sb">StringBuilder</param>
        /// <returns>Задача представляет асинхронную операцию</returns>
        protected virtual async Task WriteTotalsAsync(Order order, Language language, StringBuilder sb)
        {
            //subtotal
            string cusSubTotal;
            var displaySubTotalDiscount = false;
            var cusSubTotalDiscount = string.Empty;
            var languageId = language.Id;
            if (order.UserTaxDisplayType == TaxDisplayType.IncludingTax && !_taxSettings.ForceTaxExclusionFromOrderSubtotal)
            {
                //including tax

                //subtotal
                var orderSubtotalInclTaxInUserCurrency = _currencyService.ConvertCurrency(order.OrderSubtotalInclTax, order.CurrencyRate);
                cusSubTotal = await _priceFormatter.FormatPriceAsync(orderSubtotalInclTaxInUserCurrency, true, order.UserCurrencyCode, languageId, true);
                //discount (applied to order subtotal)
                var orderSubTotalDiscountInclTaxInUserCurrency = _currencyService.ConvertCurrency(order.OrderSubTotalDiscountInclTax, order.CurrencyRate);
                if (orderSubTotalDiscountInclTaxInUserCurrency > decimal.Zero)
                {
                    cusSubTotalDiscount = await _priceFormatter.FormatPriceAsync(-orderSubTotalDiscountInclTaxInUserCurrency, true, order.UserCurrencyCode, languageId, true);
                    displaySubTotalDiscount = true;
                }
            }
            else
            {
                //excluding tax

                //subtotal
                var orderSubtotalExclTaxInUserCurrency = _currencyService.ConvertCurrency(order.OrderSubtotalExclTax, order.CurrencyRate);
                cusSubTotal = await _priceFormatter.FormatPriceAsync(orderSubtotalExclTaxInUserCurrency, true, order.UserCurrencyCode, languageId, false);
                //discount (applied to order subtotal)
                var orderSubTotalDiscountExclTaxInUserCurrency = _currencyService.ConvertCurrency(order.OrderSubTotalDiscountExclTax, order.CurrencyRate);
                if (orderSubTotalDiscountExclTaxInUserCurrency > decimal.Zero)
                {
                    cusSubTotalDiscount = await _priceFormatter.FormatPriceAsync(-orderSubTotalDiscountExclTaxInUserCurrency, true, order.UserCurrencyCode, languageId, false);
                    displaySubTotalDiscount = true;
                }
            }

            //shipping, payment method fee
            string cusShipTotal;
            string cusPaymentMethodAdditionalFee;
            var taxRates = new SortedDictionary<decimal, decimal>();
            var cusTaxTotal = string.Empty;
            var cusDiscount = string.Empty;
            if (order.UserTaxDisplayType == TaxDisplayType.IncludingTax)
            {
                //including tax

                //shipping
                var orderShippingInclTaxInUserCurrency = _currencyService.ConvertCurrency(order.OrderShippingInclTax, order.CurrencyRate);
                cusShipTotal = await _priceFormatter.FormatShippingPriceAsync(orderShippingInclTaxInUserCurrency, true, order.UserCurrencyCode, languageId, true);
                //payment method additional fee
                var paymentMethodAdditionalFeeInclTaxInUserCurrency = _currencyService.ConvertCurrency(order.PaymentMethodAdditionalFeeInclTax, order.CurrencyRate);
                cusPaymentMethodAdditionalFee = await _priceFormatter.FormatPaymentMethodAdditionalFeeAsync(paymentMethodAdditionalFeeInclTaxInUserCurrency, true, order.UserCurrencyCode, languageId, true);
            }
            else
            {
                //excluding tax

                //shipping
                var orderShippingExclTaxInUserCurrency = _currencyService.ConvertCurrency(order.OrderShippingExclTax, order.CurrencyRate);
                cusShipTotal = await _priceFormatter.FormatShippingPriceAsync(orderShippingExclTaxInUserCurrency, true, order.UserCurrencyCode, languageId, false);
                //payment method additional fee
                var paymentMethodAdditionalFeeExclTaxInUserCurrency = _currencyService.ConvertCurrency(order.PaymentMethodAdditionalFeeExclTax, order.CurrencyRate);
                cusPaymentMethodAdditionalFee = await _priceFormatter.FormatPaymentMethodAdditionalFeeAsync(paymentMethodAdditionalFeeExclTaxInUserCurrency, true, order.UserCurrencyCode, languageId, false);
            }

            //shipping
            var displayShipping = order.ShippingStatus != ShippingStatus.ShippingNotRequired;

            //payment method fee
            var displayPaymentMethodFee = order.PaymentMethodAdditionalFeeExclTax > decimal.Zero;

            //tax
            bool displayTax;
            bool displayTaxRates;
            if (_taxSettings.HideTaxInOrderSummary && order.UserTaxDisplayType == TaxDisplayType.IncludingTax)
            {
                displayTax = false;
                displayTaxRates = false;
            }
            else
            {
                if (order.OrderTax == 0 && _taxSettings.HideZeroTax)
                {
                    displayTax = false;
                    displayTaxRates = false;
                }
                else
                {
                    taxRates = new SortedDictionary<decimal, decimal>();
                    foreach (var tr in _orderService.ParseTaxRates(order, order.TaxRates))
                        taxRates.Add(tr.Key, _currencyService.ConvertCurrency(tr.Value, order.CurrencyRate));

                    displayTaxRates = _taxSettings.DisplayTaxRates && taxRates.Any();
                    displayTax = !displayTaxRates;

                    var orderTaxInUserCurrency = _currencyService.ConvertCurrency(order.OrderTax, order.CurrencyRate);
                    var taxStr = await _priceFormatter.FormatPriceAsync(orderTaxInUserCurrency, true, order.UserCurrencyCode,
                        false, languageId);
                    cusTaxTotal = taxStr;
                }
            }

            //discount
            var displayDiscount = false;
            if (order.OrderDiscount > decimal.Zero)
            {
                var orderDiscountInUserCurrency = _currencyService.ConvertCurrency(order.OrderDiscount, order.CurrencyRate);
                cusDiscount = await _priceFormatter.FormatPriceAsync(-orderDiscountInUserCurrency, true, order.UserCurrencyCode, false, languageId);
                displayDiscount = true;
            }

            //total
            var orderTotalInUserCurrency = _currencyService.ConvertCurrency(order.OrderTotal, order.CurrencyRate);
            var cusTotal = await _priceFormatter.FormatPriceAsync(orderTotalInUserCurrency, true, order.UserCurrencyCode, false, languageId);

            //subtotal
            sb.AppendLine($"<tr style=\"text-align:right;\"><td>&nbsp;</td><td colspan=\"2\" style=\"background-color: {_templatesSettings.Color3};padding:0.6em 0.4 em;\"><strong>{await _localizationService.GetResourceAsync("Messages.Order.SubTotal", languageId)}</strong></td> <td style=\"background-color: {_templatesSettings.Color3};padding:0.6em 0.4 em;\"><strong>{cusSubTotal}</strong></td></tr>");

            //discount (applied to order subtotal)
            if (displaySubTotalDiscount)
            {
                sb.AppendLine($"<tr style=\"text-align:right;\"><td>&nbsp;</td><td colspan=\"2\" style=\"background-color: {_templatesSettings.Color3};padding:0.6em 0.4 em;\"><strong>{await _localizationService.GetResourceAsync("Messages.Order.SubTotalDiscount", languageId)}</strong></td> <td style=\"background-color: {_templatesSettings.Color3};padding:0.6em 0.4 em;\"><strong>{cusSubTotalDiscount}</strong></td></tr>");
            }

            //shipping
            if (displayShipping)
            {
                sb.AppendLine($"<tr style=\"text-align:right;\"><td>&nbsp;</td><td colspan=\"2\" style=\"background-color: {_templatesSettings.Color3};padding:0.6em 0.4 em;\"><strong>{await _localizationService.GetResourceAsync("Messages.Order.Shipping", languageId)}</strong></td> <td style=\"background-color: {_templatesSettings.Color3};padding:0.6em 0.4 em;\"><strong>{cusShipTotal}</strong></td></tr>");
            }

            //payment method fee
            if (displayPaymentMethodFee)
            {
                var paymentMethodFeeTitle = await _localizationService.GetResourceAsync("Messages.Order.PaymentMethodAdditionalFee", languageId);
                sb.AppendLine($"<tr style=\"text-align:right;\"><td>&nbsp;</td><td colspan=\"2\" style=\"background-color: {_templatesSettings.Color3};padding:0.6em 0.4 em;\"><strong>{paymentMethodFeeTitle}</strong></td> <td style=\"background-color: {_templatesSettings.Color3};padding:0.6em 0.4 em;\"><strong>{cusPaymentMethodAdditionalFee}</strong></td></tr>");
            }

            //tax
            if (displayTax)
            {
                sb.AppendLine($"<tr style=\"text-align:right;\"><td>&nbsp;</td><td colspan=\"2\" style=\"background-color: {_templatesSettings.Color3};padding:0.6em 0.4 em;\"><strong>{await _localizationService.GetResourceAsync("Messages.Order.Tax", languageId)}</strong></td> <td style=\"background-color: {_templatesSettings.Color3};padding:0.6em 0.4 em;\"><strong>{cusTaxTotal}</strong></td></tr>");
            }

            if (displayTaxRates)
            {
                foreach (var item in taxRates)
                {
                    var taxRate = string.Format(await _localizationService.GetResourceAsync("Messages.Order.TaxRateLine"),
                        _priceFormatter.FormatTaxRate(item.Key));
                    var taxValue = await _priceFormatter.FormatPriceAsync(item.Value, true, order.UserCurrencyCode, false, languageId);
                    sb.AppendLine($"<tr style=\"text-align:right;\"><td>&nbsp;</td><td colspan=\"2\" style=\"background-color: {_templatesSettings.Color3};padding:0.6em 0.4 em;\"><strong>{taxRate}</strong></td> <td style=\"background-color: {_templatesSettings.Color3};padding:0.6em 0.4 em;\"><strong>{taxValue}</strong></td></tr>");
                }
            }

            //discount
            if (displayDiscount)
            {
                sb.AppendLine($"<tr style=\"text-align:right;\"><td>&nbsp;</td><td colspan=\"2\" style=\"background-color: {_templatesSettings.Color3};padding:0.6em 0.4 em;\"><strong>{await _localizationService.GetResourceAsync("Messages.Order.TotalDiscount", languageId)}</strong></td> <td style=\"background-color: {_templatesSettings.Color3};padding:0.6em 0.4 em;\"><strong>{cusDiscount}</strong></td></tr>");
            }

            //gift cards
            foreach (var gcuh in await _giftCardService.GetGiftCardUsageHistoryAsync(order))
            {
                var giftCardText = string.Format(await _localizationService.GetResourceAsync("Messages.Order.GiftCardInfo", languageId),
                    WebUtility.HtmlEncode((await _giftCardService.GetGiftCardByIdAsync(gcuh.GiftCardId))?.GiftCardCouponCode));
                var giftCardAmount = await _priceFormatter.FormatPriceAsync(-_currencyService.ConvertCurrency(gcuh.UsedValue, order.CurrencyRate), true, order.UserCurrencyCode,
                    false, languageId);
                sb.AppendLine($"<tr style=\"text-align:right;\"><td>&nbsp;</td><td colspan=\"2\" style=\"background-color: {_templatesSettings.Color3};padding:0.6em 0.4 em;\"><strong>{giftCardText}</strong></td> <td style=\"background-color: {_templatesSettings.Color3};padding:0.6em 0.4 em;\"><strong>{giftCardAmount}</strong></td></tr>");
            }

            //reward points
            if (order.RedeemedRewardPointsEntryId.HasValue && await _rewardPointService.GetRewardPointsHistoryEntryByIdAsync(order.RedeemedRewardPointsEntryId.Value) is RewardPointsHistory redeemedRewardPointsEntry)
            {
                var rpTitle = string.Format(await _localizationService.GetResourceAsync("Messages.Order.RewardPoints", languageId),
                    -redeemedRewardPointsEntry.Points);
                var rpAmount = await _priceFormatter.FormatPriceAsync(-_currencyService.ConvertCurrency(redeemedRewardPointsEntry.UsedAmount, order.CurrencyRate), true,
                    order.UserCurrencyCode, false, languageId);
                sb.AppendLine($"<tr style=\"text-align:right;\"><td>&nbsp;</td><td colspan=\"2\" style=\"background-color: {_templatesSettings.Color3};padding:0.6em 0.4 em;\"><strong>{rpTitle}</strong></td> <td style=\"background-color: {_templatesSettings.Color3};padding:0.6em 0.4 em;\"><strong>{rpAmount}</strong></td></tr>");
            }

            //total
            sb.AppendLine($"<tr style=\"text-align:right;\"><td>&nbsp;</td><td colspan=\"2\" style=\"background-color: {_templatesSettings.Color3};padding:0.6em 0.4 em;\"><strong>{await _localizationService.GetResourceAsync("Messages.Order.OrderTotal", languageId)}</strong></td> <td style=\"background-color: {_templatesSettings.Color3};padding:0.6em 0.4 em;\"><strong>{cusTotal}</strong></td></tr>");
        }

        /// <summary>
        /// Convert a collection to a HTML table
        /// </summary>
        /// <param name="shipment">Shipment</param>
        /// <param name="languageId">Language identifier</param>
        /// <returns>
        /// Задача представляет асинхронную операцию
        /// The task result contains the hTML table of tvChannels
        /// </returns>
        protected virtual async Task<string> TvChannelListToHtmlTableAsync(Shipment shipment, int languageId)
        {
            var sb = new StringBuilder();
            sb.AppendLine("<table border=\"0\" style=\"width:100%;\">");

            sb.AppendLine($"<tr style=\"background-color:{_templatesSettings.Color1};text-align:center;\">");
            sb.AppendLine($"<th>{await _localizationService.GetResourceAsync("Messages.Order.TvChannel(s).Name", languageId)}</th>");
            sb.AppendLine($"<th>{await _localizationService.GetResourceAsync("Messages.Order.TvChannel(s).Quantity", languageId)}</th>");
            sb.AppendLine("</tr>");

            var table = await _shipmentService.GetShipmentItemsByShipmentIdAsync(shipment.Id);
            for (var i = 0; i <= table.Count - 1; i++)
            {
                var si = table[i];
                var orderItem = await _orderService.GetOrderItemByIdAsync(si.OrderItemId);

                if (orderItem == null)
                    continue;

                var tvChannel = await _tvChannelService.GetTvChannelByIdAsync(orderItem.TvChannelId);

                if (tvChannel == null)
                    continue;

                sb.AppendLine($"<tr style=\"background-color: {_templatesSettings.Color2};text-align: center;\">");
                //tvChannel name
                var tvChannelName = await _localizationService.GetLocalizedAsync(tvChannel, x => x.Name, languageId);

                sb.AppendLine("<td style=\"padding: 0.6em 0.4em;text-align: left;\">" + WebUtility.HtmlEncode(tvChannelName));

                //attributes
                if (!string.IsNullOrEmpty(orderItem.AttributeDescription))
                {
                    sb.AppendLine("<br />");
                    sb.AppendLine(orderItem.AttributeDescription);
                }

                //rental info
                if (tvChannel.IsRental)
                {
                    var rentalStartDate = orderItem.RentalStartDateUtc.HasValue
                        ? _tvChannelService.FormatRentalDate(tvChannel, orderItem.RentalStartDateUtc.Value) : string.Empty;
                    var rentalEndDate = orderItem.RentalEndDateUtc.HasValue
                        ? _tvChannelService.FormatRentalDate(tvChannel, orderItem.RentalEndDateUtc.Value) : string.Empty;
                    var rentalInfo = string.Format(await _localizationService.GetResourceAsync("Order.Rental.FormattedDate"),
                        rentalStartDate, rentalEndDate);
                    sb.AppendLine("<br />");
                    sb.AppendLine(rentalInfo);
                }

                //SKU
                if (_catalogSettings.ShowSkuOnTvChannelDetailsPage)
                {
                    var sku = await _tvChannelService.FormatSkuAsync(tvChannel, orderItem.AttributesXml);
                    if (!string.IsNullOrEmpty(sku))
                    {
                        sb.AppendLine("<br />");
                        sb.AppendLine(string.Format(await _localizationService.GetResourceAsync("Messages.Order.TvChannel(s).SKU", languageId), WebUtility.HtmlEncode(sku)));
                    }
                }

                sb.AppendLine("</td>");

                sb.AppendLine($"<td style=\"padding: 0.6em 0.4em;text-align: center;\">{si.Quantity}</td>");

                sb.AppendLine("</tr>");
            }

            sb.AppendLine("</table>");
            var result = sb.ToString();
            return result;
        }

        /// <summary>
        /// Generates an absolute URL for the specified store, routeName and route values
        /// </summary>
        /// <param name="storeId">Store identifier; Pass 0 to load URL of the current store</param>
        /// <param name="routeName">The name of the route that is used to generate URL</param>
        /// <param name="routeValues">An object that contains route values</param>
        /// <returns>
        /// Задача представляет асинхронную операцию
        /// The task result contains the generated URL
        /// </returns>
        protected virtual async Task<string> RouteUrlAsync(int storeId = 0, string routeName = null, object routeValues = null)
        {
            //try to get a store by the passed identifier
            var store = await _storeService.GetStoreByIdAsync(storeId) ?? await _storeContext.GetCurrentStoreAsync()
                ?? throw new Exception("No store could be loaded");

            //ensure that the store URL is specified
            if (string.IsNullOrEmpty(store.Url))
                throw new Exception("URL cannot be null");

            //generate the relative URL
            var urlHelper = _urlHelperFactory.GetUrlHelper(_actionContextAccessor.ActionContext);
            var url = urlHelper.RouteUrl(routeName, routeValues);

            //compose the result
            return new Uri(new Uri(store.Url), url).AbsoluteUri;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Add store tokens
        /// </summary>
        /// <param name="tokens">List of already added tokens</param>
        /// <param name="store">Store</param>
        /// <param name="emailAccount">Email account</param>
        /// <returns>Задача представляет асинхронную операцию</returns>
        public virtual async Task AddStoreTokensAsync(IList<Token> tokens, Store store, EmailAccount emailAccount)
        {
            if (emailAccount == null)
                throw new ArgumentNullException(nameof(emailAccount));

            tokens.Add(new Token("Store.Name", await _localizationService.GetLocalizedAsync(store, x => x.Name)));
            tokens.Add(new Token("Store.URL", store.Url, true));
            tokens.Add(new Token("Store.Email", emailAccount.Email));
            tokens.Add(new Token("Store.CompanyName", store.CompanyName));
            tokens.Add(new Token("Store.CompanyAddress", store.CompanyAddress));
            tokens.Add(new Token("Store.CompanyPhoneNumber", store.CompanyPhoneNumber));
            tokens.Add(new Token("Store.CompanyVat", store.CompanyVat));

            tokens.Add(new Token("Facebook.URL", _storeInformationSettings.FacebookLink));
            tokens.Add(new Token("Twitter.URL", _storeInformationSettings.TwitterLink));
            tokens.Add(new Token("YouTube.URL", _storeInformationSettings.YoutubeLink));
            tokens.Add(new Token("Instagram.URL", _storeInformationSettings.InstagramLink));

            //event notification
            await _eventPublisher.EntityTokensAddedAsync(store, tokens);
        }

        /// <summary>
        /// Add order tokens
        /// </summary>
        /// <param name="tokens">List of already added tokens</param>
        /// <param name="order"></param>
        /// <param name="languageId">Language identifier</param>
        /// <param name="vendorId">Vendor identifier</param>
        /// <returns>Задача представляет асинхронную операцию</returns>
        public virtual async Task AddOrderTokensAsync(IList<Token> tokens, Order order, int languageId, int vendorId = 0)
        {
            //lambda expression for choosing correct order address
            async Task<Address> orderAddress(Order o) => await _addressService.GetAddressByIdAsync((o.PickupInStore ? o.PickupAddressId : o.ShippingAddressId) ?? 0);

            var billingAddress = await _addressService.GetAddressByIdAsync(order.BillingAddressId);

            tokens.Add(new Token("Order.OrderId", order.Id));
            tokens.Add(new Token("Order.OrderNumber", order.CustomOrderNumber));

            tokens.Add(new Token("Order.UserFullName", $"{billingAddress.LastName} {billingAddress.FirstName} {billingAddress.MiddleName}"));
            tokens.Add(new Token("Order.UserEmail", billingAddress.Email));

            tokens.Add(new Token("Order.BillingFirstName", billingAddress.FirstName));
            tokens.Add(new Token("Order.BillingLastName", billingAddress.LastName));
            tokens.Add(new Token("Order.BillingMiddleName", billingAddress.MiddleName));
            tokens.Add(new Token("Order.BillingPhoneNumber", billingAddress.PhoneNumber));
            tokens.Add(new Token("Order.BillingEmail", billingAddress.Email));
            tokens.Add(new Token("Order.BillingFaxNumber", billingAddress.FaxNumber));
            tokens.Add(new Token("Order.BillingCompany", billingAddress.Company));
            tokens.Add(new Token("Order.BillingAddress1", billingAddress.Address1));
            tokens.Add(new Token("Order.BillingAddress2", billingAddress.Address2));
            tokens.Add(new Token("Order.BillingCity", billingAddress.City));
            tokens.Add(new Token("Order.BillingCounty", billingAddress.County));
            tokens.Add(new Token("Order.BillingStateProvince", await _stateProvinceService.GetStateProvinceByAddressAsync(billingAddress) is StateProvince billingStateProvince ? await _localizationService.GetLocalizedAsync(billingStateProvince, x => x.Name) : string.Empty));
            tokens.Add(new Token("Order.BillingZipPostalCode", billingAddress.ZipPostalCode));
            tokens.Add(new Token("Order.BillingCountry", await _countryService.GetCountryByAddressAsync(billingAddress) is Country billingCountry ? await _localizationService.GetLocalizedAsync(billingCountry, x => x.Name) : string.Empty));
            tokens.Add(new Token("Order.BillingCustomAttributes", await _addressAttributeFormatter.FormatAttributesAsync(billingAddress.CustomAttributes), true));

            tokens.Add(new Token("Order.Shippable", !string.IsNullOrEmpty(order.ShippingMethod)));
            tokens.Add(new Token("Order.ShippingMethod", order.ShippingMethod));
            tokens.Add(new Token("Order.PickupInStore", order.PickupInStore));
            tokens.Add(new Token("Order.ShippingFirstName", (await orderAddress(order))?.FirstName ?? string.Empty));
            tokens.Add(new Token("Order.ShippingLastName", (await orderAddress(order))?.LastName ?? string.Empty));
            tokens.Add(new Token("Order.ShippingMiddleName", (await orderAddress(order))?.MiddleName ?? string.Empty));
            tokens.Add(new Token("Order.ShippingPhoneNumber", (await orderAddress(order))?.PhoneNumber ?? string.Empty));
            tokens.Add(new Token("Order.ShippingEmail", (await orderAddress(order))?.Email ?? string.Empty));
            tokens.Add(new Token("Order.ShippingFaxNumber", (await orderAddress(order))?.FaxNumber ?? string.Empty));
            tokens.Add(new Token("Order.ShippingCompany", (await orderAddress(order))?.Company ?? string.Empty));
            tokens.Add(new Token("Order.ShippingAddress1", (await orderAddress(order))?.Address1 ?? string.Empty));
            tokens.Add(new Token("Order.ShippingAddress2", (await orderAddress(order))?.Address2 ?? string.Empty));
            tokens.Add(new Token("Order.ShippingCity", (await orderAddress(order))?.City ?? string.Empty));
            tokens.Add(new Token("Order.ShippingCounty", (await orderAddress(order))?.County ?? string.Empty));
            tokens.Add(new Token("Order.ShippingStateProvince", await _stateProvinceService.GetStateProvinceByAddressAsync(await orderAddress(order)) is StateProvince shippingStateProvince ? await _localizationService.GetLocalizedAsync(shippingStateProvince, x => x.Name) : string.Empty));
            tokens.Add(new Token("Order.ShippingZipPostalCode", (await orderAddress(order))?.ZipPostalCode ?? string.Empty));
            tokens.Add(new Token("Order.ShippingCountry", await _countryService.GetCountryByAddressAsync(await orderAddress(order)) is Country orderCountry ? await _localizationService.GetLocalizedAsync(orderCountry, x => x.Name) : string.Empty));
            tokens.Add(new Token("Order.ShippingCustomAttributes", await _addressAttributeFormatter.FormatAttributesAsync((await orderAddress(order))?.CustomAttributes ?? string.Empty), true));
            tokens.Add(new Token("Order.IsCompletelyShipped", !order.PickupInStore && order.ShippingStatus == ShippingStatus.Shipped));
            tokens.Add(new Token("Order.IsCompletelyReadyForPickup", order.PickupInStore && !await _orderService.HasItemsToAddToShipmentAsync(order) && !await _orderService.HasItemsToReadyForPickupAsync(order)));
            tokens.Add(new Token("Order.IsCompletelyDelivered", order.ShippingStatus == ShippingStatus.Delivered));

            var paymentMethod = await _paymentPluginManager.LoadPluginBySystemNameAsync(order.PaymentMethodSystemName);
            var paymentMethodName = paymentMethod != null ? await _localizationService.GetLocalizedFriendlyNameAsync(paymentMethod, (await _workContext.GetWorkingLanguageAsync()).Id) : order.PaymentMethodSystemName;
            tokens.Add(new Token("Order.PaymentMethod", paymentMethodName));
            tokens.Add(new Token("Order.VatNumber", order.VatNumber));
            var sbCustomValues = new StringBuilder();
            var customValues = _paymentService.DeserializeCustomValues(order);
            if (customValues != null)
            {
                foreach (var item in customValues)
                {
                    sbCustomValues.AppendFormat("{0}: {1}", WebUtility.HtmlEncode(item.Key), WebUtility.HtmlEncode(item.Value != null ? item.Value.ToString() : string.Empty));
                    sbCustomValues.Append("<br />");
                }
            }

            tokens.Add(new Token("Order.CustomValues", sbCustomValues.ToString(), true));

            tokens.Add(new Token("Order.TvChannel(s)", await TvChannelListToHtmlTableAsync(order, languageId, vendorId), true));

            var language = await _languageService.GetLanguageByIdAsync(languageId);
            if (language != null && !string.IsNullOrEmpty(language.LanguageCulture))
            {
                var user = await _userService.GetUserByIdAsync(order.UserId);
                var createdOn = _dateTimeHelper.ConvertToUserTime(order.CreatedOnUtc, TimeZoneInfo.Utc, await _dateTimeHelper.GetUserTimeZoneAsync(user));
                tokens.Add(new Token("Order.CreatedOn", createdOn.ToString("D", new CultureInfo(language.LanguageCulture))));
            }
            else
            {
                tokens.Add(new Token("Order.CreatedOn", order.CreatedOnUtc.ToString("D")));
            }

            var orderUrl = await RouteUrlAsync(order.StoreId, "OrderDetails", new { orderId = order.Id });
            tokens.Add(new Token("Order.OrderURLForUser", orderUrl, true));

            //event notification
            await _eventPublisher.EntityTokensAddedAsync(order, tokens);
        }

        /// <summary>
        /// Add refunded order tokens
        /// </summary>
        /// <param name="tokens">List of already added tokens</param>
        /// <param name="order">Order</param>
        /// <param name="refundedAmount">Refunded amount of order</param>
        /// <returns>Задача представляет асинхронную операцию</returns>
        public virtual async Task AddOrderRefundedTokensAsync(IList<Token> tokens, Order order, decimal refundedAmount)
        {
            //should we convert it to user currency?
            //most probably, no. It can cause some rounding or legal issues
            //furthermore, exchange rate could be changed
            //so let's display it the primary store currency

            var primaryStoreCurrencyCode = (await _currencyService.GetCurrencyByIdAsync(_currencySettings.PrimaryStoreCurrencyId)).CurrencyCode;
            var refundedAmountStr = await _priceFormatter.FormatPriceAsync(refundedAmount, true, primaryStoreCurrencyCode, false, (await _workContext.GetWorkingLanguageAsync()).Id);

            tokens.Add(new Token("Order.AmountRefunded", refundedAmountStr));

            //event notification
            await _eventPublisher.EntityTokensAddedAsync(order, tokens);
        }

        /// <summary>
        /// Add shipment tokens
        /// </summary>
        /// <param name="tokens">List of already added tokens</param>
        /// <param name="shipment">Shipment item</param>
        /// <param name="languageId">Language identifier</param>
        /// <returns>Задача представляет асинхронную операцию</returns>
        public virtual async Task AddShipmentTokensAsync(IList<Token> tokens, Shipment shipment, int languageId)
        {
            tokens.Add(new Token("Shipment.ShipmentNumber", shipment.Id));
            tokens.Add(new Token("Shipment.TrackingNumber", shipment.TrackingNumber));
            var trackingNumberUrl = string.Empty;
            if (!string.IsNullOrEmpty(shipment.TrackingNumber))
            {
                var shipmentTracker = await _shipmentService.GetShipmentTrackerAsync(shipment);
                if (shipmentTracker != null)
                    trackingNumberUrl = await shipmentTracker.GetUrlAsync(shipment.TrackingNumber, shipment);
            }

            tokens.Add(new Token("Shipment.TrackingNumberURL", trackingNumberUrl, true));
            tokens.Add(new Token("Shipment.TvChannel(s)", await TvChannelListToHtmlTableAsync(shipment, languageId), true));

            var shipmentUrl  = await RouteUrlAsync((await _orderService.GetOrderByIdAsync(shipment.OrderId)).StoreId, "ShipmentDetails", new { shipmentId = shipment.Id });
            tokens.Add(new Token("Shipment.URLForUser", shipmentUrl, true));

            //event notification
            await _eventPublisher.EntityTokensAddedAsync(shipment, tokens);
        }

        /// <summary>
        /// Add order note tokens
        /// </summary>
        /// <param name="tokens">List of already added tokens</param>
        /// <param name="orderNote">Order note</param>
        /// <returns>Задача представляет асинхронную операцию</returns>
        public virtual async Task AddOrderNoteTokensAsync(IList<Token> tokens, OrderNote orderNote)
        {
            var order = await _orderService.GetOrderByIdAsync(orderNote.OrderId);

            tokens.Add(new Token("Order.NewNoteText", _orderService.FormatOrderNoteText(orderNote), true));
            var orderNoteAttachmentUrl  = await RouteUrlAsync(order.StoreId, "GetOrderNoteFile", new { ordernoteid = orderNote.Id });
            tokens.Add(new Token("Order.OrderNoteAttachmentUrl", orderNoteAttachmentUrl, true));

            //event notification
            await _eventPublisher.EntityTokensAddedAsync(orderNote, tokens);
        }

        /// <summary>
        /// Add recurring payment tokens
        /// </summary>
        /// <param name="tokens">List of already added tokens</param>
        /// <param name="recurringPayment">Recurring payment</param>
        /// <returns>Задача представляет асинхронную операцию</returns>
        public virtual async Task AddRecurringPaymentTokensAsync(IList<Token> tokens, RecurringPayment recurringPayment)
        {
            tokens.Add(new Token("RecurringPayment.ID", recurringPayment.Id));
            tokens.Add(new Token("RecurringPayment.CancelAfterFailedPayment",
                recurringPayment.LastPaymentFailed && _paymentSettings.CancelRecurringPaymentsAfterFailedPayment));
            if (await _orderService.GetOrderByIdAsync(recurringPayment.InitialOrderId) is Order order)
                tokens.Add(new Token("RecurringPayment.RecurringPaymentType", (await _paymentService.GetRecurringPaymentTypeAsync(order.PaymentMethodSystemName)).ToString()));

            //event notification
            await _eventPublisher.EntityTokensAddedAsync(recurringPayment, tokens);
        }

        /// <summary>
        /// Add return request tokens
        /// </summary>
        /// <param name="tokens">List of already added tokens</param>
        /// <param name="returnRequest">Return request</param>
        /// <param name="orderItem">Order item</param>
        /// <param name="languageId">Language identifier</param>
        /// <returns>Задача представляет асинхронную операцию</returns>
        public virtual async Task AddReturnRequestTokensAsync(IList<Token> tokens, ReturnRequest returnRequest, OrderItem orderItem, int languageId)
        {
            var tvChannel = await _tvChannelService.GetTvChannelByIdAsync(orderItem.TvChannelId);

            tokens.Add(new Token("ReturnRequest.CustomNumber", returnRequest.CustomNumber));
            tokens.Add(new Token("ReturnRequest.OrderId", orderItem.OrderId));
            tokens.Add(new Token("ReturnRequest.TvChannel.Quantity", returnRequest.Quantity));
            tokens.Add(new Token("ReturnRequest.TvChannel.Name", await _localizationService.GetLocalizedAsync(tvChannel, x => x.Name, languageId)));
            tokens.Add(new Token("ReturnRequest.Reason", returnRequest.ReasonForReturn));
            tokens.Add(new Token("ReturnRequest.RequestedAction", returnRequest.RequestedAction));
            tokens.Add(new Token("ReturnRequest.UserComment", _htmlFormatter.FormatText(returnRequest.UserComments, false, true, false, false, false, false), true));
            tokens.Add(new Token("ReturnRequest.StaffNotes", _htmlFormatter.FormatText(returnRequest.StaffNotes, false, true, false, false, false, false), true));
            tokens.Add(new Token("ReturnRequest.Status", await _localizationService.GetLocalizedEnumAsync(returnRequest.ReturnRequestStatus, languageId)));

            //event notification
            await _eventPublisher.EntityTokensAddedAsync(returnRequest, tokens);
        }

        /// <summary>
        /// Add gift card tokens
        /// </summary>
        /// <param name="tokens">List of already added tokens</param>
        /// <param name="giftCard">Gift card</param>
        /// <param name="languageId">Language identifier</param>
        /// <returns>Задача представляет асинхронную операцию</returns>
        public virtual async Task AddGiftCardTokensAsync(IList<Token> tokens, GiftCard giftCard, int languageId)
        {
            tokens.Add(new Token("GiftCard.SenderName", giftCard.SenderName));
            tokens.Add(new Token("GiftCard.SenderEmail", giftCard.SenderEmail));
            tokens.Add(new Token("GiftCard.RecipientName", giftCard.RecipientName));
            tokens.Add(new Token("GiftCard.RecipientEmail", giftCard.RecipientEmail));

            var primaryStoreCurrency = await _currencyService.GetCurrencyByIdAsync(_currencySettings.PrimaryStoreCurrencyId);
            tokens.Add(new Token("GiftCard.Amount", await _priceFormatter.FormatPriceAsync(giftCard.Amount, true, primaryStoreCurrency.CurrencyCode, false, languageId)));
            tokens.Add(new Token("GiftCard.CouponCode", giftCard.GiftCardCouponCode));

            var giftCardMessage = !string.IsNullOrWhiteSpace(giftCard.Message) ?
                _htmlFormatter.FormatText(giftCard.Message, false, true, false, false, false, false) : string.Empty;

            tokens.Add(new Token("GiftCard.Message", giftCardMessage, true));

            //event notification
            await _eventPublisher.EntityTokensAddedAsync(giftCard, tokens);
        }

        /// <summary>
        /// Add user tokens
        /// </summary>
        /// <param name="tokens">List of already added tokens</param>
        /// <param name="userId">User identifier</param>
        /// <returns>Задача представляет асинхронную операцию</returns>
        public virtual async Task AddUserTokensAsync(IList<Token> tokens, int userId)
        {
            if (userId <= 0)
                throw new ArgumentOutOfRangeException(nameof(userId));

            var user = await _userService.GetUserByIdAsync(userId);

            await AddUserTokensAsync(tokens, user);
        }

        /// <summary>
        /// Add user tokens
        /// </summary>
        /// <param name="tokens">List of already added tokens</param>
        /// <param name="user">User</param>
        /// <returns>Задача представляет асинхронную операцию</returns>
        public virtual async Task AddUserTokensAsync(IList<Token> tokens, User user)
        {
            tokens.Add(new Token("User.Email", user.Email));
            tokens.Add(new Token("User.Username", user.Username));
            tokens.Add(new Token("User.FullName", await _userService.GetUserFullNameAsync(user)));
            tokens.Add(new Token("User.FirstName", user.FirstName));
            tokens.Add(new Token("User.LastName", user.LastName));
            tokens.Add(new Token("User.MiddleName", user.MiddleName));
            tokens.Add(new Token("User.VatNumber", user.VatNumber));
            tokens.Add(new Token("User.VatNumberStatus", ((VatNumberStatus)user.VatNumberStatusId).ToString()));

            var customAttributesXml = user.CustomUserAttributesXML;
            tokens.Add(new Token("User.CustomAttributes", await _userAttributeFormatter.FormatAttributesAsync(customAttributesXml), true));

            //note: we do not use SEO friendly URLS for these links because we can get errors caused by having .(dot) in the URL (from the email address)
            var passwordRecoveryUrl  = await RouteUrlAsync(routeName: "PasswordRecoveryConfirm", routeValues: new { token = await _genericAttributeService.GetAttributeAsync<string>(user, TvProgUserDefaults.PasswordRecoveryTokenAttribute), guid = user.UserGuid });
            var accountActivationUrl  = await RouteUrlAsync(routeName: "AccountActivation", routeValues: new { token = await _genericAttributeService.GetAttributeAsync<string>(user, TvProgUserDefaults.AccountActivationTokenAttribute), guid = user.UserGuid });
            var emailRevalidationUrl  = await RouteUrlAsync(routeName: "EmailRevalidation", routeValues: new { token = await _genericAttributeService.GetAttributeAsync<string>(user, TvProgUserDefaults.EmailRevalidationTokenAttribute), guid = user.UserGuid });
            var wishlistUrl  = await RouteUrlAsync(routeName: "Wishlist", routeValues: new { userGuid = user.UserGuid });
            tokens.Add(new Token("User.PasswordRecoveryURL", passwordRecoveryUrl, true));
            tokens.Add(new Token("User.AccountActivationURL", accountActivationUrl, true));
            tokens.Add(new Token("User.EmailRevalidationURL", emailRevalidationUrl, true));
            tokens.Add(new Token("Wishlist.URLForUser", wishlistUrl, true));

            //event notification
            await _eventPublisher.EntityTokensAddedAsync(user, tokens);
        }

        /// <summary>
        /// Add vendor tokens
        /// </summary>
        /// <param name="tokens">List of already added tokens</param>
        /// <param name="vendor">Vendor</param>
        /// <returns>Задача представляет асинхронную операцию</returns>
        public virtual async Task AddVendorTokensAsync(IList<Token> tokens, Vendor vendor)
        {
            tokens.Add(new Token("Vendor.Name", vendor.Name));
            tokens.Add(new Token("Vendor.Email", vendor.Email));

            var vendorAttributesXml = await _genericAttributeService.GetAttributeAsync<string>(vendor, TvProgVendorDefaults.VendorAttributes);
            tokens.Add(new Token("Vendor.VendorAttributes", await _vendorAttributeFormatter.FormatAttributesAsync(vendorAttributesXml), true));

            //event notification
            await _eventPublisher.EntityTokensAddedAsync(vendor, tokens);
        }

        /// <summary>
        /// Add newsletter subscription tokens
        /// </summary>
        /// <param name="tokens">List of already added tokens</param>
        /// <param name="subscription">Newsletter subscription</param>
        /// <returns>Задача представляет асинхронную операцию</returns>
        public virtual async Task AddNewsLetterSubscriptionTokensAsync(IList<Token> tokens, NewsLetterSubscription subscription)
        {
            tokens.Add(new Token("NewsLetterSubscription.Email", subscription.Email));

            var activationUrl = await RouteUrlAsync(routeName: "NewsletterActivation", routeValues: new { token = subscription.NewsLetterSubscriptionGuid, active = "true" });
            tokens.Add(new Token("NewsLetterSubscription.ActivationUrl", activationUrl, true));

            var deactivationUrl = await RouteUrlAsync(routeName: "NewsletterActivation", routeValues: new { token = subscription.NewsLetterSubscriptionGuid, active = "false" });
            tokens.Add(new Token("NewsLetterSubscription.DeactivationUrl", deactivationUrl, true));

            //event notification
            await _eventPublisher.EntityTokensAddedAsync(subscription, tokens);
        }

        /// <summary>
        /// Add tvChannel review tokens
        /// </summary>
        /// <param name="tokens">List of already added tokens</param>
        /// <param name="tvChannelReview">TvChannel review</param>
        /// <returns>Задача представляет асинхронную операцию</returns>
        public virtual async Task AddTvChannelReviewTokensAsync(IList<Token> tokens, TvChannelReview tvChannelReview)
        {
            tokens.Add(new Token("TvChannelReview.TvChannelName", (await _tvChannelService.GetTvChannelByIdAsync(tvChannelReview.TvChannelId))?.Name));
            tokens.Add(new Token("TvChannelReview.Title", tvChannelReview.Title));
            tokens.Add(new Token("TvChannelReview.IsApproved", tvChannelReview.IsApproved));
            tokens.Add(new Token("TvChannelReview.ReviewText", tvChannelReview.ReviewText));
            tokens.Add(new Token("TvChannelReview.ReplyText", tvChannelReview.ReplyText));

            //event notification
            await _eventPublisher.EntityTokensAddedAsync(tvChannelReview, tokens);
        }

        /// <summary>
        /// Add blog comment tokens
        /// </summary>
        /// <param name="tokens">List of already added tokens</param>
        /// <param name="blogComment">Blog post comment</param>
        /// <returns>Задача представляет асинхронную операцию</returns>
        public virtual async Task AddBlogCommentTokensAsync(IList<Token> tokens, BlogComment blogComment)
        {
            var blogPost = await _blogService.GetBlogPostByIdAsync(blogComment.BlogPostId);

            tokens.Add(new Token("BlogComment.BlogPostTitle", blogPost.Title));

            //event notification
            await _eventPublisher.EntityTokensAddedAsync(blogComment, tokens);
        }

        /// <summary>
        /// Add news comment tokens
        /// </summary>
        /// <param name="tokens">List of already added tokens</param>
        /// <param name="newsComment">News comment</param>
        /// <returns>Задача представляет асинхронную операцию</returns>
        public virtual async Task AddNewsCommentTokensAsync(IList<Token> tokens, NewsComment newsComment)
        {
            var newsItem = await _newsService.GetNewsByIdAsync(newsComment.NewsItemId);

            tokens.Add(new Token("NewsComment.NewsTitle", newsItem.Title));

            //event notification
            await _eventPublisher.EntityTokensAddedAsync(newsComment, tokens);
        }

        /// <summary>
        /// Add tvChannel tokens
        /// </summary>
        /// <param name="tokens">List of already added tokens</param>
        /// <param name="tvChannel">TvChannel</param>
        /// <param name="languageId">Language identifier</param>
        /// <returns>Задача представляет асинхронную операцию</returns>
        public virtual async Task AddTvChannelTokensAsync(IList<Token> tokens, TvChannel tvChannel, int languageId)
        {
            tokens.Add(new Token("TvChannel.ID", tvChannel.Id));
            tokens.Add(new Token("TvChannel.Name", await _localizationService.GetLocalizedAsync(tvChannel, x => x.Name, languageId)));
            tokens.Add(new Token("TvChannel.ShortDescription", await _localizationService.GetLocalizedAsync(tvChannel, x => x.ShortDescription, languageId), true));
            tokens.Add(new Token("TvChannel.SKU", tvChannel.Sku));
            tokens.Add(new Token("TvChannel.StockQuantity", await _tvChannelService.GetTotalStockQuantityAsync(tvChannel)));

            var tvChannelUrl = await RouteUrlAsync(routeName: "TvChannel", routeValues: new { SeName = await _urlRecordService.GetSeNameAsync(tvChannel) });
            tokens.Add(new Token("TvChannel.TvChannelURLForUser", tvChannelUrl, true));

            //event notification
            await _eventPublisher.EntityTokensAddedAsync(tvChannel, tokens);
        }

        /// <summary>
        /// Add tvChannel attribute combination tokens
        /// </summary>
        /// <param name="tokens">List of already added tokens</param>
        /// <param name="combination">TvChannel attribute combination</param>
        /// <param name="languageId">Language identifier</param>
        /// <returns>Задача представляет асинхронную операцию</returns>
        public virtual async Task AddAttributeCombinationTokensAsync(IList<Token> tokens, TvChannelAttributeCombination combination, int languageId)
        {
            //attributes
            //we cannot inject ITvChannelAttributeFormatter into constructor because it'll cause circular references.
            //that's why we resolve it here this way
            var tvChannelAttributeFormatter = EngineContext.Current.Resolve<ITvChannelAttributeFormatter>();

            var tvChannel = await _tvChannelService.GetTvChannelByIdAsync(combination.TvChannelId);
            var currentUser = await _workContext.GetCurrentUserAsync();
            var currentStore = await _storeContext.GetCurrentStoreAsync();
            
            var attributes = await tvChannelAttributeFormatter.FormatAttributesAsync(tvChannel,
                combination.AttributesXml,
                currentUser,
                currentStore,
                renderPrices: false);

            tokens.Add(new Token("AttributeCombination.Formatted", attributes, true));
            tokens.Add(new Token("AttributeCombination.SKU", await _tvChannelService.FormatSkuAsync(await _tvChannelService.GetTvChannelByIdAsync(combination.TvChannelId), combination.AttributesXml)));
            tokens.Add(new Token("AttributeCombination.StockQuantity", combination.StockQuantity));

            //event notification
            await _eventPublisher.EntityTokensAddedAsync(combination, tokens);
        }

        /// <summary>
        /// Add forum topic tokens
        /// </summary>
        /// <param name="tokens">List of already added tokens</param>
        /// <param name="forumTopic">Forum topic</param>
        /// <param name="friendlyForumTopicPageIndex">Friendly (starts with 1) forum topic page to use for URL generation</param>
        /// <param name="appendedPostIdentifierAnchor">Forum post identifier</param>
        /// <returns>Задача представляет асинхронную операцию</returns>
        public virtual async Task AddForumTopicTokensAsync(IList<Token> tokens, ForumTopic forumTopic,
            int? friendlyForumTopicPageIndex = null, int? appendedPostIdentifierAnchor = null)
        {
            //attributes
            //we cannot inject IForumService into constructor because it'll cause circular references.
            //that's why we resolve it here this way
            var forumService = EngineContext.Current.Resolve<IForumService>();

            string topicUrl;
            if (friendlyForumTopicPageIndex.HasValue && friendlyForumTopicPageIndex.Value > 1)
                topicUrl = await RouteUrlAsync(routeName: "TopicSlugPaged", routeValues: new { id = forumTopic.Id, slug = await forumService.GetTopicSeNameAsync(forumTopic), pageNumber = friendlyForumTopicPageIndex.Value });
            else
                topicUrl = await RouteUrlAsync(routeName: "TopicSlug", routeValues: new { id = forumTopic.Id, slug = await forumService.GetTopicSeNameAsync(forumTopic) });
            if (appendedPostIdentifierAnchor.HasValue && appendedPostIdentifierAnchor.Value > 0)
                topicUrl = $"{topicUrl}#{appendedPostIdentifierAnchor.Value}";
            tokens.Add(new Token("Forums.TopicURL", topicUrl, true));
            tokens.Add(new Token("Forums.TopicName", forumTopic.Subject));

            //event notification
            await _eventPublisher.EntityTokensAddedAsync(forumTopic, tokens);
        }

        /// <summary>
        /// Add forum post tokens
        /// </summary>
        /// <param name="tokens">List of already added tokens</param>
        /// <param name="forumPost">Forum post</param>
        /// <returns>Задача представляет асинхронную операцию</returns>
        public virtual async Task AddForumPostTokensAsync(IList<Token> tokens, ForumPost forumPost)
        {
            //attributes
            //we cannot inject IForumService into constructor because it'll cause circular references.
            //that's why we resolve it here this way
            var forumService = EngineContext.Current.Resolve<IForumService>();

            var user = await _userService.GetUserByIdAsync(forumPost.UserId);

            tokens.Add(new Token("Forums.PostAuthor", await _userService.FormatUsernameAsync(user)));
            tokens.Add(new Token("Forums.PostBody", forumService.FormatPostText(forumPost), true));

            //event notification
            await _eventPublisher.EntityTokensAddedAsync(forumPost, tokens);
        }

        /// <summary>
        /// Add forum tokens
        /// </summary>
        /// <param name="tokens">List of already added tokens</param>
        /// <param name="forum">Forum</param>
        /// <returns>Задача представляет асинхронную операцию</returns>
        public virtual async Task AddForumTokensAsync(IList<Token> tokens, Forum forum)
        {
            //attributes
            //we cannot inject IForumService into constructor because it'll cause circular references.
            //that's why we resolve it here this way
            var forumService = EngineContext.Current.Resolve<IForumService>();

            var forumUrl = await RouteUrlAsync(routeName: "ForumSlug", routeValues: new { id = forum.Id, slug = await forumService.GetForumSeNameAsync(forum) });
            tokens.Add(new Token("Forums.ForumURL", forumUrl, true));
            tokens.Add(new Token("Forums.ForumName", forum.Name));

            //event notification
            await _eventPublisher.EntityTokensAddedAsync(forum, tokens);
        }

        /// <summary>
        /// Add private message tokens
        /// </summary>
        /// <param name="tokens">List of already added tokens</param>
        /// <param name="privateMessage">Private message</param>
        /// <returns>Задача представляет асинхронную операцию</returns>
        public virtual async Task AddPrivateMessageTokensAsync(IList<Token> tokens, PrivateMessage privateMessage)
        {
            //attributes
            //we cannot inject IForumService into constructor because it'll cause circular references.
            //that's why we resolve it here this way
            var forumService = EngineContext.Current.Resolve<IForumService>();

            tokens.Add(new Token("PrivateMessage.Subject", privateMessage.Subject));
            tokens.Add(new Token("PrivateMessage.Text", forumService.FormatPrivateMessageText(privateMessage), true));

            //event notification
            await _eventPublisher.EntityTokensAddedAsync(privateMessage, tokens);
        }

        /// <summary>
        /// Add tokens of BackInStock subscription
        /// </summary>
        /// <param name="tokens">List of already added tokens</param>
        /// <param name="subscription">BackInStock subscription</param>
        /// <returns>Задача представляет асинхронную операцию</returns>
        public virtual async Task AddBackInStockTokensAsync(IList<Token> tokens, BackInStockSubscription subscription)
        {
            var tvChannel = await _tvChannelService.GetTvChannelByIdAsync(subscription.TvChannelId);

            tokens.Add(new Token("BackInStockSubscription.TvChannelName", tvChannel.Name));
            var tvChannelUrl = await RouteUrlAsync(subscription.StoreId, "TvChannel", new { SeName = await _urlRecordService.GetSeNameAsync(tvChannel) });
            tokens.Add(new Token("BackInStockSubscription.TvChannelUrl", tvChannelUrl, true));

            //event notification
            await _eventPublisher.EntityTokensAddedAsync(subscription, tokens);
        }

        /// <summary>
        /// Get collection of allowed (supported) message tokens for campaigns
        /// </summary>
        /// <returns>
        /// Задача представляет асинхронную операцию
        /// The task result contains the collection of allowed (supported) message tokens for campaigns
        /// </returns>
        public virtual async Task<IEnumerable<string>> GetListOfCampaignAllowedTokensAsync()
        {
            var additionalTokens = new CampaignAdditionalTokensAddedEvent();
            await _eventPublisher.PublishAsync(additionalTokens);

            var allowedTokens = (await GetListOfAllowedTokensAsync(new[] { TokenGroupNames.StoreTokens, TokenGroupNames.SubscriptionTokens })).ToList();
            allowedTokens.AddRange(additionalTokens.AdditionalTokens);

            return allowedTokens.Distinct();
        }

        /// <summary>
        /// Get collection of allowed (supported) message tokens
        /// </summary>
        /// <param name="tokenGroups">Collection of token groups; pass null to get all available tokens</param>
        /// <returns>
        /// Задача представляет асинхронную операцию
        /// The task result contains the collection of allowed message tokens
        /// </returns>
        public virtual async Task<IEnumerable<string>> GetListOfAllowedTokensAsync(IEnumerable<string> tokenGroups = null)
        {
            var additionalTokens = new AdditionalTokensAddedEvent
            {
                TokenGroups = tokenGroups
            };
            await _eventPublisher.PublishAsync(additionalTokens);

            var allowedTokens = AllowedTokens.Where(x => tokenGroups == null || tokenGroups.Contains(x.Key))
                .SelectMany(x => x.Value).ToList();

            allowedTokens.AddRange(additionalTokens.AdditionalTokens);

            return allowedTokens.Distinct();
        }

        /// <summary>
        /// Get token groups of message template
        /// </summary>
        /// <param name="messageTemplate">Message template</param>
        /// <returns>Collection of token group names</returns>
        public virtual IEnumerable<string> GetTokenGroups(MessageTemplate messageTemplate)
        {
            //groups depend on which tokens are added at the appropriate methods in IWorkflowMessageService
            return messageTemplate.Name switch
            {
                MessageTemplateSystemNames.UserRegisteredStoreOwnerNotification or 
                MessageTemplateSystemNames.UserWelcomeMessage or 
                MessageTemplateSystemNames.UserEmailValidationMessage or 
                MessageTemplateSystemNames.UserEmailRevalidationMessage or 
                MessageTemplateSystemNames.UserPasswordRecoveryMessage => new[] { TokenGroupNames.StoreTokens, TokenGroupNames.UserTokens },

                MessageTemplateSystemNames.OrderPlacedVendorNotification or 
                MessageTemplateSystemNames.OrderPlacedStoreOwnerNotification or 
                MessageTemplateSystemNames.OrderPlacedAffiliateNotification or 
                MessageTemplateSystemNames.OrderPaidStoreOwnerNotification or 
                MessageTemplateSystemNames.OrderPaidUserNotification or 
                MessageTemplateSystemNames.OrderPaidVendorNotification or 
                MessageTemplateSystemNames.OrderPaidAffiliateNotification or 
                MessageTemplateSystemNames.OrderPlacedUserNotification or
                MessageTemplateSystemNames.OrderProcessingUserNotification or
                MessageTemplateSystemNames.OrderCompletedUserNotification or 
                MessageTemplateSystemNames.OrderCancelledUserNotification => new[] { TokenGroupNames.StoreTokens, TokenGroupNames.OrderTokens, TokenGroupNames.UserTokens },

                MessageTemplateSystemNames.ShipmentSentUserNotification or 
                MessageTemplateSystemNames.ShipmentReadyForPickupUserNotification or 
                MessageTemplateSystemNames.ShipmentDeliveredUserNotification => new[] { TokenGroupNames.StoreTokens, TokenGroupNames.ShipmentTokens, TokenGroupNames.OrderTokens, TokenGroupNames.UserTokens },

                MessageTemplateSystemNames.OrderRefundedStoreOwnerNotification or 
                MessageTemplateSystemNames.OrderRefundedUserNotification => new[] { TokenGroupNames.StoreTokens, TokenGroupNames.OrderTokens, TokenGroupNames.RefundedOrderTokens, TokenGroupNames.UserTokens },

                MessageTemplateSystemNames.NewOrderNoteAddedUserNotification => new[] { TokenGroupNames.StoreTokens, TokenGroupNames.OrderNoteTokens, TokenGroupNames.OrderTokens, TokenGroupNames.UserTokens },

                MessageTemplateSystemNames.RecurringPaymentCancelledStoreOwnerNotification or 
                MessageTemplateSystemNames.RecurringPaymentCancelledUserNotification or 
                MessageTemplateSystemNames.RecurringPaymentFailedUserNotification => new[] { TokenGroupNames.StoreTokens, TokenGroupNames.OrderTokens, TokenGroupNames.UserTokens, TokenGroupNames.RecurringPaymentTokens },

                MessageTemplateSystemNames.NewsletterSubscriptionActivationMessage or 
                MessageTemplateSystemNames.NewsletterSubscriptionDeactivationMessage => new[] { TokenGroupNames.StoreTokens, TokenGroupNames.SubscriptionTokens },

                MessageTemplateSystemNames.EmailAFriendMessage => new[] { TokenGroupNames.StoreTokens, TokenGroupNames.UserTokens, TokenGroupNames.TvChannelTokens, TokenGroupNames.EmailAFriendTokens },
                MessageTemplateSystemNames.WishlistToFriendMessage => new[] { TokenGroupNames.StoreTokens, TokenGroupNames.UserTokens, TokenGroupNames.WishlistToFriendTokens },

                MessageTemplateSystemNames.NewReturnRequestStoreOwnerNotification or 
                MessageTemplateSystemNames.NewReturnRequestUserNotification or 
                MessageTemplateSystemNames.ReturnRequestStatusChangedUserNotification => new[] { TokenGroupNames.StoreTokens, TokenGroupNames.OrderTokens, TokenGroupNames.UserTokens, TokenGroupNames.ReturnRequestTokens },

                MessageTemplateSystemNames.NewForumTopicMessage => new[] { TokenGroupNames.StoreTokens, TokenGroupNames.ForumTopicTokens, TokenGroupNames.ForumTokens, TokenGroupNames.UserTokens },
                MessageTemplateSystemNames.NewForumPostMessage => new[] { TokenGroupNames.StoreTokens, TokenGroupNames.ForumPostTokens, TokenGroupNames.ForumTopicTokens, TokenGroupNames.ForumTokens, TokenGroupNames.UserTokens },
                MessageTemplateSystemNames.PrivateMessageNotification => new[] { TokenGroupNames.StoreTokens, TokenGroupNames.PrivateMessageTokens, TokenGroupNames.UserTokens },
                MessageTemplateSystemNames.NewVendorAccountApplyStoreOwnerNotification => new[] { TokenGroupNames.StoreTokens, TokenGroupNames.UserTokens, TokenGroupNames.VendorTokens },
                MessageTemplateSystemNames.VendorInformationChangeStoreOwnerNotification => new[] { TokenGroupNames.StoreTokens, TokenGroupNames.VendorTokens },
                MessageTemplateSystemNames.GiftCardNotification => new[] { TokenGroupNames.StoreTokens, TokenGroupNames.GiftCardTokens },

                MessageTemplateSystemNames.TvChannelReviewStoreOwnerNotification or 
                MessageTemplateSystemNames.TvChannelReviewReplyUserNotification => new[] { TokenGroupNames.StoreTokens, TokenGroupNames.TvChannelReviewTokens, TokenGroupNames.UserTokens },

                MessageTemplateSystemNames.QuantityBelowStoreOwnerNotification => new[] { TokenGroupNames.StoreTokens, TokenGroupNames.TvChannelTokens },
                MessageTemplateSystemNames.QuantityBelowAttributeCombinationStoreOwnerNotification => new[] { TokenGroupNames.StoreTokens, TokenGroupNames.TvChannelTokens, TokenGroupNames.AttributeCombinationTokens },
                MessageTemplateSystemNames.NewVatSubmittedStoreOwnerNotification => new[] { TokenGroupNames.StoreTokens, TokenGroupNames.UserTokens, TokenGroupNames.VatValidation },
                MessageTemplateSystemNames.BlogCommentStoreOwnerNotification => new[] { TokenGroupNames.StoreTokens, TokenGroupNames.BlogCommentTokens, TokenGroupNames.UserTokens },
                MessageTemplateSystemNames.NewsCommentStoreOwnerNotification => new[] { TokenGroupNames.StoreTokens, TokenGroupNames.NewsCommentTokens, TokenGroupNames.UserTokens },
                MessageTemplateSystemNames.BackInStockNotification => new[] { TokenGroupNames.StoreTokens, TokenGroupNames.UserTokens, TokenGroupNames.TvChannelBackInStockTokens },
                MessageTemplateSystemNames.ContactUsMessage => new[] { TokenGroupNames.StoreTokens, TokenGroupNames.ContactUs },
                MessageTemplateSystemNames.ContactVendorMessage => new[] { TokenGroupNames.StoreTokens, TokenGroupNames.ContactVendor },
                _ => Array.Empty<string>(),
            };
        }

        #endregion
    }
}