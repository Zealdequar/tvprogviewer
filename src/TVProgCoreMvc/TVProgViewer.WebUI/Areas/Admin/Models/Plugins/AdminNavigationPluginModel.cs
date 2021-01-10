using TVProgViewer.Web.Framework.Models;

namespace TVProgViewer.WebUI.Areas.Admin.Models.Plugins
{
    /// <summary>
    /// Represents a plugin model that is used for admin navigation
    /// </summary>
    public partial record AdminNavigationPluginModel : BaseTvProgModel
    {
        #region Properties

        public string FriendlyName { get; set; }

        public string ConfigurationUrl { get; set; }

        #endregion
    }
}