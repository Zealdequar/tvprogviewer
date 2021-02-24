using TVProgViewer.Core.Caching;
using TVProgViewer.Core.Domain.Localization;

namespace TVProgViewer.Services.Localization
{
    /// <summary>
    /// Represents default values related to localization services
    /// </summary>
    public static partial class TvProgLocalizationDefaults
    {
        #region Locales

        /// <summary>
        /// Gets a prefix of locale resources for the admin area
        /// </summary>
        public static string AdminLocaleStringResourcesPrefix => "Admin.";

        /// <summary>
        /// Gets a prefix of locale resources for enumerations 
        /// </summary>
        public static string EnumLocaleStringResourcesPrefix => "Enums.";

        /// <summary>
        /// Gets a prefix of locale resources for permissions 
        /// </summary>
        public static string PermissionLocaleStringResourcesPrefix => "Permission.";

        /// <summary>
        /// Gets a prefix of locale resources for plugin friendly names 
        /// </summary>
        public static string PluginNameLocaleStringResourcesPrefix => "Plugins.FriendlyName.";

        #endregion

        #region Caching defaults

        #region Languages

        /// <summary>
        /// Gets a key for caching
        /// </summary>
        /// <remarks>
        /// {0} : store ID
        /// {1} : show hidden records?
        /// </remarks>
        public static CacheKey LanguagesAllCacheKey => new CacheKey("TvProg.language.all.{0}-{1}", LanguagesByStorePrefix, TvProgEntityCacheDefaults<Language>.AllPrefix);

        /// <summary>
        /// Gets a key pattern to clear cache
        /// </summary>
        /// <remarks>
        /// {0} : store ID
        /// </remarks>
        public static string LanguagesByStorePrefix => "TvProg.language.all.{0}";

        #endregion

        #region Locales

        /// <summary>
        /// Gets a key for caching
        /// </summary>
        /// <remarks>
        /// {0} : language ID
        /// </remarks>
        public static CacheKey LocaleStringResourcesAllPublicCacheKey => new CacheKey("TvProg.localestringresource.bylanguage.public.{0}", TvProgEntityCacheDefaults<LocaleStringResource>.Prefix);

        /// <summary>
        /// Gets a key for caching
        /// </summary>
        /// <remarks>
        /// {0} : language ID
        /// </remarks>
        public static CacheKey LocaleStringResourcesAllAdminCacheKey => new CacheKey("TvProg.localestringresource.bylanguage.admin.{0}", TvProgEntityCacheDefaults<LocaleStringResource>.Prefix);

        /// <summary>
        /// Gets a key for caching
        /// </summary>
        /// <remarks>
        /// {0} : language ID
        /// </remarks>
        public static CacheKey LocaleStringResourcesAllCacheKey => new CacheKey("TvProg.localestringresource.bylanguage.{0}", TvProgEntityCacheDefaults<LocaleStringResource>.Prefix);

        /// <summary>
        /// Gets a key for caching
        /// </summary>
        /// <remarks>
        /// {0} : language ID
        /// {1} : resource key
        /// </remarks>
        public static CacheKey LocaleStringResourcesByNameCacheKey => new CacheKey("TvProg.localestringresource.byname.{0}-{1}", LocaleStringResourcesByNamePrefix, TvProgEntityCacheDefaults<LocaleStringResource>.Prefix);

        /// <summary>
        /// Gets a key pattern to clear cache
        /// </summary>
        /// <remarks>
        /// {0} : language ID
        /// </remarks>
        public static string LocaleStringResourcesByNamePrefix => "TvProg.localestringresource.byname.{0}";

        #endregion

        #region Localized properties

        /// <summary>
        /// Gets a key for caching
        /// </summary>
        /// <remarks>
        /// {0} : language ID
        /// {1} : entity ID
        /// {2} : locale key group
        /// {3} : locale key
        /// </remarks>
        public static CacheKey LocalizedPropertyCacheKey => new CacheKey("TvProg.localizedproperty.value.{0}-{1}-{2}-{3}");

        #endregion

        #endregion
    }
}