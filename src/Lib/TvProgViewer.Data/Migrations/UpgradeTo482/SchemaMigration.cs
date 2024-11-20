using FluentMigrator;
using TvProgViewer.Core.Domain.Users;

namespace TvProgViewer.Data.Migrations.UpgradeTo482
{
    [TvProgMigration("2024-11-20 21:28:00", "4.82.0 Schema new objects", MigrationProcessType.Update)]
    public class SchemaMigration : Migration
    {
        /// <summary>
        /// Собрать то, что нужно накатить
        /// </summary>
        public override void Up()
        {
            // Добавление колонки:
            var userChannelMappingTableName = nameof(UserChannelMapping);

            var userChannelMappingColumnName = nameof(UserChannelMapping.SetDate);

            if (!Schema.Table(userChannelMappingTableName).Column(userChannelMappingColumnName).Exists())
            {
                Alter.Table(userChannelMappingTableName)
                    .AddColumn(userChannelMappingColumnName).AsDateTime2().Nullable();
            }
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
