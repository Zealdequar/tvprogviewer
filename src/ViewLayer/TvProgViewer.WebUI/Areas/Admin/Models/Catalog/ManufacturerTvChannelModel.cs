using TvProgViewer.Web.Framework.Models;
using TvProgViewer.Web.Framework.Mvc.ModelBinding;

namespace TvProgViewer.WebUI.Areas.Admin.Models.Catalog
{
    /// <summary>
    /// Represents a manufacturer tvchannel model
    /// </summary>
    public partial record ManufacturerTvChannelModel : BaseTvProgEntityModel
    {
        #region Properties

        public int ManufacturerId { get; set; }

        public int TvChannelId { get; set; }

        [TvProgResourceDisplayName("Admin.Catalog.Manufacturers.TvChannels.Fields.TvChannel")]
        public string TvChannelName { get; set; }

        [TvProgResourceDisplayName("Admin.Catalog.Manufacturers.TvChannels.Fields.IsFeaturedTvChannel")]
        public bool IsFeaturedTvChannel { get; set; }

        [TvProgResourceDisplayName("Admin.Catalog.Manufacturers.TvChannels.Fields.DisplayOrder")]
        public int DisplayOrder { get; set; }

        #endregion
    }
}