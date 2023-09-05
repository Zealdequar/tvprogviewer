using TvProgViewer.WebUI.Areas.Admin.Models.ShoppingCart;
using TvProgViewer.Web.Framework.Models;

namespace TvProgViewer.WebUI.Areas.Admin.Models.Users
{
    /// <summary>
    /// Represents a user shopping cart list model
    /// </summary>
    public partial record UserShoppingCartListModel : BasePagedListModel<ShoppingCartItemModel>
    {
    }
}