using FluentMigrator;
using TvProgViewer.Data.Extensions;
using TvProgViewer.Data.Migrations;
using TvProgViewer.Plugin.Shipping.FixedByWeightByTotal.Domain;

namespace TvProgViewer.Plugin.Shipping.FixedByWeightByTotal.Data
{
    [TvProgMigration("2020/02/03 08:40:55:1687541", "Shipping.FixedByWeightByTotal base schema", MigrationProcessType.Installation)]
    public class SchemaMigration : AutoReversingMigration
    {
        public override void Up()
        {
            Create.TableFor<ShippingByWeightByTotalRecord>();
        }
    }
}