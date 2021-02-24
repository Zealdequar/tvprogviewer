using FluentMigrator.Builders.Create.Table;
using TVProgViewer.Core.Domain.Orders;

namespace TVProgViewer.Data.Mapping.Builders.Orders
{
    /// <summary>
    /// Represents a return request action entity builder
    /// </summary>
    public partial class ReturnRequestActionBuilder : TvProgEntityBuilder<ReturnRequestAction>
    {
        #region Methods

        /// <summary>
        /// Apply entity configuration
        /// </summary>
        /// <param name="table">Create table expression builder</param>
        public override void MapEntity(CreateTableExpressionBuilder table)
        {
            table.WithColumn(nameof(ReturnRequestAction.Name)).AsString(400).NotNullable();
        }

        #endregion
    }
}