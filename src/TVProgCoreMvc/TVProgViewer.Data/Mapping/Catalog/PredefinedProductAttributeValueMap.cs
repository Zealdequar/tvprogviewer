using LinqToDB.Mapping;
using TVProgViewer.Core.Domain.Catalog;
using TVProgViewer.Data.Extensions;

namespace TVProgViewer.Data.Mapping.Catalog
{
    /// <summary>
    /// Represents a predefined product attribute value mapping configuration
    /// </summary>
    public partial class PredefinedProductAttributeValueMap : TvProgEntityTypeConfiguration<PredefinedProductAttributeValue>
    {
        #region Methods

        /// <summary>
        /// Configures the entity
        /// </summary>
        /// <param name="builder">The builder to be used to configure the entity</param>
        public override void Configure(EntityMappingBuilder<PredefinedProductAttributeValue> builder)
        {
            builder.HasTableName(nameof(PredefinedProductAttributeValue));

            builder.Property(value => value.Name).HasLength(400).IsNullable(false);
            builder.Property(value => value.PriceAdjustment).HasDecimal();
            builder.Property(value => value.WeightAdjustment).HasDecimal();
            builder.Property(value => value.Cost).HasDecimal();
            builder.Property(value => value.ProductAttributeId);
            builder.Property(value => value.PriceAdjustmentUsePercentage);
            builder.Property(value => value.IsPreSelected);
            builder.Property(value => value.DisplayOrder);
        }

        #endregion
    }
}