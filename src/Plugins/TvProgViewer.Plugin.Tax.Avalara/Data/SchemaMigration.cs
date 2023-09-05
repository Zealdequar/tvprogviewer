using FluentMigrator;
using TvProgViewer.Data.Extensions;
using TvProgViewer.Data.Migrations;
using TvProgViewer.Plugin.Tax.Avalara.Domain;

namespace TvProgViewer.Plugin.Tax.Avalara.Data
{
    [TvProgMigration("2020/02/03 09:09:17:6455442", "Tax.Avalara base schema", MigrationProcessType.Installation)]
    public class SchemaMigration : AutoReversingMigration
    {
        #region Methods

        /// <summary>
        /// Collect the UP migration expressions
        /// </summary>
        public override void Up()
        {
            Create.TableFor<TaxTransactionLog>();
        }

        #endregion
    }
}