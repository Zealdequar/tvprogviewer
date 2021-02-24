using FluentMigrator.Builders.Create.Table;
using TVProgViewer.Core.Domain.Users;
using TVProgViewer.Core.Domain.Orders;
using TVProgViewer.Data.Extensions;

namespace TVProgViewer.Data.Mapping.Builders.Orders
{
    /// <summary>
    /// Represents a return request entity builder
    /// </summary>
    public partial class ReturnRequestBuilder : TvProgEntityBuilder<ReturnRequest>
    {
        #region Methods

        /// <summary>
        /// Apply entity configuration
        /// </summary>
        /// <param name="table">Create table expression builder</param>
        public override void MapEntity(CreateTableExpressionBuilder table)
        {
            table
                .WithColumn(nameof(ReturnRequest.ReasonForReturn)).AsString(int.MaxValue).NotNullable()
                .WithColumn(nameof(ReturnRequest.RequestedAction)).AsString(int.MaxValue).NotNullable()
                .WithColumn(nameof(ReturnRequest.UserId)).AsInt32().ForeignKey<User>();
        }

        #endregion
    }
}