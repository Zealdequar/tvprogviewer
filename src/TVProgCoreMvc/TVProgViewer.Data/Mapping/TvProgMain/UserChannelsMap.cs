using LinqToDB.Mapping;
using System;
using System.Collections.Generic;
using System.Text;
using TVProgViewer.Core.Domain.TvProgMain;

namespace TVProgViewer.Data.Mapping.TvProgMain
{
    /// <summary>
    /// Конфигурирование пользователей связанных с каналом
    /// </summary>
    public partial class UserChannelsMap: TvProgEntityTypeConfiguration<UserChannels>
    {
        #region Методы

        public override void Configure(EntityMappingBuilder<UserChannels> builder)
        {
            builder.HasTableName(nameof(UserChannels));

            builder.Property(userChannel => userChannel.UserId).IsNullable(false);
            builder.Property(userChannel => userChannel.ChannelId).IsNullable(false);
            builder.Property(userChannel => userChannel.IconId);
            builder.Property(userChannel => userChannel.DisplayName).HasLength(300);
            builder.Property(userChannel => userChannel.OrderCol);
        }

        #endregion
    }
}
