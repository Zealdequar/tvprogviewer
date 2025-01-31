﻿using TvProgViewer.Core.Caching;
using TvProgViewer.Core.Domain.Security;

namespace TvProgViewer.Services.Security
{
    /// <summary>
    /// Represents default values related to security services
    /// </summary>
    public static partial class TvProgSecurityDefaults
    {
        #region reCAPTCHA

        /// <summary>
        /// Gets a reCAPTCHA script URL
        /// </summary>
        /// <remarks>
        /// {0} : Id of recaptcha instance on page
        /// {1} : Render type
        /// {2} : language if exists
        /// </remarks>
        public static string RecaptchaScriptPath => "api.js?onload=onloadCallback{0}&render={1}{2}";

        /// <summary>
        /// Gets a reCAPTCHA validation URL
        /// </summary>
        /// <remarks>
        /// {0} : private key
        /// {1} : response value
        /// {2} : IP address
        /// </remarks>
        public static string RecaptchaValidationPath => "api/siteverify?secret={0}&response={1}&remoteip={2}";

        #endregion

        #region Caching defaults

        #region Access control list

        /// <summary>
        /// Gets a key for caching
        /// </summary>
        /// <remarks>
        /// {0} : entity ID
        /// {1} : entity name
        /// </remarks>
        public static CacheKey AclRecordCacheKey => new("TvProg.aclrecord.{0}-{1}");

        /// <summary>
        /// Gets a key for caching
        /// </summary>
        /// <remarks>
        /// {0} : entity name
        /// </remarks>
        public static CacheKey EntityAclRecordExistsCacheKey => new("TvProg.aclrecord.exists.{0}");

        #endregion

        #region Permissions

        /// <summary>
        /// Gets a key for caching
        /// </summary>
        /// <remarks>
        /// {0} : permission system name
        /// {1} : user role ID
        /// </remarks>
        public static CacheKey PermissionAllowedCacheKey => new("TvProg.permissionrecord.allowed.{0}-{1}", PermissionAllowedPrefix);

        /// <summary>
        /// Gets a key pattern to clear cache
        /// </summary>
        /// <remarks>
        /// {0} : permission system name
        /// </remarks>
        public static string PermissionAllowedPrefix => "TvProg.permissionrecord.allowed.{0}";

        /// <summary>
        /// Gets a key for caching
        /// </summary>
        /// <remarks>
        /// {0} : user role ID
        /// </remarks>
        public static CacheKey PermissionRecordsAllCacheKey => new("TvProg.permissionrecord.all.{0}", TvProgEntityCacheDefaults<PermissionRecord>.AllPrefix);

        #endregion

        #endregion
    }
}