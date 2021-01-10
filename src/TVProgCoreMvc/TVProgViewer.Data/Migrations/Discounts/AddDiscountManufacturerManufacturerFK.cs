using System.Data;
using FluentMigrator;
using TVProgViewer.Core.Domain.Catalog;
using TVProgViewer.Data.Extensions;
using TVProgViewer.Data.Mapping;

namespace TVProgViewer.Data.Migrations.Discounts
{
    [TvProgMigration("2019/11/19 04:23:34:9883529")]
    public class AddDiscountManufacturerManufacturerFK : AutoReversingMigration
    {
        #region Methods

        public override void Up()
        {
            this.AddForeignKey(TvProgMappingDefaults.DiscountAppliedToManufacturersTable,
                "Manufacturer_Id",
                nameof(Manufacturer),
                nameof(Manufacturer.Id),
                Rule.Cascade);
        }

        #endregion
    }
}