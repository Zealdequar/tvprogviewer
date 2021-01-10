using LinqToDB.Mapping;
using TVProgViewer.Core.Domain.Shipping;

namespace TVProgViewer.Data.Mapping.Shipping
{
    /// <summary>
    /// Represents a shipment item mapping configuration
    /// </summary>
    public partial class ShipmentItemMap : TvProgEntityTypeConfiguration<ShipmentItem>
    {
        #region Methods

        /// <summary>
        /// Configures the entity
        /// </summary>
        /// <param name="builder">The builder to be used to configure the entity</param>
        public override void Configure(EntityMappingBuilder<ShipmentItem> builder)
        {
            builder.HasTableName(nameof(ShipmentItem));

            builder.Property(item => item.ShipmentId);
            builder.Property(item => item.OrderItemId);
            builder.Property(item => item.Quantity);
            builder.Property(item => item.WarehouseId);
        }

        #endregion
    }
}