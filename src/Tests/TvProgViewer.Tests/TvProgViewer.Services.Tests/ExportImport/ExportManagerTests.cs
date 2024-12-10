using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using ClosedXML.Excel;
using FluentAssertions;
using TvProgViewer.Core;
using TvProgViewer.Core.Domain.Catalog;
using TvProgViewer.Core.Domain.Users;
using TvProgViewer.Core.Domain.Directory;
using TvProgViewer.Core.Domain.Localization;
using TvProgViewer.Core.Domain.Orders;
using TvProgViewer.Core.Domain.Payments;
using TvProgViewer.Core.Domain.Shipping;
using TvProgViewer.Core.Domain.Tax;
using TvProgViewer.Core.Domain.Vendors;
using TvProgViewer.Data;
using TvProgViewer.Services;
using TvProgViewer.Services.Catalog;
using TvProgViewer.Services.Common;
using TvProgViewer.Services.Users;
using TvProgViewer.Services.Directory;
using TvProgViewer.Services.ExportImport;
using TvProgViewer.Services.ExportImport.Help;
using TvProgViewer.Services.Localization;
using TvProgViewer.Services.Orders;
using TvProgViewer.Services.Shipping.Date;
using TvProgViewer.Services.Tax;
using TvProgViewer.Services.Vendors;
using NUnit.Framework;

namespace TvProgViewer.Tests.TvProgViewer.Services.Tests.ExportImport
{
    [TestFixture]
    public class ExportManagerTests : ServiceTest
    {
        #region Fields

        private CatalogSettings _catalogSettings;
        private IAddressService _addressService;
        private ICategoryService _categoryService;
        private ICountryService _countryService;
        private IUserService _userService;
        private IDateRangeService _dateRangeService;
        private IExportManager _exportManager;
        private ILanguageService _languageService;
        private IManufacturerService _manufacturerService;
        private IMeasureService _measureService;
        private IOrderService _orderService;
        private ITvChannelTemplateService _tvChannelTemplateService;
        private IRepository<TvChannel> _tvChannelRepository;
        private ITaxCategoryService _taxCategoryService;
        private IVendorService _vendorService;

        #endregion

        #region Setup

        [OneTimeSetUp]
        public async Task SetUp()
        {
            _catalogSettings = GetService<CatalogSettings>();
            _addressService = GetService<IAddressService>();
            _categoryService = GetService<ICategoryService>();
            _countryService = GetService<ICountryService>();
            _userService = GetService<IUserService>();
            _dateRangeService = GetService<IDateRangeService>();
            _exportManager = GetService<IExportManager>();
            _languageService = GetService<ILanguageService>();
            _manufacturerService = GetService<IManufacturerService>();
            _measureService = GetService<IMeasureService>();
            _orderService = GetService<IOrderService>();
            _tvChannelTemplateService = GetService<ITvChannelTemplateService>();
            _tvChannelRepository = GetService<IRepository<TvChannel>>();
            _taxCategoryService = GetService<ITaxCategoryService>();
            _vendorService = GetService<IVendorService>();

            await GetService<IGenericAttributeService>()
                .SaveAttributeAsync(await _userService.GetUserByEmailAsync(TvProgTestsDefaults.AdminEmail), "category-advanced-mode",
                    true);
            await GetService<IGenericAttributeService>()
                .SaveAttributeAsync(await _userService.GetUserByEmailAsync(TvProgTestsDefaults.AdminEmail), "manufacturer-advanced-mode",
                    true);
            await GetService<IGenericAttributeService>()
                .SaveAttributeAsync(await _userService.GetUserByEmailAsync(TvProgTestsDefaults.AdminEmail), "tvchannel-advanced-mode",
                    true);
        }

        [OneTimeTearDown]
        public async Task TearDown()
        {
            await GetService<IGenericAttributeService>()
                .SaveAttributeAsync(await _userService.GetUserByEmailAsync(TvProgTestsDefaults.AdminEmail), "category-advanced-mode",
                    false);
            await GetService<IGenericAttributeService>()
                .SaveAttributeAsync(await _userService.GetUserByEmailAsync(TvProgTestsDefaults.AdminEmail), "manufacturer-advanced-mode",
                    false);
            await GetService<IGenericAttributeService>()
                .SaveAttributeAsync(await _userService.GetUserByEmailAsync(TvProgTestsDefaults.AdminEmail), "tvchannel-advanced-mode",
                    false);
        }

        #endregion

        #region Utilities

        protected static T PropertiesShouldEqual<T, L, Tp>(T actual, PropertyManager<Tp, L> manager, IDictionary<string, string> replacePairs, params string[] filter) where L : Language
        {
            var objectProperties = typeof(T).GetProperties();
            foreach (var property in manager.GetDefaultProperties)
            {
                if (filter.Contains(property.PropertyName))
                    continue;

                var objectProperty = replacePairs.ContainsKey(property.PropertyName)
                    ? objectProperties.FirstOrDefault(p => p.Name == replacePairs[property.PropertyName])
                    : objectProperties.FirstOrDefault(p => p.Name == property.PropertyName);

                if (objectProperty == null)
                    continue;

                var objectPropertyValue = objectProperty.GetValue(actual);

                if (objectProperty.PropertyType == typeof(Guid))
                    objectPropertyValue = objectPropertyValue.ToString();

                if (objectProperty.PropertyType == typeof(string))
                    objectPropertyValue = (property.PropertyValue?.ToString() == string.Empty && objectPropertyValue == null) ? string.Empty : objectPropertyValue;

                if (objectProperty.PropertyType.IsEnum)
                    objectPropertyValue = (int)objectPropertyValue;

                //https://github.com/ClosedXML/ClosedXML/blob/develop/ClosedXML/Extensions/ObjectExtensions.cs#L61
                if (objectProperty.PropertyType == typeof(DateTime))
                    objectPropertyValue = DateTime.FromOADate(double.Parse(((DateTime)objectPropertyValue).ToOADate().ToString("G15", CultureInfo.InvariantCulture), CultureInfo.InvariantCulture));

                if (objectProperty.PropertyType == typeof(DateTime?))
                    objectPropertyValue = objectPropertyValue != null ? DateTime.FromOADate(double.Parse(((DateTime?)objectPropertyValue)?.ToOADate().ToString("G15", CultureInfo.InvariantCulture))) : null;

                //https://github.com/ClosedXML/ClosedXML/issues/544
                property.PropertyValue.Should().Be(objectPropertyValue ?? "", $"The property \"{typeof(T).Name}.{property.PropertyName}\" of these objects is not equal");
            }

            return actual;
        }

        protected async Task<PropertyManager<T, Language>> GetPropertyManagerAsync<T>(XLWorkbook workbook)
        {
            var languages = await _languageService.GetAllLanguagesAsync();

            //the columns
            var metadata = ImportManager.GetWorkbookMetadata<T>(workbook, languages);
            var defaultProperties = metadata.DefaultProperties;
            var localizedProperties = metadata.LocalizedProperties;

            return new PropertyManager<T, Language>(defaultProperties, _catalogSettings, localizedProperties);
        }

        protected XLWorkbook GetWorkbook(byte[] excelData)
        {
            var stream = new MemoryStream(excelData);
            return new XLWorkbook(stream);
        }

        protected T AreAllObjectPropertiesPresent<T, L>(T obj, PropertyManager<T, L> manager, params string[] filters) where L : Language
        {
            foreach (var propertyInfo in typeof(T).GetProperties())
            {
                if (filters.Contains(propertyInfo.Name))
                    continue;

                if (manager.GetDefaultProperties.Any(p => p.PropertyName == propertyInfo.Name))
                    continue;

                Assert.Fail("The property \"{0}.{1}\" no present on excel file", typeof(T).Name, propertyInfo.Name);
            }

            return obj;
        }

        #endregion

        #region Test export to excel

        [Test]
        public async Task CanExportOrdersXlsx()
        {
            var orders = await _orderService.SearchOrdersAsync();

            var excelData = await _exportManager.ExportOrdersToXlsxAsync(orders);
            var workbook = GetWorkbook(excelData);
            var manager = await GetPropertyManagerAsync<Order>(workbook);

            // get the first worksheet in the workbook
            var worksheet = workbook.Worksheets.FirstOrDefault();
            if (worksheet == null)
                throw new TvProgException("No worksheet found");

            manager.ReadDefaultFromXlsx(worksheet, 2);

            var replacePairs = new Dictionary<string, string>
                {
                    { "OrderId", "Id" },
                    { "OrderStatus", "OrderStatusId" },
                    { "PaymentStatus", "PaymentStatusId" },
                    { "ShippingStatus", "ShippingStatusId" },
                    { "ShippingPickupInStore", "PickupInStore" }
                };

            var order = orders.First();

            var ignore = new List<string>();
            ignore.AddRange(replacePairs.Values);

            //not exported fields
            ignore.AddRange(new[]
            {
                "BillingAddressId", "ShippingAddressId", "PickupAddressId", "UserTaxDisplayTypeId",
                "RewardPointsHistoryEntryId", "CheckoutAttributeDescription", "CheckoutAttributesXml",
                "UserLanguageId", "UserIp", "AllowStoringCreditCardNumber", "CardType", "CardName",
                "CardNumber", "MaskedCreditCardNumber", "CardCvv2", "CardExpirationMonth", "CardExpirationYear",
                "AuthorizationTransactionId", "AuthorizationTransactionCode", "AuthorizationTransactionResult",
                "CaptureTransactionId", "CaptureTransactionResult", "SubscriptionTransactionId", "PaidDateUtc",
                "Deleted", "PickupAddress", "RedeemedRewardPointsEntryId", "DiscountUsageHistory", "GiftCardUsageHistory",
                "OrderNotes", "OrderItems", "Shipments", "OrderStatus", "PaymentStatus", "ShippingStatus",
                "UserTaxDisplayType", "CustomOrderNumber"
            });

            //fields tested individually
            ignore.AddRange(new[]
            {
               "User", "BillingAddressId", "ShippingAddressId", "EntityCacheKey"
            });

            manager.SetSelectList("OrderStatus", await OrderStatus.Pending.ToSelectListAsync(useLocalization: false));
            manager.SetSelectList("PaymentStatus", await PaymentStatus.Pending.ToSelectListAsync(useLocalization: false));
            manager.SetSelectList("ShippingStatus", await ShippingStatus.ShippingNotRequired.ToSelectListAsync(useLocalization: false));
            
            AreAllObjectPropertiesPresent(order, manager, ignore.ToArray());
            PropertiesShouldEqual(order, manager, replacePairs);

            var addressFields = new List<string>
            {
                "FirstName",
                "LastName",
                "Email",
                "Company",
                "Country",
                "StateProvince",
                "City",
                "County",
                "Address1",
                "Address2",
                "ZipPostalCode",
                "PhoneNumber",
                "FaxNumber"
            };

            const string billingPattern = "Billing";
            replacePairs = addressFields.ToDictionary(p => billingPattern + p, p => p);

            var testBillingAddress = await _addressService.GetAddressByIdAsync(order.BillingAddressId);

            PropertiesShouldEqual(testBillingAddress, manager, replacePairs, "CreatedOnUtc", "BillingCountry");

            var country = await _countryService.GetCountryByAddressAsync(testBillingAddress);
            manager.GetDefaultProperties.First(p => p.PropertyName == "BillingCountry").PropertyValue.Should().Be(country.Name);

            const string shippingPattern = "Shipping";
            replacePairs = addressFields.ToDictionary(p => shippingPattern + p, p => p);
            var testShippingAddress = await _addressService.GetAddressByIdAsync(order.ShippingAddressId ?? 0);
            PropertiesShouldEqual(testShippingAddress, manager, replacePairs, "CreatedOnUtc", "ShippingCountry");
            country = await _countryService.GetCountryByAddressAsync(testShippingAddress);
            manager.GetDefaultProperties.First(p => p.PropertyName == "ShippingCountry").PropertyValue.Should().Be(country.Name);
        }

        [Test]
        public async Task CanExportManufacturersXlsx()
        {
            var manufacturers = await _manufacturerService.GetAllManufacturersAsync();

            var excelData = await _exportManager.ExportManufacturersToXlsxAsync(manufacturers);
            var workbook = GetWorkbook(excelData);
            var manager = await GetPropertyManagerAsync<Manufacturer>(workbook);

            // get the first worksheet in the workbook
            var worksheet = workbook.Worksheets.FirstOrDefault();
            if (worksheet == null)
                throw new TvProgException("No worksheet found");

            manager.ReadDefaultFromXlsx(worksheet, 2);

            var manufacturer = manufacturers.First();

            var ignore = new List<string> { "Picture", "EntityCacheKey", "PictureId", "SubjectToAcl", "LimitedToStores", "Deleted", "CreatedOnUtc", "UpdatedOnUtc", "AppliedDiscounts", "DiscountManufacturerMappings" };

            AreAllObjectPropertiesPresent(manufacturer, manager, ignore.ToArray());
            PropertiesShouldEqual(manufacturer, manager, new Dictionary<string, string>());

            manager.GetDefaultProperties.First(p => p.PropertyName == "Picture").PropertyValue.Should().NotBeNull();
        }

        [Test]
        public async Task CanExportUsersToXlsx()
        {
            var replacePairs = new Dictionary<string, string>
            {
                { "VatNumberStatus", "VatNumberStatusId" }
            };

            var users = await _userService.GetAllUsersAsync();

            var excelData = await _exportManager.ExportUsersToXlsxAsync(users);
            var workbook = GetWorkbook(excelData);
            var manager = await GetPropertyManagerAsync<User>(workbook);

            // get the first worksheet in the workbook
            var worksheet = workbook.Worksheets.FirstOrDefault();
            if (worksheet == null)
                throw new TvProgException("No worksheet found");

            manager.ReadDefaultFromXlsx(worksheet, 2);
            manager.SetSelectList("VatNumberStatus", await VatNumberStatus.Unknown.ToSelectListAsync(useLocalization: false));

            var user = users.First();

            var ignore = new List<string> { "Id", "ExternalAuthenticationRecords", "UserRoles", "ShoppingCartItems",
                "ReturnRequests", "BillingAddress", "ShippingAddress", "Addresses", "AdminComment",
                "EmailToRevalidate", "HasShoppingCartItems", "RequireReLogin", "FailedLoginAttempts",
                "CannotLoginUntilDateUtc", "Deleted", "IsSystemAccount", "SystemName", "LastIpAddress",
                "LastLoginDateUtc", "LastActivityDateUtc", "RegisteredInStoreId", "BillingAddressId", "ShippingAddressId",
                "UserUserRoleMappings", "UserAddressMappings", "EntityCacheKey", "VendorId",
                "DateOfBirth", "StreetAddress", "StreetAddress2", "ZipPostalCode", "City", "County", "CountryId",
                "StateProvinceId", "Phone", "Fax", "VatNumberStatusId", "GmtZone", "CustomUserAttributesXML",
                "CurrencyId", "LanguageId", "TaxDisplayTypeId", "TaxDisplayType", "TaxDisplayType", "VatNumberStatusId" };

            AreAllObjectPropertiesPresent(user, manager, ignore.ToArray());
            PropertiesShouldEqual(user, manager, replacePairs);
        }

        [Test]
        public async Task CanExportCategoriesToXlsx()
        {
            var categories = await _categoryService.GetAllCategoriesAsync();

            var excelData = await _exportManager.ExportCategoriesToXlsxAsync(categories);
            var workbook = GetWorkbook(excelData);
            var manager = await GetPropertyManagerAsync<Category>(workbook);

            // get the first worksheet in the workbook
            var worksheet = workbook.Worksheets.FirstOrDefault();
            if (worksheet == null)
                throw new TvProgException("No worksheet found");

            manager.ReadDefaultFromXlsx(worksheet, 2);
            var category = categories.First();

            var ignore = new List<string> { "CreatedOnUtc", "EntityCacheKey", "Picture", "PictureId", "AppliedDiscounts", "UpdatedOnUtc", "SubjectToAcl", "LimitedToStores", "Deleted", "DiscountCategoryMappings" };

            AreAllObjectPropertiesPresent(category, manager, ignore.ToArray());
            PropertiesShouldEqual(category, manager, new Dictionary<string, string>());

            manager.GetDefaultProperties.First(p => p.PropertyName == "Picture").PropertyValue.Should().NotBeNull();
        }

        [Test]
        public async Task CanExportTvChannelsToXlsx()
        {
            var replacePairs = new Dictionary<string, string>
            {
                { "TvChannelId", "Id" },
                { "TvChannelType", "TvChannelTypeId" },
                { "GiftCardType", "GiftCardTypeId" },
                { "Vendor", "VendorId" },
                { "TvChannelTemplate", "TvChannelTemplateId" },
                { "DeliveryDate", "DeliveryDateId" },
                { "TaxCategory", "TaxCategoryId" },
                { "ManageInventoryMethod", "ManageInventoryMethodId" },
                { "TvChannelAvailabilityRange", "TvChannelAvailabilityRangeId" },
                { "LowStockActivity", "LowStockActivityId" },
                { "BackorderMode", "BackorderModeId" },
                { "BasepriceUnit", "BasepriceUnitId" },
                { "BasepriceBaseUnit", "BasepriceBaseUnitId" },
                { "SKU", "Sku" },
                { "DownloadActivationType", "DownloadActivationTypeId" },
                { "RecurringCyclePeriod", "RecurringCyclePeriodId" },
                { "RentalPricePeriod", "RentalPricePeriodId" },
                { "TvChannelLiveUrl", "TvChannelLiveUrl" }
            };

            var ignore = new List<string> { "Categories", "Manufacturers", "AdminComment",
                "TvChannelType", "BackorderMode", "DownloadActivationType", "GiftCardType", "LowStockActivity",
                "ManageInventoryMethod", "RecurringCyclePeriod", "RentalPricePeriod", "TvChannelCategories",
                "TvChannelManufacturers", "TvChannelPictures", "TvChannelReviews", "TvChannelSpecificationAttributes",
                "TvChannelTags", "TvChannelAttributeMappings", "TvChannelAttributeCombinations", "TierPrices",
                "AppliedDiscounts", "TvChannelWarehouseInventory", "ApprovedRatingSum", "NotApprovedRatingSum",
                "ApprovedTotalReviews", "NotApprovedTotalReviews", "SubjectToAcl", "LimitedToStores", "Deleted",
                "DownloadExpirationDays", "HasTierPrices", "HasDiscountsApplied", "AvailableStartDateTimeUtc",
                "AvailableEndDateTimeUtc", "DisplayOrder", "CreatedOnUtc", "UpdatedOnUtc", "TvChannelTvChannelTagMappings",
                "DiscountTvChannelMappings", "EntityCacheKey" };

            ignore.AddRange(replacePairs.Values);

            var tvChannel = _tvChannelRepository.Table.ToList().First();

            var excelData = await _exportManager.ExportTvChannelsToXlsxAsync(new[] {tvChannel});
            var workbook = GetWorkbook(excelData);
            var manager = await GetPropertyManagerAsync<TvChannel>(workbook);

            // get the first worksheet in the workbook
            var worksheet = workbook.Worksheets.FirstOrDefault();
            if (worksheet == null)
                throw new TvProgException("No worksheet found");

            manager.SetSelectList("TvChannelType", await TvChannelType.SimpleTvChannel.ToSelectListAsync(useLocalization: false));
            manager.SetSelectList("GiftCardType", await GiftCardType.Virtual.ToSelectListAsync(useLocalization: false));
            manager.SetSelectList("DownloadActivationType", await DownloadActivationType.Manually.ToSelectListAsync(useLocalization: false));
            manager.SetSelectList("ManageInventoryMethod", await ManageInventoryMethod.DontManageStock.ToSelectListAsync(useLocalization: false));
            manager.SetSelectList("LowStockActivity", await LowStockActivity.Nothing.ToSelectListAsync(useLocalization: false));
            manager.SetSelectList("BackorderMode", await BackorderMode.NoBackorders.ToSelectListAsync(useLocalization: false));
            manager.SetSelectList("RecurringCyclePeriod", await RecurringTvChannelCyclePeriod.Days.ToSelectListAsync(useLocalization: false));
            manager.SetSelectList("RentalPricePeriod", await RentalPricePeriod.Days.ToSelectListAsync(useLocalization: false));

            var vendors = await _vendorService.GetAllVendorsAsync(showHidden: true);
            manager.SetSelectList("Vendor", vendors.Select(v => v as BaseEntity).ToSelectList(p => (p as Vendor)?.Name ?? string.Empty));
            var templates = await _tvChannelTemplateService.GetAllTvChannelTemplatesAsync();
            manager.SetSelectList("TvChannelTemplate", templates.Select(pt => pt as BaseEntity).ToSelectList(p => (p as TvChannelTemplate)?.Name ?? string.Empty));
            var dates = await _dateRangeService.GetAllDeliveryDatesAsync();
            manager.SetSelectList("DeliveryDate", dates.Select(dd => dd as BaseEntity).ToSelectList(p => (p as DeliveryDate)?.Name ?? string.Empty));
            var availabilityRanges = await _dateRangeService.GetAllTvChannelAvailabilityRangesAsync();
            manager.SetSelectList("TvChannelAvailabilityRange", availabilityRanges.Select(range => range as BaseEntity).ToSelectList(p => (p as TvChannelAvailabilityRange)?.Name ?? string.Empty));
            var categories = await _taxCategoryService.GetAllTaxCategoriesAsync();
            manager.SetSelectList("TaxCategory", categories.Select(tc => tc as BaseEntity).ToSelectList(p => (p as TaxCategory)?.Name ?? string.Empty));
            var measureWeights = await _measureService.GetAllMeasureWeightsAsync();
            manager.SetSelectList("BasepriceUnit", measureWeights.Select(mw => mw as BaseEntity).ToSelectList(p => (p as MeasureWeight)?.Name ?? string.Empty));
            manager.SetSelectList("BasepriceBaseUnit", measureWeights.Select(mw => mw as BaseEntity).ToSelectList(p => (p as MeasureWeight)?.Name ?? string.Empty));

            manager.Remove("TvChannelTags");

            manager.ReadDefaultFromXlsx(worksheet, 2);

            AreAllObjectPropertiesPresent(tvChannel, manager, ignore.ToArray());
            PropertiesShouldEqual(tvChannel, manager, replacePairs);
        }

        #endregion
    }
}
