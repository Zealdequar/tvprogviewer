using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using TvProgViewer.Core;
using TvProgViewer.Core.Domain.Common;
using TvProgViewer.Core.Domain.Users;
using TvProgViewer.Services.Common;
using TvProgViewer.Services.Configuration;
using TvProgViewer.Services.Localization;
using TvProgViewer.Services.Messages;
using TvProgViewer.Services.Security;
using TvProgViewer.WebUI.Areas.Admin.Factories;
using TvProgViewer.WebUI.Areas.Admin.Models.Common;
using TvProgViewer.WebUI.Areas.Admin.Models.Home;

namespace TvProgViewer.WebUI.Areas.Admin.Controllers
{
    public partial class HomeController : BaseAdminController
    {
        #region Fields

        private readonly AdminAreaSettings _adminAreaSettings;
        private readonly ICommonModelFactory _commonModelFactory;
        private readonly IHomeModelFactory _homeModelFactory;
        private readonly ILocalizationService _localizationService;
        private readonly INotificationService _notificationService;
        private readonly IPermissionService _permissionService;
        private readonly ISettingService _settingService;
        private readonly IGenericAttributeService _genericAttributeService;
        private readonly IWorkContext _workContext;

        #endregion

        #region Ctor

        public HomeController(AdminAreaSettings adminAreaSettings,
            ICommonModelFactory commonModelFactory,
            IHomeModelFactory homeModelFactory,
            ILocalizationService localizationService,
            INotificationService notificationService,
            IPermissionService permissionService,
            ISettingService settingService,
            IGenericAttributeService genericAttributeService,
            IWorkContext workContext)
        {
            _adminAreaSettings = adminAreaSettings;
            _commonModelFactory = commonModelFactory;
            _homeModelFactory = homeModelFactory;
            _localizationService = localizationService;
            _notificationService = notificationService;
            _permissionService = permissionService;
            _settingService = settingService;
            _workContext = workContext;
            _genericAttributeService = genericAttributeService;
        }

        #endregion

        #region Methods

        public virtual async Task<IActionResult> Index()
        {
            //display a warning to a store owner if there are some error
            var user = await _workContext.GetCurrentUserAsync();
            var hideCard = await _genericAttributeService.GetAttributeAsync<bool>(user, TvProgUserDefaults.HideConfigurationStepsAttribute);
            var closeCard = await _genericAttributeService.GetAttributeAsync<bool>(user, TvProgUserDefaults.CloseConfigurationStepsAttribute);

            if ((hideCard || closeCard) && await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageMaintenance))
            {
                var warnings = await _commonModelFactory.PrepareSystemWarningModelsAsync();
                if (warnings.Any(warning => warning.Level == SystemWarningLevel.Fail || warning.Level == SystemWarningLevel.Warning))
                {
                    var locale = await _localizationService.GetResourceAsync("Admin.System.Warnings.Errors");
                    _notificationService.WarningNotification(string.Format(locale, Url.Action("Warnings", "Common")), false); //do not encode URLs
                }
            }

            //progress of localization 
            var currentLanguage = await _workContext.GetWorkingLanguageAsync();
            var progress = await _genericAttributeService.GetAttributeAsync<string>(currentLanguage, TvProgCommonDefaults.LanguagePackProgressAttribute);
            if (!string.IsNullOrEmpty(progress))
            {
                var locale = await _localizationService.GetResourceAsync("Admin.Configuration.LanguagePackProgressMessage");
                _notificationService.SuccessNotification(string.Format(locale, progress, TvProgLinksDefaults.OfficialSite.Translations), false);
                await _genericAttributeService.SaveAttributeAsync(currentLanguage, TvProgCommonDefaults.LanguagePackProgressAttribute, string.Empty);
            }

            //prepare model
            var model = await _homeModelFactory.PrepareDashboardModelAsync(new DashboardModel());

            return View(model);
        }

        [HttpPost]
        public virtual async Task<IActionResult> TvProgViewerNewsHideAdv()
        {
            _adminAreaSettings.HideAdvertisementsOnAdminArea = !_adminAreaSettings.HideAdvertisementsOnAdminArea;
            await _settingService.SaveSettingAsync(_adminAreaSettings);

            return Content("Setting changed");
        }

        #endregion
    }
}