using System.Data;
using FluentMigrator.Builders.Create.Table;
using TVProgViewer.Core.Domain.Orders;
using TVProgViewer.Data.Extensions;

namespace TVProgViewer.Data.Mapping.Builders.Orders
{
    /// <summary>
    /// Represents a gift card entity builder
    /// </summary>
    public partial class GiftCardBuilder : TvProgEntityBuilder<GiftCard>
    {
        #region Methods

        /// <summary>
        /// Apply entity configuration
        /// </summary>
        /// <param name="table">Create table expression builder</param>
        public override void MapEntity(CreateTableExpressionBuilder table)
        {
            table.WithColumn(nameof(GiftCard.PurchasedWithOrderItemId)).AsInt32().Nullable().ForeignKey<OrderItem>(onDelete: Rule.None);
        }

        #endregion
    }
}