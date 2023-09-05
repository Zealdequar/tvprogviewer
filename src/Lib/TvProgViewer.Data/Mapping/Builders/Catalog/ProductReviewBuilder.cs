﻿using FluentMigrator.Builders.Create.Table;
using TvProgViewer.Core.Domain.Catalog;
using TvProgViewer.Core.Domain.Users;
using TvProgViewer.Core.Domain.Stores;
using TvProgViewer.Data.Extensions;

namespace TvProgViewer.Data.Mapping.Builders.Catalog
{
    /// <summary>
    /// Represents a product review entity builder
    /// </summary>
    public partial class ProductReviewBuilder : TvProgEntityBuilder<ProductReview>
    {
        #region Methods

        /// <summary>
        /// Apply entity configuration
        /// </summary>
        /// <param name="table">Create table expression builder</param>
        public override void MapEntity(CreateTableExpressionBuilder table)
        {
            table
                .WithColumn(nameof(ProductReview.UserId)).AsInt32().ForeignKey<User>()
                .WithColumn(nameof(ProductReview.ProductId)).AsInt32().ForeignKey<Product>()
                .WithColumn(nameof(ProductReview.StoreId)).AsInt32().ForeignKey<Store>();
        }

        #endregion
    }
}