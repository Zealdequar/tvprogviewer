using FluentMigrator;
using TvProgViewer.Core.Domain.TvProgMain;
using TvProgViewer.Core.Domain.Users;
using TvProgViewer.Data.Extensions;

namespace TvProgViewer.Data.Migrations.UpgradeTo472
{
    [TvProgMigration("2024-01-14 13:31:00", "4.72.0 Schema new objects", MigrationProcessType.Update)]
    public class SchemaMigration : Migration
    {
        /// <summary>
        /// Собрать то, что нужно накатить
        /// </summary>
        public override void Up()
        {
            Create.TableFor<UserChannelMapping>();

            // Добавление колонок:
            var channelTableName = nameof(Channels);

            var userRatingColumnName = nameof(Channels.UserRating);

            if (!Schema.Table(channelTableName).Column(userRatingColumnName).Exists())
            {
                Alter.Table(channelTableName)
                        .AddColumn(userRatingColumnName).AsInt32().Nullable();
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
