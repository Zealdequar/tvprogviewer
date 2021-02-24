using System.Data;
using FluentMigrator;
using TVProgViewer.Core.Domain.Users;
using TVProgViewer.Core.Domain.Orders;

namespace TVProgViewer.Data.Migrations.Indexes
{
    [TvProgMigration("2020/03/17 11:26:08:9037680")]
    public class AddOrderRewardPointsHistoryFK : AutoReversingMigration
    {
        #region Methods          

        public override void Up()
        {
            Create.ForeignKey().FromTable(nameof(Order)).ForeignColumn(nameof(Order.RewardPointsHistoryEntryId))
                .ToTable(nameof(RewardPointsHistory)).PrimaryColumn(nameof(RewardPointsHistory.Id)).OnDelete(Rule.None);
        }

        #endregion
    }
}
