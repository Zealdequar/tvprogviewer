using TvProgViewer.Web.Framework.Models;
using TvProgViewer.Web.Framework.Mvc.ModelBinding;

namespace TvProgViewer.WebUI.Areas.Admin.Models.Reports
{
    /// <summary>
    /// Represents a sales summary model
    /// </summary>
    public partial record SalesSummaryModel : BaseTvProgModel
    {
        #region Properties

        [TvProgResourceDisplayName("Admin.Reports.SalesSummary.Fields.Summary")]
        public string Summary { get; set; }

        [TvProgResourceDisplayName("Admin.Reports.SalesSummary.Fields.NumberOfOrders")]
        public int NumberOfOrders { get; set; }

        [TvProgResourceDisplayName("Admin.Reports.SalesSummary.Fields.Profit")]
        public string ProfitStr { get; set; }

        [TvProgResourceDisplayName("Admin.Reports.SalesSummary.Fields.Shipping")]
        public string Shipping { get; set; }

        [TvProgResourceDisplayName("Admin.Reports.SalesSummary.Fields.Tax")]
        public string Tax { get; set; }

        [TvProgResourceDisplayName("Admin.Reports.SalesSummary.Fields.OrderTotal")]
        public string OrderTotal { get; set; }

        #endregion
    }
}
