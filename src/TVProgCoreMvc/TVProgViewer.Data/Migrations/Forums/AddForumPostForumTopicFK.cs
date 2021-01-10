using System.Data;
using FluentMigrator;
using TVProgViewer.Core.Domain.Forums;
using TVProgViewer.Data.Extensions;
using TVProgViewer.Data.Mapping;

namespace TVProgViewer.Data.Migrations.Forums
{
    [TvProgMigration("2019/11/19 04:40:46:3004325")]
    public class AddForumPostForumTopicFK : AutoReversingMigration
    {
        #region Methods

        public override void Up()
        {
            this.AddForeignKey(TvProgMappingDefaults.ForumsPostTable,
                nameof(ForumPost.TopicId),
                TvProgMappingDefaults.ForumsTopicTable,
                nameof(ForumTopic.Id),
                Rule.Cascade);
        }

        #endregion
    }
}