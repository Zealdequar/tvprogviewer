using System.Data;
using FluentMigrator;
using TVProgViewer.Core.Domain.Catalog;
using TVProgViewer.Core.Domain.Shipping;
using TVProgViewer.Data.Extensions;

namespace TVProgViewer.Data.Migrations.Catalog
{
    [TvProgMigration("2019/11/19 12:58:18:0051781")]
    public class AddProductWarehouseInventoryWarehouseFK : AutoReversingMigration
    {
        #region Methods

        public override void Up()
        {
            this.AddForeignKey(nameof(ProductWarehouseInventory),
                nameof(ProductWarehouseInventory.WarehouseId), 
                nameof(Warehouse), 
                nameof(Warehouse.Id), 
                Rule.Cascade);
        }

        #endregion
    }
}