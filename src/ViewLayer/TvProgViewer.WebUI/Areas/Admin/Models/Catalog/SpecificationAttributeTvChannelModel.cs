using TvProgViewer.Web.Framework.Models;
using TvProgViewer.Web.Framework.Mvc.ModelBinding;

namespace TvProgViewer.WebUI.Areas.Admin.Models.Catalog
{
    /// <summary>
    /// Represents a model of tvchannels that use the specification attribute
    /// </summary>
    public partial record SpecificationAttributeTvChannelModel : BaseTvProgEntityModel
    {
        #region Properties

        public int SpecificationAttributeId { get; set; }

        public int TvChannelId { get; set; }

        [TvProgResourceDisplayName("Admin.Catalog.Attributes.SpecificationAttributes.SpecificationAttribute.UsedByTvChannels.TvChannel")]
        public string TvChannelName { get; set; }

        [TvProgResourceDisplayName("Admin.Catalog.Attributes.SpecificationAttributes.SpecificationAttribute.UsedByTvChannels.Published")]
        public bool Published { get; set; }

        #endregion
    }
}