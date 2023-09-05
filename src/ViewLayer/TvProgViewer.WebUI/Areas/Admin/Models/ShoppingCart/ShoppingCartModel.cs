using TvProgViewer.Web.Framework.Models;
using TvProgViewer.Web.Framework.Mvc.ModelBinding;

namespace TvProgViewer.WebUI.Areas.Admin.Models.ShoppingCart
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