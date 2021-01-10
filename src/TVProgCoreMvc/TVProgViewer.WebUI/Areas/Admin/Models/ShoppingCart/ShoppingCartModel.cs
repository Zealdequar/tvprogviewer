using TVProgViewer.Web.Framework.Models;
using TVProgViewer.Web.Framework.Mvc.ModelBinding;

namespace TVProgViewer.WebUI.Areas.Admin.Models.ShoppingCart
{
    /// <summary>
    /// Represents a shopping cart model
    /// </summary>
    public partial record ShoppingCartModel : BaseTvProgModel
    {
        #region Properties

        [TvProgResourceDisplayName("Admin.CurrentCarts.User")]
        public int UserId { get; set; }

        [TvProgResourceDisplayName("Admin.CurrentCarts.User")]
        public string UserEmail { get; set; }

        [TvProgResourceDisplayName("Admin.CurrentCarts.TotalItems")]
        public int TotalItems { get; set; }

        #endregion
    }
}