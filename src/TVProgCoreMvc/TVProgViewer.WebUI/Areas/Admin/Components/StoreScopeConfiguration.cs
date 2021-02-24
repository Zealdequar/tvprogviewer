using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using TVProgViewer.WebUI.Areas.Admin.Factories;
using TVProgViewer.Web.Framework.Components;

namespace TVProgViewer.WebUI.Areas.Admin.Components
{
    /// <summary>
    /// Represents a view component that displays the store scope configuration
    /// </summary>
    public class StoreScopeConfigurationViewComponent : TvProgViewComponent
    {
        #region Fields

        private readonly ISettingModelFactory _settingModelFactory;

        #endregion

        #region Ctor

        public StoreScopeConfigurationViewComponent(ISettingModelFactory settingModelFactory)
        {
            _settingModelFactory = settingModelFactory;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Invoke view component
        /// </summary>
        /// <returns>View component result</returns>
        public async Task<IViewComponentResult> InvokeAsync()
        {
            //prepare model
            var model = await _settingModelFactory.PrepareStoreScopeConfigurationModelAsync();

            if (model.Stores.Count < 2)
                return Content(string.Empty);
            
            return View(model);
        }

        #endregion
    }
}