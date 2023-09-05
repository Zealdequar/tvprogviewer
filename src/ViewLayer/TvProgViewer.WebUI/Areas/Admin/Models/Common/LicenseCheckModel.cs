using Newtonsoft.Json;
using TvProgViewer.Web.Framework.Models;

namespace TvProgViewer.WebUI.Areas.Admin.Models.Common
{
    /// <summary>
    /// Represents a license check model
    /// </summary>
    public partial record LicenseCheckModel : BaseTvProgModel
    {
        #region Properties

        [JsonProperty(PropertyName = "display_warning")]
        public bool? DisplayWarning { get; set; }

        [JsonProperty(PropertyName = "block_pages")]
        public bool? BlockPages { get; set; }

        [JsonProperty(PropertyName = "warning_text")]
        public string WarningText { get; set; }

        #endregion
    }
}