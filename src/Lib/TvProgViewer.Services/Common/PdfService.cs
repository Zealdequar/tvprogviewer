﻿using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Threading.Tasks;
using TvProgViewer.Core;
using TvProgViewer.Core.Domain.Catalog;
using TvProgViewer.Core.Domain.Common;
using TvProgViewer.Core.Domain.Users;
using TvProgViewer.Core.Domain.Directory;
using TvProgViewer.Core.Domain.Localization;
using TvProgViewer.Core.Domain.Orders;
using TvProgViewer.Core.Domain.Shipping;
using TvProgViewer.Core.Domain.Stores;
using TvProgViewer.Core.Domain.Tax;
using TvProgViewer.Core.Domain.Vendors;
using TvProgViewer.Core.Infrastructure;
using TvProgViewer.Services.Catalog;
using TvProgViewer.Services.Common.Pdf;
using TvProgViewer.Services.Configuration;
using TvProgViewer.Services.Directory;
using TvProgViewer.Services.Helpers;
using TvProgViewer.Services.Html;
using TvProgViewer.Services.Localization;
using TvProgViewer.Services.Media;
using TvProgViewer.Services.Orders;
using TvProgViewer.Services.Payments;
using TvProgViewer.Services.Shipping;
using TvProgViewer.Services.Stores;
using TvProgViewer.Services.Vendors;
using QuestPDF.Fluent;
using QuestPDF.Helpers;

namespace TvProgViewer.Services.Common
{
    /// <summary>
    /// PDF service
    /// </summary>
    public partial class PdfService : IPdfService
    {
        #region Fields

        private readonly AddressSettings _addressSettings;
        private readonly CatalogSettings _catalogSettings;
        private readonly CurrencySettings _currencySettings;
        private readonly IAddressAttributeFormatter _addressAttributeFormatter;
        private readonly IAddressService _addressService;
        private readonly ICountryService _countryService;
        private readonly ICurrencyService _currencyService;
        private readonly IDateTimeHelper _dateTimeHelper;
        private readonly IGiftCardService _giftCardService;
        private readonly IHtmlFormatter _htmlFormatter;
        private readonly ILanguageService _languageService;
        private readonly ILocalizationService _localizationService;
        private readonly IMeasureService _measureService;
        private readonly ITvProgFileProvider _fileProvider;
        private readonly IOrderService _orderService;
        private readonly IPaymentPluginManager _paymentPluginManager;
        private readonly IPaymentService _paymentService;
        private readonly IPictureService _pictureService;
        private readonly IPriceFormatter _priceFormatter;
        private readonly ITvChannelService _tvChannelService;
        private readonly IRewardPointService _rewardPointService;
        private readonly ISettingService _settingService;
        private readonly IShipmentService _shipmentService;
        private readonly IStateProvinceService _stateProvinceService;
        private readonly IStoreContext _storeContext;
        private readonly IStoreService _storeService;
        private readonly IVendorService _vendorService;
        private readonly IWorkContext _workContext;
        private readonly MeasureSettings _measureSettings;
        private readonly TaxSettings _taxSettings;
        private readonly VendorSettings _vendorSettings;

        #endregion

        #region Ctor

        public PdfService(AddressSettings addressSettings,
            CatalogSettings catalogSettings,
            CurrencySettings currencySettings,
            IAddressAttributeFormatter addressAttributeFormatter,
            IAddressService addressService,
            ICountryService countryService,
            ICurrencyService currencyService,
            IDateTimeHelper dateTimeHelper,
            IGiftCardService giftCardService,
            IHtmlFormatter htmlFormatter,
            ILanguageService languageService,
            ILocalizationService localizationService,
            IMeasureService measureService,
            ITvProgFileProvider fileProvider,
            IOrderService orderService,
            IPaymentPluginManager paymentPluginManager,
            IPaymentService paymentService,
            IPictureService pictureService,
            IPriceFormatter priceFormatter,
            ITvChannelService tvChannelService,
            IRewardPointService rewardPointService,
            ISettingService settingService,
            IShipmentService shipmentService,
            IStateProvinceService stateProvinceService,
            IStoreContext storeContext,
            IStoreService storeService,
            IVendorService vendorService,
            IWorkContext workContext,
            MeasureSettings measureSettings,
            TaxSettings taxSettings,
            VendorSettings vendorSettings)
        {
            _addressSettings = addressSettings;
            _addressService = addressService;
            _catalogSettings = catalogSettings;
            _countryService = countryService;
            _currencySettings = currencySettings;
            _addressAttributeFormatter = addressAttributeFormatter;
            _currencyService = currencyService;
            _dateTimeHelper = dateTimeHelper;
            _giftCardService = giftCardService;
            _htmlFormatter = htmlFormatter;
            _languageService = languageService;
            _localizationService = localizationService;
            _measureService = measureService;
            _fileProvider = fileProvider;
            _orderService = orderService;
            _paymentPluginManager = paymentPluginManager;
            _paymentService = paymentService;
            _pictureService = pictureService;
            _priceFormatter = priceFormatter;
            _tvChannelService = tvChannelService;
            _rewardPointService = rewardPointService;
            _settingService = settingService;
            _shipmentService = shipmentService;
            _storeContext = storeContext;
            _stateProvinceService = stateProvinceService;
            _storeService = storeService;
            _vendorService = vendorService;
            _workContext = workContext;
            _measureSettings = measureSettings;
            _taxSettings = taxSettings;
            _vendorSettings = vendorSettings;
        }

        #endregion

        #region Utils

        /// <summary>
        /// Get billing address
        /// </summary>
        /// <param name="vendor">Vendor</param>
        /// <param name="lang">Language</param>
        /// <param name="order">Order</param>
        /// <returns>A task that contains address item</returns>
        protected virtual async Task<AddressItem> GetBillingAddressAsync(Vendor vendor, Language lang, Order order)
        {
            var addressResult = new AddressItem();

            var billingAddress = await _addressService.GetAddressByIdAsync(order.BillingAddressId);

            if (_addressSettings.CompanyEnabled && !string.IsNullOrEmpty(billingAddress.Company))
                addressResult.Company = billingAddress.Company;

            addressResult.Name = $"{billingAddress.LastName} {billingAddress.FirstName} {billingAddress.MiddleName}";

            if (_addressSettings.SmartPhoneEnabled)
                addressResult.SmartPhone = billingAddress.PhoneNumber;

            if (_addressSettings.FaxEnabled && !string.IsNullOrEmpty(billingAddress.FaxNumber))
                addressResult.Fax = billingAddress.FaxNumber;

            if (_addressSettings.StreetAddressEnabled)
                addressResult.Address = billingAddress.Address1;

            if (_addressSettings.StreetAddress2Enabled && !string.IsNullOrEmpty(billingAddress.Address2))
                addressResult.Address2 = billingAddress.Address2;

            if (_addressSettings.CityEnabled || _addressSettings.StateProvinceEnabled ||
                _addressSettings.CountyEnabled || _addressSettings.ZipPostalCodeEnabled)
            {
                addressResult.AddressLine =
                    $"{billingAddress.City}, " +
                    $"{(!string.IsNullOrEmpty(billingAddress.County) ? $"{billingAddress.County}, " : string.Empty)}" +
                    $"{(await _stateProvinceService.GetStateProvinceByAddressAsync(billingAddress) is StateProvince stateProvince ? await _localizationService.GetLocalizedAsync(stateProvince, x => x.Name, lang.Id) : string.Empty)} " +
                    $"{billingAddress.ZipPostalCode}";
            }

            if (_addressSettings.CountryEnabled && await _countryService.GetCountryByAddressAsync(billingAddress) is Country country)
                addressResult.Country = await _localizationService.GetLocalizedAsync(country, x => x.Name, lang.Id);

            //VAT number
            if (!string.IsNullOrEmpty(order.VatNumber))
                addressResult.VATNumber = order.VatNumber;

            //custom attributes
            var customBillingAddressAttributes = await _addressAttributeFormatter
                .FormatAttributesAsync(billingAddress.CustomAttributes, "<br />");

            if (!string.IsNullOrEmpty(customBillingAddressAttributes))
            {
                var text = _htmlFormatter.ConvertHtmlToPlainText(customBillingAddressAttributes, true, true);
                addressResult.AddressAttributes = text.Split('\n').ToList();
            }

            //vendors payment details
            if (vendor is null)
            {
                //payment method
                var paymentMethod = await _paymentPluginManager.LoadPluginBySystemNameAsync(order.PaymentMethodSystemName);
                var paymentMethodStr = paymentMethod != null
                    ? await _localizationService.GetLocalizedFriendlyNameAsync(paymentMethod, lang.Id)
                    : order.PaymentMethodSystemName;
                if (!string.IsNullOrEmpty(paymentMethodStr))
                {
                    addressResult.PaymentMethod = paymentMethodStr;
                }

                //custom values
                var customValues = _paymentService.DeserializeCustomValues(order);
                if (customValues != null)
                    addressResult.CustomValues = customValues;
            }

            return addressResult;
        }

        /// <summary>
        /// Get shipping address
        /// </summary>
        /// <param name="lang">Language</param>
        /// <param name="order">Order</param>
        /// <returns>A task that contains address item</returns>
        protected virtual async Task<AddressItem> GetShippingAddressAsync(Language lang, Order order)
        {
            var addressResult = new AddressItem();

            if (order.ShippingStatus != ShippingStatus.ShippingNotRequired)
            {
                if (!order.PickupInStore)
                {
                    if (order.ShippingAddressId == null || await _addressService.GetAddressByIdAsync(order.ShippingAddressId.Value) is not Address shippingAddress)
                        throw new TvProgException($"Shipping is required, but address is not available. Order ID = {order.Id}");

                    if (!string.IsNullOrEmpty(shippingAddress.Company))
                        addressResult.Company = shippingAddress.Company;

                    addressResult.Name = $"{shippingAddress.LastName} {shippingAddress.FirstName} {shippingAddress.MiddleName}";

                    if (_addressSettings.SmartPhoneEnabled)
                        addressResult.SmartPhone = shippingAddress.PhoneNumber;

                    if (_addressSettings.FaxEnabled && !string.IsNullOrEmpty(shippingAddress.FaxNumber))
                        addressResult.Fax = shippingAddress.FaxNumber;

                    if (_addressSettings.StreetAddressEnabled)
                        addressResult.Address = shippingAddress.Address1;

                    if (_addressSettings.StreetAddress2Enabled && !string.IsNullOrEmpty(shippingAddress.Address2))
                        addressResult.Address2 = shippingAddress.Address2;

                    if (_addressSettings.CityEnabled || _addressSettings.StateProvinceEnabled ||
                        _addressSettings.CountyEnabled || _addressSettings.ZipPostalCodeEnabled)
                    {
                        addressResult.AddressLine = $"{shippingAddress.City}, " +
                            $"{(!string.IsNullOrEmpty(shippingAddress.County) ? $"{shippingAddress.County}, " : string.Empty)}" +
                            $"{(await _stateProvinceService.GetStateProvinceByAddressAsync(shippingAddress) is StateProvince stateProvince ? await _localizationService.GetLocalizedAsync(stateProvince, x => x.Name, lang.Id) : string.Empty)} " +
                            $"{shippingAddress.ZipPostalCode}";
                    }

                    if (_addressSettings.CountryEnabled && await _countryService.GetCountryByAddressAsync(shippingAddress) is Country country)
                    {
                        addressResult.Country = await _localizationService.GetLocalizedAsync(country, x => x.Name, lang.Id);
                    }

                    //custom attributes
                    var customShippingAddressAttributes = await _addressAttributeFormatter
                        .FormatAttributesAsync(shippingAddress.CustomAttributes, "<br />");
                    if (!string.IsNullOrEmpty(customShippingAddressAttributes))
                    {
                        var text = _htmlFormatter.ConvertHtmlToPlainText(customShippingAddressAttributes, true, true);
                        addressResult.AddressAttributes = text.Split('\n').ToList();
                    }
                }
                else if (order.PickupAddressId.HasValue && await _addressService.GetAddressByIdAsync(order.PickupAddressId.Value) is Address pickupAddress)
                {
                    if (!string.IsNullOrEmpty(pickupAddress.Address1))
                        addressResult.Address = pickupAddress.Address1;

                    if (_addressSettings.CityEnabled || _addressSettings.StateProvinceEnabled ||
                        _addressSettings.CountyEnabled || _addressSettings.ZipPostalCodeEnabled)
                    {
                        addressResult.AddressLine = $"{pickupAddress.City}, " +
                            $"{(!string.IsNullOrEmpty(pickupAddress.County) ? $"{pickupAddress.County}, " : string.Empty)}" +
                            $"{(await _stateProvinceService.GetStateProvinceByAddressAsync(pickupAddress) is StateProvince stateProvince ? await _localizationService.GetLocalizedAsync(stateProvince, x => x.Name, lang.Id) : string.Empty)} " +
                            $"{pickupAddress.ZipPostalCode}";
                    }

                    if (await _countryService.GetCountryByAddressAsync(pickupAddress) is Country country)
                        addressResult.Country = await _localizationService.GetLocalizedAsync(country, x => x.Name, lang.Id);
                }

                addressResult.ShippingMethod = order.ShippingMethod;
            }

            return addressResult;
        }

        /// <summary>
        /// Get order notes
        /// </summary>
        /// <param name="pdfSettingsByStore">PDF settings</param>
        /// <param name="order">Order</param>
        /// <param name="lang">Language</param>
        /// <returns>A task that contains collection of date/note pairs</returns>
        protected virtual async Task<List<(string, string)>> GetOrderNotesAsync(PdfSettings pdfSettingsByStore, Order order, Language lang)
        {
            var notesResult = new List<(string, string)>();

            if (!pdfSettingsByStore.RenderOrderNotes)
                return notesResult;

            var orderNotes = (await _orderService.GetOrderNotesByOrderIdAsync(order.Id, true))
                .OrderByDescending(on => on.CreatedOnUtc)
                .ToList();

            if (!orderNotes.Any())
                return notesResult;

            foreach (var orderNote in orderNotes)
            {
                var createdOn = (await _dateTimeHelper.ConvertToUserTimeAsync(orderNote.CreatedOnUtc, DateTimeKind.Utc)).ToString();
                var note = _htmlFormatter.ConvertHtmlToPlainText(_orderService.FormatOrderNoteText(orderNote), true, true);

                notesResult.Add((createdOn, note));

                //should we display a link to downloadable files here?
                //I think, no. Anyway, PDFs are printable documents and links (files) are useful here
            }

            return notesResult;
        }

        /// <summary>
        /// Get tvChannel entries for document data source
        /// </summary>
        /// <param name="order">Order</param>
        /// <param name="orderItems">Collection of order items</param>
        /// <param name="language">Language</param>
        /// <returns>A task that contains collection of tvChannel entries</returns>
        protected virtual async Task<List<TvChannelItem>> GetOrderTvChannelItemsAsync(Order order, IList<OrderItem> orderItems, Language language)
        {
            var vendors = _vendorSettings.ShowVendorOnOrderDetailsPage ? await _vendorService.GetVendorsByTvChannelIdsAsync(orderItems.Select(item => item.TvChannelId).ToArray()) : new List<Vendor>();

            var result = new List<TvChannelItem>();

            foreach (var oi in orderItems)
            {
                var tvChannelItem = new TvChannelItem();
                var tvChannel = await _tvChannelService.GetTvChannelByIdAsync(oi.TvChannelId);

                //tvChannel name
                tvChannelItem.Name = await _localizationService.GetLocalizedAsync(tvChannel, x => x.Name, language.Id);

                //attributes
                if (!string.IsNullOrEmpty(oi.AttributeDescription))
                {
                    var attributes = _htmlFormatter.ConvertHtmlToPlainText(oi.AttributeDescription, true, true);
                    tvChannelItem.TvChannelAttributes = attributes.Split('\n').ToList();
                }

                //SKU
                if (_catalogSettings.ShowSkuOnTvChannelDetailsPage)
                    tvChannelItem.Sku = await _tvChannelService.FormatSkuAsync(tvChannel, oi.AttributesXml);

                //Vendor name
                if (_vendorSettings.ShowVendorOnOrderDetailsPage)
                    tvChannelItem.VendorName = vendors.FirstOrDefault(v => v.Id == tvChannel.VendorId)?.Name ?? string.Empty;

                //price
                string unitPrice;
                if (order.UserTaxDisplayType == TaxDisplayType.IncludingTax)
                {
                    //including tax
                    var unitPriceInclTaxInUserCurrency =
                        _currencyService.ConvertCurrency(oi.UnitPriceInclTax, order.CurrencyRate);
                    unitPrice = await _priceFormatter.FormatPriceAsync(unitPriceInclTaxInUserCurrency, true,
                        order.UserCurrencyCode, language.Id, true);
                }
                else
                {
                    //excluding tax
                    var unitPriceExclTaxInUserCurrency =
                        _currencyService.ConvertCurrency(oi.UnitPriceExclTax, order.CurrencyRate);
                    unitPrice = await _priceFormatter.FormatPriceAsync(unitPriceExclTaxInUserCurrency, true,
                        order.UserCurrencyCode, language.Id, false);
                }

                tvChannelItem.Price = unitPrice;

                //qty
                tvChannelItem.Quantity = oi.Quantity.ToString();

                //total
                string subTotal;
                if (order.UserTaxDisplayType == TaxDisplayType.IncludingTax)
                {
                    //including tax
                    var priceInclTaxInUserCurrency =
                        _currencyService.ConvertCurrency(oi.PriceInclTax, order.CurrencyRate);
                    subTotal = await _priceFormatter.FormatPriceAsync(priceInclTaxInUserCurrency, true, order.UserCurrencyCode,
                        language.Id, true);
                }
                else
                {
                    //excluding tax
                    var priceExclTaxInUserCurrency =
                        _currencyService.ConvertCurrency(oi.PriceExclTax, order.CurrencyRate);
                    subTotal = await _priceFormatter.FormatPriceAsync(priceExclTaxInUserCurrency, true, order.UserCurrencyCode,
                        language.Id, false);
                }

                tvChannelItem.Total = subTotal;

                result.Add(tvChannelItem);
            }

            return result;
        }

        /// <summary>
        /// Get invoice totals
        /// </summary>
        /// <param name="lang">Language</param>
        /// <param name="order">Order</param>
        /// <returns>A task that contains invoice totals</returns>
        protected virtual async Task<InvoiceTotals> GetTotalsAsync(Language lang, Order order)
        {
            var result = new InvoiceTotals();
            var languageId = lang.Id;

            //order subtotal
            if (order.UserTaxDisplayType == TaxDisplayType.IncludingTax &&
                !_taxSettings.ForceTaxExclusionFromOrderSubtotal)
            {
                //including tax
                var orderSubtotalInclTaxInUserCurrency =
                    _currencyService.ConvertCurrency(order.OrderSubtotalInclTax, order.CurrencyRate);
                result.SubTotal = await _priceFormatter.FormatPriceAsync(orderSubtotalInclTaxInUserCurrency, true,
                    order.UserCurrencyCode, languageId, true);
            }
            else
            {
                //excluding tax
                var orderSubtotalExclTaxInUserCurrency =
                    _currencyService.ConvertCurrency(order.OrderSubtotalExclTax, order.CurrencyRate);
                result.SubTotal = await _priceFormatter.FormatPriceAsync(orderSubtotalExclTaxInUserCurrency, true,
                    order.UserCurrencyCode, languageId, false);
            }

            //discount (applied to order subtotal)
            if (order.OrderSubTotalDiscountExclTax > decimal.Zero)
            {
                //order subtotal
                if (order.UserTaxDisplayType == TaxDisplayType.IncludingTax &&
                    !_taxSettings.ForceTaxExclusionFromOrderSubtotal)
                {
                    //including tax
                    var orderSubTotalDiscountInclTaxInUserCurrency =
                        _currencyService.ConvertCurrency(order.OrderSubTotalDiscountInclTax, order.CurrencyRate);
                    result.Discount = await _priceFormatter.FormatPriceAsync(
                        -orderSubTotalDiscountInclTaxInUserCurrency, true, order.UserCurrencyCode, languageId, true);
                }
                else
                {
                    //excluding tax
                    var orderSubTotalDiscountExclTaxInUserCurrency =
                        _currencyService.ConvertCurrency(order.OrderSubTotalDiscountExclTax, order.CurrencyRate);
                    result.Discount = await _priceFormatter.FormatPriceAsync(
                        -orderSubTotalDiscountExclTaxInUserCurrency, true, order.UserCurrencyCode, languageId, false);
                }
            }

            //shipping
            if (order.ShippingStatus != ShippingStatus.ShippingNotRequired)
            {
                if (order.UserTaxDisplayType == TaxDisplayType.IncludingTax)
                {
                    //including tax
                    var orderShippingInclTaxInUserCurrency =
                        _currencyService.ConvertCurrency(order.OrderShippingInclTax, order.CurrencyRate);
                    result.Shipping = await _priceFormatter.FormatShippingPriceAsync(
                        orderShippingInclTaxInUserCurrency, true, order.UserCurrencyCode, languageId, true);
                }
                else
                {
                    //excluding tax
                    var orderShippingExclTaxInUserCurrency =
                        _currencyService.ConvertCurrency(order.OrderShippingExclTax, order.CurrencyRate);
                    result.Shipping = await _priceFormatter.FormatShippingPriceAsync(
                        orderShippingExclTaxInUserCurrency, true, order.UserCurrencyCode, languageId, false);
                }
            }

            //payment fee
            if (order.PaymentMethodAdditionalFeeExclTax > decimal.Zero)
            {
                if (order.UserTaxDisplayType == TaxDisplayType.IncludingTax)
                {
                    //including tax
                    var paymentMethodAdditionalFeeInclTaxInUserCurrency =
                        _currencyService.ConvertCurrency(order.PaymentMethodAdditionalFeeInclTax, order.CurrencyRate);
                    result.PaymentMethodAdditionalFee = await _priceFormatter.FormatPaymentMethodAdditionalFeeAsync(
                        paymentMethodAdditionalFeeInclTaxInUserCurrency, true, order.UserCurrencyCode, languageId, true);
                }
                else
                {
                    //excluding tax
                    var paymentMethodAdditionalFeeExclTaxInUserCurrency =
                        _currencyService.ConvertCurrency(order.PaymentMethodAdditionalFeeExclTax, order.CurrencyRate);
                    result.PaymentMethodAdditionalFee = await _priceFormatter.FormatPaymentMethodAdditionalFeeAsync(
                        paymentMethodAdditionalFeeExclTaxInUserCurrency, true, order.UserCurrencyCode, languageId, false);
                }
            }

            //tax
            var taxStr = string.Empty;
            var taxRates = new SortedDictionary<decimal, decimal>();
            bool displayTax;
            var displayTaxRates = true;
            if (_taxSettings.HideTaxInOrderSummary && order.UserTaxDisplayType == TaxDisplayType.IncludingTax)
            {
                displayTax = false;
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
                    taxRates = _orderService.ParseTaxRates(order, order.TaxRates);

                    displayTaxRates = _taxSettings.DisplayTaxRates && taxRates.Any();
                    displayTax = !displayTaxRates;

                    var orderTaxInUserCurrency = _currencyService.ConvertCurrency(order.OrderTax, order.CurrencyRate);
                    taxStr = await _priceFormatter.FormatPriceAsync(orderTaxInUserCurrency, true, order.UserCurrencyCode,
                        false, languageId);
                }
            }

            if (displayTax)
            {
                result.Tax = taxStr;
            }

            if (displayTaxRates)
            {
                foreach (var item in taxRates)
                {
                    var taxRate = string.Format(await _localizationService.GetResourceAsync("Pdf.TaxRate", languageId),
                        _priceFormatter.FormatTaxRate(item.Key));
                    var taxValue = await _priceFormatter.FormatPriceAsync(
                        _currencyService.ConvertCurrency(item.Value, order.CurrencyRate), true, order.UserCurrencyCode,
                        false, languageId);

                    result.TaxRates.Add($"{taxRate} {taxValue}");
                }
            }

            //discount (applied to order total)
            if (order.OrderDiscount > decimal.Zero)
            {
                var orderDiscountInUserCurrency =
                    _currencyService.ConvertCurrency(order.OrderDiscount, order.CurrencyRate);
                result.Discount = await _priceFormatter.FormatPriceAsync(-orderDiscountInUserCurrency,
                    true, order.UserCurrencyCode, false, languageId);
            }

            //gift cards
            foreach (var gcuh in await _giftCardService.GetGiftCardUsageHistoryAsync(order))
            {
                var gcTitle = string.Format(await _localizationService.GetResourceAsync("Pdf.GiftCardInfo", languageId),
                    (await _giftCardService.GetGiftCardByIdAsync(gcuh.GiftCardId))?.GiftCardCouponCode);
                var gcAmountStr = await _priceFormatter.FormatPriceAsync(
                    -_currencyService.ConvertCurrency(gcuh.UsedValue, order.CurrencyRate), true,
                    order.UserCurrencyCode, false, languageId);

                result.GiftCards.Add($"{gcTitle} {gcAmountStr}");
            }

            //reward points
            if (order.RedeemedRewardPointsEntryId.HasValue && await _rewardPointService.GetRewardPointsHistoryEntryByIdAsync(order.RedeemedRewardPointsEntryId.Value) is RewardPointsHistory redeemedRewardPointsEntry)
            {
                var rpTitle = string.Format(await _localizationService.GetResourceAsync("Pdf.RewardPoints", languageId),
                    -redeemedRewardPointsEntry.Points);
                var rpAmount = await _priceFormatter.FormatPriceAsync(
                    -_currencyService.ConvertCurrency(redeemedRewardPointsEntry.UsedAmount, order.CurrencyRate),
                    true, order.UserCurrencyCode, false, languageId);

                result.RewardPoints = $"{rpTitle} {rpAmount}";
            }

            //order total
            var orderTotalInUserCurrency = _currencyService.ConvertCurrency(order.OrderTotal, order.CurrencyRate);
            var orderTotalStr = await _priceFormatter.FormatPriceAsync(orderTotalInUserCurrency, true, order.UserCurrencyCode, false, languageId);
            result.OrderTotal = $"{await _localizationService.GetResourceAsync("Pdf.OrderTotal", languageId)} {orderTotalStr}";

            return result;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Write PDF invoice to the specified stream
        /// </summary>
        /// <param name="stream">Stream to save PDF</param>
        /// <param name="order">Order</param>
        /// <param name="language">Language; null to use a language used when placing an order</param>
        /// <param name="store">Store</param>
        /// <param name="vendor">Vendor to limit tvChannels; null to print all tvChannels. If specified, then totals won't be printed</param>
        /// <returns>
        /// Задача представляет асинхронную операцию
        /// </returns>
        public virtual async Task PrintOrderToPdfAsync(Stream stream, Order order, Language language = null, Store store = null, Vendor vendor = null)
        {
            if (order == null)
                throw new ArgumentNullException(nameof(order));

            //store info
            store ??= await _storeContext.GetCurrentStoreAsync();

            var orderStore = order.StoreId == 0 || order.StoreId == store?.Id ?
                store : await _storeService.GetStoreByIdAsync(order.StoreId);

            //language info
            language ??= await _languageService.GetLanguageByIdAsync(order.UserLanguageId);

            if (language?.Published != true)
                language = await _workContext.GetWorkingLanguageAsync();

            //by default _pdfSettings contains settings for the current active store
            //and we need PdfSettings for the store which was used to place an order
            //so let's load it based on a store of the current order
            var pdfSettingsByStore = await _settingService.LoadSettingAsync<PdfSettings>(orderStore.Id);

            byte[] logo = null;
            var logoPicture = await _pictureService.GetPictureByIdAsync(pdfSettingsByStore.LogoPictureId);
            if (logoPicture != null)
            {
                var logoFilePath = await _pictureService.GetThumbLocalPathAsync(logoPicture, 0, false);

                if (logoPicture.MimeType == MimeTypes.ImageSvg)
                {
                    logo = await _pictureService.ConvertSvgToPngAsync(logoFilePath);
                }
                else
                {
                    logo = await _fileProvider.ReadAllBytesAsync(logoFilePath);
                }
            }

            var date = await _dateTimeHelper.ConvertToUserTimeAsync(order.CreatedOnUtc, DateTimeKind.Utc);

            //a vendor should have access only to tvChannels
            var orderItems = await _orderService.GetOrderItemsAsync(order.Id, vendorId: vendor?.Id ?? 0);

            var column1Lines = string.IsNullOrEmpty(pdfSettingsByStore.InvoiceFooterTextColumn1) ?
                new List<string>()
                : pdfSettingsByStore.InvoiceFooterTextColumn1
                    .Split(new[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries)
                    .ToList();

            var column2Lines = string.IsNullOrEmpty(pdfSettingsByStore.InvoiceFooterTextColumn2) ?
                new List<string>()
                : pdfSettingsByStore.InvoiceFooterTextColumn2
                    .Split(new[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries)
                    .ToList();

            var source = new InvoiceSource()
            {
                StoreUrl = orderStore.Url?.Trim('/'),
                Language = language,
                FontFamily = pdfSettingsByStore.FontFamily,
                OrderDateUser = date,
                LogoData = logo,
                OrderNumberText = order.CustomOrderNumber,
                PageSize = pdfSettingsByStore.LetterPageSizeEnabled ? PageSizes.Letter : PageSizes.A4,
                BillingAddress = await GetBillingAddressAsync(vendor, language, order),
                ShippingAddress = await GetShippingAddressAsync(language, order),
                TvChannels = await GetOrderTvChannelItemsAsync(order, orderItems, language),
                ShowSkuInTvChannelList = _catalogSettings.ShowSkuOnTvChannelDetailsPage,
                ShowVendorInTvChannelList = _vendorSettings.ShowVendorOnOrderDetailsPage,
                CheckoutAttributes = vendor is null ? order.CheckoutAttributeDescription : string.Empty, //vendors cannot see checkout attributes
                Totals = vendor is null ? await GetTotalsAsync(language, order) : new(), //vendors cannot see totals
                OrderNotes = await GetOrderNotesAsync(pdfSettingsByStore, order, language),
                FooterTextColumn1 = column1Lines,
                FooterTextColumn2 = column2Lines
            };

            await using var pdfStream = new MemoryStream();
            new InvoiceDocument(source, _localizationService)
                .GeneratePdf(pdfStream);

            pdfStream.Position = 0;
            await pdfStream.CopyToAsync(stream);
        }

        /// <summary>
        /// Write ZIP archive with invoices to the specified stream
        /// </summary>
        /// <param name="stream">Stream</param>
        /// <param name="orders">Orders</param>
        /// <param name="language">Language; null to use a language used when placing an order</param>
        /// <param name="vendor">Vendor to limit tvChannels; null to print all tvChannels. If specified, then totals won't be printed</param>
        /// <returns>Задача представляет асинхронную операцию</returns>
        public virtual async Task PrintOrdersToPdfAsync(Stream stream, IList<Order> orders, Language language = null, Vendor vendor = null)
        {
            if (stream == null)
                throw new ArgumentNullException(nameof(stream));

            if (orders == null)
                throw new ArgumentNullException(nameof(orders));

            var currentStore = await _storeContext.GetCurrentStoreAsync();

            using var archive = new ZipArchive(stream, ZipArchiveMode.Create, true);

            foreach (var order in orders)
            {
                var entryName = string.Format("{0} {1}", await _localizationService.GetResourceAsync("Pdf.Order"), order.CustomOrderNumber);

                await using var fileStreamInZip = archive.CreateEntry($"{entryName}.pdf").Open();
                await using var pdfStream = new MemoryStream();
                await PrintOrderToPdfAsync(pdfStream, order, language, currentStore, vendor);
                pdfStream.Position = 0;
                await pdfStream.CopyToAsync(fileStreamInZip);
            }
        }

        /// <summary>
        /// Write ZIP archive with packaging slips to the specified stream
        /// </summary>
        /// <param name="stream">Stream</param>
        /// <param name="shipments">Shipments</param>
        /// <param name="language">Language; null to use a language used when placing an order</param>
        /// <returns>Задача представляет асинхронную операцию</returns>
        public virtual async Task PrintPackagingSlipsToPdfAsync(Stream stream, IList<Shipment> shipments, Language language = null)
        {
            if (stream == null)
                throw new ArgumentNullException(nameof(stream));

            if (shipments == null)
                throw new ArgumentNullException(nameof(shipments));

            using var archive = new ZipArchive(stream, ZipArchiveMode.Create, true);

            foreach (var shipment in shipments)
            {
                var entryName = $"{await _localizationService.GetResourceAsync("Pdf.Shipment")}{shipment.Id}";

                await using var fileStreamInZip = archive.CreateEntry($"{entryName}.pdf").Open();
                await using var pdfStream = new MemoryStream();
                await PrintPackagingSlipToPdfAsync(pdfStream, shipment, language);

                pdfStream.Position = 0;
                await pdfStream.CopyToAsync(fileStreamInZip);
            }
        }

        /// <summary>
        /// Write packaging slip to the specified stream
        /// </summary>
        /// <param name="stream">Stream</param>
        /// <param name="shipment">Shipment</param>
        /// <param name="language">Language; null to use a language used when placing an order</param>
        /// <returns>Задача представляет асинхронную операцию</returns>
        public virtual async Task PrintPackagingSlipToPdfAsync(Stream stream, Shipment shipment, Language language = null)
        {
            if (stream == null)
                throw new ArgumentNullException(nameof(stream));

            if (shipment == null)
                throw new ArgumentNullException(nameof(shipment));

            var order = await _orderService.GetOrderByIdAsync(shipment.OrderId);

            var pdfSettingsByStore = await _settingService.LoadSettingAsync<PdfSettings>(order.StoreId);

            //language info
            language ??= await _languageService.GetLanguageByIdAsync(order.UserLanguageId);

            if (language?.Published != true)
                language = await _workContext.GetWorkingLanguageAsync();

            var shipmentItems = await _shipmentService.GetShipmentItemsByShipmentIdAsync(shipment.Id);

            if (shipmentItems?.Any() != true)
                return;

            var orderItems = await shipmentItems
                .SelectAwait(async si => await _orderService.GetOrderItemByIdAsync(si.OrderItemId))
                .Where(pi => pi != null)
                .ToListAsync();

            if (orderItems?.Any() != true)
                return;

            var source = new ShipmentSource
            {
                PageSize = pdfSettingsByStore.LetterPageSizeEnabled ? PageSizes.Letter : PageSizes.A4,
                Language = language,
                FontFamily = pdfSettingsByStore.FontFamily,
                ShipmentNumberText = shipment.Id.ToString(),
                OrderNumberText = order.CustomOrderNumber,
                Address = await GetShippingAddressAsync(language, order),
                TvChannels = await GetOrderTvChannelItemsAsync(order, orderItems, language)
            };

            await using var pdfStream = new MemoryStream();

            new ShipmentDocument(source, _localizationService)
                .GeneratePdf(pdfStream);

            pdfStream.Position = 0;
            await pdfStream.CopyToAsync(stream);
        }

        /// <summary>
        /// Write PDF catalog to the specified stream
        /// </summary>
        /// <param name="stream">Stream</param>
        /// <param name="tvChannels">TvChannels</param>
        /// <returns>Задача представляет асинхронную операцию</returns>
        public virtual async Task PrintTvChannelsToPdfAsync(Stream stream, IList<TvChannel> tvChannels)
        {
            if (stream == null)
                throw new ArgumentNullException(nameof(stream));

            if (tvChannels == null)
                throw new ArgumentNullException(nameof(tvChannels));

            var currentStore = await _storeContext.GetCurrentStoreAsync();
            var pdfSettingsByStore = await _settingService.LoadSettingAsync<PdfSettings>(currentStore.Id);
            var lang = await _workContext.GetWorkingLanguageAsync();

            var tvChannelItems = new List<CatalogItem>();

            foreach (var tvChannel in tvChannels)
            {
                var priceStr = $"{tvChannel.Price:0.00} {(await _currencyService.GetCurrencyByIdAsync(_currencySettings.PrimaryStoreCurrencyId)).CurrencyCode}";
                if (tvChannel.IsRental)
                    priceStr = await _priceFormatter.FormatRentalTvChannelPeriodAsync(tvChannel, priceStr);

                var rawDescription = await _localizationService.GetLocalizedAsync(tvChannel, x => x.FullDescription, lang.Id);

                var tvChannelNumber = tvChannels.IndexOf(tvChannel) + 1;
                var tvChannelName = await _localizationService.GetLocalizedAsync(tvChannel, x => x.Name, lang.Id);

                var item = new CatalogItem()
                {
                    Name = $"{tvChannelNumber}. {tvChannelName}",
                    Description = _htmlFormatter.StripTags(_htmlFormatter.ConvertHtmlToPlainText(rawDescription, decode: true)),
                    Price = priceStr,
                    Sku = tvChannel.Sku,
                    Weight = tvChannel.IsShipEnabled && tvChannel.Weight > decimal.Zero ?
                        $"{tvChannel.Weight:0.00} {(await _measureService.GetMeasureWeightByIdAsync(_measureSettings.BaseWeightId)).Name}" :
                        string.Empty,
                    Stock = tvChannel.ManageInventoryMethod == ManageInventoryMethod.ManageStock ?
                        $"{await _tvChannelService.GetTotalStockQuantityAsync(tvChannel)}" :
                        string.Empty
                };

                var pictures = await _pictureService.GetPicturesByTvChannelIdAsync(tvChannel.Id);

                if (pictures.Any())
                {
                    var picturePaths = new HashSet<string>();

                    foreach (var pic in pictures)
                    {
                        var picPath = await _pictureService.GetThumbLocalPathAsync(pic, 200, false);
                        if (!string.IsNullOrEmpty(picPath))
                        {
                            picturePaths.Add(picPath);
                        }
                    }

                    item.PicturePaths = picturePaths;
                }

                tvChannelItems.Add(item);
            }

            var source = new CatalogSource
            {
                Language = lang,
                PageSize = pdfSettingsByStore.LetterPageSizeEnabled ? PageSizes.Letter : PageSizes.A4,
                FontFamily = pdfSettingsByStore.FontFamily,
                TvChannels = tvChannelItems
            };

            await using var pdfStream = new MemoryStream();

            new CatalogDocument(source, _localizationService)
                .GeneratePdf(pdfStream);

            pdfStream.Position = 0;
            await pdfStream.CopyToAsync(stream);
        }

        /// <summary>
        /// Export an order to PDF and save to disk
        /// </summary>
        /// <param name="order">Order</param>
        /// <param name="language">Language identifier; null to use a language used when placing an order</param>
        /// <param name="vendor">Vendor to limit tvChannels; null to print all tvChannels. If specified, then totals won't be printed</param>
        /// <returns>
        /// Задача представляет асинхронную операцию
        /// The task result contains a path of generated file
        /// </returns>
        public virtual async Task<string> SaveOrderPdfToDiskAsync(Order order, Language language = null, Vendor vendor = null)
        {
            var fileName = $"order_{order.OrderGuid}_{CommonHelper.GenerateRandomDigitCode(4)}.pdf";
            var filePath = _fileProvider.Combine(_fileProvider.MapPath("~/wwwroot/files/exportimport"), fileName);
            await using var fileStream = new FileStream(filePath, FileMode.Create);

            await PrintOrderToPdfAsync(fileStream, order, language, store: null, vendor: vendor);

            return filePath;
        }

        #endregion
    }
}