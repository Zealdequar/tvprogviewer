using System.Data;
using FluentMigrator;
using TVProgViewer.Core.Domain.Blogs;
using TVProgViewer.Data.Extensions;

namespace TVProgViewer.Data.Migrations.Blogs
{
    [TvProgMigration("2019/11/19 11:42:20:4497785")]
    public class AddBlogCommentBlogPostFK : AutoReversingMigration
    {
        #region Methods

        public override void Up()
        {
            this.AddForeignKey(nameof(BlogComment), 
                nameof(BlogComment.BlogPostId), 
                nameof(BlogPost),
                nameof(BlogPost.Id), Rule.Cascade);
        }

        #endregion
    }
}