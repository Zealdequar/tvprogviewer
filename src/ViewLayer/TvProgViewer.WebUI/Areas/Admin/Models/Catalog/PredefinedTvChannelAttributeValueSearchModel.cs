using TvProgViewer.Web.Framework.Models;

namespace TvProgViewer.WebUI.Areas.Admin.Models.Catalog
{
    /// <summary>
    /// Represents a predefined tvchannel attribute value search model
    /// </summary>
    public partial record PredefinedTvChannelAttributeValueSearchModel : BaseSearchModel
    {
        #region Properties

        public int TvChannelAttributeId { get; set; }

        #endregion
    }
}