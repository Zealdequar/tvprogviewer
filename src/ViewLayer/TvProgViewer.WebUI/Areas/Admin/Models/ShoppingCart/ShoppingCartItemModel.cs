using System;
using TvProgViewer.Web.Framework.Mvc.ModelBinding;
using TvProgViewer.Web.Framework.Models;

namespace TvProgViewer.WebUI.Areas.Admin.Models.ShoppingCart
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
        public decimal UnitPriceValue { get; set; }

        [TvProgResourceDisplayName("Admin.CurrentCarts.Quantity")]
        public int Quantity { get; set; }

        [TvProgResourceDisplayName("Admin.CurrentCarts.Total")]
        public string Total { get; set; }
        public decimal TotalValue { get; set; }

        [TvProgResourceDisplayName("Admin.CurrentCarts.UpdatedOn")]
        public DateTime UpdatedOn { get; set; }

        #endregion
    }
}