using TVProgViewer.WebUI.Areas.Admin.Models.Catalog;
using TVProgViewer.Web.Framework.Models;

namespace TVProgViewer.WebUI.Areas.Admin.Models.Users
{
    /// <summary>
    /// Represents a user role product list model
    /// </summary>
    public partial record UserRoleProductListModel : BasePagedListModel<ProductModel>
    {
    }
}