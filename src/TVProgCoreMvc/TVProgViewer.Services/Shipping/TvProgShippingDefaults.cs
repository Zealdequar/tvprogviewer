using TVProgViewer.Core.Caching;
using TVProgViewer.Core.Domain.Shipping;

namespace TVProgViewer.Services.Shipping
{
    public static partial class TvProgShippingDefaults
    {
        #region Caching defaults

        /// <summary>
        /// Gets a key for caching
        /// </summary>
        /// <remarks>
        /// {0} : country identifier
        /// </remarks>
        public static CacheKey ShippingMethodsAllCacheKey => new CacheKey("TvProg.shippingmethod.all.{0}", TvProgEntityCacheDefaults<ShippingMethod>.AllPrefix);

        #endregion
    }
}
