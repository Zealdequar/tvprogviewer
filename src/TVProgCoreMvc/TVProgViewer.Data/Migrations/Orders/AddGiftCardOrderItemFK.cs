using FluentMigrator;
using TVProgViewer.Core.Domain.Orders;
using TVProgViewer.Data.Extensions;

namespace TVProgViewer.Data.Migrations.Orders
{
    [TvProgMigration("2019/11/19 05:11:32:2130581")]
    public class AddGiftCardOrderItemFK : AutoReversingMigration
    {
        #region Methods

        public override void Up()
        {
            this.AddForeignKey(nameof(GiftCard),
                nameof(GiftCard.PurchasedWithOrderItemId),
                nameof(OrderItem),
                nameof(OrderItem.Id));
        }

        #endregion
    }
}