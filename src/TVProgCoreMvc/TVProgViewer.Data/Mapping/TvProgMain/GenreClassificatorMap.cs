using LinqToDB.Mapping;
using System;
using System.Collections.Generic;
using System.Text;
using TVProgViewer.Core.Domain.TvProgMain;

namespace TVProgViewer.Data.Mapping.TvProgMain
{
    /// <summary>
    ///  Конфигурирование классификатора жанров
    /// </summary>
    public partial class GenreClassificatorMap: TvProgEntityTypeConfiguration<GenreClassificator>
    {

        #region Методы

        public override void Configure(EntityMappingBuilder<GenreClassificator> builder)
        {
            builder.HasTableName(nameof(GenreClassificator));

            builder.Property(genreClassificator => genreClassificator.GenreId).IsNullable(false);
            builder.Property(genreClassificator => genreClassificator.UserId);
            builder.Property(genreClassificator => genreClassificator.ContainPhrases).HasLength(350);
            builder.Property(genreClassificator => genreClassificator.NonContainPhrases).HasLength(350);
            builder.Property(genreClassificator => genreClassificator.OrderCol);
            builder.Property(genreClassificator => genreClassificator.DeleteAfterDate);
        }

        #endregion
    }
}
