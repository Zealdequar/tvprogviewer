using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TvProgViewer.Core.Domain.Catalog;
using TvProgViewer.Core.Domain.Users;
using TvProgViewer.Core.Domain.Discounts;
using TvProgViewer.Core.Domain.Orders;
using TvProgViewer.Core.Domain.Stores;

namespace TvProgViewer.Services.Orders
{
    /// <summary>
    /// Shopping cart service
    /// </summary>
    public partial interface IShoppingCartService
    {
        /// <summary>
        /// Delete shopping cart item
        /// </summary>
        /// <param name="shoppingCartItem">Shopping cart item</param>
        /// <param name="resetCheckoutData">A value indicating whether to reset checkout data</param>
        /// <param name="ensureOnlyActiveCheckoutAttributes">A value indicating whether to ensure that only active checkout attributes are attached to the current user</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        Task DeleteShoppingCartItemAsync(ShoppingCartItem shoppingCartItem, bool resetCheckoutData = true,
            bool ensureOnlyActiveCheckoutAttributes = false);

        /// <summary>
        /// Delete shopping cart item
        /// </summary>
        /// <param name="shoppingCartItemId">Shopping cart item ID</param>
        /// <param name="resetCheckoutData">A value indicating whether to reset checkout data</param>
        /// <param name="ensureOnlyActiveCheckoutAttributes">A value indicating whether to ensure that only active checkout attributes are attached to the current user</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        Task DeleteShoppingCartItemAsync(int shoppingCartItemId, bool resetCheckoutData = true,
            bool ensureOnlyActiveCheckoutAttributes = false);

        /// <summary>
        /// Deletes expired shopping cart items
        /// </summary>
        /// <param name="olderThanUtc">Older than date and time</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the number of deleted items
        /// </returns>
        Task<int> DeleteExpiredShoppingCartItemsAsync(DateTime olderThanUtc);

        /// <summary>
        /// Get tvchannels from shopping cart whether requiring specific tvchannel
        /// </summary>
        /// <param name="cart">Shopping cart </param>
        /// <param name="tvchannel">TvChannel</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the result
        /// </returns>
        Task<IList<TvChannel>> GetTvChannelsRequiringTvChannelAsync(IList<ShoppingCartItem> cart, TvChannel tvchannel);

        /// <summary>
        /// Gets shopping cart
        /// </summary>
        /// <param name="user">User</param>
        /// <param name="shoppingCartType">Shopping cart type; pass null to load all records</param>
        /// <param name="storeId">Store identifier; pass 0 to load all records</param>
        /// <param name="tvchannelId">TvChannel identifier; pass null to load all records</param>
        /// <param name="createdFromUtc">Created date from (UTC); pass null to load all records</param>
        /// <param name="createdToUtc">Created date to (UTC); pass null to load all records</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the shopping Cart
        /// </returns>
        Task<IList<ShoppingCartItem>> GetShoppingCartAsync(User user, ShoppingCartType? shoppingCartType = null,
            int storeId = 0, int? tvchannelId = null, DateTime? createdFromUtc = null, DateTime? createdToUtc = null);

        /// <summary>
        /// Validates shopping cart item attributes
        /// </summary>
        /// <param name="user">User</param>
        /// <param name="shoppingCartType">Shopping cart type</param>
        /// <param name="tvchannel">TvChannel</param>
        /// <param name="quantity">Quantity</param>
        /// <param name="attributesXml">Attributes in XML format</param>
        /// <param name="ignoreNonCombinableAttributes">A value indicating whether we should ignore non-combinable attributes</param>
        /// <param name="ignoreConditionMet">A value indicating whether we should ignore filtering by "is condition met" property</param>
        /// <param name="ignoreBundledTvChannels">A value indicating whether we should ignore bundled (associated) tvchannels</param>
        /// <param name="shoppingCartItemId">Shopping cart identifier; pass 0 if it's a new item</param> 
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the warnings
        /// </returns>
        Task<IList<string>> GetShoppingCartItemAttributeWarningsAsync(User user,
            ShoppingCartType shoppingCartType,
            TvChannel tvchannel,
            int quantity = 1,
            string attributesXml = "",
            bool ignoreNonCombinableAttributes = false,
            bool ignoreConditionMet = false,
            bool ignoreBundledTvChannels = false,
            int shoppingCartItemId = 0);

        /// <summary>
        /// Validates shopping cart item (gift card)
        /// </summary>
        /// <param name="shoppingCartType">Shopping cart type</param>
        /// <param name="tvchannel">TvChannel</param>
        /// <param name="attributesXml">Attributes in XML format</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the warnings
        /// </returns>
        Task<IList<string>> GetShoppingCartItemGiftCardWarningsAsync(ShoppingCartType shoppingCartType,
            TvChannel tvchannel, string attributesXml);

        /// <summary>
        /// Validates shopping cart item for rental tvchannels
        /// </summary>
        /// <param name="tvchannel">TvChannel</param>
        /// <param name="rentalStartDate">Rental start date</param>
        /// <param name="rentalEndDate">Rental end date</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the warnings
        /// </returns>
        Task<IList<string>> GetRentalTvChannelWarningsAsync(TvChannel tvchannel,
            DateTime? rentalStartDate = null, DateTime? rentalEndDate = null);

        /// <summary>
        /// Validates shopping cart item
        /// </summary>
        /// <param name="user">User</param>
        /// <param name="shoppingCartType">Shopping cart type</param>
        /// <param name="tvchannel">TvChannel</param>
        /// <param name="storeId">Store identifier</param>
        /// <param name="attributesXml">Attributes in XML format</param>
        /// <param name="userEnteredPrice">User entered price</param>
        /// <param name="rentalStartDate">Rental start date</param>
        /// <param name="rentalEndDate">Rental end date</param>
        /// <param name="quantity">Quantity</param>
        /// <param name="addRequiredTvChannels">Whether to add required tvchannels</param>
        /// <param name="shoppingCartItemId">Shopping cart identifier; pass 0 if it's a new item</param>
        /// <param name="getStandardWarnings">A value indicating whether we should validate a tvchannel for standard properties</param>
        /// <param name="getAttributesWarnings">A value indicating whether we should validate tvchannel attributes</param>
        /// <param name="getGiftCardWarnings">A value indicating whether we should validate gift card properties</param>
        /// <param name="getRequiredTvChannelWarnings">A value indicating whether we should validate required tvchannels (tvchannels which require other tvchannels to be added to the cart)</param>
        /// <param name="getRentalWarnings">A value indicating whether we should validate rental properties</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the warnings
        /// </returns>
        Task<IList<string>> GetShoppingCartItemWarningsAsync(User user, ShoppingCartType shoppingCartType,
            TvChannel tvchannel, int storeId,
            string attributesXml, decimal userEnteredPrice,
            DateTime? rentalStartDate = null, DateTime? rentalEndDate = null,
            int quantity = 1, bool addRequiredTvChannels = true, int shoppingCartItemId = 0,
            bool getStandardWarnings = true, bool getAttributesWarnings = true,
            bool getGiftCardWarnings = true, bool getRequiredTvChannelWarnings = true,
            bool getRentalWarnings = true);

        /// <summary>
        /// Validates whether this shopping cart is valid
        /// </summary>
        /// <param name="shoppingCart">Shopping cart</param>
        /// <param name="checkoutAttributesXml">Checkout attributes in XML format</param>
        /// <param name="validateCheckoutAttributes">A value indicating whether to validate checkout attributes</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the warnings
        /// </returns>
        Task<IList<string>> GetShoppingCartWarningsAsync(IList<ShoppingCartItem> shoppingCart,
            string checkoutAttributesXml, bool validateCheckoutAttributes);

        /// <summary>
        /// Gets the shopping cart unit price (one item)
        /// </summary>
        /// <param name="shoppingCartItem">The shopping cart item</param>
        /// <param name="includeDiscounts">A value indicating whether include discounts or not for price computation</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the shopping cart unit price (one item). Applied discount amount. Applied discounts
        /// </returns>
        Task<(decimal unitPrice, decimal discountAmount, List<Discount> appliedDiscounts)> GetUnitPriceAsync(ShoppingCartItem shoppingCartItem,
            bool includeDiscounts);

        /// <summary>
        /// Gets the shopping cart unit price (one item)
        /// </summary>
        /// <param name="tvchannel">TvChannel</param>
        /// <param name="user">User</param>
        /// <param name="shoppingCartType">Shopping cart type</param>
        /// <param name="quantity">Quantity</param>
        /// <param name="attributesXml">TvChannel attributes (XML format)</param>
        /// <param name="userEnteredPrice">User entered price (if specified)</param>
        /// <param name="rentalStartDate">Rental start date (null for not rental tvchannels)</param>
        /// <param name="rentalEndDate">Rental end date (null for not rental tvchannels)</param>
        /// <param name="includeDiscounts">A value indicating whether include discounts or not for price computation</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the shopping cart unit price (one item)
        /// </returns>
        Task<(decimal unitPrice, decimal discountAmount, List<Discount> appliedDiscounts)> GetUnitPriceAsync(TvChannel tvchannel,
            User user,
            Store store,
            ShoppingCartType shoppingCartType,
            int quantity,
            string attributesXml,
            decimal userEnteredPrice,
            DateTime? rentalStartDate, DateTime? rentalEndDate,
            bool includeDiscounts);

        /// <summary>
        /// Gets the shopping cart item sub total
        /// </summary>
        /// <param name="shoppingCartItem">The shopping cart item</param>
        /// <param name="includeDiscounts">A value indicating whether include discounts or not for price computation</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the shopping cart item sub total. Applied discount amount.Applied discounts. Maximum discounted qty. Return not nullable value if discount cannot be applied to ALL items
        /// </returns>
        Task<(decimal subTotal, decimal discountAmount, List<Discount> appliedDiscounts, int? maximumDiscountQty)> GetSubTotalAsync(ShoppingCartItem shoppingCartItem,
            bool includeDiscounts);

        /// <summary>
        /// Finds a shopping cart item in the cart
        /// </summary>
        /// <param name="shoppingCart">Shopping cart</param>
        /// <param name="shoppingCartType">Shopping cart type</param>
        /// <param name="tvchannel">TvChannel</param>
        /// <param name="attributesXml">Attributes in XML format</param>
        /// <param name="userEnteredPrice">Price entered by a user</param>
        /// <param name="rentalStartDate">Rental start date</param>
        /// <param name="rentalEndDate">Rental end date</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the found shopping cart item
        /// </returns>
        Task<ShoppingCartItem> FindShoppingCartItemInTheCartAsync(IList<ShoppingCartItem> shoppingCart,
            ShoppingCartType shoppingCartType,
            TvChannel tvchannel,
            string attributesXml = "",
            decimal userEnteredPrice = decimal.Zero,
            DateTime? rentalStartDate = null,
            DateTime? rentalEndDate = null);

        /// <summary>
        /// Add a tvchannel to shopping cart
        /// </summary>
        /// <param name="user">User</param>
        /// <param name="tvchannel">TvChannel</param>
        /// <param name="shoppingCartType">Shopping cart type</param>
        /// <param name="storeId">Store identifier</param>
        /// <param name="attributesXml">Attributes in XML format</param>
        /// <param name="userEnteredPrice">The price enter by a user</param>
        /// <param name="rentalStartDate">Rental start date</param>
        /// <param name="rentalEndDate">Rental end date</param>
        /// <param name="quantity">Quantity</param>
        /// <param name="addRequiredTvChannels">Whether to add required tvchannels</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the warnings
        /// </returns>
        Task<IList<string>> AddToCartAsync(User user, TvChannel tvchannel,
            ShoppingCartType shoppingCartType, int storeId, string attributesXml = null,
            decimal userEnteredPrice = decimal.Zero,
            DateTime? rentalStartDate = null, DateTime? rentalEndDate = null,
            int quantity = 1, bool addRequiredTvChannels = true);

        /// <summary>
        /// Updates the shopping cart item
        /// </summary>
        /// <param name="user">User</param>
        /// <param name="shoppingCartItemId">Shopping cart item identifier</param>
        /// <param name="attributesXml">Attributes in XML format</param>
        /// <param name="userEnteredPrice">New user entered price</param>
        /// <param name="rentalStartDate">Rental start date</param>
        /// <param name="rentalEndDate">Rental end date</param>
        /// <param name="quantity">New shopping cart item quantity</param>
        /// <param name="resetCheckoutData">A value indicating whether to reset checkout data</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the warnings
        /// </returns>
        Task<IList<string>> UpdateShoppingCartItemAsync(User user,
            int shoppingCartItemId, string attributesXml,
            decimal userEnteredPrice,
            DateTime? rentalStartDate = null, DateTime? rentalEndDate = null,
            int quantity = 1, bool resetCheckoutData = true);

        /// <summary>
        /// Migrate shopping cart
        /// </summary>
        /// <param name="fromUser">From user</param>
        /// <param name="toUser">To user</param>
        /// <param name="includeCouponCodes">A value indicating whether to coupon codes (discount and gift card) should be also re-applied</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        Task MigrateShoppingCartAsync(User fromUser, User toUser, bool includeCouponCodes);

        /// <summary>
        /// Indicates whether the shopping cart requires shipping
        /// </summary>
        /// <param name="shoppingCart">Shopping cart</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the rue if the shopping cart requires shipping; otherwise, false.
        /// </returns>
        Task<bool> ShoppingCartRequiresShippingAsync(IList<ShoppingCartItem> shoppingCart);

        /// <summary>
        /// Gets a value indicating whether shopping cart is recurring
        /// </summary>
        /// <param name="shoppingCart">Shopping cart</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the result
        /// </returns>
        Task<bool> ShoppingCartIsRecurringAsync(IList<ShoppingCartItem> shoppingCart);

        /// <summary>
        /// Get a recurring cycle information
        /// </summary>
        /// <param name="shoppingCart">Shopping cart</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the error (if exists); otherwise, empty string
        /// </returns>
        Task<(string error, int cycleLength, RecurringTvChannelCyclePeriod cyclePeriod, int totalCycles)> GetRecurringCycleInfoAsync(IList<ShoppingCartItem> shoppingCart);
    }
}