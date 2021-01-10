
using FluentMigrator;
using TVProgViewer.Core.Domain.TvProgMain;
using TVProgViewer.Data.Extensions;

namespace TVProgViewer.Data.Migrations.TvProgMain
{
    [TvProgMigration("2020/06/28 22:50:02:2957497")]
    public class AddUserChannelsChannelsFK: AutoReversingMigration
    {
        #region Методы

        public override void Up()
        {
            this.AddForeignKey(nameof(UserChannels),
                nameof(UserChannels.ChannelId),
                nameof(Channels),
                nameof(Channels.Id));
        }
        #endregion
    }
}
