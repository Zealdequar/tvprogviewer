using FluentMigrator;
using TVProgViewer.Core.Domain.Catalog;
using TVProgViewer.Core.Domain.Shipping;
using TVProgViewer.Data.Extensions;

namespace TVProgViewer.Data.Migrations.Catalog
{
    [TvProgMigration("2019/11/19 01:06:56:5419187")]
    public class AddStockQuantityHistoryWarehouseFK : AutoReversingMigration
    {
        #region Methods

        public override void Up()
        {
            this.AddForeignKey(nameof(StockQuantityHistory), 
                nameof(StockQuantityHistory.WarehouseId), 
                nameof(Warehouse), 
                nameof(Warehouse.Id));
        }

        #endregion
    }
}