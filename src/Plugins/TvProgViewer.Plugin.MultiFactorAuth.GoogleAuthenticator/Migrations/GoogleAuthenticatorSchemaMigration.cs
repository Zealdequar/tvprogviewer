using FluentMigrator;
using TvProgViewer.Data.Extensions;
using TvProgViewer.Data.Migrations;
using TvProgViewer.Plugin.MultiFactorAuth.GoogleAuthenticator.Domains;

namespace TvProgViewer.Plugin.MultiFactorAuth.GoogleAuthenticator.Migrations
{
    [TvProgMigration("2020/07/30 12:00:00", "TvProg.Plugin.MultiFactorAuth.GoogleAuthenticator schema", MigrationProcessType.Installation)]
    public class GoogleAuthenticatorSchemaMigration : AutoReversingMigration
    {
        /// <summary>
        /// Collect the UP migration expressions
        /// </summary>
        public override void Up()
        {
            Create.TableFor<GoogleAuthenticatorRecord>();
        }
    }
}
