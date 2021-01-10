using FluentMigrator;
using TVProgViewer.Core.Domain.Catalog;
using TVProgViewer.Data.Extensions;
using TVProgViewer.Data.Mapping;

namespace TVProgViewer.Data.Migrations.Indexes
{
    [TvProgMigration("2019/12/19 11:35:09:1647938")]
    public class AddProductManufacturerMappingIsFeaturedProductIX : AutoReversingMigration
    {
        #region Methods

        public override void Up()
        {
            this.AddIndex("IX_Product_Manufacturer_Mapping_IsFeaturedProduct",
                TvProgMappingDefaults.ProductManufacturerTable, i => i.Ascending(),
                nameof(ProductManufacturer.IsFeaturedProduct));
        }

        #endregion
    }
}