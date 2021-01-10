using LinqToDB.Mapping;
using TVProgViewer.Core.Domain.Catalog;

namespace TVProgViewer.Data.Mapping.Catalog
{
    /// <summary>
    /// Represents a product tag mapping configuration
    /// </summary>
    public partial class ProductTagMap : TvProgEntityTypeConfiguration<ProductTag>
    {
        #region Methods

        /// <summary>
        /// Configures the entity
        /// </summary>
        /// <param name="builder">The builder to be used to configure the entity</param>
        public override void Configure(EntityMappingBuilder<ProductTag> builder)
        {
            builder.HasTableName(nameof(ProductTag));

            builder.Property(productTag => productTag.Name).HasLength(400).IsNullable(false);
        }

        #endregion
    }
}