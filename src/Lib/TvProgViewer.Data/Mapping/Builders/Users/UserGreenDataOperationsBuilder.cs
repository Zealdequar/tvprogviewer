using FluentMigrator.Builders.Create.Table;
using TvProgViewer.Core.Domain.Users;
using TvProgViewer.Data.Extensions;


namespace TvProgViewer.Data.Mapping.Builders.Users
{
    public partial class UserGreenDataOperationsBuilder: TvProgEntityBuilder<UserGreenDataOperations>
    {
        #region Методы
        /// <summary>
        /// Применяет конфигурацию сущности
        /// </summary>
        /// <param name="table">Создаёт строителя табличного выражения</param>
        public override void MapEntity(CreateTableExpressionBuilder table)
        {
            
        }

        #endregion
    }
}
