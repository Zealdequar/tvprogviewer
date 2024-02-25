using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using TvProgViewer.Core;
using TvProgViewer.Core.Domain.Catalog;
using TvProgViewer.Core.Domain.Common;
using TvProgViewer.Core.Domain.Users;
using TvProgViewer.Core.Domain.Directory;
using TvProgViewer.Core.Domain.Orders;
using TvProgViewer.Core.Domain.Shipping;
using TvProgViewer.Core.Domain.Tax;
using TvProgViewer.Core.Events;
using TvProgViewer.Services.Common;
using TvProgViewer.Services.Users;
using TvProgViewer.Services.Directory;
using TvProgViewer.Services.Logging;
using TvProgViewer.Services.Tax.Events;

namespace TvProgViewer.Services.Tax
{
    /// <summary>
    /// Tax service
    /// </summary>
    public partial class TaxService : ITaxService
    {
        #region Fields

        protected readonly AddressSettings _addressSettings;
        protected readonly UserSettings _userSettings;
        protected readonly IAddressService _addressService;
        protected readonly ICountryService _countryService;
        protected readonly IUserService _userService;
        protected readonly IEventPublisher _eventPublisher;
        protected readonly IGenericAttributeService _genericAttributeService;
        protected readonly IGeoLookupService _geoLookupService;
        protected readonly ILogger _logger;
        protected readonly IStateProvinceService _stateProvinceService;
        protected readonly IStoreContext _storeContext;
        protected readonly ITaxPluginManager _taxPluginManager;
        protected readonly IWebHelper _webHelper;
        protected readonly IWorkContext _workContext;
        protected readonly ShippingSettings _shippingSettings;
        protected readonly TaxSettings _taxSettings;

        #endregion

        #region Ctor

        public TaxService(AddressSettings addressSettings,
            UserSettings userSettings,
            IAddressService addressService,
            ICountryService countryService,
            IUserService userService,
            IEventPublisher eventPublisher,
            IGenericAttributeService genericAttributeService,
            IGeoLookupService geoLookupService,
            ILogger logger,
            IStateProvinceService stateProvinceService,
            IStoreContext storeContext,
            ITaxPluginManager taxPluginManager,
            IWebHelper webHelper,
            IWorkContext workContext,
            ShippingSettings shippingSettings,
            TaxSettings taxSettings)
        {
            _addressSettings = addressSettings;
            _userSettings = userSettings;
            _addressService = addressService;
            _countryService = countryService;
            _userService = userService;
            _eventPublisher = eventPublisher;
            _genericAttributeService = genericAttributeService;
            _geoLookupService = geoLookupService;
            _logger = logger;
            _stateProvinceService = stateProvinceService;
            _storeContext = storeContext;
            _taxPluginManager = taxPluginManager;
            _webHelper = webHelper;
            _workContext = workContext;
            _shippingSettings = shippingSettings;
            _taxSettings = taxSettings;
        }

        #endregion

        #region Utilities

        /// <summary>
        /// Get a value indicating whether a user is consumer (a person, not a company) located in Europe Union
        /// </summary>
        /// <param name="user">User</param>
        /// <returns>
        /// Задача представляет асинхронную операцию
        /// The task result contains the result
        /// </returns>
        protected virtual async Task<bool> IsEuConsumerAsync(User user)
        {
            if (user == null)
                throw new ArgumentNullException(nameof(user));

            Country country = null;

            //get country from billing address
            if (_addressSettings.CountryEnabled && await _userService.GetUserShippingAddressAsync(user) is Address billingAddress)
                country = await _countryService.GetCountryByAddressAsync(billingAddress);

            //get country specified during registration?
            if (country == null && _userSettings.CountryEnabled)
                country = await _countryService.GetCountryByIdAsync(user.CountryId);

            //get country by IP address
            if (country == null)
            {
                var ipAddress = _webHelper.GetCurrentIpAddress();
                var countryIsoCode = _geoLookupService.LookupCountryIsoCode(ipAddress);
                country = await _countryService.GetCountryByTwoLetterIsoCodeAsync(countryIsoCode);
            }

            //we cannot detect country
            if (country == null)
                return false;

            //outside EU
            if (!country.SubjectToVat)
                return false;

            //company (business) or consumer?
            var userVatStatus = (VatNumberStatus)user.VatNumberStatusId;
            if (userVatStatus == VatNumberStatus.Valid)
                return false;

            //consumer
            return true;
        }

        /// <summary>
        /// Gets a default tax address
        /// </summary>
        /// <returns>
        /// Задача представляет асинхронную операцию
        /// The task result contains the address
        /// </returns>
        protected virtual async Task<Address> LoadDefaultTaxAddressAsync()
        {
            var addressId = _taxSettings.DefaultTaxAddressId;

            return await _addressService.GetAddressByIdAsync(addressId);
        }

        /// <summary>
        /// Gets or sets a pickup point address for tax calculation
        /// </summary>
        /// <param name="pickupPoint">Pickup point</param>
        /// <returns>
        /// Задача представляет асинхронную операцию
        /// The task result contains the address
        /// </returns>
        protected virtual async Task<Address> LoadPickupPointTaxAddressAsync(PickupPoint pickupPoint)
        {
            if (pickupPoint == null)
                throw new ArgumentNullException(nameof(pickupPoint));

            var country = await _countryService.GetCountryByTwoLetterIsoCodeAsync(pickupPoint.CountryCode);
            var state = await _stateProvinceService.GetStateProvinceByAbbreviationAsync(pickupPoint.StateAbbreviation, country?.Id);

            return new Address
            {
                CountryId = country?.Id ?? 0,
                StateProvinceId = state?.Id ?? 0,
                County = pickupPoint.County,
                City = pickupPoint.City,
                Address1 = pickupPoint.Address,
                ZipPostalCode = pickupPoint.ZipPostalCode
            };
        }

        /// <summary>
        /// Prepare request to get tax rate
        /// </summary>
        /// <param name="tvchannel">TvChannel</param>
        /// <param name="taxCategoryId">Tax category identifier</param>
        /// <param name="user">User</param>
        /// <param name="price">Price</param>
        /// <returns>
        /// Задача представляет асинхронную операцию
        /// The task result contains the package for tax calculation
        /// </returns>
        protected virtual async Task<TaxRateRequest> PrepareTaxRateRequestAsync(TvChannel tvchannel, int taxCategoryId, User user, decimal price)
        {
            if (user == null)
                throw new ArgumentNullException(nameof(user));

            var store = await _storeContext.GetCurrentStoreAsync();
            var taxRateRequest = new TaxRateRequest
            {
                User = user,
                TvChannel = tvchannel,
                Price = price,
                TaxCategoryId = taxCategoryId > 0 ? taxCategoryId : tvchannel?.TaxCategoryId ?? 0,
                CurrentStoreId = store.Id
            };

            var basedOn = _taxSettings.TaxBasedOn;

            //new EU VAT rules starting January 1st 2015
            //find more info at http://ec.europa.eu/taxation_customs/taxation/vat/how_vat_works/telecom/index_en.htm#new_rules
            var overriddenBasedOn =
                //EU VAT enabled?
                _taxSettings.EuVatEnabled &&
                //telecommunications, broadcasting and electronic services?
                tvchannel != null && tvchannel.IsTelecommunicationsOrBroadcastingOrElectronicServices &&
                //January 1st 2015 passed? Yes, not required anymore
                //DateTime.UtcNow > new DateTime(2015, 1, 1, 0, 0, 0, DateTimeKind.Utc) &&
                //Europe Union consumer?
                await IsEuConsumerAsync(user);
            if (overriddenBasedOn)
            {
                //We must charge VAT in the EU country where the user belongs (not where the business is based)
                basedOn = TaxBasedOn.BillingAddress;
            }

            //tax is based on pickup point address
            if (!overriddenBasedOn && _taxSettings.TaxBasedOnPickupPointAddress && _shippingSettings.AllowPickupInStore)
            {
                var pickupPoint = await _genericAttributeService.GetAttributeAsync<PickupPoint>(user,
                    TvProgUserDefaults.SelectedPickupPointAttribute, store.Id);
                if (pickupPoint != null)
                {
                    taxRateRequest.Address = await LoadPickupPointTaxAddressAsync(pickupPoint);
                    return taxRateRequest;
                }
            }

            if (basedOn == TaxBasedOn.BillingAddress && user.BillingAddressId == null ||
                basedOn == TaxBasedOn.ShippingAddress && user.ShippingAddressId == null)
                basedOn = TaxBasedOn.DefaultAddress;

            switch (basedOn)
            {
                case TaxBasedOn.BillingAddress:
                    var billingAddress = await _userService.GetUserBillingAddressAsync(user);
                    taxRateRequest.Address = billingAddress;
                    break;
                case TaxBasedOn.ShippingAddress:
                    var shippingAddress = await _userService.GetUserShippingAddressAsync(user);
                    taxRateRequest.Address = shippingAddress;
                    break;
                case TaxBasedOn.DefaultAddress:
                default:
                    taxRateRequest.Address = await LoadDefaultTaxAddressAsync();
                    break;
            }

            return taxRateRequest;
        }

        /// <summary>
        /// Calculated price
        /// </summary>
        /// <param name="price">Price</param>
        /// <param name="percent">Percent</param>
        /// <param name="increase">Increase</param>
        /// <returns>New price</returns>
        protected virtual decimal CalculatePrice(decimal price, decimal percent, bool increase)
        {
            if (percent == decimal.Zero)
                return price;

            decimal result;
            if (increase)
                result = price * (1 + percent / 100);
            else
                result = price - price / (100 + percent) * percent;

            return result;
        }

        /// <summary>
        /// Gets tax rate
        /// </summary>
        /// <param name="tvchannel">TvChannel</param>
        /// <param name="taxCategoryId">Tax category identifier</param>
        /// <param name="user">User</param>
        /// <param name="price">Price (taxable value)</param>
        /// <returns>
        /// Задача представляет асинхронную операцию
        /// The task result contains the calculated tax rate. A value indicating whether a request is taxable
        /// </returns>
        protected virtual async Task<(decimal taxRate, bool isTaxable)> GetTaxRateAsync(TvChannel tvchannel, int taxCategoryId,
            User user, decimal price)
        {
            var taxRate = decimal.Zero;

            //active tax provider
            var store = await _storeContext.GetCurrentStoreAsync();
            var activeTaxProvider = await _taxPluginManager.LoadPrimaryPluginAsync(user, store.Id);
            if (activeTaxProvider == null)
                return (taxRate, true);

            //tax request
            var taxRateRequest = await PrepareTaxRateRequestAsync(tvchannel, taxCategoryId, user, price);

            var isTaxable = !await IsTaxExemptAsync(tvchannel, taxRateRequest.User);

            //tax exempt

            //make EU VAT exempt validation (the European Union Value Added Tax)
            if (isTaxable &&
                _taxSettings.EuVatEnabled &&
                await IsVatExemptAsync(taxRateRequest.Address, taxRateRequest.User))
                //VAT is not chargeable
                isTaxable = false;

            //get tax rate
            var taxRateResult = await activeTaxProvider.GetTaxRateAsync(taxRateRequest);

            //tax rate is calculated, now consumers can adjust it
            await _eventPublisher.PublishAsync(new TaxRateCalculatedEvent(taxRateResult));

            if (taxRateResult.Success)
            {
                //ensure that tax is equal or greater than zero
                if (taxRateResult.TaxRate < decimal.Zero)
                    taxRateResult.TaxRate = decimal.Zero;

                taxRate = taxRateResult.TaxRate;
            }
            else if (_taxSettings.LogErrors)
                foreach (var error in taxRateResult.Errors)
                    await _logger.ErrorAsync($"{activeTaxProvider.PluginDescriptor.FriendlyName} - {error}", null, user);

            return (taxRate, isTaxable);
        }

        /// <summary>
        /// Gets VAT Number status
        /// </summary>
        /// <param name="twoLetterIsoCode">Two letter ISO code of a country</param>
        /// <param name="vatNumber">VAT number</param>
        /// <returns>
        /// Задача представляет асинхронную операцию
        /// The task result contains the vAT Number status. Name (if received). Address (if received)
        /// </returns>
        protected virtual async Task<(VatNumberStatus vatNumberStatus, string name, string address)> GetVatNumberStatusAsync(string twoLetterIsoCode, string vatNumber)
        {
            var name = string.Empty;
            var address = string.Empty;

            if (string.IsNullOrEmpty(twoLetterIsoCode))
                return (VatNumberStatus.Empty, name, address);

            if (string.IsNullOrEmpty(vatNumber))
                return (VatNumberStatus.Empty, name, address);

            if (_taxSettings.EuVatAssumeValid)
                return (VatNumberStatus.Valid, name, address);

            if (!_taxSettings.EuVatUseWebService)
                return (VatNumberStatus.Unknown, name, address);

            var rez = await DoVatCheckAsync(twoLetterIsoCode, vatNumber);

            return (rez.vatNumberStatus, rez.name, rez.address);
        }

        /// <summary>
        /// Performs a basic check of a VAT number for validity
        /// </summary>
        /// <param name="twoLetterIsoCode">Two letter ISO code of a country</param>
        /// <param name="vatNumber">VAT number</param>
        /// <returns>
        /// Задача представляет асинхронную операцию
        /// The task result contains the vAT number status. Company name. Address. Exception
        /// </returns>
        protected virtual async Task<(VatNumberStatus vatNumberStatus, string name, string address, Exception exception)> DoVatCheckAsync(string twoLetterIsoCode, string vatNumber)
        {
            if (vatNumber == null)
                vatNumber = string.Empty;
            vatNumber = vatNumber.Trim().Replace(" ", string.Empty);

            if (twoLetterIsoCode == null)
                twoLetterIsoCode = string.Empty;
            if (!string.IsNullOrEmpty(twoLetterIsoCode))
                //The service returns INVALID_INPUT for country codes that are not uppercase.
                twoLetterIsoCode = twoLetterIsoCode.ToUpperInvariant();

            string name;
            string address;

            try
            {
                var s = new EuropaCheckVatService.checkVatPortTypeClient();
                var result = await s.checkVatAsync(new EuropaCheckVatService.checkVatRequest
                {
                    vatNumber = vatNumber,
                    countryCode = twoLetterIsoCode
                });

                var valid = result.valid;
                name = result.name;
                address = result.address;

                return (valid ? VatNumberStatus.Valid : VatNumberStatus.Invalid, name, address, null);
            }
            catch (Exception ex)
            {
                name = address = string.Empty;
                var exception = ex;

                return (VatNumberStatus.Unknown, name, address, exception);
            }
        }

        /// <summary>
        /// Gets a value indicating whether EU VAT exempt (the European Union Value Added Tax)
        /// </summary>
        /// <param name="address">Address</param>
        /// <param name="user">User</param>
        /// <returns>
        /// Задача представляет асинхронную операцию
        /// The task result contains the result
        /// </returns>
        protected virtual async Task<bool> IsVatExemptAsync(Address address, User user)
        {
            if (!_taxSettings.EuVatEnabled)
                return false;

            if (user == null || address == null)
                return false;

            var country = await _countryService.GetCountryByIdAsync(address.CountryId ?? 0);
            if (country == null)
                return false;

            if (!country.SubjectToVat)
                // VAT not chargeable if shipping outside VAT zone
                return true;

            // VAT not chargeable if address, user and config meet our VAT exemption requirements:
            // returns true if this user is VAT exempt because they are shipping within the EU but outside our shop country, they have supplied a validated VAT number, and the shop is configured to allow VAT exemption
            var userVatStatus = (VatNumberStatus)user.VatNumberStatusId;

            return country.Id != _taxSettings.EuVatShopCountryId &&
                   userVatStatus == VatNumberStatus.Valid &&
                   _taxSettings.EuVatAllowVatExemption;
        }

        #endregion

        #region Methods

        #region TvChannel price

        /// <summary>
        /// Gets price
        /// </summary>
        /// <param name="tvchannel">TvChannel</param>
        /// <param name="price">Price</param>
        /// <returns>
        /// Задача представляет асинхронную операцию
        /// The task result contains the price. Tax rate
        /// </returns>
        public virtual async Task<(decimal price, decimal taxRate)> GetTvChannelPriceAsync(TvChannel tvchannel, decimal price)
        {
            var user = await _workContext.GetCurrentUserAsync();

            return await GetTvChannelPriceAsync(tvchannel, price, user);
        }

        /// <summary>
        /// Gets price
        /// </summary>
        /// <param name="tvchannel">TvChannel</param>
        /// <param name="price">Price</param>
        /// <param name="user">User</param>
        /// <returns>
        /// Задача представляет асинхронную операцию
        /// The task result contains the price. Tax rate
        /// </returns>
        public virtual async Task<(decimal price, decimal taxRate)> GetTvChannelPriceAsync(TvChannel tvchannel, decimal price,
            User user)
        {
            var includingTax = await _workContext.GetTaxDisplayTypeAsync() == TaxDisplayType.IncludingTax;
            return await GetTvChannelPriceAsync(tvchannel, price, includingTax, user);
        }

        /// <summary>
        /// Gets price
        /// </summary>
        /// <param name="tvchannel">TvChannel</param>
        /// <param name="price">Price</param>
        /// <param name="includingTax">A value indicating whether calculated price should include tax</param>
        /// <param name="user">User</param>
        /// <returns>
        /// Задача представляет асинхронную операцию
        /// The task result contains the price. Tax rate
        /// </returns>
        public virtual async Task<(decimal price, decimal taxRate)> GetTvChannelPriceAsync(TvChannel tvchannel, decimal price,
            bool includingTax, User user)
        {
            var priceIncludesTax = _taxSettings.PricesIncludeTax;
            var taxCategoryId = 0;
            return await GetTvChannelPriceAsync(tvchannel, taxCategoryId, price, includingTax, user, priceIncludesTax);
        }

        /// <summary>
        /// Gets price
        /// </summary>
        /// <param name="tvchannel">TvChannel</param>
        /// <param name="taxCategoryId">Tax category identifier</param>
        /// <param name="price">Price</param>
        /// <param name="includingTax">A value indicating whether calculated price should include tax</param>
        /// <param name="user">User</param>
        /// <param name="priceIncludesTax">A value indicating whether price already includes tax</param>
        /// <returns>
        /// Задача представляет асинхронную операцию
        /// The task result contains the price. Tax rate
        /// </returns>
        public virtual async Task<(decimal price, decimal taxRate)> GetTvChannelPriceAsync(TvChannel tvchannel, int taxCategoryId,
            decimal price, bool includingTax, User user,
            bool priceIncludesTax)
        {
            var taxRate = decimal.Zero;

            //no need to calculate tax rate if passed "price" is 0
            if (price == decimal.Zero) 
                return (price, taxRate);

            bool isTaxable;

            (taxRate, isTaxable) = await GetTaxRateAsync(tvchannel, taxCategoryId, user, price);

            if (priceIncludesTax)
            {
                //"price" already includes tax
                if (includingTax)
                {
                    //we should calculate price WITH tax
                    if (!isTaxable)
                    {
                        //but our request is not taxable
                        //hence we should calculate price WITHOUT tax
                        price = CalculatePrice(price, taxRate, false);
                    }
                }
                else
                {
                    //we should calculate price WITHOUT tax
                    price = CalculatePrice(price, taxRate, false);
                }
            }
            else
            {
                //"price" doesn't include tax
                if (includingTax)
                {
                    //we should calculate price WITH tax
                    //do it only when price is taxable
                    if (isTaxable)
                    {
                        price = CalculatePrice(price, taxRate, true);
                    }
                }
            }

            if (!isTaxable)
            {
                //we return 0% tax rate in case a request is not taxable
                taxRate = decimal.Zero;
            }

            //allowed to support negative price adjustments
            //if (price < decimal.Zero)
            //    price = decimal.Zero;

            return (price, taxRate);
        }

        /// <summary>
        /// Gets a value indicating whether a tvchannel is tax exempt
        /// </summary>
        /// <param name="tvchannel">TvChannel</param>
        /// <param name="user">User</param>
        /// <returns>
        /// Задача представляет асинхронную операцию
        /// The task result contains a value indicating whether a tvchannel is tax exempt
        /// </returns>
        public virtual async Task<bool> IsTaxExemptAsync(TvChannel tvchannel, User user)
        {
            if (user != null)
            {
                if (user.IsTaxExempt)
                    return true;

                if ((await _userService.GetUserRolesAsync(user)).Any(cr => cr.TaxExempt))
                    return true;
            }

            if (tvchannel == null)
                return false;

            if (tvchannel.IsTaxExempt)
                return true;

            return false;
        }

        #endregion

        #region Shipping price

        /// <summary>
        /// Gets shipping price
        /// </summary>
        /// <param name="price">Price</param>
        /// <param name="user">User</param>
        /// <returns>
        /// Задача представляет асинхронную операцию
        /// The task result contains the price. Tax rate
        /// </returns>
        public virtual async Task<(decimal price, decimal taxRate)> GetShippingPriceAsync(decimal price, User user)
        {
            var includingTax = await _workContext.GetTaxDisplayTypeAsync() == TaxDisplayType.IncludingTax;

            return await GetShippingPriceAsync(price, includingTax, user);
        }

        /// <summary>
        /// Gets shipping price
        /// </summary>
        /// <param name="price">Price</param>
        /// <param name="includingTax">A value indicating whether calculated price should include tax</param>
        /// <param name="user">User</param>
        /// <returns>
        /// Задача представляет асинхронную операцию
        /// The task result contains the price. Tax rate
        /// </returns>
        public virtual async Task<(decimal price, decimal taxRate)> GetShippingPriceAsync(decimal price, bool includingTax, User user)
        {
            var taxRate = decimal.Zero;

            if (!_taxSettings.ShippingIsTaxable)
            {
                return (price, taxRate);
            }

            var taxClassId = _taxSettings.ShippingTaxClassId;
            var priceIncludesTax = _taxSettings.ShippingPriceIncludesTax;

            return await GetTvChannelPriceAsync(null, taxClassId, price, includingTax, user, priceIncludesTax);
        }

        #endregion

        #region Payment additional fee

        /// <summary>
        /// Gets payment method additional handling fee
        /// </summary>
        /// <param name="price">Price</param>
        /// <param name="user">User</param>
        /// <returns>
        /// Задача представляет асинхронную операцию
        /// The task result contains the price. Tax rate
        /// </returns>
        public virtual async Task<(decimal price, decimal taxRate)> GetPaymentMethodAdditionalFeeAsync(decimal price, User user)
        {
            var includingTax = await _workContext.GetTaxDisplayTypeAsync() == TaxDisplayType.IncludingTax;
            
            return await GetPaymentMethodAdditionalFeeAsync(price, includingTax, user);
        }

        /// <summary>
        /// Gets payment method additional handling fee
        /// </summary>
        /// <param name="price">Price</param>
        /// <param name="includingTax">A value indicating whether calculated price should include tax</param>
        /// <param name="user">User</param>
        /// <returns>
        /// Задача представляет асинхронную операцию
        /// The task result contains the price. Tax rate
        /// </returns>
        public virtual async Task<(decimal price, decimal taxRate)> GetPaymentMethodAdditionalFeeAsync(decimal price, bool includingTax, User user)
        {
            var taxRate = decimal.Zero;

            if (!_taxSettings.PaymentMethodAdditionalFeeIsTaxable)
            {
                return (price, taxRate);
            }

            var taxClassId = _taxSettings.PaymentMethodAdditionalFeeTaxClassId;
            var priceIncludesTax = _taxSettings.PaymentMethodAdditionalFeeIncludesTax;
            return await GetTvChannelPriceAsync(null, taxClassId, price, includingTax, user, priceIncludesTax);
        }

        #endregion

        #region Checkout attribute price

        /// <summary>
        /// Gets checkout attribute value price
        /// </summary>
        /// <param name="ca">Checkout attribute</param>
        /// <param name="cav">Checkout attribute value</param>
        /// <returns>
        /// Задача представляет асинхронную операцию
        /// The task result contains the price. Tax rate
        /// </returns>
        public virtual async Task<(decimal price, decimal taxRate)> GetCheckoutAttributePriceAsync(CheckoutAttribute ca, CheckoutAttributeValue cav)
        {
            var user = await _workContext.GetCurrentUserAsync();

            return await GetCheckoutAttributePriceAsync(ca, cav, user);
        }

        /// <summary>
        /// Gets checkout attribute value price
        /// </summary>
        /// <param name="ca">Checkout attribute</param>
        /// <param name="cav">Checkout attribute value</param>
        /// <param name="user">User</param>
        /// <returns>
        /// Задача представляет асинхронную операцию
        /// The task result contains the price. Tax rate
        /// </returns>
        public virtual async Task<(decimal price, decimal taxRate)> GetCheckoutAttributePriceAsync(CheckoutAttribute ca, CheckoutAttributeValue cav, User user)
        {
            var includingTax = await _workContext.GetTaxDisplayTypeAsync() == TaxDisplayType.IncludingTax;

            return await GetCheckoutAttributePriceAsync(ca, cav, includingTax, user);
        }

        /// <summary>
        /// Gets checkout attribute value price
        /// </summary>
        /// <param name="ca">Checkout attribute</param>
        /// <param name="cav">Checkout attribute value</param>
        /// <param name="includingTax">A value indicating whether calculated price should include tax</param>
        /// <param name="user">User</param>
        /// <returns>
        /// Задача представляет асинхронную операцию
        /// The task result contains the price. Tax rate
        /// </returns>
        public virtual async Task<(decimal price, decimal taxRate)> GetCheckoutAttributePriceAsync(CheckoutAttribute ca, CheckoutAttributeValue cav,
            bool includingTax, User user)
        {
            if (cav == null)
                throw new ArgumentNullException(nameof(cav));

            var taxRate = decimal.Zero;

            var price = cav.PriceAdjustment;
            if (ca.IsTaxExempt) 
                return (price, taxRate);

            var priceIncludesTax = _taxSettings.PricesIncludeTax;
            var taxClassId = ca.TaxCategoryId;

            return await GetTvChannelPriceAsync(null, taxClassId, price, includingTax, user, priceIncludesTax);
        }

        #endregion

        #region VAT
        
        /// <summary>
        /// Gets VAT Number status
        /// </summary>
        /// <param name="fullVatNumber">Two letter ISO code of a country and VAT number (e.g. GB 111 1111 111)</param>
        /// <returns>
        /// Задача представляет асинхронную операцию
        /// The task result contains the vAT Number status. Name (if received). Address (if received)
        /// </returns>
        public virtual async Task<(VatNumberStatus vatNumberStatus, string name, string address)> GetVatNumberStatusAsync(string fullVatNumber)
        {
            var name = string.Empty;
            var address = string.Empty;

            if (string.IsNullOrWhiteSpace(fullVatNumber))
                return (VatNumberStatus.Empty, name, address);
            fullVatNumber = fullVatNumber.Trim();

            //GB 111 1111 111 or GB 1111111111
            //more advanced regex - http://codeigniter.com/wiki/European_Vat_Checker
            var r = new Regex(@"^(\w{2})(.*)");
            var match = r.Match(fullVatNumber);
            if (!match.Success)
                return (VatNumberStatus.Invalid, name, address); 

            var twoLetterIsoCode = match.Groups[1].Value;
            var vatNumber = match.Groups[2].Value;

            return await GetVatNumberStatusAsync(twoLetterIsoCode, vatNumber);
        }

        #endregion

        #region Tax total

        /// <summary>
        /// Get tax total for the passed shopping cart
        /// </summary>
        /// <param name="cart">Shopping cart</param>
        /// <param name="usePaymentMethodAdditionalFee">A value indicating whether we should use payment method additional fee when calculating tax</param>
        /// <returns>
        /// Задача представляет асинхронную операцию
        /// The task result contains the result
        /// </returns>
        public virtual async Task<TaxTotalResult> GetTaxTotalAsync(IList<ShoppingCartItem> cart, bool usePaymentMethodAdditionalFee = true)
        {
            var user = await _userService.GetShoppingCartUserAsync(cart);
            var store = await _storeContext.GetCurrentStoreAsync();
            var activeTaxProvider = await _taxPluginManager.LoadPrimaryPluginAsync(user, store.Id);
            if (activeTaxProvider == null)
                return null;

            //get result by using primary tax provider
            var taxTotalRequest = new TaxTotalRequest
            {
                ShoppingCart = cart,
                User = user,
                StoreId = store.Id,
                UsePaymentMethodAdditionalFee = usePaymentMethodAdditionalFee
            };
            var taxTotalResult = await activeTaxProvider.GetTaxTotalAsync(taxTotalRequest);

            //tax total is calculated, now consumers can adjust it
            await _eventPublisher.PublishAsync(new TaxTotalCalculatedEvent(taxTotalRequest, taxTotalResult));

            //error logging
            if (taxTotalResult != null && !taxTotalResult.Success && _taxSettings.LogErrors)
            {
                foreach (var error in taxTotalResult.Errors)
                {
                    await _logger.ErrorAsync($"{activeTaxProvider.PluginDescriptor.FriendlyName} - {error}", null, user);
                }
            }

            return taxTotalResult;
        }

        #endregion

        #endregion
    }
}