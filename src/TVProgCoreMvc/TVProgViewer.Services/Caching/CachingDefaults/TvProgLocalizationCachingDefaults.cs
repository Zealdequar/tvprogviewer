using TVProgViewer.Core.Caching;

namespace TVProgViewer.Services.Caching.CachingDefaults
{
    /// <summary>
    /// Represents default values related to localization services
    /// </summary>
    public static partial class TvProgLocalizationCachingDefaults
    {
        #region Languages

        /// <summary>
        /// Gets a key for caching
        /// </summary>
        /// <remarks>
        /// {0} : store ID
        /// {1} : show hidden records?
        /// </remarks>
        public static CacheKey LanguagesAllCacheKey => new CacheKey("TVProgViewer.language.all-{0}-{1}", LanguagesByStoreIdPrefixCacheKey, LanguagesAllPrefixCacheKey);

        /// <summary>
        /// Gets a key pattern to clear cache
        /// </summary>
        /// <remarks>
        /// {0} : store ID
        /// </remarks>
        public static string LanguagesByStoreIdPrefixCacheKey => "TVProgViewer.language.all-{0}";

        /// <summary>
        /// Gets a key pattern to clear cache
        /// </summary>
        public static string LanguagesAllPrefixCacheKey => "TVProgViewer.language.all";

        #endregion

        #region Locales

        /// <summary>
        /// Gets a key for caching
        /// </summary>
        /// <remarks>
        /// {0} : language ID
        /// </remarks>
        public static CacheKey LocaleStringResourcesAllPublicCacheKey => new CacheKey("Nop.localestringresource.bylanguage.public.{0}", LocaleStringResourcesPrefixCacheKey);

        /// <summary>
        /// Gets a key for caching
        /// </summary>
        /// <remarks>
        /// {0} : language ID
        /// </remarks>
        public static CacheKey LocaleStringResourcesAllAdminCacheKey => new CacheKey("Nop.localestringresource.bylanguage.admin.{0}", LocaleStringResourcesPrefixCacheKey);

        /// <summary>
        /// Gets a key for caching
        /// </summary>
        /// <remarks>
        /// {0} : language ID
        /// </remarks>
        public static CacheKey LocaleStringResourcesAllCacheKey => new CacheKey("Nop.localestringresource.bylanguage.{0}", LocaleStringResourcesPrefixCacheKey);

        /// <summary>
        /// Gets a key for caching
        /// </summary>
        /// <remarks>
        /// {0} : language ID
        /// {1} : resource key
        /// </remarks>
        public static CacheKey LocaleStringResourcesByResourceNameCacheKey => new CacheKey("Nop.localestringresource.byname.{0}-{1}", LocaleStringResourcesByResourceNamePrefixCacheKey, LocaleStringResourcesPrefixCacheKey);

        /// <summary>
        /// Gets a key for caching
        /// </summary>
        /// <remarks>
        /// {0} : language ID
        /// </remarks>
        public static string LocaleStringResourcesByResourceNamePrefixCacheKey => "TVProgViewer.lsr.{0}";

        /// <summary>
        /// Gets a key pattern to clear cache
        /// </summary>
        public static string LocaleStringResourcesPrefixCacheKey => "TVProgViewer.lsr.";

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
        public static CacheKey LocalizedPropertyCacheKey => new CacheKey("TVProgViewer.localizedproperty.value-{0}-{1}-{2}-{3}");

        /// <summary>
        /// Gets a key for caching
        /// </summary>
        public static CacheKey LocalizedPropertyAllCacheKey => new CacheKey("TVProgViewer.localizedproperty.all");
        
        #endregion
    }
}