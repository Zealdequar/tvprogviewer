using LinqToDB.Mapping;
using TVProgViewer.Core.Domain.Shipping;

namespace TVProgViewer.Data.Mapping.Shipping
{
    /// <summary>
    /// Represents a shipping method mapping configuration
    /// </summary>
    public partial class ShippingMethodMap : TvProgEntityTypeConfiguration<ShippingMethod>
    {
        #region Methods

        /// <summary>
        /// Configures the entity
        /// </summary>
        /// <param name="builder">The builder to be used to configure the entity</param>
        public override void Configure(EntityMappingBuilder<ShippingMethod> builder)
        {
            builder.HasTableName(nameof(ShippingMethod));

            builder.Property(method => method.Name).HasLength(400).IsNullable(false);
            builder.Property(method => method.Description);
            builder.Property(method => method.DisplayOrder);
        }

        #endregion
    }
}