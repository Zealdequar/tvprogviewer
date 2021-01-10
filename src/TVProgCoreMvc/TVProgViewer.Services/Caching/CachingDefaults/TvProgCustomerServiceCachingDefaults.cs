using TVProgViewer.Core.Caching;

namespace TVProgViewer.Services.Caching.CachingDefaults
{
    /// <summary>
    /// Represents default values related to user services
    /// </summary>
    public static partial class TvProgUserServiceCachingDefaults
    {
        #region User attributes

        /// <summary>
        /// Gets a key for caching
        /// </summary>
        public static CacheKey UserAttributesAllCacheKey => new CacheKey("TVProgViewer.userattribute.all");

        /// <summary>
        /// Gets a key for caching
        /// </summary>
        /// <remarks>
        /// {0} : user attribute ID
        /// </remarks>
        public static CacheKey UserAttributeValuesAllCacheKey => new CacheKey("TVProgViewer.userattributevalue.all-{0}");
        
        #endregion

        #region User roles

        /// <summary>
        /// Gets a key for caching
        /// </summary>
        /// <remarks>
        /// {0} : show hidden records?
        /// </remarks>
        public static CacheKey UserRolesAllCacheKey => new CacheKey("TVProgViewer.userrole.all-{0}", UserRolesPrefixCacheKey);

        /// <summary>
        /// Gets a key for caching
        /// </summary>
        /// <remarks>
        /// {0} : system name
        /// </remarks>
        public static CacheKey UserRolesBySystemNameCacheKey => new CacheKey("TVProgViewer.userrole.systemname-{0}", UserRolesPrefixCacheKey);

        /// <summary>
        /// Gets a key pattern to clear cache
        /// </summary>
        public static string UserRolesPrefixCacheKey => "TVProgViewer.userrole.";

        /// <summary>
        /// Gets a key for caching
        /// </summary>
        /// <remarks>
        /// {0} : user identifier
        /// {1} : show hidden
        /// </remarks>
        public static CacheKey UserRoleIdsCacheKey => new CacheKey("TVProgViewer.user.userrole.ids-{0}-{1}", UserUserRolesPrefixCacheKey);

        /// <summary>
        /// Gets a key for caching
        /// </summary>
        /// <remarks>
        /// {0} : user identifier
        /// {1} : show hidden
        /// </remarks>
        public static CacheKey UserRolesCacheKey => new CacheKey("TVProgViewer.user.userrole-{0}-{1}", UserUserRolesPrefixCacheKey);
        
        /// <summary>
        /// Gets a key pattern to clear cache
        /// </summary>
        public static string UserUserRolesPrefixCacheKey => "TVProgViewer.user.userrole";

        /// <summary>
        /// Gets a key for caching
        /// </summary>
        /// <remarks>
        /// {0} : user identifier
        /// </remarks>
        public static CacheKey UserAddressesByUserIdCacheKey => new CacheKey("TVProgViewer.user.addresses.by.id-{0}", UserAddressesPrefixCacheKey);

        /// <summary>
        /// Gets a key for caching
        /// </summary>
        /// <remarks>
        /// {0} : user identifier
        /// {1} : address identifier
        /// </remarks>
        public static CacheKey UserAddressCacheKeyCacheKey => new CacheKey("TVProgViewer.user.addresses.address-{0}-{1}", UserAddressesPrefixCacheKey);

        /// <summary>
        /// Gets a key pattern to clear cache
        /// </summary>
        public static string UserAddressesPrefixCacheKey => "TVProgViewer.user.addresses";

        #endregion

        #region User password
        
        /// <summary>
        /// Gets a key for caching current user password lifetime
        /// </summary>
        /// <remarks>
        /// {0} : user identifier
        /// </remarks>
        public static CacheKey UserPasswordLifetimeCacheKey => new CacheKey("TVProgViewer.users.passwordlifetime-{0}");

        #endregion
    }
}