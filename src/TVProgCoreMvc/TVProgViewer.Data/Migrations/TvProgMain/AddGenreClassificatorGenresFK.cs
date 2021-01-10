using FluentMigrator;
using TVProgViewer.Core.Domain.TvProgMain;
using TVProgViewer.Data.Extensions;

namespace TVProgViewer.Data.Migrations.TvProgMain
{
    [TvProgMigration("2020/06/28 21:14:30:7618722")]
    public class AddGenreClassificatorGenresFK: AutoReversingMigration
    {
        #region Методы

        public override void Up()
        {
            this.AddForeignKey(nameof(GenreClassificator),
                nameof(GenreClassificator.GenreId),
                nameof(Genres),
                nameof(Genres.Id));
        }
        #endregion
    }
}
