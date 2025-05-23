﻿using FluentMigrator.Builders.Create.Table;
using TvProgViewer.Core.Domain.Catalog;
using TvProgViewer.Data.Extensions;

namespace TvProgViewer.Data.Mapping.Builders.Catalog
{
    /// <summary>
    /// Represents a tvChannel review review type mapping entity builder
    /// </summary>
    public partial class TvChannelReviewReviewTypeMappingBuilder : TvProgEntityBuilder<TvChannelReviewReviewTypeMapping>
    {
        #region Methods

        /// <summary>
        /// Apply entity configuration
        /// </summary>
        /// <param name="table">Create table expression builder</param>
        public override void MapEntity(CreateTableExpressionBuilder table)
        {
            table
                .WithColumn(nameof(TvChannelReviewReviewTypeMapping.TvChannelReviewId)).AsInt32().ForeignKey<TvChannelReview>()
                .WithColumn(nameof(TvChannelReviewReviewTypeMapping.ReviewTypeId)).AsInt32().ForeignKey<ReviewType>();
        }

        #endregion
    }
}
