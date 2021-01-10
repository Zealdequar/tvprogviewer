using LinqToDB.Mapping;
using TVProgViewer.Core.Domain.Forums;

namespace TVProgViewer.Data.Mapping.Forums
{
    /// <summary>
    /// Represents a forum group mapping configuration
    /// </summary>
    public partial class ForumGroupMap : TvProgEntityTypeConfiguration<ForumGroup>
    {
        #region Methods

        /// <summary>
        /// Configures the entity
        /// </summary>
        /// <param name="builder">The builder to be used to configure the entity</param>
        public override void Configure(EntityMappingBuilder<ForumGroup> builder)
        {
            builder.HasTableName(TvProgMappingDefaults.ForumsGroupTable);

            builder.Property(forumGroup => forumGroup.Name).HasLength(200).IsNullable(false);
            builder.Property(forumgroup => forumgroup.DisplayOrder);
            builder.Property(forumgroup => forumgroup.CreatedOnUtc);
            builder.Property(forumgroup => forumgroup.UpdatedOnUtc);
        }

        #endregion
    }
}