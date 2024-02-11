using FluentMigrator;
using TvProgViewer.Core.Domain.Catalog;
using TvProgViewer.Core.Domain.Media;
using TvProgViewer.Data.Extensions;
using TvProgViewer.Data.Mapping;

namespace TvProgViewer.Data.Migrations.UpgradeTo460
{
    [TvProgMigration("2022-03-16 00:00:00", "TvChannel video", MigrationProcessType.Update)]
    public class VideoMigration : Migration
    {
        /// <summary>
        /// Collect the UP migration expressions
        /// </summary>
        public override void Up()
        {
            if (!Schema.Table(NameCompatibilityManager.GetTableName(typeof(Video))).Exists())
            {
                Create.TableFor<Video>();
            }
            if (!Schema.Table(NameCompatibilityManager.GetTableName(typeof(TvChannelVideo))).Exists())
            {
                Create.TableFor<TvChannelVideo>();
            }

        }

        public override void Down()
        {
            //add the downgrade logic if necessary 
        }
    }
}
