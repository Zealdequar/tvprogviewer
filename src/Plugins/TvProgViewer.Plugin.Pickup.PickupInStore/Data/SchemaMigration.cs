using FluentMigrator;
using TvProgViewer.Data.Extensions;
using TvProgViewer.Data.Migrations;
using TvProgViewer.Plugin.Pickup.PickupInStore.Domain;

namespace TvProgViewer.Plugin.Pickup.PickupInStore.Data
{
    [TvProgMigration("2020/02/03 09:30:17:6455422", "Pickup.PickupInStore base schema", MigrationProcessType.Installation)]
    public class SchemaMigration : AutoReversingMigration
    {
        public override void Up()
        {
            Create.TableFor<StorePickupPoint>();
        }
    }
}