using LinqToDB.Mapping;
using TVProgViewer.Core.Domain.Common;

namespace TVProgViewer.Data.Mapping.Common
{
    /// <summary>
    /// Represents a generic attribute mapping configuration
    /// </summary>
    public partial class GenericAttributeMap : TvProgEntityTypeConfiguration<GenericAttribute>
    {
        #region Methods

        /// <summary>
        /// Configures the entity
        /// </summary>
        /// <param name="builder">The builder to be used to configure the entity</param>
        public override void Configure(EntityMappingBuilder<GenericAttribute> builder)
        {
            builder.HasTableName(nameof(GenericAttribute));

            builder.Property(attribute => attribute.KeyGroup).HasLength(400).IsNullable(false);
            builder.Property(attribute => attribute.Key).HasLength(400).IsNullable(false);
            builder.Property(attribute => attribute.Value).IsNullable(false);
            builder.Property(genericattribute => genericattribute.EntityId);
            builder.Property(genericattribute => genericattribute.StoreId);
        }

        #endregion
    }
}