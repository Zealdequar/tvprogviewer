using LinqToDB.Mapping;
using System;
using System.Collections.Generic;
using System.Text;
using TVProgViewer.Core.Domain.TvProgMain;

namespace TVProgViewer.Data.Mapping.TvProgMain
{
    /// <summary>
    /// Конфигурирование лога обновлений телеканалов и телепередач
    /// </summary>
    public partial class UpdateProgLogMap: TvProgEntityTypeConfiguration<UpdateProgLog>
    {
        #region Методы

        public override void Configure(EntityMappingBuilder<UpdateProgLog> builder)
        {
            builder.HasTableName(nameof(UpdateProgLog));

            builder.Property(updateProgLog => updateProgLog.WebResourceId).IsNullable(false);
            builder.Property(updateProgLog => updateProgLog.TsUpdateStart).IsNullable(false);
            builder.Property(updateProgLog => updateProgLog.TsUpdateEnd).IsNullable(false);
            builder.Property(updateProgLog => updateProgLog.UdtElapsedSec);
            builder.Property(updateProgLog => updateProgLog.MinProgDate).IsNullable(false);
            builder.Property(updateProgLog => updateProgLog.MaxProgDate).IsNullable(false);
            builder.Property(updateProgLog => updateProgLog.QtyChans).IsNullable(false);
            builder.Property(updateProgLog => updateProgLog.QtyProgrammes).IsNullable(false);
            builder.Property(updateProgLog => updateProgLog.QtyNewChans).IsNullable(false);
            builder.Property(updateProgLog => updateProgLog.QtyNewProgrammes).IsNullable(false);
            builder.Property(updateProgLog => updateProgLog.IsSuccess).IsNullable(false);
            builder.Property(updateProgLog => updateProgLog.ErrorMessage).HasLength(1000);
        }

        #endregion
    }
}
