using LinqToDB.Mapping;
using TVProgViewer.Core.Domain.Catalog;

namespace TVProgViewer.Data.Mapping.Catalog
{
    /// <summary>
    /// Represents a specification attribute mapping configuration
    /// </summary>
    public partial class SpecificationAttributeMap : TvProgEntityTypeConfiguration<SpecificationAttribute>
    {
        #region Methods

        /// <summary>
        /// Configures the entity
        /// </summary>
        /// <param name="builder">The builder to be used to configure the entity</param>
        public override void Configure(EntityMappingBuilder<SpecificationAttribute> builder)
        {
            builder.HasTableName(nameof(SpecificationAttribute));

            builder.Property(attribute => attribute.Name).IsNullable(false);
            builder.Property(attribute => attribute.DisplayOrder);
        }

        #endregion
    }
}