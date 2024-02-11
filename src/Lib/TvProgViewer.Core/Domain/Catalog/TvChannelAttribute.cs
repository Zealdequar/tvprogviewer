using TvProgViewer.Core.Domain.Localization;

namespace TvProgViewer.Core.Domain.Catalog
{
    /// <summary>
    /// Represents a tvchannel attribute
    /// </summary>
    public partial class TvChannelAttribute : BaseEntity, ILocalizedEntity
    {
        /// <summary>
        /// Gets or sets the name
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the description
        /// </summary>
        public string Description { get; set; }
    }
}
