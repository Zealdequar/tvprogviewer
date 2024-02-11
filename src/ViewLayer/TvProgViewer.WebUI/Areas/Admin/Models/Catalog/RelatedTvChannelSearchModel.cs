using TvProgViewer.Web.Framework.Models;

namespace TvProgViewer.WebUI.Areas.Admin.Models.Catalog
{
    /// <summary>
    /// Represents a related tvchannel search model
    /// </summary>
    public partial record RelatedTvChannelSearchModel : BaseSearchModel
    {
        #region Properties

        public int TvChannelId { get; set; }

        #endregion
    }
}