using LinqToDB.Mapping;
using TVProgViewer.Core.Domain.Catalog;

namespace TVProgViewer.Data.Mapping.Catalog
{
    /// <summary>
    /// Represents a back in stock subscription mapping configuration
    /// </summary>
    public partial class BackInStockSubscriptionMap : TvProgEntityTypeConfiguration<BackInStockSubscription>
    {
        #region Methods

        /// <summary>
        /// Configures the entity
        /// </summary>
        /// <param name="builder">The builder to be used to configure the entity</param>
        public override void Configure(EntityMappingBuilder<BackInStockSubscription> builder)
        {
            builder.HasTableName(nameof(BackInStockSubscription));

            builder.Property(backinstocksubscription => backinstocksubscription.StoreId);
            builder.Property(backinstocksubscription => backinstocksubscription.ProductId);
            builder.Property(backinstocksubscription => backinstocksubscription.UserId);
            builder.Property(backinstocksubscription => backinstocksubscription.CreatedOnUtc);
        }

        #endregion
    }
}