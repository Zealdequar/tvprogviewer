using FluentMigrator;
using TVProgViewer.Core.Domain.TvProgMain;
using TVProgViewer.Data.Extensions;

namespace TVProgViewer.Data.Migrations.TvProgMain
{
    [TvProgMigration("2020/06/24 22:51:16:2551771")]
    public class AddChannelsTvProgProvidersFK: AutoReversingMigration
    {
        #region Методы

        public override void Up()
        {
            this.AddForeignKey(nameof(Channels),
                nameof(Channels.TvProgProviderId),
                nameof(TvProgProviders),
                nameof(TvProgProviders.Id));
        }

        #endregion
    }
}
