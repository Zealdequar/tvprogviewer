using TVProgViewer.Web.Framework.Models;

namespace TVProgViewer.WebUI.Areas.Admin.Models.ShoppingCart
{
    /// <summary>
    /// Represents a shopping cart item list model
    /// </summary>
    public partial record ShoppingCartItemListModel : BasePagedListModel<ShoppingCartItemModel>
    {
    }
}