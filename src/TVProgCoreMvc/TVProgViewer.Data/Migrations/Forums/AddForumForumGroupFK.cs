using System.Data;
using FluentMigrator;
using TVProgViewer.Core.Domain.Forums;
using TVProgViewer.Data.Extensions;
using TVProgViewer.Data.Mapping;

namespace TVProgViewer.Data.Migrations.Forums
{
    [TvProgMigration("2019/11/19 04:39:22:7313370")]
    public class AddForumForumGroupFK : AutoReversingMigration
    {
        #region Methods

        public override void Up()
        {
            this.AddForeignKey(TvProgMappingDefaults.ForumTable,
                nameof(Forum.ForumGroupId),
                TvProgMappingDefaults.ForumsGroupTable,
                nameof(ForumGroup.Id),
                Rule.Cascade);
        }

        #endregion
    }
}