using FluentMigrator.Builders.Create.Table;
using TvProgViewer.Core.Domain.Catalog;
using TvProgViewer.Data.Extensions;

namespace TvProgViewer.Data.Mapping.Builders.Catalog
{
    /// <summary>
    /// Represents a tvchannel review helpfulness entity builder
    /// </summary>
    public partial class TvChannelReviewHelpfulnessBuilder : TvProgEntityBuilder<TvChannelReviewHelpfulness>
    {
        #region Methods

        /// <summary>
        /// Apply entity configuration
        /// </summary>
        /// <param name="table">Create table expression builder</param>
        public override void MapEntity(CreateTableExpressionBuilder table)
        {
            table
                .WithColumn(nameof(TvChannelReviewHelpfulness.TvChannelReviewId)).AsInt32().ForeignKey<TvChannelReview>();
        }

        #endregion
    }
}