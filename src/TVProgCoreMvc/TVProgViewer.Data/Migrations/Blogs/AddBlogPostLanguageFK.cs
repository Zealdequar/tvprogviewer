using System.Data;
using FluentMigrator;
using TVProgViewer.Core.Domain.Blogs;
using TVProgViewer.Core.Domain.Localization;
using TVProgViewer.Data.Extensions;

namespace TVProgViewer.Data.Migrations.Blogs
{
    [TvProgMigration("2019/11/19 11:45:59:5956342")]
    public class AddBlogPostLanguageFK : AutoReversingMigration
    {
        #region Methods

        public override void Up()
        {
            this.AddForeignKey(nameof(BlogPost), 
                nameof(BlogPost.LanguageId), 
                nameof(Language),
                nameof(Language.Id), 
                Rule.Cascade);
        }

        #endregion
    }
}