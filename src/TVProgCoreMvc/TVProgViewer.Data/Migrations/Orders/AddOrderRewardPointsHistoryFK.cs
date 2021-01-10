using FluentMigrator;
using TVProgViewer.Core.Domain.Users;
using TVProgViewer.Core.Domain.Orders;
using TVProgViewer.Data.Extensions;

namespace TVProgViewer.Data.Migrations.Orders
{
    [TvProgMigration("2019/12/16 04:36:01:7140897")]
    public class AddOrderRewardPointsHistoryFK : AutoReversingMigration
    {
        #region Methods

        public override void Up()
        {
            this.AddForeignKey(nameof(Order),
                nameof(Order.RewardPointsHistoryEntryId),
                nameof(RewardPointsHistory),
                nameof(RewardPointsHistory.Id));
        }

        #endregion
    }
}