using FluentMigrator;
using TVProgViewer.Core.Domain.Catalog;
using TVProgViewer.Data.Extensions;
using TVProgViewer.Data.Mapping;

namespace TVProgViewer.Data.Migrations.Indexes
{
    [TvProgMigration("2019/12/19 11:35:09:1647942")]
    public class AddPMMProductManufacturerIX : AutoReversingMigration
    {
        #region Methods

        public override void Up()
        {
            this.AddIndex("IX_PMM_Product_and_Manufacturer", TvProgMappingDefaults.ProductManufacturerTable,
                i => i.Ascending(), nameof(ProductManufacturer.ManufacturerId), nameof(ProductManufacturer.ProductId));
        }

        #endregion
    }
}