namespace TvProgViewer.Core.Domain.Blogs
{
    /// <summary>
    /// Represents a blog post tag
    /// </summary>
    public partial class BlogPostTag
    {
        /// <summary>
        /// Gets or sets the name
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the tagged tvChannel count
        /// </summary>
        public int BlogPostCount { get; set; }
    }
}