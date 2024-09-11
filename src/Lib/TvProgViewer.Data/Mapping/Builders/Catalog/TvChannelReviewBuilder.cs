using FluentMigrator.Builders.Create.Table;
using TvProgViewer.Core.Domain.Catalog;
using TvProgViewer.Core.Domain.Users;
using TvProgViewer.Core.Domain.Stores;
using TvProgViewer.Data.Extensions;

namespace TvProgViewer.Data.Mapping.Builders.Catalog
{
    /// <summary>
    /// Represents a tvChannel review entity builder
    /// </summary>
    public partial class TvChannelReviewBuilder : TvProgEntityBuilder<TvChannelReview>
    {
        #region Methods

        /// <summary>
        /// Apply entity configuration
        /// </summary>
        /// <param name="table">Create table expression builder</param>
        public override void MapEntity(CreateTableExpressionBuilder table)
        {
            table
                .WithColumn(nameof(TvChannelReview.UserId)).AsInt32().ForeignKey<User>()
                .WithColumn(nameof(TvChannelReview.TvChannelId)).AsInt32().ForeignKey<TvChannel>()
                .WithColumn(nameof(TvChannelReview.StoreId)).AsInt32().ForeignKey<Store>();
        }

        #endregion
    }
}