using TvProgViewer.Web.Framework.Mvc.ModelBinding;
using TvProgViewer.Web.Framework.Models;

namespace TvProgViewer.WebUI.Areas.Admin.Models.Settings
{
    /// <summary>
    /// Represents a tvchannel editor settings model
    /// </summary>
    public partial record TvChannelEditorSettingsModel : BaseTvProgModel, ISettingsModel
    {
        #region Properties

        public int ActiveStoreScopeConfiguration { get; set; }

        [TvProgResourceDisplayName("Admin.Configuration.Settings.TvChannelEditor.TvChannelType")]
        public bool TvChannelType { get; set; }

        [TvProgResourceDisplayName("Admin.Configuration.Settings.TvChannelEditor.VisibleIndividually")]
        public bool VisibleIndividually { get; set; }

        [TvProgResourceDisplayName("Admin.Configuration.Settings.TvChannelEditor.TvChannelTemplate")]
        public bool TvChannelTemplate { get; set; }

        [TvProgResourceDisplayName("Admin.Configuration.Settings.TvChannelEditor.AdminComment")]
        public bool AdminComment { get; set; }

        [TvProgResourceDisplayName("Admin.Configuration.Settings.TvChannelEditor.Vendor")]
        public bool Vendor { get; set; }

        [TvProgResourceDisplayName("Admin.Configuration.Settings.TvChannelEditor.Stores")]
        public bool Stores { get; set; }

        [TvProgResourceDisplayName("Admin.Configuration.Settings.TvChannelEditor.ACL")]
        public bool ACL { get; set; }

        [TvProgResourceDisplayName("Admin.Configuration.Settings.TvChannelEditor.ShowOnHomepage")]
        public bool ShowOnHomepage { get; set; }

        [TvProgResourceDisplayName("Admin.Configuration.Settings.TvChannelEditor.AllowUserReviews")]
        public bool AllowUserReviews { get; set; }

        [TvProgResourceDisplayName("Admin.Configuration.Settings.TvChannelEditor.TvChannelTags")]
        public bool TvChannelTags { get; set; }

        [TvProgResourceDisplayName("Admin.Configuration.Settings.TvChannelEditor.ManufacturerPartNumber")]
        public bool ManufacturerPartNumber { get; set; }

        [TvProgResourceDisplayName("Admin.Configuration.Settings.TvChannelEditor.GTIN")]
        public bool GTIN { get; set; }

        [TvProgResourceDisplayName("Admin.Configuration.Settings.TvChannelEditor.TvChannelCost")]
        public bool TvChannelCost { get; set; }

        [TvProgResourceDisplayName("Admin.Configuration.Settings.TvChannelEditor.TierPrices")]
        public bool TierPrices { get; set; }

        [TvProgResourceDisplayName("Admin.Configuration.Settings.TvChannelEditor.Discounts")]
        public bool Discounts { get; set; }

        [TvProgResourceDisplayName("Admin.Configuration.Settings.TvChannelEditor.DisableBuyButton")]
        public bool DisableBuyButton { get; set; }

        [TvProgResourceDisplayName("Admin.Configuration.Settings.TvChannelEditor.DisableWishlistButton")]
        public bool DisableWishlistButton { get; set; }

        [TvProgResourceDisplayName("Admin.Configuration.Settings.TvChannelEditor.AvailableForPreOrder")]
        public bool AvailableForPreOrder { get; set; }

        [TvProgResourceDisplayName("Admin.Configuration.Settings.TvChannelEditor.CallForPrice")]
        public bool CallForPrice { get; set; }

        [TvProgResourceDisplayName("Admin.Configuration.Settings.TvChannelEditor.OldPrice")]
        public bool OldPrice { get; set; }

        [TvProgResourceDisplayName("Admin.Configuration.Settings.TvChannelEditor.UserEntersPrice")]
        public bool UserEntersPrice { get; set; }

        [TvProgResourceDisplayName("Admin.Configuration.Settings.TvChannelEditor.PAngV")]
        public bool PAngV { get; set; }

        [TvProgResourceDisplayName("Admin.Configuration.Settings.TvChannelEditor.RequireOtherTvChannelsAddedToCart")]
        public bool RequireOtherTvChannelsAddedToCart { get; set; }

        [TvProgResourceDisplayName("Admin.Configuration.Settings.TvChannelEditor.IsGiftCard")]
        public bool IsGiftCard { get; set; }

        [TvProgResourceDisplayName("Admin.Configuration.Settings.TvChannelEditor.DownloadableTvChannel")]
        public bool DownloadableTvChannel { get; set; }

        [TvProgResourceDisplayName("Admin.Configuration.Settings.TvChannelEditor.RecurringTvChannel")]
        public bool RecurringTvChannel { get; set; }

        [TvProgResourceDisplayName("Admin.Configuration.Settings.TvChannelEditor.IsRental")]
        public bool IsRental { get; set; }

        [TvProgResourceDisplayName("Admin.Configuration.Settings.TvChannelEditor.FreeShipping")]
        public bool FreeShipping { get; set; }

        [TvProgResourceDisplayName("Admin.Configuration.Settings.TvChannelEditor.ShipSeparately")]
        public bool ShipSeparately { get; set; }

        [TvProgResourceDisplayName("Admin.Configuration.Settings.TvChannelEditor.AdditionalShippingCharge")]
        public bool AdditionalShippingCharge { get; set; }

        [TvProgResourceDisplayName("Admin.Configuration.Settings.TvChannelEditor.DeliveryDate")]
        public bool DeliveryDate { get; set; }

        [TvProgResourceDisplayName("Admin.Configuration.Settings.TvChannelEditor.TelecommunicationsBroadcastingElectronicServices")]
        public bool TelecommunicationsBroadcastingElectronicServices { get; set; }

        [TvProgResourceDisplayName("Admin.Configuration.Settings.TvChannelEditor.TvChannelAvailabilityRange")]
        public bool TvChannelAvailabilityRange { get; set; }

        [TvProgResourceDisplayName("Admin.Configuration.Settings.TvChannelEditor.UseMultipleWarehouses")]
        public bool UseMultipleWarehouses { get; set; }

        [TvProgResourceDisplayName("Admin.Configuration.Settings.TvChannelEditor.Warehouse")]
        public bool Warehouse { get; set; }

        [TvProgResourceDisplayName("Admin.Configuration.Settings.TvChannelEditor.DisplayStockAvailability")]
        public bool DisplayStockAvailability { get; set; }

        [TvProgResourceDisplayName("Admin.Configuration.Settings.TvChannelEditor.MinimumStockQuantity")]
        public bool MinimumStockQuantity { get; set; }

        [TvProgResourceDisplayName("Admin.Configuration.Settings.TvChannelEditor.LowStockActivity")]
        public bool LowStockActivity { get; set; }

        [TvProgResourceDisplayName("Admin.Configuration.Settings.TvChannelEditor.NotifyAdminForQuantityBelow")]
        public bool NotifyAdminForQuantityBelow { get; set; }

        [TvProgResourceDisplayName("Admin.Configuration.Settings.TvChannelEditor.Backorders")]
        public bool Backorders { get; set; }

        [TvProgResourceDisplayName("Admin.Configuration.Settings.TvChannelEditor.AllowBackInStockSubscriptions")]
        public bool AllowBackInStockSubscriptions { get; set; }

        [TvProgResourceDisplayName("Admin.Configuration.Settings.TvChannelEditor.MinimumCartQuantity")]
        public bool MinimumCartQuantity { get; set; }

        [TvProgResourceDisplayName("Admin.Configuration.Settings.TvChannelEditor.MaximumCartQuantity")]
        public bool MaximumCartQuantity { get; set; }

        [TvProgResourceDisplayName("Admin.Configuration.Settings.TvChannelEditor.AllowedQuantities")]
        public bool AllowedQuantities { get; set; }

        [TvProgResourceDisplayName("Admin.Configuration.Settings.TvChannelEditor.AllowAddingOnlyExistingAttributeCombinations")]
        public bool AllowAddingOnlyExistingAttributeCombinations { get; set; }

        [TvProgResourceDisplayName("Admin.Configuration.Settings.TvChannelEditor.NotReturnable")]
        public bool NotReturnable { get; set; }

        [TvProgResourceDisplayName("Admin.Configuration.Settings.TvChannelEditor.Weight")]
        public bool Weight { get; set; }

        [TvProgResourceDisplayName("Admin.Configuration.Settings.TvChannelEditor.Dimensions")]
        public bool Dimensions { get; set; }

        [TvProgResourceDisplayName("Admin.Configuration.Settings.TvChannelEditor.AvailableStartDate")]
        public bool AvailableStartDate { get; set; }

        [TvProgResourceDisplayName("Admin.Configuration.Settings.TvChannelEditor.AvailableEndDate")]
        public bool AvailableEndDate { get; set; }

        [TvProgResourceDisplayName("Admin.Configuration.Settings.TvChannelEditor.MarkAsNew")]
        public bool MarkAsNew { get; set; }

        [TvProgResourceDisplayName("Admin.Configuration.Settings.TvChannelEditor.Published")]
        public bool Published { get; set; }
        
        [TvProgResourceDisplayName("Admin.Configuration.Settings.TvChannelEditor.RelatedTvChannels")]
        public bool RelatedTvChannels { get; set; }

        [TvProgResourceDisplayName("Admin.Configuration.Settings.TvChannelEditor.CrossSellsTvChannels")]
        public bool CrossSellsTvChannels { get; set; }

        [TvProgResourceDisplayName("Admin.Configuration.Settings.TvChannelEditor.Seo")]
        public bool Seo { get; set; }

        [TvProgResourceDisplayName("Admin.Configuration.Settings.TvChannelEditor.PurchasedWithOrders")]
        public bool PurchasedWithOrders { get; set; }
       
        [TvProgResourceDisplayName("Admin.Configuration.Settings.TvChannelEditor.TvChannelAttributes")]
        public bool TvChannelAttributes { get; set; }

        [TvProgResourceDisplayName("Admin.Configuration.Settings.TvChannelEditor.SpecificationAttributes")]
        public bool SpecificationAttributes { get; set; }

        [TvProgResourceDisplayName("Admin.Configuration.Settings.TvChannelEditor.Manufacturers")]
        public bool Manufacturers { get; set; }

        [TvProgResourceDisplayName("Admin.Configuration.Settings.TvChannelEditor.StockQuantityHistory")]
        public bool StockQuantityHistory { get; set; }

        #endregion
    }
}