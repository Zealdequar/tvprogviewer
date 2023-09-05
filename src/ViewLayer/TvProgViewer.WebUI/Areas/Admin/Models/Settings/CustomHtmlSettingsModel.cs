using TvProgViewer.Web.Framework.Models;
using TvProgViewer.Web.Framework.Mvc.ModelBinding;

namespace TvProgViewer.WebUI.Areas.Admin.Models.Settings
{
    /// <summary>
    /// Represents an custom html settings model
    /// </summary>
    public partial record CustomHtmlSettingsModel : BaseTvProgModel, ISettingsModel
    {
        #region Properties

        public int ActiveStoreScopeConfiguration { get; set; }

        [TvProgResourceDisplayName("Admin.Configuration.Settings.GeneralCommon.HeaderCustomHtml")]
        public string HeaderCustomHtml { get; set; }
        public bool HeaderCustomHtml_OverrideForStore { get; set; }

        [TvProgResourceDisplayName("Admin.Configuration.Settings.GeneralCommon.FooterCustomHtml")]
        public string FooterCustomHtml { get; set; }
        public bool FooterCustomHtml_OverrideForStore { get; set; }

        #endregion
    }
}
