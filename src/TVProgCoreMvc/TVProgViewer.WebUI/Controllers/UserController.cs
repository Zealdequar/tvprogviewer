using System;
using System.Linq;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Primitives;
using TVProgViewer.Core;
using TVProgViewer.Core.Domain;
using TVProgViewer.Core.Domain.Catalog;
using TVProgViewer.Core.Domain.Common;
using TVProgViewer.Core.Domain.Users;
using TVProgViewer.Core.Domain.Forums;
using TVProgViewer.Core.Domain.Gdpr;
using TVProgViewer.Core.Domain.Localization;
using TVProgViewer.Core.Domain.Media;
using TVProgViewer.Core.Domain.Messages;
using TVProgViewer.Core.Domain.Security;
using TVProgViewer.Core.Domain.Tax;
using TVProgViewer.Core.Http;
using TVProgViewer.Services.Authentication;
using TVProgViewer.Services.Authentication.External;
using TVProgViewer.Services.Catalog;
using TVProgViewer.Services.Common;
using TVProgViewer.Services.Users;
using TVProgViewer.Services.Directory;
using TVProgViewer.Services.Events;
using TVProgViewer.Services.ExportImport;
using TVProgViewer.Services.Gdpr;
using TVProgViewer.Services.Helpers;
using TVProgViewer.Services.Localization;
using TVProgViewer.Services.Logging;
using TVProgViewer.Services.Media;
using TVProgViewer.Services.Messages;
using TVProgViewer.Services.Orders;
using TVProgViewer.Services.Tax;
using TVProgViewer.WebUI.Extensions;
using TVProgViewer.WebUI.Factories;
using TVProgViewer.Web.Framework;
using TVProgViewer.Web.Framework.Controllers;
using TVProgViewer.Web.Framework.Mvc.Filters;
using TVProgViewer.Web.Framework.Security;
using TVProgViewer.Web.Framework.Validators;
using TVProgViewer.WebUI.Models.User;

namespace TVProgViewer.WebUI.Controllers
{
    [AutoValidateAntiforgeryToken]
    public partial class UserController : BasePublicController
    {
        #region Fields

        private readonly AddressSettings _addressSettings;
        private readonly CaptchaSettings _captchaSettings;
        private readonly UserSettings _userSettings;
        private readonly DateTimeSettings _dateTimeSettings;
        private readonly IDownloadService _downloadService;
        private readonly ForumSettings _forumSettings;
        private readonly GdprSettings _gdprSettings;
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
        private readonly IEventPublisher _eventPublisher;
        private readonly IExportManager _exportManager;
        private readonly IExternalAuthenticationService _externalAuthenticationService;
        private readonly IGdprService _gdprService;
        private readonly IGenericAttributeService _genericAttributeService;
        private readonly IGiftCardService _giftCardService;
        private readonly ILocalizationService _localizationService;
        private readonly ILogger _logger;
        private readonly INewsLetterSubscriptionService _newsLetterSubscriptionService;
        private readonly IOrderService _orderService;
        private readonly IPictureService _pictureService;
        private readonly IPriceFormatter _priceFormatter;
        private readonly IProductService _productService;
        private readonly IShoppingCartService _shoppingCartService;
        private readonly IStateProvinceService _stateProvinceService;
        private readonly IStoreContext _storeContext;
        private readonly ITaxService _taxService;
        private readonly IWebHelper _webHelper;
        private readonly IWorkContext _workContext;
        private readonly IWorkflowMessageService _workflowMessageService;
        private readonly LocalizationSettings _localizationSettings;
        private readonly MediaSettings _mediaSettings;
        private readonly StoreInformationSettings _storeInformationSettings;
        private readonly TaxSettings _taxSettings;

        #endregion

        #region Ctor

        public UserController(AddressSettings addressSettings,
            CaptchaSettings captchaSettings,
            UserSettings userSettings,
            DateTimeSettings dateTimeSettings,
            IDownloadService downloadService,
            ForumSettings forumSettings,
            GdprSettings gdprSettings,
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
            IEventPublisher eventPublisher,
            IExportManager exportManager,
            IExternalAuthenticationService externalAuthenticationService,
            IGdprService gdprService,
            IGenericAttributeService genericAttributeService,
            IGiftCardService giftCardService,
            ILocalizationService localizationService,
            ILogger logger,
            INewsLetterSubscriptionService newsLetterSubscriptionService,
            IOrderService orderService,
            IPictureService pictureService,
            IPriceFormatter priceFormatter,
            IProductService productService,
            IShoppingCartService shoppingCartService,
            IStateProvinceService stateProvinceService,
            IStoreContext storeContext,
            ITaxService taxService,
            IWebHelper webHelper,
            IWorkContext workContext,
            IWorkflowMessageService workflowMessageService,
            LocalizationSettings localizationSettings,
            MediaSettings mediaSettings,
            StoreInformationSettings storeInformationSettings,
            TaxSettings taxSettings)
        {
            _addressSettings = addressSettings;
            _captchaSettings = captchaSettings;
            _userSettings = userSettings;
            _dateTimeSettings = dateTimeSettings;
            _downloadService = downloadService;
            _forumSettings = forumSettings;
            _gdprSettings = gdprSettings;
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
            _eventPublisher = eventPublisher;
            _exportManager = exportManager;
            _externalAuthenticationService = externalAuthenticationService;
            _gdprService = gdprService;
            _genericAttributeService = genericAttributeService;
            _giftCardService = giftCardService;
            _localizationService = localizationService;
            _logger = logger;
            _newsLetterSubscriptionService = newsLetterSubscriptionService;
            _orderService = orderService;
            _pictureService = pictureService;
            _priceFormatter = priceFormatter;
            _productService = productService;
            _shoppingCartService = shoppingCartService;
            _stateProvinceService = stateProvinceService;
            _storeContext = storeContext;
            _taxService = taxService;
            _webHelper = webHelper;
            _workContext = workContext;
            _workflowMessageService = workflowMessageService;
            _localizationSettings = localizationSettings;
            _mediaSettings = mediaSettings;
            _storeInformationSettings = storeInformationSettings;
            _taxSettings = taxSettings;
        }

        #endregion

        #region Utilities

        protected virtual string ParseCustomUserAttributes(IFormCollection form)
        {
            if (form == null)
                throw new ArgumentNullException(nameof(form));

            var attributesXml = "";
            var attributes = _userAttributeService.GetAllUserAttributes();
            foreach (var attribute in attributes)
            {
                var controlId = $"{TvProgAttributePrefixDefaults.User}{attribute.Id}";
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
                            var attributeValues = _userAttributeService.GetUserAttributeValues(attribute.Id);
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

        protected virtual void LogGdpr(User user, UserInfoModel oldUserInfoModel,
            UserInfoModel newUserInfoModel, IFormCollection form)
        {
            try
            {
                //consents
                var consents = _gdprService.GetAllConsents().Where(consent => consent.DisplayOnUserInfoPage).ToList();
                foreach (var consent in consents)
                {
                    var previousConsentValue = _gdprService.IsConsentAccepted(consent.Id, _workContext.CurrentUser.Id);
                    var controlId = $"consent{consent.Id}";
                    var cbConsent = form[controlId];
                    if (!StringValues.IsNullOrEmpty(cbConsent) && cbConsent.ToString().Equals("on"))
                    {
                        //agree
                        if (!previousConsentValue.HasValue || !previousConsentValue.Value)
                        {
                            _gdprService.InsertLog(user, consent.Id, GdprRequestType.ConsentAgree, consent.Message);
                        }
                    }
                    else
                    {
                        //disagree
                        if (!previousConsentValue.HasValue || previousConsentValue.Value)
                        {
                            _gdprService.InsertLog(user, consent.Id, GdprRequestType.ConsentDisagree, consent.Message);
                        }
                    }
                }

                //newsletter subscriptions
                if (_gdprSettings.LogNewsletterConsent)
                {
                    if (oldUserInfoModel.Newsletter && !newUserInfoModel.Newsletter)
                        _gdprService.InsertLog(user, 0, GdprRequestType.ConsentDisagree, _localizationService.GetResource("Gdpr.Consent.Newsletter"));
                    if (!oldUserInfoModel.Newsletter && newUserInfoModel.Newsletter)
                        _gdprService.InsertLog(user, 0, GdprRequestType.ConsentAgree, _localizationService.GetResource("Gdpr.Consent.Newsletter"));
                }

                //user profile changes
                if (!_gdprSettings.LogUserProfileChanges)
                    return;

                if (oldUserInfoModel.Gender != newUserInfoModel.Gender)
                    _gdprService.InsertLog(user, 0, GdprRequestType.ProfileChanged, $"{_localizationService.GetResource("Account.Fields.Gender")} = {newUserInfoModel.Gender}");

                if (oldUserInfoModel.FirstName != newUserInfoModel.FirstName)
                    _gdprService.InsertLog(user, 0, GdprRequestType.ProfileChanged, $"{_localizationService.GetResource("Account.Fields.FirstName")} = {newUserInfoModel.FirstName}");

                if (oldUserInfoModel.LastName != newUserInfoModel.LastName)
                    _gdprService.InsertLog(user, 0, GdprRequestType.ProfileChanged, $"{_localizationService.GetResource("Account.Fields.LastName")} = {newUserInfoModel.LastName}");

                if (oldUserInfoModel.ParseDateOfBirth() != newUserInfoModel.ParseDateOfBirth())
                    _gdprService.InsertLog(user, 0, GdprRequestType.ProfileChanged, $"{_localizationService.GetResource("Account.Fields.DateOfBirth")} = {newUserInfoModel.ParseDateOfBirth().ToString()}");

                if (oldUserInfoModel.Email != newUserInfoModel.Email)
                    _gdprService.InsertLog(user, 0, GdprRequestType.ProfileChanged, $"{_localizationService.GetResource("Account.Fields.Email")} = {newUserInfoModel.Email}");

                if (oldUserInfoModel.Company != newUserInfoModel.Company)
                    _gdprService.InsertLog(user, 0, GdprRequestType.ProfileChanged, $"{_localizationService.GetResource("Account.Fields.Company")} = {newUserInfoModel.Company}");

                if (oldUserInfoModel.StreetAddress != newUserInfoModel.StreetAddress)
                    _gdprService.InsertLog(user, 0, GdprRequestType.ProfileChanged, $"{_localizationService.GetResource("Account.Fields.StreetAddress")} = {newUserInfoModel.StreetAddress}");

                if (oldUserInfoModel.StreetAddress2 != newUserInfoModel.StreetAddress2)
                    _gdprService.InsertLog(user, 0, GdprRequestType.ProfileChanged, $"{_localizationService.GetResource("Account.Fields.StreetAddress2")} = {newUserInfoModel.StreetAddress2}");

                if (oldUserInfoModel.ZipPostalCode != newUserInfoModel.ZipPostalCode)
                    _gdprService.InsertLog(user, 0, GdprRequestType.ProfileChanged, $"{_localizationService.GetResource("Account.Fields.ZipPostalCode")} = {newUserInfoModel.ZipPostalCode}");

                if (oldUserInfoModel.City != newUserInfoModel.City)
                    _gdprService.InsertLog(user, 0, GdprRequestType.ProfileChanged, $"{_localizationService.GetResource("Account.Fields.City")} = {newUserInfoModel.City}");

                if (oldUserInfoModel.County != newUserInfoModel.County)
                    _gdprService.InsertLog(user, 0, GdprRequestType.ProfileChanged, $"{_localizationService.GetResource("Account.Fields.County")} = {newUserInfoModel.County}");

                if (oldUserInfoModel.CountryId != newUserInfoModel.CountryId)
                {
                    var countryName = _countryService.GetCountryById(newUserInfoModel.CountryId)?.Name;
                    _gdprService.InsertLog(user, 0, GdprRequestType.ProfileChanged, $"{_localizationService.GetResource("Account.Fields.Country")} = {countryName}");
                }

                if (oldUserInfoModel.StateProvinceId != newUserInfoModel.StateProvinceId)
                {
                    var stateProvinceName = _stateProvinceService.GetStateProvinceById(newUserInfoModel.StateProvinceId)?.Name;
                    _gdprService.InsertLog(user, 0, GdprRequestType.ProfileChanged, $"{_localizationService.GetResource("Account.Fields.StateProvince")} = {stateProvinceName}");
                }
            }
            catch (Exception exception)
            {
                _logger.Error(exception.Message, exception, user);
            }
        }

        #endregion

        #region Methods

        #region Login / logout

        [HttpsRequirement(SslRequirement.Yes)]
        //available even when a store is closed
        [CheckAccessClosedStore(true)]
        //available even when navigation is not allowed
        [CheckAccessPublicStore(true)]
        public virtual IActionResult Login(bool? checkoutAsGuest)
        {
            var model = _userModelFactory.PrepareLoginModel(checkoutAsGuest);
            return View(model);
        }

        [HttpPost]
        [ValidateCaptcha]
        //available even when a store is closed
        [CheckAccessClosedStore(true)]
        //available even when navigation is not allowed
        [CheckAccessPublicStore(true)]
        public virtual IActionResult Login(LoginModel model, string returnUrl, bool captchaValid)
        {
            //validate CAPTCHA
            if (_captchaSettings.Enabled && _captchaSettings.ShowOnLoginPage && !captchaValid)
            {
                ModelState.AddModelError("", _localizationService.GetResource("Common.WrongCaptchaMessage"));
            }

            if (ModelState.IsValid)
            {
                if (_userSettings.UsernamesEnabled && model.Username != null)
                {
                    model.Username = model.Username.Trim();
                }
                var loginResult = _userRegistrationService.ValidateUser(_userSettings.UsernamesEnabled ? model.Username : model.Email, model.Password);
                switch (loginResult)
                {
                    case UserLoginResults.Successful:
                        {
                            var user = _userSettings.UsernamesEnabled
                                ? _userService.GetUserByUsername(model.Username)
                                : _userService.GetUserByEmail(model.Email);

                            //migrate shopping cart
                            _shoppingCartService.MigrateShoppingCart(_workContext.CurrentUser, user, true);

                            //sign in new user
                            _authenticationService.SignIn(user, model.RememberMe);

                            //raise event       
                            _eventPublisher.Publish(new UserLoggedinEvent(user));

                            //activity log
                            _userActivityService.InsertActivity(user, "PublicStore.Login",
                                _localizationService.GetResource("ActivityLog.PublicStore.Login"), user);

                            if (string.IsNullOrEmpty(returnUrl) || !Url.IsLocalUrl(returnUrl))
                                return RedirectToRoute("Homepage");

                            return Redirect(returnUrl);
                        }
                    case UserLoginResults.UserNotExist:
                        ModelState.AddModelError("", _localizationService.GetResource("Account.Login.WrongCredentials.UserNotExist"));
                        break;
                    case UserLoginResults.Deleted:
                        ModelState.AddModelError("", _localizationService.GetResource("Account.Login.WrongCredentials.Deleted"));
                        break;
                    case UserLoginResults.NotActive:
                        ModelState.AddModelError("", _localizationService.GetResource("Account.Login.WrongCredentials.NotActive"));
                        break;
                    case UserLoginResults.NotRegistered:
                        ModelState.AddModelError("", _localizationService.GetResource("Account.Login.WrongCredentials.NotRegistered"));
                        break;
                    case UserLoginResults.LockedOut:
                        ModelState.AddModelError("", _localizationService.GetResource("Account.Login.WrongCredentials.LockedOut"));
                        break;
                    case UserLoginResults.WrongPassword:
                    default:
                        ModelState.AddModelError("", _localizationService.GetResource("Account.Login.WrongCredentials"));
                        break;
                }
            }

            //If we got this far, something failed, redisplay form
            model = _userModelFactory.PrepareLoginModel(model.CheckoutAsGuest);
            return View(model);
        }

        //available even when a store is closed
        [CheckAccessClosedStore(true)]
        //available even when navigation is not allowed
        [CheckAccessPublicStore(true)]
        public virtual IActionResult Logout()
        {
            if (_workContext.OriginalUserIfImpersonated != null)
            {
                //activity log
                _userActivityService.InsertActivity(_workContext.OriginalUserIfImpersonated, "Impersonation.Finished",
                    string.Format(_localizationService.GetResource("ActivityLog.Impersonation.Finished.StoreOwner"),
                        _workContext.CurrentUser.Email, _workContext.CurrentUser.Id),
                    _workContext.CurrentUser);

                _userActivityService.InsertActivity("Impersonation.Finished",
                    string.Format(_localizationService.GetResource("ActivityLog.Impersonation.Finished.User"),
                        _workContext.OriginalUserIfImpersonated.Email, _workContext.OriginalUserIfImpersonated.Id),
                    _workContext.OriginalUserIfImpersonated);

                //logout impersonated user
                _genericAttributeService
                    .SaveAttribute<int?>(_workContext.OriginalUserIfImpersonated, TvProgUserDefaults.ImpersonatedUserIdAttribute, null);

                //redirect back to user details page (admin area)
                return RedirectToAction("Edit", "User", new { id = _workContext.CurrentUser.Id, area = AreaNames.Admin });
            }

            //activity log
            _userActivityService.InsertActivity(_workContext.CurrentUser, "PublicStore.Logout",
                _localizationService.GetResource("ActivityLog.PublicStore.Logout"), _workContext.CurrentUser);

            //standard logout 
            _authenticationService.SignOut();

            //raise logged out event       
            _eventPublisher.Publish(new UserLoggedOutEvent(_workContext.CurrentUser));

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

        [HttpsRequirement(SslRequirement.Yes)]
        //available even when navigation is not allowed
        [CheckAccessPublicStore(true)]
        public virtual IActionResult PasswordRecovery()
        {
            var model = new PasswordRecoveryModel();
            model = _userModelFactory.PreparePasswordRecoveryModel(model);

            return View(model);
        }

        [ValidateCaptcha]
        [HttpPost, ActionName("PasswordRecovery")]
        [FormValueRequired("send-email")]
        //available even when navigation is not allowed
        [CheckAccessPublicStore(true)]
        public virtual IActionResult PasswordRecoverySend(PasswordRecoveryModel model, bool captchaValid)
        {
            // validate CAPTCHA
            if (_captchaSettings.Enabled && _captchaSettings.ShowOnForgotPasswordPage && !captchaValid)
            {
                ModelState.AddModelError("", _localizationService.GetResource("Common.WrongCaptchaMessage"));
            }

            if (ModelState.IsValid)
            {
                var user = _userService.GetUserByEmail(model.Email);
                if (user != null && user.Active && user.Deleted == null)
                {
                    //save token and current date
                    var passwordRecoveryToken = Guid.NewGuid();
                    _genericAttributeService.SaveAttribute(user, TvProgUserDefaults.PasswordRecoveryTokenAttribute,
                        passwordRecoveryToken.ToString());
                    DateTime? generatedDateTime = DateTime.UtcNow;
                    _genericAttributeService.SaveAttribute(user,
                        TvProgUserDefaults.PasswordRecoveryTokenDateGeneratedAttribute, generatedDateTime);

                    //send email
                    _workflowMessageService.SendUserPasswordRecoveryMessage(user,
                        _workContext.WorkingLanguage.Id);

                    model.Result = _localizationService.GetResource("Account.PasswordRecovery.EmailHasBeenSent");
                }
                else
                {
                    model.Result = _localizationService.GetResource("Account.PasswordRecovery.EmailNotFound");
                }
            }

            model = _userModelFactory.PreparePasswordRecoveryModel(model);
            return View(model);
        }

        [HttpsRequirement(SslRequirement.Yes)]
        //available even when navigation is not allowed
        [CheckAccessPublicStore(true)]
        public virtual IActionResult PasswordRecoveryConfirm(string token, string email, Guid guid)
        {
            //For backward compatibility with previous versions where email was used as a parameter in the URL
            var user = _userService.GetUserByEmail(email);
            if (user == null)
                user = _userService.GetUserByGuid(guid);

            if (user == null)
                return RedirectToRoute("Homepage");

            if (string.IsNullOrEmpty(_genericAttributeService.GetAttribute<string>(user, TvProgUserDefaults.PasswordRecoveryTokenAttribute)))
            {
                return View(new PasswordRecoveryConfirmModel
                {
                    DisablePasswordChanging = true,
                    Result = _localizationService.GetResource("Account.PasswordRecovery.PasswordAlreadyHasBeenChanged")
                });
            }

            var model = _userModelFactory.PreparePasswordRecoveryConfirmModel();

            //validate token
            if (!_userService.IsPasswordRecoveryTokenValid(user, token))
            {
                model.DisablePasswordChanging = true;
                model.Result = _localizationService.GetResource("Account.PasswordRecovery.WrongToken");
            }

            //validate token expiration date
            if (_userService.IsPasswordRecoveryLinkExpired(user))
            {
                model.DisablePasswordChanging = true;
                model.Result = _localizationService.GetResource("Account.PasswordRecovery.LinkExpired");
            }

            return View(model);
        }

        [HttpPost, ActionName("PasswordRecoveryConfirm")]
        [FormValueRequired("set-password")]
        //available even when navigation is not allowed
        [CheckAccessPublicStore(true)]
        public virtual IActionResult PasswordRecoveryConfirmPOST(string token, string email, Guid guid, PasswordRecoveryConfirmModel model)
        {
            //For backward compatibility with previous versions where email was used as a parameter in the URL
            var user = _userService.GetUserByEmail(email);
            if (user == null)
                user = _userService.GetUserByGuid(guid);

            if (user == null)
                return RedirectToRoute("Homepage");

            //validate token
            if (!_userService.IsPasswordRecoveryTokenValid(user, token))
            {
                model.DisablePasswordChanging = true;
                model.Result = _localizationService.GetResource("Account.PasswordRecovery.WrongToken");
                return View(model);
            }

            //validate token expiration date
            if (_userService.IsPasswordRecoveryLinkExpired(user))
            {
                model.DisablePasswordChanging = true;
                model.Result = _localizationService.GetResource("Account.PasswordRecovery.LinkExpired");
                return View(model);
            }

            if (ModelState.IsValid)
            {
                var response = _userRegistrationService.ChangePassword(new ChangePasswordRequest(user.Email,
                    false, _userSettings.DefaultPasswordFormat, model.NewPassword));
                if (response.Success)
                {
                    _genericAttributeService.SaveAttribute(user, TvProgUserDefaults.PasswordRecoveryTokenAttribute, "");

                    model.DisablePasswordChanging = true;
                    model.Result = _localizationService.GetResource("Account.PasswordRecovery.PasswordHasBeenChanged");
                }
                else
                {
                    model.Result = response.Errors.FirstOrDefault();
                }

                return View(model);
            }

            //If we got this far, something failed, redisplay form
            return View(model);
        }

        #endregion     

        #region Register

        [HttpsRequirement(SslRequirement.Yes)]
        //available even when navigation is not allowed
        [CheckAccessPublicStore(true)]
        public virtual IActionResult Register()
        {
            //check whether registration is allowed
            if (_userSettings.UserRegistrationType == UserRegistrationType.Disabled)
                return RedirectToRoute("RegisterResult", new { resultId = (int)UserRegistrationType.Disabled });

            var model = new RegisterModel();
            model = _userModelFactory.PrepareRegisterModel(model, false, setDefaultValues: true);

            return View(model);
        }

        [HttpPost]
        [ValidateCaptcha]
        [ValidateHoneypot]
        //available even when navigation is not allowed
        [CheckAccessPublicStore(true)]
        public virtual IActionResult Register(RegisterModel model, string returnUrl, bool captchaValid, IFormCollection form)
        {
            //check whether registration is allowed
            if (_userSettings.UserRegistrationType == UserRegistrationType.Disabled)
                return RedirectToRoute("RegisterResult", new { resultId = (int)UserRegistrationType.Disabled });

            if (_userService.IsRegistered(_workContext.CurrentUser))
            {
                //Already registered user. 
                _authenticationService.SignOut();

                //raise logged out event       
                _eventPublisher.Publish(new UserLoggedOutEvent(_workContext.CurrentUser));

                //Save a new record
                _workContext.CurrentUser = _userService.InsertGuestUser();
            }
            var user = _workContext.CurrentUser;
            user.RegisteredInStoreId = _storeContext.CurrentStore.Id;

            //custom user attributes
            var userAttributesXml = ParseCustomUserAttributes(form);
            var userAttributeWarnings = _userAttributeParser.GetAttributeWarnings(userAttributesXml);
            foreach (var error in userAttributeWarnings)
            {
                ModelState.AddModelError("", error);
            }

            //validate CAPTCHA
            if (_captchaSettings.Enabled && _captchaSettings.ShowOnRegistrationPage && !captchaValid)
            {
                ModelState.AddModelError("", _localizationService.GetResource("Common.WrongCaptchaMessage"));
            }

            if (ModelState.IsValid)
            {
                if (_userSettings.UsernamesEnabled && model.Username != null)
                {
                    model.Username = model.Username.Trim();
                }

                var isApproved = _userSettings.UserRegistrationType == UserRegistrationType.Standard;
                var registrationRequest = new UserRegistrationRequest(user,
                    model.Email,
                    _userSettings.UsernamesEnabled ? model.Username : model.Email,
                    model.Password,
                    _userSettings.DefaultPasswordFormat,
                    _storeContext.CurrentStore.Id,
                    isApproved);
                var registrationResult = _userRegistrationService.RegisterUser(registrationRequest);
                if (registrationResult.Success)
                {
                    //properties
                    if (_dateTimeSettings.AllowUsersToSetTimeZone)
                    {
                        _genericAttributeService.SaveAttribute(user, TvProgUserDefaults.TimeZoneIdAttribute, model.TimeZoneId);
                    }
                    //VAT number
                    if (_taxSettings.EuVatEnabled)
                    {
                        _genericAttributeService.SaveAttribute(user, TvProgUserDefaults.VatNumberAttribute, model.VatNumber);

                        var vatNumberStatus = _taxService.GetVatNumberStatus(model.VatNumber, out _, out var vatAddress);
                        _genericAttributeService.SaveAttribute(user, TvProgUserDefaults.VatNumberStatusIdAttribute, (int)vatNumberStatus);
                        //send VAT number admin notification
                        if (!string.IsNullOrEmpty(model.VatNumber) && _taxSettings.EuVatEmailAdminWhenNewVatSubmitted)
                            _workflowMessageService.SendNewVatSubmittedStoreOwnerNotification(user, model.VatNumber, vatAddress, _localizationSettings.DefaultAdminLanguageId);
                    }

                    //form fields
                    if (_userSettings.GenderEnabled)
                        _genericAttributeService.SaveAttribute(user, TvProgUserDefaults.GenderAttribute, model.Gender);
                    if (_userSettings.FirstNameEnabled)
                        _genericAttributeService.SaveAttribute(user, TvProgUserDefaults.FirstNameAttribute, model.FirstName);
                    if (_userSettings.LastNameEnabled)
                        _genericAttributeService.SaveAttribute(user, TvProgUserDefaults.LastNameAttribute, model.LastName);
                    if (_userSettings.DateOfBirthEnabled)
                    {
                        var dateOfBirth = model.ParseDateOfBirth();
                        _genericAttributeService.SaveAttribute(user, TvProgUserDefaults.DateOfBirthAttribute, dateOfBirth);
                    }
                    if (_userSettings.CompanyEnabled)
                        _genericAttributeService.SaveAttribute(user, TvProgUserDefaults.CompanyAttribute, model.Company);
                    if (_userSettings.StreetAddressEnabled)
                        _genericAttributeService.SaveAttribute(user, TvProgUserDefaults.StreetAddressAttribute, model.StreetAddress);
                    if (_userSettings.StreetAddress2Enabled)
                        _genericAttributeService.SaveAttribute(user, TvProgUserDefaults.StreetAddress2Attribute, model.StreetAddress2);
                    if (_userSettings.ZipPostalCodeEnabled)
                        _genericAttributeService.SaveAttribute(user, TvProgUserDefaults.ZipPostalCodeAttribute, model.ZipPostalCode);
                    if (_userSettings.CityEnabled)
                        _genericAttributeService.SaveAttribute(user, TvProgUserDefaults.CityAttribute, model.City);
                    if (_userSettings.CountyEnabled)
                        _genericAttributeService.SaveAttribute(user, TvProgUserDefaults.CountyAttribute, model.County);
                    if (_userSettings.CountryEnabled)
                        _genericAttributeService.SaveAttribute(user, TvProgUserDefaults.CountryIdAttribute, model.CountryId);
                    if (_userSettings.CountryEnabled && _userSettings.StateProvinceEnabled)
                        _genericAttributeService.SaveAttribute(user, TvProgUserDefaults.StateProvinceIdAttribute,
                            model.StateProvinceId);
                    if (_userSettings.PhoneEnabled)
                        _genericAttributeService.SaveAttribute(user, TvProgUserDefaults.PhoneAttribute, model.Phone);
                    if (_userSettings.FaxEnabled)
                        _genericAttributeService.SaveAttribute(user, TvProgUserDefaults.FaxAttribute, model.Fax);

                    //newsletter
                    if (_userSettings.NewsletterEnabled)
                    {
                        //save newsletter value
                        var newsletter = _newsLetterSubscriptionService.GetNewsLetterSubscriptionByEmailAndStoreId(model.Email, _storeContext.CurrentStore.Id);
                        if (newsletter != null)
                        {
                            if (model.Newsletter)
                            {
                                newsletter.Active = true;
                                _newsLetterSubscriptionService.UpdateNewsLetterSubscription(newsletter);

                                //GDPR
                                if (_gdprSettings.GdprEnabled && _gdprSettings.LogNewsletterConsent)
                                {
                                    _gdprService.InsertLog(user, 0, GdprRequestType.ConsentAgree, _localizationService.GetResource("Gdpr.Consent.Newsletter"));
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
                                _newsLetterSubscriptionService.InsertNewsLetterSubscription(new NewsLetterSubscription
                                {
                                    NewsLetterSubscriptionGuid = Guid.NewGuid(),
                                    Email = model.Email,
                                    Active = true,
                                    StoreId = _storeContext.CurrentStore.Id,
                                    CreatedOnUtc = DateTime.UtcNow
                                });

                                //GDPR
                                if (_gdprSettings.GdprEnabled && _gdprSettings.LogNewsletterConsent)
                                {
                                    _gdprService.InsertLog(user, 0, GdprRequestType.ConsentAgree, _localizationService.GetResource("Gdpr.Consent.Newsletter"));
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
                            _gdprService.InsertLog(user, 0, GdprRequestType.ConsentAgree, _localizationService.GetResource("Gdpr.Consent.PrivacyPolicy"));
                        }
                    }

                    //GDPR
                    if (_gdprSettings.GdprEnabled)
                    {
                        var consents = _gdprService.GetAllConsents().Where(consent => consent.DisplayDuringRegistration).ToList();
                        foreach (var consent in consents)
                        {
                            var controlId = $"consent{consent.Id}";
                            var cbConsent = form[controlId];
                            if (!StringValues.IsNullOrEmpty(cbConsent) && cbConsent.ToString().Equals("on"))
                            {
                                //agree
                                _gdprService.InsertLog(user, consent.Id, GdprRequestType.ConsentAgree, consent.Message);
                            }
                            else
                            {
                                //disagree
                                _gdprService.InsertLog(user, consent.Id, GdprRequestType.ConsentDisagree, consent.Message);
                            }
                        }
                    }

                    //save user attributes
                    _genericAttributeService.SaveAttribute(user, TvProgUserDefaults.CustomUserAttributes, userAttributesXml);

                    //login user now
                    if (isApproved)
                        _authenticationService.SignIn(user, true);

                    //insert default address (if possible)
                    var defaultAddress = new Address
                    {
                        FirstName = _genericAttributeService.GetAttribute<string>(user, TvProgUserDefaults.FirstNameAttribute),
                        LastName = _genericAttributeService.GetAttribute<string>(user, TvProgUserDefaults.LastNameAttribute),
                        Email = user.Email,
                        Company = _genericAttributeService.GetAttribute<string>(user, TvProgUserDefaults.CompanyAttribute),
                        CountryId = _genericAttributeService.GetAttribute<int>(user, TvProgUserDefaults.CountryIdAttribute) > 0
                            ? (int?)_genericAttributeService.GetAttribute<int>(user, TvProgUserDefaults.CountryIdAttribute)
                            : null,
                        StateProvinceId = _genericAttributeService.GetAttribute<int>(user, TvProgUserDefaults.StateProvinceIdAttribute) > 0
                            ? (int?)_genericAttributeService.GetAttribute<int>(user, TvProgUserDefaults.StateProvinceIdAttribute)
                            : null,
                        County = _genericAttributeService.GetAttribute<string>(user, TvProgUserDefaults.CountyAttribute),
                        City = _genericAttributeService.GetAttribute<string>(user, TvProgUserDefaults.CityAttribute),
                        Address1 = _genericAttributeService.GetAttribute<string>(user, TvProgUserDefaults.StreetAddressAttribute),
                        Address2 = _genericAttributeService.GetAttribute<string>(user, TvProgUserDefaults.StreetAddress2Attribute),
                        ZipPostalCode = _genericAttributeService.GetAttribute<string>(user, TvProgUserDefaults.ZipPostalCodeAttribute),
                        PhoneNumber = _genericAttributeService.GetAttribute<string>(user, TvProgUserDefaults.PhoneAttribute),
                        FaxNumber = _genericAttributeService.GetAttribute<string>(user, TvProgUserDefaults.FaxAttribute),
                        CreatedOnUtc = user.CreatedOnUtc
                    };
                    if (_addressService.IsAddressValid(defaultAddress))
                    {
                        //some validation
                        if (defaultAddress.CountryId == 0)
                            defaultAddress.CountryId = null;
                        if (defaultAddress.StateProvinceId == 0)
                            defaultAddress.StateProvinceId = null;
                        //set default address
                        //user.Addresses.Add(defaultAddress);

                        _addressService.InsertAddress(defaultAddress);

                        _userService.InsertUserAddress(user, defaultAddress);

                        user.BillingAddressId = defaultAddress.Id;
                        user.ShippingAddressId = defaultAddress.Id;

                        _userService.UpdateUser(user);
                    }

                    //notifications
                    if (_userSettings.NotifyNewUserRegistration)
                        _workflowMessageService.SendUserRegisteredNotificationMessage(user,
                            _localizationSettings.DefaultAdminLanguageId);

                    //raise event       
                    _eventPublisher.Publish(new UserRegisteredEvent(user));

                    switch (_userSettings.UserRegistrationType)
                    {
                        case UserRegistrationType.EmailValidation:
                            {
                                //email validation message
                                _genericAttributeService.SaveAttribute(user, TvProgUserDefaults.AccountActivationTokenAttribute, Guid.NewGuid().ToString());
                                _workflowMessageService.SendUserEmailValidationMessage(user, _workContext.WorkingLanguage.Id);

                                //result
                                return RedirectToRoute("RegisterResult",
                                    new { resultId = (int)UserRegistrationType.EmailValidation });
                            }
                        case UserRegistrationType.AdminApproval:
                            {
                                return RedirectToRoute("RegisterResult",
                                    new { resultId = (int)UserRegistrationType.AdminApproval });
                            }
                        case UserRegistrationType.Standard:
                            {
                                //send user welcome message
                                _workflowMessageService.SendUserWelcomeMessage(user, _workContext.WorkingLanguage.Id);

                                //raise event       
                                _eventPublisher.Publish(new UserActivatedEvent(user));

                                var redirectUrl = Url.RouteUrl("RegisterResult",
                                    new { resultId = (int)UserRegistrationType.Standard, returnUrl }, _webHelper.GetCurrentRequestProtocol());
                                return Redirect(redirectUrl);
                            }
                        default:
                            {
                                return RedirectToRoute("Homepage");
                            }
                    }
                }

                //errors
                foreach (var error in registrationResult.Errors)
                    ModelState.AddModelError("", error);
            }

            //If we got this far, something failed, redisplay form
            model = _userModelFactory.PrepareRegisterModel(model, true, userAttributesXml);
            return View(model);
        }

        //available even when navigation is not allowed
        [CheckAccessPublicStore(true)]
        public virtual IActionResult RegisterResult(int resultId)
        {
            var model = _userModelFactory.PrepareRegisterResultModel(resultId);
            return View(model);
        }

        //available even when navigation is not allowed
        [CheckAccessPublicStore(true)]
        [HttpPost]
        [IgnoreAntiforgeryToken]
        public virtual IActionResult RegisterResult(string returnUrl)
        {
            if (string.IsNullOrEmpty(returnUrl) || !Url.IsLocalUrl(returnUrl))
                return RedirectToRoute("Homepage");

            return Redirect(returnUrl);
        }

        [HttpPost]
        //available even when navigation is not allowed
        [CheckAccessPublicStore(true)]
        public virtual IActionResult CheckUsernameAvailability(string username)
        {
            var usernameAvailable = false;
            var statusText = _localizationService.GetResource("Account.CheckUsernameAvailability.NotAvailable");

            if (!UsernamePropertyValidator.IsValid(username, _userSettings))
            {
                statusText = _localizationService.GetResource("Account.Fields.Username.NotValid");
            }
            else if (_userSettings.UsernamesEnabled && !string.IsNullOrWhiteSpace(username))
            {
                if (_workContext.CurrentUser != null &&
                    _workContext.CurrentUser.UserName != null &&
                    _workContext.CurrentUser.UserName.Equals(username, StringComparison.InvariantCultureIgnoreCase))
                {
                    statusText = _localizationService.GetResource("Account.CheckUsernameAvailability.CurrentUsername");
                }
                else
                {
                    var user = _userService.GetUserByUsername(username);
                    if (user == null)
                    {
                        statusText = _localizationService.GetResource("Account.CheckUsernameAvailability.Available");
                        usernameAvailable = true;
                    }
                }
            }

            return Json(new { Available = usernameAvailable, Text = statusText });
        }

        [HttpsRequirement(SslRequirement.Yes)]
        //available even when navigation is not allowed
        [CheckAccessPublicStore(true)]
        public virtual IActionResult AccountActivation(string token, string email, Guid guid)
        {
            //For backward compatibility with previous versions where email was used as a parameter in the URL
            var user = _userService.GetUserByEmail(email);
            if (user == null)
                user = _userService.GetUserByGuid(guid);

            if (user == null)
                return RedirectToRoute("Homepage");

            var cToken = _genericAttributeService.GetAttribute<string>(user, TvProgUserDefaults.AccountActivationTokenAttribute);
            if (string.IsNullOrEmpty(cToken))
                return
                    View(new AccountActivationModel
                    {
                        Result = _localizationService.GetResource("Account.AccountActivation.AlreadyActivated")
                    });

            if (!cToken.Equals(token, StringComparison.InvariantCultureIgnoreCase))
                return RedirectToRoute("Homepage");

            //activate user account
            user.Active = true;
            _userService.UpdateUser(user);
            _genericAttributeService.SaveAttribute(user, TvProgUserDefaults.AccountActivationTokenAttribute, "");
            //send welcome message
            _workflowMessageService.SendUserWelcomeMessage(user, _workContext.WorkingLanguage.Id);

            //raise event       
            _eventPublisher.Publish(new UserActivatedEvent(user));

            var model = new AccountActivationModel
            {
                Result = _localizationService.GetResource("Account.AccountActivation.Activated")
            };
            return View(model);
        }

        #endregion

        #region My account / Info

        [HttpsRequirement(SslRequirement.Yes)]
        public virtual IActionResult Info()
        {
            if (!_userService.IsRegistered(_workContext.CurrentUser))
                return Challenge();

            var model = new UserInfoModel();
            model = _userModelFactory.PrepareUserInfoModel(model, _workContext.CurrentUser, false);

            return View(model);
        }

        [HttpPost]
        public virtual IActionResult Info(UserInfoModel model, IFormCollection form)
        {
            if (!_userService.IsRegistered(_workContext.CurrentUser))
                return Challenge();

            var oldUserModel = new UserInfoModel();

            var user = _workContext.CurrentUser;

            //get user info model before changes for gdpr log
            if (_gdprSettings.GdprEnabled & _gdprSettings.LogUserProfileChanges)
                oldUserModel = _userModelFactory.PrepareUserInfoModel(oldUserModel, user, false);

            //custom user attributes
            var userAttributesXml = ParseCustomUserAttributes(form);
            var userAttributeWarnings = _userAttributeParser.GetAttributeWarnings(userAttributesXml);
            foreach (var error in userAttributeWarnings)
            {
                ModelState.AddModelError("", error);
            }

            try
            {
                if (ModelState.IsValid)
                {
                    //username 
                    if (_userSettings.UsernamesEnabled && _userSettings.AllowUsersToChangeUsernames)
                    {
                        if (
                            !user.UserName.Equals(model.Username.Trim(), StringComparison.InvariantCultureIgnoreCase))
                        {
                            //change username
                            _userRegistrationService.SetUsername(user, model.Username.Trim());

                            //re-authenticate
                            //do not authenticate users in impersonation mode
                            if (_workContext.OriginalUserIfImpersonated == null)
                                _authenticationService.SignIn(user, true);
                        }
                    }
                    //email
                    if (!user.Email.Equals(model.Email.Trim(), StringComparison.InvariantCultureIgnoreCase))
                    {
                        //change email
                        var requireValidation = _userSettings.UserRegistrationType == UserRegistrationType.EmailValidation;
                        _userRegistrationService.SetEmail(user, model.Email.Trim(), requireValidation);

                        //do not authenticate users in impersonation mode
                        if (_workContext.OriginalUserIfImpersonated == null)
                        {
                            //re-authenticate (if usernames are disabled)
                            if (!_userSettings.UsernamesEnabled && !requireValidation)
                                _authenticationService.SignIn(user, true);
                        }
                    }

                    //properties
                    if (_dateTimeSettings.AllowUsersToSetTimeZone)
                    {
                        _genericAttributeService.SaveAttribute(user, TvProgUserDefaults.TimeZoneIdAttribute,
                            model.TimeZoneId);
                    }
                    //VAT number
                    if (_taxSettings.EuVatEnabled)
                    {
                        var prevVatNumber = _genericAttributeService.GetAttribute<string>(user, TvProgUserDefaults.VatNumberAttribute);

                        _genericAttributeService.SaveAttribute(user, TvProgUserDefaults.VatNumberAttribute,
                            model.VatNumber);
                        if (prevVatNumber != model.VatNumber)
                        {
                            var vatNumberStatus = _taxService.GetVatNumberStatus(model.VatNumber, out _, out var vatAddress);
                            _genericAttributeService.SaveAttribute(user, TvProgUserDefaults.VatNumberStatusIdAttribute, (int)vatNumberStatus);
                            //send VAT number admin notification
                            if (!string.IsNullOrEmpty(model.VatNumber) && _taxSettings.EuVatEmailAdminWhenNewVatSubmitted)
                                _workflowMessageService.SendNewVatSubmittedStoreOwnerNotification(user,
                                    model.VatNumber, vatAddress, _localizationSettings.DefaultAdminLanguageId);
                        }
                    }

                    //form fields
                    if (_userSettings.GenderEnabled)
                        _genericAttributeService.SaveAttribute(user, TvProgUserDefaults.GenderAttribute, model.Gender);
                    if (_userSettings.FirstNameEnabled)
                        _genericAttributeService.SaveAttribute(user, TvProgUserDefaults.FirstNameAttribute, model.FirstName);
                    if (_userSettings.LastNameEnabled)
                        _genericAttributeService.SaveAttribute(user, TvProgUserDefaults.LastNameAttribute, model.LastName);
                    if (_userSettings.DateOfBirthEnabled)
                    {
                        var dateOfBirth = model.ParseDateOfBirth();
                        _genericAttributeService.SaveAttribute(user, TvProgUserDefaults.DateOfBirthAttribute, dateOfBirth);
                    }
                    if (_userSettings.CompanyEnabled)
                        _genericAttributeService.SaveAttribute(user, TvProgUserDefaults.CompanyAttribute, model.Company);
                    if (_userSettings.StreetAddressEnabled)
                        _genericAttributeService.SaveAttribute(user, TvProgUserDefaults.StreetAddressAttribute, model.StreetAddress);
                    if (_userSettings.StreetAddress2Enabled)
                        _genericAttributeService.SaveAttribute(user, TvProgUserDefaults.StreetAddress2Attribute, model.StreetAddress2);
                    if (_userSettings.ZipPostalCodeEnabled)
                        _genericAttributeService.SaveAttribute(user, TvProgUserDefaults.ZipPostalCodeAttribute, model.ZipPostalCode);
                    if (_userSettings.CityEnabled)
                        _genericAttributeService.SaveAttribute(user, TvProgUserDefaults.CityAttribute, model.City);
                    if (_userSettings.CountyEnabled)
                        _genericAttributeService.SaveAttribute(user, TvProgUserDefaults.CountyAttribute, model.County);
                    if (_userSettings.CountryEnabled)
                        _genericAttributeService.SaveAttribute(user, TvProgUserDefaults.CountryIdAttribute, model.CountryId);
                    if (_userSettings.CountryEnabled && _userSettings.StateProvinceEnabled)
                        _genericAttributeService.SaveAttribute(user, TvProgUserDefaults.StateProvinceIdAttribute, model.StateProvinceId);
                    if (_userSettings.PhoneEnabled)
                        _genericAttributeService.SaveAttribute(user, TvProgUserDefaults.PhoneAttribute, model.Phone);
                    if (_userSettings.FaxEnabled)
                        _genericAttributeService.SaveAttribute(user, TvProgUserDefaults.FaxAttribute, model.Fax);

                    //newsletter
                    if (_userSettings.NewsletterEnabled)
                    {
                        //save newsletter value
                        var newsletter = _newsLetterSubscriptionService.GetNewsLetterSubscriptionByEmailAndStoreId(user.Email, _storeContext.CurrentStore.Id);
                        if (newsletter != null)
                        {
                            if (model.Newsletter)
                            {
                                var wasActive = newsletter.Active;
                                newsletter.Active = true;
                                _newsLetterSubscriptionService.UpdateNewsLetterSubscription(newsletter);
                            }
                            else
                            {
                                _newsLetterSubscriptionService.DeleteNewsLetterSubscription(newsletter);
                            }
                        }
                        else
                        {
                            if (model.Newsletter)
                            {
                                _newsLetterSubscriptionService.InsertNewsLetterSubscription(new NewsLetterSubscription
                                {
                                    NewsLetterSubscriptionGuid = Guid.NewGuid(),
                                    Email = user.Email,
                                    Active = true,
                                    StoreId = _storeContext.CurrentStore.Id,
                                    CreatedOnUtc = DateTime.UtcNow
                                });
                            }
                        }
                    }

                    if (_forumSettings.ForumsEnabled && _forumSettings.SignaturesEnabled)
                        _genericAttributeService.SaveAttribute(user, TvProgUserDefaults.SignatureAttribute, model.Signature);

                    //save user attributes
                    _genericAttributeService.SaveAttribute(_workContext.CurrentUser,
                        TvProgUserDefaults.CustomUserAttributes, userAttributesXml);

                    //GDPR
                    if (_gdprSettings.GdprEnabled)
                        LogGdpr(user, oldUserModel, model, form);

                    return RedirectToRoute("UserInfo");
                }
            }
            catch (Exception exc)
            {
                ModelState.AddModelError("", exc.Message);
            }

            //If we got this far, something failed, redisplay form
            model = _userModelFactory.PrepareUserInfoModel(model, user, true, userAttributesXml);
            return View(model);
        }

        [HttpPost]
        public virtual IActionResult RemoveExternalAssociation(int id)
        {
            if (!_userService.IsRegistered(_workContext.CurrentUser))
                return Challenge();

            //ensure it's our record
            var ear = _externalAuthenticationService.GetExternalAuthenticationRecordById(id);

            if (ear == null)
            {
                return Json(new
                {
                    redirect = Url.Action("Info"),
                });
            }

            _externalAuthenticationService.DeleteExternalAuthenticationRecord(ear);

            return Json(new
            {
                redirect = Url.Action("Info"),
            });
        }

        [HttpsRequirement(SslRequirement.Yes)]
        //available even when navigation is not allowed
        [CheckAccessPublicStore(true)]
        public virtual IActionResult EmailRevalidation(string token, string email, Guid guid)
        {
            //For backward compatibility with previous versions where email was used as a parameter in the URL
            var user = _userService.GetUserByEmail(email);
            if (user == null)
                user = _userService.GetUserByGuid(guid);

            if (user == null)
                return RedirectToRoute("Homepage");

            var cToken = _genericAttributeService.GetAttribute<string>(user, TvProgUserDefaults.EmailRevalidationTokenAttribute);
            if (string.IsNullOrEmpty(cToken))
                return View(new EmailRevalidationModel
                {
                    Result = _localizationService.GetResource("Account.EmailRevalidation.AlreadyChanged")
                });

            if (!cToken.Equals(token, StringComparison.InvariantCultureIgnoreCase))
                return RedirectToRoute("Homepage");

            if (string.IsNullOrEmpty(user.EmailToRevalidate))
                return RedirectToRoute("Homepage");

            if (_userSettings.UserRegistrationType != UserRegistrationType.EmailValidation)
                return RedirectToRoute("Homepage");

            //change email
            try
            {
                _userRegistrationService.SetEmail(user, user.EmailToRevalidate, false);
            }
            catch (Exception exc)
            {
                return View(new EmailRevalidationModel
                {
                    Result = _localizationService.GetResource(exc.Message)
                });
            }
            user.EmailToRevalidate = null;
            _userService.UpdateUser(user);
            _genericAttributeService.SaveAttribute(user, TvProgUserDefaults.EmailRevalidationTokenAttribute, "");

            //re-authenticate (if usernames are disabled)
            if (!_userSettings.UsernamesEnabled)
            {
                _authenticationService.SignIn(user, true);
            }

            var model = new EmailRevalidationModel()
            {
                Result = _localizationService.GetResource("Account.EmailRevalidation.Changed")
            };
            return View(model);
        }

        #endregion

        #region My account / Addresses

        [HttpsRequirement(SslRequirement.Yes)]
        public virtual IActionResult Addresses()
        {
            if (!_userService.IsRegistered(_workContext.CurrentUser))
                return Challenge();

            var model = _userModelFactory.PrepareUserAddressListModel();
            return View(model);
        }

        [HttpPost]
        [HttpsRequirement(SslRequirement.Yes)]
        public virtual IActionResult AddressDelete(int addressId)
        {
            if (!_userService.IsRegistered(_workContext.CurrentUser))
                return Challenge();

            var user = _workContext.CurrentUser;

            //find address (ensure that it belongs to the current user)
            var address = _userService.GetUserAddress(user.Id, addressId);
            if (address != null)
            {
                _userService.RemoveUserAddress(user, address);
                _userService.UpdateUser(user);
                //now delete the address record
                _addressService.DeleteAddress(address);
            }

            //redirect to the address list page
            return Json(new
            {
                redirect = Url.RouteUrl("UserAddresses"),
            });
        }

        [HttpsRequirement(SslRequirement.Yes)]
        public virtual IActionResult AddressAdd()
        {
            if (!_userService.IsRegistered(_workContext.CurrentUser))
                return Challenge();

            var model = new UserAddressEditModel();
            _addressModelFactory.PrepareAddressModel(model.Address,
                address: null,
                excludeProperties: false,
                addressSettings: _addressSettings,
                loadCountries: () => _countryService.GetAllCountries(_workContext.WorkingLanguage.Id));

            return View(model);
        }

        [HttpPost]
        public virtual IActionResult AddressAdd(UserAddressEditModel model, IFormCollection form)
        {
            if (!_userService.IsRegistered(_workContext.CurrentUser))
                return Challenge();

            //custom address attributes
            var customAttributes = _addressAttributeParser.ParseCustomAddressAttributes(form);
            var customAttributeWarnings = _addressAttributeParser.GetAttributeWarnings(customAttributes);
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


                _addressService.InsertAddress(address);

                _userService.InsertUserAddress(_workContext.CurrentUser, address);

                return RedirectToRoute("UserAddresses");
            }

            //If we got this far, something failed, redisplay form
            _addressModelFactory.PrepareAddressModel(model.Address,
                address: null,
                excludeProperties: true,
                addressSettings: _addressSettings,
                loadCountries: () => _countryService.GetAllCountries(_workContext.WorkingLanguage.Id),
                overrideAttributesXml: customAttributes);

            return View(model);
        }

        [HttpsRequirement(SslRequirement.Yes)]
        public virtual IActionResult AddressEdit(int addressId)
        {
            if (!_userService.IsRegistered(_workContext.CurrentUser))
                return Challenge();

            var user = _workContext.CurrentUser;
            //find address (ensure that it belongs to the current user)
            var address = _userService.GetUserAddress(user.Id, addressId);
            if (address == null)
                //address is not found
                return RedirectToRoute("UserAddresses");

            var model = new UserAddressEditModel();
            _addressModelFactory.PrepareAddressModel(model.Address,
                address: address,
                excludeProperties: false,
                addressSettings: _addressSettings,
                loadCountries: () => _countryService.GetAllCountries(_workContext.WorkingLanguage.Id));

            return View(model);
        }

        [HttpPost]
        public virtual IActionResult AddressEdit(UserAddressEditModel model, int addressId, IFormCollection form)
        {
            if (!_userService.IsRegistered(_workContext.CurrentUser))
                return Challenge();

            var user = _workContext.CurrentUser;
            //find address (ensure that it belongs to the current user)
            var address = _userService.GetUserAddress(user.Id, addressId);
            if (address == null)
                //address is not found
                return RedirectToRoute("UserAddresses");

            //custom address attributes
            var customAttributes = _addressAttributeParser.ParseCustomAddressAttributes(form);
            var customAttributeWarnings = _addressAttributeParser.GetAttributeWarnings(customAttributes);
            foreach (var error in customAttributeWarnings)
            {
                ModelState.AddModelError("", error);
            }

            if (ModelState.IsValid)
            {
                address = model.Address.ToEntity(address);
                address.CustomAttributes = customAttributes;
                _addressService.UpdateAddress(address);

                return RedirectToRoute("UserAddresses");
            }

            //If we got this far, something failed, redisplay form
            _addressModelFactory.PrepareAddressModel(model.Address,
                address: address,
                excludeProperties: true,
                addressSettings: _addressSettings,
                loadCountries: () => _countryService.GetAllCountries(_workContext.WorkingLanguage.Id),
                overrideAttributesXml: customAttributes);
            return View(model);
        }

        #endregion

        #region My account / Downloadable products

        [HttpsRequirement(SslRequirement.Yes)]
        public virtual IActionResult DownloadableProducts()
        {
            if (!_userService.IsRegistered(_workContext.CurrentUser))
                return Challenge();

            if (_userSettings.HideDownloadableProductsTab)
                return RedirectToRoute("UserInfo");

            var model = _userModelFactory.PrepareUserDownloadableProductsModel();
            return View(model);
        }

        public virtual IActionResult UserAgreement(Guid orderItemId)
        {
            var orderItem = _orderService.GetOrderItemByGuid(orderItemId);
            if (orderItem == null)
                return RedirectToRoute("Homepage");

            var product = _productService.GetProductById(orderItem.ProductId);

            if (product == null || !product.HasUserAgreement)
                return RedirectToRoute("Homepage");

            var model = _userModelFactory.PrepareUserAgreementModel(orderItem, product);
            return View(model);
        }

        #endregion

        #region My account / Change password

        [HttpsRequirement(SslRequirement.Yes)]
        public virtual IActionResult ChangePassword()
        {
            if (!_userService.IsRegistered(_workContext.CurrentUser))
                return Challenge();

            var model = _userModelFactory.PrepareChangePasswordModel();

            //display the cause of the change password 
            if (_userService.PasswordIsExpired(_workContext.CurrentUser))
                ModelState.AddModelError(string.Empty, _localizationService.GetResource("Account.ChangePassword.PasswordIsExpired"));

            return View(model);
        }

        [HttpPost]
        public virtual IActionResult ChangePassword(ChangePasswordModel model)
        {
            if (!_userService.IsRegistered(_workContext.CurrentUser))
                return Challenge();

            var user = _workContext.CurrentUser;

            if (ModelState.IsValid)
            {
                var changePasswordRequest = new ChangePasswordRequest(user.Email,
                    true, _userSettings.DefaultPasswordFormat, model.NewPassword, model.OldPassword);
                var changePasswordResult = _userRegistrationService.ChangePassword(changePasswordRequest);
                if (changePasswordResult.Success)
                {
                    model.Result = _localizationService.GetResource("Account.ChangePassword.Success");
                    return View(model);
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

        [HttpsRequirement(SslRequirement.Yes)]
        public virtual IActionResult Avatar()
        {
            if (!_userService.IsRegistered(_workContext.CurrentUser))
                return Challenge();

            if (!_userSettings.AllowUsersToUploadAvatars)
                return RedirectToRoute("UserInfo");

            var model = new UserAvatarModel();
            model = _userModelFactory.PrepareUserAvatarModel(model);
            return View(model);
        }

        [HttpPost, ActionName("Avatar")]
        [FormValueRequired("upload-avatar")]
        public virtual IActionResult UploadAvatar(UserAvatarModel model, IFormFile uploadedFile)
        {
            if (!_userService.IsRegistered(_workContext.CurrentUser))
                return Challenge();

            if (!_userSettings.AllowUsersToUploadAvatars)
                return RedirectToRoute("UserInfo");

            var user = _workContext.CurrentUser;

            if (ModelState.IsValid)
            {
                try
                {
                    var userAvatar = _pictureService.GetPictureById(_genericAttributeService.GetAttribute<int>(user, TvProgUserDefaults.AvatarPictureIdAttribute));
                    if (uploadedFile != null && !string.IsNullOrEmpty(uploadedFile.FileName))
                    {
                        var avatarMaxSize = _userSettings.AvatarMaximumSizeBytes;
                        if (uploadedFile.Length > avatarMaxSize)
                            throw new TvProgException(string.Format(_localizationService.GetResource("Account.Avatar.MaximumUploadedFileSize"), avatarMaxSize));

                        var userPictureBinary = _downloadService.GetDownloadBits(uploadedFile);
                        if (userAvatar != null)
                            userAvatar = _pictureService.UpdatePicture(userAvatar.Id, userPictureBinary, uploadedFile.ContentType, null);
                        else
                            userAvatar = _pictureService.InsertPicture(userPictureBinary, uploadedFile.ContentType, null);
                    }

                    var userAvatarId = 0;
                    if (userAvatar != null)
                        userAvatarId = userAvatar.Id;

                    _genericAttributeService.SaveAttribute(user, TvProgUserDefaults.AvatarPictureIdAttribute, userAvatarId);

                    model.AvatarUrl = _pictureService.GetPictureUrl(
                        _genericAttributeService.GetAttribute<int>(user, TvProgUserDefaults.AvatarPictureIdAttribute),
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
            model = _userModelFactory.PrepareUserAvatarModel(model);
            return View(model);
        }

        [HttpPost, ActionName("Avatar")]
        [FormValueRequired("remove-avatar")]
        public virtual IActionResult RemoveAvatar(UserAvatarModel model)
        {
            if (!_userService.IsRegistered(_workContext.CurrentUser))
                return Challenge();

            if (!_userSettings.AllowUsersToUploadAvatars)
                return RedirectToRoute("UserInfo");

            var user = _workContext.CurrentUser;

            var userAvatar = _pictureService.GetPictureById(_genericAttributeService.GetAttribute<int>(user, TvProgUserDefaults.AvatarPictureIdAttribute));
            if (userAvatar != null)
                _pictureService.DeletePicture(userAvatar);
            _genericAttributeService.SaveAttribute(user, TvProgUserDefaults.AvatarPictureIdAttribute, 0);

            return RedirectToRoute("UserAvatar");
        }

        #endregion

        #region GDPR tools

        [HttpsRequirement(SslRequirement.Yes)]
        public virtual IActionResult GdprTools()
        {
            if (!_userService.IsRegistered(_workContext.CurrentUser))
                return Challenge();

            if (!_gdprSettings.GdprEnabled)
                return RedirectToRoute("UserInfo");

            var model = _userModelFactory.PrepareGdprToolsModel();
            return View(model);
        }

        [HttpPost, ActionName("GdprTools")]
        [FormValueRequired("export-data")]
        public virtual IActionResult GdprToolsExport()
        {
            if (!_userService.IsRegistered(_workContext.CurrentUser))
                return Challenge();

            if (!_gdprSettings.GdprEnabled)
                return RedirectToRoute("UserInfo");

            //log
            _gdprService.InsertLog(_workContext.CurrentUser, 0, GdprRequestType.ExportData, _localizationService.GetResource("Gdpr.Exported"));

            //export
            var bytes = _exportManager.ExportUserGdprInfoToXlsx(_workContext.CurrentUser, _storeContext.CurrentStore.Id);

            return File(bytes, MimeTypes.TextXlsx, "userdata.xlsx");
        }

        [HttpPost, ActionName("GdprTools")]
        [FormValueRequired("delete-account")]
        public virtual IActionResult GdprToolsDelete()
        {
            if (!_userService.IsRegistered(_workContext.CurrentUser))
                return Challenge();

            if (!_gdprSettings.GdprEnabled)
                return RedirectToRoute("UserInfo");

            //log
            _gdprService.InsertLog(_workContext.CurrentUser, 0, GdprRequestType.DeleteUser, _localizationService.GetResource("Gdpr.DeleteRequested"));

            var model = _userModelFactory.PrepareGdprToolsModel();
            model.Result = _localizationService.GetResource("Gdpr.DeleteRequested.Success");
            return View(model);
        }

        #endregion

        #region Check gift card balance

        //check gift card balance page
        [HttpsRequirement(SslRequirement.Yes)]
        //available even when a store is closed
        [CheckAccessClosedStore(true)]
        public virtual IActionResult CheckGiftCardBalance()
        {
            if (!(_captchaSettings.Enabled && _userSettings.AllowUsersToCheckGiftCardBalance))
            {
                return RedirectToRoute("UserInfo");
            }

            var model = _userModelFactory.PrepareCheckGiftCardBalanceModel();
            return View(model);
        }

        [HttpPost, ActionName("CheckGiftCardBalance")]
        [FormValueRequired("checkbalancegiftcard")]
        [ValidateCaptcha]
        public virtual IActionResult CheckBalance(CheckGiftCardBalanceModel model, bool captchaValid)
        {
            //validate CAPTCHA
            if (_captchaSettings.Enabled && !captchaValid)
            {
                ModelState.AddModelError("", _localizationService.GetResource("Common.WrongCaptchaMessage"));
            }

            if (ModelState.IsValid)
            {
                var giftCard = _giftCardService.GetAllGiftCards(giftCardCouponCode: model.GiftCardCode).FirstOrDefault();
                if (giftCard != null && _giftCardService.IsGiftCardValid(giftCard))
                {
                    var remainingAmount = _currencyService.ConvertFromPrimaryStoreCurrency(_giftCardService.GetGiftCardRemainingAmount(giftCard), _workContext.WorkingCurrency);
                    model.Result = _priceFormatter.FormatPrice(remainingAmount, true, false);
                }
                else
                {
                    model.Message = _localizationService.GetResource("CheckGiftCardBalance.GiftCardCouponCode.Invalid");
                }
            }

            return View(model);
        }

        #endregion

        #endregion
    }
}