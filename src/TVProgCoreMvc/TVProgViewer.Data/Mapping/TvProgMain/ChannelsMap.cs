using LinqToDB.Mapping;
using System;
using System.Collections.Generic;
using System.Text;
using TVProgViewer.Core.Domain.TvProgMain;

namespace TVProgViewer.Data.Mapping.TvProgMain
{
    /// <summary>
    /// Конфигурирование телеканалов
    /// </summary>
    public partial class ChannelsMap: TvProgEntityTypeConfiguration<Channels>
    {
        #region Методы

        public override void Configure(EntityMappingBuilder<Channels> builder)
        {
            builder.HasTableName(nameof(Channels));

            builder.Property(channels => channels.TvProgProviderId).IsNullable(false);
            builder.Property(channels => channels.InternalId);
            builder.Property(channels => channels.IconId);
            builder.Property(channels => channels.CreateDate).IsNullable(false);
            builder.Property(channels => channels.TitleChannel).HasLength(300).IsNullable(false);
            builder.Property(channels => channels.IconWebSrc).HasLength(550);
            builder.Property(channels => channels.Deleted);
        }

        #endregion
    }
}
