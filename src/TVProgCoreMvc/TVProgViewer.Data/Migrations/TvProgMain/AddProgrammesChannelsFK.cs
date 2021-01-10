
using FluentMigrator;
using TVProgViewer.Core.Domain.TvProgMain;
using TVProgViewer.Data.Extensions;

namespace TVProgViewer.Data.Migrations.TvProgMain
{
    [TvProgMigration("2020/06/28 21:31:03:3616738")]
    public class AddProgrammesChannelsFK: AutoReversingMigration
    {
        #region Методы

        public override void Up()
        {
            this.AddForeignKey(nameof(Programmes),
                nameof(Programmes.ChannelId),
                nameof(Channels),
                nameof(Channels.Id));
        }
        #endregion
    }
}
