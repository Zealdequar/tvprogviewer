using LinqToDB.Mapping;
using System;
using System.Collections.Generic;
using System.Text;
using TVProgViewer.Core.Domain.TvProgMain;

namespace TVProgViewer.Data.Mapping.TvProgMain
{
    /// <summary>
    /// Конфигурирование телепрограммы
    /// </summary>
    public partial class ProgrammesMap: TvProgEntityTypeConfiguration<Programmes>
    {

        #region Методы

        public override void Configure(EntityMappingBuilder<Programmes> builder)
        {
            builder.HasTableName(nameof(Programmes));

            builder.Property(programmes => programmes.TypeProgId).IsNullable(false);
            builder.Property(programmes => programmes.ChannelId).IsNullable(false);
            builder.Property(programmes => programmes.InternalChanId);
            builder.Property(programmes => programmes.TsStart).IsNullable(false);
            builder.Property(programmes => programmes.TsStop).IsNullable(false);
            builder.Property(programmes => programmes.TsStartMo).IsNullable(false);
            builder.Property(programmes => programmes.TsStopMo).IsNullable(false);
            builder.Property(programmes => programmes.Title).HasLength(300).IsNullable(false);
            builder.Property(programmes => programmes.Descr).HasLength(1000);
            builder.Property(programmes => programmes.Category).HasLength(500);
        }

        #endregion
    }
}
