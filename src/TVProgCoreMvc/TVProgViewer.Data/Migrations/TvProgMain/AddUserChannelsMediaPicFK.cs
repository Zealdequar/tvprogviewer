
using FluentMigrator;
using TVProgViewer.Core.Domain.TvProgMain;
using TVProgViewer.Data.Extensions;

namespace TVProgViewer.Data.Migrations.TvProgMain
{
    [TvProgMigration("2020/06/28 22:52:08:8963923")]
    public class AddUserChannelsMediaPicFK: AutoReversingMigration
    {
        #region Методы

        public override void Up()
        {
            this.AddForeignKey(nameof(UserChannels),
                nameof(UserChannels.IconId),
                nameof(MediaPic),
                nameof(MediaPic.Id));
        }
        #endregion
    }
}
