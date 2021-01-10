using LinqToDB.Mapping;
using LinqToDB.Tools;
using System;
using System.Collections.Generic;
using System.Text;
using TVProgViewer.Core.Domain.TvProgMain;

namespace TVProgViewer.Data.Mapping.TvProgMain
{
    /// <summary>
    /// Конфигурирование веб-ресурсов источников телепрограммы
    /// </summary>
    public partial class WebResourcesMap: TvProgEntityTypeConfiguration<WebResources>
    {
        #region Методы

        public override void Configure(EntityMappingBuilder<WebResources> builder)
        {
            builder.HasTableName(nameof(WebResources));

            builder.Property(webResource => webResource.TypeProgId).IsNullable(false);
            builder.Property(webResource => webResource.FileName).HasLength(300).IsNullable(false);
            builder.Property(webResource => webResource.ResourceName).HasLength(150).IsNullable(false);
            builder.Property(webResource => webResource.ResourceUrl).HasLength(500).IsNullable(false);
        }

        #endregion
    }
}
