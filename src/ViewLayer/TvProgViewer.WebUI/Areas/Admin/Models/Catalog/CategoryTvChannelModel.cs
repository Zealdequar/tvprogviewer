using TvProgViewer.Web.Framework.Models;
using TvProgViewer.Web.Framework.Mvc.ModelBinding;

namespace TvProgViewer.WebUI.Areas.Admin.Models.Catalog
{
    /// <summary>
    /// Represents a category tvchannel model
    /// </summary>
    public partial record CategoryTvChannelModel : BaseTvProgEntityModel
    {
        #region Properties

        public int CategoryId { get; set; }

        public int TvChannelId { get; set; }

        [TvProgResourceDisplayName("Admin.Catalog.Categories.TvChannels.Fields.TvChannel")]
        public string TvChannelName { get; set; }

        [TvProgResourceDisplayName("Admin.Catalog.Categories.TvChannels.Fields.IsFeaturedTvChannel")]
        public bool IsFeaturedTvChannel { get; set; }

        [TvProgResourceDisplayName("Admin.Catalog.Categories.TvChannels.Fields.DisplayOrder")]
        public int DisplayOrder { get; set; }

        #endregion
    }
}