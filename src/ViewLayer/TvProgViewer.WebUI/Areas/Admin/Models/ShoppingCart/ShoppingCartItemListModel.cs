using TvProgViewer.Web.Framework.Models;

namespace TvProgViewer.WebUI.Areas.Admin.Models.ShoppingCart
{
    /// <summary>
    /// Represents a shopping cart item list model
    /// </summary>
    public partial record ShoppingCartItemListModel : BasePagedListModel<ShoppingCartItemModel>
    {
    }
}