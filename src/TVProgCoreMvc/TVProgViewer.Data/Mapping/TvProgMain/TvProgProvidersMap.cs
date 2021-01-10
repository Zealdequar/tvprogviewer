using LinqToDB.Mapping;
using System;
using System.Collections.Generic;
using System.Text;
using TVProgViewer.Core.Domain.TvProgMain;

namespace TVProgViewer.Data.Mapping.TvProgMain
{
    /// <summary>
    /// Конфигурирование провайдеров телепрограммы
    /// </summary>
    public partial class TvProgProvidersMap : TvProgEntityTypeConfiguration<TvProgProviders>
    {

        #region Методы

        public override void Configure(EntityMappingBuilder<TvProgProviders> builder)
        {
            builder.HasTableName(nameof(TvProgProviders));

            builder.Property(tvProgProvider => tvProgProvider.ProviderName).HasLength(150).IsNullable(false);
            builder.Property(tvProgProvider => tvProgProvider.ProviderWebSite).HasLength(100).IsNullable(false);
            builder.Property(tvProgProvider => tvProgProvider.ContactName).HasLength(250);
            builder.Property(tvProgProvider => tvProgProvider.ContactEmail).HasLength(100);
            builder.Property(tvProgProvider => tvProgProvider.Rss).HasLength(100);
        }

        #endregion
    }
}
