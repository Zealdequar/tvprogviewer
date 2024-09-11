using TvProgViewer.Web.Framework.Models;
using TvProgViewer.Web.Framework.Mvc.ModelBinding;

namespace TvProgViewer.WebUI.Areas.Admin.Models.Catalog
{
    /// <summary>
    /// Represents a model of tvChannels that use the tvChannel attribute
    /// </summary>
    public partial record TvChannelAttributeTvChannelModel : BaseTvProgEntityModel
    {
        #region Properties

        [TvProgResourceDisplayName("Admin.Catalog.Attributes.TvChannelAttributes.UsedByTvChannels.TvChannel")]
        public string TvChannelName { get; set; }

        [TvProgResourceDisplayName("Admin.Catalog.Attributes.TvChannelAttributes.UsedByTvChannels.Published")]
        public bool Published { get; set; }

        #endregion
    }
}