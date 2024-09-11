using TvProgViewer.Web.Framework.Models;

namespace TvProgViewer.WebUI.Areas.Admin.Models.Catalog
{
    /// <summary>
    /// Represents a tvChannel specification attribute model
    /// </summary>
    public partial record TvChannelSpecificationAttributeModel : BaseTvProgEntityModel
    {
        #region Properties

        public int AttributeTypeId { get; set; }

        public string AttributeTypeName { get; set; }

        public int AttributeId { get; set; }

        public string AttributeName { get; set; }

        public string ValueRaw { get; set; }

        public bool AllowFiltering { get; set; }

        public bool ShowOnTvChannelPage { get; set; }

        public int DisplayOrder { get; set; }

        public int SpecificationAttributeOptionId { get; set; }

        #endregion
    }
}