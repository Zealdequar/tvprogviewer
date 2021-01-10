using LinqToDB.Mapping;
using TVProgViewer.Core.Domain.Forums;

namespace TVProgViewer.Data.Mapping.Forums
{
    /// <summary>
    /// Represents a forum post vote mapping configuration
    /// </summary>
    public partial class ForumPostVoteMap : TvProgEntityTypeConfiguration<ForumPostVote>
    {
        #region Methods

        /// <summary>
        /// Configures the entity
        /// </summary>
        /// <param name="builder">The builder to be used to configure the entity</param>
        public override void Configure(EntityMappingBuilder<ForumPostVote> builder)
        {
            builder.HasTableName(TvProgMappingDefaults.ForumsPostVoteTable);

            builder.Property(forumpostvote => forumpostvote.ForumPostId);
            builder.Property(forumpostvote => forumpostvote.UserId);
            builder.Property(forumpostvote => forumpostvote.IsUp);
            builder.Property(forumpostvote => forumpostvote.CreatedOnUtc);
        }

        #endregion
    }
}