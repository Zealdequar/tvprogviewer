
using FluentMigrator;
using TVProgViewer.Core.Domain.TvProgMain;
using TVProgViewer.Data.Extensions;

namespace TVProgViewer.Data.Migrations.TvProgMain
{
    [TvProgMigration("2020/06/28 22:58:08:4078550")]
    public class AddUsersProgramsUserChannelsFK: AutoReversingMigration
    {
        #region Методы

        public override void Up()
        {
            this.AddForeignKey(nameof(UsersPrograms),
                nameof(UsersPrograms.UserChannelId),
                nameof(UserChannels),
                nameof(UserChannels.Id));
        }
        #endregion
    }
}
