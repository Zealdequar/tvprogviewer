using FluentMigrator;
using TVProgViewer.Core.Domain.Catalog;

namespace TVProgViewer.Data.Migrations.Indexes
{
    [TvProgMigration("2020/03/13 11:35:09:1647932")]
    public class AddManufacturerLimitedToStoresIX : AutoReversingMigration
    {
        #region Methods

        public override void Up()
        {
            Create.Index("IX_Manufacturer_LimitedToStores").OnTable(nameof(Manufacturer))
                .OnColumn(nameof(Manufacturer.LimitedToStores)).Ascending()
                .WithOptions().NonClustered();
        }

        #endregion
    }
}