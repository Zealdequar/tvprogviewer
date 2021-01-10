using FluentMigrator;
using TVProgViewer.Core.Domain.Catalog;
using TVProgViewer.Data.Extensions;

namespace TVProgViewer.Data.Migrations.Indexes
{
    [TvProgMigration("2019/12/19 09:36:08:9037695")]
    public class AddManufacturerDisplayOrderIX : AutoReversingMigration
    {
        #region Methods          

        public override void Up()
        {
            this.AddIndex("IX_Manufacturer_DisplayOrder", nameof(Manufacturer), i => i.Ascending(),
                nameof(Manufacturer.DisplayOrder));
        }

        #endregion
    }
}