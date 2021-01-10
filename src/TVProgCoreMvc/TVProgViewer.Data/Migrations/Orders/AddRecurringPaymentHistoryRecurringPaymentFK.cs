using System.Data;
using FluentMigrator;
using TVProgViewer.Core.Domain.Orders;
using TVProgViewer.Data.Extensions;

namespace TVProgViewer.Data.Migrations.Orders
{
    [TvProgMigration("2019/11/19 05:23:41:0887644")]
    public class AddRecurringPaymentHistoryRecurringPaymentFK : AutoReversingMigration
    {
        #region Methods

        public override void Up()
        {
            this.AddForeignKey(nameof(RecurringPaymentHistory),
                nameof(RecurringPaymentHistory.RecurringPaymentId),
                nameof(RecurringPayment),
                nameof(RecurringPayment.Id),
                Rule.Cascade);
        }

        #endregion
    }
}