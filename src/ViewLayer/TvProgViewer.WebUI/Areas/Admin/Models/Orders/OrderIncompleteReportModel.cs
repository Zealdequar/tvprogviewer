using TvProgViewer.Web.Framework.Mvc.ModelBinding;
using TvProgViewer.Web.Framework.Models;

namespace TvProgViewer.WebUI.Areas.Admin.Models.Orders
{
    /// <summary>
    /// Represents an incomplete order report model
    /// </summary>
    public partial record OrderIncompleteReportModel : BaseTvProgModel
    {
        #region Properties

        [TvProgResourceDisplayName("Admin.SalesReport.Incomplete.Item")]
        public string Item { get; set; }

        [TvProgResourceDisplayName("Admin.SalesReport.Incomplete.Total")]
        public string Total { get; set; }

        [TvProgResourceDisplayName("Admin.SalesReport.Incomplete.Count")]
        public int Count { get; set; }

        [TvProgResourceDisplayName("Admin.SalesReport.Incomplete.View")]
        public string ViewLink { get; set; }

        #endregion
    }
}