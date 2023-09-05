using TvProgViewer.Core.Configuration;

namespace TvProgViewer.Core.Security
{
    public partial class CookieSettings : ISettings
    {
        /// <summary>
        /// Expiration time on hours for the "Compare products" cookie
        /// </summary>
        public int CompareProductsCookieExpires { get; set; }

        /// <summary>
        /// Expiration time on hours for the "Recently viewed products" cookie
        /// </summary>
        public int RecentlyViewedProductsCookieExpires { get; set; }

        /// <summary>
        /// Expiration time on hours for the "User" cookie
        /// </summary>
        public int UserCookieExpires { get; set; }
    }
}
