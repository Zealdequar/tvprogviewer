using LinqToDB.Mapping;
using System;
using System.Collections.Generic;
using System.Text;
using TVProgViewer.Core.Domain.TvProgMain;

namespace TVProgViewer.Data.Mapping.TvProgMain
{
    /// <summary>
    /// Конфигурирование типов программ
    /// </summary>
    public partial class TypeProgMap: TvProgEntityTypeConfiguration<TypeProg>
    {
        #region Методы

        public override void Configure(EntityMappingBuilder<TypeProg> builder)
        {
            builder.HasTableName(nameof(TypeProg));

            builder.Property(typeProg => typeProg.TvProgProviderId).IsNullable(false);
            builder.Property(typeProg => typeProg.TypeName).HasLength(150).IsNullable(false);
            builder.Property(typeProg => typeProg.FileFormat).HasLength(5);
        }

        #endregion
    }
}
