using TVProgViewer.Web.Framework.Models;

namespace TVProgViewer.WebUI.Areas.Admin.Models.Plugins.Marketplace
{
    /// <summary>
    /// Represents an official feed plugin model
    /// </summary>
    public partial record OfficialFeedPluginModel : BaseTvProgModel
    {
        #region Properties

        public string Url { get; set; }

        public string Name { get; set; }

        public string CategoryName { get; set; }

        public string SupportedVersions { get; set; }

        public string PictureUrl { get; set; }

        public string Price { get; set; }

        #endregion
    }
}