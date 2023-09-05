﻿using TvProgViewer.Core.Caching;

namespace TvProgViewer.Services.Stores
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
        public static CacheKey StoreMappingIdsCacheKey => new("TvProg.storemapping.ids.{0}-{1}");

        /// <summary>
        /// Gets a key for caching
        /// </summary>
        /// <remarks>
        /// {0} : entity ID
        /// {1} : entity name
        /// </remarks>
        public static CacheKey StoreMappingsCacheKey => new("TvProg.storemapping.{0}-{1}");

        /// <summary>
        /// Gets a key for caching
        /// </summary>
        /// <remarks>
        /// {0} : entity name
        /// </remarks>
        public static CacheKey StoreMappingExistsCacheKey => new("TvProg.storemapping.exists.{0}");

        #endregion
    }
}