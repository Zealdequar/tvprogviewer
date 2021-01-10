using System;
using TVProgViewer.Web.Framework.Mvc.ModelBinding;
using TVProgViewer.Web.Framework.Models;

namespace TVProgViewer.WebUI.Areas.Admin.Models.ShoppingCart
{
    /// <summary>
    /// Represents a shopping cart item model
    /// </summary>
    public partial record ShoppingCartItemModel : BaseTvProgEntityModel
    {
        #region Properties

        [TvProgResourceDisplayName("Admin.CurrentCarts.Store")]
        public string Store { get; set; }

        [TvProgResourceDisplayName("Admin.CurrentCarts.Product")]
        public int ProductId { get; set; }

        [TvProgResourceDisplayName("Admin.CurrentCarts.Product")]
        public string ProductName { get; set; }

        public string AttributeInfo { get; set; }

        [TvProgResourceDisplayName("Admin.CurrentCarts.UnitPrice")]
        public string UnitPrice { get; set; }

        [TvProgResourceDisplayName("Admin.CurrentCarts.Quantity")]
        public int Quantity { get; set; }

        [TvProgResourceDisplayName("Admin.CurrentCarts.Total")]
        public string Total { get; set; }

        [TvProgResourceDisplayName("Admin.CurrentCarts.UpdatedOn")]
        public DateTime UpdatedOn { get; set; }

        #endregion
    }
}