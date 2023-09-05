using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using TvProgViewer.Core;
using TvProgViewer.Core.Domain.Users;
using TvProgViewer.Plugin.MultiFactorAuth.GoogleAuthenticator.Models;
using TvProgViewer.Plugin.MultiFactorAuth.GoogleAuthenticator.Services;
using TvProgViewer.Services.Common;
using TvProgViewer.Services.Configuration;
using TvProgViewer.Services.Users;
using TvProgViewer.Services.Localization;
using TvProgViewer.Services.Messages;
using TvProgViewer.Services.Security;
using TvProgViewer.Web.Framework;
using TvProgViewer.Web.Framework.Controllers;
using TvProgViewer.Web.Framework.Models.Extensions;
using TvProgViewer.Web.Framework.Mvc;
using TvProgViewer.Web.Framework.Mvc.Filters;
using TvProgViewer.Web.Framework.Mvc.ModelBinding;

namespace TvProgViewer.Plugin.MultiFactorAuth.GoogleAuthenticator.Controllers
{
    [AutoValidateAntiforgeryToken]
    [AuthorizeAdmin]
    [Area(AreaNames.Admin)]
    public class GoogleAuthenticatorController : BasePluginController
    {
        #region Fields

        private readonly GoogleAuthenticatorService _googleAuthenticatorService;
        private readonly GoogleAuthenticatorSettings _googleAuthenticatorSettings;
        private readonly IUserService _userService;
        private readonly IGenericAttributeService _genericAttributeService;        
        private readonly ILocalizationService _localizationService;
        private readonly INotificationService _notificationService;
        private readonly IPermissionService _permissionService;
        private readonly ISettingService _settingService;
        private readonly IWorkContext _workContext;


        #endregion

        #region Ctor 

        public GoogleAuthenticatorController(GoogleAuthenticatorService googleAuthenticatorService,
            GoogleAuthenticatorSettings googleAuthenticatorSettings,
            IUserService userService,
            IGenericAttributeService genericAttributeService,
            ILocalizationService localizationService,
            INotificationService notificationService,
            IPermissionService permissionService,
            ISettingService settingService,
            IWorkContext workContext
            )
        {
            _googleAuthenticatorService = googleAuthenticatorService;
            _googleAuthenticatorSettings = googleAuthenticatorSettings;
            _userService = userService;
            _genericAttributeService = genericAttributeService;
            _localizationService = localizationService;
            _notificationService = notificationService;
            _permissionService = permissionService;
            _settingService = settingService;
            _workContext = workContext;
        }

        #endregion

        #region Methods

        public async Task<IActionResult> Configure()
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageMultifactorAuthenticationMethods))
                return AccessDeniedView();

            //prepare model
            var model = new ConfigurationModel
            {
                QRPixelsPerModule = _googleAuthenticatorSettings.QRPixelsPerModule,
                BusinessPrefix = _googleAuthenticatorSettings.BusinessPrefix                
            };
            model.GoogleAuthenticatorSearchModel.HideSearchBlock = await _genericAttributeService
                .GetAttributeAsync<bool>(await _workContext.GetCurrentUserAsync(), GoogleAuthenticatorDefaults.HideSearchBlockAttribute);

            return View("~/Plugins/MultiFactorAuth.GoogleAuthenticator/Views/Configure.cshtml", model);
        }

        [HttpPost]        
        public async Task<IActionResult> Configure(ConfigurationModel model)
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageMultifactorAuthenticationMethods))
                return AccessDeniedView();

            if (!ModelState.IsValid)
                return await Configure();

            //set new settings values
            _googleAuthenticatorSettings.QRPixelsPerModule = model.QRPixelsPerModule;
            _googleAuthenticatorSettings.BusinessPrefix = model.BusinessPrefix;
            await _settingService.SaveSettingAsync(_googleAuthenticatorSettings);

            _notificationService.SuccessNotification(await _localizationService.GetResourceAsync("Admin.Plugins.Saved"));

            return await Configure();
        }

        [HttpPost]
        public async Task<IActionResult> GoogleAuthenticatorList(GoogleAuthenticatorSearchModel searchModel)
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageMultifactorAuthenticationMethods))
                return AccessDeniedView();

            //get GoogleAuthenticator configuration records
            var configurations = await _googleAuthenticatorService.GetPagedConfigurationsAsync(searchModel.SearchEmail,
                searchModel.Page - 1, searchModel.PageSize);
            var model = new GoogleAuthenticatorListModel().PrepareToGrid(searchModel, configurations, () =>
            {
                //fill in model values from the configuration
                return configurations.Select(configuration => new GoogleAuthenticatorModel
                {
                    Id = configuration.Id,
                    User = configuration.User,
                    SecretKey = configuration.SecretKey
                });
            });

            return Json(model);
        }

        [HttpPost]
        public async Task<IActionResult> GoogleAuthenticatorDelete (GoogleAuthenticatorModel model)
        {
            if (!ModelState.IsValid)
                return ErrorJson(ModelState.SerializeErrors());

            //delete configuration
            var configuration = await _googleAuthenticatorService.GetConfigurationByIdAsync(model.Id);
            if (configuration != null)
            {
                await _googleAuthenticatorService.DeleteConfigurationAsync(configuration);
            }
            var user = await _userService.GetUserByEmailAsync(configuration.User) ??
                await _userService.GetUserByUsernameAsync(configuration.User);

            if (user != null)
                await _genericAttributeService.SaveAttributeAsync(user, TvProgUserDefaults.SelectedMultiFactorAuthenticationProviderAttribute, string.Empty);

            return new NullJsonResult();
        }

        #endregion
    }
}
