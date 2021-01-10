using LinqToDB.Mapping;
using System;
using System.Collections.Generic;
using System.Text;
using TVProgViewer.Core.Domain.TvProgMain;

namespace TVProgViewer.Data.Mapping.TvProgMain
{
    /// <summary>
    /// Конфигурирование дополнительных пользовательских настроек
    /// </summary>
    public partial class ExtUserSettingsMap: TvProgEntityTypeConfiguration<ExtUserSettings>
    {

        #region Методы

        public override void Configure(EntityMappingBuilder<ExtUserSettings> builder)
        {
            builder.HasTableName(nameof(ExtUserSettings));

            builder.Property(extUserSettings => extUserSettings.TvProgProviderId).IsNullable(false);
            builder.Property(extUserSettings => extUserSettings.UncheckedChannels);
        }

        #endregion
    }
}
