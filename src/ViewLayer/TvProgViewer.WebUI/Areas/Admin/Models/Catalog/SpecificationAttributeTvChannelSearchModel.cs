using TvProgViewer.Web.Framework.Models;

namespace TvProgViewer.WebUI.Areas.Admin.Models.Catalog
{
    /// <summary>
    /// Represents a search model of tvchannels that use the specification attribute
    /// </summary>
    public partial record SpecificationAttributeTvChannelSearchModel : BaseSearchModel
    {
        #region Properties

        public int SpecificationAttributeId { get; set; }

        #endregion
    }
}