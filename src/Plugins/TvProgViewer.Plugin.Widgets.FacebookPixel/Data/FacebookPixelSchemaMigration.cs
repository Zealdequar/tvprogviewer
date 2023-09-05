using FluentMigrator;
using TvProgViewer.Data.Extensions;
using TvProgViewer.Data.Migrations;
using TvProgViewer.Plugin.Widgets.FacebookPixel.Domain;

namespace TvProgViewer.Plugin.Widgets.FacebookPixel.Data
{
    [TvProgMigration("2020/03/25 12:00:00", "Widgets.FacebookPixel base schema", MigrationProcessType.Installation)]
    public class FacebookPixelSchemaMigration : AutoReversingMigration
    {

        #region Methods

        /// <summary>
        /// Collect the UP migration expressions
        /// </summary>
        public override void Up()
        {
            Create.TableFor<FacebookPixelConfiguration>();
        }

        #endregion
    }
}