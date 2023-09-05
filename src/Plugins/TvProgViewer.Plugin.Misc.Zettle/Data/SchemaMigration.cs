using FluentMigrator;
using TvProgViewer.Data.Extensions;
using TvProgViewer.Data.Migrations;
using TvProgViewer.Plugin.Misc.Zettle.Domain;

namespace TvProgViewer.Plugin.Misc.Zettle.Data
{
    [TvProgMigration("2022/09/15 12:00:00", "Misc.Zettle base schema", MigrationProcessType.Installation)]
    public class SchemaMigration : AutoReversingMigration
    {
        #region Methods

        /// <summary>
        /// Collect the UP migration expressions
        /// </summary>
        public override void Up()
        {
            Create.TableFor<ZettleRecord>();
        }

        #endregion
    }
}