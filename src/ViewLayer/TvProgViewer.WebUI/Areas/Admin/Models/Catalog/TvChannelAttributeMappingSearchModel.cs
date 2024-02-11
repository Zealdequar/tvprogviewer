using TvProgViewer.Web.Framework.Models;

namespace TvProgViewer.WebUI.Areas.Admin.Models.Catalog
{
    /// <summary>
    /// Represents a tvchannel attribute mapping search model
    /// </summary>
    public partial record TvChannelAttributeMappingSearchModel : BaseSearchModel
    {
        #region Properties

        public int TvChannelId { get; set; }

        #endregion
    }
}