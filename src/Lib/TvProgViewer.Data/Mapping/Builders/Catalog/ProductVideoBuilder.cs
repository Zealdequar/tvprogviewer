﻿using FluentMigrator.Builders.Create.Table;
using TvProgViewer.Core.Domain.Catalog;
using TvProgViewer.Core.Domain.Media;
using TvProgViewer.Data.Extensions;

namespace TvProgViewer.Data.Mapping.Builders.Catalog
{
    /// <summary>
    /// Represents a product video mapping entity builder
    /// </summary>
    public partial class ProductVideoBuilder : TvProgEntityBuilder<ProductVideo>
    {
        #region Methods

        /// <summary>
        /// Apply entity configuration
        /// </summary>
        /// <param name="table">Create table expression builder</param>
        public override void MapEntity(CreateTableExpressionBuilder table)
        {
            table
                .WithColumn(nameof(ProductVideo.VideoId)).AsInt32().ForeignKey<Video>()
                .WithColumn(nameof(ProductVideo.ProductId)).AsInt32().ForeignKey<Product>();
        }

        #endregion
    }
}
