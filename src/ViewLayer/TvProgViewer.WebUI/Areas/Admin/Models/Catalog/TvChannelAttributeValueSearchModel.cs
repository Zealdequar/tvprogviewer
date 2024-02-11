using TvProgViewer.Web.Framework.Models;

namespace TvProgViewer.WebUI.Areas.Admin.Models.Catalog
{
    /// <summary>
    /// Represents a tvchannel attribute value search model
    /// </summary>
    public partial record TvChannelAttributeValueSearchModel : BaseSearchModel
    {
        #region Properties

        public int TvChannelAttributeMappingId { get; set; }

        #endregion
    }
}