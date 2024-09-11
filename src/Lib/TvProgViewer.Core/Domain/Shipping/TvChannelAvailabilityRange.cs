using TvProgViewer.Core.Domain.Localization;

namespace TvProgViewer.Core.Domain.Shipping
{
    /// <summary>
    /// Represents a tvChannel availability range
    /// </summary>
    public partial class TvChannelAvailabilityRange : BaseEntity, ILocalizedEntity
    {
        /// <summary>
        /// Gets or sets the name
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the display order
        /// </summary>
        public int DisplayOrder { get; set; }
    }
}