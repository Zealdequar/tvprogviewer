using LinqToDB.Mapping;
using System;
using System.Collections.Generic;
using System.Text;
using TVProgViewer.Core.Domain.TvProgMain;

namespace TVProgViewer.Data.Mapping.TvProgMain
{
    public partial class GenresMap: TvProgEntityTypeConfiguration<Genres>
    {

        #region Методы

        public override void Configure(EntityMappingBuilder<Genres> builder)
        {
            builder.HasTableName(nameof(Genres));

            builder.Property(genres => genres.UserId);
            builder.Property(genres => genres.IconId);
            builder.Property(genres => genres.CreateDate).IsNullable(false);
            builder.Property(genres => genres.GenreName).HasLength(150).IsNullable(false);
            builder.Property(genres => genres.Visible).IsNullable(false);
            builder.Property(genres => genres.DeleteDate);
        }

        #endregion 
    }
}
