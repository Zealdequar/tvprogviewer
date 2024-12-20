using TvProgViewer.Core.Configuration;

namespace TvProgViewer.Core.Domain.Common
{
    /// <summary>
    /// Display default menu item settings
    /// </summary>
    public partial class DisplayDefaultMenuItemSettings : ISettings
    {
        /// <summary>
        /// Gets or sets a value indicating whether to display "home page" menu item
        /// </summary>
        public bool DisplayHomepageMenuItem { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether to display "new tvChannels" menu item
        /// </summary>
        public bool DisplayNewTvChannelsMenuItem { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether to display "tvChannel search" menu item
        /// </summary>
        public bool DisplayTvChannelSearchMenuItem { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether to display "user info" menu item
        /// </summary>
        public bool DisplayUserInfoMenuItem { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether to display "blog" menu item
        /// </summary>
        public bool DisplayBlogMenuItem { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether to display "forums" menu item
        /// </summary>
        public bool DisplayForumsMenuItem { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether to display "contact us" menu item
        /// </summary>
        public bool DisplayContactUsMenuItem { get; set; }
    }
}