using FluentMigrator;
using TvProgViewer.Core.Domain.TvProgMain;
using TvProgViewer.Core.Domain.Users;
using TvProgViewer.Data.Extensions;

namespace TvProgViewer.Data.Migrations.UpgradeTo489
{
    [TvProgMigration("2026-01-16 20:04:33", "4.89.0 Schema new objects", MigrationProcessType.Update)]
    public class SchemaMigration: Migration
    {
        /// <summary>
        /// Собрать то, что нужно накатить
        /// </summary>
        public override void Up()
        {
            Create.TableFor<TvProgCategory>();
            Create.TableFor<ProgrammesTvProgCategoryMapping>();
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
