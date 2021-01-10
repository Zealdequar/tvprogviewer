using LinqToDB.Mapping;
using TVProgViewer.Core.Domain.Shipping;
using TVProgViewer.Data.Extensions;

namespace TVProgViewer.Data.Mapping.Shipping
{
    /// <summary>
    /// Represents a shipment mapping configuration
    /// </summary>
    public partial class ShipmentMap : TvProgEntityTypeConfiguration<Shipment>
    {
        #region Methods

        /// <summary>
        /// Configures the entity
        /// </summary>
        /// <param name="builder">The builder to be used to configure the entity</param>
        public override void Configure(EntityMappingBuilder<Shipment> builder)
        {
            builder.HasTableName(nameof(Shipment));

            builder.Property(shipment => shipment.TotalWeight.Value).HasDecimal();

            builder.Property(shipment => shipment.OrderId);
            builder.Property(shipment => shipment.TrackingNumber);
            builder.Property(shipment => shipment.ShippedDateUtc);
            builder.Property(shipment => shipment.DeliveryDateUtc);
            builder.Property(shipment => shipment.AdminComment);
            builder.Property(shipment => shipment.CreatedOnUtc);
        }

        #endregion
    }
}