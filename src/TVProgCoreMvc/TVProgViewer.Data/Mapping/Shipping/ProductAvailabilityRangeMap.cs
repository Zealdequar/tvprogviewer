using LinqToDB.Mapping;
using TVProgViewer.Core.Domain.Shipping;

namespace TVProgViewer.Data.Mapping.Shipping
{
    /// <summary>
    /// Represents a product availability range mapping configuration
    /// </summary>
    public partial class ProductAvailabilityRangeMap : TvProgEntityTypeConfiguration<ProductAvailabilityRange>
    {
        #region Methods

        /// <summary>
        /// Configures the entity
        /// </summary>
        /// <param name="builder">The builder to be used to configure the entity</param>
        public override void Configure(EntityMappingBuilder<ProductAvailabilityRange> builder)
        {
            builder.HasTableName(nameof(ProductAvailabilityRange));

            builder.Property(range => range.Name).HasLength(400).IsNullable(false);
            builder.Property(range => range.DisplayOrder);
        }

        #endregion
    }
}