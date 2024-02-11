using TvProgViewer.Web.Framework.Mvc.ModelBinding;
using TvProgViewer.Web.Framework.Models;

namespace TvProgViewer.WebUI.Areas.Admin.Models.Catalog
{
    /// <summary>
    /// Represents a copy tvchannel model
    /// </summary>
    public partial record CopyTvChannelModel : BaseTvProgEntityModel
    {
        #region Properties

        [TvProgResourceDisplayName("Admin.Catalog.TvChannels.Copy.Name")]
        public string Name { get; set; }

        [TvProgResourceDisplayName("Admin.Catalog.TvChannels.Copy.CopyMultimedia")]
        public bool CopyMultimedia { get; set; }

        [TvProgResourceDisplayName("Admin.Catalog.TvChannels.Copy.Published")]
        public bool Published { get; set; }

        #endregion
    }
}