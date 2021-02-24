using FluentMigrator.Builders.Create.Table;
using TVProgViewer.Core.Domain.Users;
using TVProgViewer.Core.Domain.News;
using TVProgViewer.Core.Domain.Stores;
using TVProgViewer.Data.Extensions;

namespace TVProgViewer.Data.Mapping.Builders.News
{
    /// <summary>
    /// Represents a news comment entity builder
    /// </summary>
    public partial class NewsCommentBuilder : TvProgEntityBuilder<NewsComment>
    {
        #region Methods

        /// <summary>
        /// Apply entity configuration
        /// </summary>
        /// <param name="table">Create table expression builder</param>
        public override void MapEntity(CreateTableExpressionBuilder table)
        {
            table
                .WithColumn(nameof(NewsComment.UserId)).AsInt32().ForeignKey<User>()
                .WithColumn(nameof(NewsComment.NewsItemId)).AsInt32().ForeignKey<NewsItem>()
                .WithColumn(nameof(NewsComment.StoreId)).AsInt32().ForeignKey<Store>();
        }

        #endregion
    }
}