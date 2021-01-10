using TVProgViewer.Web.Framework.Models;

namespace TVProgViewer.WebUI.Areas.Admin.Models.ShoppingCart
{
    /// <summary>
    /// Represents a shopping cart list model
    /// </summary>
    public partial record ShoppingCartListModel : BasePagedListModel<ShoppingCartModel>
    {
    }
}