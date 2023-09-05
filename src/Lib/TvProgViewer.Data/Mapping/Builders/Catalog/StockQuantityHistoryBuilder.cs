﻿using FluentMigrator.Builders.Create.Table;
using TvProgViewer.Core.Domain.Catalog;
using TvProgViewer.Data.Extensions;

namespace TvProgViewer.Data.Mapping.Builders.Catalog
{
    /// <summary>
    /// Represents a stock quantity history entity builder
    /// </summary>
    public partial class StockQuantityHistoryBuilder : TvProgEntityBuilder<StockQuantityHistory>
    {
        #region Methods

        /// <summary>
        /// Apply entity configuration
        /// </summary>
        /// <param name="table">Create table expression builder</param>
        public override void MapEntity(CreateTableExpressionBuilder table)
        {
            table
                .WithColumn(nameof(StockQuantityHistory.ProductId)).AsInt32().ForeignKey<Product>();
        }

        #endregion
    }
}