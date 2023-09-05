using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using TvProgViewer.Core;
using TvProgViewer.Core.Domain.Users;
using TvProgViewer.Core.Http.Extensions;
using TvProgViewer.Plugin.MultiFactorAuth.GoogleAuthenticator.Models;
using TvProgViewer.Plugin.MultiFactorAuth.GoogleAuthenticator.Services;
using TvProgViewer.Services.Users;
using TvProgViewer.Services.Localization;
using TvProgViewer.Services.Messages;
using TvProgViewer.Web.Framework.Controllers;

namespace TvProgViewer.Plugin.MultiFactorAuth.GoogleAuthenticator.Controllers
{
    [AutoValidateAntiforgeryToken]
    public class AuthenticationController : BasePluginController
    {
        #region Fields

        private readonly UserSettings _userSettings;
        private readonly GoogleAuthenticatorService _googleAuthenticatorService;
        private readonly IUserRegistrationService _userRegistrationService;
        private readonly IUserService _userService;
        private readonly ILocalizationService _localizationService;
        private readonly INotificationService _notificationService;
        private readonly IWorkContext _workContext;

        #endregion

        #region Ctor
        public AuthenticationController(
            UserSettings userSettings,
            GoogleAuthenticatorService googleAuthenticatorService,
            IUserRegistrationService userRegistrationService,
            IUserService userService,
            ILocalizationService localizationService,
            INotificationService notificationService,
            IWorkContext workContext)
        {
            _userSettings = userSettings;
            _googleAuthenticatorService = googleAuthenticatorService;
            _userRegistrationService = userRegistrationService;
            _userService = userService;
            _localizationService = localizationService;
            _notificationService = notificationService;
            _workContext = workContext;
        }

        #endregion

        #region Methods

        [HttpPost]
        public async Task<IActionResult> RegisterGoogleAuthenticator(AuthModel model)
        {
            var currentUser = await _workContext.GetCurrentUserAsync();

            var isValidToken = _googleAuthenticatorService.ValidateTwoFactorToken(model.SecretKey, model.Code);
            if (isValidToken)
            {
                //try to find config with current user and update
                if (_googleAuthenticatorService.IsRegisteredUser(currentUser.Email))
                {
                    await _googleAuthenticatorService.UpdateGoogleAuthenticatorAccountAsync(currentUser.Email, model.SecretKey);
                }
                else
                {
                    await _googleAuthenticatorService.AddGoogleAuthenticatorAccountAsync(currentUser.Email, model.SecretKey);
                }
                _notificationService.SuccessNotification(await _localizationService.GetResourceAsync("Plugins.MultiFactorAuth.GoogleAuthenticator.Token.Successful"));
            }
            else
            {
                _notificationService.ErrorNotification(await _localizationService.GetResourceAsync("Plugins.MultiFactorAuth.GoogleAuthenticator.Token.Unsuccessful"));
                return RedirectToRoute("UserMultiFactorAuthenticationProviderConfig", new { providerSysName = GoogleAuthenticatorDefaults.SystemName });
            }
            
            return RedirectToRoute("MultiFactorAuthenticationSettings");
        }

        [HttpPost]
        public async Task<IActionResult> VerifyGoogleAuthenticator(TokenModel model)
        {
            var userMultiFactorAuthenticationInfo = HttpContext.Session.Get<UserMultiFactorAuthenticationInfo>(TvProgUserDefaults.UserMultiFactorAuthenticationInfo);
            var username = userMultiFactorAuthenticationInfo.UserName;
            var returnUrl = userMultiFactorAuthenticationInfo.ReturnUrl;
            var isPersist = userMultiFactorAuthenticationInfo.RememberMe;

            var user = _userSettings.UsernamesEnabled ? await _userService.GetUserByUsernameAsync(username) : await _userService.GetUserByEmailAsync(username);
            if (user == null)
                return RedirectToRoute("Login");

            var record = _googleAuthenticatorService.GetConfigurationByUserEmail(user.Email);
            if (record != null)
            {
                var isValidToken = _googleAuthenticatorService.ValidateTwoFactorToken(record.SecretKey, model.Token);
                if (isValidToken)
                {
                    HttpContext.Session.Set<UserMultiFactorAuthenticationInfo>(TvProgUserDefaults.UserMultiFactorAuthenticationInfo, null);

                    return await _userRegistrationService.SignInUserAsync(user, returnUrl, isPersist);
                }
                else
                {
                    _notificationService.ErrorNotification(await _localizationService.GetResourceAsync("Plugins.MultiFactorAuth.GoogleAuthenticator.Token.Unsuccessful"));
                }
            }
            else
            {
                _notificationService.ErrorNotification(await _localizationService.GetResourceAsync("Plugins.MultiFactorAuth.GoogleAuthenticator.Record.Notfound"));
            }

            return RedirectToRoute("MultiFactorVerification");
        }

        #endregion

    }
}
