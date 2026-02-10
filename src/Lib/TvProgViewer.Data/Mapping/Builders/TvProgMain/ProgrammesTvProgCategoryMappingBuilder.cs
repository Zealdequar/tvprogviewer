using FluentMigrator.Builders.Create.Table;
using TvProgViewer.Core.Domain.TvProgMain;
using TvProgViewer.Data.Extensions;

namespace TvProgViewer.Data.Mapping.Builders.TvProgMain
{
    /// <summary>
    /// Представляет построитель для сущности сопоставления передач и их категорий
    /// </summary>
    public partial class ProgrammesTvProgCategoryMappingBuilder: TvProgEntityBuilder<ProgrammesTvProgCategoryMapping>
    {
        #region Методы

        /// <summary>
        /// Применяет конфигурацию сущности
        /// </summary>
        /// <param name="table">Создаёт строителя табличного выражения</param>
        public override void MapEntity(CreateTableExpressionBuilder table)
        {
            table
                .WithColumn(NameCompatibilityManager.GetColumnName(typeof(ProgrammesTvProgCategoryMapping), nameof(ProgrammesTvProgCategoryMapping.TvProgCategoryId)))
                    .AsInt32().ForeignKey<TvProgCategory>().PrimaryKey()
                .WithColumn(NameCompatibilityManager.GetColumnName(typeof(ProgrammesTvProgCategoryMapping), nameof(ProgrammesTvProgCategoryMapping.ProgrammesId)))
                    .AsInt32().ForeignKey<Programmes>().PrimaryKey();
        }

        #endregion
    }
}
