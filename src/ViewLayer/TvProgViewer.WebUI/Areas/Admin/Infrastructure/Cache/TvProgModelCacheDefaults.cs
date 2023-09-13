using TvProgViewer.Core.Caching;

namespace TvProgViewer.WebUI.Areas.Admin.Infrastructure.Cache
{
    public static partial class TvProgModelCacheDefaults
    {
        /// <summary>
        /// Key for tvprogviewer.ru news cache
        /// </summary>
        public static CacheKey OfficialNewsModelKey => new("TvProg.pres.admin.official.news");
        
        /// <summary>
        /// Key for categories caching
        /// </summary>
        public static CacheKey CategoriesListKey => new("TvProg.pres.admin.categories.list");

        /// <summary>
        /// Key for manufacturers caching
        /// </summary>
        public static CacheKey ManufacturersListKey => new("TvProg.pres.admin.manufacturers.list");

        /// <summary>
        /// Key for vendors caching
        /// </summary>
        public static CacheKey VendorsListKey => new("TvProg.pres.admin.vendors.list");
    }
}
