using System.Data;
using FluentMigrator;
using TVProgViewer.Core.Domain.Orders;
using TVProgViewer.Core.Domain.Shipping;
using TVProgViewer.Data.Extensions;

namespace TVProgViewer.Data.Migrations.Shipping
{
    [TvProgMigration("2019/11/19 05:42:48:1126845")]
    public class AddShipmentOrderFK : AutoReversingMigration
    {
        #region Methods

        public override void Up()
        {
            this.AddForeignKey(nameof(Shipment),
                nameof(Shipment.OrderId),
                nameof(Order),
                nameof(Order.Id),
                Rule.Cascade);
        }

        #endregion
    }
}