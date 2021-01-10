using System.Data;
using FluentMigrator;
using TVProgViewer.Core.Domain.Shipping;
using TVProgViewer.Data.Extensions;

namespace TVProgViewer.Data.Migrations.Shipping
{
    [TvProgMigration("2019/11/19 05:41:32:1984734")]
    public class AddShipmentItemShipmentFK : AutoReversingMigration
    {
        #region Methods

        public override void Up()
        {
            this.AddForeignKey(nameof(ShipmentItem),
                nameof(ShipmentItem.ShipmentId),
                nameof(Shipment),
                nameof(Shipment.Id),
                Rule.Cascade);
        }

        #endregion
    }
}