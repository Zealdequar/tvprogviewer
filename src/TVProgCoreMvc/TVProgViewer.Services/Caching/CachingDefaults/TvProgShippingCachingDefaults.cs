using TVProgViewer.Core.Caching;

namespace TVProgViewer.Services.Caching.CachingDefaults
{
    /// <summary>
    /// Represents default values related to shipping services
    /// </summary>
    public static partial class TvProgShippingCachingDefaults
    {
        #region Shipping methods

        /// <summary>
        /// Gets a key for caching
        /// </summary>
        /// <remarks>
        /// {0} : country identifier
        /// </remarks>
        public static CacheKey ShippingMethodsAllCacheKey => new CacheKey("TVProgViewer.shippingmethod.all-{0}", ShippingMethodsAllPrefixCacheKey);

        /// <summary>
        /// Gets a key pattern to clear cache
        /// </summary>
        public static string ShippingMethodsAllPrefixCacheKey => "TVProgViewer.shippingmethod.all";

        #endregion

        #region Warehouses
        
        /// <summary>
        /// Gets a key for caching
        /// </summary>
        /// <remarks>
        /// </remarks>
        public static CacheKey WarehousesAllCacheKey => new CacheKey("TVProgViewer.warehouse.all");

        #endregion

        #region Date

        /// <summary>
        /// Gets a key for caching
        /// </summary>
        /// <remarks>
        /// </remarks>
        public static CacheKey DeliveryDatesAllCacheKey => new CacheKey("TVProgViewer.deliverydates.all");

        /// <summary>
        /// Gets a key for caching
        /// </summary>
        /// <remarks>
        /// </remarks>
        public static CacheKey ProductAvailabilityAllCacheKey => new CacheKey("TVProgViewer.productavailability.all");
        
        #endregion
    }
}