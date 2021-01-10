using LinqToDB.Mapping;
using System;
using System.Collections.Generic;
using System.Text;
using TVProgViewer.Core.Domain.TvProgMain;

namespace TVProgViewer.Data.Mapping.TvProgMain
{
    /// <summary>
    /// Конфигурирование рейтингов
    /// </summary>
    public partial class RatingsMap: TvProgEntityTypeConfiguration<Ratings>
    {

        #region Методы

        public override void Configure(EntityMappingBuilder<Ratings> builder)
        {
            builder.HasTableName(nameof(Ratings));

            builder.Property(rating => rating.UserId).IsNullable(false);
            builder.Property(rating => rating.IconId);
            builder.Property(rating => rating.CreateDate).IsNullable(false);
            builder.Property(rating => rating.RatingName).HasLength(150).IsNullable(false);
            builder.Property(rating => rating.Visible).IsNullable(false);
            builder.Property(rating => rating.DeleteDate);
        }

        #endregion
    }
}
