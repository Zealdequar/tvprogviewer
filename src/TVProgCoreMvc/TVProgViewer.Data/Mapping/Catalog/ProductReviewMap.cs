using LinqToDB.Mapping;
using TVProgViewer.Core.Domain.Catalog;

namespace TVProgViewer.Data.Mapping.Catalog
{
    /// <summary>
    /// Represents a product review mapping configuration
    /// </summary>
    public partial class ProductReviewMap : TvProgEntityTypeConfiguration<ProductReview>
    {
        #region Methods

        /// <summary>
        /// Configures the entity
        /// </summary>
        /// <param name="builder">The builder to be used to configure the entity</param>
        public override void Configure(EntityMappingBuilder<ProductReview> builder)
        {
            builder.HasTableName(nameof(ProductReview));

            builder.Property(productreview => productreview.UserId);
            builder.Property(productreview => productreview.ProductId);
            builder.Property(productreview => productreview.StoreId);
            builder.Property(productreview => productreview.IsApproved);
            builder.Property(productreview => productreview.Title);
            builder.Property(productreview => productreview.ReviewText);
            builder.Property(productreview => productreview.ReplyText);
            builder.Property(productreview => productreview.UserNotifiedOfReply);
            builder.Property(productreview => productreview.Rating);
            builder.Property(productreview => productreview.HelpfulYesTotal);
            builder.Property(productreview => productreview.HelpfulNoTotal);
            builder.Property(productreview => productreview.CreatedOnUtc);
        }

        #endregion
    }
}