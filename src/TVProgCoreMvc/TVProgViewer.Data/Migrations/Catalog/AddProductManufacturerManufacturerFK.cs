using System.Data;
using FluentMigrator;
using TVProgViewer.Core.Domain.Catalog;
using TVProgViewer.Data.Extensions;
using TVProgViewer.Data.Mapping;

namespace TVProgViewer.Data.Migrations.Catalog
{
    [TvProgMigration("2019/11/19 12:07:33:9067594")]
    public class AddProductManufacturerManufacturerFK : AutoReversingMigration
    {
        #region Methods

        public override void Up()
        {
            this.AddForeignKey(TvProgMappingDefaults.ProductManufacturerTable,
                nameof(ProductManufacturer.ManufacturerId),
                nameof(Manufacturer),
                nameof(Manufacturer.Id),
                Rule.Cascade);
        }

        #endregion
    }
}