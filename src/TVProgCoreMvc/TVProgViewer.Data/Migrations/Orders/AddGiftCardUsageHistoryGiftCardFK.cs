using System.Data;
using FluentMigrator;
using TVProgViewer.Core.Domain.Orders;
using TVProgViewer.Data.Extensions;

namespace TVProgViewer.Data.Migrations.Orders
{
    [TvProgMigration("2019/11/19 05:11:55:6452475")]
    public class AddGiftCardUsageHistoryGiftCardFK : AutoReversingMigration
    {
        #region Methods
        
        public override void Up()
        {
            this.AddForeignKey(nameof(GiftCardUsageHistory),
                nameof(GiftCardUsageHistory.GiftCardId),
                nameof(GiftCard),
                nameof(GiftCard.Id),
                Rule.Cascade);
        }

        #endregion
    }
}