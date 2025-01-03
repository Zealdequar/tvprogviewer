﻿using TvProgViewer.Core.Configuration;

namespace TvProgViewer.Core.Domain.Orders
{
    /// <summary>
    /// Shopping cart settings
    /// </summary>
    public partial class ShoppingCartSettings : ISettings
    {
        /// <summary>
        /// Gets or sets a value indicating whether a user should be redirected to the shopping cart page after adding a tvChannel to the cart/wishlist
        /// </summary>
        public bool DisplayCartAfterAddingTvChannel { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether a user should be redirected to the shopping cart page after adding a tvChannel to the cart/wishlist
        /// </summary>
        public bool DisplayWishlistAfterAddingTvChannel { get; set; }

        /// <summary>
        /// Gets or sets a value indicating maximum number of items in the shopping cart
        /// </summary>
        public int MaximumShoppingCartItems { get; set; }

        /// <summary>
        /// Gets or sets a value indicating maximum number of items in the wishlist
        /// </summary>
        public int MaximumWishlistItems { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether to show tvChannel images in the mini-shopping cart block
        /// </summary>
        public bool AllowOutOfStockItemsToBeAddedToWishlist { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether to move items from wishlist to cart when clicking "Add to cart" button. Otherwise, they are copied.
        /// </summary>
        public bool MoveItemsFromWishlistToCart { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether shopping carts (and wishlist) are shared between stores (in multi-store environment)
        /// </summary>
        public bool CartsSharedBetweenStores { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether to show tvChannel image on shopping cart page
        /// </summary>
        public bool ShowTvChannelImagesOnShoppingCart { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether to show tvChannel image on wishlist page
        /// </summary>
        public bool ShowTvChannelImagesOnWishList { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether to show discount box on shopping cart page
        /// </summary>
        public bool ShowDiscountBox { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether to show gift card box on shopping cart page
        /// </summary>
        public bool ShowGiftCardBox { get; set; }

        /// <summary>
        /// Gets or sets a number of "Cross-sells" on shopping cart page
        /// </summary>
        public int CrossSellsNumber { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether "email a wishlist" feature is enabled
        /// </summary>
        public bool EmailWishlistEnabled { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether to enabled "email a wishlist" for anonymous users.
        /// </summary>
        public bool AllowAnonymousUsersToEmailWishlist { get; set; }

        /// <summary>Gets or sets a value indicating whether mini-shopping cart is enabled
        /// </summary>
        public bool MiniShoppingCartEnabled { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether to show tvChannel images in the mini-shopping cart block
        /// </summary>
        public bool ShowTvChannelImagesInMiniShoppingCart { get; set; }

        /// <summary>Gets or sets a maximum number of tvChannels which can be displayed in the mini-shopping cart block
        /// </summary>
        public int MiniShoppingCartTvChannelNumber { get; set; }

        //Round is already an issue. 
        //When enabled it can cause one issue: https://tvprogviewer.ru/boards/topic/7679/vattax-rounding-error-important-fix
        //When disable it causes another one: https://tvprogviewer.ru/boards/topic/11419/tvprog-20-order-of-steps-in-checkout/page/3#46924

        /// <summary>
        /// Gets or sets a value indicating whether to round calculated prices and total during calculation
        /// </summary>
        public bool RoundPricesDuringCalculation { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether a store owner will be able to offer special prices when users buy bigger amounts of a particular tvChannel.
        /// For example, a user could have two shopping cart items for the same tvChannels (different tvChannel attributes).
        /// </summary>
        public bool GroupTierPricesForDistinctShoppingCartItems { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether a user will be able to edit tvChannels in the cart
        /// </summary>
        public bool AllowCartItemEditing { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether a user will see quantity of attribute values associated to tvChannels (when qty > 1)
        /// </summary>
        public bool RenderAssociatedAttributeValueQuantity { get; set; }
    }
}