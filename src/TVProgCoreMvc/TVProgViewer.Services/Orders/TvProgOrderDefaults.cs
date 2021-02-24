using TVProgViewer.Core.Caching;
using TVProgViewer.Core.Domain.Orders;

namespace TVProgViewer.Services.Orders
{
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
        public static CacheKey CheckoutAttributesAllCacheKey => new CacheKey("TvProg.checkoutattribute.all.{0}-{1}", TvProgEntityCacheDefaults<CheckoutAttribute>.AllPrefix);

        /// <summary>
        /// Gets a key for caching
        /// </summary>
        /// <remarks>
        /// {0} : checkout attribute ID
        /// </remarks>
        public static CacheKey CheckoutAttributeValuesAllCacheKey => new CacheKey("TvProg.checkoutattributevalue.byattribute.{0}");

        #endregion

        #region ShoppingCart

        /// <summary>
        /// Gets a key for caching
        /// </summary>
        /// <remarks>
        /// {0} : user ID
        /// {1} : shopping cart type
        /// {2} : store ID
        /// {3} : product ID
        /// {4} : created from date
        /// {5} : created to date
        /// </remarks>
        public static CacheKey ShoppingCartItemsAllCacheKey => new CacheKey("TvProg.shoppingcartitem.all.{0}-{1}-{2}-{3}-{4}-{5}", TvProgEntityCacheDefaults<ShoppingCartItem>.AllPrefix);

        #endregion

        #endregion
    }
}
