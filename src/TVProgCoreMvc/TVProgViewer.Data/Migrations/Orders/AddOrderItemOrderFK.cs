using System.Data;
using FluentMigrator;
using TVProgViewer.Core.Domain.Orders;
using TVProgViewer.Data.Extensions;

namespace TVProgViewer.Data.Migrations.Orders
{
    [TvProgMigration("2019/11/19 05:14:20:9436788")]
    public class AddOrderItemOrderFK : AutoReversingMigration
    {
        #region Methods

        public override void Up()
        {
            this.AddForeignKey(nameof(OrderItem),
                nameof(OrderItem.OrderId),
                nameof(Order),
                nameof(Order.Id),
                Rule.Cascade);
        }

        #endregion
    }
}