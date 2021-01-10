
using FluentMigrator;
using TVProgViewer.Core.Domain.TvProgMain;
using TVProgViewer.Data.Extensions;

namespace TVProgViewer.Data.Migrations.TvProgMain
{
    [TvProgMigration("2020/06/28 23:07:04:4968884")]
    public class AddUsersProgramsRatingsFK: AutoReversingMigration
    {
        #region Методы

        public override void Up()
        {
            this.AddForeignKey(nameof(UsersPrograms),
                nameof(UsersPrograms.RatingId),
                nameof(Ratings),
                nameof(Ratings.Id));
        }
        #endregion
    }
}
