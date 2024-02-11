using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc.Rendering;
using TvProgViewer.WebUI.Areas.Admin.Models.Catalog;
using TvProgViewer.Web.Framework.Models;
using TvProgViewer.Web.Framework.Mvc.ModelBinding;

namespace TvProgViewer.WebUI.Areas.Admin.Models.Settings
{
    /// <summary>
    /// Represents a catalog settings model
    /// </summary>
    public partial record CatalogSettingsModel : BaseTvProgModel, ISettingsModel
    {
        #region Ctor

        public CatalogSettingsModel()
        {
            AvailableViewModes = new List<SelectListItem>();
            SortOptionSearchModel = new SortOptionSearchModel();
            ReviewTypeSearchModel = new ReviewTypeSearchModel();
        }

        #endregion

        #region Properties

        public int ActiveStoreScopeConfiguration { get; set; }

        [TvProgResourceDisplayName("Admin.Configuration.Settings.Catalog.AllowViewUnpublishedTvChannelPage")]
        public bool AllowViewUnpublishedTvChannelPage { get; set; }
        public bool AllowViewUnpublishedTvChannelPage_OverrideForStore { get; set; }

        [TvProgResourceDisplayName("Admin.Configuration.Settings.Catalog.DisplayDiscontinuedMessageForUnpublishedTvChannels")]
        public bool DisplayDiscontinuedMessageForUnpublishedTvChannels { get; set; }
        public bool DisplayDiscontinuedMessageForUnpublishedTvChannels_OverrideForStore { get; set; }

        [TvProgResourceDisplayName("Admin.Configuration.Settings.Catalog.ShowSkuOnTvChannelDetailsPage")]
        public bool ShowSkuOnTvChannelDetailsPage { get; set; }
        public bool ShowSkuOnTvChannelDetailsPage_OverrideForStore { get; set; }

        [TvProgResourceDisplayName("Admin.Configuration.Settings.Catalog.ShowSkuOnCatalogPages")]
        public bool ShowSkuOnCatalogPages { get; set; }
        public bool ShowSkuOnCatalogPages_OverrideForStore { get; set; }

        [TvProgResourceDisplayName("Admin.Configuration.Settings.Catalog.ShowManufacturerPartNumber")]
        public bool ShowManufacturerPartNumber { get; set; }
        public bool ShowManufacturerPartNumber_OverrideForStore { get; set; }

        [TvProgResourceDisplayName("Admin.Configuration.Settings.Catalog.ShowGtin")]
        public bool ShowGtin { get; set; }
        public bool ShowGtin_OverrideForStore { get; set; }

        [TvProgResourceDisplayName("Admin.Configuration.Settings.Catalog.ShowFreeShippingNotification")]
        public bool ShowFreeShippingNotification { get; set; }
        public bool ShowFreeShippingNotification_OverrideForStore { get; set; }

        [TvProgResourceDisplayName("Admin.Configuration.Settings.Catalog.ShowShortDescriptionOnCatalogPages")]
        public bool ShowShortDescriptionOnCatalogPages { get; set; }
        public bool ShowShortDescriptionOnCatalogPages_OverrideForStore { get; set; }

        [TvProgResourceDisplayName("Admin.Configuration.Settings.Catalog.AllowTvChannelSorting")]
        public bool AllowTvChannelSorting { get; set; }
        public bool AllowTvChannelSorting_OverrideForStore { get; set; }

        [TvProgResourceDisplayName("Admin.Configuration.Settings.Catalog.AllowTvChannelViewModeChanging")]
        public bool AllowTvChannelViewModeChanging { get; set; }
        public bool AllowTvChannelViewModeChanging_OverrideForStore { get; set; }

        [TvProgResourceDisplayName("Admin.Configuration.Settings.Catalog.DefaultViewMode")]
        public string DefaultViewMode { get; set; }
        public bool DefaultViewMode_OverrideForStore { get; set; }
        public IList<SelectListItem> AvailableViewModes { get; set; }

        [TvProgResourceDisplayName("Admin.Configuration.Settings.Catalog.ShowTvChannelsFromSubcategories")]
        public bool ShowTvChannelsFromSubcategories { get; set; }
        public bool ShowTvChannelsFromSubcategories_OverrideForStore { get; set; }

        [TvProgResourceDisplayName("Admin.Configuration.Settings.Catalog.ShowCategoryTvChannelNumber")]
        public bool ShowCategoryTvChannelNumber { get; set; }
        public bool ShowCategoryTvChannelNumber_OverrideForStore { get; set; }

        [TvProgResourceDisplayName("Admin.Configuration.Settings.Catalog.ShowCategoryTvChannelNumberIncludingSubcategories")]
        public bool ShowCategoryTvChannelNumberIncludingSubcategories { get; set; }
        public bool ShowCategoryTvChannelNumberIncludingSubcategories_OverrideForStore { get; set; }

        [TvProgResourceDisplayName("Admin.Configuration.Settings.Catalog.CategoryBreadcrumbEnabled")]
        public bool CategoryBreadcrumbEnabled { get; set; }
        public bool CategoryBreadcrumbEnabled_OverrideForStore { get; set; }

        [TvProgResourceDisplayName("Admin.Configuration.Settings.Catalog.ShowShareButton")]
        public bool ShowShareButton { get; set; }
        public bool ShowShareButton_OverrideForStore { get; set; }

        [TvProgResourceDisplayName("Admin.Configuration.Settings.Catalog.PageShareCode")]
        public string PageShareCode { get; set; }
        public bool PageShareCode_OverrideForStore { get; set; }

        [TvProgResourceDisplayName("Admin.Configuration.Settings.Catalog.TvChannelReviewsMustBeApproved")]
        public bool TvChannelReviewsMustBeApproved { get; set; }
        public bool TvChannelReviewsMustBeApproved_OverrideForStore { get; set; }

        [TvProgResourceDisplayName("Admin.Configuration.Settings.Catalog.OneReviewPerTvChannelFromUser")]
        public bool OneReviewPerTvChannelFromUser { get; set; }
        public bool OneReviewPerTvChannelFromUser_OverrideForStore { get; set; }

        [TvProgResourceDisplayName("Admin.Configuration.Settings.Catalog.AllowAnonymousUsersToReviewTvChannel")]
        public bool AllowAnonymousUsersToReviewTvChannel { get; set; }
        public bool AllowAnonymousUsersToReviewTvChannel_OverrideForStore { get; set; }

        [TvProgResourceDisplayName("Admin.Configuration.Settings.Catalog.TvChannelReviewPossibleOnlyAfterPurchasing")]
        public bool TvChannelReviewPossibleOnlyAfterPurchasing { get; set; }
        public bool TvChannelReviewPossibleOnlyAfterPurchasing_OverrideForStore { get; set; }

        [TvProgResourceDisplayName("Admin.Configuration.Settings.Catalog.NotifyStoreOwnerAboutNewTvChannelReviews")]
        public bool NotifyStoreOwnerAboutNewTvChannelReviews { get; set; }
        public bool NotifyStoreOwnerAboutNewTvChannelReviews_OverrideForStore { get; set; }

        [TvProgResourceDisplayName("Admin.Configuration.Settings.Catalog.NotifyUserAboutTvChannelReviewReply")]
        public bool NotifyUserAboutTvChannelReviewReply { get; set; }
        public bool NotifyUserAboutTvChannelReviewReply_OverrideForStore { get; set; }

        [TvProgResourceDisplayName("Admin.Configuration.Settings.Catalog.ShowTvChannelReviewsPerStore")]
        public bool ShowTvChannelReviewsPerStore { get; set; }
        public bool ShowTvChannelReviewsPerStore_OverrideForStore { get; set; }

        [TvProgResourceDisplayName("Admin.Configuration.Settings.Catalog.ShowTvChannelReviewsTabOnAccountPage")]
        public bool ShowTvChannelReviewsTabOnAccountPage { get; set; }
        public bool ShowTvChannelReviewsOnAccountPage_OverrideForStore { get; set; }

        [TvProgResourceDisplayName("Admin.Configuration.Settings.Catalog.TvChannelReviewsPageSizeOnAccountPage")]
        public int TvChannelReviewsPageSizeOnAccountPage { get; set; }
        public bool TvChannelReviewsPageSizeOnAccountPage_OverrideForStore { get; set; }

        [TvProgResourceDisplayName("Admin.Configuration.Settings.Catalog.TvChannelReviewsSortByCreatedDateAscending")]
        public bool TvChannelReviewsSortByCreatedDateAscending { get; set; }
        public bool TvChannelReviewsSortByCreatedDateAscending_OverrideForStore { get; set; }

        [TvProgResourceDisplayName("Admin.Configuration.Settings.Catalog.EmailAFriendEnabled")]
        public bool EmailAFriendEnabled { get; set; }
        public bool EmailAFriendEnabled_OverrideForStore { get; set; }

        [TvProgResourceDisplayName("Admin.Configuration.Settings.Catalog.AllowAnonymousUsersToEmailAFriend")]
        public bool AllowAnonymousUsersToEmailAFriend { get; set; }
        public bool AllowAnonymousUsersToEmailAFriend_OverrideForStore { get; set; }

        [TvProgResourceDisplayName("Admin.Configuration.Settings.Catalog.RecentlyViewedTvChannelsNumber")]
        public int RecentlyViewedTvChannelsNumber { get; set; }
        public bool RecentlyViewedTvChannelsNumber_OverrideForStore { get; set; }

        [TvProgResourceDisplayName("Admin.Configuration.Settings.Catalog.RecentlyViewedTvChannelsEnabled")]
        public bool RecentlyViewedTvChannelsEnabled { get; set; }
        public bool RecentlyViewedTvChannelsEnabled_OverrideForStore { get; set; }

        [TvProgResourceDisplayName("Admin.Configuration.Settings.Catalog.NewTvChannelsEnabled")]
        public bool NewTvChannelsEnabled { get; set; }
        public bool NewTvChannelsEnabled_OverrideForStore { get; set; }

        [TvProgResourceDisplayName("Admin.Configuration.Settings.Catalog.NewTvChannelsPageSize")]
        public int NewTvChannelsPageSize { get; set; }
        public bool NewTvChannelsPageSize_OverrideForStore { get; set; }

        [TvProgResourceDisplayName("Admin.Configuration.Settings.Catalog.NewTvChannelsAllowUsersToSelectPageSize")]
        public bool NewTvChannelsAllowUsersToSelectPageSize { get; set; }
        public bool NewTvChannelsAllowUsersToSelectPageSize_OverrideForStore { get; set; }

        [TvProgResourceDisplayName("Admin.Configuration.Settings.Catalog.NewTvChannelsPageSizeOptions")]
        public string NewTvChannelsPageSizeOptions { get; set; }
        public bool NewTvChannelsPageSizeOptions_OverrideForStore { get; set; }

        [TvProgResourceDisplayName("Admin.Configuration.Settings.Catalog.CompareTvChannelsEnabled")]
        public bool CompareTvChannelsEnabled { get; set; }
        public bool CompareTvChannelsEnabled_OverrideForStore { get; set; }

        [TvProgResourceDisplayName("Admin.Configuration.Settings.Catalog.ShowBestsellersOnHomepage")]
        public bool ShowBestsellersOnHomepage { get; set; }
        public bool ShowBestsellersOnHomepage_OverrideForStore { get; set; }

        [TvProgResourceDisplayName("Admin.Configuration.Settings.Catalog.NumberOfBestsellersOnHomepage")]
        public int NumberOfBestsellersOnHomepage { get; set; }
        public bool NumberOfBestsellersOnHomepage_OverrideForStore { get; set; }

        [TvProgResourceDisplayName("Admin.Configuration.Settings.Catalog.SearchPageTvChannelsPerPage")]
        public int SearchPageTvChannelsPerPage { get; set; }
        public bool SearchPageTvChannelsPerPage_OverrideForStore { get; set; }

        [TvProgResourceDisplayName("Admin.Configuration.Settings.Catalog.SearchPageAllowUsersToSelectPageSize")]
        public bool SearchPageAllowUsersToSelectPageSize { get; set; }
        public bool SearchPageAllowUsersToSelectPageSize_OverrideForStore { get; set; }

        [TvProgResourceDisplayName("Admin.Configuration.Settings.Catalog.SearchPagePageSizeOptions")]
        public string SearchPagePageSizeOptions { get; set; }
        public bool SearchPagePageSizeOptions_OverrideForStore { get; set; }

        [TvProgResourceDisplayName("Admin.Configuration.Settings.Catalog.SearchPagePriceRangeFiltering")]
        public bool SearchPagePriceRangeFiltering { get; set; }
        public bool SearchPagePriceRangeFiltering_OverrideForStore { get; set; }

        [TvProgResourceDisplayName("Admin.Configuration.Settings.Catalog.SearchPagePriceFrom")]
        public decimal SearchPagePriceFrom { get; set; }
        public bool SearchPagePriceFrom_OverrideForStore { get; set; }

        [TvProgResourceDisplayName("Admin.Configuration.Settings.Catalog.SearchPagePriceTo")]
        public decimal SearchPagePriceTo { get; set; }
        public bool SearchPagePriceTo_OverrideForStore { get; set; }

        [TvProgResourceDisplayName("Admin.Configuration.Settings.Catalog.SearchPageManuallyPriceRange")]
        public bool SearchPageManuallyPriceRange { get; set; }
        public bool SearchPageManuallyPriceRange_OverrideForStore { get; set; }

        [TvProgResourceDisplayName("Admin.Configuration.Settings.Catalog.TvChannelSearchAutoCompleteEnabled")]
        public bool TvChannelSearchAutoCompleteEnabled { get; set; }
        public bool TvChannelSearchAutoCompleteEnabled_OverrideForStore { get; set; }

        [TvProgResourceDisplayName("Admin.Configuration.Settings.Catalog.TvChannelSearchEnabled")]
        public bool TvChannelSearchEnabled { get; set; }
        public bool TvChannelSearchEnabled_OverrideForStore { get; set; }

        [TvProgResourceDisplayName("Admin.Configuration.Settings.Catalog.TvChannelSearchAutoCompleteNumberOfTvChannels")]
        public int TvChannelSearchAutoCompleteNumberOfTvChannels { get; set; }
        public bool TvChannelSearchAutoCompleteNumberOfTvChannels_OverrideForStore { get; set; }

        [TvProgResourceDisplayName("Admin.Configuration.Settings.Catalog.ShowTvChannelImagesInSearchAutoComplete")]
        public bool ShowTvChannelImagesInSearchAutoComplete { get; set; }
        public bool ShowTvChannelImagesInSearchAutoComplete_OverrideForStore { get; set; }

        [TvProgResourceDisplayName("Admin.Configuration.Settings.Catalog.ShowLinkToAllResultInSearchAutoComplete")]
        public bool ShowLinkToAllResultInSearchAutoComplete { get; set; }
        public bool ShowLinkToAllResultInSearchAutoComplete_OverrideForStore { get; set; }

        [TvProgResourceDisplayName("Admin.Configuration.Settings.Catalog.TvChannelSearchTermMinimumLength")]
        public int TvChannelSearchTermMinimumLength { get; set; }
        public bool TvChannelSearchTermMinimumLength_OverrideForStore { get; set; }

        [TvProgResourceDisplayName("Admin.Configuration.Settings.Catalog.TvChannelsAlsoPurchasedEnabled")]
        public bool TvChannelsAlsoPurchasedEnabled { get; set; }
        public bool TvChannelsAlsoPurchasedEnabled_OverrideForStore { get; set; }

        [TvProgResourceDisplayName("Admin.Configuration.Settings.Catalog.TvChannelsAlsoPurchasedNumber")]
        public int TvChannelsAlsoPurchasedNumber { get; set; }
        public bool TvChannelsAlsoPurchasedNumber_OverrideForStore { get; set; }

        [TvProgResourceDisplayName("Admin.Configuration.Settings.Catalog.NumberOfTvChannelTags")]
        public int NumberOfTvChannelTags { get; set; }
        public bool NumberOfTvChannelTags_OverrideForStore { get; set; }

        [TvProgResourceDisplayName("Admin.Configuration.Settings.Catalog.TvChannelsByTagPageSize")]
        public int TvChannelsByTagPageSize { get; set; }
        public bool TvChannelsByTagPageSize_OverrideForStore { get; set; }

        [TvProgResourceDisplayName("Admin.Configuration.Settings.Catalog.TvChannelsByTagAllowUsersToSelectPageSize")]
        public bool TvChannelsByTagAllowUsersToSelectPageSize { get; set; }
        public bool TvChannelsByTagAllowUsersToSelectPageSize_OverrideForStore { get; set; }

        [TvProgResourceDisplayName("Admin.Configuration.Settings.Catalog.TvChannelsByTagPageSizeOptions")]
        public string TvChannelsByTagPageSizeOptions { get; set; }
        public bool TvChannelsByTagPageSizeOptions_OverrideForStore { get; set; }

        [TvProgResourceDisplayName("Admin.Configuration.Settings.Catalog.TvChannelsByTagPriceRangeFiltering")]
        public bool TvChannelsByTagPriceRangeFiltering { get; set; }
        public bool TvChannelsByTagPriceRangeFiltering_OverrideForStore { get; set; }

        [TvProgResourceDisplayName("Admin.Configuration.Settings.Catalog.TvChannelsByTagPriceFrom")]
        public decimal TvChannelsByTagPriceFrom { get; set; }
        public bool TvChannelsByTagPriceFrom_OverrideForStore { get; set; }

        [TvProgResourceDisplayName("Admin.Configuration.Settings.Catalog.TvChannelsByTagPriceTo")]
        public decimal TvChannelsByTagPriceTo { get; set; }
        public bool TvChannelsByTagPriceTo_OverrideForStore { get; set; }

        [TvProgResourceDisplayName("Admin.Configuration.Settings.Catalog.TvChannelsByTagManuallyPriceRange")]
        public bool TvChannelsByTagManuallyPriceRange { get; set; }
        public bool TvChannelsByTagManuallyPriceRange_OverrideForStore { get; set; }

        [TvProgResourceDisplayName("Admin.Configuration.Settings.Catalog.IncludeShortDescriptionInCompareTvChannels")]
        public bool IncludeShortDescriptionInCompareTvChannels { get; set; }
        public bool IncludeShortDescriptionInCompareTvChannels_OverrideForStore { get; set; }

        [TvProgResourceDisplayName("Admin.Configuration.Settings.Catalog.IncludeFullDescriptionInCompareTvChannels")]
        public bool IncludeFullDescriptionInCompareTvChannels { get; set; }
        public bool IncludeFullDescriptionInCompareTvChannels_OverrideForStore { get; set; }

        [TvProgResourceDisplayName("Admin.Configuration.Settings.Catalog.ManufacturersBlockItemsToDisplay")]
        public int ManufacturersBlockItemsToDisplay { get; set; }
        public bool ManufacturersBlockItemsToDisplay_OverrideForStore { get; set; }

        [TvProgResourceDisplayName("Admin.Configuration.Settings.Catalog.DisplayTaxShippingInfoFooter")]
        public bool DisplayTaxShippingInfoFooter { get; set; }
        public bool DisplayTaxShippingInfoFooter_OverrideForStore { get; set; }

        [TvProgResourceDisplayName("Admin.Configuration.Settings.Catalog.DisplayTaxShippingInfoTvChannelDetailsPage")]
        public bool DisplayTaxShippingInfoTvChannelDetailsPage { get; set; }
        public bool DisplayTaxShippingInfoTvChannelDetailsPage_OverrideForStore { get; set; }

        [TvProgResourceDisplayName("Admin.Configuration.Settings.Catalog.DisplayTaxShippingInfoTvChannelBoxes")]
        public bool DisplayTaxShippingInfoTvChannelBoxes { get; set; }
        public bool DisplayTaxShippingInfoTvChannelBoxes_OverrideForStore { get; set; }

        [TvProgResourceDisplayName("Admin.Configuration.Settings.Catalog.DisplayTaxShippingInfoShoppingCart")]
        public bool DisplayTaxShippingInfoShoppingCart { get; set; }
        public bool DisplayTaxShippingInfoShoppingCart_OverrideForStore { get; set; }

        [TvProgResourceDisplayName("Admin.Configuration.Settings.Catalog.DisplayTaxShippingInfoWishlist")]
        public bool DisplayTaxShippingInfoWishlist { get; set; }
        public bool DisplayTaxShippingInfoWishlist_OverrideForStore { get; set; }

        [TvProgResourceDisplayName("Admin.Configuration.Settings.Catalog.DisplayTaxShippingInfoOrderDetailsPage")]
        public bool DisplayTaxShippingInfoOrderDetailsPage { get; set; }
        public bool DisplayTaxShippingInfoOrderDetailsPage_OverrideForStore { get; set; }

        [TvProgResourceDisplayName("Admin.Configuration.Settings.Catalog.ExportImportTvChannelAttributes")]
        public bool ExportImportTvChannelAttributes { get; set; }
        public bool ExportImportTvChannelAttributes_OverrideForStore { get; set; }

        [TvProgResourceDisplayName("Admin.Configuration.Settings.Catalog.ExportImportTvChannelSpecificationAttributes")]
        public bool ExportImportTvChannelSpecificationAttributes { get; set; }
        public bool ExportImportTvChannelSpecificationAttributes_OverrideForStore { get; set; }

        [TvProgResourceDisplayName("Admin.Configuration.Settings.Catalog.ExportImportTvChannelCategoryBreadcrumb")]
        public bool ExportImportTvChannelCategoryBreadcrumb { get; set; }
        public bool ExportImportTvChannelCategoryBreadcrumb_OverrideForStore { get; set; }

        [TvProgResourceDisplayName("Admin.Configuration.Settings.Catalog.ExportImportCategoriesUsingCategoryName")]
        public bool ExportImportCategoriesUsingCategoryName { get; set; }
        public bool ExportImportCategoriesUsingCategoryName_OverrideForStore { get; set; }

        [TvProgResourceDisplayName("Admin.Configuration.Settings.Catalog.ExportImportAllowDownloadImages")]
        public bool ExportImportAllowDownloadImages { get; set; }
        public bool ExportImportAllowDownloadImages_OverrideForStore { get; set; }

        [TvProgResourceDisplayName("Admin.Configuration.Settings.Catalog.ExportImportSplitTvChannelsFile")]
        public bool ExportImportSplitTvChannelsFile { get; set; }
        public bool ExportImportSplitTvChannelsFile_OverrideForStore { get; set; }

        [TvProgResourceDisplayName("Admin.Configuration.Settings.Catalog.RemoveRequiredTvChannels")]
        public bool RemoveRequiredTvChannels { get; set; }
        public bool RemoveRequiredTvChannels_OverrideForStore { get; set; }

        [TvProgResourceDisplayName("Admin.Configuration.Settings.Catalog.ExportImportRelatedEntitiesByName")]
        public bool ExportImportRelatedEntitiesByName { get; set; }
        public bool ExportImportRelatedEntitiesByName_OverrideForStore { get; set; }

        [TvProgResourceDisplayName("Admin.Configuration.Settings.Catalog.ExportImportTvChannelUseLimitedToStores")]
        public bool ExportImportTvChannelUseLimitedToStores { get; set; }
        public bool ExportImportTvChannelUseLimitedToStores_OverrideForStore { get; set; }

        [TvProgResourceDisplayName("Admin.Configuration.Settings.Catalog.IgnoreDiscounts")]
        public bool IgnoreDiscounts { get; set; }

        [TvProgResourceDisplayName("Admin.Configuration.Settings.Catalog.IgnoreFeaturedTvChannels")]
        public bool IgnoreFeaturedTvChannels { get; set; }

        [TvProgResourceDisplayName("Admin.Configuration.Settings.Catalog.IgnoreAcl")]
        public bool IgnoreAcl { get; set; }

        [TvProgResourceDisplayName("Admin.Configuration.Settings.Catalog.IgnoreStoreLimitations")]
        public bool IgnoreStoreLimitations { get; set; }

        [TvProgResourceDisplayName("Admin.Configuration.Settings.Catalog.CacheTvChannelPrices")]
        public bool CacheTvChannelPrices { get; set; }

        [TvProgResourceDisplayName("Admin.Configuration.Settings.Catalog.DisplayDatePreOrderAvailability")]
        public bool DisplayDatePreOrderAvailability { get; set; }
        public bool DisplayDatePreOrderAvailability_OverrideForStore { get; set; }

        [TvProgResourceDisplayName("Admin.Configuration.Settings.Catalog.AttributeValueOutOfStockDisplayType")]
        public int AttributeValueOutOfStockDisplayType { get; set; }
        public bool AttributeValueOutOfStockDisplayType_OverrideForStore { get; set; }
        public SelectList AttributeValueOutOfStockDisplayTypes { get; set; }

        public SortOptionSearchModel SortOptionSearchModel { get; set; }

        public ReviewTypeSearchModel ReviewTypeSearchModel { get; set; }

        [TvProgResourceDisplayName("Admin.Configuration.Settings.Catalog.UseAjaxCatalogTvChannelsLoading")]
        public bool UseAjaxCatalogTvChannelsLoading { get; set; }
        public bool UseAjaxCatalogTvChannelsLoading_OverrideForStore { get; set; }

        [TvProgResourceDisplayName("Admin.Configuration.Settings.Catalog.EnableManufacturerFiltering")]
        public bool EnableManufacturerFiltering { get; set; }
        public bool EnableManufacturerFiltering_OverrideForStore { get; set; }

        [TvProgResourceDisplayName("Admin.Configuration.Settings.Catalog.EnablePriceRangeFiltering")]
        public bool EnablePriceRangeFiltering { get; set; }
        public bool EnablePriceRangeFiltering_OverrideForStore { get; set; }

        [TvProgResourceDisplayName("Admin.Configuration.Settings.Catalog.EnableSpecificationAttributeFiltering")]
        public bool EnableSpecificationAttributeFiltering { get; set; }
        public bool EnableSpecificationAttributeFiltering_OverrideForStore { get; set; }

        [TvProgResourceDisplayName("Admin.Configuration.Settings.Catalog.DisplayFromPrices")]
        public bool DisplayFromPrices { get; set; }
        public bool DisplayFromPrices_OverrideForStore { get; set; }

        public string PrimaryStoreCurrencyCode { get; set; }

        [TvProgResourceDisplayName("Admin.Configuration.Settings.Catalog.AllowUsersToSearchWithManufacturerName")]
        public bool AllowUsersToSearchWithManufacturerName { get; set; }
        public bool AllowUsersToSearchWithManufacturerName_OverrideForStore { get; set; }

        [TvProgResourceDisplayName("Admin.Configuration.Settings.Catalog.AllowUsersToSearchWithCategoryName")]
        public bool AllowUsersToSearchWithCategoryName { get; set; }
        public bool AllowUsersToSearchWithCategoryName_OverrideForStore { get; set; }

        [TvProgResourceDisplayName("Admin.Configuration.Settings.Catalog.DisplayAllPicturesOnCatalogPages")]
        public bool DisplayAllPicturesOnCatalogPages { get; set; }
        public bool DisplayAllPicturesOnCatalogPages_OverrideForStore { get; set; }

        [TvProgResourceDisplayName("Admin.Configuration.Settings.Catalog.TvChannelUrlStructureType")]
        public int TvChannelUrlStructureTypeId { get; set; }
        public bool TvChannelUrlStructureTypeId_OverrideForStore { get; set; }
        public SelectList TvChannelUrlStructureTypes { get; set; }

        #endregion
    }
}