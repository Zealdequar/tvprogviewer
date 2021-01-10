using System;
using TVProgViewer.Web.Framework.Models;
using TVProgViewer.Web.Framework.Mvc.ModelBinding;

namespace TVProgViewer.WebUI.Areas.Admin.Models.Orders
{
    /// <summary>
    /// Represents a recurring payment history model
    /// </summary>
    public partial record RecurringPaymentHistoryModel : BaseTvProgEntityModel
    {
        #region Properties

        public int OrderId { get; set; }

        [TvProgResourceDisplayName("Admin.RecurringPayments.History.CustomOrderNumber")]
        public string CustomOrderNumber { get; set; }

        public int RecurringPaymentId { get; set; }

        [TvProgResourceDisplayName("Admin.RecurringPayments.History.OrderStatus")]
        public string OrderStatus { get; set; }

        [TvProgResourceDisplayName("Admin.RecurringPayments.History.PaymentStatus")]
        public string PaymentStatus { get; set; }

        [TvProgResourceDisplayName("Admin.RecurringPayments.History.ShippingStatus")]
        public string ShippingStatus { get; set; }

        [TvProgResourceDisplayName("Admin.RecurringPayments.History.CreatedOn")]
        public DateTime CreatedOn { get; set; }

        #endregion
    }
}