using TVProgViewer.Core.Caching;

namespace TVProgViewer.Services.Stores
{
    /// <summary>
    /// Represents default values related to stores services
    /// </summary>
    public static partial class TvProgStoreDefaults
    {
        #region Caching defaults

        /// <summary>
        /// Gets a key for caching
        /// </summary>
        /// <remarks>
        /// {0} : entity ID
        /// {1} : entity name
        /// </remarks>
        public static CacheKey StoreMappingIdsCacheKey => new CacheKey("TvProg.storemapping.ids.{0}-{1}");

        /// <summary>
        /// Gets a key for caching
        /// </summary>
        /// <remarks>
        /// {0} : entity ID
        /// {1} : entity name
        /// </remarks>
        public static CacheKey StoreMappingsCacheKey => new CacheKey("TvProg.storemapping.{0}-{1}");

        /// <summary>
        /// Gets a key for caching
        /// </summary>
        /// <remarks>
        /// {0} : store ID
        /// {1} : entity name
        /// </remarks>
        public static CacheKey StoreMappingExistsCacheKey => new CacheKey("TvProg.storemapping.exists.{0}-{1}");

        #endregion
    }
}