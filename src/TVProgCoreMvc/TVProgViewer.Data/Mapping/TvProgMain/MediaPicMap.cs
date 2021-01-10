using LinqToDB.Mapping;
using System;
using System.Collections.Generic;
using System.Text;
using TVProgViewer.Core.Domain.TvProgMain;

namespace TVProgViewer.Data.Mapping.TvProgMain
{
    /// <summary>
    /// Конфигурирование таблицы для пиктограмм
    /// </summary>
    public partial class MediaPicMap: TvProgEntityTypeConfiguration<MediaPic>
    {

        #region Методы

        public override void Configure(EntityMappingBuilder<MediaPic> builder)
        {
            builder.HasTableName(nameof(MediaPic));

            builder.Property(mediaPic => mediaPic.FileName).HasLength(256).IsNullable(false);
            builder.Property(mediaPic => mediaPic.ContentType).HasLength(256).IsNullable(false);
            builder.Property(mediaPic => mediaPic.ContentCoding).HasLength(256).IsNullable(false);
            builder.Property(mediaPic => mediaPic.Length).IsNullable(false);
            builder.Property(mediaPic => mediaPic.Length25);
            builder.Property(mediaPic => mediaPic.IsSystem).IsNullable(false);
            builder.Property(mediaPic => mediaPic.PathOrig).HasLength(256);
            builder.Property(mediaPic => mediaPic.Path25).HasLength(256);
        }

        #endregion
    }
}
