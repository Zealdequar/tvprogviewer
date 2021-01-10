
using FluentMigrator;
using TVProgViewer.Core.Domain.TvProgMain;
using TVProgViewer.Core.Domain.Users;
using TVProgViewer.Data.Extensions;

namespace TVProgViewer.Data.Migrations.TvProgMain
{
    [TvProgMigration("2020/06/28 21:21:55:5449751")]
    public class AddGenresUsersFK: AutoReversingMigration
    {
        #region Методы

        public override void Up()
        {
            this.AddForeignKey(nameof(Genres),
                nameof(Genres.UserId),
                nameof(User),
                nameof(User.Id));
        }
        #endregion
    }
}
