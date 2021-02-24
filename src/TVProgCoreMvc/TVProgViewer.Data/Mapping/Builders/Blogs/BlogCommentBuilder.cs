using FluentMigrator.Builders.Create.Table;
using TVProgViewer.Core.Domain.Blogs;
using TVProgViewer.Core.Domain.Users;
using TVProgViewer.Core.Domain.Stores;
using TVProgViewer.Data.Extensions;

namespace TVProgViewer.Data.Mapping.Builders.Blogs
{
    /// <summary>
    /// Represents a blog comment entity builder
    /// </summary>
    public partial class BlogCommentBuilder : TvProgEntityBuilder<BlogComment>
    {
        #region Methods

        /// <summary>
        /// Apply entity configuration
        /// </summary>
        /// <param name="table">Create table expression builder</param>
        public override void MapEntity(CreateTableExpressionBuilder table)
        {
            table
                .WithColumn(nameof(BlogComment.StoreId)).AsInt32().ForeignKey<Store>()
                .WithColumn(nameof(BlogComment.UserId)).AsInt32().ForeignKey<User>()
                .WithColumn(nameof(BlogComment.BlogPostId)).AsInt32().ForeignKey<BlogPost>();
        }

        #endregion
    }
}