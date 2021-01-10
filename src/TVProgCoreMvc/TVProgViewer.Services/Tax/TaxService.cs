using System;
using System.Linq;
using System.Text.RegularExpressions;
using TVProgViewer.Core;
using TVProgViewer.Core.Caching;
using TVProgViewer.Core.Domain.Catalog;
using TVProgViewer.Core.Domain.Common;
using TVProgViewer.Core.Domain.Users;
using TVProgViewer.Core.Domain.Directory;
using TVProgViewer.Core.Domain.Orders;
using TVProgViewer.Core.Domain.Shipping;
using TVProgViewer.Core.Domain.Tax;
using TVProgViewer.Services.Common;
using TVProgViewer.Services.Users;
using TVProgViewer.Services.Directory;
using TVProgViewer.Services.Logging;

namespace TVProgViewer.Services.Tax
{
    /// <summary>
    /// Tax service
    /// </summary>
    public partial class TaxService : ITaxService
    {
        #region Fields

        private readonly AddressSettings _addressSettings;
        private readonly UserSettings _UserSettings;
        private readonly IAddressService _addressService;
        private readonly ICountryService _countryService;
        private readonly IUserService _UserService;
        private readonly IGenericAttributeService _genericAttributeService;
        private readonly IGeoLookupService _geoLookupService;
        private readonly ILogger _logger;
        private readonly IStateProvinceService _stateProvinceService;
        private readonly IStoreContext _storeContext;
        private readonly ITaxPluginManager _taxPluginManager;
        private readonly IWebHelper _webHelper;
        private readonly IWorkContext _workContext;
        private readonly ShippingSettings _shippingSettings;
        private readonly TaxSettings _taxSettings;

        #endregion

        #region Ctor

        public TaxService(AddressSettings addressSettings,
            UserSettings UserSettings,
            IAddressService addressService,
            ICountryService countryService,
            IUserService UserService,
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
            _UserSettings = UserSettings;
            _addressService = addressService;
            _countryService = countryService;
            _UserService = UserService;
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
        /// Get a value indicating whether a User is consumer (a person, not a company) located in Europe Union
        /// </summary>
        /// <param name="User">User</param>
        /// <returns>Result</returns>
        protected virtual bool IsEuConsumer(User User)
        {
            if (User == null)
                throw new ArgumentNullException(nameof(User));

            Country country = null;

            //get country specified during registration?
            if (country == null && _UserSettings.CountryEnabled)
            {
                var countryId = _genericAttributeService.GetAttribute<User, int>(User.Id, TvProgUserDefaults.CountryIdAttribute);
                country = _countryService.GetCountryById(countryId);
            }

            //get country by IP address
            if (country == null)
            {
                var ipAddress = _webHelper.GetCurrentIpAddress();
                var countryIsoCode = _geoLookupService.LookupCountryIsoCode(ipAddress);
                country = _countryService.GetCountryByTwoLetterIsoCode(countryIsoCode);
            }

            //we cannot detect country
            if (country == null)
                return false;

            //outside EU
            if (!country.SubjectToVat)
                return false;

            //company (business) or consumer?
            var UserVatStatus = (VatNumberStatus)_genericAttributeService.GetAttribute<int>(User, TvProgUserDefaults.VatNumberStatusIdAttribute);
            if (UserVatStatus == VatNumberStatus.Valid)
                return false;

            //TODO: use specified company name? (both address and registration one)

            //consumer
            return true;
        }

        /// <summary>
        /// Gets a default tax address
        /// </summary>
        /// <returns>Address</returns>
        protected virtual Address LoadDefaultTaxAddress()
        {
            var addressId = _taxSettings.DefaultTaxAddressId;

            return _addressService.GetAddressById(addressId);
        }

        /// <summary>
        /// Gets or sets a pickup point address for tax calculation
        /// </summary>
        /// <param name="pickupPoint">Pickup point</param>
        /// <returns>Address</returns>
        protected virtual Address LoadPickupPointTaxAddress(PickupPoint pickupPoint)
        {
            if (pickupPoint == null)
                throw new ArgumentNullException(nameof(pickupPoint));

            var country = _countryService.GetCountryByTwoLetterIsoCode(pickupPoint.CountryCode);
            var state = _stateProvinceService.GetStateProvinceByAbbreviation(pickupPoint.StateAbbreviation, country?.Id);

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
        /// <param name="product">Product</param>
        /// <param name="taxCategoryId">Tax category identifier</param>
        /// <param name="User">User</param>
        /// <param name="price">Price (taxable value)</param>
        /// <param name="taxRate">Calculated tax rate</param>
        /// <param name="isTaxable">A value indicating whether a request is taxable</param>
        protected virtual void GetTaxRate(Product product, int taxCategoryId,
            User User, decimal price, out decimal taxRate, out bool isTaxable)
        {
            taxRate = decimal.Zero;
            isTaxable = true;

            //active tax provider
            var activeTaxProvider = _taxPluginManager.LoadPrimaryPlugin(User, _storeContext.CurrentStore.Id);
            if (activeTaxProvider == null)
                return;

        }

        #endregion

        #region Methods

        #region Product price

        /// <summary>
        /// Gets price
        /// </summary>
        /// <param name="product">Product</param>
        /// <param name="price">Price</param>
        /// <param name="taxRate">Tax rate</param>
        /// <returns>Price</returns>
        public virtual decimal GetProductPrice(Product product, decimal price,
            out decimal taxRate)
        {
            var User = _workContext.CurrentUser;
            return GetProductPrice(product, price, User, out taxRate);
        }

        /// <summary>
        /// Gets price
        /// </summary>
        /// <param name="product">Product</param>
        /// <param name="price">Price</param>
        /// <param name="User">User</param>
        /// <param name="taxRate">Tax rate</param>
        /// <returns>Price</returns>
        public virtual decimal GetProductPrice(Product product, decimal price,
            User User, out decimal taxRate)
        {
            var includingTax = _workContext.TaxDisplayType == TaxDisplayType.IncludingTax;
            return GetProductPrice(product, price, includingTax, User, out taxRate);
        }

        /// <summary>
        /// Gets price
        /// </summary>
        /// <param name="product">Product</param>
        /// <param name="price">Price</param>
        /// <param name="includingTax">A value indicating whether calculated price should include tax</param>
        /// <param name="User">User</param>
        /// <param name="taxRate">Tax rate</param>
        /// <returns>Price</returns>
        public virtual decimal GetProductPrice(Product product, decimal price,
            bool includingTax, User User, out decimal taxRate)
        {
            var priceIncludesTax = _taxSettings.PricesIncludeTax;
            var taxCategoryId = 0;
            return GetProductPrice(product, taxCategoryId, price, includingTax,
                User, priceIncludesTax, out taxRate);
        }

        /// <summary>
        /// Gets price
        /// </summary>
        /// <param name="product">Product</param>
        /// <param name="taxCategoryId">Tax category identifier</param>
        /// <param name="price">Price</param>
        /// <param name="includingTax">A value indicating whether calculated price should include tax</param>
        /// <param name="User">User</param>
        /// <param name="priceIncludesTax">A value indicating whether price already includes tax</param>
        /// <param name="taxRate">Tax rate</param>
        /// <returns>Price</returns>
        public virtual decimal GetProductPrice(Product product, int taxCategoryId,
            decimal price, bool includingTax, User User,
            bool priceIncludesTax, out decimal taxRate)
        {
            //no need to calculate tax rate if passed "price" is 0
            if (price == decimal.Zero)
            {
                taxRate = decimal.Zero;
                return taxRate;
            }

            GetTaxRate(product, taxCategoryId, User, price, out taxRate, out var isTaxable);

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

            return price;
        }

        #endregion

        #region Shipping price

        /// <summary>
        /// Gets shipping price
        /// </summary>
        /// <param name="price">Price</param>
        /// <param name="User">User</param>
        /// <returns>Price</returns>
        public virtual decimal GetShippingPrice(decimal price, User User)
        {
            var includingTax = _workContext.TaxDisplayType == TaxDisplayType.IncludingTax;
            return GetShippingPrice(price, includingTax, User);
        }

        /// <summary>
        /// Gets shipping price
        /// </summary>
        /// <param name="price">Price</param>
        /// <param name="includingTax">A value indicating whether calculated price should include tax</param>
        /// <param name="User">User</param>
        /// <returns>Price</returns>
        public virtual decimal GetShippingPrice(decimal price, bool includingTax, User User)
        {
            return GetShippingPrice(price, includingTax, User, out var _);
        }

        /// <summary>
        /// Gets shipping price
        /// </summary>
        /// <param name="price">Price</param>
        /// <param name="includingTax">A value indicating whether calculated price should include tax</param>
        /// <param name="User">User</param>
        /// <param name="taxRate">Tax rate</param>
        /// <returns>Price</returns>
        public virtual decimal GetShippingPrice(decimal price, bool includingTax, User User, out decimal taxRate)
        {
            taxRate = decimal.Zero;

            if (!_taxSettings.ShippingIsTaxable)
            {
                return price;
            }

            var taxClassId = _taxSettings.ShippingTaxClassId;
            var priceIncludesTax = _taxSettings.ShippingPriceIncludesTax;
            return GetProductPrice(null, taxClassId, price, includingTax, User,
                priceIncludesTax, out taxRate);
        }

        #endregion

        #region Payment additional fee

        /// <summary>
        /// Gets payment method additional handling fee
        /// </summary>
        /// <param name="price">Price</param>
        /// <param name="User">User</param>
        /// <returns>Price</returns>
        public virtual decimal GetPaymentMethodAdditionalFee(decimal price, User User)
        {
            var includingTax = _workContext.TaxDisplayType == TaxDisplayType.IncludingTax;
            return GetPaymentMethodAdditionalFee(price, includingTax, User);
        }

        /// <summary>
        /// Gets payment method additional handling fee
        /// </summary>
        /// <param name="price">Price</param>
        /// <param name="includingTax">A value indicating whether calculated price should include tax</param>
        /// <param name="User">User</param>
        /// <returns>Price</returns>
        public virtual decimal GetPaymentMethodAdditionalFee(decimal price, bool includingTax, User User)
        {
            return GetPaymentMethodAdditionalFee(price, includingTax, User, out var _);
        }

        /// <summary>
        /// Gets payment method additional handling fee
        /// </summary>
        /// <param name="price">Price</param>
        /// <param name="includingTax">A value indicating whether calculated price should include tax</param>
        /// <param name="User">User</param>
        /// <param name="taxRate">Tax rate</param>
        /// <returns>Price</returns>
        public virtual decimal GetPaymentMethodAdditionalFee(decimal price, bool includingTax, User User, out decimal taxRate)
        {
            taxRate = decimal.Zero;

            if (!_taxSettings.PaymentMethodAdditionalFeeIsTaxable)
            {
                return price;
            }

            var taxClassId = _taxSettings.PaymentMethodAdditionalFeeTaxClassId;
            var priceIncludesTax = _taxSettings.PaymentMethodAdditionalFeeIncludesTax;
            return GetProductPrice(null, taxClassId, price, includingTax, User,
                priceIncludesTax, out taxRate);
        }

        #endregion

        #region Checkout attribute price

        /// <summary>
        /// Gets checkout attribute value price
        /// </summary>
        /// <param name="ca">Checkout attribute</param>
        /// <param name="cav">Checkout attribute value</param>
        /// <returns>Price</returns>
        public virtual decimal GetCheckoutAttributePrice(CheckoutAttribute ca, CheckoutAttributeValue cav)
        {
            var User = _workContext.CurrentUser;
            return GetCheckoutAttributePrice(ca, cav, User);
        }

        /// <summary>
        /// Gets checkout attribute value price
        /// </summary>
        /// <param name="ca">Checkout attribute</param>
        /// <param name="cav">Checkout attribute value</param>
        /// <param name="User">User</param>
        /// <returns>Price</returns>
        public virtual decimal GetCheckoutAttributePrice(CheckoutAttribute ca, CheckoutAttributeValue cav, User User)
        {
            var includingTax = _workContext.TaxDisplayType == TaxDisplayType.IncludingTax;
            return GetCheckoutAttributePrice(ca, cav, includingTax, User);
        }

        /// <summary>
        /// Gets checkout attribute value price
        /// </summary>
        /// <param name="ca">Checkout attribute</param>
        /// <param name="cav">Checkout attribute value</param>
        /// <param name="includingTax">A value indicating whether calculated price should include tax</param>
        /// <param name="User">User</param>
        /// <returns>Price</returns>
        public virtual decimal GetCheckoutAttributePrice(CheckoutAttribute ca, CheckoutAttributeValue cav,
            bool includingTax, User User)
        {
            return GetCheckoutAttributePrice(ca, cav, includingTax, User, out var _);
        }
        
        /// <summary>
        /// Gets checkout attribute value price
        /// </summary>
        /// <param name="ca">Checkout attribute</param>
        /// <param name="cav">Checkout attribute value</param>
        /// <param name="includingTax">A value indicating whether calculated price should include tax</param>
        /// <param name="User">User</param>
        /// <param name="taxRate">Tax rate</param>
        /// <returns>Price</returns>
        public virtual decimal GetCheckoutAttributePrice(CheckoutAttribute ca, CheckoutAttributeValue cav,
            bool includingTax, User User, out decimal taxRate)
        {
            if (cav == null)
                throw new ArgumentNullException(nameof(cav));

            taxRate = decimal.Zero;

            var price = cav.PriceAdjustment;
            if (ca.IsTaxExempt)
            {
                return price;
            }

            var priceIncludesTax = _taxSettings.PricesIncludeTax;
            var taxClassId = ca.TaxCategoryId;
            return GetProductPrice(null, taxClassId, price, includingTax, User,
                priceIncludesTax, out taxRate);
        }

        #endregion

        #region VAT

        /// <summary>
        /// Gets VAT Number status
        /// </summary>
        /// <param name="fullVatNumber">Two letter ISO code of a country and VAT number (e.g. GB 111 1111 111)</param>
        /// <returns>VAT Number status</returns>
        public virtual VatNumberStatus GetVatNumberStatus(string fullVatNumber)
        {
            return GetVatNumberStatus(fullVatNumber, out var _, out var _);
        }

        /// <summary>
        /// Gets VAT Number status
        /// </summary>
        /// <param name="fullVatNumber">Two letter ISO code of a country and VAT number (e.g. GB 111 1111 111)</param>
        /// <param name="name">Name (if received)</param>
        /// <param name="address">Address (if received)</param>
        /// <returns>VAT Number status</returns>
        public virtual VatNumberStatus GetVatNumberStatus(string fullVatNumber,
            out string name, out string address)
        {
            name = string.Empty;
            address = string.Empty;

            if (string.IsNullOrWhiteSpace(fullVatNumber))
                return VatNumberStatus.Empty;
            fullVatNumber = fullVatNumber.Trim();

            //GB 111 1111 111 or GB 1111111111
            //more advanced regex - http://codeigniter.com/wiki/European_Vat_Checker
            var r = new Regex(@"^(\w{2})(.*)");
            var match = r.Match(fullVatNumber);
            if (!match.Success)
                return VatNumberStatus.Invalid;
            var twoLetterIsoCode = match.Groups[1].Value;
            var vatNumber = match.Groups[2].Value;

            return GetVatNumberStatus(twoLetterIsoCode, vatNumber, out name, out address);
        }

        /// <summary>
        /// Gets VAT Number status
        /// </summary>
        /// <param name="twoLetterIsoCode">Two letter ISO code of a country</param>
        /// <param name="vatNumber">VAT number</param>
        /// <returns>VAT Number status</returns>
        public virtual VatNumberStatus GetVatNumberStatus(string twoLetterIsoCode, string vatNumber)
        {
            return GetVatNumberStatus(twoLetterIsoCode, vatNumber, out var _, out var _);
        }

        /// <summary>
        /// Gets VAT Number status
        /// </summary>
        /// <param name="twoLetterIsoCode">Two letter ISO code of a country</param>
        /// <param name="vatNumber">VAT number</param>
        /// <param name="name">Name (if received)</param>
        /// <param name="address">Address (if received)</param>
        /// <returns>VAT Number status</returns>
        public virtual VatNumberStatus GetVatNumberStatus(string twoLetterIsoCode, string vatNumber,
            out string name, out string address)
        {
            name = string.Empty;
            address = string.Empty;

            if (string.IsNullOrEmpty(twoLetterIsoCode))
                return VatNumberStatus.Empty;

            if (string.IsNullOrEmpty(vatNumber))
                return VatNumberStatus.Empty;

            if (_taxSettings.EuVatAssumeValid)
                return VatNumberStatus.Valid;

            if (!_taxSettings.EuVatUseWebService)
                return VatNumberStatus.Unknown;

            return DoVatCheck(twoLetterIsoCode, vatNumber, out name, out address, out var _);
        }

        /// <summary>
        /// Performs a basic check of a VAT number for validity
        /// </summary>
        /// <param name="twoLetterIsoCode">Two letter ISO code of a country</param>
        /// <param name="vatNumber">VAT number</param>
        /// <param name="name">Company name</param>
        /// <param name="address">Address</param>
        /// <param name="exception">Exception</param>
        /// <returns>VAT number status</returns>
        public virtual VatNumberStatus DoVatCheck(string twoLetterIsoCode, string vatNumber,
            out string name, out string address, out Exception exception)
        {
            name = string.Empty;
            address = string.Empty;

            if (vatNumber == null)
                vatNumber = string.Empty;
            vatNumber = vatNumber.Trim().Replace(" ", string.Empty);

            if (twoLetterIsoCode == null)
                twoLetterIsoCode = string.Empty;
            if (!string.IsNullOrEmpty(twoLetterIsoCode))
                //The service returns INVALID_INPUT for country codes that are not uppercase.
                twoLetterIsoCode = twoLetterIsoCode.ToUpper();

            try
            {
                var s = new EuropaCheckVatService.checkVatPortTypeClient();
                var task = s.checkVatAsync(new EuropaCheckVatService.checkVatRequest
                {
                    vatNumber = vatNumber,
                    countryCode = twoLetterIsoCode
                });
                task.Wait();

                var result = task.Result;

                var valid = result.valid;
                name = result.name;
                address = result.address;

                exception = null;
                return valid ? VatNumberStatus.Valid : VatNumberStatus.Invalid;
            }
            catch (Exception ex)
            {
                name = address = string.Empty;
                exception = ex;
                return VatNumberStatus.Unknown;
            }
            finally
            {
                if (name == null)
                    name = string.Empty;

                if (address == null)
                    address = string.Empty;
            }
        }

        #endregion

        #region Exempts

        /// <summary>
        /// Gets a value indicating whether a product is tax exempt
        /// </summary>
        /// <param name="product">Product</param>
        /// <param name="User">User</param>
        /// <returns>A value indicating whether a product is tax exempt</returns>
        public virtual bool IsTaxExempt(Product product, User User)
        {
            if (User != null)
            {
               
                if (_UserService.GetUserRoles(User).Any(cr => cr.TaxExempt))
                    return true;
            }

            if (product == null)
            {
                return false;
            }

            if (product.IsTaxExempt)
            {
                return true;
            }

            return false;
        }

        /// <summary>
        /// Gets a value indicating whether EU VAT exempt (the European Union Value Added Tax)
        /// </summary>
        /// <param name="address">Address</param>
        /// <param name="User">User</param>
        /// <returns>Result</returns>
        public virtual bool IsVatExempt(Address address, User User)
        {
            if (!_taxSettings.EuVatEnabled)
                return false;

            if (User == null || address == null)
                return false;

            var country = _countryService.GetCountryById(address.CountryId ?? 0);
            if (country == null)
                return false;

            if (!country.SubjectToVat)
                // VAT not chargeable if shipping outside VAT zone
                return true;

            // VAT not chargeable if address, User and config meet our VAT exemption requirements:
            // returns true if this User is VAT exempt because they are shipping within the EU but outside our shop country, they have supplied a validated VAT number, and the shop is configured to allow VAT exemption
            var UserVatStatus = (VatNumberStatus)_genericAttributeService.GetAttribute<int>(User, TvProgUserDefaults.VatNumberStatusIdAttribute);
            return country.Id != _taxSettings.EuVatShopCountryId &&
                   UserVatStatus == VatNumberStatus.Valid &&
                   _taxSettings.EuVatAllowVatExemption;
        }

        #endregion

        #endregion
    }
}