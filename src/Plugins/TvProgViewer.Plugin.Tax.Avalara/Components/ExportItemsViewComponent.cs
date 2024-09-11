using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using TvProgViewer.Services.Security;
using TvProgViewer.Services.Tax;
using TvProgViewer.Web.Framework.Components;
using TvProgViewer.Web.Framework.Infrastructure;

namespace TvProgViewer.Plugin.Tax.Avalara.Components
{
    /// <summary>
    /// Represents a view component to render the button on a tvChannel list view
    /// </summary>
    public class ExportItemsViewComponent : TvProgViewComponent
    {
        #region Fields

        private readonly IPermissionService _permissionService;
        private readonly ITaxPluginManager _taxPluginManager;

        #endregion

        #region Ctor

        public ExportItemsViewComponent(IPermissionService permissionService,
            ITaxPluginManager taxPluginManager)
        {
            _permissionService = permissionService;
            _taxPluginManager = taxPluginManager;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Invoke the widget view component
        /// </summary>
        /// <param name="widgetZone">Widget zone</param>
        /// <param name="additionalData">Additional parameters</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the view component result
        /// </returns>
        public async Task<IViewComponentResult> InvokeAsync(string widgetZone, object additionalData)
        {
            //ensure that Avalara tax provider is active
            if (!await _taxPluginManager.IsPluginActiveAsync(AvalaraTaxDefaults.SystemName))
                return Content(string.Empty);

            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageTaxSettings))
                return Content(string.Empty);

            //ensure that it's a proper widget zone
            if (!widgetZone.Equals(AdminWidgetZones.TvChannelListButtons))
                return Content(string.Empty);

            return View("~/Plugins/Tax.Avalara/Views/TvChannel/ExportItems.cshtml");
        }

        #endregion
    }
}