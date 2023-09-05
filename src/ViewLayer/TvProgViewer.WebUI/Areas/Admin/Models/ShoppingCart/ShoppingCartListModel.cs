using TvProgViewer.Web.Framework.Models;

namespace TvProgViewer.WebUI.Areas.Admin.Models.ShoppingCart
{
    /// <summary>
    /// Represents a shopping cart list model
    /// </summary>
    public partial record ShoppingCartListModel : BasePagedListModel<ShoppingCartModel>
    {
    }
}