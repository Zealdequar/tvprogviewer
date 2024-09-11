using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Rendering;
using TvProgViewer.Core.Caching;
using TvProgViewer.Core.Domain.Catalog;
using TvProgViewer.Core.Domain.Discounts;
using TvProgViewer.Core.Domain.Gdpr;
using TvProgViewer.Core.Domain.Logging;
using TvProgViewer.Core.Domain.Orders;
using TvProgViewer.Core.Domain.Payments;
using TvProgViewer.Core.Domain.Shipping;
using TvProgViewer.Core.Domain.Tax;
using TvProgViewer.Services;
using TvProgViewer.Services.Catalog;
using TvProgViewer.Services.Users;
using TvProgViewer.Services.Directory;
using TvProgViewer.Services.Helpers;
using TvProgViewer.Services.Localization;
using TvProgViewer.Services.Logging;
using TvProgViewer.Services.Messages;
using TvProgViewer.Services.Plugins;
using TvProgViewer.Services.Shipping;
using TvProgViewer.Services.Shipping.Date;
using TvProgViewer.Services.Stores;
using TvProgViewer.Services.Tax;
using TvProgViewer.Services.Topics;
using TvProgViewer.Services.Vendors;
using TvProgViewer.WebUI.Areas.Admin.Infrastructure.Cache;

namespace TvProgViewer.WebUI.Areas.Admin.Factories
{
    /// <summary>
    /// Represents the implementation of the base model factory that implements a most common admin model factories methods
    /// </summary>
    public partial class BaseAdminModelFactory : IBaseAdminModelFactory
    {
        #region Fields

        private readonly ICategoryService _categoryService;
        private readonly ICategoryTemplateService _categoryTemplateService;
        private readonly ICountryService _countryService;
        private readonly ICurrencyService _currencyService;
        private readonly IUserActivityService _userActivityService;
        private readonly IUserService _userService;
        private readonly IDateRangeService _dateRangeService;
        private readonly IDateTimeHelper _dateTimeHelper;
        private readonly IEmailAccountService _emailAccountService;
        private readonly ILanguageService _languageService;
        private readonly ILocalizationService _localizationService;
        private readonly IManufacturerService _manufacturerService;
        private readonly IManufacturerTemplateService _manufacturerTemplateService;
        private readonly IPluginService _pluginService;
        private readonly ITvChannelTemplateService _tvChannelTemplateService;
        private readonly ISpecificationAttributeService _specificationAttributeService;
        private readonly IShippingService _shippingService;
        private readonly IStateProvinceService _stateProvinceService;
        private readonly IStaticCacheManager _staticCacheManager;
        private readonly IStoreService _storeService;
        private readonly ITaxCategoryService _taxCategoryService;
        private readonly ITopicTemplateService _topicTemplateService;
        private readonly IVendorService _vendorService;

        #endregion

        #region Ctor

        public BaseAdminModelFactory(ICategoryService categoryService,
            ICategoryTemplateService categoryTemplateService,
            ICountryService countryService,
            ICurrencyService currencyService,
            IUserActivityService userActivityService,
            IUserService userService,
            IDateRangeService dateRangeService,
            IDateTimeHelper dateTimeHelper,
            IEmailAccountService emailAccountService,
            ILanguageService languageService,
            ILocalizationService localizationService,
            IManufacturerService manufacturerService,
            IManufacturerTemplateService manufacturerTemplateService,
            IPluginService pluginService,
            ITvChannelTemplateService tvChannelTemplateService,
            ISpecificationAttributeService specificationAttributeService,
            IShippingService shippingService,
            IStateProvinceService stateProvinceService,
            IStaticCacheManager staticCacheManager,
            IStoreService storeService,
            ITaxCategoryService taxCategoryService,
            ITopicTemplateService topicTemplateService,
            IVendorService vendorService)
        {
            _categoryService = categoryService;
            _categoryTemplateService = categoryTemplateService;
            _countryService = countryService;
            _currencyService = currencyService;
            _userActivityService = userActivityService;
            _userService = userService;
            _dateRangeService = dateRangeService;
            _dateTimeHelper = dateTimeHelper;
            _emailAccountService = emailAccountService;
            _languageService = languageService;
            _localizationService = localizationService;
            _manufacturerService = manufacturerService;
            _manufacturerTemplateService = manufacturerTemplateService;
            _pluginService = pluginService;
            _tvChannelTemplateService = tvChannelTemplateService;
            _specificationAttributeService = specificationAttributeService;
            _shippingService = shippingService;
            _stateProvinceService = stateProvinceService;
            _staticCacheManager = staticCacheManager;
            _storeService = storeService;
            _taxCategoryService = taxCategoryService;
            _topicTemplateService = topicTemplateService;
            _vendorService = vendorService;
        }

        #endregion

        #region Utilities

        /// <summary>
        /// Prepare default item
        /// </summary>
        /// <param name="items">Available items</param>
        /// <param name="withSpecialDefaultItem">Whether to insert the first special item for the default value</param>
        /// <param name="defaultItemText">Default item text; pass null to use "All" text</param>
        /// <param name="defaultItemValue">Default item value; defaults 0</param>
        /// <returns>Задача представляет асинхронную операцию</returns>
        protected virtual async Task PrepareDefaultItemAsync(IList<SelectListItem> items, bool withSpecialDefaultItem, string defaultItemText = null, string defaultItemValue = "0")
        {
            if (items == null)
                throw new ArgumentNullException(nameof(items));

            //whether to insert the first special item for the default value
            if (!withSpecialDefaultItem)
                return;

            //prepare item text
            defaultItemText ??= await _localizationService.GetResourceAsync("Admin.Common.All");

            //insert this default item at first
            items.Insert(0, new SelectListItem { Text = defaultItemText, Value = defaultItemValue });
        }

        /// <summary>
        /// Get category list
        /// </summary>
        /// <returns>
        /// Задача представляет асинхронную операцию
        /// The task result contains the category list
        /// </returns>
        protected virtual async Task<List<SelectListItem>> GetCategoryListAsync()
        {
            var listItems = await _staticCacheManager.GetAsync(TvProgModelCacheDefaults.CategoriesListKey, async () =>
            {
                var categories = await _categoryService.GetAllCategoriesAsync(showHidden: true);
                return await categories.SelectAwait(async c => new SelectListItem
                {
                    Text = await _categoryService.GetFormattedBreadCrumbAsync(c, categories),
                    Value = c.Id.ToString()
                }).ToListAsync();
            });

            var result = new List<SelectListItem>();
            //clone the list to ensure that "selected" property is not set
            foreach (var item in listItems)
            {
                result.Add(new SelectListItem
                {
                    Text = item.Text,
                    Value = item.Value
                });
            }

            return result;
        }

        /// <summary>
        /// Get manufacturer list
        /// </summary>
        /// <returns>
        /// Задача представляет асинхронную операцию
        /// The task result contains the manufacturer list
        /// </returns>
        protected virtual async Task<List<SelectListItem>> GetManufacturerListAsync()
        {
            var listItems = await _staticCacheManager.GetAsync(TvProgModelCacheDefaults.ManufacturersListKey, async () =>
            {
                var manufacturers = await _manufacturerService.GetAllManufacturersAsync(showHidden: true);
                return manufacturers.Select(m => new SelectListItem
                {
                    Text = m.Name,
                    Value = m.Id.ToString()
                });
            });

            var result = new List<SelectListItem>();
            //clone the list to ensure that "selected" property is not set
            foreach (var item in listItems)
            {
                result.Add(new SelectListItem
                {
                    Text = item.Text,
                    Value = item.Value
                });
            }

            return result;
        }

        /// <summary>
        /// Get vendor list
        /// </summary>
        /// <returns>
        /// Задача представляет асинхронную операцию
        /// The task result contains the vendor list
        /// </returns>
        protected virtual async Task<List<SelectListItem>> GetVendorListAsync()
        {
            var listItems = await _staticCacheManager.GetAsync(TvProgModelCacheDefaults.VendorsListKey, async () =>
            {
                var vendors = await _vendorService.GetAllVendorsAsync(showHidden: true);
                return vendors.Select(v => new SelectListItem
                {
                    Text = v.Name,
                    Value = v.Id.ToString()
                });
            });

            var result = new List<SelectListItem>();
            //clone the list to ensure that "selected" property is not set
            foreach (var item in listItems)
            {
                result.Add(new SelectListItem
                {
                    Text = item.Text,
                    Value = item.Value
                });
            }

            return result;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Prepare available activity log types
        /// </summary>
        /// <param name="items">Activity log type items</param>
        /// <param name="withSpecialDefaultItem">Whether to insert the first special item for the default value</param>
        /// <param name="defaultItemText">Default item text; pass null to use default value of the default item text</param>
        /// <returns>Задача представляет асинхронную операцию</returns>
        public virtual async Task PrepareActivityLogTypesAsync(IList<SelectListItem> items, bool withSpecialDefaultItem = true, string defaultItemText = null)
        {
            if (items == null)
                throw new ArgumentNullException(nameof(items));

            //prepare available activity log types
            var availableActivityTypes = await _userActivityService.GetAllActivityTypesAsync();
            foreach (var activityType in availableActivityTypes)
            {
                items.Add(new SelectListItem { Value = activityType.Id.ToString(), Text = activityType.Name });
            }

            //insert special item for the default value
            await PrepareDefaultItemAsync(items, withSpecialDefaultItem, defaultItemText);
        }

        /// <summary>
        /// Prepare available order statuses
        /// </summary>
        /// <param name="items">Order status items</param>
        /// <param name="withSpecialDefaultItem">Whether to insert the first special item for the default value</param>
        /// <param name="defaultItemText">Default item text; pass null to use default value of the default item text</param>
        /// <returns>Задача представляет асинхронную операцию</returns>
        public virtual async Task PrepareOrderStatusesAsync(IList<SelectListItem> items, bool withSpecialDefaultItem = true, string defaultItemText = null)
        {
            if (items == null)
                throw new ArgumentNullException(nameof(items));

            //prepare available order statuses
            var availableStatusItems = await OrderStatus.Pending.ToSelectListAsync(false);
            foreach (var statusItem in availableStatusItems)
            {
                items.Add(statusItem);
            }

            //insert special item for the default value
            await PrepareDefaultItemAsync(items, withSpecialDefaultItem, defaultItemText);
        }

        /// <summary>
        /// Prepare available payment statuses
        /// </summary>
        /// <param name="items">Payment status items</param>
        /// <param name="withSpecialDefaultItem">Whether to insert the first special item for the default value</param>
        /// <param name="defaultItemText">Default item text; pass null to use default value of the default item text</param>
        /// <returns>Задача представляет асинхронную операцию</returns>
        public virtual async Task PreparePaymentStatusesAsync(IList<SelectListItem> items, bool withSpecialDefaultItem = true, string defaultItemText = null)
        {
            if (items == null)
                throw new ArgumentNullException(nameof(items));

            //prepare available payment statuses
            var availableStatusItems = await PaymentStatus.Pending.ToSelectListAsync(false);
            foreach (var statusItem in availableStatusItems)
            {
                items.Add(statusItem);
            }

            //insert special item for the default value
            await PrepareDefaultItemAsync(items, withSpecialDefaultItem, defaultItemText);
        }

        /// <summary>
        /// Prepare available shipping statuses
        /// </summary>
        /// <param name="items">Shipping status items</param>
        /// <param name="withSpecialDefaultItem">Whether to insert the first special item for the default value</param>
        /// <param name="defaultItemText">Default item text; pass null to use default value of the default item text</param>
        /// <returns>Задача представляет асинхронную операцию</returns>
        public virtual async Task PrepareShippingStatusesAsync(IList<SelectListItem> items, bool withSpecialDefaultItem = true, string defaultItemText = null)
        {
            if (items == null)
                throw new ArgumentNullException(nameof(items));

            //prepare available shipping statuses
            var availableStatusItems = await ShippingStatus.NotYetShipped.ToSelectListAsync(false);
            foreach (var statusItem in availableStatusItems)
            {
                items.Add(statusItem);
            }

            //insert special item for the default value
            await PrepareDefaultItemAsync(items, withSpecialDefaultItem, defaultItemText);
        }

        /// <summary>
        /// Prepare available countries
        /// </summary>
        /// <param name="items">Country items</param>
        /// <param name="withSpecialDefaultItem">Whether to insert the first special item for the default value</param>
        /// <param name="defaultItemText">Default item text; pass null to use default value of the default item text</param>
        /// <returns>Задача представляет асинхронную операцию</returns>
        public virtual async Task PrepareCountriesAsync(IList<SelectListItem> items, bool withSpecialDefaultItem = true, string defaultItemText = null)
        {
            if (items == null)
                throw new ArgumentNullException(nameof(items));

            //prepare available countries
            var availableCountries = await _countryService.GetAllCountriesAsync(showHidden: true);
            foreach (var country in availableCountries)
            {
                items.Add(new SelectListItem { Value = country.Id.ToString(), Text = country.Name });
            }

            //insert special item for the default value
            await PrepareDefaultItemAsync(items, withSpecialDefaultItem, defaultItemText ?? await _localizationService.GetResourceAsync("Admin.Address.SelectCountry"));
        }

        /// <summary>
        /// Prepare available states and provinces
        /// </summary>
        /// <param name="items">State and province items</param>
        /// <param name="countryId">Country identifier; pass null to don't load states and provinces</param>
        /// <param name="withSpecialDefaultItem">Whether to insert the first special item for the default value</param>
        /// <param name="defaultItemText">Default item text; pass null to use default value of the default item text</param>
        /// <returns>Задача представляет асинхронную операцию</returns>
        public virtual async Task PrepareStatesAndProvincesAsync(IList<SelectListItem> items, int? countryId,
            bool withSpecialDefaultItem = true, string defaultItemText = null)
        {
            if (items == null)
                throw new ArgumentNullException(nameof(items));

            if (countryId.HasValue)
            {
                //prepare available states and provinces of the country
                var availableStates = await _stateProvinceService.GetStateProvincesByCountryIdAsync(countryId.Value, showHidden: true);
                foreach (var state in availableStates)
                {
                    items.Add(new SelectListItem { Value = state.Id.ToString(), Text = state.Name });
                }

                //insert special item for the default value
                if (items.Count > 1)
                    await PrepareDefaultItemAsync(items, withSpecialDefaultItem, defaultItemText ?? await _localizationService.GetResourceAsync("Admin.Address.SelectState"));
            }

            //insert special item for the default value
            if (!items.Any())
                await PrepareDefaultItemAsync(items, withSpecialDefaultItem, defaultItemText ?? await _localizationService.GetResourceAsync("Admin.Address.Other"));
        }

        /// <summary>
        /// Prepare available languages
        /// </summary>
        /// <param name="items">Language items</param>
        /// <param name="withSpecialDefaultItem">Whether to insert the first special item for the default value</param>
        /// <param name="defaultItemText">Default item text; pass null to use default value of the default item text</param>
        /// <returns>Задача представляет асинхронную операцию</returns>
        public virtual async Task PrepareLanguagesAsync(IList<SelectListItem> items, bool withSpecialDefaultItem = true, string defaultItemText = null)
        {
            if (items == null)
                throw new ArgumentNullException(nameof(items));

            //prepare available languages
            var availableLanguages = await _languageService.GetAllLanguagesAsync(showHidden: true);
            foreach (var language in availableLanguages)
            {
                items.Add(new SelectListItem { Value = language.Id.ToString(), Text = language.Name });
            }

            //insert special item for the default value
            await PrepareDefaultItemAsync(items, withSpecialDefaultItem, defaultItemText);
        }

        /// <summary>
        /// Prepare available stores
        /// </summary>
        /// <param name="items">Store items</param>
        /// <param name="withSpecialDefaultItem">Whether to insert the first special item for the default value</param>
        /// <param name="defaultItemText">Default item text; pass null to use default value of the default item text</param>
        /// <returns>Задача представляет асинхронную операцию</returns>
        public virtual async Task PrepareStoresAsync(IList<SelectListItem> items, bool withSpecialDefaultItem = true, string defaultItemText = null)
        {
            if (items == null)
                throw new ArgumentNullException(nameof(items));

            //prepare available stores
            var availableStores = await _storeService.GetAllStoresAsync();
            foreach (var store in availableStores)
            {
                items.Add(new SelectListItem { Value = store.Id.ToString(), Text = store.Name });
            }

            //insert special item for the default value
            await PrepareDefaultItemAsync(items, withSpecialDefaultItem, defaultItemText);
        }

        /// <summary>
        /// Prepare available user roles
        /// </summary>
        /// <param name="items">User role items</param>
        /// <param name="withSpecialDefaultItem">Whether to insert the first special item for the default value</param>
        /// <param name="defaultItemText">Default item text; pass null to use default value of the default item text</param>
        /// <returns>Задача представляет асинхронную операцию</returns>
        public virtual async Task PrepareUserRolesAsync(IList<SelectListItem> items, bool withSpecialDefaultItem = true, string defaultItemText = null)
        {
            if (items == null)
                throw new ArgumentNullException(nameof(items));

            //prepare available user roles
            var availableUserRoles = await _userService.GetAllUserRolesAsync();
            foreach (var userRole in availableUserRoles)
            {
                items.Add(new SelectListItem { Value = userRole.Id.ToString(), Text = userRole.Name });
            }

            //insert special item for the default value
            await PrepareDefaultItemAsync(items, withSpecialDefaultItem, defaultItemText);
        }

        /// <summary>
        /// Prepare available email accounts
        /// </summary>
        /// <param name="items">Email account items</param>
        /// <param name="withSpecialDefaultItem">Whether to insert the first special item for the default value</param>
        /// <param name="defaultItemText">Default item text; pass null to use default value of the default item text</param>
        /// <returns>Задача представляет асинхронную операцию</returns>
        public virtual async Task PrepareEmailAccountsAsync(IList<SelectListItem> items, bool withSpecialDefaultItem = true, string defaultItemText = null)
        {
            if (items == null)
                throw new ArgumentNullException(nameof(items));

            //prepare available email accounts
            var availableEmailAccounts = await _emailAccountService.GetAllEmailAccountsAsync();
            foreach (var emailAccount in availableEmailAccounts)
            {
                items.Add(new SelectListItem { Value = emailAccount.Id.ToString(), Text = $"{emailAccount.DisplayName} ({emailAccount.Email})" });
            }

            //insert special item for the default value
            await PrepareDefaultItemAsync(items, withSpecialDefaultItem, defaultItemText);
        }

        /// <summary>
        /// Prepare available tax categories
        /// </summary>
        /// <param name="items">Tax category items</param>
        /// <param name="withSpecialDefaultItem">Whether to insert the first special item for the default value</param>
        /// <param name="defaultItemText">Default item text; pass null to use default value of the default item text</param>
        /// <returns>Задача представляет асинхронную операцию</returns>
        public virtual async Task PrepareTaxCategoriesAsync(IList<SelectListItem> items, bool withSpecialDefaultItem = true, string defaultItemText = null)
        {
            if (items == null)
                throw new ArgumentNullException(nameof(items));

            //prepare available tax categories
            var availableTaxCategories = await _taxCategoryService.GetAllTaxCategoriesAsync();
            foreach (var taxCategory in availableTaxCategories)
            {
                items.Add(new SelectListItem { Value = taxCategory.Id.ToString(), Text = taxCategory.Name });
            }

            //insert special item for the default value
            await PrepareDefaultItemAsync(items, withSpecialDefaultItem,
                defaultItemText ?? await _localizationService.GetResourceAsync("Admin.Configuration.Settings.Tax.TaxCategories.None"));
        }

        /// <summary>
        /// Prepare available categories
        /// </summary>
        /// <param name="items">Category items</param>
        /// <param name="withSpecialDefaultItem">Whether to insert the first special item for the default value</param>
        /// <param name="defaultItemText">Default item text; pass null to use default value of the default item text</param>
        /// <returns>Задача представляет асинхронную операцию</returns>
        public virtual async Task PrepareCategoriesAsync(IList<SelectListItem> items, bool withSpecialDefaultItem = true, string defaultItemText = null)
        {
            if (items == null)
                throw new ArgumentNullException(nameof(items));

            //prepare available categories
            var availableCategoryItems = await GetCategoryListAsync();
            foreach (var categoryItem in availableCategoryItems)
            {
                items.Add(categoryItem);
            }

            //insert special item for the default value
            await PrepareDefaultItemAsync(items, withSpecialDefaultItem, defaultItemText);
        }

        /// <summary>
        /// Prepare available manufacturers
        /// </summary>
        /// <param name="items">Manufacturer items</param>
        /// <param name="withSpecialDefaultItem">Whether to insert the first special item for the default value</param>
        /// <param name="defaultItemText">Default item text; pass null to use default value of the default item text</param>
        /// <returns>Задача представляет асинхронную операцию</returns>
        public virtual async Task PrepareManufacturersAsync(IList<SelectListItem> items, bool withSpecialDefaultItem = true, string defaultItemText = null)
        {
            if (items == null)
                throw new ArgumentNullException(nameof(items));

            //prepare available manufacturers
            var availableManufacturerItems = await GetManufacturerListAsync();
            foreach (var manufacturerItem in availableManufacturerItems)
            {
                items.Add(manufacturerItem);
            }

            //insert special item for the default value
            await PrepareDefaultItemAsync(items, withSpecialDefaultItem, defaultItemText);
        }

        /// <summary>
        /// Prepare available vendors
        /// </summary>
        /// <param name="items">Vendor items</param>
        /// <param name="withSpecialDefaultItem">Whether to insert the first special item for the default value</param>
        /// <param name="defaultItemText">Default item text; pass null to use default value of the default item text</param>
        /// <returns>Задача представляет асинхронную операцию</returns>
        public virtual async Task PrepareVendorsAsync(IList<SelectListItem> items, bool withSpecialDefaultItem = true, string defaultItemText = null)
        {
            if (items == null)
                throw new ArgumentNullException(nameof(items));

            //prepare available vendors
            var availableVendorItems = await GetVendorListAsync();
            foreach (var vendorItem in availableVendorItems)
            {
                items.Add(vendorItem);
            }

            //insert special item for the default value
            await PrepareDefaultItemAsync(items, withSpecialDefaultItem, defaultItemText);
        }

        /// <summary>
        /// Prepare available tvChannel types
        /// </summary>
        /// <param name="items">TvChannel type items</param>
        /// <param name="withSpecialDefaultItem">Whether to insert the first special item for the default value</param>
        /// <param name="defaultItemText">Default item text; pass null to use default value of the default item text</param>
        /// <returns>Задача представляет асинхронную операцию</returns>
        public virtual async Task PrepareTvChannelTypesAsync(IList<SelectListItem> items, bool withSpecialDefaultItem = true, string defaultItemText = null)
        {
            if (items == null)
                throw new ArgumentNullException(nameof(items));

            //prepare available tvChannel types
            var availableTvChannelTypeItems = await TvChannelType.SimpleTvChannel.ToSelectListAsync(false);
            foreach (var tvChannelTypeItem in availableTvChannelTypeItems)
            {
                items.Add(tvChannelTypeItem);
            }

            //insert special item for the default value
            await PrepareDefaultItemAsync(items, withSpecialDefaultItem, defaultItemText);
        }

        /// <summary>
        /// Prepare available category templates
        /// </summary>
        /// <param name="items">Category template items</param>
        /// <param name="withSpecialDefaultItem">Whether to insert the first special item for the default value</param>
        /// <param name="defaultItemText">Default item text; pass null to use default value of the default item text</param>
        /// <returns>Задача представляет асинхронную операцию</returns>
        public virtual async Task PrepareCategoryTemplatesAsync(IList<SelectListItem> items, bool withSpecialDefaultItem = true, string defaultItemText = null)
        {
            if (items == null)
                throw new ArgumentNullException(nameof(items));

            //prepare available category templates
            var availableTemplates = await _categoryTemplateService.GetAllCategoryTemplatesAsync();
            foreach (var template in availableTemplates)
            {
                items.Add(new SelectListItem { Value = template.Id.ToString(), Text = template.Name });
            }

            //insert special item for the default value
            await PrepareDefaultItemAsync(items, withSpecialDefaultItem, defaultItemText);
        }

        /// <summary>
        /// Prepare available time zones
        /// </summary>
        /// <param name="items">Time zone items</param>
        /// <param name="withSpecialDefaultItem">Whether to insert the first special item for the default value</param>
        /// <param name="defaultItemText">Default item text; pass null to use default value of the default item text</param>
        /// <returns>Задача представляет асинхронную операцию</returns>
        public virtual async Task PrepareTimeZonesAsync(IList<SelectListItem> items, bool withSpecialDefaultItem = true, string defaultItemText = null)
        {
            if (items == null)
                throw new ArgumentNullException(nameof(items));

            //prepare available time zones
            var availableTimeZones = _dateTimeHelper.GetSystemTimeZones();
            foreach (var timeZone in availableTimeZones)
            {
                items.Add(new SelectListItem { Value = timeZone.Id, Text = timeZone.DisplayName });
            }

            //insert special item for the default value
            await PrepareDefaultItemAsync(items, withSpecialDefaultItem, defaultItemText);
        }

        /// <summary>
        /// Prepare available shopping cart types
        /// </summary>
        /// <param name="items">Shopping cart type items</param>
        /// <param name="withSpecialDefaultItem">Whether to insert the first special item for the default value</param>
        /// <param name="defaultItemText">Default item text; pass null to use default value of the default item text</param>
        /// <returns>Задача представляет асинхронную операцию</returns>
        public virtual async Task PrepareShoppingCartTypesAsync(IList<SelectListItem> items, bool withSpecialDefaultItem = true, string defaultItemText = null)
        {
            if (items == null)
                throw new ArgumentNullException(nameof(items));

            //prepare available shopping cart types
            var availableShoppingCartTypeItems = await ShoppingCartType.ShoppingCart.ToSelectListAsync(false);
            foreach (var shoppingCartTypeItem in availableShoppingCartTypeItems)
            {
                items.Add(shoppingCartTypeItem);
            }

            //insert special item for the default value
            await PrepareDefaultItemAsync(items, withSpecialDefaultItem, defaultItemText);
        }

        /// <summary>
        /// Prepare available tax display types
        /// </summary>
        /// <param name="items">Tax display type items</param>
        /// <param name="withSpecialDefaultItem">Whether to insert the first special item for the default value</param>
        /// <param name="defaultItemText">Default item text; pass null to use default value of the default item text</param>
        /// <returns>Задача представляет асинхронную операцию</returns>
        public virtual async Task PrepareTaxDisplayTypesAsync(IList<SelectListItem> items, bool withSpecialDefaultItem = true, string defaultItemText = null)
        {
            if (items == null)
                throw new ArgumentNullException(nameof(items));

            //prepare available tax display types
            var availableTaxDisplayTypeItems = await TaxDisplayType.ExcludingTax.ToSelectListAsync(false);
            foreach (var taxDisplayTypeItem in availableTaxDisplayTypeItems)
            {
                items.Add(taxDisplayTypeItem);
            }

            //insert special item for the default value
            await PrepareDefaultItemAsync(items, withSpecialDefaultItem, defaultItemText);
        }

        /// <summary>
        /// Prepare available currencies
        /// </summary>
        /// <param name="items">Currency items</param>
        /// <param name="withSpecialDefaultItem">Whether to insert the first special item for the default value</param>
        /// <param name="defaultItemText">Default item text; pass null to use default value of the default item text</param>
        /// <returns>Задача представляет асинхронную операцию</returns>
        public virtual async Task PrepareCurrenciesAsync(IList<SelectListItem> items, bool withSpecialDefaultItem = true, string defaultItemText = null)
        {
            if (items == null)
                throw new ArgumentNullException(nameof(items));

            //prepare available currencies
            var availableCurrencies = await _currencyService.GetAllCurrenciesAsync(true);
            foreach (var currency in availableCurrencies)
            {
                items.Add(new SelectListItem { Value = currency.Id.ToString(), Text = currency.Name });
            }

            //insert special item for the default value
            await PrepareDefaultItemAsync(items, withSpecialDefaultItem, defaultItemText);
        }

        /// <summary>
        /// Prepare available discount types
        /// </summary>
        /// <param name="items">Discount type items</param>
        /// <param name="withSpecialDefaultItem">Whether to insert the first special item for the default value</param>
        /// <param name="defaultItemText">Default item text; pass null to use default value of the default item text</param>
        /// <returns>Задача представляет асинхронную операцию</returns>
        public virtual async Task PrepareDiscountTypesAsync(IList<SelectListItem> items, bool withSpecialDefaultItem = true, string defaultItemText = null)
        {
            if (items == null)
                throw new ArgumentNullException(nameof(items));

            //prepare available discount types
            var availableDiscountTypeItems = await DiscountType.AssignedToOrderTotal.ToSelectListAsync(false);
            foreach (var discountTypeItem in availableDiscountTypeItems)
            {
                items.Add(discountTypeItem);
            }

            //insert special item for the default value
            await PrepareDefaultItemAsync(items, withSpecialDefaultItem, defaultItemText);
        }

        /// <summary>
        /// Prepare available log levels
        /// </summary>
        /// <param name="items">Log level items</param>
        /// <param name="withSpecialDefaultItem">Whether to insert the first special item for the default value</param>
        /// <param name="defaultItemText">Default item text; pass null to use default value of the default item text</param>
        /// <returns>Задача представляет асинхронную операцию</returns>
        public virtual async Task PrepareLogLevelsAsync(IList<SelectListItem> items, bool withSpecialDefaultItem = true, string defaultItemText = null)
        {
            if (items == null)
                throw new ArgumentNullException(nameof(items));

            //prepare available log levels
            var availableLogLevelItems = await LogLevel.Debug.ToSelectListAsync(false);
            foreach (var logLevelItem in availableLogLevelItems)
            {
                items.Add(logLevelItem);
            }

            //insert special item for the default value
            await PrepareDefaultItemAsync(items, withSpecialDefaultItem, defaultItemText);
        }

        /// <summary>
        /// Prepare available manufacturer templates
        /// </summary>
        /// <param name="items">Manufacturer template items</param>
        /// <param name="withSpecialDefaultItem">Whether to insert the first special item for the default value</param>
        /// <param name="defaultItemText">Default item text; pass null to use default value of the default item text</param>
        /// <returns>Задача представляет асинхронную операцию</returns>
        public virtual async Task PrepareManufacturerTemplatesAsync(IList<SelectListItem> items,
            bool withSpecialDefaultItem = true, string defaultItemText = null)
        {
            if (items == null)
                throw new ArgumentNullException(nameof(items));

            //prepare available manufacturer templates
            var availableTemplates = await _manufacturerTemplateService.GetAllManufacturerTemplatesAsync();
            foreach (var template in availableTemplates)
            {
                items.Add(new SelectListItem { Value = template.Id.ToString(), Text = template.Name });
            }

            //insert special item for the default value
            await PrepareDefaultItemAsync(items, withSpecialDefaultItem, defaultItemText);
        }

        /// <summary>
        /// Prepare available load plugin modes
        /// </summary>
        /// <param name="items">Load plugin mode items</param>
        /// <param name="withSpecialDefaultItem">Whether to insert the first special item for the default value</param>
        /// <param name="defaultItemText">Default item text; pass null to use default value of the default item text</param>
        /// <returns>Задача представляет асинхронную операцию</returns>
        public virtual async Task PrepareLoadPluginModesAsync(IList<SelectListItem> items, bool withSpecialDefaultItem = true, string defaultItemText = null)
        {
            if (items == null)
                throw new ArgumentNullException(nameof(items));

            //prepare available load plugin modes
            var availableLoadPluginModeItems = await LoadPluginsMode.All.ToSelectListAsync(false);
            foreach (var loadPluginModeItem in availableLoadPluginModeItems)
            {
                items.Add(loadPluginModeItem);
            }

            //insert special item for the default value
            await PrepareDefaultItemAsync(items, withSpecialDefaultItem, defaultItemText);
        }

        /// <summary>
        /// Prepare available plugin groups
        /// </summary>
        /// <param name="items">Plugin group items</param>
        /// <param name="withSpecialDefaultItem">Whether to insert the first special item for the default value</param>
        /// <param name="defaultItemText">Default item text; pass null to use default value of the default item text</param>
        /// <returns>Задача представляет асинхронную операцию</returns>
        public virtual async Task PreparePluginGroupsAsync(IList<SelectListItem> items, bool withSpecialDefaultItem = true, string defaultItemText = null)
        {
            if (items == null)
                throw new ArgumentNullException(nameof(items));

            //prepare available plugin groups
            var availablePluginGroups = (await _pluginService.GetPluginDescriptorsAsync<IPlugin>(LoadPluginsMode.All))
                .Select(plugin => plugin.Group).Distinct().OrderBy(groupName => groupName).ToList();
            foreach (var group in availablePluginGroups)
                items.Add(new SelectListItem { Value = @group, Text = @group });

            //insert special item for the default value
            await PrepareDefaultItemAsync(items, withSpecialDefaultItem, defaultItemText);
        }

        /// <summary>
        /// Prepare available return request statuses
        /// </summary>
        /// <param name="items">Return request status items</param>
        /// <param name="withSpecialDefaultItem">Whether to insert the first special item for the default value</param>
        /// <param name="defaultItemText">Default item text; pass null to use default value of the default item text</param>
        /// <returns>Задача представляет асинхронную операцию</returns>
        public virtual async Task PrepareReturnRequestStatusesAsync(IList<SelectListItem> items,
            bool withSpecialDefaultItem = true, string defaultItemText = null)
        {
            if (items == null)
                throw new ArgumentNullException(nameof(items));

            //prepare available return request statuses
            var availableStatusItems = await ReturnRequestStatus.Pending.ToSelectListAsync(false);
            foreach (var statusItem in availableStatusItems)
            {
                items.Add(statusItem);
            }

            //insert special item for the default value
            await PrepareDefaultItemAsync(items, withSpecialDefaultItem, defaultItemText);
        }

        /// <summary>
        /// Prepare available tvChannel templates
        /// </summary>
        /// <param name="items">TvChannel template items</param>
        /// <param name="withSpecialDefaultItem">Whether to insert the first special item for the default value</param>
        /// <param name="defaultItemText">Default item text; pass null to use default value of the default item text</param>
        /// <returns>Задача представляет асинхронную операцию</returns>
        public virtual async Task PrepareTvChannelTemplatesAsync(IList<SelectListItem> items, bool withSpecialDefaultItem = true, string defaultItemText = null)
        {
            if (items == null)
                throw new ArgumentNullException(nameof(items));

            //prepare available tvChannel templates
            var availableTemplates = await _tvChannelTemplateService.GetAllTvChannelTemplatesAsync();
            foreach (var template in availableTemplates)
            {
                items.Add(new SelectListItem { Value = template.Id.ToString(), Text = template.Name });
            }

            //insert special item for the default value
            await PrepareDefaultItemAsync(items, withSpecialDefaultItem, defaultItemText);
        }

        /// <summary>
        /// Prepare available topic templates
        /// </summary>
        /// <param name="items">Topic template items</param>
        /// <param name="withSpecialDefaultItem">Whether to insert the first special item for the default value</param>
        /// <param name="defaultItemText">Default item text; pass null to use default value of the default item text</param>
        /// <returns>Задача представляет асинхронную операцию</returns>
        public virtual async Task PrepareTopicTemplatesAsync(IList<SelectListItem> items, bool withSpecialDefaultItem = true, string defaultItemText = null)
        {
            if (items == null)
                throw new ArgumentNullException(nameof(items));

            //prepare available topic templates
            var availableTemplates = await _topicTemplateService.GetAllTopicTemplatesAsync();
            foreach (var template in availableTemplates)
            {
                items.Add(new SelectListItem { Value = template.Id.ToString(), Text = template.Name });
            }

            //insert special item for the default value
            await PrepareDefaultItemAsync(items, withSpecialDefaultItem, defaultItemText);
        }

        /// <summary>
        /// Prepare available warehouses
        /// </summary>
        /// <param name="items">Warehouse items</param>
        /// <param name="withSpecialDefaultItem">Whether to insert the first special item for the default value</param>
        /// <param name="defaultItemText">Default item text; pass null to use default value of the default item text</param>
        /// <returns>Задача представляет асинхронную операцию</returns>
        public virtual async Task PrepareWarehousesAsync(IList<SelectListItem> items, bool withSpecialDefaultItem = true, string defaultItemText = null)
        {
            if (items == null)
                throw new ArgumentNullException(nameof(items));

            //prepare available warehouses
            var availableWarehouses = await _shippingService.GetAllWarehousesAsync();
            foreach (var warehouse in availableWarehouses)
            {
                items.Add(new SelectListItem { Value = warehouse.Id.ToString(), Text = warehouse.Name });
            }

            //insert special item for the default value
            await PrepareDefaultItemAsync(items, withSpecialDefaultItem, defaultItemText);
        }

        /// <summary>
        /// Prepare available delivery dates
        /// </summary>
        /// <param name="items">Delivery date items</param>
        /// <param name="withSpecialDefaultItem">Whether to insert the first special item for the default value</param>
        /// <param name="defaultItemText">Default item text; pass null to use default value of the default item text</param>
        /// <returns>Задача представляет асинхронную операцию</returns>
        public virtual async Task PrepareDeliveryDatesAsync(IList<SelectListItem> items, bool withSpecialDefaultItem = true, string defaultItemText = null)
        {
            if (items == null)
                throw new ArgumentNullException(nameof(items));

            //prepare available delivery dates
            var availableDeliveryDates = await _dateRangeService.GetAllDeliveryDatesAsync();
            foreach (var date in availableDeliveryDates)
            {
                items.Add(new SelectListItem { Value = date.Id.ToString(), Text = date.Name });
            }

            //insert special item for the default value
            await PrepareDefaultItemAsync(items, withSpecialDefaultItem, defaultItemText);
        }

        /// <summary>
        /// Prepare available tvChannel availability ranges
        /// </summary>
        /// <param name="items">TvChannel availability range items</param>
        /// <param name="withSpecialDefaultItem">Whether to insert the first special item for the default value</param>
        /// <param name="defaultItemText">Default item text; pass null to use default value of the default item text</param>
        /// <returns>Задача представляет асинхронную операцию</returns>
        public virtual async Task PrepareTvChannelAvailabilityRangesAsync(IList<SelectListItem> items,
            bool withSpecialDefaultItem = true, string defaultItemText = null)
        {
            if (items == null)
                throw new ArgumentNullException(nameof(items));

            //prepare available tvChannel availability ranges
            var availableTvChannelAvailabilityRanges = await _dateRangeService.GetAllTvChannelAvailabilityRangesAsync();
            foreach (var range in availableTvChannelAvailabilityRanges)
            {
                items.Add(new SelectListItem { Value = range.Id.ToString(), Text = range.Name });
            }

            //insert special item for the default value
            await PrepareDefaultItemAsync(items, withSpecialDefaultItem, defaultItemText);
        }

        /// <summary>
        /// Prepare available GDPR request types
        /// </summary>
        /// <param name="items">Request type items</param>
        /// <param name="withSpecialDefaultItem">Whether to insert the first special item for the default value</param>
        /// <param name="defaultItemText">Default item text; pass null to use default value of the default item text</param>
        /// <returns>Задача представляет асинхронную операцию</returns>
        public virtual async Task PrepareGdprRequestTypesAsync(IList<SelectListItem> items, bool withSpecialDefaultItem = true, string defaultItemText = null)
        {
            if (items == null)
                throw new ArgumentNullException(nameof(items));

            //prepare available request types
            var gdprRequestTypeItems = await GdprRequestType.ConsentAgree.ToSelectListAsync(false);
            foreach (var gdprRequestTypeItem in gdprRequestTypeItems)
            {
                items.Add(gdprRequestTypeItem);
            }

            //insert special item for the default value
            await PrepareDefaultItemAsync(items, withSpecialDefaultItem, defaultItemText);
        }

        /// <summary>
        /// Prepare available specification attribute groups
        /// </summary>
        /// <param name="items">Specification attributes</param>
        /// <param name="withSpecialDefaultItem">Whether to insert the first special item for the default value</param>
        /// <param name="defaultItemText">Default item text; pass null to use default value of the default item text</param>
        /// <returns>Задача представляет асинхронную операцию</returns>
        public virtual async Task PrepareSpecificationAttributeGroupsAsync(IList<SelectListItem> items, bool withSpecialDefaultItem = true, string defaultItemText = null)
        {
            if (items == null)
                throw new ArgumentNullException(nameof(items));

            //prepare available specification attribute groups
            var availableSpecificationAttributeGroups = await _specificationAttributeService.GetSpecificationAttributeGroupsAsync();
            foreach (var group in availableSpecificationAttributeGroups)
            {
                items.Add(new SelectListItem { Value = group.Id.ToString(), Text = group.Name });
            }

            // use empty string for nullable field
            var defaultItemValue = string.Empty;

            //insert special item for the default value
            await PrepareDefaultItemAsync(items, withSpecialDefaultItem, defaultItemText, defaultItemValue);
        }

        #endregion
    }
}