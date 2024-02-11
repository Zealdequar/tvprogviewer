using System.Collections.Generic;
using TvProgViewer.Core.Configuration;

namespace TvProgViewer.Core.Domain.Catalog
{
    /// <summary>
    /// Catalog settings
    /// </summary>
    public partial class CatalogSettings : ISettings
    {
        public CatalogSettings()
        {
            TvChannelSortingEnumDisabled = new List<int>();
            TvChannelSortingEnumDisplayOrder = new Dictionary<int, int>();
        }

        /// <summary>
        /// Gets or sets a value indicating details pages of unpublished tvchannel details pages could be open (for SEO optimization)
        /// </summary>
        public bool AllowViewUnpublishedTvChannelPage { get; set; }

        /// <summary>
        /// Gets or sets a value indicating users should see "discontinued" message when visiting details pages of unpublished tvchannels (if "AllowViewUnpublishedTvChannelPage" is "true)
        /// </summary>
        public bool DisplayDiscontinuedMessageForUnpublishedTvChannels { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether "Published" or "Disable buy/wishlist buttons" flags should be updated after order cancellation (deletion).
        /// Of course, when qty > configured minimum stock level
        /// </summary>
        public bool PublishBackTvChannelWhenCancellingOrders { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether to display tvchannel SKU on the tvchannel details page
        /// </summary>
        public bool ShowSkuOnTvChannelDetailsPage { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether to display tvchannel SKU on catalog pages
        /// </summary>
        public bool ShowSkuOnCatalogPages { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether to display manufacturer part number of a tvchannel
        /// </summary>
        public bool ShowManufacturerPartNumber { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether to display GTIN of a tvchannel
        /// </summary>
        public bool ShowGtin { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether "Free shipping" icon should be displayed for tvchannels
        /// </summary>
        public bool ShowFreeShippingNotification { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether short description should be displayed in tvchannel box
        /// </summary>
        public bool ShowShortDescriptionOnCatalogPages { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether tvchannel sorting is enabled
        /// </summary>
        public bool AllowTvChannelSorting { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether users are allowed to change tvchannel view mode
        /// </summary>
        public bool AllowTvChannelViewModeChanging { get; set; }

        /// <summary>
        /// Gets or sets a default view mode
        /// </summary>
        public string DefaultViewMode { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether a category details page should include tvchannels from subcategories
        /// </summary>
        public bool ShowTvChannelsFromSubcategories { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether number of tvchannels should be displayed beside each category
        /// </summary>
        public bool ShowCategoryTvChannelNumber { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether we include subcategories (when 'ShowCategoryTvChannelNumber' is 'true')
        /// </summary>
        public bool ShowCategoryTvChannelNumberIncludingSubcategories { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether category breadcrumb is enabled
        /// </summary>
        public bool CategoryBreadcrumbEnabled { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether a 'Share button' is enabled
        /// </summary>
        public bool ShowShareButton { get; set; }

        /// <summary>
        /// Gets or sets a share code (e.g. AddThis button code)
        /// </summary>
        public string PageShareCode { get; set; }

        /// <summary>
        /// Gets or sets a value indicating tvchannel reviews must be approved
        /// </summary>
        public bool TvChannelReviewsMustBeApproved { get; set; }

        /// <summary>
        /// Gets or sets a value indicating that user can add only one review per tvchannel
        /// </summary>
        public bool OneReviewPerTvChannelFromUser { get; set; }

        /// <summary>
        /// Gets or sets a value indicating the default rating value of the tvchannel reviews
        /// </summary>
        public int DefaultTvChannelRatingValue { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether to allow anonymous users write tvchannel reviews.
        /// </summary>
        public bool AllowAnonymousUsersToReviewTvChannel { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether tvchannel can be reviewed only by user who have already ordered it
        /// </summary>
        public bool TvChannelReviewPossibleOnlyAfterPurchasing { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether notification of a store owner about new tvchannel reviews is enabled
        /// </summary>
        public bool NotifyStoreOwnerAboutNewTvChannelReviews { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether user notification about tvchannel review reply is enabled
        /// </summary>
        public bool NotifyUserAboutTvChannelReviewReply { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the tvchannel reviews will be filtered per store
        /// </summary>
        public bool ShowTvChannelReviewsPerStore { get; set; }

        /// <summary>
        /// Gets or sets a show tvchannel reviews tab on account page
        /// </summary>
        public bool ShowTvChannelReviewsTabOnAccountPage { get; set; }

        /// <summary>
        /// Gets or sets the page size for tvchannel reviews in account page
        /// </summary>
        public int TvChannelReviewsPageSizeOnAccountPage { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the tvchannel reviews should be sorted by creation date as ascending
        /// </summary>
        public bool TvChannelReviewsSortByCreatedDateAscending { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether tvchannel 'Email a friend' feature is enabled
        /// </summary>
        public bool EmailAFriendEnabled { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether to allow anonymous users to email a friend.
        /// </summary>
        public bool AllowAnonymousUsersToEmailAFriend { get; set; }

        /// <summary>
        /// Gets or sets a number of "Recently viewed tvchannels"
        /// </summary>
        public int RecentlyViewedTvChannelsNumber { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether "Recently viewed tvchannels" feature is enabled
        /// </summary>
        public bool RecentlyViewedTvChannelsEnabled { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether "New tvchannels" page is enabled
        /// </summary>
        public bool NewTvChannelsEnabled { get; set; }

        /// <summary>
        /// Gets or sets a number of tvchannels on the "New tvchannels" page
        /// </summary>
        public int NewTvChannelsPageSize { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether users are allowed to select page size on the "New tvchannels" page
        /// </summary>
        public bool NewTvChannelsAllowUsersToSelectPageSize { get; set; }

        /// <summary>
        /// Gets or sets the available user selectable page size options on the "New tvchannels" page
        /// </summary>
        public string NewTvChannelsPageSizeOptions { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether "Compare tvchannels" feature is enabled
        /// </summary>
        public bool CompareTvChannelsEnabled { get; set; }

        /// <summary>
        /// Gets or sets an allowed number of tvchannels to be compared
        /// </summary>
        public int CompareTvChannelsNumber { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether autocomplete is enabled
        /// </summary>
        public bool TvChannelSearchAutoCompleteEnabled { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the search box is displayed
        /// </summary>
        public bool TvChannelSearchEnabled { get; set; }

        /// <summary>
        /// Gets or sets a number of tvchannels to return when using "autocomplete" feature
        /// </summary>
        public int TvChannelSearchAutoCompleteNumberOfTvChannels { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether to show tvchannel images in the auto complete search
        /// </summary>
        public bool ShowTvChannelImagesInSearchAutoComplete { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether to show link to all result in the auto complete search
        /// </summary>
        public bool ShowLinkToAllResultInSearchAutoComplete { get; set; }

        /// <summary>
        /// Gets or sets a minimum search term length
        /// </summary>
        public int TvChannelSearchTermMinimumLength { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether to show bestsellers on home page
        /// </summary>
        public bool ShowBestsellersOnHomepage { get; set; }

        /// <summary>
        /// Gets or sets a number of bestsellers on home page
        /// </summary>
        public int NumberOfBestsellersOnHomepage { get; set; }

        /// <summary>
        /// Gets or sets a number of tvchannels per page on the search tvchannels page
        /// </summary>
        public int SearchPageTvChannelsPerPage { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether users are allowed to select page size on the search tvchannels page
        /// </summary>
        public bool SearchPageAllowUsersToSelectPageSize { get; set; }

        /// <summary>
        /// Gets or sets the available user selectable page size options on the search tvchannels page
        /// </summary>
        public string SearchPagePageSizeOptions { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the price range filtering is enabled on search page
        /// </summary>
        public bool SearchPagePriceRangeFiltering { get; set; }

        /// <summary>
        /// Gets or sets the "from" price on search page
        /// </summary>
        public decimal SearchPagePriceFrom { get; set; }

        /// <summary>
        /// Gets or sets the "to" price on search page
        /// </summary>
        public decimal SearchPagePriceTo { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the price range should be entered manually on search page
        /// </summary>
        public bool SearchPageManuallyPriceRange { get; set; }

        /// <summary>
        /// Gets or sets "List of tvchannels purchased by other users who purchased the above" option is enable
        /// </summary>
        public bool TvChannelsAlsoPurchasedEnabled { get; set; }

        /// <summary>
        /// Gets or sets a number of tvchannels also purchased by other users to display
        /// </summary>
        public int TvChannelsAlsoPurchasedNumber { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether we should process attribute change using AJAX. It's used for dynamical attribute change, SKU/GTIN update of combinations, conditional attributes
        /// </summary>
        public bool AjaxProcessAttributeChange { get; set; }

        /// <summary>
        /// Gets or sets a number of tvchannel tags that appear in the tag cloud
        /// </summary>
        public int NumberOfTvChannelTags { get; set; }

        /// <summary>
        /// Gets or sets a number of tvchannels per page on 'tvchannels by tag' page
        /// </summary>
        public int TvChannelsByTagPageSize { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether users can select the page size for 'tvchannels by tag'
        /// </summary>
        public bool TvChannelsByTagAllowUsersToSelectPageSize { get; set; }

        /// <summary>
        /// Gets or sets the available user selectable page size options for 'tvchannels by tag'
        /// </summary>
        public string TvChannelsByTagPageSizeOptions { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the price range filtering is enabled on 'tvchannels by tag' page
        /// </summary>
        public bool TvChannelsByTagPriceRangeFiltering { get; set; }

        /// <summary>
        /// Gets or sets the "from" price on 'tvchannels by tag' page
        /// </summary>
        public decimal TvChannelsByTagPriceFrom { get; set; }

        /// <summary>
        /// Gets or sets the "to" price on 'tvchannels by tag' page
        /// </summary>
        public decimal TvChannelsByTagPriceTo { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the price range should be entered manually on 'tvchannels by tag' page
        /// </summary>
        public bool TvChannelsByTagManuallyPriceRange { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether to include "Short description" in compare tvchannels
        /// </summary>
        public bool IncludeShortDescriptionInCompareTvChannels { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether to include "Full description" in compare tvchannels
        /// </summary>
        public bool IncludeFullDescriptionInCompareTvChannels { get; set; }

        /// <summary>
        /// An option indicating whether tvchannels on category and manufacturer pages should include featured tvchannels as well
        /// </summary>
        public bool IncludeFeaturedTvChannelsInNormalLists { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether to render link to required tvchannels in "Require other tvchannels added to the cart" warning
        /// </summary>
        public bool UseLinksInRequiredTvChannelWarnings { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether tier prices should be displayed with applied discounts (if available)
        /// </summary>
        public bool DisplayTierPricesWithDiscounts { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether to ignore discounts (side-wide). It can significantly improve performance when enabled.
        /// </summary>
        public bool IgnoreDiscounts { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether to ignore featured tvchannels (side-wide). It can significantly improve performance when enabled.
        /// </summary>
        public bool IgnoreFeaturedTvChannels { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether to ignore ACL rules (side-wide). It can significantly improve performance when enabled.
        /// </summary>
        public bool IgnoreAcl { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether to ignore "limit per store" rules (side-wide). It can significantly improve performance when enabled.
        /// </summary>
        public bool IgnoreStoreLimitations { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether to cache tvchannel prices. It can significantly improve performance when enabled.
        /// </summary>
        public bool CacheTvChannelPrices { get; set; }

        /// <summary>
        /// Gets or sets a value indicating maximum number of 'back in stock' subscription
        /// </summary>
        public int MaximumBackInStockSubscriptions { get; set; }

        /// <summary>
        /// Gets or sets the value indicating how many manufacturers to display in manufacturers block
        /// </summary>
        public int ManufacturersBlockItemsToDisplay { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether to display information about shipping and tax in the footer (used in Germany)
        /// </summary>
        public bool DisplayTaxShippingInfoFooter { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether to display information about shipping and tax on tvchannel details pages (used in Germany)
        /// </summary>
        public bool DisplayTaxShippingInfoTvChannelDetailsPage { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether to display information about shipping and tax in tvchannel boxes (used in Germany)
        /// </summary>
        public bool DisplayTaxShippingInfoTvChannelBoxes { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether to display information about shipping and tax on shopping cart page (used in Germany)
        /// </summary>
        public bool DisplayTaxShippingInfoShoppingCart { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether to display information about shipping and tax on wishlist page (used in Germany)
        /// </summary>
        public bool DisplayTaxShippingInfoWishlist { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether to display information about shipping and tax on order details page (used in Germany)
        /// </summary>
        public bool DisplayTaxShippingInfoOrderDetailsPage { get; set; }

        /// <summary>
        /// Gets or sets the default value to use for Category page size options (for new categories)
        /// </summary>
        public string DefaultCategoryPageSizeOptions { get; set; }

        /// <summary>
        /// Gets or sets the default value to use for Category page size (for new categories)
        /// </summary>
        public int DefaultCategoryPageSize { get; set; }

        /// <summary>
        /// Gets or sets the default value to use for Manufacturer page size options (for new manufacturers)
        /// </summary>
        public string DefaultManufacturerPageSizeOptions { get; set; }

        /// <summary>
        /// Gets or sets the default value to use for Manufacturer page size (for new manufacturers)
        /// </summary>
        public int DefaultManufacturerPageSize { get; set; }

        /// <summary>
        /// Gets or sets a list of disabled values of TvChannelSortingEnum
        /// </summary>
        public List<int> TvChannelSortingEnumDisabled { get; set; }

        /// <summary>
        /// Gets or sets a display order of TvChannelSortingEnum values 
        /// </summary>
        public Dictionary<int, int> TvChannelSortingEnumDisplayOrder { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the tvchannels need to be exported/imported with their attributes
        /// </summary>
        public bool ExportImportTvChannelAttributes { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether need to use "limited to stores" property for exported/imported tvchannels
        /// </summary>
        public bool ExportImportTvChannelUseLimitedToStores { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the tvchannels need to be exported/imported with their specification attributes
        /// </summary>
        public bool ExportImportTvChannelSpecificationAttributes { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether need create dropdown list for export
        /// </summary>
        public bool ExportImportUseDropdownlistsForAssociatedEntities { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the tvchannels should be exported/imported with a full category name including names of all its parents
        /// </summary>
        public bool ExportImportTvChannelCategoryBreadcrumb { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the categories need to be exported/imported using name of category
        /// </summary>
        public bool ExportImportCategoriesUsingCategoryName { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the images can be downloaded from remote server
        /// </summary>
        public bool ExportImportAllowDownloadImages { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether tvchannels must be importing by separated files
        /// </summary>
        public bool ExportImportSplitTvChannelsFile { get; set; }

        /// <summary>
        /// Gets or sets a value of max tvchannels count in one file 
        /// </summary>
        public int ExportImportTvChannelsCountInOneFile { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether to remove required tvchannels from the cart if the main one is removed
        /// </summary>
        public bool RemoveRequiredTvChannels { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the related entities need to be exported/imported using name
        /// </summary>
        public bool ExportImportRelatedEntitiesByName { get; set; }

        /// <summary>
        /// Gets or sets count of displayed years for datepicker
        /// </summary>
        public int CountDisplayedYearsDatePicker { get; set; }

        /// <summary>
        /// Get or set a value indicating whether it's necessary to show the date for pre-order availability in a public store
        /// </summary>
        public bool DisplayDatePreOrderAvailability { get; set; }

        /// <summary>
        /// Get or set a value indicating whether use standart menu in public store or use Ajax to load menu
        /// </summary>
        public bool UseAjaxLoadMenu { get; set; }

        /// <summary>
        /// Get or set a value indicating whether use standart or AJAX tvchannels loading (applicable to 'paging', 'filtering', 'view modes') in catalog
        /// </summary>
        public bool UseAjaxCatalogTvChannelsLoading { get; set; }

        /// <summary>
        /// Get or set a value indicating whether the manufacturer filtering is enabled on catalog pages
        /// </summary>
        public bool EnableManufacturerFiltering { get; set; }

        /// <summary>
        /// Get or set a value indicating whether the price range filtering is enabled on catalog pages
        /// </summary>
        public bool EnablePriceRangeFiltering { get; set; }

        /// <summary>
        /// Get or set a value indicating whether the specification attribute filtering is enabled on catalog pages
        /// </summary>
        public bool EnableSpecificationAttributeFiltering { get; set; }

        /// <summary>
        /// Get or set a value indicating whether the "From" prices (based on price adjustments of combinations and attributes) are displayed on catalog pages
        /// </summary>
        public bool DisplayFromPrices { get; set; }

        /// <summary>
        /// Gets or sets the attribute value display type when out of stock
        /// </summary>
        public AttributeValueOutOfStockDisplayType AttributeValueOutOfStockDisplayType { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether user can search with manufacturer name
        /// </summary>
        public bool AllowUsersToSearchWithManufacturerName { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether user can search with category name
        /// </summary>
        public bool AllowUsersToSearchWithCategoryName { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether all pictures will be displayed on catalog pages
        /// </summary>
        public bool DisplayAllPicturesOnCatalogPages { get; set; }

        /// <summary>
        /// Gets or sets the identifier of tvchannel URL structure type (e.g. '/category-seo-name/tvchannel-seo-name' or '/tvchannel-seo-name')
        /// </summary>
        /// <remarks>We have TvChannelUrlStructureType enum, but we use int value here so that it can be overridden in third-party plugins</remarks>
        public int TvChannelUrlStructureTypeId { get; set; }

        /// <summary>
        /// Gets or sets an system name of active search provider
        /// </summary>
        public string ActiveSearchProviderSystemName { get; set; }
    }
}