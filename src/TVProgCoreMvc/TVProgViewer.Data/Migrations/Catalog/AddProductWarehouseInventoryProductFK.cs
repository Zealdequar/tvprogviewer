using System.Data;
using FluentMigrator;
using TVProgViewer.Core.Domain.Catalog;
using TVProgViewer.Data.Extensions;

namespace TVProgViewer.Data.Migrations.Catalog
{
    [TvProgMigration("2019/11/19 12:58:18:0051780")]
    public class AddProductWarehouseInventoryProductFK : AutoReversingMigration
    {
        #region Methods

        public override void Up()
        {
            this.AddForeignKey(nameof(ProductWarehouseInventory), 
                nameof(ProductWarehouseInventory.ProductId), 
                nameof(Product), 
                nameof(Product.Id), 
                Rule.Cascade);
        }
        
        #endregion
    }
}