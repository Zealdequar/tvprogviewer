using FluentMigrator;
using TvProgViewer.Core.Domain.Users;
using TvProgViewer.Data.Extensions;

namespace TvProgViewer.Data.Migrations.UpgradeTo487
{
    [TvProgMigration("2026-01-12 20:04:34", "4.87.0 Schema new objects", MigrationProcessType.Update)]
    public class SchemaMigration : Migration
    {
        /// <summary>
        /// Собрать то, что нужно накатить
        /// </summary>
        public override void Up()
        {
            Create.TableFor<UserGreenDataOperations>();
            Create.TableFor<UserGreenDataInfo>();
        }

        /// <summary>
        /// Собрать то, что нужно откатить 
        /// </summary>
        public override void Down()
        {
            // Ничего откатывать не надо
        }
    }
}
