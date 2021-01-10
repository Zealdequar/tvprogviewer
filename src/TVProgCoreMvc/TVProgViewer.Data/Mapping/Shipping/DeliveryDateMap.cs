using LinqToDB.Mapping;
using TVProgViewer.Core.Domain.Shipping;

namespace TVProgViewer.Data.Mapping.Shipping
{
    /// <summary>
    /// Represents a delivery date mapping configuration
    /// </summary>
    public partial class DeliveryDateMap : TvProgEntityTypeConfiguration<DeliveryDate>
    {
        #region Methods

        /// <summary>
        /// Configures the entity
        /// </summary>
        /// <param name="builder">The builder to be used to configure the entity</param>
        public override void Configure(EntityMappingBuilder<DeliveryDate> builder)
        {
            builder.HasTableName(nameof(DeliveryDate));

            builder.Property(date => date.Name).HasLength(400).IsNullable(false);
            builder.Property(date => date.DisplayOrder);
        }

        #endregion
    }
}