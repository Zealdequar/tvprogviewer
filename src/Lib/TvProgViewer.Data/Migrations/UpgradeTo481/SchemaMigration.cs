using FluentMigrator;
using TvProgViewer.Core.Domain.Catalog;
using TvProgViewer.Core.Domain.Common;

namespace TvProgViewer.Data.Migrations.UpgradeTo481
{
    [TvProgMigration("2024-03-21 11:55:00", "4.81.0 Schema new objects", MigrationProcessType.Update)]
    public class SchemaMigration : Migration
    {
        /// <summary>
        /// Собрать то, что нужно накатить
        /// </summary>
        public override void Up()
        {
            // Добавление колонки:
            var tvChannelTableName = nameof(TvChannel);

            var tvChannelLiveUrlColumnName = nameof(TvChannel.TvChannelLiveUrl);

            if (!Schema.Table(tvChannelTableName).Column(tvChannelLiveUrlColumnName).Exists())
            {
                Alter.Table(tvChannelTableName)
                    .AddColumn(tvChannelLiveUrlColumnName).AsString(80000).Nullable();
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
