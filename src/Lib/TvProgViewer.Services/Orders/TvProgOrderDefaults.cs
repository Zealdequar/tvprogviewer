using TvProgViewer.Core.Caching;
using TvProgViewer.Core.Domain.Orders;

namespace TvProgViewer.Services.Orders
{
    /// <summary>
    /// Represents default values related to orders services
    /// </summary>
    public static partial class TvProgOrderDefaults
    {
        #region Caching defaults

        #region Checkout attributes

        /// <summary>
        /// Gets a key for caching
        /// </summary>
        /// <remarks>
        /// {0} : store ID
        /// {1} : A value indicating whether we should exclude shippable attributes
        /// </remarks>
        public static CacheKey CheckoutAttributesAllCacheKey => new("TvProg.checkoutattribute.all.{0}-{1}", TvProgEntityCacheDefaults<CheckoutAttribute>.AllPrefix);

        /// <summary>
        /// Gets a key for caching
        /// </summary>
        /// <remarks>
        /// {0} : checkout attribute ID
        /// </remarks>
        public static CacheKey CheckoutAttributeValuesAllCacheKey => new("TvProg.checkoutattributevalue.byattribute.{0}");

        #endregion

        #region ShoppingCart

        /// <summary>
        /// Gets a key for caching
        /// </summary>
        /// <remarks>
        /// {0} : user ID
        /// {1} : shopping cart type
        /// {2} : store ID
        /// {3} : tvchannel ID
        /// {4} : created from date
        /// {5} : created to date
        /// </remarks>
        public static CacheKey ShoppingCartItemsAllCacheKey => new("TvProg.shoppingcartitem.all.{0}-{1}-{2}-{3}-{4}-{5}", ShoppingCartItemsByUserPrefix, TvProgEntityCacheDefaults<ShoppingCartItem>.AllPrefix);

        /// <summary>
        /// Gets a key pattern to clear cache
        /// </summary>
        /// <remarks>
        /// {0} : user identifier
        /// </remarks>
        public static string ShoppingCartItemsByUserPrefix => "TvProg.shoppingcartitem.all.{0}";


        #endregion

        #endregion
    }
}