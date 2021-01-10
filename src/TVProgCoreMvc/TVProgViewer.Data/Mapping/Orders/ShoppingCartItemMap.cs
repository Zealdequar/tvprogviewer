using LinqToDB;
using LinqToDB.Mapping;
using TVProgViewer.Core.Domain.Orders;
using TVProgViewer.Data.Extensions;

namespace TVProgViewer.Data.Mapping.Orders
{
    /// <summary>
    /// Represents a shopping cart item mapping configuration
    /// </summary>
    public partial class ShoppingCartItemMap : TvProgEntityTypeConfiguration<ShoppingCartItem>
    {
        #region Methods

        /// <summary>
        /// Configures the entity
        /// </summary>
        /// <param name="builder">The builder to be used to configure the entity</param>
        public override void Configure(EntityMappingBuilder<ShoppingCartItem> builder)
        {
            builder.HasTableName(nameof(ShoppingCartItem));

            builder.Property(item => item.UserEnteredPrice).HasDecimal();

            builder.Property(item => item.StoreId);
            builder.Property(item => item.ShoppingCartTypeId);
            builder.Property(item => item.UserId);
            builder.Property(item => item.ProductId);
            builder.Property(item => item.AttributesXml);
            builder.Property(item => item.Quantity);
            builder.Property(item => item.RentalStartDateUtc);
            builder.Property(item => item.RentalEndDateUtc);
            builder.Property(item => item.CreatedOnUtc);
            builder.Property(item => item.UpdatedOnUtc);

            builder.Ignore(item => item.ShoppingCartType);
        }

        #endregion
    }
}