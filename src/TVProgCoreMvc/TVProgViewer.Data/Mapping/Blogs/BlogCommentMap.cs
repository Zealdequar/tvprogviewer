using LinqToDB.Mapping;
using TVProgViewer.Core.Domain.Blogs;

namespace TVProgViewer.Data.Mapping.Blogs
{
    /// <summary>
    /// Represents a blog comment mapping configuration
    /// </summary>
    public partial class BlogCommentMap : TvProgEntityTypeConfiguration<BlogComment>
    {
        #region Methods

        /// <summary>
        /// Configures the entity
        /// </summary>
        /// <param name="builder">The builder to be used to configure the entity</param>
        public override void Configure(EntityMappingBuilder<BlogComment> builder)
        {
            builder.HasTableName(nameof(BlogComment));

            builder.Property(blogcomment => blogcomment.UserId);
            builder.Property(blogcomment => blogcomment.CommentText);
            builder.Property(blogcomment => blogcomment.IsApproved);
            builder.Property(blogcomment => blogcomment.StoreId);
            builder.Property(blogcomment => blogcomment.BlogPostId);
            builder.Property(blogcomment => blogcomment.CreatedOnUtc);
        }

        #endregion
    }
}