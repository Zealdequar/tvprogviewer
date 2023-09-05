using TvProgViewer.Core.Caching;
using TvProgViewer.Core.Domain.Configuration;

namespace TvProgViewer.Services.Configuration
{
    /// <summary>
    /// Represents default values related to settings
    /// </summary>
    public static partial class TvProgSettingsDefaults
    {
        #region Caching defaults

        /// <summary>
        /// Gets a key for caching
        /// </summary>
        public static CacheKey SettingsAllAsDictionaryCacheKey => new("TvProg.setting.all.dictionary.", TvProgEntityCacheDefaults<Setting>.Prefix);

        #endregion
    }
}