using System;
using System.Linq;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Localization;
using TVProgViewer.Services.Authentication;
using TVProgViewer.Services.Common;
using TVProgViewer.Services.Users;
using TVProgViewer.Services.Directory;
using TVProgViewer.Services.Helpers;
using TVProgViewer.Services.Localization;
using TVProgViewer.Services.Stores;
using TVProgViewer.Services.Vendors;
using TVProgViewer.Core;
using TVProgViewer.Core.Domain.Users;
using TVProgViewer.Core.Domain.Directory;
using TVProgViewer.Core.Domain.Localization;
using TVProgViewer.Core.Domain.Tax;
using TVProgViewer.Core.Domain.Vendors;
using TVProgViewer.Core.Http;
using TVProgViewer.Services.Tasks;
using TVProgViewer.Core.Domain.TvProgMain;
using TVProgViewer.Services.TvProgMain;

namespace TVProgViewer.Web.Framework
{
    /// <summary>
    /// Represents work context for web application
    /// </summary>
    public partial class WebWorkContext : IWorkContext
    {
        #region Поля

        private readonly CurrencySettings _currencySettings;
        private readonly ProgrammeSettings _programmeSettings;
        private readonly IAuthenticationService _authenticationService;
        private readonly ICurrencyService _currencyService;
        private readonly IUserService _userService;
        private readonly IProgrammeService _programmeService;
        private readonly IGenericAttributeService _genericAttributeService;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ILanguageService _languageService;
        private readonly IStoreContext _storeContext;
        private readonly IStoreMappingService _storeMappingService;
        private readonly IUserAgentHelper _userAgentHelper;
        private readonly IVendorService _vendorService;
        private readonly LocalizationSettings _localizationSettings;
        private readonly TaxSettings _taxSettings;

        private User _cachedUser;
        private User _originalUserIfImpersonated;
        private Vendor _cachedVendor;
        private Language _cachedLanguage;
        private Currency _cachedCurrency;
        private TvProgProviders _cachedProvider;
        private TypeProg _cachedTypeProg;
        private string _cachedCategory;
        private TaxDisplayType? _cachedTaxDisplayType;

        #endregion

        #region Ctor

        public WebWorkContext(CurrencySettings currencySettings,
            ProgrammeSettings programmeSettings,
            IAuthenticationService authenticationService,
            ICurrencyService currencyService,
            IUserService userService,
            IProgrammeService programmeService,
            IGenericAttributeService genericAttributeService,
            IHttpContextAccessor httpContextAccessor,
            ILanguageService languageService,
            IStoreContext storeContext,
            IStoreMappingService storeMappingService,
            IUserAgentHelper userAgentHelper,
            IVendorService vendorService,
            LocalizationSettings localizationSettings,
            TaxSettings taxSettings)
        {
            _currencySettings = currencySettings;
            _programmeSettings = programmeSettings;
            _authenticationService = authenticationService;
            _currencyService = currencyService;
            _userService = userService;
            _programmeService = programmeService;
            _genericAttributeService = genericAttributeService;
            _httpContextAccessor = httpContextAccessor;
            _languageService = languageService;
            _storeContext = storeContext;
            _storeMappingService = storeMappingService;
            _userAgentHelper = userAgentHelper;
            _vendorService = vendorService;
            _localizationSettings = localizationSettings;
            _taxSettings = taxSettings;
        }

        #endregion

        #region Utilities

        /// <summary>
        /// Get TVProgViewer User cookie
        /// </summary>
        /// <returns>String value of cookie</returns>
        protected virtual string GetUserCookie()
        {
            var cookieName = $"{TvProgCookieDefaults.Prefix}{TvProgCookieDefaults.UserCookie}";
            return _httpContextAccessor.HttpContext?.Request?.Cookies[cookieName];
        }

        /// <summary>
        /// Set TVProgViewer User cookie
        /// </summary>
        /// <param name="UserGuid">Guid of the User</param>
        protected virtual void SetUserCookie(Guid UserGuid)
        {
            if (_httpContextAccessor.HttpContext?.Response == null)
                return;

            //delete current cookie value
            var cookieName = $"{TvProgCookieDefaults.Prefix}{TvProgCookieDefaults.UserCookie}";
            _httpContextAccessor.HttpContext.Response.Cookies.Delete(cookieName);

            //get date of cookie expiration
            var cookieExpires = 24 * 365; //TODO make configurable
            var cookieExpiresDate = DateTime.Now.AddHours(cookieExpires);

            //if passed guid is empty set cookie as expired
            if (UserGuid == Guid.Empty)
                cookieExpiresDate = DateTime.Now.AddMonths(-1);

            //set new cookie value
            var options = new CookieOptions
            {
                HttpOnly = true,
                Expires = cookieExpiresDate,
                Secure = _httpContextAccessor.HttpContext.Request.IsHttps
            };
            _httpContextAccessor.HttpContext.Response.Cookies.Append(cookieName, UserGuid.ToString(), options);
        }

        /// <summary>
        /// Get language from the requested page URL
        /// </summary>
        /// <returns>The found language</returns>
        protected virtual Language GetLanguageFromUrl()
        {
            if (_httpContextAccessor.HttpContext?.Request == null)
                return null;

            //whether the requsted URL is localized
            var path = _httpContextAccessor.HttpContext.Request.Path.Value;
            if (!path.IsLocalizedUrl(_httpContextAccessor.HttpContext.Request.PathBase, false, out Language language))
                return null;

            //check language availability
            if (!_storeMappingService.Authorize(language))
                return null;

            return language;
        }

        /// <summary>
        /// Get language from the request
        /// </summary>
        /// <returns>The found language</returns>
        protected virtual Language GetLanguageFromRequest()
        {
            if (_httpContextAccessor.HttpContext?.Request == null)
                return null;

            //get request culture
            var requestCulture = _httpContextAccessor.HttpContext.Features.Get<IRequestCultureFeature>()?.RequestCulture;
            if (requestCulture == null)
                return null;

            //try to get language by culture name
            var requestLanguage = _languageService.GetAllLanguages().FirstOrDefault(language =>
                language.LanguageCulture.Equals(requestCulture.Culture.Name, StringComparison.InvariantCultureIgnoreCase));

            //check language availability
            if (requestLanguage == null || !requestLanguage.Published || !_storeMappingService.Authorize(requestLanguage))
                return null;

            return requestLanguage;
        }

        #endregion

        #region Properties

        /// <summary>
        /// Текущий пользователь
        /// </summary>
        public virtual User CurrentUser
        {
            get
            {
                // Закэширован ли он сейчас:
                if (_cachedUser != null)
                    return _cachedUser;

                User user = null;

                // Проверить, сделан ли запрос фоновой (по расписанию) задачей:
                if (_httpContextAccessor.HttpContext == null ||
                    _httpContextAccessor.HttpContext.Request.Path.Equals(new PathString($"/{TvProgTaskDefaults.ScheduleTaskPath}"), StringComparison.InvariantCultureIgnoreCase))
                {
                    // В этом случае, вернуть встроенную пользовательскую запись для фоновой задачи:
                    user = _userService.GetUserBySystemName(TvProgUserDefaults.BackgroundTaskUserName);
                }

                if (user == null || user.Deleted != null || !user.Active || user.RequireReLogin)
                {
                    // Проверить, если запрос сделан поисковой системой, в этом случае возвращать встроенную запись пользователя для поисковых систем:
                    if (_userAgentHelper.IsSearchEngine())
                        user = _userService.GetUserBySystemName(TvProgUserDefaults.SearchEngineUserName);
                }

                if (user == null || user.Deleted != null || !user.Active || user.RequireReLogin)
                {
                    // Попытка получить регистрированного пользователя:
                    user = _authenticationService.GetAuthenticatedUser();
                }

                if (user != null && user.Deleted == null && user.Active && !user.RequireReLogin)
                {
                    // Получить олицетворение пользователя, если требуется:
                    var impersonatedUserId = _genericAttributeService
                        .GetAttribute<int?>(user, TvProgUserDefaults.ImpersonatedUserIdAttribute);
                    if (impersonatedUserId.HasValue && impersonatedUserId.Value > 0)
                    {
                        var impersonatedUser = _userService.GetUserById(impersonatedUserId.Value);
                        if (impersonatedUser != null && impersonatedUser.Deleted != null && impersonatedUser.Active && !impersonatedUser.RequireReLogin)
                        {
                            // Установка олицетворения пользователя:
                            _originalUserIfImpersonated = user;
                            user = impersonatedUser;
                        }
                    }
                }

                if (user == null || user.Deleted != null || !user.Active || user.RequireReLogin)
                {
                    // Получение гостевого пользователя
                    var userCookie = GetUserCookie();
                    if (!string.IsNullOrEmpty(userCookie))
                    {
                        if (Guid.TryParse(userCookie, out Guid userGuid))
                        {
                            // Получение пользователя из куки (не должен быть зарегистрирован(а)):
                            var userByCookie = _userService.GetUserByGuid(userGuid);
                            if (userByCookie != null && !_userService.IsRegistered(userByCookie))
                                user = userByCookie;
                        }
                    }
                }

                if (user == null || user.Deleted != null || !user.Active || user.RequireReLogin)
                {
                    // Создание гостя, если не существует
                    user = _userService.InsertGuestUser();
                }

                if (user.Deleted == null && user.Active && !user.RequireReLogin)
                {
                    // Установка пользовательского куки
                    SetUserCookie(user.UserGuid);

                    // Кэширование найденного пользователя
                    _cachedUser = user;
                }

                return _cachedUser;
            }
            set
            {
                SetUserCookie(value.UserGuid);
                _cachedUser = value;
            }
        }

        /// <summary>
        /// Gets the original User (in case the current one is impersonated)
        /// </summary>
        public virtual User OriginalUserIfImpersonated
        {
            get { return _originalUserIfImpersonated; }
        }

        /// <summary>
        /// Gets the current vendor (logged-in manager)
        /// </summary>
        public virtual Vendor CurrentVendor
        {
            get
            {
                //whether there is a cached value
                if (_cachedVendor != null)
                    return _cachedVendor;

                if (CurrentUser == null)
                    return null;

                return _cachedVendor;
            }
        }

        /// <summary>
        /// Gets or sets current user working language
        /// </summary>
        public virtual Language WorkingLanguage
        {
            get
            {
                //whether there is a cached value
                if (_cachedLanguage != null)
                    return _cachedLanguage;

                Language detectedLanguage = null;

                //localized URLs are enabled, so try to get language from the requested page URL
                if (_localizationSettings.SeoFriendlyUrlsForLanguagesEnabled)
                    detectedLanguage = GetLanguageFromUrl();

                //whether we should detect the language from the request
                if (detectedLanguage == null && _localizationSettings.AutomaticallyDetectLanguage)
                {
                    //whether language already detected by this way
                    var alreadyDetected = _genericAttributeService.GetAttribute<bool>(CurrentUser,
                        TvProgUserDefaults.LanguageAutomaticallyDetectedAttribute, _storeContext.CurrentStore.Id);

                    //if not, try to get language from the request
                    if (!alreadyDetected)
                    {
                        detectedLanguage = GetLanguageFromRequest();
                        if (detectedLanguage != null)
                        {
                            //language already detected
                            _genericAttributeService.SaveAttribute(CurrentUser,
                                TvProgUserDefaults.LanguageAutomaticallyDetectedAttribute, true, _storeContext.CurrentStore.Id);
                        }
                    }
                }

                //if the language is detected we need to save it
                if (detectedLanguage != null)
                {
                    //get current saved language identifier
                    var currentLanguageId = _genericAttributeService.GetAttribute<int>(CurrentUser,
                        TvProgUserDefaults.LanguageIdAttribute, _storeContext.CurrentStore.Id);

                    //save the detected language identifier if it differs from the current one
                    if (detectedLanguage.Id != currentLanguageId)
                    {
                        _genericAttributeService.SaveAttribute(CurrentUser,
                            TvProgUserDefaults.LanguageIdAttribute, detectedLanguage.Id, _storeContext.CurrentStore.Id);
                    }
                }

                //get current User language identifier
                var UserLanguageId = _genericAttributeService.GetAttribute<int>(CurrentUser,
                    TvProgUserDefaults.LanguageIdAttribute, _storeContext.CurrentStore.Id);

                var allStoreLanguages = _languageService.GetAllLanguages(storeId: _storeContext.CurrentStore.Id);

                //check User language availability
                var UserLanguage = allStoreLanguages.FirstOrDefault(language => language.Id == UserLanguageId);
                if (UserLanguage == null)
                {
                    //it not found, then try to get the default language for the current store (if specified)
                    UserLanguage = allStoreLanguages.FirstOrDefault(language => language.Id == _storeContext.CurrentStore.DefaultLanguageId);
                }

                //if the default language for the current store not found, then try to get the first one
                if (UserLanguage == null)
                    UserLanguage = allStoreLanguages.FirstOrDefault();

                //if there are no languages for the current store try to get the first one regardless of the store
                if (UserLanguage == null)
                    UserLanguage = _languageService.GetAllLanguages().FirstOrDefault();

                //cache the found language
                _cachedLanguage = UserLanguage;

                return _cachedLanguage;
            }
            set
            {
                //get passed language identifier
                var languageId = value?.Id ?? 0;

                //and save it
                _genericAttributeService.SaveAttribute(CurrentUser,
                    TvProgUserDefaults.LanguageIdAttribute, languageId, _storeContext.CurrentStore.Id);

                //then reset the cached value
                _cachedLanguage = null;
            }
        }
        
        /// <summary>
        /// Для удаления
        /// </summary>
        public virtual Currency WorkingCurrency { get; set; }
        /// <summary>
        /// Получение и установка ТВ-провайдера текущего пользователя
        /// </summary>
        public virtual TvProgProviders WorkingProvider
        {
            get 
            {
                // Убедиться, есть ли закешированное значение:
                if (_cachedProvider != null)
                    return _cachedProvider;

                // Первичное значение провайдера, когда мы под пространством/режимом админа
                if (IsAdmin)
                {
                    var primarySystemProvider = _programmeService.GetProviderById(_programmeSettings.PrimarySystemProviderId);
                    if (primarySystemProvider != null)
                    {
                        _cachedProvider = primarySystemProvider;
                        return primarySystemProvider;
                    }
                }

                // Поиск ТВ-провайдера первоначально выбранного пользователем
                var userProviderId = _genericAttributeService.GetAttribute<int>(CurrentUser,
                    TvProgUserDefaults.ProviderIdAttribute, _storeContext.CurrentStore.Id);

                var allStoreProviders = _programmeService.GetAllProviders();

                // Проверка доступности ТВ-провайдера пользователю:
                var userProvider = allStoreProviders.FirstOrDefault(provider => provider.Id == userProviderId);
                if (userProvider == null)
                {
                   // Если он не найден, тогда попытаться получить ТВ-провайдера по умолчанию:
                   userProvider = allStoreProviders.FirstOrDefault(provider => provider.Id == _programmeSettings.PrimarySystemProviderId);
                }

                // Если по умолчанию ТВ-провайдер Не найден, тогда попытаться получить первый попавшийся:
                if (userProvider == null)
                    userProvider = allStoreProviders.FirstOrDefault();

                
                // Кэширование найденного ТВ-провайдера:
                _cachedProvider = userProvider;

                return _cachedProvider;
            }
            set
            {
                // Получение идентификатора ТВ-провайдера
                var providerId = value?.Id ?? 0;

                // И его сохранение
                _genericAttributeService.SaveAttribute(CurrentUser,
                    TvProgUserDefaults.ProviderIdAttribute, providerId, _storeContext.CurrentStore.Id);

                // Тогда сбросим закэшированное значение
                _cachedProvider = null;
            }
        }

        /// <summary>
        /// Получение или установка типа ТВ-программы текущего пользователя
        /// </summary>
        public virtual TypeProg WorkingTypeProg
        {
            get 
            {
                // Убедиться, есть ли закешированное значение
                if (_cachedTypeProg != null)
                   return _cachedTypeProg;

                // Первичное значение типа, когда мы под пространством/режимом админа
                if (IsAdmin)
                {
                    var primarySystemTypeProg = _programmeService.GetTypeProgById(_programmeSettings.PrimarySystemTypeProgId);
                    if (primarySystemTypeProg != null)
                    {
                        _cachedTypeProg = primarySystemTypeProg;
                        return primarySystemTypeProg;
                    }
                }

                // Поиск типа ТВ-программы, первоначально выбранного пользователем
                var userTypeProgId = _genericAttributeService.GetAttribute<int>(CurrentUser,
                    TvProgUserDefaults.TypeProgIdAttribute, _storeContext.CurrentStore.Id);

                var allStoreTypeProgs = _programmeService.GetAllTypeProgs();

                // Проверка доступности типа ТВ-программы пользователю:
                var userTypeProg = allStoreTypeProgs.FirstOrDefault(typeProg => typeProg.Id == userTypeProgId);
                if (userTypeProg == null)
                {
                    // Если он не найден, тогда попытаться получить тип ТВ-программы по умолчанию:
                    userTypeProg = allStoreTypeProgs.FirstOrDefault(typeProg => typeProg.Id == _programmeSettings.PrimarySystemTypeProgId);
                }

                // Если по умолчанию тип ТВ-программы не найден, тогда попытаться получить первый попавшийся:
                if (userTypeProg == null)
                    userTypeProg = allStoreTypeProgs.FirstOrDefault();


                // Кэширование найденного типа ТВ-программы:
                _cachedTypeProg = userTypeProg;

                return _cachedTypeProg;
            }
            set
            {
                // Получение идентификатора типа ТВ-программы
                var typeProgId = value?.Id ?? 0;

                // И его сохранение
                _genericAttributeService.SaveAttribute(CurrentUser,
                    TvProgUserDefaults.TypeProgIdAttribute, typeProgId, _storeContext.CurrentStore.Id);

                // Тогда сбросим закэшированное значение
                _cachedTypeProg = null;
            }
        }

        /// <summary>
        /// Получение или установка пользовательской категории ТВ-программы
        /// </summary>
        public virtual string WorkingCategory 
        {
            get
            {
                // Убедиться, есть ли закешированное значение
                if (_cachedCategory != null)
                    return _cachedCategory;

                // Первичное значение типа, когда мы под пространством/режимом админа
                if (IsAdmin)
                {
                    var primarySystemCategory = _programmeSettings.PrimarySystemCategory;
                    if (primarySystemCategory != null)
                    {
                        _cachedCategory = primarySystemCategory;
                        return primarySystemCategory;
                    }
                }

                // Поиск категории ТВ-программы, первоначально выбранного пользователем
                var userCategoryName = _genericAttributeService.GetAttribute<string>(CurrentUser,
                    TvProgUserDefaults.CategoryAttribute, _storeContext.CurrentStore.Id);

                var allStoreCategory = _programmeService.GetCategories();

                // Проверка доступности категории ТВ-программы пользователю:
                var userCategory = allStoreCategory.FirstOrDefault(category => category == userCategoryName);
                if (userCategory == null)
                {
                    // Если он не найден, тогда попытаться получить категорию ТВ-программы по умолчанию:
                    userCategory = allStoreCategory.FirstOrDefault(category => category == _programmeSettings.PrimarySystemCategory);
                }

                // Если по умолчанию категория ТВ-программы не найдена, тогда попытаться получить первую попавшуюся:
                if (userCategory == null)
                    userCategory = allStoreCategory.FirstOrDefault();


                // Кэширование найденной категории ТВ-программы:
                _cachedCategory = userCategory;

                return _cachedCategory;
            } 
            set 
            {
                // Получение категории ТВ-программы
                var category = value ?? "Все категории";

                // И её сохранение
                _genericAttributeService.SaveAttribute(CurrentUser,
                    TvProgUserDefaults.CategoryAttribute, category, _storeContext.CurrentStore.Id);

                // Тогда сбросим закэшированное значение
                _cachedCategory = null;
            } 
        }

        /// <summary>
        /// Gets or sets current tax display type
        /// </summary>
        public virtual TaxDisplayType TaxDisplayType
        {
            get
            {
                //whether there is a cached value
                if (_cachedTaxDisplayType.HasValue)
                    return _cachedTaxDisplayType.Value;

                var taxDisplayType = TaxDisplayType.IncludingTax;

                //whether Users are allowed to select tax display type
                if (_taxSettings.AllowUsersToSelectTaxDisplayType && CurrentUser != null)
                {
                    //try to get previously saved tax display type
                    var taxDisplayTypeId = _genericAttributeService.GetAttribute<int?>(CurrentUser,
                        TvProgUserDefaults.TaxDisplayTypeIdAttribute, _storeContext.CurrentStore.Id);
                    if (taxDisplayTypeId.HasValue)
                    {
                        taxDisplayType = (TaxDisplayType)taxDisplayTypeId.Value;
                    }
                    else
                    {
                        //default tax type by User roles
                        var defaultRoleTaxDisplayType = _userService.GetUserDefaultTaxDisplayType(CurrentUser);
                        if (defaultRoleTaxDisplayType != null)
                        {
                            taxDisplayType = defaultRoleTaxDisplayType.Value;
                        }
                    }
                }
                else
                {
                    //default tax type by User roles
                    var defaultRoleTaxDisplayType = _userService.GetUserDefaultTaxDisplayType(CurrentUser);
                    if (defaultRoleTaxDisplayType != null)
                    {
                        taxDisplayType = defaultRoleTaxDisplayType.Value;
                    }
                    else
                    {
                        //or get the default tax display type
                        taxDisplayType = _taxSettings.TaxDisplayType;
                    }
                }

                //cache the value
                _cachedTaxDisplayType = taxDisplayType;

                return _cachedTaxDisplayType.Value;

            }
            set
            {
                //whether Users are allowed to select tax display type
                if (!_taxSettings.AllowUsersToSelectTaxDisplayType)
                    return;

                //save passed value
                _genericAttributeService.SaveAttribute(CurrentUser,
                    TvProgUserDefaults.TaxDisplayTypeIdAttribute, (int)value, _storeContext.CurrentStore.Id);

                //then reset the cached value
                _cachedTaxDisplayType = null;
            }
        }

        /// <summary>
        /// Gets or sets value indicating whether we're in admin area
        /// </summary>
        public virtual bool IsAdmin { get; set; }

        #endregion
    }
}