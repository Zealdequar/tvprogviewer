using System.Data;
using FluentMigrator;
using TVProgViewer.Core.Domain.Blogs;
using TVProgViewer.Core.Domain.Stores;
using TVProgViewer.Data.Extensions;

namespace TVProgViewer.Data.Migrations.Blogs
{
    [TvProgMigration("2019/11/19 11:42:20:4497787")]
    public class AddBlogCommentStoreFK : AutoReversingMigration
    {
        #region Methods

        public override void Up()
        {
            this.AddForeignKey(nameof(BlogComment), 
                nameof(BlogComment.StoreId), 
                nameof(Store), 
                nameof(Store.Id), 
                Rule.Cascade);
        }

        #endregion
    }
}