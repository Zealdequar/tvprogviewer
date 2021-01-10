using TVProgViewer.Web.Framework.Models;

namespace TVProgViewer.WebUI.Areas.Admin.Models.Catalog
{
    /// <summary>
    /// Represents an associated product list model
    /// </summary>
    public partial record AssociatedProductListModel : BasePagedListModel<AssociatedProductModel>
    {
    }
}