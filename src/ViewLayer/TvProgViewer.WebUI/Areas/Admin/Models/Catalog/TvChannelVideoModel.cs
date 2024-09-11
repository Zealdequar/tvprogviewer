using TvProgViewer.Web.Framework.Models;
using TvProgViewer.Web.Framework.Mvc.ModelBinding;

namespace TvProgViewer.WebUI.Areas.Admin.Models.Catalog
{
    /// <summary>
    /// Represents a tvChannel video model
    /// </summary>
    public partial record TvChannelVideoModel : BaseTvProgEntityModel
    {
        #region Properties

        public int TvChannelId { get; set; }

        [TvProgResourceDisplayName("Admin.Catalog.TvChannels.Multimedia.Videos.Fields.VideoUrl")]
        public string VideoUrl { get; set; }

        [TvProgResourceDisplayName("Admin.Catalog.TvChannels.Multimedia.Videos.Fields.DisplayOrder")]
        public int DisplayOrder { get; set; }

        #endregion
    }
}
