using TvProgViewer.Web.Framework.Mvc.ModelBinding;
using TvProgViewer.Web.Framework.Models;

namespace TvProgViewer.WebUI.Areas.Admin.Models.Reports
{
    /// <summary>
    /// Represents a never sold tvChannels report model
    /// </summary>
    public partial record NeverSoldReportModel : BaseTvProgModel
    {
        #region Properties

        public int TvChannelId { get; set; }

        [TvProgResourceDisplayName("Admin.Reports.Sales.NeverSold.Fields.Name")]
        public string TvChannelName { get; set; }

        #endregion
    }
}