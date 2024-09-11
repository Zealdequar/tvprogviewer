namespace TvProgViewer.Core.Domain.Catalog
{
    /// <summary>
    /// Represents a tvChannel template
    /// </summary>
    public partial class TvChannelTemplate : BaseEntity
    {
        /// <summary>
        /// Gets or sets the template name
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the view path
        /// </summary>
        public string ViewPath { get; set; }

        /// <summary>
        /// Gets or sets the display order
        /// </summary>
        public int DisplayOrder { get; set; }

        /// <summary>
        /// Gets or sets a comma-separated list of tvChannel type identifiers NOT supported by this template
        /// </summary>
        public string IgnoredTvChannelTypes { get; set; }
    }
}
