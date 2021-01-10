using FluentMigrator;
using TVProgViewer.Core.Domain.TvProgMain;
using TVProgViewer.Data.Extensions;

namespace TVProgViewer.Data.Migrations.TvProgMain
{
    [TvProgMigration("2020/06/28 20:55:47:8971489")]
    public class AddChannelsMediaPicFK: AutoReversingMigration
    {
        #region Методы

        public override void Up()
        {
            this.AddForeignKey(nameof(Channels),
                nameof(Channels.IconId),
                nameof(MediaPic),
                nameof(MediaPic.Id));
        }

        #endregion
    }
}
