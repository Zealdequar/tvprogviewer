using TVProgViewer.Core.Caching;

namespace TVProgViewer.WebUI.Areas.Admin.Infrastructure.Cache
{
    public static partial class TvProgModelCacheDefaults
    {
        /// <summary>
        /// Key for TvProgViewer.ru news cache
        /// </summary>
        public static CacheKey OfficialNewsModelKey => new CacheKey("TVProgViewer.pres.admin.official.news");

        /// <summary>
        /// Key for categories caching
        /// </summary>
        /// <remarks>
        /// {0} : show hidden records?
        /// </remarks>
        public static CacheKey CategoriesListKey => new CacheKey("TVProgViewer.pres.admin.categories.list-{0}", CategoriesListPrefixCacheKey);
        public static string CategoriesListPrefixCacheKey => "TVProgViewer.pres.admin.categories.list";

        /// <summary>
        /// Key for manufacturers caching
        /// </summary>
        /// <remarks>
        /// {0} : show hidden records?
        /// </remarks>
        public static CacheKey ManufacturersListKey => new CacheKey("TVProgViewer.pres.admin.manufacturers.list-{0}", ManufacturersListPrefixCacheKey);
        public static string ManufacturersListPrefixCacheKey => "TVProgViewer.pres.admin.manufacturers.list";

        /// <summary>
        /// Key for vendors caching
        /// </summary>
        /// <remarks>
        /// {0} : show hidden records?
        /// </remarks>
        public static CacheKey VendorsListKey => new CacheKey("TVProgViewer.pres.admin.vendors.list-{0}", VendorsListPrefixCacheKey);
        public static string VendorsListPrefixCacheKey => "TVProgViewer.pres.admin.vendors.list";
    }
}
