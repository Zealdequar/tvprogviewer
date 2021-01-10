using TVProgViewer.Core.Caching;

namespace TVProgViewer.Services.Caching.CachingDefaults
{
    /// <summary>
    /// Represents default values related to configuration services
    /// </summary>
    public static partial class TvProgConfigurationCachingDefaults
    {
        /// <summary>
        /// Gets a key for caching
        /// </summary>
        public static CacheKey SettingsAllAsDictionaryCacheKey => new CacheKey("TVProgViewer.setting.all.as.dictionary", SettingsPrefixCacheKey);

        /// <summary>
        /// Gets a key for caching
        /// </summary>
        public static CacheKey SettingsAllCacheKey => new CacheKey("TVProgViewer.setting.all", SettingsPrefixCacheKey);

        /// <summary>
        /// Gets a key pattern to clear cache
        /// </summary>
        public static string SettingsPrefixCacheKey => "TVProgViewer.setting.";
    }
}