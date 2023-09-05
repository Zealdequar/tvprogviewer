using FluentMigrator;
using TvProgViewer.Data.Extensions;
using TvProgViewer.Data.Migrations;
using TvProgViewer.Plugin.Tax.FixedOrByCountryStateZip.Domain;

namespace TvProgViewer.Plugin.Tax.FixedOrByCountryStateZip.Data
{
    [TvProgMigration("2020/02/03 09:27:23:6455432", "Tax.FixedOrByCountryStateZip base schema", MigrationProcessType.Installation)]
    public class SchemaMigration : AutoReversingMigration
    {
        public override void Up()
        {
            Create.TableFor<TaxRate>();
        }
    }
}