using System.Data;
using FluentMigrator;
using TVProgViewer.Core.Domain.Users;
using TVProgViewer.Core.Domain.Orders;
using TVProgViewer.Data.Extensions;

namespace TVProgViewer.Data.Migrations.Users
{
    [TvProgMigration("2019/11/19 02:35:25:2342367")]
    public class AddRewardPointsHistoryOrderFK : AutoReversingMigration
    {
        #region Methods

        public override void Up()
        {
            this.AddForeignKey(nameof(RewardPointsHistory),
                nameof(RewardPointsHistory.OrderId),
                nameof(Order),
                nameof(Order.Id),
                Rule.SetNull);
        }

        #endregion
    }
}