using LinqToDB.Mapping;
using TVProgViewer.Core.Domain.Catalog;

namespace TVProgViewer.Data.Mapping.Catalog
{
    /// <summary>
    /// Represent a review type mapping class
    /// </summary>
    public partial class ReviewTypeMap : TvProgEntityTypeConfiguration<ReviewType>
    {
        #region Methods

        /// <summary>
        /// Configures the entity
        /// </summary>
        /// <param name="builder">The builder to be used to configure the entity</param>
        public override void Configure(EntityMappingBuilder<ReviewType> builder)
        {
            builder.HasTableName(nameof(ReviewType));

            builder.Property(reviewType => reviewType.Name).HasLength(400).IsNullable(false);
            builder.Property(reviewType => reviewType.Description).HasLength(400).IsNullable(false);
            builder.Property(reviewtype => reviewtype.DisplayOrder);
            builder.Property(reviewtype => reviewtype.VisibleToAllUsers);
            builder.Property(reviewtype => reviewtype.IsRequired);
        }

        #endregion
    }
}
