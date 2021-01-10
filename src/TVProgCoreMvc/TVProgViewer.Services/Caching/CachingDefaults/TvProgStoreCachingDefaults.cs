using TVProgViewer.Core.Caching;

namespace TVProgViewer.Services.Caching.CachingDefaults
{
    /// <summary>
    /// Represents default values related to stores services
    /// </summary>
    public static partial class TvProgStoreCachingDefaults
    {
        #region Store mappings

        /// <summary>
        /// Gets a key for caching
        /// </summary>
        /// <remarks>
        /// {0} : entity ID
        /// {1} : entity name
        /// </remarks>
        public static CacheKey StoreMappingIdsByEntityIdNameCacheKey => new CacheKey("TVProgViewer.storemapping.ids.entityid-name-{0}-{1}");

        /// <summary>
        /// Gets a key for caching
        /// </summary>
        /// <remarks>
        /// {0} : entity ID
        /// {1} : entity name
        /// </remarks>
        public static CacheKey StoreMappingsByEntityIdNameCacheKey => new CacheKey("TVProgViewer.storemapping.entityid-name-{0}-{1}");

        #endregion

        #region Stores

        /// <summary>
        /// Gets a key for caching
        /// </summary>
        public static CacheKey StoresAllCacheKey => new CacheKey("TVProgViewer.stores.all");
        
        #endregion
    }
}