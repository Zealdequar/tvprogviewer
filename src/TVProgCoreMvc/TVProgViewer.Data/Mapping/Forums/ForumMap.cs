using LinqToDB.Mapping;
using TVProgViewer.Core.Domain.Forums;

namespace TVProgViewer.Data.Mapping.Forums
{
    /// <summary>
    /// Represents a forum mapping configuration
    /// </summary>
    public partial class ForumMap : TvProgEntityTypeConfiguration<Forum>
    {
        #region Methods

        /// <summary>
        /// Configures the entity
        /// </summary>
        /// <param name="builder">The builder to be used to configure the entity</param>
        public override void Configure(EntityMappingBuilder<Forum> builder)
        {
            builder.HasTableName(TvProgMappingDefaults.ForumTable);

            builder.Property(forum => forum.Name).HasLength(200).IsNullable(false);
            builder.Property(forum => forum.ForumGroupId);
            builder.Property(forum => forum.Description);
            builder.Property(forum => forum.NumTopics);
            builder.Property(forum => forum.NumPosts);
            builder.Property(forum => forum.LastTopicId);
            builder.Property(forum => forum.LastPostId);
            builder.Property(forum => forum.LastPostUserId);
            builder.Property(forum => forum.LastPostTime);
            builder.Property(forum => forum.DisplayOrder);
            builder.Property(forum => forum.CreatedOnUtc);
            builder.Property(forum => forum.UpdatedOnUtc);
        }

        #endregion
    }
}