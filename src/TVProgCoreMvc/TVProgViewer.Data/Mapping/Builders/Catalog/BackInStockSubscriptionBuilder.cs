using FluentMigrator.Builders.Create.Table;
using TVProgViewer.Core.Domain.Catalog;
using TVProgViewer.Core.Domain.Users;
using TVProgViewer.Data.Extensions;

namespace TVProgViewer.Data.Mapping.Builders.Catalog
{
    /// <summary>
    /// Represents a back in stock subscription entity builder
    /// </summary>
    public partial class BackInStockSubscriptionBuilder : TvProgEntityBuilder<BackInStockSubscription>
    {
        #region Methods

        /// <summary>
        /// Apply entity configuration
        /// </summary>
        /// <param name="table">Create table expression builder</param>
        public override void MapEntity(CreateTableExpressionBuilder table)
        {
            table
                .WithColumn(nameof(BackInStockSubscription.UserId)).AsInt32().ForeignKey<User>()
                .WithColumn(nameof(BackInStockSubscription.ProductId)).AsInt32().ForeignKey<Product>();
        }

        #endregion
    }
}