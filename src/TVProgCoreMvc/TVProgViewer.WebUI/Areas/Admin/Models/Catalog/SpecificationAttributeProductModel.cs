using TVProgViewer.Web.Framework.Models;
using TVProgViewer.Web.Framework.Mvc.ModelBinding;

namespace TVProgViewer.WebUI.Areas.Admin.Models.Catalog
{
    /// <summary>
    /// Represents a model of products that use the specification attribute
    /// </summary>
    public partial record SpecificationAttributeProductModel : BaseTvProgEntityModel
    {
        #region Properties

        public int SpecificationAttributeId { get; set; }

        public int ProductId { get; set; }

        [TvProgResourceDisplayName("Admin.Catalog.Attributes.SpecificationAttributes.UsedByProducts.Product")]
        public string ProductName { get; set; }

        [TvProgResourceDisplayName("Admin.Catalog.Attributes.SpecificationAttributes.UsedByProducts.Published")]
        public bool Published { get; set; }

        #endregion
    }
}