using TvProgViewer.Web.Framework.Mvc.ModelBinding;
using TvProgViewer.Web.Framework.Models;

namespace TvProgViewer.WebUI.Areas.Admin.Models.Settings
{
    /// <summary>
    /// Represents a shopping cart settings model
    /// </summary>
    public partial record ShoppingCartSettingsModel : BaseTvProgModel, ISettingsModel
    {
        #region Properties

        public int ActiveStoreScopeConfiguration { get; set; }

        [TvProgResourceDisplayName("Admin.Configuration.Settings.ShoppingCart.DisplayCartAfterAddingTvChannel")]
        public bool DisplayCartAfterAddingTvChannel { get; set; }
        public bool DisplayCartAfterAddingTvChannel_OverrideForStore { get; set; }

        [TvProgResourceDisplayName("Admin.Configuration.Settings.ShoppingCart.DisplayWishlistAfterAddingTvChannel")]
        public bool DisplayWishlistAfterAddingTvChannel { get; set; }
        public bool DisplayWishlistAfterAddingTvChannel_OverrideForStore { get; set; }

        [TvProgResourceDisplayName("Admin.Configuration.Settings.ShoppingCart.MaximumShoppingCartItems")]
        public int MaximumShoppingCartItems { get; set; }
        public bool MaximumShoppingCartItems_OverrideForStore { get; set; }

        [TvProgResourceDisplayName("Admin.Configuration.Settings.ShoppingCart.MaximumWishlistItems")]
        public int MaximumWishlistItems { get; set; }
        public bool MaximumWishlistItems_OverrideForStore { get; set; }

        [TvProgResourceDisplayName("Admin.Configuration.Settings.ShoppingCart.AllowOutOfStockItemsToBeAddedToWishlist")]
        public bool AllowOutOfStockItemsToBeAddedToWishlist { get; set; }
        public bool AllowOutOfStockItemsToBeAddedToWishlist_OverrideForStore { get; set; }

        [TvProgResourceDisplayName("Admin.Configuration.Settings.ShoppingCart.MoveItemsFromWishlistToCart")]
        public bool MoveItemsFromWishlistToCart { get; set; }
        public bool MoveItemsFromWishlistToCart_OverrideForStore { get; set; }

        [TvProgResourceDisplayName("Admin.Configuration.Settings.ShoppingCart.CartsSharedBetweenStores")]
        public bool CartsSharedBetweenStores { get; set; }
        public bool CartsSharedBetweenStores_OverrideForStore { get; set; }

        [TvProgResourceDisplayName("Admin.Configuration.Settings.ShoppingCart.ShowTvChannelImagesOnShoppingCart")]
        public bool ShowTvChannelImagesOnShoppingCart { get; set; }
        public bool ShowTvChannelImagesOnShoppingCart_OverrideForStore { get; set; }

        [TvProgResourceDisplayName("Admin.Configuration.Settings.ShoppingCart.ShowTvChannelImagesOnWishList")]
        public bool ShowTvChannelImagesOnWishList { get; set; }
        public bool ShowTvChannelImagesOnWishList_OverrideForStore { get; set; }

        [TvProgResourceDisplayName("Admin.Configuration.Settings.ShoppingCart.ShowDiscountBox")]
        public bool ShowDiscountBox { get; set; }
        public bool ShowDiscountBox_OverrideForStore { get; set; }

        [TvProgResourceDisplayName("Admin.Configuration.Settings.ShoppingCart.ShowGiftCardBox")]
        public bool ShowGiftCardBox { get; set; }
        public bool ShowGiftCardBox_OverrideForStore { get; set; }

        [TvProgResourceDisplayName("Admin.Configuration.Settings.ShoppingCart.CrossSellsNumber")]
        public int CrossSellsNumber { get; set; }
        public bool CrossSellsNumber_OverrideForStore { get; set; }

        [TvProgResourceDisplayName("Admin.Configuration.Settings.ShoppingCart.EmailWishlistEnabled")]
        public bool EmailWishlistEnabled { get; set; }
        public bool EmailWishlistEnabled_OverrideForStore { get; set; }

        [TvProgResourceDisplayName("Admin.Configuration.Settings.ShoppingCart.AllowAnonymousUsersToEmailWishlist")]
        public bool AllowAnonymousUsersToEmailWishlist { get; set; }
        public bool AllowAnonymousUsersToEmailWishlist_OverrideForStore { get; set; }

        [TvProgResourceDisplayName("Admin.Configuration.Settings.ShoppingCart.MiniShoppingCartEnabled")]
        public bool MiniShoppingCartEnabled { get; set; }
        public bool MiniShoppingCartEnabled_OverrideForStore { get; set; }

        [TvProgResourceDisplayName("Admin.Configuration.Settings.ShoppingCart.ShowTvChannelImagesInMiniShoppingCart")]
        public bool ShowTvChannelImagesInMiniShoppingCart { get; set; }
        public bool ShowTvChannelImagesInMiniShoppingCart_OverrideForStore { get; set; }

        [TvProgResourceDisplayName("Admin.Configuration.Settings.ShoppingCart.MiniShoppingCartTvChannelNumber")]
        public int MiniShoppingCartTvChannelNumber { get; set; }
        public bool MiniShoppingCartTvChannelNumber_OverrideForStore { get; set; }
        
        [TvProgResourceDisplayName("Admin.Configuration.Settings.ShoppingCart.AllowCartItemEditing")]
        public bool AllowCartItemEditing { get; set; }
        public bool AllowCartItemEditing_OverrideForStore { get; set; }
        
        [TvProgResourceDisplayName("Admin.Configuration.Settings.ShoppingCart.GroupTierPricesForDistinctShoppingCartItems")]
        public bool GroupTierPricesForDistinctShoppingCartItems { get; set; }
        public bool GroupTierPricesForDistinctShoppingCartItems_OverrideForStore { get; set; }

        #endregion
    }
}