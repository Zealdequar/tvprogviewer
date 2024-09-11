using TvProgViewer.Web.Framework.Models;

namespace TvProgViewer.WebUI.Areas.Admin.Models.Catalog
{
    /// <summary>
    /// Represents a search model of tvChannels that use the tvChannel attribute
    /// </summary>
    public partial record TvChannelAttributeTvChannelSearchModel : BaseSearchModel
    {
        #region Properties

        public int TvChannelAttributeId { get; set; }

        #endregion
    }
}