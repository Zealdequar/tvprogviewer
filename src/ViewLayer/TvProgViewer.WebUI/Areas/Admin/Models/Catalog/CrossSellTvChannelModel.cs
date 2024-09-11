using TvProgViewer.Web.Framework.Models;
using TvProgViewer.Web.Framework.Mvc.ModelBinding;

namespace TvProgViewer.WebUI.Areas.Admin.Models.Catalog
{
    /// <summary>
    /// Represents a cross-sell tvChannel model
    /// </summary>
    public partial record CrossSellTvChannelModel : BaseTvProgEntityModel
    {
        #region Properties

        public int TvChannelId2 { get; set; }

        [TvProgResourceDisplayName("Admin.Catalog.TvChannels.CrossSells.Fields.TvChannel")]
        public string TvChannel2Name { get; set; }

        #endregion
    }
}