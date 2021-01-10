using System.Data;
using FluentMigrator;
using TVProgViewer.Core.Domain.Forums;
using TVProgViewer.Data.Extensions;
using TVProgViewer.Data.Mapping;

namespace TVProgViewer.Data.Migrations.Forums
{
    [TvProgMigration("2019/11/19 04:48:30:1910240")]
    public class AddForumTopicForumFK : AutoReversingMigration
    {
        #region Methods

        public override void Up()
        {
            this.AddForeignKey(TvProgMappingDefaults.ForumsTopicTable,
                nameof(ForumTopic.ForumId),
                TvProgMappingDefaults.ForumTable,
                nameof(Forum.Id),
                Rule.Cascade);
        }

        #endregion
    }
}