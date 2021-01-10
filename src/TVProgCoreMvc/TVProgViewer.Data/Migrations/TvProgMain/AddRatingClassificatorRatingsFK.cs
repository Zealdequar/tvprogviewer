
using FluentMigrator;
using TVProgViewer.Core.Domain.TvProgMain;
using TVProgViewer.Data.Extensions;

namespace TVProgViewer.Data.Migrations.TvProgMain
{
    [TvProgMigration("2020/06/28 21:36:15:8730898")]
    public class AddRatingClassificatorRatingsFK: AutoReversingMigration
    {
        #region Методы 

        public override void Up()
        {
            this.AddForeignKey(nameof(RatingClassificator),
                nameof(RatingClassificator.RatingId),
                nameof(Ratings),
                nameof(Ratings.Id));
        }
        #endregion
    }
}
