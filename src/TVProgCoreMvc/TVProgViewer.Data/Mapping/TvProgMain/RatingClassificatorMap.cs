using LinqToDB.Mapping;
using System;
using System.Collections.Generic;
using System.Text;
using TVProgViewer.Core.Domain.TvProgMain;

namespace TVProgViewer.Data.Mapping.TvProgMain
{
    /// <summary>
    ///  Конфигурирование классификатора рейтингов
    /// </summary>
    public partial class RatingClassificatorMap : TvProgEntityTypeConfiguration<RatingClassificator>
    {

        #region Методы

        public override void Configure(EntityMappingBuilder<RatingClassificator> builder)
        {
            builder.HasTableName(nameof(RatingClassificator));

            builder.Property(ratingClassificator => ratingClassificator.RatingId).IsNullable(false);
            builder.Property(ratingClassificator => ratingClassificator.UserId);
            builder.Property(ratingClassificator => ratingClassificator.ContainPhrases).HasLength(350);
            builder.Property(ratingClassificator => ratingClassificator.NonContainPhrases).HasLength(350);
            builder.Property(ratingClassificator => ratingClassificator.OrderCol);
            builder.Property(ratingClassificator => ratingClassificator.DeleteAfterDate);
        }

        #endregion
    }
}
