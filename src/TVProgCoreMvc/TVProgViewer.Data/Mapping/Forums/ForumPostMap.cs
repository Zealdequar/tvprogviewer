using LinqToDB.Mapping;
using TVProgViewer.Core.Domain.Forums;

namespace TVProgViewer.Data.Mapping.Forums
{
    /// <summary>
    /// Represents a forum post mapping configuration
    /// </summary>
    public partial class ForumPostMap : TvProgEntityTypeConfiguration<ForumPost>
    {
        #region Methods

        /// <summary>
        /// Configures the entity
        /// </summary>
        /// <param name="builder">The builder to be used to configure the entity</param>
        public override void Configure(EntityMappingBuilder<ForumPost> builder)
        {
            builder.HasTableName(TvProgMappingDefaults.ForumsPostTable);

            builder.Property(post => post.Text).IsNullable(false);
            builder.Property(post => post.IPAddress).HasLength(100);
            builder.Property(forumpost => forumpost.TopicId);
            builder.Property(forumpost => forumpost.UserId);
            builder.Property(forumpost => forumpost.CreatedOnUtc);
            builder.Property(forumpost => forumpost.UpdatedOnUtc);
            builder.Property(forumpost => forumpost.VoteCount);
        }

        #endregion
    }
}