using FluentMigrator;
using TVProgViewer.Core.Domain.Orders;
using TVProgViewer.Data.Extensions;

namespace TVProgViewer.Data.Migrations.Orders
{
    [TvProgMigration("2019/11/19 05:25:41:0960207")]
    public class AddRecurringPaymentOrderFK : AutoReversingMigration
    {
        #region Methods

        public override void Up()
        {
            this.AddForeignKey(nameof(RecurringPayment),
                nameof(RecurringPayment.InitialOrderId),
                nameof(Order),
                nameof(Order.Id));
        }

        #endregion
    }
}