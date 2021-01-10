using System.Data;
using FluentMigrator;
using TVProgViewer.Core.Domain.Discounts;
using TVProgViewer.Data.Extensions;

namespace TVProgViewer.Data.Migrations.Discounts
{
    [TvProgMigration("2019/11/19 04:33:24:1180783")]
    public class AddDiscountUsageHistoryDiscountFK : AutoReversingMigration
    {
        #region Methods

        public override void Up()
        {
            this.AddForeignKey(nameof(DiscountUsageHistory),
                nameof(DiscountUsageHistory.DiscountId),
                nameof(Discount),
                nameof(Discount.Id),
                Rule.Cascade);
        }

        #endregion
    }
}