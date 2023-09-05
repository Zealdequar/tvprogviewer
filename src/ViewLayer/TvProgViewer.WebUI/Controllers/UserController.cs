using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Primitives;
using TvProgViewer.Core;
using TvProgViewer.Core.Domain;
using TvProgViewer.Core.Domain.Catalog;
using TvProgViewer.Core.Domain.Common;
using TvProgViewer.Core.Domain.Users;
using TvProgViewer.Core.Domain.Forums;
using TvProgViewer.Core.Domain.Gdpr;
using TvProgViewer.Core.Domain.Localization;
using TvProgViewer.Core.Domain.Media;
using TvProgViewer.Core.Domain.Messages;
using TvProgViewer.Core.Domain.Security;
using TvProgViewer.Core.Domain.Tax;
using TvProgViewer.Core.Events;
using TvProgViewer.Core.Http;
using TvProgViewer.Core.Http.Extensions;
using TvProgViewer.Services.Authentication;
using TvProgViewer.Services.Authentication.External;
using TvProgViewer.Services.Authentication.MultiFactor;
using TvProgViewer.Services.Catalog;
using TvProgViewer.Services.Common;
using TvProgViewer.Services.Users;
using TvProgViewer.Services.Directory;
using TvProgViewer.Services.ExportImport;
using TvProgViewer.Services.Gdpr;
using TvProgViewer.Services.Helpers;
using TvProgViewer.Services.Localization;
using TvProgViewer.Services.Logging;
using TvProgViewer.Services.Media;
using TvProgViewer.Services.Messages;
using TvProgViewer.Services.Orders;
using TvProgViewer.Services.Security;
using TvProgViewer.Services.Tax;
using TvProgViewer.WebUI.Extensions;
using TvProgViewer.WebUI.Factories;
using TvProgViewer.Web.Framework;
using TvProgViewer.Web.Framework.Controllers;
using TvProgViewer.Web.Framework.Mvc.Filters;
using TvProgViewer.Web.Framework.Validators;
using TvProgViewer.WebUI.Models.User;

namespace TvProgViewer.WebUI.Controllers
{
    [AutoValidateAntiforgeryToken]
    public partial class UserController : BasePublicController
    {
        #region Fields

        private readonly AddressSettings _addressSettings;
        private readonly CaptchaSettings _captchaSettings;
        private readonly UserSettings _userSettings;
        private readonly DateTimeSettings _dateTimeSettings;
        private readonly ForumSettings _forumSettings;
        private readonly GdprSettings _gdprSettings;
        private readonly HtmlEncoder _htmlEncoder;
        private readonly IAddressAttributeParser _addressAttributeParser;
        private readonly IAddressModelFactory _addressModelFactory;
        private readonly IAddressService _addressService;
        private readonly IAuthenticationService _authenticationService;
        private readonly ICountryService _countryService;
        private readonly ICurrencyService _currencyService;
        private readonly IUserActivityService _userActivityService;
        private readonly IUserAttributeParser _userAttributeParser;
        private readonly IUserAttributeService _userAttributeService;
        private readonly IUserModelFactory _userModelFactory;
        private readonly IUserRegistrationService _userRegistrationService;
        private readonly IUserService _userService;
        private readonly IDownloadService _downloadService;
        private readonly IEventPublisher _eventPublisher;
        private readonly IExportManager _exportManager;
        private readonly IExternalAuthenticationService _externalAuthenticationService;
        private readonly IGdprService _gdprService;
        private readonly IGenericAttributeService _genericAttributeService;
        private readonly IGiftCardService _giftCardService;
        private readonly ILocalizationService _localizationService;
        private readonly ILogger _logger;
        private readonly IMultiFactorAuthenticationPluginManager _multiFactorAuthenticationPluginManager;
        private readonly INewsLetterSubscriptionService _newsLetterSubscriptionService;
        private readonly INotificationService _notificationService;
        private readonly IOrderService _orderService;
        private readonly IPermissionService _permissionService;
        private readonly IPictureService _pictureService;
        private readonly IPriceFormatter _priceFormatter;
        private readonly IProductService _productService;
        private readonly IStateProvinceService _stateProvinceService;
        private readonly IStoreContext _storeContext;
        private readonly ITaxService _taxService;
        private readonly IWorkContext _workContext;
        private readonly IWorkflowMessageService _workflowMessageService;
        private readonly LocalizationSettings _localizationSettings;
        private readonly MediaSettings _mediaSettings;
        private readonly MultiFactorAuthenticationSettings _multiFactorAuthenticationSettings;
        private readonly StoreInformationSettings _storeInformationSettings;
        private readonly TaxSettings _taxSettings;

        #endregion

        #region Ctor

        public UserController(AddressSettings addressSettings,
            CaptchaSettings captchaSettings,
            UserSettings userSettings,
            DateTimeSettings dateTimeSettings,
            ForumSettings forumSettings,
            GdprSettings gdprSettings,
            HtmlEncoder htmlEncoder,
            IAddressAttributeParser addressAttributeParser,
            IAddressModelFactory addressModelFactory,
            IAddressService addressService,
            IAuthenticationService authenticationService,
            ICountryService countryService,
            ICurrencyService currencyService,
            IUserActivityService userActivityService,
            IUserAttributeParser userAttributeParser,
            IUserAttributeService userAttributeService,
            IUserModelFactory userModelFactory,
            IUserRegistrationService userRegistrationService,
            IUserService userService,
            IDownloadService downloadService,
            IEventPublisher eventPublisher,
            IExportManager exportManager,
            IExternalAuthenticationService externalAuthenticationService,
            IGdprService gdprService,
            IGenericAttributeService genericAttributeService,
            IGiftCardService giftCardService,
            ILocalizationService localizationService,
            ILogger logger,
            IMultiFactorAuthenticationPluginManager multiFactorAuthenticationPluginManager,
            INewsLetterSubscriptionService newsLetterSubscriptionService,
            INotificationService notificationService,
            IOrderService orderService,
            IPermissionService permissionService,
            IPictureService pictureService,
            IPriceFormatter priceFormatter,
            IProductService productService,
            IStateProvinceService stateProvinceService,
            IStoreContext storeContext,
            ITaxService taxService,
            IWorkContext workContext,
            IWorkflowMessageService workflowMessageService,
            LocalizationSettings localizationSettings,
            MediaSettings mediaSettings,
            MultiFactorAuthenticationSettings multiFactorAuthenticationSettings,
            StoreInformationSettings storeInformationSettings,
            TaxSettings taxSettings)
        {
            _addressSettings = addressSettings;
            _captchaSettings = captchaSettings;
            _userSettings = userSettings;
            _dateTimeSettings = dateTimeSettings;
            _forumSettings = forumSettings;
            _gdprSettings = gdprSettings;
            _htmlEncoder = htmlEncoder;
            _addressAttributeParser = addressAttributeParser;
            _addressModelFactory = addressModelFactory;
            _addressService = addressService;
            _authenticationService = authenticationService;
            _countryService = countryService;
            _currencyService = currencyService;
            _userActivityService = userActivityService;
            _userAttributeParser = userAttributeParser;
            _userAttributeService = userAttributeService;
            _userModelFactory = userModelFactory;
            _userRegistrationService = userRegistrationService;
            _userService = userService;
            _downloadService = downloadService;
            _eventPublisher = eventPublisher;
            _exportManager = exportManager;
            _externalAuthenticationService = externalAuthenticationService;
            _gdprService = gdprService;
            _genericAttributeService = genericAttributeService;
            _giftCardService = giftCardService;
            _localizationService = localizationService;
            _logger = logger;
            _multiFactorAuthenticationPluginManager = multiFactorAuthenticationPluginManager;
            _newsLetterSubscriptionService = newsLetterSubscriptionService;
            _notificationService = notificationService;
            _orderService = orderService;
            _permissionService = permissionService;
            _pictureService = pictureService;
            _priceFormatter = priceFormatter;
            _productService = productService;
            _stateProvinceService = stateProvinceService;
            _storeContext = storeContext;
            _taxService = taxService;
            _workContext = workContext;
            _workflowMessageService = workflowMessageService;
            _localizationSettings = localizationSettings;
            _mediaSettings = mediaSettings;
            _multiFactorAuthenticationSettings = multiFactorAuthenticationSettings;
            _storeInformationSettings = storeInformationSettings;
            _taxSettings = taxSettings;
        }

        #endregion

        #region Utilities

        protected virtual void ValidateRequiredConsents(List<GdprConsent> consents, IFormCollection form)
        {
            foreach (var consent in consents)
            {
                var controlId = $"consent{consent.Id}";
                var cbConsent = form[controlId];
                if (StringValues.IsNullOrEmpty(cbConsent) || !cbConsent.ToString().Equals("on"))
                {
                    ModelState.AddModelError("", consent.RequiredMessage);
                }
            }
        }

        protected virtual async Task<string> ParseSelectedProviderAsync(IFormCollection form)
        {
            if (form == null)
                throw new ArgumentNullException(nameof(form));

            var store = await _storeContext.GetCurrentStoreAsync();

            var multiFactorAuthenticationProviders = await _multiFactorAuthenticationPluginManager.LoadActivePluginsAsync(await _workContext.GetCurrentUserAsync(), store.Id);
            foreach (var provider in multiFactorAuthenticationProviders)
            {
                var controlId = $"provider_{provider.PluginDescriptor.SystemName}";

                var curProvider = form[controlId];
                if (!StringValues.IsNullOrEmpty(curProvider))
                {
                    var selectedProvider = curProvider.ToString();
                    if (!string.IsNullOrEmpty(selectedProvider))
                    {
                        return selectedProvider;
                    }
                }
            }
            return string.Empty;
        }

        protected virtual async Task<string> ParseCustomUserAttributesAsync(IFormCollection form)
        {
            if (form == null)
                throw new ArgumentNullException(nameof(form));

            var attributesXml = "";
            var attributes = await _userAttributeService.GetAllUserAttributesAsync();
            foreach (var attribute in attributes)
            {
                var controlId = $"{TvProgUserServicesDefaults.UserAttributePrefix}{attribute.Id}";
                switch (attribute.AttributeControlType)
                {
                    case AttributeControlType.DropdownList:
                    case AttributeControlType.RadioList:
                        {
                            var ctrlAttributes = form[controlId];
                            if (!StringValues.IsNullOrEmpty(ctrlAttributes))
                            {
                                var selectedAttributeId = int.Parse(ctrlAttributes);
                                if (selectedAttributeId > 0)
                                    attributesXml = _userAttributeParser.AddUserAttribute(attributesXml,
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
                                        attributesXml = _userAttributeParser.AddUserAttribute(attributesXml,
                                            attribute, selectedAttributeId.ToString());
                                }
                            }
                        }
                        break;
                    case AttributeControlType.ReadonlyCheckboxes:
                        {
                            //load read-only (already server-side selected) values
                            var attributeValues = await _userAttributeService.GetUserAttributeValuesAsync(attribute.Id);
                            foreach (var selectedAttributeId in attributeValues
                                .Where(v => v.IsPreSelected)
                                .Select(v => v.Id)
                                .ToList())
                            {
                                attributesXml = _userAttributeParser.AddUserAttribute(attributesXml,
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
                                attributesXml = _userAttributeParser.AddUserAttribute(attributesXml,
                                    attribute, enteredText);
                            }
                        }
                        break;
                    case AttributeControlType.Datepicker:
                    case AttributeControlType.ColorSquares:
                    case AttributeControlType.ImageSquares:
                    case AttributeControlType.FileUpload:
                    //not supported user attributes
                    default:
                        break;
                }
            }

            return attributesXml;
        }

        protected virtual async Task LogGdprAsync(User user, UserInfoModel oldUserInfoModel,
            UserInfoModel newUserInfoModel, IFormCollection form)
        {
            try
            {
                //consents
                var consents = (await _gdprService.GetAllConsentsAsync()).Where(consent => consent.DisplayOnUserInfoPage).ToList();
                foreach (var consent in consents)
                {
                    var previousConsentValue = await _gdprService.IsConsentAcceptedAsync(consent.Id, user.Id);
                    var controlId = $"consent{consent.Id}";
                    var cbConsent = form[controlId];
                    if (!StringValues.IsNullOrEmpty(cbConsent) && cbConsent.ToString().Equals("on"))
                    {
                        //agree
                        if (!previousConsentValue.HasValue || !previousConsentValue.Value)
                        {
                            await _gdprService.InsertLogAsync(user, consent.Id, GdprRequestType.ConsentAgree, consent.Message);
                        }
                    }
                    else
                    {
                        //disagree
                        if (!previousConsentValue.HasValue || previousConsentValue.Value)
                        {
                            await _gdprService.InsertLogAsync(user, consent.Id, GdprRequestType.ConsentDisagree, consent.Message);
                        }
                    }
                }

                //newsletter subscriptions
                if (_gdprSettings.LogNewsletterConsent)
                {
                    if (oldUserInfoModel.Newsletter && !newUserInfoModel.Newsletter)
                        await _gdprService.InsertLogAsync(user, 0, GdprRequestType.ConsentDisagree, await _localizationService.GetResourceAsync("Gdpr.Consent.Newsletter"));
                    if (!oldUserInfoModel.Newsletter && newUserInfoModel.Newsletter)
                        await _gdprService.InsertLogAsync(user, 0, GdprRequestType.ConsentAgree, await _localizationService.GetResourceAsync("Gdpr.Consent.Newsletter"));
                }

                //user profile changes
                if (!_gdprSettings.LogUserProfileChanges)
                    return;

                if (oldUserInfoModel.Gender != newUserInfoModel.Gender)
                    await _gdprService.InsertLogAsync(user, 0, GdprRequestType.ProfileChanged, $"{await _localizationService.GetResourceAsync("Account.Fields.Gender")} = {newUserInfoModel.Gender}");

                if (oldUserInfoModel.FirstName != newUserInfoModel.FirstName)
                    await _gdprService.InsertLogAsync(user, 0, GdprRequestType.ProfileChanged, $"{await _localizationService.GetResourceAsync("Account.Fields.FirstName")} = {newUserInfoModel.FirstName}");

                if (oldUserInfoModel.LastName != newUserInfoModel.LastName)
                    await _gdprService.InsertLogAsync(user, 0, GdprRequestType.ProfileChanged, $"{await _localizationService.GetResourceAsync("Account.Fields.LastName")} = {newUserInfoModel.LastName}");

                if (oldUserInfoModel.ParseBirthDate() != newUserInfoModel.ParseBirthDate())
                    await _gdprService.InsertLogAsync(user, 0, GdprRequestType.ProfileChanged, $"{await _localizationService.GetResourceAsync("Account.Fields.BirthDate")} = {newUserInfoModel.ParseBirthDate()}");

                if (oldUserInfoModel.Email != newUserInfoModel.Email)
                    await _gdprService.InsertLogAsync(user, 0, GdprRequestType.ProfileChanged, $"{await _localizationService.GetResourceAsync("Account.Fields.Email")} = {newUserInfoModel.Email}");

                if (oldUserInfoModel.Company != newUserInfoModel.Company)
                    await _gdprService.InsertLogAsync(user, 0, GdprRequestType.ProfileChanged, $"{await _localizationService.GetResourceAsync("Account.Fields.Company")} = {newUserInfoModel.Company}");

                if (oldUserInfoModel.StreetAddress != newUserInfoModel.StreetAddress)
                    await _gdprService.InsertLogAsync(user, 0, GdprRequestType.ProfileChanged, $"{await _localizationService.GetResourceAsync("Account.Fields.StreetAddress")} = {newUserInfoModel.StreetAddress}");

                if (oldUserInfoModel.StreetAddress2 != newUserInfoModel.StreetAddress2)
                    await _gdprService.InsertLogAsync(user, 0, GdprRequestType.ProfileChanged, $"{await _localizationService.GetResourceAsync("Account.Fields.StreetAddress2")} = {newUserInfoModel.StreetAddress2}");

                if (oldUserInfoModel.ZipPostalCode != newUserInfoModel.ZipPostalCode)
                    await _gdprService.InsertLogAsync(user, 0, GdprRequestType.ProfileChanged, $"{await _localizationService.GetResourceAsync("Account.Fields.ZipPostalCode")} = {newUserInfoModel.ZipPostalCode}");

                if (oldUserInfoModel.City != newUserInfoModel.City)
                    await _gdprService.InsertLogAsync(user, 0, GdprRequestType.ProfileChanged, $"{await _localizationService.GetResourceAsync("Account.Fields.City")} = {newUserInfoModel.City}");

                if (oldUserInfoModel.County != newUserInfoModel.County)
                    await _gdprService.InsertLogAsync(user, 0, GdprRequestType.ProfileChanged, $"{await _localizationService.GetResourceAsync("Account.Fields.County")} = {newUserInfoModel.County}");

                if (oldUserInfoModel.CountryId != newUserInfoModel.CountryId)
                {
                    var countryName = (await _countryService.GetCountryByIdAsync(newUserInfoModel.CountryId))?.Name;
                    await _gdprService.InsertLogAsync(user, 0, GdprRequestType.ProfileChanged, $"{await _localizationService.GetResourceAsync("Account.Fields.Country")} = {countryName}");
                }

                if (oldUserInfoModel.StateProvinceId != newUserInfoModel.StateProvinceId)
                {
                    var stateProvinceName = (await _stateProvinceService.GetStateProvinceByIdAsync(newUserInfoModel.StateProvinceId))?.Name;
                    await _gdprService.InsertLogAsync(user, 0, GdprRequestType.ProfileChanged, $"{await _localizationService.GetResourceAsync("Account.Fields.StateProvince")} = {stateProvinceName}");
                }
            }
            catch (Exception exception)
            {
                await _logger.ErrorAsync(exception.Message, exception, user);
            }
        }

        #endregion

        #region Methods

        #region Login / logout

        //available even when a store is closed
        [CheckAccessClosedStore(ignore: true)]
        //available even when navigation is not allowed
        [CheckAccessPublicStore(ignore: true)]
        public virtual async Task<IActionResult> Login(bool? checkoutAsGuest)
        {
            var model = await _userModelFactory.PrepareLoginModelAsync(checkoutAsGuest);
            var user = await _workContext.GetCurrentUserAsync();

            if (await _userService.IsRegisteredAsync(user))
            {
                var fullName = await _userService.GetUserFullNameAsync(user);
                var message = await _localizationService.GetResourceAsync("Account.Login.AlreadyLogin");
                _notificationService.SuccessNotification(string.Format(message, _htmlEncoder.Encode(fullName)));
            }

            return View(model);
        }

        [HttpPost]
        [ValidateCaptcha]
        //available even when a store is closed
        [CheckAccessClosedStore(ignore: true)]
        //available even when navigation is not allowed
        [CheckAccessPublicStore(ignore: true)]
        public virtual async Task<IActionResult> Login(LoginModel model, string returnUrl, bool captchaValid)
        {
            //validate CAPTCHA
            if (_captchaSettings.Enabled && _captchaSettings.ShowOnLoginPage && !captchaValid)
            {
                ModelState.AddModelError("", await _localizationService.GetResourceAsync("Common.WrongCaptchaMessage"));
            }

            if (ModelState.IsValid)
            {
                var userUserName = model.Username?.Trim();
                var userEmail = model.Email?.Trim();
                var userNameOrEmail = _userSettings.UsernamesEnabled ? userUserName : userEmail;

                var loginResult = await _userRegistrationService.ValidateUserAsync(userNameOrEmail, model.Password);
                switch (loginResult)
                {
                    case UserLoginResults.Successful:
                        {
                            var user = _userSettings.UsernamesEnabled
                                ? await _userService.GetUserByUsernameAsync(userUserName)
                                : await _userService.GetUserByEmailAsync(userEmail);

                            return await _userRegistrationService.SignInUserAsync(user, returnUrl, model.RememberMe);
                        }
                    case UserLoginResults.MultiFactorAuthenticationRequired:
                        {
                            var userMultiFactorAuthenticationInfo = new UserMultiFactorAuthenticationInfo
                            {
                                UserName = userNameOrEmail,
                                RememberMe = model.RememberMe,
                                ReturnUrl = returnUrl
                            };
                            HttpContext.Session.Set(TvProgUserDefaults.UserMultiFactorAuthenticationInfo, userMultiFactorAuthenticationInfo);
                            return RedirectToRoute("MultiFactorVerification");
                        }
                    case UserLoginResults.UserNotExist:
                        ModelState.AddModelError("", await _localizationService.GetResourceAsync("Account.Login.WrongCredentials.UserNotExist"));
                        break;
                    case UserLoginResults.Deleted:
                        ModelState.AddModelError("", await _localizationService.GetResourceAsync("Account.Login.WrongCredentials.Deleted"));
                        break;
                    case UserLoginResults.NotActive:
                        ModelState.AddModelError("", await _localizationService.GetResourceAsync("Account.Login.WrongCredentials.NotActive"));
                        break;
                    case UserLoginResults.NotRegistered:
                        ModelState.AddModelError("", await _localizationService.GetResourceAsync("Account.Login.WrongCredentials.NotRegistered"));
                        break;
                    case UserLoginResults.LockedOut:
                        ModelState.AddModelError("", await _localizationService.GetResourceAsync("Account.Login.WrongCredentials.LockedOut"));
                        break;
                    case UserLoginResults.WrongPassword:
                    default:
                        ModelState.AddModelError("", await _localizationService.GetResourceAsync("Account.Login.WrongCredentials"));
                        break;
                }
            }

            //If we got this far, something failed, redisplay form
            model = await _userModelFactory.PrepareLoginModelAsync(model.CheckoutAsGuest);
            return View(model);
        }

        /// <summary>
        /// The entry point for injecting a plugin component of type "MultiFactorAuth"
        /// </summary>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the user verification page for Multi-factor authentication. Served by an authentication provider.
        /// </returns>
        public virtual async Task<IActionResult> MultiFactorVerification()
        {
            if (!await _multiFactorAuthenticationPluginManager.HasActivePluginsAsync())
                return RedirectToRoute("Login");

            var userMultiFactorAuthenticationInfo = HttpContext.Session.Get<UserMultiFactorAuthenticationInfo>(TvProgUserDefaults.UserMultiFactorAuthenticationInfo);
            var userName = userMultiFactorAuthenticationInfo?.UserName;
            if (string.IsNullOrEmpty(userName))
                return RedirectToRoute("Homepage");

            var user = _userSettings.UsernamesEnabled ? await _userService.GetUserByUsernameAsync(userName) : await _userService.GetUserByEmailAsync(userName);
            if (user == null)
                return RedirectToRoute("Homepage");

            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.EnableMultiFactorAuthentication, user))
                return RedirectToRoute("Homepage");

            var selectedProvider = await _genericAttributeService.GetAttributeAsync<string>(user, TvProgUserDefaults.SelectedMultiFactorAuthenticationProviderAttribute);
            if (string.IsNullOrEmpty(selectedProvider))
                return RedirectToRoute("Homepage");

            var model = new MultiFactorAuthenticationProviderModel();
            model = await _userModelFactory.PrepareMultiFactorAuthenticationProviderModelAsync(model, selectedProvider, true);

            return View(model);
        }

        //available even when a store is closed
        [CheckAccessClosedStore(ignore: true)]
        //available even when navigation is not allowed
        [CheckAccessPublicStore(ignore: true)]
        public virtual async Task<IActionResult> Logout()
        {
            var user = await _workContext.GetCurrentUserAsync();
            if (_workContext.OriginalUserIfImpersonated != null)
            {
                //activity log
                await _userActivityService.InsertActivityAsync(_workContext.OriginalUserIfImpersonated, "Impersonation.Finished",
                    string.Format(await _localizationService.GetResourceAsync("ActivityLog.Impersonation.Finished.StoreOwner"),
                        user.Email, user.Id),
                    user);

                await _userActivityService.InsertActivityAsync("Impersonation.Finished",
                    string.Format(await _localizationService.GetResourceAsync("ActivityLog.Impersonation.Finished.User"),
                        _workContext.OriginalUserIfImpersonated.Email, _workContext.OriginalUserIfImpersonated.Id),
                    _workContext.OriginalUserIfImpersonated);

                //logout impersonated user
                await _genericAttributeService
                    .SaveAttributeAsync<int?>(_workContext.OriginalUserIfImpersonated, TvProgUserDefaults.ImpersonatedUserIdAttribute, null);

                //redirect back to user details page (admin area)
                return RedirectToAction("Edit", "User", new { id = user.Id, area = AreaNames.Admin });
            }

            //activity log
            await _userActivityService.InsertActivityAsync(user, "PublicStore.Logout",
                await _localizationService.GetResourceAsync("ActivityLog.PublicStore.Logout"), user);

            //standard logout 
            await _authenticationService.SignOutAsync();

            //raise logged out event       
            await _eventPublisher.PublishAsync(new UserLoggedOutEvent(user));

            //EU Cookie
            if (_storeInformationSettings.DisplayEuCookieLawWarning)
            {
                //the cookie law message should not pop up immediately after logout.
                //otherwise, the user will have to click it again...
                //and thus next visitor will not click it... so violation for that cookie law..
                //the only good solution in this case is to store a temporary variable
                //indicating that the EU cookie popup window should not be displayed on the next page open (after logout redirection to homepage)
                //but it'll be displayed for further page loads
                TempData[$"{TvProgCookieDefaults.Prefix}{TvProgCookieDefaults.IgnoreEuCookieLawWarning}"] = true;
            }

            return RedirectToRoute("Homepage");
        }

        #endregion

        #region Password recovery

        //available even when navigation is not allowed
        [CheckAccessPublicStore(ignore: true)]
        //available even when a store is closed
        [CheckAccessClosedStore(ignore: true)]
        public virtual async Task<IActionResult> PasswordRecovery()
        {
            var model = new PasswordRecoveryModel();
            model = await _userModelFactory.PreparePasswordRecoveryModelAsync(model);

            return View(model);
        }

        [ValidateCaptcha]
        [HttpPost, ActionName("PasswordRecovery")]
        [FormValueRequired("send-email")]
        //available even when navigation is not allowed
        [CheckAccessPublicStore(ignore: true)]
        //available even when a store is closed
        [CheckAccessClosedStore(ignore: true)]
        public virtual async Task<IActionResult> PasswordRecoverySend(PasswordRecoveryModel model, bool captchaValid)
        {
            // validate CAPTCHA
            if (_captchaSettings.Enabled && _captchaSettings.ShowOnForgotPasswordPage && !captchaValid)
            {
                ModelState.AddModelError("", await _localizationService.GetResourceAsync("Common.WrongCaptchaMessage"));
            }

            if (ModelState.IsValid)
            {
                var user = await _userService.GetUserByEmailAsync(model.Email.Trim());
                if (user != null && user.Active && !user.Deleted)
                {
                    //save token and current date
                    var passwordRecoveryToken = Guid.NewGuid();
                    await _genericAttributeService.SaveAttributeAsync(user, TvProgUserDefaults.PasswordRecoveryTokenAttribute,
                        passwordRecoveryToken.ToString());
                    DateTime? generatedDateTime = DateTime.UtcNow;
                    await _genericAttributeService.SaveAttributeAsync(user,
                        TvProgUserDefaults.PasswordRecoveryTokenDateGeneratedAttribute, generatedDateTime);

                    //send email
                    await _workflowMessageService.SendUserPasswordRecoveryMessageAsync(user,
                        (await _workContext.GetWorkingLanguageAsync()).Id);

                    _notificationService.SuccessNotification(await _localizationService.GetResourceAsync("Account.PasswordRecovery.EmailHasBeenSent"));
                }
                else
                {
                    _notificationService.ErrorNotification(await _localizationService.GetResourceAsync("Account.PasswordRecovery.EmailNotFound"));
                }
            }

            model = await _userModelFactory.PreparePasswordRecoveryModelAsync(model);

            return View(model);
        }

        //available even when navigation is not allowed
        [CheckAccessPublicStore(ignore: true)]
        //available even when a store is closed
        [CheckAccessClosedStore(ignore: true)]
        public virtual async Task<IActionResult> PasswordRecoveryConfirm(string token, string email, Guid guid)
        {
            //For backward compatibility with previous versions where email was used as a parameter in the URL
            var user = await _userService.GetUserByEmailAsync(email)
                ?? await _userService.GetUserByGuidAsync(guid);

            if (user == null)
                return RedirectToRoute("Homepage");

            var model = new PasswordRecoveryConfirmModel { ReturnUrl = Url.RouteUrl("Homepage") };
            if (string.IsNullOrEmpty(await _genericAttributeService.GetAttributeAsync<string>(user, TvProgUserDefaults.PasswordRecoveryTokenAttribute)))
            {
                model.DisablePasswordChanging = true;
                model.Result = await _localizationService.GetResourceAsync("Account.PasswordRecovery.PasswordAlreadyHasBeenChanged");
                return View(model);
            }

            //validate token
            if (!await _userService.IsPasswordRecoveryTokenValidAsync(user, token))
            {
                model.DisablePasswordChanging = true;
                model.Result = await _localizationService.GetResourceAsync("Account.PasswordRecovery.WrongToken");
                return View(model);
            }

            //validate token expiration date
            if (await _userService.IsPasswordRecoveryLinkExpiredAsync(user))
            {
                model.DisablePasswordChanging = true;
                model.Result = await _localizationService.GetResourceAsync("Account.PasswordRecovery.LinkExpired");
                return View(model);
            }

            return View(model);
        }

        [HttpPost, ActionName("PasswordRecoveryConfirm")]
        [FormValueRequired("set-password")]
        //available even when navigation is not allowed
        [CheckAccessPublicStore(ignore: true)]
        //available even when a store is closed
        [CheckAccessClosedStore(ignore: true)]
        public virtual async Task<IActionResult> PasswordRecoveryConfirmPOST(string token, string email, Guid guid, PasswordRecoveryConfirmModel model)
        {
            //For backward compatibility with previous versions where email was used as a parameter in the URL
            var user = await _userService.GetUserByEmailAsync(email)
                ?? await _userService.GetUserByGuidAsync(guid);

            if (user == null)
                return RedirectToRoute("Homepage");

            model.ReturnUrl = Url.RouteUrl("Homepage");

            //validate token
            if (!await _userService.IsPasswordRecoveryTokenValidAsync(user, token))
            {
                model.DisablePasswordChanging = true;
                model.Result = await _localizationService.GetResourceAsync("Account.PasswordRecovery.WrongToken");
                return View(model);
            }

            //validate token expiration date
            if (await _userService.IsPasswordRecoveryLinkExpiredAsync(user))
            {
                model.DisablePasswordChanging = true;
                model.Result = await _localizationService.GetResourceAsync("Account.PasswordRecovery.LinkExpired");
                return View(model);
            }

            if (!ModelState.IsValid)
                return View(model);

            var response = await _userRegistrationService
                .ChangePasswordAsync(new ChangePasswordRequest(user.Email, false, _userSettings.DefaultPasswordFormat, model.NewPassword));
            if (!response.Success)
            {
                model.Result = string.Join(';', response.Errors);
                return View(model);
            }

            await _genericAttributeService.SaveAttributeAsync(user, TvProgUserDefaults.PasswordRecoveryTokenAttribute, "");

            //authenticate user after changing password
            await _userRegistrationService.SignInUserAsync(user, null, true);

            model.DisablePasswordChanging = true;
            model.Result = await _localizationService.GetResourceAsync("Account.PasswordRecovery.PasswordHasBeenChanged");
            return View(model);
        }

        #endregion     

        #region Register

        //available even when navigation is not allowed
        [CheckAccessPublicStore(ignore: true)]
        public virtual async Task<IActionResult> Register(string returnUrl)
        {
            //check whether registration is allowed
            if (_userSettings.UserRegistrationType == UserRegistrationType.Disabled)
                return RedirectToRoute("RegisterResult", new { resultId = (int)UserRegistrationType.Disabled, returnUrl });

            var model = new RegisterModel();
            model = await _userModelFactory.PrepareRegisterModelAsync(model, false, setDefaultValues: true);

            return View(model);
        }

        [HttpPost]
        [ValidateCaptcha]
        [ValidateHoneypot]
        //available even when navigation is not allowed
        [CheckAccessPublicStore(ignore: true)]
        public virtual async Task<IActionResult> Register(RegisterModel model, string returnUrl, bool captchaValid, IFormCollection form)
        {
            //check whether registration is allowed
            if (_userSettings.UserRegistrationType == UserRegistrationType.Disabled)
                return RedirectToRoute("RegisterResult", new { resultId = (int)UserRegistrationType.Disabled, returnUrl });

            var user = await _workContext.GetCurrentUserAsync();
            if (await _userService.IsRegisteredAsync(user))
            {
                //Already registered user. 
                await _authenticationService.SignOutAsync();

                //raise logged out event       
                await _eventPublisher.PublishAsync(new UserLoggedOutEvent(user));

                //Save a new record
                await _workContext.SetCurrentUserAsync(await _userService.InsertGuestUserAsync());
            }

            var store = await _storeContext.GetCurrentStoreAsync();
            user.RegisteredInStoreId = store.Id;

            //custom user attributes
            var userAttributesXml = await ParseCustomUserAttributesAsync(form);
            var userAttributeWarnings = await _userAttributeParser.GetAttributeWarningsAsync(userAttributesXml);
            foreach (var error in userAttributeWarnings)
            {
                ModelState.AddModelError("", error);
            }

            //validate CAPTCHA
            if (_captchaSettings.Enabled && _captchaSettings.ShowOnRegistrationPage && !captchaValid)
            {
                ModelState.AddModelError("", await _localizationService.GetResourceAsync("Common.WrongCaptchaMessage"));
            }

            //GDPR
            if (_gdprSettings.GdprEnabled)
            {
                var consents = (await _gdprService
                    .GetAllConsentsAsync()).Where(consent => consent.DisplayDuringRegistration && consent.IsRequired).ToList();

                ValidateRequiredConsents(consents, form);
            }

            if (ModelState.IsValid)
            {
                var userUserName = model.Username?.Trim();
                var userEmail = model.Email?.Trim();

                var isApproved = _userSettings.UserRegistrationType == UserRegistrationType.Standard;
                var registrationRequest = new UserRegistrationRequest(user,
                    userEmail,
                    _userSettings.UsernamesEnabled ? userUserName : userEmail,
                    model.Password,
                    _userSettings.DefaultPasswordFormat,
                    store.Id,
                    isApproved);
                var registrationResult = await _userRegistrationService.RegisterUserAsync(registrationRequest);
                if (registrationResult.Success)
                {
                    //properties
                    if (_dateTimeSettings.AllowUsersToSetTimeZone)
                        user.GmtZone = model.GmtZone;

                    //VAT number
                    if (_taxSettings.EuVatEnabled)
                    {
                        user.VatNumber = model.VatNumber;

                        var (vatNumberStatus, _, vatAddress) = await _taxService.GetVatNumberStatusAsync(model.VatNumber);
                        user.VatNumberStatusId = (int)vatNumberStatus;
                        //send VAT number admin notification
                        if (!string.IsNullOrEmpty(model.VatNumber) && _taxSettings.EuVatEmailAdminWhenNewVatSubmitted)
                            await _workflowMessageService.SendNewVatSubmittedStoreOwnerNotificationAsync(user, model.VatNumber, vatAddress, _localizationSettings.DefaultAdminLanguageId);
                    }

                    //form fields
                    if (_userSettings.GenderEnabled)
                        user.Gender = model.Gender;
                    if (_userSettings.FirstNameEnabled)
                        user.FirstName = model.FirstName;
                    if (_userSettings.LastNameEnabled)
                        user.LastName = model.LastName;
                    if (_userSettings.BirthDateEnabled)
                        user.BirthDate = model.ParseBirthDate();
                    if (_userSettings.CompanyEnabled)
                        user.Company = model.Company;
                    if (_userSettings.StreetAddressEnabled)
                        user.StreetAddress = model.StreetAddress;
                    if (_userSettings.StreetAddress2Enabled)
                        user.StreetAddress2 = model.StreetAddress2;
                    if (_userSettings.ZipPostalCodeEnabled)
                        user.ZipPostalCode = model.ZipPostalCode;
                    if (_userSettings.CityEnabled)
                        user.City = model.City;
                    if (_userSettings.CountyEnabled)
                        user.County = model.County;
                    if (_userSettings.CountryEnabled)
                        user.CountryId = model.CountryId;
                    if (_userSettings.CountryEnabled && _userSettings.StateProvinceEnabled)
                        user.StateProvinceId = model.StateProvinceId;
                    if (_userSettings.SmartPhoneEnabled)
                        user.SmartPhone = model.SmartPhone;
                    if (_userSettings.FaxEnabled)
                        user.Fax = model.Fax;

                    //save user attributes
                    user.CustomUserAttributesXML = userAttributesXml;
                    await _userService.UpdateUserAsync(user);

                    //newsletter
                    if (_userSettings.NewsletterEnabled)
                    {
                        var isNewsletterActive = _userSettings.UserRegistrationType != UserRegistrationType.EmailValidation;

                        //save newsletter value
                        var newsletter = await _newsLetterSubscriptionService.GetNewsLetterSubscriptionByEmailAndStoreIdAsync(userEmail, store.Id);
                        if (newsletter != null)
                        {
                            if (model.Newsletter)
                            {
                                newsletter.Active = isNewsletterActive;
                                await _newsLetterSubscriptionService.UpdateNewsLetterSubscriptionAsync(newsletter);

                                //GDPR
                                if (_gdprSettings.GdprEnabled && _gdprSettings.LogNewsletterConsent)
                                {
                                    await _gdprService.InsertLogAsync(user, 0, GdprRequestType.ConsentAgree, await _localizationService.GetResourceAsync("Gdpr.Consent.Newsletter"));
                                }
                            }
                            //else
                            //{
                            //When registering, not checking the newsletter check box should not take an existing email address off of the subscription list.
                            //_newsLetterSubscriptionService.DeleteNewsLetterSubscription(newsletter);
                            //}
                        }
                        else
                        {
                            if (model.Newsletter)
                            {
                                await _newsLetterSubscriptionService.InsertNewsLetterSubscriptionAsync(new NewsLetterSubscription
                                {
                                    NewsLetterSubscriptionGuid = Guid.NewGuid(),
                                    Email = userEmail,
                                    Active = isNewsletterActive,
                                    StoreId = store.Id,
                                    CreatedOnUtc = DateTime.UtcNow
                                });

                                //GDPR
                                if (_gdprSettings.GdprEnabled && _gdprSettings.LogNewsletterConsent)
                                {
                                    await _gdprService.InsertLogAsync(user, 0, GdprRequestType.ConsentAgree, await _localizationService.GetResourceAsync("Gdpr.Consent.Newsletter"));
                                }
                            }
                        }
                    }

                    if (_userSettings.AcceptPrivacyPolicyEnabled)
                    {
                        //privacy policy is required
                        //GDPR
                        if (_gdprSettings.GdprEnabled && _gdprSettings.LogPrivacyPolicyConsent)
                        {
                            await _gdprService.InsertLogAsync(user, 0, GdprRequestType.ConsentAgree, await _localizationService.GetResourceAsync("Gdpr.Consent.PrivacyPolicy"));
                        }
                    }

                    //GDPR
                    if (_gdprSettings.GdprEnabled)
                    {
                        var consents = (await _gdprService.GetAllConsentsAsync()).Where(consent => consent.DisplayDuringRegistration).ToList();
                        foreach (var consent in consents)
                        {
                            var controlId = $"consent{consent.Id}";
                            var cbConsent = form[controlId];
                            if (!StringValues.IsNullOrEmpty(cbConsent) && cbConsent.ToString().Equals("on"))
                            {
                                //agree
                                await _gdprService.InsertLogAsync(user, consent.Id, GdprRequestType.ConsentAgree, consent.Message);
                            }
                            else
                            {
                                //disagree
                                await _gdprService.InsertLogAsync(user, consent.Id, GdprRequestType.ConsentDisagree, consent.Message);
                            }
                        }
                    }

                    //insert default address (if possible)
                    var defaultAddress = new Address
                    {
                        FirstName = user.FirstName,
                        LastName = user.LastName,
                        Email = user.Email,
                        Company = user.Company,
                        CountryId = user.CountryId > 0
                            ? (int?)user.CountryId
                            : null,
                        StateProvinceId = user.StateProvinceId > 0
                            ? (int?)user.StateProvinceId
                            : null,
                        County = user.County,
                        City = user.City,
                        Address1 = user.StreetAddress,
                        Address2 = user.StreetAddress2,
                        ZipPostalCode = user.ZipPostalCode,
                        PhoneNumber = user.SmartPhone,
                        FaxNumber = user.Fax,
                        CreatedOnUtc = user.CreatedOnUtc
                    };
                    if (await _addressService.IsAddressValidAsync(defaultAddress))
                    {
                        //some validation
                        if (defaultAddress.CountryId == 0)
                            defaultAddress.CountryId = null;
                        if (defaultAddress.StateProvinceId == 0)
                            defaultAddress.StateProvinceId = null;
                        //set default address
                        //user.Addresses.Add(defaultAddress);

                        await _addressService.InsertAddressAsync(defaultAddress);

                        await _userService.InsertUserAddressAsync(user, defaultAddress);

                        user.BillingAddressId = defaultAddress.Id;
                        user.ShippingAddressId = defaultAddress.Id;

                        await _userService.UpdateUserAsync(user);
                    }

                    //notifications
                    if (_userSettings.NotifyNewUserRegistration)
                        await _workflowMessageService.SendUserRegisteredStoreOwnerNotificationMessageAsync(user,
                            _localizationSettings.DefaultAdminLanguageId);

                    //raise event       
                    await _eventPublisher.PublishAsync(new UserRegisteredEvent(user));
                    var currentLanguage = await _workContext.GetWorkingLanguageAsync();

                    switch (_userSettings.UserRegistrationType)
                    {
                        case UserRegistrationType.EmailValidation:
                            //email validation message
                            await _genericAttributeService.SaveAttributeAsync(user, TvProgUserDefaults.AccountActivationTokenAttribute, Guid.NewGuid().ToString());
                            await _workflowMessageService.SendUserEmailValidationMessageAsync(user, currentLanguage.Id);

                            //result
                            return RedirectToRoute("RegisterResult", new { resultId = (int)UserRegistrationType.EmailValidation, returnUrl });

                        case UserRegistrationType.AdminApproval:
                            return RedirectToRoute("RegisterResult", new { resultId = (int)UserRegistrationType.AdminApproval, returnUrl });

                        case UserRegistrationType.Standard:
                            //send user welcome message
                            await _workflowMessageService.SendUserWelcomeMessageAsync(user, currentLanguage.Id);

                            //raise event       
                            await _eventPublisher.PublishAsync(new UserActivatedEvent(user));

                            returnUrl = Url.RouteUrl("RegisterResult", new { resultId = (int)UserRegistrationType.Standard, returnUrl });
                            return await _userRegistrationService.SignInUserAsync(user, returnUrl, true);

                        default:
                            return RedirectToRoute("Homepage");
                    }
                }

                //errors
                foreach (var error in registrationResult.Errors)
                    ModelState.AddModelError("", error);
            }

            //If we got this far, something failed, redisplay form
            model = await _userModelFactory.PrepareRegisterModelAsync(model, true, userAttributesXml);

            return View(model);
        }

        //available even when navigation is not allowed
        [CheckAccessPublicStore(ignore: true)]
        public virtual async Task<IActionResult> RegisterResult(int resultId, string returnUrl)
        {
            if (string.IsNullOrEmpty(returnUrl) || !Url.IsLocalUrl(returnUrl))
                returnUrl = Url.RouteUrl("Homepage");

            var model = await _userModelFactory.PrepareRegisterResultModelAsync(resultId, returnUrl);
            return View(model);
        }

        [HttpPost]
        //available even when navigation is not allowed
        [CheckAccessPublicStore(ignore: true)]
        public virtual async Task<IActionResult> CheckUsernameAvailability(string username)
        {
            var usernameAvailable = false;
            var statusText = await _localizationService.GetResourceAsync("Account.CheckUsernameAvailability.NotAvailable");

            if (!UsernamePropertyValidator<string, string>.IsValid(username, _userSettings))
            {
                statusText = await _localizationService.GetResourceAsync("Account.Fields.Username.NotValid");
            }
            else if (_userSettings.UsernamesEnabled && !string.IsNullOrWhiteSpace(username))
            {
                var currentUser = await _workContext.GetCurrentUserAsync();
                if (currentUser != null &&
                    currentUser.Username != null &&
                    currentUser.Username.Equals(username, StringComparison.InvariantCultureIgnoreCase))
                {
                    statusText = await _localizationService.GetResourceAsync("Account.CheckUsernameAvailability.CurrentUsername");
                }
                else
                {
                    var user = await _userService.GetUserByUsernameAsync(username);
                    if (user == null)
                    {
                        statusText = await _localizationService.GetResourceAsync("Account.CheckUsernameAvailability.Available");
                        usernameAvailable = true;
                    }
                }
            }

            return Json(new { Available = usernameAvailable, Text = statusText });
        }

        //available even when navigation is not allowed
        [CheckAccessPublicStore(ignore: true)]
        public virtual async Task<IActionResult> AccountActivation(string token, string email, Guid guid)
        {
            //For backward compatibility with previous versions where email was used as a parameter in the URL
            var user = await _userService.GetUserByEmailAsync(email)
                ?? await _userService.GetUserByGuidAsync(guid);

            if (user == null)
                return RedirectToRoute("Homepage");

            var model = new AccountActivationModel { ReturnUrl = Url.RouteUrl("Homepage") };
            var cToken = await _genericAttributeService.GetAttributeAsync<string>(user, TvProgUserDefaults.AccountActivationTokenAttribute);
            if (string.IsNullOrEmpty(cToken))
            {
                model.Result = await _localizationService.GetResourceAsync("Account.AccountActivation.AlreadyActivated");
                return View(model);
            }

            if (!cToken.Equals(token, StringComparison.InvariantCultureIgnoreCase))
                return RedirectToRoute("Homepage");

            //activate user account
            user.Active = true;
            await _userService.UpdateUserAsync(user);
            await _genericAttributeService.SaveAttributeAsync(user, TvProgUserDefaults.AccountActivationTokenAttribute, "");

            //send welcome message
            await _workflowMessageService.SendUserWelcomeMessageAsync(user, (await _workContext.GetWorkingLanguageAsync()).Id);

            //raise event       
            await _eventPublisher.PublishAsync(new UserActivatedEvent(user));

            //authenticate user after activation
            await _userRegistrationService.SignInUserAsync(user, null, true);

            //activating newsletter if need
            var store = await _storeContext.GetCurrentStoreAsync();
            var newsletter = await _newsLetterSubscriptionService.GetNewsLetterSubscriptionByEmailAndStoreIdAsync(user.Email, store.Id);
            if (newsletter != null && !newsletter.Active)
            {
                newsletter.Active = true;
                await _newsLetterSubscriptionService.UpdateNewsLetterSubscriptionAsync(newsletter);
            }

            model.Result = await _localizationService.GetResourceAsync("Account.AccountActivation.Activated");
            return View(model);
        }

        #endregion

        #region My account / Info

        public virtual async Task<IActionResult> Info()
        {
            var user = await _workContext.GetCurrentUserAsync();
            if (!await _userService.IsRegisteredAsync(user))
                return Challenge();

            var model = new UserInfoModel();
            model = await _userModelFactory.PrepareUserInfoModelAsync(model, user, false);

            return View(model);
        }

        [HttpPost]
        public virtual async Task<IActionResult> Info(UserInfoModel model, IFormCollection form)
        {
            var user = await _workContext.GetCurrentUserAsync();
            if (!await _userService.IsRegisteredAsync(user))
                return Challenge();

            var oldUserModel = new UserInfoModel();

            //get user info model before changes for gdpr log
            if (_gdprSettings.GdprEnabled & _gdprSettings.LogUserProfileChanges)
                oldUserModel = await _userModelFactory.PrepareUserInfoModelAsync(oldUserModel, user, false);

            //custom user attributes
            var userAttributesXml = await ParseCustomUserAttributesAsync(form);
            var userAttributeWarnings = await _userAttributeParser.GetAttributeWarningsAsync(userAttributesXml);
            foreach (var error in userAttributeWarnings)
            {
                ModelState.AddModelError("", error);
            }

            //GDPR
            if (_gdprSettings.GdprEnabled)
            {
                var consents = (await _gdprService
                    .GetAllConsentsAsync()).Where(consent => consent.DisplayOnUserInfoPage && consent.IsRequired).ToList();

                ValidateRequiredConsents(consents, form);
            }

            try
            {
                if (ModelState.IsValid)
                {
                    //username 
                    if (_userSettings.UsernamesEnabled && _userSettings.AllowUsersToChangeUsernames)
                    {
                        var userName = model.Username.Trim();
                        if (!user.Username.Equals(userName, StringComparison.InvariantCultureIgnoreCase))
                        {
                            //change username
                            await _userRegistrationService.SetUsernameAsync(user, userName);

                            //re-authenticate
                            //do not authenticate users in impersonation mode
                            if (_workContext.OriginalUserIfImpersonated == null)
                                await _authenticationService.SignInAsync(user, true);
                        }
                    }
                    //email
                    var email = model.Email.Trim();
                    if (!user.Email.Equals(email, StringComparison.InvariantCultureIgnoreCase))
                    {
                        //change email
                        var requireValidation = _userSettings.UserRegistrationType == UserRegistrationType.EmailValidation;
                        await _userRegistrationService.SetEmailAsync(user, email, requireValidation);

                        //do not authenticate users in impersonation mode
                        if (_workContext.OriginalUserIfImpersonated == null)
                        {
                            //re-authenticate (if usernames are disabled)
                            if (!_userSettings.UsernamesEnabled && !requireValidation)
                                await _authenticationService.SignInAsync(user, true);
                        }
                    }

                    //properties
                    if (_dateTimeSettings.AllowUsersToSetTimeZone)
                        user.GmtZone = model.GmtZone;
                    //VAT number
                    if (_taxSettings.EuVatEnabled)
                    {
                        var prevVatNumber = user.VatNumber;
                        user.VatNumber = model.VatNumber;

                        if (prevVatNumber != model.VatNumber)
                        {
                            var (vatNumberStatus, _, vatAddress) = await _taxService.GetVatNumberStatusAsync(model.VatNumber);
                            user.VatNumberStatusId = (int)vatNumberStatus;

                            //send VAT number admin notification
                            if (!string.IsNullOrEmpty(model.VatNumber) && _taxSettings.EuVatEmailAdminWhenNewVatSubmitted)
                                await _workflowMessageService.SendNewVatSubmittedStoreOwnerNotificationAsync(user,
                                    model.VatNumber, vatAddress, _localizationSettings.DefaultAdminLanguageId);
                        }
                    }

                    //form fields
                    if (_userSettings.GenderEnabled)
                        user.Gender = model.Gender;
                    if (_userSettings.FirstNameEnabled)
                        user.FirstName = model.FirstName;
                    if (_userSettings.LastNameEnabled)
                        user.LastName = model.LastName;
                    if (_userSettings.BirthDateEnabled)
                        user.BirthDate = model.ParseBirthDate();
                    if (_userSettings.CompanyEnabled)
                        user.Company = model.Company;
                    if (_userSettings.StreetAddressEnabled)
                        user.StreetAddress = model.StreetAddress;
                    if (_userSettings.StreetAddress2Enabled)
                        user.StreetAddress2 = model.StreetAddress2;
                    if (_userSettings.ZipPostalCodeEnabled)
                        user.ZipPostalCode = model.ZipPostalCode;
                    if (_userSettings.CityEnabled)
                        user.City = model.City;
                    if (_userSettings.CountyEnabled)
                        user.County = model.County;
                    if (_userSettings.CountryEnabled)
                        user.CountryId = model.CountryId;
                    if (_userSettings.CountryEnabled && _userSettings.StateProvinceEnabled)
                        user.StateProvinceId = model.StateProvinceId;
                    if (_userSettings.SmartPhoneEnabled)
                        user.SmartPhone = model.SmartPhone;
                    if (_userSettings.FaxEnabled)
                        user.Fax = model.Fax;

                    user.CustomUserAttributesXML = userAttributesXml;
                    await _userService.UpdateUserAsync(user);

                    //newsletter
                    if (_userSettings.NewsletterEnabled)
                    {
                        //save newsletter value
                        var store = await _storeContext.GetCurrentStoreAsync();
                        var newsletter = await _newsLetterSubscriptionService.GetNewsLetterSubscriptionByEmailAndStoreIdAsync(user.Email, store.Id);
                        if (newsletter != null)
                        {
                            if (model.Newsletter)
                            {
                                newsletter.Active = true;
                                await _newsLetterSubscriptionService.UpdateNewsLetterSubscriptionAsync(newsletter);
                            }
                            else
                            {
                                await _newsLetterSubscriptionService.DeleteNewsLetterSubscriptionAsync(newsletter);
                            }
                        }
                        else
                        {
                            if (model.Newsletter)
                            {
                                await _newsLetterSubscriptionService.InsertNewsLetterSubscriptionAsync(new NewsLetterSubscription
                                {
                                    NewsLetterSubscriptionGuid = Guid.NewGuid(),
                                    Email = user.Email,
                                    Active = true,
                                    StoreId = store.Id,
                                    CreatedOnUtc = DateTime.UtcNow
                                });
                            }
                        }
                    }

                    if (_forumSettings.ForumsEnabled && _forumSettings.SignaturesEnabled)
                        await _genericAttributeService.SaveAttributeAsync(user, TvProgUserDefaults.SignatureAttribute, model.Signature);

                    //GDPR
                    if (_gdprSettings.GdprEnabled)
                        await LogGdprAsync(user, oldUserModel, model, form);

                    _notificationService.SuccessNotification(await _localizationService.GetResourceAsync("Account.UserInfo.Updated"));

                    return RedirectToRoute("UserInfo");
                }
            }
            catch (Exception exc)
            {
                ModelState.AddModelError("", exc.Message);
            }

            //If we got this far, something failed, redisplay form
            model = await _userModelFactory.PrepareUserInfoModelAsync(model, user, true, userAttributesXml);

            return View(model);
        }

        [HttpPost]
        public virtual async Task<IActionResult> RemoveExternalAssociation(int id)
        {
            if (!await _userService.IsRegisteredAsync(await _workContext.GetCurrentUserAsync()))
                return Challenge();

            //ensure it's our record
            var ear = await _externalAuthenticationService.GetExternalAuthenticationRecordByIdAsync(id);

            if (ear == null)
            {
                return Json(new
                {
                    redirect = Url.Action("Info"),
                });
            }

            await _externalAuthenticationService.DeleteExternalAuthenticationRecordAsync(ear);

            return Json(new
            {
                redirect = Url.Action("Info"),
            });
        }

        //available even when navigation is not allowed
        [CheckAccessPublicStore(ignore: true)]
        public virtual async Task<IActionResult> EmailRevalidation(string token, string email, Guid guid)
        {
            //For backward compatibility with previous versions where email was used as a parameter in the URL
            var user = await _userService.GetUserByEmailAsync(email)
                ?? await _userService.GetUserByGuidAsync(guid);

            if (user == null)
                return RedirectToRoute("Homepage");

            var model = new EmailRevalidationModel { ReturnUrl = Url.RouteUrl("Homepage") };
            var cToken = await _genericAttributeService.GetAttributeAsync<string>(user, TvProgUserDefaults.EmailRevalidationTokenAttribute);
            if (string.IsNullOrEmpty(cToken))
            {
                model.Result = await _localizationService.GetResourceAsync("Account.EmailRevalidation.AlreadyChanged");
                return View(model);
            }

            if (!cToken.Equals(token, StringComparison.InvariantCultureIgnoreCase))
                return RedirectToRoute("Homepage");

            if (string.IsNullOrEmpty(user.EmailToRevalidate))
                return RedirectToRoute("Homepage");

            if (_userSettings.UserRegistrationType != UserRegistrationType.EmailValidation)
                return RedirectToRoute("Homepage");

            //change email
            try
            {
                await _userRegistrationService.SetEmailAsync(user, user.EmailToRevalidate, false);
            }
            catch (Exception exc)
            {
                model.Result = await _localizationService.GetResourceAsync(exc.Message);
                return View(model);
            }

            user.EmailToRevalidate = null;
            await _userService.UpdateUserAsync(user);
            await _genericAttributeService.SaveAttributeAsync(user, TvProgUserDefaults.EmailRevalidationTokenAttribute, "");

            //authenticate user after changing email
            await _userRegistrationService.SignInUserAsync(user, null, true);

            model.Result = await _localizationService.GetResourceAsync("Account.EmailRevalidation.Changed");
            return View(model);
        }

        #endregion

        #region My account / Addresses

        public virtual async Task<IActionResult> Addresses()
        {
            if (!await _userService.IsRegisteredAsync(await _workContext.GetCurrentUserAsync()))
                return Challenge();

            var model = await _userModelFactory.PrepareUserAddressListModelAsync();

            return View(model);
        }

        [HttpPost]
        public virtual async Task<IActionResult> AddressDelete(int addressId)
        {
            var user = await _workContext.GetCurrentUserAsync();
            if (!await _userService.IsRegisteredAsync(user))
                return Challenge();

            //find address (ensure that it belongs to the current user)
            var address = await _userService.GetUserAddressAsync(user.Id, addressId);
            if (address != null)
            {
                await _userService.RemoveUserAddressAsync(user, address);
                await _userService.UpdateUserAsync(user);
                //now delete the address record
                await _addressService.DeleteAddressAsync(address);
            }

            //redirect to the address list page
            return Json(new
            {
                redirect = Url.RouteUrl("UserAddresses"),
            });
        }

        public virtual async Task<IActionResult> AddressAdd()
        {
            if (!await _userService.IsRegisteredAsync(await _workContext.GetCurrentUserAsync()))
                return Challenge();

            var model = new UserAddressEditModel();
            await _addressModelFactory.PrepareAddressModelAsync(model.Address,
                address: null,
                excludeProperties: false,
                addressSettings: _addressSettings,
                loadCountries: async () => await _countryService.GetAllCountriesAsync((await _workContext.GetWorkingLanguageAsync()).Id));

            return View(model);
        }

        [HttpPost]
        public virtual async Task<IActionResult> AddressAdd(UserAddressEditModel model, IFormCollection form)
        {
            var user = await _workContext.GetCurrentUserAsync();
            if (!await _userService.IsRegisteredAsync(user))
                return Challenge();

            //custom address attributes
            var customAttributes = await _addressAttributeParser.ParseCustomAddressAttributesAsync(form);
            var customAttributeWarnings = await _addressAttributeParser.GetAttributeWarningsAsync(customAttributes);
            foreach (var error in customAttributeWarnings)
            {
                ModelState.AddModelError("", error);
            }

            if (ModelState.IsValid)
            {
                var address = model.Address.ToEntity();
                address.CustomAttributes = customAttributes;
                address.CreatedOnUtc = DateTime.UtcNow;
                //some validation
                if (address.CountryId == 0)
                    address.CountryId = null;
                if (address.StateProvinceId == 0)
                    address.StateProvinceId = null;


                await _addressService.InsertAddressAsync(address);

                await _userService.InsertUserAddressAsync(user, address);

                _notificationService.SuccessNotification(await _localizationService.GetResourceAsync("Account.UserAddresses.Added"));

                return RedirectToRoute("UserAddresses");
            }

            //If we got this far, something failed, redisplay form
            await _addressModelFactory.PrepareAddressModelAsync(model.Address,
                address: null,
                excludeProperties: true,
                addressSettings: _addressSettings,
                loadCountries: async () => await _countryService.GetAllCountriesAsync((await _workContext.GetWorkingLanguageAsync()).Id),
                overrideAttributesXml: customAttributes);

            return View(model);
        }

        public virtual async Task<IActionResult> AddressEdit(int addressId)
        {
            var user = await _workContext.GetCurrentUserAsync();
            if (!await _userService.IsRegisteredAsync(user))
                return Challenge();

            //find address (ensure that it belongs to the current user)
            var address = await _userService.GetUserAddressAsync(user.Id, addressId);
            if (address == null)
                //address is not found
                return RedirectToRoute("UserAddresses");

            var model = new UserAddressEditModel();
            await _addressModelFactory.PrepareAddressModelAsync(model.Address,
                address: address,
                excludeProperties: false,
                addressSettings: _addressSettings,
                loadCountries: async () => await _countryService.GetAllCountriesAsync((await _workContext.GetWorkingLanguageAsync()).Id));

            return View(model);
        }

        [HttpPost]
        public virtual async Task<IActionResult> AddressEdit(UserAddressEditModel model, IFormCollection form)
        {
            var user = await _workContext.GetCurrentUserAsync();
            if (!await _userService.IsRegisteredAsync(user))
                return Challenge();

            //find address (ensure that it belongs to the current user)
            var address = await _userService.GetUserAddressAsync(user.Id, model.Address.Id);
            if (address == null)
                //address is not found
                return RedirectToRoute("UserAddresses");

            //custom address attributes
            var customAttributes = await _addressAttributeParser.ParseCustomAddressAttributesAsync(form);
            var customAttributeWarnings = await _addressAttributeParser.GetAttributeWarningsAsync(customAttributes);
            foreach (var error in customAttributeWarnings)
            {
                ModelState.AddModelError("", error);
            }

            if (ModelState.IsValid)
            {
                address = model.Address.ToEntity(address);
                address.CustomAttributes = customAttributes;
                await _addressService.UpdateAddressAsync(address);

                _notificationService.SuccessNotification(await _localizationService.GetResourceAsync("Account.UserAddresses.Updated"));

                return RedirectToRoute("UserAddresses");
            }

            //If we got this far, something failed, redisplay form
            await _addressModelFactory.PrepareAddressModelAsync(model.Address,
                address: address,
                excludeProperties: true,
                addressSettings: _addressSettings,
                loadCountries: async () => await _countryService.GetAllCountriesAsync((await _workContext.GetWorkingLanguageAsync()).Id),
                overrideAttributesXml: customAttributes);

            return View(model);
        }

        #endregion

        #region My account / Downloadable products

        public virtual async Task<IActionResult> DownloadableProducts()
        {
            if (!await _userService.IsRegisteredAsync(await _workContext.GetCurrentUserAsync()))
                return Challenge();

            if (_userSettings.HideDownloadableProductsTab)
                return RedirectToRoute("UserInfo");

            var model = await _userModelFactory.PrepareUserDownloadableProductsModelAsync();

            return View(model);
        }

        //ignore SEO friendly URLs checks
        [CheckLanguageSeoCode(ignore: true)]
        public virtual async Task<IActionResult> UserAgreement(Guid orderItemId)
        {
            var orderItem = await _orderService.GetOrderItemByGuidAsync(orderItemId);
            if (orderItem == null)
                return RedirectToRoute("Homepage");

            var product = await _productService.GetProductByIdAsync(orderItem.ProductId);

            if (product == null || !product.HasUserAgreement)
                return RedirectToRoute("Homepage");

            var model = await _userModelFactory.PrepareUserAgreementModelAsync(orderItem, product);

            return View(model);
        }

        #endregion

        #region My account / Change password

        public virtual async Task<IActionResult> ChangePassword()
        {
            var user = await _workContext.GetCurrentUserAsync();
            if (!await _userService.IsRegisteredAsync(user))
                return Challenge();

            var model = await _userModelFactory.PrepareChangePasswordModelAsync();

            //display the cause of the change password 
            if (await _userService.IsPasswordExpiredAsync(user))
                ModelState.AddModelError(string.Empty, await _localizationService.GetResourceAsync("Account.ChangePassword.PasswordIsExpired"));

            return View(model);
        }

        [HttpPost]
        public virtual async Task<IActionResult> ChangePassword(ChangePasswordModel model, string returnUrl)
        {
            var user = await _workContext.GetCurrentUserAsync();
            if (!await _userService.IsRegisteredAsync(user))
                return Challenge();

            if (ModelState.IsValid)
            {
                var changePasswordRequest = new ChangePasswordRequest(user.Email,
                    true, _userSettings.DefaultPasswordFormat, model.NewPassword, model.OldPassword);
                var changePasswordResult = await _userRegistrationService.ChangePasswordAsync(changePasswordRequest);
                if (changePasswordResult.Success)
                {
                    _notificationService.SuccessNotification(await _localizationService.GetResourceAsync("Account.ChangePassword.Success"));

                    //authenticate user after changing password
                    await _userRegistrationService.SignInUserAsync(user, null, true);

                    if (string.IsNullOrEmpty(returnUrl))
                        return View(model);

                    //prevent open redirection attack
                    if (!Url.IsLocalUrl(returnUrl))
                        returnUrl = Url.RouteUrl("Homepage");

                    return new RedirectResult(returnUrl);
                }

                //errors
                foreach (var error in changePasswordResult.Errors)
                    ModelState.AddModelError("", error);
            }

            //If we got this far, something failed, redisplay form
            return View(model);
        }

        #endregion

        #region My account / Avatar

        public virtual async Task<IActionResult> Avatar()
        {
            if (!await _userService.IsRegisteredAsync(await _workContext.GetCurrentUserAsync()))
                return Challenge();

            if (!_userSettings.AllowUsersToUploadAvatars)
                return RedirectToRoute("UserInfo");

            var model = new UserAvatarModel();
            model = await _userModelFactory.PrepareUserAvatarModelAsync(model);

            return View(model);
        }

        [HttpPost, ActionName("Avatar")]
        [FormValueRequired("upload-avatar")]
        public virtual async Task<IActionResult> UploadAvatar(UserAvatarModel model, IFormFile uploadedFile)
        {
            var user = await _workContext.GetCurrentUserAsync();
            if (!await _userService.IsRegisteredAsync(user))
                return Challenge();

            if (!_userSettings.AllowUsersToUploadAvatars)
                return RedirectToRoute("UserInfo");

            var contentType = uploadedFile?.ContentType.ToLowerInvariant();

            if (contentType != null && !contentType.Equals("image/jpeg") && !contentType.Equals("image/gif"))
                ModelState.AddModelError("", await _localizationService.GetResourceAsync("Account.Avatar.UploadRules"));

            if (ModelState.IsValid)
            {
                try
                {
                    var userAvatar = await _pictureService.GetPictureByIdAsync(await _genericAttributeService.GetAttributeAsync<int>(user, TvProgUserDefaults.AvatarPictureIdAttribute));
                    if (uploadedFile != null && !string.IsNullOrEmpty(uploadedFile.FileName))
                    {
                        var avatarMaxSize = _userSettings.AvatarMaximumSizeBytes;
                        if (uploadedFile.Length > avatarMaxSize)
                            throw new TvProgException(string.Format(await _localizationService.GetResourceAsync("Account.Avatar.MaximumUploadedFileSize"), avatarMaxSize));

                        var userPictureBinary = await _downloadService.GetDownloadBitsAsync(uploadedFile);
                        if (userAvatar != null)
                            userAvatar = await _pictureService.UpdatePictureAsync(userAvatar.Id, userPictureBinary, contentType, null);
                        else
                            userAvatar = await _pictureService.InsertPictureAsync(userPictureBinary, contentType, null);
                    }

                    var userAvatarId = 0;
                    if (userAvatar != null)
                        userAvatarId = userAvatar.Id;

                    await _genericAttributeService.SaveAttributeAsync(user, TvProgUserDefaults.AvatarPictureIdAttribute, userAvatarId);

                    model.AvatarUrl = await _pictureService.GetPictureUrlAsync(
                        await _genericAttributeService.GetAttributeAsync<int>(user, TvProgUserDefaults.AvatarPictureIdAttribute),
                        _mediaSettings.AvatarPictureSize,
                        false);

                    return View(model);
                }
                catch (Exception exc)
                {
                    ModelState.AddModelError("", exc.Message);
                }
            }

            //If we got this far, something failed, redisplay form
            model = await _userModelFactory.PrepareUserAvatarModelAsync(model);
            return View(model);
        }

        [HttpPost, ActionName("Avatar")]
        [FormValueRequired("remove-avatar")]
        public virtual async Task<IActionResult> RemoveAvatar(UserAvatarModel model)
        {
            var user = await _workContext.GetCurrentUserAsync();
            if (!await _userService.IsRegisteredAsync(user))
                return Challenge();

            if (!_userSettings.AllowUsersToUploadAvatars)
                return RedirectToRoute("UserInfo");

            var userAvatar = await _pictureService.GetPictureByIdAsync(await _genericAttributeService.GetAttributeAsync<int>(user, TvProgUserDefaults.AvatarPictureIdAttribute));
            if (userAvatar != null)
                await _pictureService.DeletePictureAsync(userAvatar);
            await _genericAttributeService.SaveAttributeAsync(user, TvProgUserDefaults.AvatarPictureIdAttribute, 0);

            return RedirectToRoute("UserAvatar");
        }

        #endregion

        #region GDPR tools

        public virtual async Task<IActionResult> GdprTools()
        {
            if (!await _userService.IsRegisteredAsync(await _workContext.GetCurrentUserAsync()))
                return Challenge();

            if (!_gdprSettings.GdprEnabled)
                return RedirectToRoute("UserInfo");

            var model = await _userModelFactory.PrepareGdprToolsModelAsync();

            return View(model);
        }

        [HttpPost, ActionName("GdprTools")]
        [FormValueRequired("export-data")]
        public virtual async Task<IActionResult> GdprToolsExport()
        {
            var user = await _workContext.GetCurrentUserAsync();
            if (!await _userService.IsRegisteredAsync(user))
                return Challenge();

            if (!_gdprSettings.GdprEnabled)
                return RedirectToRoute("UserInfo");

            //log
            await _gdprService.InsertLogAsync(user, 0, GdprRequestType.ExportData, await _localizationService.GetResourceAsync("Gdpr.Exported"));

            var store = await _storeContext.GetCurrentStoreAsync();

            //export
            var bytes = await _exportManager.ExportUserGdprInfoToXlsxAsync(user, store.Id);

            return File(bytes, MimeTypes.TextXlsx, "userdata.xlsx");
        }

        [HttpPost, ActionName("GdprTools")]
        [FormValueRequired("delete-account")]
        public virtual async Task<IActionResult> GdprToolsDelete()
        {
            var user = await _workContext.GetCurrentUserAsync();
            if (!await _userService.IsRegisteredAsync(user))
                return Challenge();

            if (!_gdprSettings.GdprEnabled)
                return RedirectToRoute("UserInfo");

            //log
            await _gdprService.InsertLogAsync(user, 0, GdprRequestType.DeleteUser, await _localizationService.GetResourceAsync("Gdpr.DeleteRequested"));

            var model = await _userModelFactory.PrepareGdprToolsModelAsync();
            model.Result = await _localizationService.GetResourceAsync("Gdpr.DeleteRequested.Success");

            return View(model);
        }

        #endregion

        #region Check gift card balance

        //check gift card balance page
        //available even when a store is closed
        [CheckAccessClosedStore(ignore: true)]
        public virtual async Task<IActionResult> CheckGiftCardBalance()
        {
            if (!(_captchaSettings.Enabled && _userSettings.AllowUsersToCheckGiftCardBalance))
            {
                return RedirectToRoute("UserInfo");
            }

            var model = await _userModelFactory.PrepareCheckGiftCardBalanceModelAsync();

            return View(model);
        }

        [HttpPost, ActionName("CheckGiftCardBalance")]
        [FormValueRequired("checkbalancegiftcard")]
        [ValidateCaptcha]
        public virtual async Task<IActionResult> CheckBalance(CheckGiftCardBalanceModel model, bool captchaValid)
        {
            //validate CAPTCHA
            if (_captchaSettings.Enabled && !captchaValid)
            {
                ModelState.AddModelError("", await _localizationService.GetResourceAsync("Common.WrongCaptchaMessage"));
            }

            if (ModelState.IsValid)
            {
                var giftCard = (await _giftCardService.GetAllGiftCardsAsync(giftCardCouponCode: model.GiftCardCode)).FirstOrDefault();
                if (giftCard != null && await _giftCardService.IsGiftCardValidAsync(giftCard))
                {
                    var remainingAmount = await _currencyService.ConvertFromPrimaryStoreCurrencyAsync(await _giftCardService.GetGiftCardRemainingAmountAsync(giftCard), await _workContext.GetWorkingCurrencyAsync());
                    model.Result = await _priceFormatter.FormatPriceAsync(remainingAmount, true, false);
                }
                else
                {
                    model.Message = await _localizationService.GetResourceAsync("CheckGiftCardBalance.GiftCardCouponCode.Invalid");
                }
            }

            return View(model);
        }

        #endregion

        #region Multi-factor Authentication

        //available even when a store is closed
        [CheckAccessClosedStore(ignore: true)]
        public virtual async Task<IActionResult> MultiFactorAuthentication()
        {
            if (!await _multiFactorAuthenticationPluginManager.HasActivePluginsAsync())
            {
                return RedirectToRoute("UserInfo");
            }

            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.EnableMultiFactorAuthentication))
                return RedirectToRoute("UserInfo");

            var model = new MultiFactorAuthenticationModel();
            model = await _userModelFactory.PrepareMultiFactorAuthenticationModelAsync(model);
            return View(model);
        }

        [HttpPost]
        public virtual async Task<IActionResult> MultiFactorAuthentication(MultiFactorAuthenticationModel model, IFormCollection form)
        {
            var user = await _workContext.GetCurrentUserAsync();
            if (!await _userService.IsRegisteredAsync(user))
                return Challenge();

            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.EnableMultiFactorAuthentication))
                return RedirectToRoute("UserInfo");

            try
            {
                if (ModelState.IsValid)
                {
                    //save MultiFactorIsEnabledAttribute
                    if (!model.IsEnabled)
                    {
                        if (!_multiFactorAuthenticationSettings.ForceMultifactorAuthentication)
                        {
                            await _genericAttributeService
                                .SaveAttributeAsync(user, TvProgUserDefaults.SelectedMultiFactorAuthenticationProviderAttribute, string.Empty);

                            //raise change multi-factor authentication provider event       
                            await _eventPublisher.PublishAsync(new UserChangeMultiFactorAuthenticationProviderEvent(user));
                        }
                        else
                        {
                            model = await _userModelFactory.PrepareMultiFactorAuthenticationModelAsync(model);
                            model.Message = await _localizationService.GetResourceAsync("Account.MultiFactorAuthentication.Warning.ForceActivation");
                            return View(model);
                        }
                    }
                    else
                    {
                        //save selected multi-factor authentication provider
                        var selectedProvider = await ParseSelectedProviderAsync(form);
                        var lastSavedProvider = await _genericAttributeService.GetAttributeAsync<string>(user, TvProgUserDefaults.SelectedMultiFactorAuthenticationProviderAttribute);
                        if (string.IsNullOrEmpty(selectedProvider) && !string.IsNullOrEmpty(lastSavedProvider))
                        {
                            selectedProvider = lastSavedProvider;
                        }

                        if (selectedProvider != lastSavedProvider)
                        {
                            await _genericAttributeService.SaveAttributeAsync(user, TvProgUserDefaults.SelectedMultiFactorAuthenticationProviderAttribute, selectedProvider);

                            //raise change multi-factor authentication provider event       
                            await _eventPublisher.PublishAsync(new UserChangeMultiFactorAuthenticationProviderEvent(user));
                        }
                    }

                    return RedirectToRoute("MultiFactorAuthenticationSettings");
                }
            }
            catch (Exception exc)
            {
                ModelState.AddModelError("", exc.Message);
            }

            //If we got this far, something failed, redisplay form
            model = await _userModelFactory.PrepareMultiFactorAuthenticationModelAsync(model);
            return View(model);
        }

        public virtual async Task<IActionResult> ConfigureMultiFactorAuthenticationProvider(string providerSysName)
        {
            if (!await _userService.IsRegisteredAsync(await _workContext.GetCurrentUserAsync()))
                return Challenge();

            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.EnableMultiFactorAuthentication))
                return RedirectToRoute("UserInfo");

            var model = new MultiFactorAuthenticationProviderModel();
            model = await _userModelFactory.PrepareMultiFactorAuthenticationProviderModelAsync(model, providerSysName);

            return View(model);
        }

        #endregion

        #endregion
    }
}