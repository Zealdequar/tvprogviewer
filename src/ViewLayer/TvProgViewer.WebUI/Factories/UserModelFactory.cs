using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Rendering;
using TvProgViewer.Core;
using TvProgViewer.Core.Domain.Catalog;
using TvProgViewer.Core.Domain.Common;
using TvProgViewer.Core.Domain.Users;
using TvProgViewer.Core.Domain.Forums;
using TvProgViewer.Core.Domain.Gdpr;
using TvProgViewer.Core.Domain.Media;
using TvProgViewer.Core.Domain.Orders;
using TvProgViewer.Core.Domain.Security;
using TvProgViewer.Core.Domain.Tax;
using TvProgViewer.Core.Domain.Vendors;
using TvProgViewer.Services.Authentication.External;
using TvProgViewer.Services.Authentication.MultiFactor;
using TvProgViewer.Services.Catalog;
using TvProgViewer.Services.Common;
using TvProgViewer.Services.Users;
using TvProgViewer.Services.Directory;
using TvProgViewer.Services.Gdpr;
using TvProgViewer.Services.Helpers;
using TvProgViewer.Services.Localization;
using TvProgViewer.Services.Media;
using TvProgViewer.Services.Messages;
using TvProgViewer.Services.Orders;
using TvProgViewer.Services.Security;
using TvProgViewer.Services.Seo;
using TvProgViewer.Services.Stores;
using TvProgViewer.WebUI.Models.Common;
using TvProgViewer.WebUI.Models.User;

namespace TvProgViewer.WebUI.Factories
{
    /// <summary>
    /// Represents the user model factory
    /// </summary>
    public partial class UserModelFactory : IUserModelFactory
    {
        #region Fields

        private readonly AddressSettings _addressSettings;
        private readonly CaptchaSettings _captchaSettings;
        private readonly CatalogSettings _catalogSettings;
        private readonly CommonSettings _commonSettings;
        private readonly UserSettings _userSettings;
        private readonly DateTimeSettings _dateTimeSettings;
        private readonly ExternalAuthenticationSettings _externalAuthenticationSettings;
        private readonly ForumSettings _forumSettings;
        private readonly GdprSettings _gdprSettings;
        private readonly IAddressModelFactory _addressModelFactory;
        private readonly IAuthenticationPluginManager _authenticationPluginManager;
        private readonly ICountryService _countryService;
        private readonly IUserAttributeParser _userAttributeParser;
        private readonly IUserAttributeService _userAttributeService;
        private readonly IUserService _userService;
        private readonly IDateTimeHelper _dateTimeHelper;
        private readonly IExternalAuthenticationService _externalAuthenticationService;
        private readonly IGdprService _gdprService;
        private readonly IGenericAttributeService _genericAttributeService;
        private readonly ILocalizationService _localizationService;
        private readonly IMultiFactorAuthenticationPluginManager _multiFactorAuthenticationPluginManager;
        private readonly INewsLetterSubscriptionService _newsLetterSubscriptionService;
        private readonly IOrderService _orderService;
        private readonly IPermissionService _permissionService;
        private readonly IPictureService _pictureService;
        private readonly ITvChannelService _tvchannelService;
        private readonly IReturnRequestService _returnRequestService;
        private readonly IStateProvinceService _stateProvinceService;
        private readonly IStoreContext _storeContext;
        private readonly IStoreMappingService _storeMappingService;
        private readonly IUrlRecordService _urlRecordService;
        private readonly IWorkContext _workContext;
        private readonly MediaSettings _mediaSettings;
        private readonly OrderSettings _orderSettings;
        private readonly RewardPointsSettings _rewardPointsSettings;
        private readonly SecuritySettings _securitySettings;
        private readonly TaxSettings _taxSettings;
        private readonly VendorSettings _vendorSettings;

        #endregion

        #region Ctor

        public UserModelFactory(AddressSettings addressSettings,
            CaptchaSettings captchaSettings,
            CatalogSettings catalogSettings,
            CommonSettings commonSettings,
            UserSettings userSettings,
            DateTimeSettings dateTimeSettings,
            ExternalAuthenticationSettings externalAuthenticationSettings,
            ForumSettings forumSettings,
            GdprSettings gdprSettings,
            IAddressModelFactory addressModelFactory,
            IAuthenticationPluginManager authenticationPluginManager,
            ICountryService countryService,
            IUserAttributeParser userAttributeParser,
            IUserAttributeService userAttributeService,
            IUserService userService,
            IDateTimeHelper dateTimeHelper,
            IExternalAuthenticationService externalAuthenticationService,
            IGdprService gdprService,
            IGenericAttributeService genericAttributeService,
            ILocalizationService localizationService,
            IMultiFactorAuthenticationPluginManager multiFactorAuthenticationPluginManager,
            INewsLetterSubscriptionService newsLetterSubscriptionService,
            IOrderService orderService,
            IPermissionService permissionService,
            IPictureService pictureService,
            ITvChannelService tvchannelService,
            IReturnRequestService returnRequestService,
            IStateProvinceService stateProvinceService,
            IStoreContext storeContext,
            IStoreMappingService storeMappingService,
            IUrlRecordService urlRecordService,
            IWorkContext workContext,
            MediaSettings mediaSettings,
            OrderSettings orderSettings,
            RewardPointsSettings rewardPointsSettings,
            SecuritySettings securitySettings,
            TaxSettings taxSettings,
            VendorSettings vendorSettings)
        {
            _addressSettings = addressSettings;
            _captchaSettings = captchaSettings;
            _catalogSettings = catalogSettings;
            _commonSettings = commonSettings;
            _userSettings = userSettings;
            _dateTimeSettings = dateTimeSettings;
            _externalAuthenticationService = externalAuthenticationService;
            _externalAuthenticationSettings = externalAuthenticationSettings;
            _forumSettings = forumSettings;
            _gdprSettings = gdprSettings;
            _addressModelFactory = addressModelFactory;
            _authenticationPluginManager = authenticationPluginManager;
            _countryService = countryService;
            _userAttributeParser = userAttributeParser;
            _userAttributeService = userAttributeService;
            _userService = userService;
            _dateTimeHelper = dateTimeHelper;
            _gdprService = gdprService;
            _genericAttributeService = genericAttributeService;
            _localizationService = localizationService;
            _multiFactorAuthenticationPluginManager = multiFactorAuthenticationPluginManager;
            _newsLetterSubscriptionService = newsLetterSubscriptionService;
            _orderService = orderService;
            _permissionService = permissionService;
            _pictureService = pictureService;
            _tvchannelService = tvchannelService;
            _returnRequestService = returnRequestService;
            _stateProvinceService = stateProvinceService;
            _storeContext = storeContext;
            _storeMappingService = storeMappingService;
            _urlRecordService = urlRecordService;
            _workContext = workContext;
            _mediaSettings = mediaSettings;
            _orderSettings = orderSettings;
            _rewardPointsSettings = rewardPointsSettings;
            _securitySettings = securitySettings;
            _taxSettings = taxSettings;
            _vendorSettings = vendorSettings;
        }

        #endregion

        #region Utilities

        /// <returns>Задача представляет асинхронную операцию</returns>
        protected virtual async Task<GdprConsentModel> PrepareGdprConsentModelAsync(GdprConsent consent, bool accepted)
        {
            if (consent == null)
                throw new ArgumentNullException(nameof(consent));

            var requiredMessage = await _localizationService.GetLocalizedAsync(consent, x => x.RequiredMessage);
            return new GdprConsentModel
            {
                Id = consent.Id,
                Message = await _localizationService.GetLocalizedAsync(consent, x => x.Message),
                IsRequired = consent.IsRequired,
                RequiredMessage = !string.IsNullOrEmpty(requiredMessage) ? requiredMessage : $"'{consent.Message}' is required",
                Accepted = accepted
            };
        }

        #endregion

        #region Methods

        /// <summary>
        /// Prepare the user info model
        /// </summary>
        /// <param name="model">User info model</param>
        /// <param name="user">User</param>
        /// <param name="excludeProperties">Whether to exclude populating of model properties from the entity</param>
        /// <param name="overrideCustomUserAttributesXml">Overridden user attributes in XML format; pass null to use CustomUserAttributes of user</param>
        /// <returns>
        /// Задача представляет асинхронную операцию
        /// The task result contains the user info model
        /// </returns>
        public virtual async Task<UserInfoModel> PrepareUserInfoModelAsync(UserInfoModel model, User user,
            bool excludeProperties, string overrideCustomUserAttributesXml = "")
        {
            if (model == null)
                throw new ArgumentNullException(nameof(model));

            if (user == null)
                throw new ArgumentNullException(nameof(user));

            model.AllowUsersToSetTimeZone = _dateTimeSettings.AllowUsersToSetTimeZone;
            foreach (var tzi in _dateTimeHelper.GetSystemTimeZones())
                model.AvailableTimeZones.Add(new SelectListItem { Text = tzi.DisplayName, Value = tzi.Id, Selected = (excludeProperties ? tzi.Id == model.GmtZone : tzi.Id == (await _dateTimeHelper.GetCurrentTimeZoneAsync()).Id) });

            var store = await _storeContext.GetCurrentStoreAsync();
            if (!excludeProperties)
            {
                model.VatNumber = user.VatNumber;
                model.FirstName = user.FirstName;
                model.LastName = user.LastName;
                model.MiddleName = user.MiddleName; 
                model.Gender = user.Gender;
                var birthDate = user.BirthDate;
                if (birthDate.HasValue)
                {
                    var currentCalendar = CultureInfo.CurrentCulture.Calendar;

                    model.BirthDateDay = currentCalendar.GetDayOfMonth(birthDate.Value);
                    model.BirthDateMonth = currentCalendar.GetMonth(birthDate.Value);
                    model.BirthDateYear = currentCalendar.GetYear(birthDate.Value);
                }
                model.Company = user.Company;
                model.StreetAddress = user.StreetAddress;
                model.StreetAddress2 = user.StreetAddress2;
                model.ZipPostalCode = user.ZipPostalCode;
                model.City = user.City;
                model.County = user.County;
                model.CountryId = user.CountryId;
                model.StateProvinceId = user.StateProvinceId;
                model.SmartPhone = user.SmartPhone;
                model.Fax = user.Fax;

                //newsletter
                var newsletter = await _newsLetterSubscriptionService.GetNewsLetterSubscriptionByEmailAndStoreIdAsync(user.Email, store.Id);
                model.Newsletter = newsletter != null && newsletter.Active;

                model.Signature = await _genericAttributeService.GetAttributeAsync<string>(user, TvProgUserDefaults.SignatureAttribute);

                model.Email = user.Email;
                model.Username = user.Username;
            }
            else
            {
                if (_userSettings.UsernamesEnabled && !_userSettings.AllowUsersToChangeUsernames)
                    model.Username = user.Username;
            }

            if (_userSettings.UserRegistrationType == UserRegistrationType.EmailValidation)
                model.EmailToRevalidate = user.EmailToRevalidate;

            var currentLanguage = await _workContext.GetWorkingLanguageAsync();
            //countries and states
            if (_userSettings.CountryEnabled)
            {
                model.AvailableCountries.Add(new SelectListItem { Text = await _localizationService.GetResourceAsync("Address.SelectCountry"), Value = "0" });
                foreach (var c in await _countryService.GetAllCountriesAsync(currentLanguage.Id))
                {
                    model.AvailableCountries.Add(new SelectListItem
                    {
                        Text = await _localizationService.GetLocalizedAsync(c, x => x.Name),
                        Value = c.Id.ToString(),
                        Selected = c.Id == model.CountryId
                    });
                }

                if (_userSettings.StateProvinceEnabled)
                {
                    //states
                    var states = (await _stateProvinceService.GetStateProvincesByCountryIdAsync(model.CountryId, currentLanguage.Id)).ToList();
                    if (states.Any())
                    {
                        model.AvailableStates.Add(new SelectListItem { Text = await _localizationService.GetResourceAsync("Address.SelectState"), Value = "0" });

                        foreach (var s in states)
                        {
                            model.AvailableStates.Add(new SelectListItem { Text = await _localizationService.GetLocalizedAsync(s, x => x.Name), Value = s.Id.ToString(), Selected = (s.Id == model.StateProvinceId) });
                        }
                    }
                    else
                    {
                        var anyCountrySelected = model.AvailableCountries.Any(x => x.Selected);

                        model.AvailableStates.Add(new SelectListItem
                        {
                            Text = await _localizationService.GetResourceAsync(anyCountrySelected ? "Address.Other" : "Address.SelectState"),
                            Value = "0"
                        });
                    }

                }
            }

            model.DisplayVatNumber = _taxSettings.EuVatEnabled;
            model.VatNumberStatusNote = await _localizationService.GetLocalizedEnumAsync(user.VatNumberStatus);
            model.FirstNameEnabled = _userSettings.FirstNameEnabled;
            model.LastNameEnabled = _userSettings.LastNameEnabled;
            model.MiddleNameEnabled = _userSettings.MiddleNameEnabled;
            model.FirstNameRequired = _userSettings.FirstNameRequired;
            model.LastNameRequired = _userSettings.LastNameRequired;
            model.GenderEnabled = _userSettings.GenderEnabled;
            model.BirthDateEnabled = _userSettings.BirthDateEnabled;
            model.BirthDateRequired = _userSettings.BirthDateRequired;
            model.CompanyEnabled = _userSettings.CompanyEnabled;
            model.CompanyRequired = _userSettings.CompanyRequired;
            model.StreetAddressEnabled = _userSettings.StreetAddressEnabled;
            model.StreetAddressRequired = _userSettings.StreetAddressRequired;
            model.StreetAddress2Enabled = _userSettings.StreetAddress2Enabled;
            model.StreetAddress2Required = _userSettings.StreetAddress2Required;
            model.ZipPostalCodeEnabled = _userSettings.ZipPostalCodeEnabled;
            model.ZipPostalCodeRequired = _userSettings.ZipPostalCodeRequired;
            model.CityEnabled = _userSettings.CityEnabled;
            model.CityRequired = _userSettings.CityRequired;
            model.CountyEnabled = _userSettings.CountyEnabled;
            model.CountyRequired = _userSettings.CountyRequired;
            model.CountryEnabled = _userSettings.CountryEnabled;
            model.CountryRequired = _userSettings.CountryRequired;
            model.StateProvinceEnabled = _userSettings.StateProvinceEnabled;
            model.StateProvinceRequired = _userSettings.StateProvinceRequired;
            model.SmartPhoneEnabled = _userSettings.SmartPhoneEnabled;
            model.SmartPhoneRequired = _userSettings.SmartPhoneRequired;
            model.FaxEnabled = _userSettings.FaxEnabled;
            model.FaxRequired = _userSettings.FaxRequired;
            model.NewsletterEnabled = _userSettings.NewsletterEnabled;
            model.UsernamesEnabled = _userSettings.UsernamesEnabled;
            model.AllowUsersToChangeUsernames = _userSettings.AllowUsersToChangeUsernames;
            model.CheckUsernameAvailabilityEnabled = _userSettings.CheckUsernameAvailabilityEnabled;
            model.SignatureEnabled = _forumSettings.ForumsEnabled && _forumSettings.SignaturesEnabled;

            //external authentication
            var currentUser = await _workContext.GetCurrentUserAsync();
            model.AllowUsersToRemoveAssociations = _externalAuthenticationSettings.AllowUsersToRemoveAssociations;
            model.NumberOfExternalAuthenticationProviders = (await _authenticationPluginManager
                .LoadActivePluginsAsync(currentUser, store.Id))
                .Count;
            foreach (var record in await _externalAuthenticationService.GetUserExternalAuthenticationRecordsAsync(user))
            {
                var authMethod = await _authenticationPluginManager
                    .LoadPluginBySystemNameAsync(record.ProviderSystemName, currentUser, store.Id);
                if (!_authenticationPluginManager.IsPluginActive(authMethod))
                    continue;

                model.AssociatedExternalAuthRecords.Add(new UserInfoModel.AssociatedExternalAuthModel
                {
                    Id = record.Id,
                    Email = record.Email,
                    ExternalIdentifier = !string.IsNullOrEmpty(record.ExternalDisplayIdentifier)
                        ? record.ExternalDisplayIdentifier : record.ExternalIdentifier,
                    AuthMethodName = await _localizationService.GetLocalizedFriendlyNameAsync(authMethod, currentLanguage.Id)
                });
            }

            //custom user attributes
            var customAttributes = await PrepareCustomUserAttributesAsync(user, overrideCustomUserAttributesXml);
            foreach (var attribute in customAttributes)
                model.UserAttributes.Add(attribute);

            //GDPR
            if (_gdprSettings.GdprEnabled)
            {
                var consents = (await _gdprService.GetAllConsentsAsync()).Where(consent => consent.DisplayOnUserInfoPage).ToList();
                foreach (var consent in consents)
                {
                    var accepted = await _gdprService.IsConsentAcceptedAsync(consent.Id, currentUser.Id);
                    model.GdprConsents.Add(await PrepareGdprConsentModelAsync(consent, accepted.HasValue && accepted.Value));
                }
            }
            return model;
        }

        /// <summary>
        /// Prepare the user register model
        /// </summary>
        /// <param name="model">User register model</param>
        /// <param name="excludeProperties">Whether to exclude populating of model properties from the entity</param>
        /// <param name="overrideCustomUserAttributesXml">Overridden user attributes in XML format; pass null to use CustomUserAttributes of user</param>
        /// <param name="setDefaultValues">Whether to populate model properties by default values</param>
        /// <returns>
        /// Задача представляет асинхронную операцию
        /// The task result contains the user register model
        /// </returns>
        public virtual async Task<RegisterModel> PrepareRegisterModelAsync(RegisterModel model, bool excludeProperties,
            string overrideCustomUserAttributesXml = "", bool setDefaultValues = false)
        {
            if (model == null)
                throw new ArgumentNullException(nameof(model));

            var user = await _workContext.GetCurrentUserAsync();

            model.AllowUsersToSetTimeZone = _dateTimeSettings.AllowUsersToSetTimeZone;
            foreach (var tzi in _dateTimeHelper.GetSystemTimeZones())
                model.AvailableTimeZones.Add(new SelectListItem { Text = tzi.DisplayName, Value = tzi.Id, Selected = (excludeProperties ? tzi.Id == model.GmtZone : tzi.Id == (await _dateTimeHelper.GetCurrentTimeZoneAsync()).Id) });

            //VAT
            model.DisplayVatNumber = _taxSettings.EuVatEnabled;
            if (_taxSettings.EuVatEnabled && _taxSettings.EuVatEnabledForGuests)
                model.VatNumber = user.VatNumber;

            //form fields
            model.FirstNameEnabled = _userSettings.FirstNameEnabled;
            model.LastNameEnabled = _userSettings.LastNameEnabled;
            model.MiddleNameEnabled = _userSettings.MiddleNameEnabled;
            model.FirstNameRequired = _userSettings.FirstNameRequired;
            model.LastNameRequired = _userSettings.LastNameRequired;
            model.GenderEnabled = _userSettings.GenderEnabled;
            model.BirthDateEnabled = _userSettings.BirthDateEnabled;
            model.BirthDateRequired = _userSettings.BirthDateRequired;
            model.CompanyEnabled = _userSettings.CompanyEnabled;
            model.CompanyRequired = _userSettings.CompanyRequired;
            model.StreetAddressEnabled = _userSettings.StreetAddressEnabled;
            model.StreetAddressRequired = _userSettings.StreetAddressRequired;
            model.StreetAddress2Enabled = _userSettings.StreetAddress2Enabled;
            model.StreetAddress2Required = _userSettings.StreetAddress2Required;
            model.ZipPostalCodeEnabled = _userSettings.ZipPostalCodeEnabled;
            model.ZipPostalCodeRequired = _userSettings.ZipPostalCodeRequired;
            model.CityEnabled = _userSettings.CityEnabled;
            model.CityRequired = _userSettings.CityRequired;
            model.CountyEnabled = _userSettings.CountyEnabled;
            model.CountyRequired = _userSettings.CountyRequired;
            model.CountryEnabled = _userSettings.CountryEnabled;
            model.CountryRequired = _userSettings.CountryRequired;
            model.StateProvinceEnabled = _userSettings.StateProvinceEnabled;
            model.StateProvinceRequired = _userSettings.StateProvinceRequired;
            model.SmartPhoneEnabled = _userSettings.SmartPhoneEnabled;
            model.SmartPhoneRequired = _userSettings.SmartPhoneRequired;
            model.FaxEnabled = _userSettings.FaxEnabled;
            model.FaxRequired = _userSettings.FaxRequired;
            model.NewsletterEnabled = _userSettings.NewsletterEnabled;
            model.AcceptPrivacyPolicyEnabled = _userSettings.AcceptPrivacyPolicyEnabled;
            model.AcceptPrivacyPolicyPopup = _commonSettings.PopupForTermsOfServiceLinks;
            model.UsernamesEnabled = _userSettings.UsernamesEnabled;
            model.CheckUsernameAvailabilityEnabled = _userSettings.CheckUsernameAvailabilityEnabled;
            model.HoneypotEnabled = _securitySettings.HoneypotEnabled;
            model.DisplayCaptcha = _captchaSettings.Enabled && _captchaSettings.ShowOnRegistrationPage;
            model.EnteringEmailTwice = _userSettings.EnteringEmailTwice;
            model.PersonalDataAgreementEnabled = _userSettings.AcceptPersonalDataAgreementEnabled;
            model.PersonalDataAgreementRequired = _userSettings.AcceptPersonalDataAgreementRequired;
            if (setDefaultValues)
            {
                //enable newsletter by default
                model.Newsletter = _userSettings.NewsletterTickedByDefault;
            }

            //countries and states
            if (_userSettings.CountryEnabled)
            {
                model.AvailableCountries.Add(new SelectListItem { Text = await _localizationService.GetResourceAsync("Address.SelectCountry"), Value = "0" });
                var currentLanguage = await _workContext.GetWorkingLanguageAsync();
                foreach (var c in await _countryService.GetAllCountriesAsync(currentLanguage.Id))
                {
                    model.AvailableCountries.Add(new SelectListItem
                    {
                        Text = await _localizationService.GetLocalizedAsync(c, x => x.Name),
                        Value = c.Id.ToString(),
                        Selected = c.Id == model.CountryId
                    });
                }

                if (_userSettings.StateProvinceEnabled)
                {
                    //states
                    var states = (await _stateProvinceService.GetStateProvincesByCountryIdAsync(model.CountryId, currentLanguage.Id)).ToList();
                    if (states.Any())
                    {
                        model.AvailableStates.Add(new SelectListItem { Text = await _localizationService.GetResourceAsync("Address.SelectState"), Value = "0" });

                        foreach (var s in states)
                        {
                            model.AvailableStates.Add(new SelectListItem { Text = await _localizationService.GetLocalizedAsync(s, x => x.Name), Value = s.Id.ToString(), Selected = (s.Id == model.StateProvinceId) });
                        }
                    }
                    else
                    {
                        var anyCountrySelected = model.AvailableCountries.Any(x => x.Selected);

                        model.AvailableStates.Add(new SelectListItem
                        {
                            Text = await _localizationService.GetResourceAsync(anyCountrySelected ? "Address.Other" : "Address.SelectState"),
                            Value = "0"
                        });
                    }

                }
            }

            //custom user attributes
            var customAttributes = await PrepareCustomUserAttributesAsync(user, overrideCustomUserAttributesXml);
            foreach (var attribute in customAttributes)
                model.UserAttributes.Add(attribute);

            //GDPR
            if (_gdprSettings.GdprEnabled)
            {
                var consents = (await _gdprService.GetAllConsentsAsync()).Where(consent => consent.DisplayDuringRegistration).ToList();
                foreach (var consent in consents)
                {
                    model.GdprConsents.Add(await PrepareGdprConsentModelAsync(consent, false));
                }
            }

            return model;
        }

        /// <summary>
        /// Prepare the login model
        /// </summary>
        /// <param name="checkoutAsGuest">Whether to checkout as guest is enabled</param>
        /// <returns>
        /// Задача представляет асинхронную операцию
        /// The task result contains the login model
        /// </returns>
        public virtual Task<LoginModel> PrepareLoginModelAsync(bool? checkoutAsGuest)
        {
            var model = new LoginModel
            {
                UsernamesEnabled = _userSettings.UsernamesEnabled,
                RegistrationType = _userSettings.UserRegistrationType,
                CheckoutAsGuest = checkoutAsGuest.GetValueOrDefault(),
                DisplayCaptcha = _captchaSettings.Enabled && _captchaSettings.ShowOnLoginPage
            };

            return Task.FromResult(model);
        }

        /// <summary>
        /// Prepare the password recovery model
        /// </summary>
        /// <param name="model">Password recovery model</param>
        /// <returns>
        /// Задача представляет асинхронную операцию
        /// The task result contains the password recovery model
        /// </returns>
        public virtual async Task<PasswordRecoveryModel> PreparePasswordRecoveryModelAsync(PasswordRecoveryModel model)
        {
            if (model == null)
                throw new ArgumentNullException(nameof(model));

            model.DisplayCaptcha = _captchaSettings.Enabled && _captchaSettings.ShowOnForgotPasswordPage;
            model.MetaKeywords = await _localizationService.GetResourceAsync("PageTitle.PasswordRecovery.MetaKeywords");
            model.MetaDescription = await _localizationService.GetResourceAsync("PageTitle.PasswordRecovery.MetaDescription");

            return model;
        }

        /// <summary>
        /// Prepare the register result model
        /// </summary>
        /// <param name="resultId">Value of UserRegistrationType enum</param>
        /// <param name="returnUrl">URL to redirect</param>
        /// <returns>
        /// Задача представляет асинхронную операцию
        /// The task result contains the register result model
        /// </returns>
        public virtual async Task<RegisterResultModel> PrepareRegisterResultModelAsync(int resultId, string returnUrl)
        {
            var resultText = (UserRegistrationType)resultId switch
            {
                UserRegistrationType.Disabled => await _localizationService.GetResourceAsync("Account.Register.Result.Disabled"),
                UserRegistrationType.Standard => await _localizationService.GetResourceAsync("Account.Register.Result.Standard"),
                UserRegistrationType.AdminApproval => await _localizationService.GetResourceAsync("Account.Register.Result.AdminApproval"),
                UserRegistrationType.EmailValidation => await _localizationService.GetResourceAsync("Account.Register.Result.EmailValidation"),
                _ => null
            };

            var model = new RegisterResultModel
            {
                Result = resultText,
                ReturnUrl = returnUrl
            };

            return model;
        }

        /// <summary>
        /// Prepare the user navigation model
        /// </summary>
        /// <param name="selectedTabId">Identifier of the selected tab</param>
        /// <returns>
        /// Задача представляет асинхронную операцию
        /// The task result contains the user navigation model
        /// </returns>
        public virtual async Task<UserNavigationModel> PrepareUserNavigationModelAsync(int selectedTabId = 0)
        {
            var model = new UserNavigationModel();

            model.UserNavigationItems.Add(new UserNavigationItemModel
            {
                RouteName = "UserInfo",
                Title = await _localizationService.GetResourceAsync("Account.UserInfo"),
                Tab = (int)UserNavigationEnum.Info,
                ItemClass = "user-info"
            });

            model.UserNavigationItems.Add(new UserNavigationItemModel
            {
                RouteName = "UserAddresses",
                Title = await _localizationService.GetResourceAsync("Account.UserAddresses"),
                Tab = (int)UserNavigationEnum.Addresses,
                ItemClass = "user-addresses"
            });

            model.UserNavigationItems.Add(new UserNavigationItemModel
            {
                RouteName = "UserOrders",
                Title = await _localizationService.GetResourceAsync("Account.UserOrders"),
                Tab = (int)UserNavigationEnum.Orders,
                ItemClass = "user-orders"
            });

            var store = await _storeContext.GetCurrentStoreAsync();
            var user = await _workContext.GetCurrentUserAsync();

            if (_orderSettings.ReturnRequestsEnabled &&
                (await _returnRequestService.SearchReturnRequestsAsync(store.Id,
                    user.Id, pageIndex: 0, pageSize: 1)).Any())
            {
                model.UserNavigationItems.Add(new UserNavigationItemModel
                {
                    RouteName = "UserReturnRequests",
                    Title = await _localizationService.GetResourceAsync("Account.UserReturnRequests"),
                    Tab = (int)UserNavigationEnum.ReturnRequests,
                    ItemClass = "return-requests"
                });
            }

            if (!_userSettings.HideDownloadableTvChannelsTab)
            {
                model.UserNavigationItems.Add(new UserNavigationItemModel
                {
                    RouteName = "UserDownloadableTvChannels",
                    Title = await _localizationService.GetResourceAsync("Account.DownloadableTvChannels"),
                    Tab = (int)UserNavigationEnum.DownloadableTvChannels,
                    ItemClass = "downloadable-tvchannels"
                });
            }

            if (!_userSettings.HideBackInStockSubscriptionsTab)
            {
                model.UserNavigationItems.Add(new UserNavigationItemModel
                {
                    RouteName = "UserBackInStockSubscriptions",
                    Title = await _localizationService.GetResourceAsync("Account.BackInStockSubscriptions"),
                    Tab = (int)UserNavigationEnum.BackInStockSubscriptions,
                    ItemClass = "back-in-stock-subscriptions"
                });
            }

            if (_rewardPointsSettings.Enabled)
            {
                model.UserNavigationItems.Add(new UserNavigationItemModel
                {
                    RouteName = "UserRewardPoints",
                    Title = await _localizationService.GetResourceAsync("Account.RewardPoints"),
                    Tab = (int)UserNavigationEnum.RewardPoints,
                    ItemClass = "reward-points"
                });
            }

            model.UserNavigationItems.Add(new UserNavigationItemModel
            {
                RouteName = "UserChangePassword",
                Title = await _localizationService.GetResourceAsync("Account.ChangePassword"),
                Tab = (int)UserNavigationEnum.ChangePassword,
                ItemClass = "change-password"
            });

            if (_userSettings.AllowUsersToUploadAvatars)
            {
                model.UserNavigationItems.Add(new UserNavigationItemModel
                {
                    RouteName = "UserAvatar",
                    Title = await _localizationService.GetResourceAsync("Account.Avatar"),
                    Tab = (int)UserNavigationEnum.Avatar,
                    ItemClass = "user-avatar"
                });
            }

            if (_forumSettings.ForumsEnabled && _forumSettings.AllowUsersToManageSubscriptions)
            {
                model.UserNavigationItems.Add(new UserNavigationItemModel
                {
                    RouteName = "UserForumSubscriptions",
                    Title = await _localizationService.GetResourceAsync("Account.ForumSubscriptions"),
                    Tab = (int)UserNavigationEnum.ForumSubscriptions,
                    ItemClass = "forum-subscriptions"
                });
            }
            if (_catalogSettings.ShowTvChannelReviewsTabOnAccountPage)
            {
                model.UserNavigationItems.Add(new UserNavigationItemModel
                {
                    RouteName = "UserTvChannelReviews",
                    Title = await _localizationService.GetResourceAsync("Account.UserTvChannelReviews"),
                    Tab = (int)UserNavigationEnum.TvChannelReviews,
                    ItemClass = "user-reviews"
                });
            }
            if (_vendorSettings.AllowVendorsToEditInfo && await _workContext.GetCurrentVendorAsync() != null)
            {
                model.UserNavigationItems.Add(new UserNavigationItemModel
                {
                    RouteName = "UserVendorInfo",
                    Title = await _localizationService.GetResourceAsync("Account.VendorInfo"),
                    Tab = (int)UserNavigationEnum.VendorInfo,
                    ItemClass = "user-vendor-info"
                });
            }
            if (_gdprSettings.GdprEnabled)
            {
                model.UserNavigationItems.Add(new UserNavigationItemModel
                {
                    RouteName = "GdprTools",
                    Title = await _localizationService.GetResourceAsync("Account.Gdpr"),
                    Tab = (int)UserNavigationEnum.GdprTools,
                    ItemClass = "user-gdpr"
                });
            }

            if (_captchaSettings.Enabled && _userSettings.AllowUsersToCheckGiftCardBalance)
            {
                model.UserNavigationItems.Add(new UserNavigationItemModel
                {
                    RouteName = "CheckGiftCardBalance",
                    Title = await _localizationService.GetResourceAsync("CheckGiftCardBalance"),
                    Tab = (int)UserNavigationEnum.CheckGiftCardBalance,
                    ItemClass = "user-check-gift-card-balance"
                });
            }

            if (await _permissionService.AuthorizeAsync(StandardPermissionProvider.EnableMultiFactorAuthentication) &&
                await _multiFactorAuthenticationPluginManager.HasActivePluginsAsync())
            {
                model.UserNavigationItems.Add(new UserNavigationItemModel
                {
                    RouteName = "MultiFactorAuthenticationSettings",
                    Title = await _localizationService.GetResourceAsync("PageTitle.MultiFactorAuthentication"),
                    Tab = (int)UserNavigationEnum.MultiFactorAuthentication,
                    ItemClass = "user-multiFactor-authentication"
                });
            }

            model.SelectedTab = selectedTabId;

            return model;
        }

        /// <summary>
        /// Prepare the user address list model
        /// </summary>
        /// <returns>
        /// Задача представляет асинхронную операцию
        /// The task result contains the user address list model
        /// </returns>
        public virtual async Task<UserAddressListModel> PrepareUserAddressListModelAsync()
        {
            var user = await _workContext.GetCurrentUserAsync();

            var addresses = await (await _userService.GetAddressesByUserIdAsync(user.Id))
                //enabled for the current store
                .WhereAwait(async a => a.CountryId == null || await _storeMappingService.AuthorizeAsync(await _countryService.GetCountryByAddressAsync(a)))
                .ToListAsync();

            var model = new UserAddressListModel();
            foreach (var address in addresses)
            {
                var addressModel = new AddressModel();
                await _addressModelFactory.PrepareAddressModelAsync(addressModel,
                    address: address,
                    excludeProperties: false,
                    addressSettings: _addressSettings,
                    loadCountries: async () => await _countryService.GetAllCountriesAsync((await _workContext.GetWorkingLanguageAsync()).Id));
                model.Addresses.Add(addressModel);
            }
            return model;
        }

        /// <summary>
        /// Prepare the user downloadable tvchannels model
        /// </summary>
        /// <returns>
        /// Задача представляет асинхронную операцию
        /// The task result contains the user downloadable tvchannels model
        /// </returns>
        public virtual async Task<UserDownloadableTvChannelsModel> PrepareUserDownloadableTvChannelsModelAsync()
        {
            var model = new UserDownloadableTvChannelsModel();
            var user = await _workContext.GetCurrentUserAsync();
            var items = await _orderService.GetDownloadableOrderItemsAsync(user.Id);
            foreach (var item in items)
            {
                var order = await _orderService.GetOrderByIdAsync(item.OrderId);
                var tvchannel = await _tvchannelService.GetTvChannelByIdAsync(item.TvChannelId);

                var itemModel = new UserDownloadableTvChannelsModel.DownloadableTvChannelsModel
                {
                    OrderItemGuid = item.OrderItemGuid,
                    OrderId = order.Id,
                    CustomOrderNumber = order.CustomOrderNumber,
                    CreatedOn = await _dateTimeHelper.ConvertToUserTimeAsync(order.CreatedOnUtc, DateTimeKind.Utc),
                    TvChannelName = await _localizationService.GetLocalizedAsync(tvchannel, x => x.Name),
                    TvChannelSeName = await _urlRecordService.GetSeNameAsync(tvchannel),
                    TvChannelAttributes = item.AttributeDescription,
                    TvChannelId = item.TvChannelId
                };
                model.Items.Add(itemModel);

                if (await _orderService.IsDownloadAllowedAsync(item))
                    itemModel.DownloadId = tvchannel.DownloadId;

                if (await _orderService.IsLicenseDownloadAllowedAsync(item))
                    itemModel.LicenseId = item.LicenseDownloadId ?? 0;
            }

            return model;
        }

        /// <summary>
        /// Prepare the user agreement model
        /// </summary>
        /// <param name="orderItem">Order item</param>
        /// <param name="tvchannel">TvChannel</param>
        /// <returns>
        /// Задача представляет асинхронную операцию
        /// The task result contains the user agreement model
        /// </returns>
        public virtual Task<UserAgreementModel> PrepareUserAgreementModelAsync(OrderItem orderItem, TvChannel tvchannel)
        {
            if (orderItem == null)
                throw new ArgumentNullException(nameof(orderItem));

            if (tvchannel == null)
                throw new ArgumentNullException(nameof(tvchannel));

            var model = new UserAgreementModel
            {
                UserAgreementText = tvchannel.UserAgreementText,
                OrderItemGuid = orderItem.OrderItemGuid
            };

            return Task.FromResult(model);
        }

        /// <summary>
        /// Prepare the change password model
        /// </summary>
        /// <returns>
        /// Задача представляет асинхронную операцию
        /// The task result contains the change password model
        /// </returns>
        public virtual Task<ChangePasswordModel> PrepareChangePasswordModelAsync()
        {
            var model = new ChangePasswordModel();

            return Task.FromResult(model);
        }

        /// <summary>
        /// Prepare the user avatar model
        /// </summary>
        /// <param name="model">User avatar model</param>
        /// <returns>
        /// Задача представляет асинхронную операцию
        /// The task result contains the user avatar model
        /// </returns>
        public virtual async Task<UserAvatarModel> PrepareUserAvatarModelAsync(UserAvatarModel model)
        {
            if (model == null)
                throw new ArgumentNullException(nameof(model));

            model.AvatarUrl = await _pictureService.GetPictureUrlAsync(
                await _genericAttributeService.GetAttributeAsync<int>(await _workContext.GetCurrentUserAsync(), TvProgUserDefaults.AvatarPictureIdAttribute),
                _mediaSettings.AvatarPictureSize,
                false);

            return model;
        }

        /// <summary>
        /// Prepare the GDPR tools model
        /// </summary>
        /// <returns>
        /// Задача представляет асинхронную операцию
        /// The task result contains the gDPR tools model
        /// </returns>
        public virtual Task<GdprToolsModel> PrepareGdprToolsModelAsync()
        {
            var model = new GdprToolsModel();

            return Task.FromResult(model);
        }

        /// <summary>
        /// Prepare the check gift card balance madel
        /// </summary>
        /// <returns>
        /// Задача представляет асинхронную операцию
        /// The task result contains the check gift card balance madel
        /// </returns>
        public virtual Task<CheckGiftCardBalanceModel> PrepareCheckGiftCardBalanceModelAsync()
        {
            var model = new CheckGiftCardBalanceModel();

            return Task.FromResult(model);
        }

        /// <summary>
        /// Prepare the multi-factor authentication model
        /// </summary>
        /// <param name="model">Multi-factor authentication model</param>
        /// <returns>
        /// Задача представляет асинхронную операцию
        /// The task result contains the multi-factor authentication model
        /// </returns>
        public virtual async Task<MultiFactorAuthenticationModel> PrepareMultiFactorAuthenticationModelAsync(MultiFactorAuthenticationModel model)
        {
            var user = await _workContext.GetCurrentUserAsync();

            model.IsEnabled = !string.IsNullOrEmpty(
                await _genericAttributeService.GetAttributeAsync<string>(user, TvProgUserDefaults.SelectedMultiFactorAuthenticationProviderAttribute));

            var store = await _storeContext.GetCurrentStoreAsync();
            var multiFactorAuthenticationProviders = (await _multiFactorAuthenticationPluginManager.LoadActivePluginsAsync(user, store.Id)).ToList();
            foreach (var multiFactorAuthenticationProvider in multiFactorAuthenticationProviders)
            {
                var providerModel = new MultiFactorAuthenticationProviderModel();
                var sysName = multiFactorAuthenticationProvider.PluginDescriptor.SystemName;
                providerModel = await PrepareMultiFactorAuthenticationProviderModelAsync(providerModel, sysName);
                model.Providers.Add(providerModel);
            }

            return model;
        }

        /// <summary>
        /// Prepare the multi-factor authentication provider model
        /// </summary>
        /// <param name="providerModel">Multi-factor authentication provider model</param>
        /// <param name="sysName">Multi-factor authentication provider system name</param>
        /// <returns>
        /// Задача представляет асинхронную операцию
        /// The task result contains the multi-factor authentication model
        /// </returns>
        public virtual async Task<MultiFactorAuthenticationProviderModel> PrepareMultiFactorAuthenticationProviderModelAsync(MultiFactorAuthenticationProviderModel providerModel, string sysName, bool isLogin = false)
        {
            var user = await _workContext.GetCurrentUserAsync();
            var selectedProvider = await _genericAttributeService.GetAttributeAsync<string>(user, TvProgUserDefaults.SelectedMultiFactorAuthenticationProviderAttribute);
            var store = await _storeContext.GetCurrentStoreAsync();

            var multiFactorAuthenticationProvider = (await _multiFactorAuthenticationPluginManager.LoadActivePluginsAsync(user, store.Id))
                    .FirstOrDefault(provider => provider.PluginDescriptor.SystemName == sysName);

            if (multiFactorAuthenticationProvider != null)
            {
                providerModel.Name = await _localizationService.GetLocalizedFriendlyNameAsync(multiFactorAuthenticationProvider, (await _workContext.GetWorkingLanguageAsync()).Id);
                providerModel.SystemName = sysName;
                providerModel.Description = await multiFactorAuthenticationProvider.GetDescriptionAsync();
                providerModel.LogoUrl = await _multiFactorAuthenticationPluginManager.GetPluginLogoUrlAsync(multiFactorAuthenticationProvider);
                providerModel.ViewComponent = isLogin ? multiFactorAuthenticationProvider.GetVerificationViewComponent() : multiFactorAuthenticationProvider.GetPublicViewComponent();
                providerModel.Selected = sysName == selectedProvider;
            }

            return providerModel;
        }

        /// <summary>
        /// Prepare the custom user attribute models
        /// </summary>
        /// <param name="user">User</param>
        /// <param name="overrideAttributesXml">Overridden user attributes in XML format; pass null to use CustomUserAttributes of user</param>
        /// <returns>
        /// Задача представляет асинхронную операцию
        /// The task result contains the list of the user attribute model
        /// </returns>
        public virtual async Task<IList<UserAttributeModel>> PrepareCustomUserAttributesAsync(User user, string overrideAttributesXml = "")
        {
            if (user == null)
                throw new ArgumentNullException(nameof(user));

            var result = new List<UserAttributeModel>();

            var userAttributes = await _userAttributeService.GetAllUserAttributesAsync();
            foreach (var attribute in userAttributes)
            {
                var attributeModel = new UserAttributeModel
                {
                    Id = attribute.Id,
                    Name = await _localizationService.GetLocalizedAsync(attribute, x => x.Name),
                    IsRequired = attribute.IsRequired,
                    AttributeControlType = attribute.AttributeControlType,
                };

                if (attribute.ShouldHaveValues())
                {
                    //values
                    var attributeValues = await _userAttributeService.GetUserAttributeValuesAsync(attribute.Id);
                    foreach (var attributeValue in attributeValues)
                    {
                        var valueModel = new UserAttributeValueModel
                        {
                            Id = attributeValue.Id,
                            Name = await _localizationService.GetLocalizedAsync(attributeValue, x => x.Name),
                            IsPreSelected = attributeValue.IsPreSelected
                        };
                        attributeModel.Values.Add(valueModel);
                    }
                }

                //set already selected attributes
                var selectedAttributesXml = !string.IsNullOrEmpty(overrideAttributesXml) ?
                    overrideAttributesXml :
                    user.CustomUserAttributesXML;
                switch (attribute.AttributeControlType)
                {
                    case AttributeControlType.DropdownList:
                    case AttributeControlType.RadioList:
                    case AttributeControlType.Checkboxes:
                        {
                            if (!string.IsNullOrEmpty(selectedAttributesXml))
                            {
                                if (!_userAttributeParser.ParseValues(selectedAttributesXml, attribute.Id).Any())
                                    break;

                                //clear default selection                                
                                foreach (var item in attributeModel.Values)
                                    item.IsPreSelected = false;

                                //select new values
                                var selectedValues = await _userAttributeParser.ParseUserAttributeValuesAsync(selectedAttributesXml);
                                foreach (var attributeValue in selectedValues)
                                    foreach (var item in attributeModel.Values)
                                        if (attributeValue.Id == item.Id)
                                            item.IsPreSelected = true;
                            }
                        }
                        break;
                    case AttributeControlType.ReadonlyCheckboxes:
                        {
                            //do nothing
                            //values are already pre-set
                        }
                        break;
                    case AttributeControlType.TextBox:
                    case AttributeControlType.MultilineTextbox:
                        {
                            if (!string.IsNullOrEmpty(selectedAttributesXml))
                            {
                                var enteredText = _userAttributeParser.ParseValues(selectedAttributesXml, attribute.Id);
                                if (enteredText.Any())
                                    attributeModel.DefaultValue = enteredText[0];
                            }
                        }
                        break;
                    case AttributeControlType.ColorSquares:
                    case AttributeControlType.ImageSquares:
                    case AttributeControlType.Datepicker:
                    case AttributeControlType.FileUpload:
                    default:
                        //not supported attribute control types
                        break;
                }

                result.Add(attributeModel);
            }

            return result;
        }

        #endregion
    }
}