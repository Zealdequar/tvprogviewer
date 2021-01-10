using FluentMigrator;
using TVProgViewer.Core.Domain.Forums;
using TVProgViewer.Data.Extensions;
using TVProgViewer.Data.Mapping;

namespace TVProgViewer.Data.Migrations.Indexes
{
    [TvProgMigration("2019/12/19 09:36:08:9037698")]
    public class AddForumsGroupDisplayOrderIX : AutoReversingMigration
    {
        #region Methods          

        public override void Up()
        {
            this.AddIndex("IX_Forums_Group_DisplayOrder", TvProgMappingDefaults.ForumsGroupTable, i => i.Ascending(),
                nameof(ForumGroup.DisplayOrder));
        }

        #endregion
    }
}