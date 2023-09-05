using TvProgViewer.Core.Caching;
using TvProgViewer.Core.Domain.Shipping;

namespace TvProgViewer.Services.Shipping
{
    /// <summary>
    /// Represents default values related to shipping services
    /// </summary>
    public static partial class TvProgShippingDefaults
    {
        #region Caching defaults

        /// <summary>
        /// Gets a key for caching
        /// </summary>
        /// <remarks>
        /// {0} : country identifier
        /// </remarks>
        public static CacheKey ShippingMethodsAllCacheKey => new("TvProg.shippingmethod.all.{0}", TvProgEntityCacheDefaults<ShippingMethod>.AllPrefix);

        #endregion
    }
}