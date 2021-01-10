using TVProgViewer.Web.Framework.Models;

namespace TVProgViewer.WebUI.Areas.Admin.Models.Catalog
{
    /// <summary>
    /// Represents a product specification attribute model
    /// </summary>
    public partial record ProductSpecificationAttributeModel : BaseTvProgEntityModel
    {
        #region Properties

        public int AttributeTypeId { get; set; }

        public string AttributeTypeName { get; set; }

        public int AttributeId { get; set; }

        public string AttributeName { get; set; }

        public string ValueRaw { get; set; }

        public bool AllowFiltering { get; set; }

        public bool ShowOnProductPage { get; set; }

        public int DisplayOrder { get; set; }

        public int SpecificationAttributeOptionId { get; set; }

        #endregion
    }
}