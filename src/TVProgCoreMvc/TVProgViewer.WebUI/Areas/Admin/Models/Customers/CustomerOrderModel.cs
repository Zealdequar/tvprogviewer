using System;
using TVProgViewer.Web.Framework.Models;
using TVProgViewer.Web.Framework.Mvc.ModelBinding;

namespace TVProgViewer.WebUI.Areas.Admin.Models.Users
{
    /// <summary>
    /// Represents a user order model
    /// </summary>
    public partial record UserOrderModel : BaseTvProgEntityModel
    {
        #region Properties

        public override int Id { get; set; }

        [TvProgResourceDisplayName("Admin.Users.Users.Orders.CustomOrderNumber")]
        public string CustomOrderNumber { get; set; }

        [TvProgResourceDisplayName("Admin.Users.Users.Orders.OrderStatus")]
        public string OrderStatus { get; set; }

        [TvProgResourceDisplayName("Admin.Users.Users.Orders.OrderStatus")]
        public int OrderStatusId { get; set; }

        [TvProgResourceDisplayName("Admin.Users.Users.Orders.PaymentStatus")]
        public string PaymentStatus { get; set; }

        [TvProgResourceDisplayName("Admin.Users.Users.Orders.ShippingStatus")]
        public string ShippingStatus { get; set; }

        [TvProgResourceDisplayName("Admin.Users.Users.Orders.OrderTotal")]
        public string OrderTotal { get; set; }

        [TvProgResourceDisplayName("Admin.Users.Users.Orders.Store")]
        public string StoreName { get; set; }

        [TvProgResourceDisplayName("Admin.Users.Users.Orders.CreatedOn")]
        public DateTime CreatedOn { get; set; }

        #endregion
    }
}