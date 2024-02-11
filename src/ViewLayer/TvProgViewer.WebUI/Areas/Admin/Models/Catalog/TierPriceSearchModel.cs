using TvProgViewer.Web.Framework.Models;

namespace TvProgViewer.WebUI.Areas.Admin.Models.Catalog
{
    /// <summary>
    /// Represents a tier price search model
    /// </summary>
    public partial record TierPriceSearchModel : BaseSearchModel
    {
        #region Properties

        public int TvChannelId { get; set; }

        #endregion
    }
}