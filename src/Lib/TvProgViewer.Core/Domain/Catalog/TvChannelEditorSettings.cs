using TvProgViewer.Core.Configuration;

namespace TvProgViewer.Core.Domain.Catalog
{
    /// <summary>
    /// TvChannel editor settings
    /// </summary>
    public partial class TvChannelEditorSettings : ISettings
    {
        /// <summary>
        /// Gets or sets a value indicating whether 'TvChannel type' field is shown
        /// </summary>
        public bool TvChannelType { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether 'Visible individually' field is shown
        /// </summary>
        public bool VisibleIndividually { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether 'TvChannel template' field is shown
        /// </summary>
        public bool TvChannelTemplate { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether 'Admin comment' field is shown
        /// </summary>
        public bool AdminComment { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether 'Vendor' field is shown
        /// </summary>
        public bool Vendor { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether 'Stores' field is shown
        /// </summary>
        public bool Stores { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether 'ACL' field is shown
        /// </summary>
        public bool ACL { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether 'Show on home page' field is shown
        /// </summary>
        public bool ShowOnHomepage { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether 'Allow user reviews' field is shown
        /// </summary>
        public bool AllowUserReviews { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether 'TvChannel tags' field is shown
        /// </summary>
        public bool TvChannelTags { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether 'Manufacturer part number' field is shown
        /// </summary>
        public bool ManufacturerPartNumber { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether 'GTIN' field is shown
        /// </summary>
        public bool GTIN { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether 'TvChannel cost' field is shown
        /// </summary>
        public bool TvChannelCost { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether 'Tier prices' field is shown
        /// </summary>
        public bool TierPrices { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether 'Discounts' field is shown
        /// </summary>
        public bool Discounts { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether 'Disable buy button' field is shown
        /// </summary>
        public bool DisableBuyButton { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether 'Disable wishlist button' field is shown
        /// </summary>
        public bool DisableWishlistButton { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether 'Available for pre-order' field is shown
        /// </summary>
        public bool AvailableForPreOrder { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether 'Call for price' field is shown
        /// </summary>
        public bool CallForPrice { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether 'Old price' field is shown
        /// </summary>
        public bool OldPrice { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether 'User enters price' field is shown
        /// </summary>
        public bool UserEntersPrice { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether 'PAngV' field is shown
        /// </summary>
        public bool PAngV { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether 'Require other tvchannels added to the cart' field is shown
        /// </summary>
        public bool RequireOtherTvChannelsAddedToCart { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether 'Is gift card' field is shown
        /// </summary>
        public bool IsGiftCard { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether 'Downloadable tvchannel' field is shown
        /// </summary>
        public bool DownloadableTvChannel { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether 'Recurring tvchannel' field is shown
        /// </summary>
        public bool RecurringTvChannel { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether 'Is rental' field is shown
        /// </summary>
        public bool IsRental { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether 'Free shipping' field is shown
        /// </summary>
        public bool FreeShipping { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether 'Ship separately' field is shown
        /// </summary>
        public bool ShipSeparately { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether 'Additional shipping charge' field is shown
        /// </summary>
        public bool AdditionalShippingCharge { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether 'Delivery date' field is shown
        /// </summary>
        public bool DeliveryDate { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether 'Telecommunications, broadcasting and electronic services' field is shown
        /// </summary>
        public bool TelecommunicationsBroadcastingElectronicServices { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether 'TvChannel availability range' field is shown
        /// </summary>
        public bool TvChannelAvailabilityRange { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether 'Use multiple warehouses' field is shown
        /// </summary>
        public bool UseMultipleWarehouses { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether 'Warehouse' field is shown
        /// </summary>
        public bool Warehouse { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether 'Display stock availability' field is shown
        /// </summary>
        public bool DisplayStockAvailability { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether 'Minimum stock quantity' field is shown
        /// </summary>
        public bool MinimumStockQuantity { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether 'Low stock activity' field is shown
        /// </summary>
        public bool LowStockActivity { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether 'Notify admin for quantity below' field is shown
        /// </summary>
        public bool NotifyAdminForQuantityBelow { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether 'Backorders' field is shown
        /// </summary>
        public bool Backorders { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether 'Allow back in stock subscriptions' field is shown
        /// </summary>
        public bool AllowBackInStockSubscriptions { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether 'Minimum cart quantity' field is shown
        /// </summary>
        public bool MinimumCartQuantity { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether 'Maximum cart quantity' field is shown
        /// </summary>
        public bool MaximumCartQuantity { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether 'Allowed quantities' field is shown
        /// </summary>
        public bool AllowedQuantities { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether 'Allow only existing attribute combinations' field is shown
        /// </summary>
        public bool AllowAddingOnlyExistingAttributeCombinations { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether 'Not returnable' field is shown
        /// </summary>
        public bool NotReturnable { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether 'Weight' field is shown
        /// </summary>
        public bool Weight { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether 'Dimension' fields (height, length, width) are shown
        /// </summary>
        public bool Dimensions { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether 'Available start date' field is shown
        /// </summary>
        public bool AvailableStartDate { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether 'Available end date' field is shown
        /// </summary>
        public bool AvailableEndDate { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether 'Mark as new' field is shown
        /// </summary>
        public bool MarkAsNew { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether 'Published' field is shown
        /// </summary>
        public bool Published { get; set; }
        
        /// <summary>
        /// Gets or sets a value indicating whether 'Related tvchannels' block is shown
        /// </summary>
        public bool RelatedTvChannels { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether 'Cross-sells tvchannels' block is shown
        /// </summary>
        public bool CrossSellsTvChannels { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether 'SEO' tab is shown
        /// </summary>
        public bool Seo { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether 'Purchased with orders' tab is shown
        /// </summary>
        public bool PurchasedWithOrders { get; set; }
        
        /// <summary>
        /// Gets or sets a value indicating whether 'TvChannel attributes' tab is shown
        /// </summary>
        public bool TvChannelAttributes { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether 'Specification attributes' tab is shown
        /// </summary>
        public bool SpecificationAttributes { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether 'Manufacturers' field is shown
        /// </summary>
        public bool Manufacturers { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether 'Stock quantity history' tab is shown
        /// </summary>
        public bool StockQuantityHistory { get; set; }
    }
}