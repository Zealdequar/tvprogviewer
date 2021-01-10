using LinqToDB.Mapping;
using TVProgViewer.Core.Domain.Catalog;

namespace TVProgViewer.Data.Mapping.Catalog
{
    /// <summary>
    /// Represents a product review helpfulness mapping configuration
    /// </summary>
    public partial class ProductReviewHelpfulnessMap : TvProgEntityTypeConfiguration<ProductReviewHelpfulness>
    {
        #region Methods

        /// <summary>
        /// Configures the entity
        /// </summary>
        /// <param name="builder">The builder to be used to configure the entity</param>
        public override void Configure(EntityMappingBuilder<ProductReviewHelpfulness> builder)
        {
            builder.HasTableName(nameof(ProductReviewHelpfulness));

            builder.Property(productReviewHelpfulness => productReviewHelpfulness.ProductReviewId);
            builder.Property(productReviewHelpfulness => productReviewHelpfulness.WasHelpful);
            builder.Property(productReviewHelpfulness => productReviewHelpfulness.UserId);
        }

        #endregion
    }
}