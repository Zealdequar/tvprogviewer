using TvProgViewer.Core.Caching;
using TvProgViewer.Core.Domain.Users;

namespace TvProgViewer.Services.Users
{
    /// <summary>
    /// Represents default values related to user services
    /// </summary>
    public static partial class TvProgUserServicesDefaults
    {
        /// <summary>
        /// Gets a password salt key size
        /// </summary>
        public static int PasswordSaltKeySize => 5;

        /// <summary>
        /// Gets a max username length
        /// </summary>
        public static int UserUsernameLength => 100;

        /// <summary>
        /// Gets a default hash format for user password
        /// </summary>
        public static string DefaultHashedPasswordFormat => "SHA512";

        /// <summary>
        /// Gets default prefix for user
        /// </summary>
        public static string UserAttributePrefix => "user_attribute_";

        #region Caching defaults

        #region User

        /// <summary>
        /// Gets a key for caching
        /// </summary>
        /// <remarks>
        /// {0} : system name
        /// </remarks>
        public static CacheKey UserBySystemNameCacheKey => new("TvProg.user.bysystemname.{0}");

        /// <summary>
        /// Gets a key for caching
        /// </summary>
        /// <remarks>
        /// {0} : user GUID
        /// </remarks>
        public static CacheKey UserByGuidCacheKey => new("TvProg.user.byguid.{0}");

        #endregion

        #region User attributes

        /// <summary>
        /// Gets a key for caching
        /// </summary>
        /// <remarks>
        /// {0} : user attribute ID
        /// </remarks>
        public static CacheKey UserAttributeValuesByAttributeCacheKey => new("TvProg.userattributevalue.byattribute.{0}");

        #endregion

        #region User roles

        /// <summary>
        /// Gets a key for caching
        /// </summary>
        /// <remarks>
        /// {0} : show hidden records?
        /// </remarks>
        public static CacheKey UserRolesAllCacheKey => new("TvProg.userrole.all.{0}", TvProgEntityCacheDefaults<UserRole>.AllPrefix);

        /// <summary>
        /// Gets a key for caching
        /// </summary>
        /// <remarks>
        /// {0} : system name
        /// </remarks>
        public static CacheKey UserRolesBySystemNameCacheKey => new("TvProg.userrole.bysystemname.{0}", UserRolesBySystemNamePrefix);

        /// <summary>
        /// Gets a key pattern to clear cache
        /// </summary>
        public static string UserRolesBySystemNamePrefix => "TvProg.userrole.bysystemname.";

        /// <summary>
        /// Gets a key for caching
        /// </summary>
        /// <remarks>
        /// {0} : user identifier
        /// {1} : show hidden
        /// </remarks>
        public static CacheKey UserRoleIdsCacheKey => new("TvProg.user.userrole.ids.{0}-{1}", UserUserRolesPrefix);

        /// <summary>
        /// Gets a key for caching
        /// </summary>
        /// <remarks>
        /// {0} : user identifier
        /// {1} : show hidden
        /// </remarks>
        public static CacheKey UserRolesCacheKey => new("TvProg.user.userrole.{0}-{1}", UserUserRolesByUserPrefix, UserUserRolesPrefix);

        /// <summary>
        /// Gets a key pattern to clear cache
        /// </summary>
        public static string UserUserRolesPrefix => "TvProg.user.userrole.";

        /// <summary>
        /// Gets a key pattern to clear cache
        /// </summary>
        /// <remarks>
        /// {0} : user identifier
        /// </remarks>
        public static string UserUserRolesByUserPrefix => "TvProg.user.userrole.{0}";
        
        #endregion

        #region Addresses

        /// <summary>
        /// Gets a key for caching
        /// </summary>
        /// <remarks>
        /// {0} : user identifier
        /// </remarks>
        public static CacheKey UserAddressesCacheKey => new("TvProg.user.addresses.{0}", UserAddressesPrefix);

        /// <summary>
        /// Gets a key for caching
        /// </summary>
        /// <remarks>
        /// {0} : user identifier
        /// {1} : address identifier
        /// </remarks>
        public static CacheKey UserAddressCacheKey => new("TvProg.user.addresses.{0}-{1}", UserAddressesByUserPrefix, UserAddressesPrefix);

        /// <summary>
        /// Gets a key pattern to clear cache
        /// </summary>
        public static string UserAddressesPrefix => "TvProg.user.addresses.";

        /// <summary>
        /// Gets a key pattern to clear cache
        /// </summary>
        /// <remarks>
        /// {0} : user identifier
        /// </remarks>
        public static string UserAddressesByUserPrefix => "TvProg.user.addresses.{0}";
        
        #endregion

        #region User password

        /// <summary>
        /// Gets a key for caching current user password lifetime
        /// </summary>
        /// <remarks>
        /// {0} : user identifier
        /// </remarks>
        public static CacheKey UserPasswordLifetimeCacheKey => new("TvProg.userpassword.lifetime.{0}");

        #endregion

        #endregion

    }
}