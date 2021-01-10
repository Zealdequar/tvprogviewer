using LinqToDB.Mapping;
using System;
using System.Collections.Generic;
using System.Text;
using TVProgViewer.Core.Domain.TvProgMain;

namespace TVProgViewer.Data.Mapping.TvProgMain
{
    /// <summary>
    /// Конфигурирование настроек поиска
    /// </summary>
    public partial class SearchSettingsMap: TvProgEntityTypeConfiguration<SearchSettings>
    {

        #region Методы

        public override void Configure(EntityMappingBuilder<SearchSettings> builder)
        {
            builder.HasTableName(nameof(SearchSettings));

            builder.Property(searchSetting => searchSetting.UserId).IsNullable(false);
            builder.Property(searchSetting => searchSetting.LoadSettings).IsNullable(false);
            builder.Property(searchSetting => searchSetting.Match).HasLength(1000);
            builder.Property(searchSetting => searchSetting.NotMatch).HasLength(1000);
            builder.Property(searchSetting => searchSetting.InAnons);
            builder.Property(searchSetting => searchSetting.TsFinalFrom);
            builder.Property(searchSetting => searchSetting.TsFinalTo);
            builder.Property(searchSetting => searchSetting.TrackBarFrom);
            builder.Property(searchSetting => searchSetting.TrackBarTo);
        }

        #endregion
    }
}
