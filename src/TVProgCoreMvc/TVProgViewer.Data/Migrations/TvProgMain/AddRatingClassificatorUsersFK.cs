
using FluentMigrator;
using TVProgViewer.Core.Domain.TvProgMain;
using TVProgViewer.Core.Domain.Users;
using TVProgViewer.Data.Extensions;

namespace TVProgViewer.Data.Migrations.TvProgMain
{
    [TvProgMigration("2020/06/28 22:24:49:1036844")]
    public class AddRatingClassificatorUsersFK: AutoReversingMigration
    {
        #region Методы

        public override void Up()
        {
            this.AddForeignKey(nameof(RatingClassificator),
                nameof(RatingClassificator.UserId),
                nameof(User),
                nameof(User.Id));
        }
        #endregion
    }
}
