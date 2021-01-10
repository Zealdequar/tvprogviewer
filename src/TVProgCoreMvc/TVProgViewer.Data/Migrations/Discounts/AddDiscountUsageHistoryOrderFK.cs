using System.Data;
using FluentMigrator;
using TVProgViewer.Core.Domain.Discounts;
using TVProgViewer.Core.Domain.Orders;
using TVProgViewer.Data.Extensions;

namespace TVProgViewer.Data.Migrations.Discounts
{
    [TvProgMigration("2019/11/19 04:33:24:1180784")]
    public class AddDiscountUsageHistoryOrderFK : AutoReversingMigration
    {
        #region Methods

        public override void Up()
        {
            this.AddForeignKey(nameof(DiscountUsageHistory),
                nameof(DiscountUsageHistory.OrderId),
                nameof(Order),
                nameof(Order.Id),
                Rule.Cascade);
        }

        #endregion
    }
}