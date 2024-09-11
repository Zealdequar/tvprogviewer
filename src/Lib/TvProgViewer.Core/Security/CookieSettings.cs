using TvProgViewer.Core.Configuration;

namespace TvProgViewer.Core.Security
{
    public partial class CookieSettings : ISettings
    {
        /// <summary>
        /// Expiration time on hours for the "Compare tvChannels" cookie
        /// </summary>
        public int CompareTvChannelsCookieExpires { get; set; }

        /// <summary>
        /// Expiration time on hours for the "Recently viewed tvChannels" cookie
        /// </summary>
        public int RecentlyViewedTvChannelsCookieExpires { get; set; }

        /// <summary>
        /// Expiration time on hours for the "User" cookie
        /// </summary>
        public int UserCookieExpires { get; set; }
    }
}
