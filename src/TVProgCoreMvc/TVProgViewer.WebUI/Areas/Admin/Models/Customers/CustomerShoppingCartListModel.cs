using TVProgViewer.WebUI.Areas.Admin.Models.ShoppingCart;
using TVProgViewer.Web.Framework.Models;

namespace TVProgViewer.WebUI.Areas.Admin.Models.Users
{
    /// <summary>
    /// Represents a user shopping cart list model
    /// </summary>
    public partial record UserShoppingCartListModel : BasePagedListModel<ShoppingCartItemModel>
    {
    }
}