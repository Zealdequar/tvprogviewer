using FluentMigrator;
using FluentMigrator.SqlServer;
using TVProgViewer.Core.Domain.Catalog;
using TVProgViewer.Data.Extensions;
using TVProgViewer.Data.Mapping;

namespace TVProgViewer.Data.Migrations.Indexes
{
    [TvProgMigration("2019/12/19 12:02:35:9280390")]
    public class AddPMMProductIdExtendedIX : AutoReversingMigration
    {
        #region Methods          

        public override void Up()
        {
            this.AddIndex("IX_PMM_ProductId_Extended", TvProgMappingDefaults.ProductManufacturerTable, i => i.Ascending(),
                    nameof(ProductManufacturer.ProductId), nameof(ProductManufacturer.IsFeaturedProduct))
                .Include(nameof(ProductManufacturer.ManufacturerId));
        }

        #endregion
    }
}