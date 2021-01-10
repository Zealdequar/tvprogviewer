using FluentMigrator;
using TVProgViewer.Core.Domain.TvProgMain;
using TVProgViewer.Data.Extensions;

namespace TVProgViewer.Data.Migrations.TvProgMain
{
    [TvProgMigration("2020/06/28 23:04:49:5119940")]
    public class AddUsersProgramsGenresFK: AutoReversingMigration
    {
        #region Методы

        public override void Up()
        {
            this.AddForeignKey(nameof(UsersPrograms),
                nameof(UsersPrograms.GenreId),
                nameof(Genres),
                nameof(Genres.Id));
        }
        #endregion
    }
}
