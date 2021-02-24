using TVProgViewer.Core.Caching;
using TVProgViewer.Core.Domain.Configuration;

namespace TVProgViewer.Services.Configuration
{
    /// <summary>
    /// Represents default values related to configuration services
    /// </summary>
    public static partial class TvProgConfigurationDefaults
    {
        #region Caching defaults

        /// <summary>
        /// Gets a key for caching
        /// </summary>
        public static CacheKey SettingsAllAsDictionaryCacheKey => new CacheKey("TvProg.setting.all.dictionary.", TvProgEntityCacheDefaults<Setting>.Prefix);

        #endregion

        /// <summary>
        /// Gets the path to file that contains app settings
        /// </summary>
        public static string AppSettingsFilePath => "App_Data/appsettings.json";
    }
}