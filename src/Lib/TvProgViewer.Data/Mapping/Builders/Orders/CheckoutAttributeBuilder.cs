﻿using FluentMigrator.Builders.Create.Table;
using TvProgViewer.Core.Domain.Orders;

namespace TvProgViewer.Data.Mapping.Builders.Orders
{
    /// <summary>
    /// Represents a checkout attribute entity builder
    /// </summary>
    public partial class CheckoutAttributeBuilder : TvProgEntityBuilder<CheckoutAttribute>
    {
        #region Methods

        /// <summary>
        /// Apply entity configuration
        /// </summary>
        /// <param name="table">Create table expression builder</param>
        public override void MapEntity(CreateTableExpressionBuilder table)
        {
            table.WithColumn(nameof(CheckoutAttribute.Name)).AsString(400).NotNullable();
        }

        #endregion
    }
}