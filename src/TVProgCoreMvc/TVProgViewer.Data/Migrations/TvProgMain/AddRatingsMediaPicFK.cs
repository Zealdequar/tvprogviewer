
using FluentMigrator;
using TVProgViewer.Core.Domain.TvProgMain;
using TVProgViewer.Data.Extensions;

namespace TVProgViewer.Data.Migrations.TvProgMain
{
    [TvProgMigration("2020/06/28 22:30:59:9688130")]
    public class AddRatingsMediaPicFK: AutoReversingMigration
    {
        #region Методы

        public override void Up()
        {
            this.AddForeignKey(nameof(Ratings),
                nameof(Ratings.IconId),
                nameof(MediaPic),
                nameof(MediaPic.Id));
        }
        #endregion
    }
}
