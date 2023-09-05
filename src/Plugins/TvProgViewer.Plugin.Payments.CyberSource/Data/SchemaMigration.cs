using FluentMigrator;
using TvProgViewer.Data.Extensions;
using TvProgViewer.Data.Migrations;
using TvProgViewer.Plugin.Payments.CyberSource.Domain;

namespace TvProgViewer.Plugin.Payments.CyberSource.Data
{
    [TvProgMigration("2022-04-13 00:00:00", "Payments.CyberSource base schema", MigrationProcessType.Installation)]
    public class SchemaMigration : AutoReversingMigration
    {
        #region Methods

        /// <summary>
        /// Collect the UP migration expressions
        /// </summary>
        public override void Up()
        {
            Create.TableFor<CyberSourceUserToken>();
        }

        #endregion
    }
}