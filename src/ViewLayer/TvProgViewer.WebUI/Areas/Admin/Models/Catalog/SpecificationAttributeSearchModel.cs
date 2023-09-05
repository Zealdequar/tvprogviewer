using TvProgViewer.Web.Framework.Models;

namespace TvProgViewer.WebUI.Areas.Admin.Models.Catalog
{
    /// <summary>
    /// Represents a specification attribute search model
    /// </summary>
    public partial record SpecificationAttributeSearchModel : BaseSearchModel
    {
        #region Properties

        public int SpecificationAttributeGroupId { get; set; }

        #endregion
    }
}