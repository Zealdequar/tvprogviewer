using TvProgViewer.Web.Framework.Mvc.ModelBinding;
using TvProgViewer.Web.Framework.Models;

namespace TvProgViewer.WebUI.Areas.Admin.Models.Reports
{
    /// <summary>
    /// Represents a low stock tvchannel model
    /// </summary>
    public partial record LowStockTvChannelModel : BaseTvProgEntityModel
    {
        #region Properties

        [TvProgResourceDisplayName("Admin.Catalog.TvChannels.Fields.Name")]
        public string Name { get; set; }

        public string Attributes { get; set; }

        [TvProgResourceDisplayName("Admin.Catalog.TvChannels.Fields.ManageInventoryMethod")]
        public string ManageInventoryMethod { get; set; }

        [TvProgResourceDisplayName("Admin.Catalog.TvChannels.Fields.StockQuantity")]
        public int StockQuantity { get; set; }

        [TvProgResourceDisplayName("Admin.Catalog.TvChannels.Fields.Published")]
        public bool Published { get; set; }

        #endregion
    }
}