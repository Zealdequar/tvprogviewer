using TvProgViewer.Web.Framework.Models;
using TvProgViewer.Web.Framework.Mvc.ModelBinding;

namespace TvProgViewer.WebUI.Areas.Admin.Models.Catalog
{
    /// <summary>
    /// Represents a related tvChannel model
    /// </summary>
    public partial record RelatedTvChannelModel : BaseTvProgEntityModel
    {
        #region Properties

        public int TvChannelId2 { get; set; }

        [TvProgResourceDisplayName("Admin.Catalog.TvChannels.RelatedTvChannels.Fields.TvChannel")]
        public string TvChannel2Name { get; set; }

        [TvProgResourceDisplayName("Admin.Catalog.TvChannels.RelatedTvChannels.Fields.DisplayOrder")]
        public int DisplayOrder { get; set; }

        #endregion
    }
}