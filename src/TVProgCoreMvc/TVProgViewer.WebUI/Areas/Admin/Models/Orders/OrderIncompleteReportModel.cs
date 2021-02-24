using TVProgViewer.Web.Framework.Mvc.ModelBinding;
using TVProgViewer.Web.Framework.Models;

namespace TVProgViewer.WebUI.Areas.Admin.Models.Orders
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