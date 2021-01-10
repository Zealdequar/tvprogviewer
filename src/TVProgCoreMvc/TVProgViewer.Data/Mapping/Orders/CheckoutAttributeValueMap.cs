using LinqToDB.Mapping;
using TVProgViewer.Core.Domain.Orders;
using TVProgViewer.Data.Extensions;

namespace TVProgViewer.Data.Mapping.Orders
{
    /// <summary>
    /// Represents a checkout attribute value mapping configuration
    /// </summary>
    public partial class CheckoutAttributeValueMap : TvProgEntityTypeConfiguration<CheckoutAttributeValue>
    {
        #region Methods

        /// <summary>
        /// Configures the entity
        /// </summary>
        /// <param name="builder">The builder to be used to configure the entity</param>
        public override void Configure(EntityMappingBuilder<CheckoutAttributeValue> builder)
        {
            builder.HasTableName(nameof(CheckoutAttributeValue));

            builder.Property(value => value.Name).HasLength(400).IsNullable(false);
            builder.Property(value => value.ColorSquaresRgb).HasLength(100);
            builder.Property(value => value.PriceAdjustment).HasDecimal();
            builder.Property(value => value.WeightAdjustment).HasDecimal();
            builder.Property(value => value.CheckoutAttributeId);
            builder.Property(value => value.IsPreSelected);
            builder.Property(value => value.DisplayOrder);
        }

        #endregion
    }
}