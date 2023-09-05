using System.Data;
using FluentMigrator.Builders.Create.Table;
using TvProgViewer.Core.Domain.Common;
using TvProgViewer.Core.Domain.Users;
using TvProgViewer.Core.Domain.Orders;
using TvProgViewer.Data.Extensions;

namespace TvProgViewer.Data.Mapping.Builders.Orders
{
    /// <summary>
    /// Represents a order entity builder
    /// </summary>
    public partial class OrderBuilder : TvProgEntityBuilder<Order>
    {
        #region Methods

        /// <summary>
        /// Apply entity configuration
        /// </summary>
        /// <param name="table">Create table expression builder</param>
        public override void MapEntity(CreateTableExpressionBuilder table)
        {
            table
                .WithColumn(nameof(Order.CustomOrderNumber)).AsString(int.MaxValue).NotNullable()
                .WithColumn(nameof(Order.BillingAddressId)).AsInt32().ForeignKey<Address>(onDelete: Rule.None)
                .WithColumn(nameof(Order.UserId)).AsInt32().ForeignKey<User>(onDelete: Rule.None)
                .WithColumn(nameof(Order.PickupAddressId)).AsInt32().Nullable().ForeignKey<Address>(onDelete: Rule.None)
                .WithColumn(nameof(Order.ShippingAddressId)).AsInt32().Nullable().ForeignKey<Address>(onDelete: Rule.None);
        }

        #endregion
    }
}