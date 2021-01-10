using LinqToDB.Mapping;
using TVProgViewer.Core.Domain.Discounts;

namespace TVProgViewer.Data.Mapping.Discounts
{
    /// <summary>
    /// Represents a discount-manufacturer mapping configuration
    /// </summary>
    public partial class DiscountManufacturerMap : TvProgEntityTypeConfiguration<DiscountManufacturerMapping>
    {
        #region Methods

        /// <summary>
        /// Configures the entity
        /// </summary>
        /// <param name="builder">The builder to be used to configure the entity</param>
        public override void Configure(EntityMappingBuilder<DiscountManufacturerMapping> builder)
        {
            builder.HasTableName(TvProgMappingDefaults.DiscountAppliedToManufacturersTable);
            builder.HasPrimaryKey(mapping => new { mapping.DiscountId, mapping.EntityId });

            builder.Property(mapping => mapping.DiscountId).HasColumnName("Discount_Id");
            builder.Property(mapping => mapping.EntityId).HasColumnName("Manufacturer_Id");

            builder.Ignore(mapping => mapping.Id);
        }

        #endregion
    }
}