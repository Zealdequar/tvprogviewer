using FluentMigrator.Builders.Create.Table;
using TvProgViewer.Core.Domain.Users;
using TvProgViewer.Data.Extensions;

namespace TvProgViewer.Data.Mapping.Builders.Users
{
    /// <summary>
    /// Представляет постоитель для данных пользователей из GreenData
    /// </summary>
    public partial class UserGreenDataInfoBuilder: TvProgEntityBuilder<UserGreenDataInfo>
    {
        #region Методы
        /// <summary>
        /// Применяет конфигурацию сущности
        /// </summary>
        /// <param name="table">Создаёт строителя табличного выражения</param>
        public override void MapEntity(CreateTableExpressionBuilder table)
        {
            table
                .WithColumn(NameCompatibilityManager.GetColumnName(typeof(UserGreenDataInfo), nameof(UserGreenDataInfo.UserId)))
                    .AsInt32().ForeignKey<User>().PrimaryKey()
                .WithColumn(NameCompatibilityManager.GetColumnName(typeof(UserGreenDataInfo), nameof(UserGreenDataInfo.OperationId)))
                    .AsInt32().ForeignKey<UserGreenDataOperations>().PrimaryKey()
                .WithColumn(NameCompatibilityManager.GetColumnName(typeof(UserGreenDataInfo), nameof(UserGreenDataInfo.LastOperationRaiseOnUtc)))
                    .AsDateTime2().Nullable();
        }

        #endregion
    }
}
