using LinqToDB.Mapping;
using TVProgViewer.Core.Domain.Stores;

namespace TVProgViewer.Data.Mapping.Stores
{
    /// <summary>
    /// Represents a store mapping mapping configuration
    /// </summary>
    public partial class StoreMappingMap : TvProgEntityTypeConfiguration<StoreMapping>
    {
        #region Methods

        /// <summary>
        /// Configures the entity
        /// </summary>
        /// <param name="builder">The builder to be used to configure the entity</param>
        public override void Configure(EntityMappingBuilder<StoreMapping> builder)
        {
            builder.HasTableName(nameof(StoreMapping));

            builder.Property(storeMapping => storeMapping.EntityName).HasLength(400).IsNullable(false);
            builder.Property(storemapping => storemapping.EntityId);
            builder.Property(storemapping => storemapping.StoreId);
        }

        #endregion
    }
}