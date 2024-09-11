using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using TvProgViewer.Plugin.Tax.Avalara.Services;
using TvProgViewer.Services.Localization;
using TvProgViewer.Services.Messages;
using TvProgViewer.Services.Security;
using TvProgViewer.Services.Tax;
using TvProgViewer.WebUI.Areas.Admin.Controllers;

namespace TvProgViewer.Plugin.Tax.Avalara.Controllers
{
    public class AvalaraTvChannelController : BaseAdminController
    {
        #region Fields

        private readonly AvalaraTaxManager _avalaraTaxManager;
        private readonly ILocalizationService _localizationService;
        private readonly INotificationService _notificationService;
        private readonly IPermissionService _permissionService;
        private readonly ITaxPluginManager _taxPluginManager;

        #endregion

        #region Ctor

        public AvalaraTvChannelController(AvalaraTaxManager avalaraTaxManager,
            ILocalizationService localizationService,
            INotificationService notificationService,
            IPermissionService permissionService,
            ITaxPluginManager taxPluginManager)
        {
            _avalaraTaxManager = avalaraTaxManager;
            _localizationService = localizationService;
            _notificationService = notificationService;
            _permissionService = permissionService;
            _taxPluginManager = taxPluginManager;
        }

        #endregion

        #region Methods

        [HttpPost]
        public async Task<IActionResult> ExportTvChannels(string selectedIds)
        {
            //ensure that Avalara tax provider is active
            if (!await _taxPluginManager.IsPluginActiveAsync(AvalaraTaxDefaults.SystemName))
                return RedirectToAction("List", "TvChannel");

            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageTaxSettings))
                return AccessDeniedView();

            //export items
            var exportedItems = await _avalaraTaxManager.ExportTvChannelsAsync(selectedIds);
            if (exportedItems.HasValue)
            {
                if (exportedItems > 0)
                    _notificationService.SuccessNotification(string.Format(await _localizationService.GetResourceAsync("Plugins.Tax.Avalara.Items.Export.Success"), exportedItems));
                else
                    _notificationService.SuccessNotification(await _localizationService.GetResourceAsync("Plugins.Tax.Avalara.Items.Export.AlreadyExported"));
            }
            else
                _notificationService.ErrorNotification(await _localizationService.GetResourceAsync("Plugins.Tax.Avalara.Items.Export.Error"));

            return RedirectToAction("List", "TvChannel");
        }

        #endregion
    }
}