using TVProgViewer.Web.Framework.Mvc.ModelBinding;
using TVProgViewer.Web.Framework.Models;

namespace TVProgViewer.WebUI.Areas.Admin.Models.Orders
{
    /// <summary>
    /// Represents a recurring payment model
    /// </summary>
    public partial record RecurringPaymentModel : BaseTvProgEntityModel
    {
        #region Ctor

        public RecurringPaymentModel()
        {
            RecurringPaymentHistorySearchModel = new RecurringPaymentHistorySearchModel();
        }

        #endregion

        #region Properties

        [TvProgResourceDisplayName("Admin.RecurringPayments.Fields.ID")]
        public override int Id { get; set; }

        [TvProgResourceDisplayName("Admin.RecurringPayments.Fields.CycleLength")]
        public int CycleLength { get; set; }

        [TvProgResourceDisplayName("Admin.RecurringPayments.Fields.CyclePeriod")]
        public int CyclePeriodId { get; set; }

        [TvProgResourceDisplayName("Admin.RecurringPayments.Fields.CyclePeriod")]
        public string CyclePeriodStr { get; set; }

        [TvProgResourceDisplayName("Admin.RecurringPayments.Fields.TotalCycles")]
        public int TotalCycles { get; set; }

        [TvProgResourceDisplayName("Admin.RecurringPayments.Fields.StartDate")]
        public string StartDate { get; set; }

        [TvProgResourceDisplayName("Admin.RecurringPayments.Fields.IsActive")]
        public bool IsActive { get; set; }

        [TvProgResourceDisplayName("Admin.RecurringPayments.Fields.NextPaymentDate")]
        public string NextPaymentDate { get; set; }

        [TvProgResourceDisplayName("Admin.RecurringPayments.Fields.CyclesRemaining")]
        public int CyclesRemaining { get; set; }

        [TvProgResourceDisplayName("Admin.RecurringPayments.Fields.InitialOrder")]
        public int InitialOrderId { get; set; }

        [TvProgResourceDisplayName("Admin.RecurringPayments.Fields.User")]
        public int UserId { get; set; }

        [TvProgResourceDisplayName("Admin.RecurringPayments.Fields.User")]
        public string UserEmail { get; set; }

        [TvProgResourceDisplayName("Admin.RecurringPayments.Fields.PaymentType")]
        public string PaymentType { get; set; }

        public bool CanCancelRecurringPayment { get; set; }

        public bool LastPaymentFailed { get; set; }

        public RecurringPaymentHistorySearchModel RecurringPaymentHistorySearchModel { get; set; }

        #endregion
    }
}