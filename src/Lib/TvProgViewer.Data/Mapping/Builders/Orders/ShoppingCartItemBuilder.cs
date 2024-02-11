using FluentMigrator.Builders.Create.Table;
using TvProgViewer.Core.Domain.Catalog;
using TvProgViewer.Core.Domain.Users;
using TvProgViewer.Core.Domain.Orders;
using TvProgViewer.Data.Extensions;

namespace TvProgViewer.Data.Mapping.Builders.Orders
{
    /// <summary>
    /// Represents a shopping cart item entity builder
    /// </summary>
    public partial class ShoppingCartItemBuilder : TvProgEntityBuilder<ShoppingCartItem>
    {
        #region Methods

        /// <summary>
        /// Apply entity configuration
        /// </summary>
        /// <param name="table">Create table expression builder</param>
        public override void MapEntity(CreateTableExpressionBuilder table)
        {
            table
                .WithColumn(nameof(ShoppingCartItem.UserId)).AsInt32().ForeignKey<User>()
                .WithColumn(nameof(ShoppingCartItem.TvChannelId)).AsInt32().ForeignKey<TvChannel>();
        }

        #endregion
    }
}