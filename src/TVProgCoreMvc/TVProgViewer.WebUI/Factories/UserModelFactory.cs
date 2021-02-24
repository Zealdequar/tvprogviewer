using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc.Rendering;
using TVProgViewer.Core;
using TVProgViewer.Core.Domain.Catalog;
using TVProgViewer.Core.Domain.Common;
using TVProgViewer.Core.Domain.Users;
using TVProgViewer.Core.Domain.Forums;
using TVProgViewer.Core.Domain.Gdpr;
using TVProgViewer.Core.Domain.Media;
using TVProgViewer.Core.Domain.Orders;
using TVProgViewer.Core.Domain.Security;
using TVProgViewer.Core.Domain.Tax;
using TVProgViewer.Core.Domain.Vendors;
using TVProgViewer.Services.Authentication.External;
using TVProgViewer.Services.Catalog;
using TVProgViewer.Services.Common;
using TVProgViewer.Services.Users;
using TVProgViewer.Services.Directory;
using TVProgViewer.Services.Gdpr;
using TVProgViewer.Services.Helpers;
using TVProgViewer.Services.Localization;
using TVProgViewer.Services.Media;
using TVProgViewer.Services.Messages;
using TVProgViewer.Services.Orders;
using TVProgViewer.Services.Seo;
using TVProgViewer.Services.Stores;
using TVProgViewer.WebUI.Models.Common;
using TVProgViewer.WebUI.Models.User;
using System.Threading.Tasks;
using TVProgViewer.Services.Authentication.MultiFactor;

namespace TVProgViewer.WebUI.Factories
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
        private readonly IPictureService _pictureService;
        private readonly IProductService _productService;
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
            IPictureService pictureService,
            IProductService productService,
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
            _pictureService = pictureService;
            _productService = productService;
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

        /// <summary>
        /// Prepare the custom user attribute models
        /// </summary>
        /// <param name="user">User</param>
        /// <param name="overrideAttributesXml">Overridden user attributes in XML format; pass null to use CustomUserAttributes of user</param>
        /// <returns>List of the user attribute model</returns>
        protected virtual async Task<IList<UserAttributeModel>> PrepareCustomUserAttributesAsync(User user, string overrideAttributesXml = "")
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
                    await _genericAttributeService.GetAttributeAsync<string>(user, TvProgUserDefaults.CustomUserAttributes);
                switch (attribute.AttributeControlType)
                {
                    case AttributeControlType.DropdownList:
                    case AttributeControlType.RadioList:
                    case AttributeControlType.Checkboxes:
                        {
                            if (!string.IsNullOrEmpty(selectedAttributesXml))
                            {
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

        #region Methods

        /// <summary>
        /// Prepare the user info model
        /// </summary>
        /// <param name="model">User info model</param>
        /// <param name="user">User</param>
        /// <param name="excludeProperties">Whether to exclude populating of model properties from the entity</param>
        /// <param name="overrideCustomUserAttributesXml">Overridden user attributes in XML format; pass null to use CustomUserAttributes of user</param>
        /// <returns>User info model</returns>
        public virtual async Task<UserInfoModel> PrepareUserInfoModelAsync(UserInfoModel model, User user,
            bool excludeProperties, string overrideCustomUserAttributesXml = "")
        {
            if (model == null)
                throw new ArgumentNullException(nameof(model));

            if (user == null)
                throw new ArgumentNullException(nameof(user));

            model.AllowUsersToSetTimeZone = _dateTimeSettings.AllowUsersToSetTimeZone;
            foreach (var tzi in _dateTimeHelper.GetSystemTimeZones())
                model.AvailableTimeZones.Add(new SelectListItem { Text = tzi.DisplayName, Value = tzi.Id, Selected = (excludeProperties ? tzi.Id == model.TimeZoneId : tzi.Id == (await _dateTimeHelper.GetCurrentTimeZoneAsync()).Id) });

            if (!excludeProperties)
            {
                model.VatNumber = await _genericAttributeService.GetAttributeAsync<string>(user, TvProgUserDefaults.VatNumberAttribute);
                model.FirstName = await _genericAttributeService.GetAttributeAsync<string>(user, TvProgUserDefaults.FirstNameAttribute);
                model.LastName = await _genericAttributeService.GetAttributeAsync<string>(user, TvProgUserDefaults.LastNameAttribute);
                model.Gender = await _genericAttributeService.GetAttributeAsync<string>(user, TvProgUserDefaults.GenderAttribute);
                var dateOfBirth = await _genericAttributeService.GetAttributeAsync<DateTime?>(user, TvProgUserDefaults.DateOfBirthAttribute);
                if (dateOfBirth.HasValue)
                {
                    model.DateOfBirthDay = dateOfBirth.Value.Day;
                    model.DateOfBirthMonth = dateOfBirth.Value.Month;
                    model.DateOfBirthYear = dateOfBirth.Value.Year;
                }
                model.Company = await _genericAttributeService.GetAttributeAsync<string>(user, TvProgUserDefaults.CompanyAttribute);
                model.StreetAddress = await _genericAttributeService.GetAttributeAsync<string>(user, TvProgUserDefaults.StreetAddressAttribute);
                model.StreetAddress2 = await _genericAttributeService.GetAttributeAsync<string>(user, TvProgUserDefaults.StreetAddress2Attribute);
                model.ZipPostalCode = await _genericAttributeService.GetAttributeAsync<string>(user, TvProgUserDefaults.ZipPostalCodeAttribute);
                model.City = await _genericAttributeService.GetAttributeAsync<string>(user, TvProgUserDefaults.CityAttribute);
                model.County = await _genericAttributeService.GetAttributeAsync<string>(user, TvProgUserDefaults.CountyAttribute);
                model.CountryId = await _genericAttributeService.GetAttributeAsync<int>(user, TvProgUserDefaults.CountryIdAttribute);
                model.StateProvinceId = await _genericAttributeService.GetAttributeAsync<int>(user, TvProgUserDefaults.StateProvinceIdAttribute);
                model.Phone = await _genericAttributeService.GetAttributeAsync<string>(user, TvProgUserDefaults.PhoneAttribute);
                model.Fax = await _genericAttributeService.GetAttributeAsync<string>(user, TvProgUserDefaults.FaxAttribute);

                //newsletter
                var newsletter = await _newsLetterSubscriptionService.GetNewsLetterSubscriptionByEmailAndStoreIdAsync(user.Email, (await _storeContext.GetCurrentStoreAsync()).Id);
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

            //countries and states
            if (_userSettings.CountryEnabled)
            {
                model.AvailableCountries.Add(new SelectListItem { Text = await _localizationService.GetResourceAsync("Address.SelectCountry"), Value = "0" });
                foreach (var c in await _countryService.GetAllCountriesAsync((await _workContext.GetWorkingLanguageAsync()).Id))
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
                    var states = (await _stateProvinceService.GetStateProvincesByCountryIdAsync(model.CountryId, (await _workContext.GetWorkingLanguageAsync()).Id)).ToList();
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
            model.VatNumberStatusNote = await _localizationService.GetLocalizedEnumAsync((VatNumberStatus)await _genericAttributeService
                .GetAttributeAsync<int>(user, TvProgUserDefaults.VatNumberStatusIdAttribute));
            model.FirstNameEnabled = _userSettings.FirstNameEnabled;
            model.LastNameEnabled = _userSettings.LastNameEnabled;
            model.FirstNameRequired = _userSettings.FirstNameRequired;
            model.LastNameRequired = _userSettings.LastNameRequired;
            model.GenderEnabled = _userSettings.GenderEnabled;
            model.DateOfBirthEnabled = _userSettings.DateOfBirthEnabled;
            model.DateOfBirthRequired = _userSettings.DateOfBirthRequired;
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
            model.PhoneEnabled = _userSettings.PhoneEnabled;
            model.PhoneRequired = _userSettings.PhoneRequired;
            model.FaxEnabled = _userSettings.FaxEnabled;
            model.FaxRequired = _userSettings.FaxRequired;
            model.NewsletterEnabled = _userSettings.NewsletterEnabled;
            model.UsernamesEnabled = _userSettings.UsernamesEnabled;
            model.AllowUsersToChangeUsernames = _userSettings.AllowUsersToChangeUsernames;
            model.CheckUsernameAvailabilityEnabled = _userSettings.CheckUsernameAvailabilityEnabled;
            model.SignatureEnabled = _forumSettings.ForumsEnabled && _forumSettings.SignaturesEnabled;

            //external authentication
            model.AllowUsersToRemoveAssociations = _externalAuthenticationSettings.AllowUsersToRemoveAssociations;
            model.NumberOfExternalAuthenticationProviders = (await _authenticationPluginManager
                .LoadActivePluginsAsync(await _workContext.GetCurrentUserAsync(), (await _storeContext.GetCurrentStoreAsync()).Id))
                .Count;
            foreach (var record in await _externalAuthenticationService.GetUserExternalAuthenticationRecordsAsync(user))
            {
                var authMethod = await _authenticationPluginManager
                    .LoadPluginBySystemNameAsync(record.ProviderSystemName, await _workContext.GetCurrentUserAsync(), (await _storeContext.GetCurrentStoreAsync()).Id);
                if (!_authenticationPluginManager.IsPluginActive(authMethod))
                    continue;

                model.AssociatedExternalAuthRecords.Add(new UserInfoModel.AssociatedExternalAuthModel
                {
                    Id = record.Id,
                    Email = record.Email,
                    ExternalIdentifier = !string.IsNullOrEmpty(record.ExternalDisplayIdentifier)
                        ? record.ExternalDisplayIdentifier : record.ExternalIdentifier,
                    AuthMethodName = await _localizationService.GetLocalizedFriendlyNameAsync(authMethod, (await _workContext.GetWorkingLanguageAsync()).Id)
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
                    var accepted = await _gdprService.IsConsentAcceptedAsync(consent.Id, (await _workContext.GetCurrentUserAsync()).Id);
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
        /// <returns>User register model</returns>
        public virtual async Task<RegisterModel> PrepareRegisterModelAsync(RegisterModel model, bool excludeProperties,
            string overrideCustomUserAttributesXml = "", bool setDefaultValues = false)
        {
            if (model == null)
                throw new ArgumentNullException(nameof(model));

            model.AllowUsersToSetTimeZone = _dateTimeSettings.AllowUsersToSetTimeZone;
            foreach (var tzi in _dateTimeHelper.GetSystemTimeZones())
                model.AvailableTimeZones.Add(new SelectListItem { Text = tzi.DisplayName, Value = tzi.Id, Selected = (excludeProperties ? tzi.Id == model.TimeZoneId : tzi.Id == (await _dateTimeHelper.GetCurrentTimeZoneAsync()).Id) });

            model.DisplayVatNumber = _taxSettings.EuVatEnabled;
            //form fields
            model.FirstNameEnabled = _userSettings.FirstNameEnabled;
            model.LastNameEnabled = _userSettings.LastNameEnabled;
            model.FirstNameRequired = _userSettings.FirstNameRequired;
            model.LastNameRequired = _userSettings.LastNameRequired;
            model.GenderEnabled = _userSettings.GenderEnabled;
            model.DateOfBirthEnabled = _userSettings.DateOfBirthEnabled;
            model.DateOfBirthRequired = _userSettings.DateOfBirthRequired;
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
            model.PhoneEnabled = _userSettings.PhoneEnabled;
            model.PhoneRequired = _userSettings.PhoneRequired;
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
            if (setDefaultValues)
            {
                //enable newsletter by default
                model.Newsletter = _userSettings.NewsletterTickedByDefault;
            }

            //countries and states
            if (_userSettings.CountryEnabled)
            {
                model.AvailableCountries.Add(new SelectListItem { Text = await _localizationService.GetResourceAsync("Address.SelectCountry"), Value = "0" });

                foreach (var c in await _countryService.GetAllCountriesAsync((await _workContext.GetWorkingLanguageAsync()).Id))
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
                    var states = (await _stateProvinceService.GetStateProvincesByCountryIdAsync(model.CountryId, (await _workContext.GetWorkingLanguageAsync()).Id)).ToList();
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
            var customAttributes = await PrepareCustomUserAttributesAsync(await _workContext.GetCurrentUserAsync(), overrideCustomUserAttributesXml);
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
        /// <returns>Login model</returns>
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
        /// <returns>Password recovery model</returns>
        public virtual Task<PasswordRecoveryModel> PreparePasswordRecoveryModelAsync(PasswordRecoveryModel model)
        {
            if (model == null)
                throw new ArgumentNullException(nameof(model));

            model.DisplayCaptcha = _captchaSettings.Enabled && _captchaSettings.ShowOnForgotPasswordPage;

            return Task.FromResult(model);
        }

        /// <summary>
        /// Prepare the register result model
        /// </summary>
        /// <param name="resultId">Value of UserRegistrationType enum</param>
        /// <param name="returnUrl">URL to redirect</param>
        /// <returns>Register result model</returns>
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
        /// <returns>User navigation model</returns>
        public virtual async Task<UserNavigationModel> PrepareUserNavigationModelAsync(int selectedTabId = 0)
        {
            var model = new UserNavigationModel();

            model.UserNavigationItems.Add(new UserNavigationItemModel
            {
                RouteName = "UserInfo",
                Title = await _localizationService.GetResourceAsync("Account.UserInfo"),
                Tab = UserNavigationEnum.Info,
                ItemClass = "user-info"
            });

            model.UserNavigationItems.Add(new UserNavigationItemModel
            {
                RouteName = "UserAddresses",
                Title = await _localizationService.GetResourceAsync("Account.UserAddresses"),
                Tab = UserNavigationEnum.Addresses,
                ItemClass = "user-addresses"
            });

            model.UserNavigationItems.Add(new UserNavigationItemModel
            {
                RouteName = "UserOrders",
                Title = await _localizationService.GetResourceAsync("Account.UserOrders"),
                Tab = UserNavigationEnum.Orders,
                ItemClass = "user-orders"
            });

            if (_orderSettings.ReturnRequestsEnabled &&
                (await _returnRequestService.SearchReturnRequestsAsync((await _storeContext.GetCurrentStoreAsync()).Id,
                    (await _workContext.GetCurrentUserAsync()).Id, pageIndex: 0, pageSize: 1)).Any())
            {
                model.UserNavigationItems.Add(new UserNavigationItemModel
                {
                    RouteName = "UserReturnRequests",
                    Title = await _localizationService.GetResourceAsync("Account.UserReturnRequests"),
                    Tab = UserNavigationEnum.ReturnRequests,
                    ItemClass = "return-requests"
                });
            }

            if (!_userSettings.HideDownloadableProductsTab)
            {
                model.UserNavigationItems.Add(new UserNavigationItemModel
                {
                    RouteName = "UserDownloadableProducts",
                    Title = await _localizationService.GetResourceAsync("Account.DownloadableProducts"),
                    Tab = UserNavigationEnum.DownloadableProducts,
                    ItemClass = "downloadable-products"
                });
            }

            if (!_userSettings.HideBackInStockSubscriptionsTab)
            {
                model.UserNavigationItems.Add(new UserNavigationItemModel
                {
                    RouteName = "UserBackInStockSubscriptions",
                    Title = await _localizationService.GetResourceAsync("Account.BackInStockSubscriptions"),
                    Tab = UserNavigationEnum.BackInStockSubscriptions,
                    ItemClass = "back-in-stock-subscriptions"
                });
            }

            if (_rewardPointsSettings.Enabled)
            {
                model.UserNavigationItems.Add(new UserNavigationItemModel
                {
                    RouteName = "UserRewardPoints",
                    Title = await _localizationService.GetResourceAsync("Account.RewardPoints"),
                    Tab = UserNavigationEnum.RewardPoints,
                    ItemClass = "reward-points"
                });
            }

            model.UserNavigationItems.Add(new UserNavigationItemModel
            {
                RouteName = "UserChangePassword",
                Title = await _localizationService.GetResourceAsync("Account.ChangePassword"),
                Tab = UserNavigationEnum.ChangePassword,
                ItemClass = "change-password"
            });

            if (_userSettings.AllowUsersToUploadAvatars)
            {
                model.UserNavigationItems.Add(new UserNavigationItemModel
                {
                    RouteName = "UserAvatar",
                    Title = await _localizationService.GetResourceAsync("Account.Avatar"),
                    Tab = UserNavigationEnum.Avatar,
                    ItemClass = "user-avatar"
                });
            }

            if (_forumSettings.ForumsEnabled && _forumSettings.AllowUsersToManageSubscriptions)
            {
                model.UserNavigationItems.Add(new UserNavigationItemModel
                {
                    RouteName = "UserForumSubscriptions",
                    Title = await _localizationService.GetResourceAsync("Account.ForumSubscriptions"),
                    Tab = UserNavigationEnum.ForumSubscriptions,
                    ItemClass = "forum-subscriptions"
                });
            }
            if (_catalogSettings.ShowProductReviewsTabOnAccountPage)
            {
                model.UserNavigationItems.Add(new UserNavigationItemModel
                {
                    RouteName = "UserProductReviews",
                    Title = await _localizationService.GetResourceAsync("Account.UserProductReviews"),
                    Tab = UserNavigationEnum.ProductReviews,
                    ItemClass = "user-reviews"
                });
            }
            if (_vendorSettings.AllowVendorsToEditInfo && await _workContext.GetCurrentVendorAsync() != null)
            {
                model.UserNavigationItems.Add(new UserNavigationItemModel
                {
                    RouteName = "UserVendorInfo",
                    Title = await _localizationService.GetResourceAsync("Account.VendorInfo"),
                    Tab = UserNavigationEnum.VendorInfo,
                    ItemClass = "user-vendor-info"
                });
            }
            if (_gdprSettings.GdprEnabled)
            {
                model.UserNavigationItems.Add(new UserNavigationItemModel
                {
                    RouteName = "GdprTools",
                    Title = await _localizationService.GetResourceAsync("Account.Gdpr"),
                    Tab = UserNavigationEnum.GdprTools,
                    ItemClass = "user-gdpr"
                });
            }

            if (_captchaSettings.Enabled && _userSettings.AllowUsersToCheckGiftCardBalance)
            {
                model.UserNavigationItems.Add(new UserNavigationItemModel
                {
                    RouteName = "CheckGiftCardBalance",
                    Title = await _localizationService.GetResourceAsync("CheckGiftCardBalance"),
                    Tab = UserNavigationEnum.CheckGiftCardBalance,
                    ItemClass = "user-check-gift-card-balance"
                });
            }

            if (await _multiFactorAuthenticationPluginManager.HasActivePluginsAsync())
            {
                model.UserNavigationItems.Add(new UserNavigationItemModel
                {
                    RouteName = "MultiFactorAuthenticationSettings",
                    Title = await _localizationService.GetResourceAsync("PageTitle.MultiFactorAuthentication"),
                    Tab = UserNavigationEnum.MultiFactorAuthentication,
                    ItemClass = "user-multiFactor-authentication"
                });
            }

            model.SelectedTab = (UserNavigationEnum)selectedTabId;

            return model;
        }

        /// <summary>
        /// Prepare the user address list model
        /// </summary>
        /// <returns>User address list model</returns>
        public virtual async Task<UserAddressListModel> PrepareUserAddressListModelAsync()
        {
            var addresses = await (await _userService.GetAddressesByUserIdAsync((await _workContext.GetCurrentUserAsync()).Id))
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
        /// Prepare the user downloadable products model
        /// </summary>
        /// <returns>User downloadable products model</returns>
        public virtual async Task<UserDownloadableProductsModel> PrepareUserDownloadableProductsModelAsync()
        {
            var model = new UserDownloadableProductsModel();
            var items = await _orderService.GetDownloadableOrderItemsAsync((await _workContext.GetCurrentUserAsync()).Id);
            foreach (var item in items)
            {
                var order = await _orderService.GetOrderByIdAsync(item.OrderId);
                var product = await _productService.GetProductByIdAsync(item.ProductId);

                var itemModel = new UserDownloadableProductsModel.DownloadableProductsModel
                {
                    OrderItemGuid = item.OrderItemGuid,
                    OrderId = order.Id,
                    CustomOrderNumber = order.CustomOrderNumber,
                    CreatedOn = await _dateTimeHelper.ConvertToUserTimeAsync(order.CreatedOnUtc, DateTimeKind.Utc),
                    ProductName = await _localizationService.GetLocalizedAsync(product, x => x.Name),
                    ProductSeName = await _urlRecordService.GetSeNameAsync(product),
                    ProductAttributes = item.AttributeDescription,
                    ProductId = item.ProductId
                };
                model.Items.Add(itemModel);

                if (await _orderService.IsDownloadAllowedAsync(item))
                    itemModel.DownloadId = product.DownloadId;

                if (await _orderService.IsLicenseDownloadAllowedAsync(item))
                    itemModel.LicenseId = item.LicenseDownloadId ?? 0;
            }

            return model;
        }

        /// <summary>
        /// Prepare the user agreement model
        /// </summary>
        /// <param name="orderItem">Order item</param>
        /// <param name="product">Product</param>
        /// <returns>User agreement model</returns>
        public virtual Task<UserAgreementModel> PrepareUserAgreementModelAsync(OrderItem orderItem, Product product)
        {
            if (orderItem == null)
                throw new ArgumentNullException(nameof(orderItem));

            if (product == null)
                throw new ArgumentNullException(nameof(product));

            var model = new UserAgreementModel
            {
                UserAgreementText = product.UserAgreementText,
                OrderItemGuid = orderItem.OrderItemGuid
            };

            return Task.FromResult(model);
        }

        /// <summary>
        /// Prepare the change password model
        /// </summary>
        /// <returns>Change password model</returns>
        public virtual Task<ChangePasswordModel> PrepareChangePasswordModelAsync()
        {
            var model = new ChangePasswordModel();

            return Task.FromResult(model);
        }

        /// <summary>
        /// Prepare the user avatar model
        /// </summary>
        /// <param name="model">User avatar model</param>
        /// <returns>User avatar model</returns>
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
        /// <returns>GDPR tools model</returns>
        public virtual Task<GdprToolsModel> PrepareGdprToolsModelAsync()
        {
            var model = new GdprToolsModel();

            return Task.FromResult(model);
        }

        /// <summary>
        /// Prepare the check gift card balance madel
        /// </summary>
        /// <returns>Check gift card balance madel</returns>
        public virtual Task<CheckGiftCardBalanceModel> PrepareCheckGiftCardBalanceModelAsync()
        {
            var model = new CheckGiftCardBalanceModel();

            return Task.FromResult(model);
        }

        /// <summary>
        /// Prepare the multi-factor authentication model
        /// </summary>
        /// <param name="model">Multi-factor authentication model</param>
        /// <returns>Multi-factor authentication model</returns>
        public virtual async Task<MultiFactorAuthenticationModel> PrepareMultiFactorAuthenticationModelAsync(MultiFactorAuthenticationModel model)
        {
            var user = await _workContext.GetCurrentUserAsync();

            model.IsEnabled = !string.IsNullOrEmpty(
                await _genericAttributeService.GetAttributeAsync<string>(user, TvProgUserDefaults.SelectedMultiFactorAuthenticationProviderAttribute));

            var multiFactorAuthenticationProviders = (await _multiFactorAuthenticationPluginManager.LoadActivePluginsAsync(user, (await _storeContext.GetCurrentStoreAsync()).Id)).ToList();
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
        /// <returns>Multi-factor authentication model</returns>
        public virtual async Task<MultiFactorAuthenticationProviderModel> PrepareMultiFactorAuthenticationProviderModelAsync(MultiFactorAuthenticationProviderModel providerModel, string sysName, bool isLogin = false)
        {
            var user = await _workContext.GetCurrentUserAsync();
            var selectedProvider = await _genericAttributeService.GetAttributeAsync<string>(user, TvProgUserDefaults.SelectedMultiFactorAuthenticationProviderAttribute);

            var multiFactorAuthenticationProvider = (await _multiFactorAuthenticationPluginManager.LoadActivePluginsAsync(user, (await _storeContext.GetCurrentStoreAsync()).Id))
                    .FirstOrDefault(provider => provider.PluginDescriptor.SystemName == sysName);

            if (multiFactorAuthenticationProvider != null)
            {
                providerModel.Name = await _localizationService.GetLocalizedFriendlyNameAsync(multiFactorAuthenticationProvider, (await _workContext.GetWorkingLanguageAsync()).Id);
                providerModel.SystemName = sysName;
                providerModel.Description = await multiFactorAuthenticationProvider.GetDescriptionAsync();
                providerModel.LogoUrl = await _multiFactorAuthenticationPluginManager.GetPluginLogoUrlAsync(multiFactorAuthenticationProvider);
                providerModel.ViewComponentName = isLogin ? multiFactorAuthenticationProvider.GetVerificationViewComponentName() : multiFactorAuthenticationProvider.GetPublicViewComponentName();
                providerModel.Selected = sysName == selectedProvider;
            }

            return providerModel;
        }

        #endregion
    }
}