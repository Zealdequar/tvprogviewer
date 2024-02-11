using TvProgViewer.Core.Domain.Localization;
using TvProgViewer.Core.Domain.Seo;

namespace TvProgViewer.Core.Domain.Catalog
{
    /// <summary>
    /// Represents a tvchannel tag
    /// </summary>
    public partial class TvChannelTag : BaseEntity, ILocalizedEntity, ISlugSupported
    {
        /// <summary>
        /// Gets or sets the name
        /// </summary>
        public string Name { get; set; }
    }
}