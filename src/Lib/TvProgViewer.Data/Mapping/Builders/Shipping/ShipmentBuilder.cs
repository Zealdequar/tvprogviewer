﻿using FluentMigrator.Builders.Create.Table;
using TvProgViewer.Core.Domain.Orders;
using TvProgViewer.Core.Domain.Shipping;
using TvProgViewer.Data.Extensions;

namespace TvProgViewer.Data.Mapping.Builders.Shipping
{
    /// <summary>
    /// Represents a shipment entity builder
    /// </summary>
    public partial class ShipmentBuilder : TvProgEntityBuilder<Shipment>
    {
        #region Methods

        /// <summary>
        /// Apply entity configuration
        /// </summary>
        /// <param name="table">Create table expression builder</param>
        public override void MapEntity(CreateTableExpressionBuilder table)
        {
            table.WithColumn(nameof(Shipment.OrderId)).AsInt32().ForeignKey<Order>();
        }

        #endregion
    }
}