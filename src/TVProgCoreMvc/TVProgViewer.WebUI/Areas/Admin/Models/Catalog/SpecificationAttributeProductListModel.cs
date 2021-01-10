using TVProgViewer.Web.Framework.Models;

namespace TVProgViewer.WebUI.Areas.Admin.Models.Catalog
{
    /// <summary>
    /// Represents a list model of products that use the specification attribute
    /// </summary>
    public partial record SpecificationAttributeProductListModel : BasePagedListModel<SpecificationAttributeProductModel>
    {
    }
}