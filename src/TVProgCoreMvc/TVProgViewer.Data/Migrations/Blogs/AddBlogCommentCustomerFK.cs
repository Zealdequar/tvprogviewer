using System.Data;
using FluentMigrator;
using TVProgViewer.Core.Domain.Blogs;
using TVProgViewer.Core.Domain.Users;
using TVProgViewer.Data.Extensions;

namespace TVProgViewer.Data.Migrations.Blogs
{
    [TvProgMigration("2019/11/19 11:42:20:4497786")]
    public class AddBlogCommentUserFK : AutoReversingMigration
    {
        #region Methods

        public override void Up()
        {
           
        }

        #endregion
    }
}