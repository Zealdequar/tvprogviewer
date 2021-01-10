using LinqToDB.Mapping;
using TVProgViewer.Core.Domain.Discounts;

namespace TVProgViewer.Data.Mapping.Discounts
{
    /// <summary>
    /// Represents a discount-category mapping configuration
    /// </summary>
    public partial class DiscountCategoryMap : TvProgEntityTypeConfiguration<DiscountCategoryMapping>
    {
        #region Methods

        /// <summary>
        /// Configures the entity
        /// </summary>
        /// <param name="builder">The builder to be used to configure the entity</param>
        public override void Configure(EntityMappingBuilder<DiscountCategoryMapping> builder)
        {
            builder.HasTableName(TvProgMappingDefaults.DiscountAppliedToCategoriesTable);
            builder.HasPrimaryKey(mapping => new { mapping.DiscountId, mapping.EntityId });

            builder.Property(mapping => mapping.DiscountId).HasColumnName("Discount_Id");
            builder.Property(mapping => mapping.EntityId).HasColumnName("Category_Id");

            builder.Ignore(mapping => mapping.Id);
        }

        #endregion
    }
}