using TvProgViewer.Core.Configuration;

namespace TvProgViewer.Core.Domain.Common
{
    /// <summary>
    /// Display default menu item settings
    /// </summary>
    public partial class DisplayDefaultFooterItemSettings : ISettings
    {
        /// <summary>
        /// Gets or sets a value indicating whether to display "sitemap" footer item
        /// </summary>
        public bool DisplaySitemapFooterItem { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether to display "contact us" footer item
        /// </summary>
        public bool DisplayContactUsFooterItem { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether to display "tvChannel search" footer item
        /// </summary>
        public bool DisplayTvChannelSearchFooterItem { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether to display "news" footer item
        /// </summary>
        public bool DisplayNewsFooterItem { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether to display "blog" footer item
        /// </summary>
        public bool DisplayBlogFooterItem { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether to display "forums" footer item
        /// </summary>
        public bool DisplayForumsFooterItem { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether to display "recently viewed tvChannels" footer item
        /// </summary>
        public bool DisplayRecentlyViewedTvChannelsFooterItem { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether to display "compare tvChannels" footer item
        /// </summary>
        public bool DisplayCompareTvChannelsFooterItem { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether to display "new tvChannels" footer item
        /// </summary>
        public bool DisplayNewTvChannelsFooterItem { get; set; }        

        /// <summary>
        /// Gets or sets a value indicating whether to display "user info" footer item
        /// </summary>
        public bool DisplayUserInfoFooterItem { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether to display "user orders" footer item
        /// </summary>
        public bool DisplayUserOrdersFooterItem { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether to display "user addresses" footer item
        /// </summary>
        public bool DisplayUserAddressesFooterItem { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether to display "shopping cart" footer item
        /// </summary>
        public bool DisplayShoppingCartFooterItem { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether to display "wishlist" footer item
        /// </summary>
        public bool DisplayWishlistFooterItem { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether to display "apply vendor account" footer item
        /// </summary>
        public bool DisplayApplyVendorAccountFooterItem { get; set; }
    }
}
