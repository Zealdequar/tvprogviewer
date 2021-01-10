using System.Data;
using FluentMigrator;
using TVProgViewer.Core.Domain.Forums;
using TVProgViewer.Data.Extensions;
using TVProgViewer.Data.Mapping;

namespace TVProgViewer.Data.Migrations.Forums
{
    [TvProgMigration("2019/11/19 04:46:03:3262801")]
    public class AddForumPostVoteForumPostFK : AutoReversingMigration
    {
        #region Methods

        public override void Up()
        {
            this.AddForeignKey(TvProgMappingDefaults.ForumsPostVoteTable,
                nameof(ForumPostVote.ForumPostId),
                TvProgMappingDefaults.ForumsPostTable,
                nameof(ForumPost.Id),
                Rule.Cascade);
        }

        #endregion
    }
}