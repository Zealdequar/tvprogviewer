using FluentMigrator;
using TVProgViewer.Core.Domain.Catalog;
using TVProgViewer.Data.Extensions;

namespace TVProgViewer.Data.Migrations.Indexes
{
    [TvProgMigration("2019/12/19 11:35:09:1647941")]
    public class AddGetLowStockProductsIX : AutoReversingMigration
    {
        #region Methods

        public override void Up()
        {
            this.AddIndex("IX_GetLowStockProducts", nameof(Product), i => i.Ascending(), nameof(Product.Deleted),
                nameof(Product.VendorId), nameof(Product.ProductTypeId), nameof(Product.ManageInventoryMethodId),
                nameof(Product.MinStockQuantity), nameof(Product.UseMultipleWarehouses));
        }

        #endregion
    }
}