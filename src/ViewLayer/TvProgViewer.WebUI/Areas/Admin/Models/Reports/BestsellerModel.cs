using TvProgViewer.Web.Framework.Mvc.ModelBinding;
using TvProgViewer.Web.Framework.Models;

namespace TvProgViewer.WebUI.Areas.Admin.Models.Reports
{
    /// <summary>
    /// Represents a bestseller model
    /// </summary>
    public partial record BestsellerModel : BaseTvProgModel
    {
        #region Properties

        public int TvChannelId { get; set; }

        [TvProgResourceDisplayName("Admin.Reports.Sales.Bestsellers.Fields.Name")]
        public string TvChannelName { get; set; }

        [TvProgResourceDisplayName("Admin.Reports.Sales.Bestsellers.Fields.TotalAmount")]
        public string TotalAmount { get; set; }

        [TvProgResourceDisplayName("Admin.Reports.Sales.Bestsellers.Fields.TotalQuantity")]
        public decimal TotalQuantity { get; set; }

        #endregion
    }
}