using FluentMigrator.Builders.Create.Table;
using TVProgViewer.Core.Domain.Shipping;
using TVProgViewer.Data.Extensions;

namespace TVProgViewer.Data.Mapping.Builders.Shipping
{
    /// <summary>
    /// Represents a shipment item entity builder
    /// </summary>
    public partial class ShipmentItemBuilder : TvProgEntityBuilder<ShipmentItem>
    {
        #region Methods

        /// <summary>
        /// Apply entity configuration
        /// </summary>
        /// <param name="table">Create table expression builder</param>
        public override void MapEntity(CreateTableExpressionBuilder table)
        {
            table.WithColumn(nameof(ShipmentItem.ShipmentId)).AsInt32().ForeignKey<Shipment>();
        }

        #endregion
    }
}