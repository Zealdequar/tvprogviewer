using LinqToDB.Mapping;
using TVProgViewer.Core.Domain.Catalog;
using TVProgViewer.Data.Extensions;

namespace TVProgViewer.Data.Mapping.Catalog
{
    /// <summary>
    /// Represents a tier price mapping configuration
    /// </summary>
    public partial class TierPriceMap : TvProgEntityTypeConfiguration<TierPrice>
    {
        #region Methods

        /// <summary>
        /// Configures the entity
        /// </summary>
        /// <param name="builder">The builder to be used to configure the entity</param>
        public override void Configure(EntityMappingBuilder<TierPrice> builder)
        {
            builder.HasTableName(nameof(TierPrice));

            builder.Property(price => price.Price).HasDecimal();
            builder.Property(price => price.ProductId);
            builder.Property(price => price.StoreId);
            builder.Property(price => price.UserRoleId);
            builder.Property(price => price.Quantity);
            builder.Property(price => price.StartDateTimeUtc);
            builder.Property(price => price.EndDateTimeUtc);
        }

        #endregion
    }
}