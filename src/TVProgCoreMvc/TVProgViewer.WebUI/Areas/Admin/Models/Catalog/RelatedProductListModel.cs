using TVProgViewer.Web.Framework.Models;

namespace TVProgViewer.WebUI.Areas.Admin.Models.Catalog
{
    /// <summary>
    /// Represents a related product list model
    /// </summary>
    public partial record RelatedProductListModel : BasePagedListModel<RelatedProductModel>
    {
    }
}