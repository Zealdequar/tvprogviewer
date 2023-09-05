using FluentMigrator.Builders.Create.Table;
using TvProgViewer.Core.Domain.Shipping;

namespace TvProgViewer.Data.Mapping.Builders.Shipping
{
    /// <summary>
    /// Represents a delivery date entity builder
    /// </summary>
    public partial class DeliveryDateBuilder : TvProgEntityBuilder<DeliveryDate>
    {
        #region Methods

        /// <summary>
        /// Apply entity configuration
        /// </summary>
        /// <param name="table">Create table expression builder</param>
        public override void MapEntity(CreateTableExpressionBuilder table)
        {
            table.WithColumn(nameof(DeliveryDate.Name)).AsString(400).NotNullable();
        }

        #endregion
    }
}