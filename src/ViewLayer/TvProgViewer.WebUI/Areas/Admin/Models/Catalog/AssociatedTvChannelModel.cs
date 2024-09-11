using TvProgViewer.Web.Framework.Models;
using TvProgViewer.Web.Framework.Mvc.ModelBinding;

namespace TvProgViewer.WebUI.Areas.Admin.Models.Catalog
{
    /// <summary>
    /// Represents an associated tvChannel model
    /// </summary>
    public partial record AssociatedTvChannelModel : BaseTvProgEntityModel
    {
        #region Properties

        [TvProgResourceDisplayName("Admin.Catalog.TvChannels.AssociatedTvChannels.Fields.TvChannel")]
        public string TvChannelName { get; set; }

        [TvProgResourceDisplayName("Admin.Catalog.TvChannels.AssociatedTvChannels.Fields.DisplayOrder")]
        public int DisplayOrder { get; set; }

        #endregion
    }
}