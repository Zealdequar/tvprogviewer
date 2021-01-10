
using FluentMigrator;
using TVProgViewer.Core.Domain.TvProgMain;
using TVProgViewer.Core.Domain.Users;
using TVProgViewer.Data.Extensions;

namespace TVProgViewer.Data.Migrations.TvProgMain
{
    [TvProgMigration("2020/06/28 21:18:51:5764615")]
    public class AddGenreClassificatorUsersFK: AutoReversingMigration
    {
        #region Методы

        public override void Up()
        {
            this.AddForeignKey(nameof(GenreClassificator),
                nameof(GenreClassificator.UserId),
                nameof(User),
                nameof(User.Id));
        }
        #endregion
    }
}
