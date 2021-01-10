using FluentMigrator;
using TVProgViewer.Core.Domain.Forums;
using TVProgViewer.Data.Extensions;
using TVProgViewer.Data.Mapping;

namespace TVProgViewer.Data.Migrations.Indexes
{
    [TvProgMigration("2019/12/19 09:36:08:9037699")]
    public class AddForumsForumDisplayOrderIX : AutoReversingMigration
    {
        #region Methods          

        public override void Up()
        {
            this.AddIndex("IX_Forums_Forum_DisplayOrder", TvProgMappingDefaults.ForumTable, i => i.Ascending(),
                nameof(Forum.DisplayOrder));
        }

        #endregion
    }
}