using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;
using TvProgViewer.WebUI.Areas.Admin.Models.Settings;
using TvProgViewer.Web.Framework.Models;
using TvProgViewer.Web.Framework.Mvc.ModelBinding;

namespace TvProgViewer.WebUI.Areas.Admin.Models.Catalog
{
    /// <summary>
    /// Represents a tvchannel model
    /// </summary>
    public partial record TvChannelModel : BaseTvProgEntityModel, 
        IAclSupportedModel, IDiscountSupportedModel, ILocalizedModel<TvChannelLocalizedModel>, IStoreMappingSupportedModel
    {
        #region Ctor

        public TvChannelModel()
        {
            TvChannelPictureModels = new List<TvChannelPictureModel>();
            TvChannelVideoModels = new List<TvChannelVideoModel>();
            Locales = new List<TvChannelLocalizedModel>();
            CopyTvChannelModel = new CopyTvChannelModel();
            AddPictureModel = new TvChannelPictureModel();
            AddVideoModel = new TvChannelVideoModel();
            TvChannelWarehouseInventoryModels = new List<TvChannelWarehouseInventoryModel>();
            TvChannelEditorSettingsModel = new TvChannelEditorSettingsModel();
            StockQuantityHistory = new StockQuantityHistoryModel();

            AvailableBasepriceUnits = new List<SelectListItem>();
            AvailableBasepriceBaseUnits = new List<SelectListItem>();
            AvailableTvChannelTemplates = new List<SelectListItem>();
            AvailableTaxCategories = new List<SelectListItem>();
            AvailableDeliveryDates = new List<SelectListItem>();
            AvailableTvChannelAvailabilityRanges = new List<SelectListItem>();
            AvailableWarehouses = new List<SelectListItem>();
            TvChannelsTypesSupportedByTvChannelTemplates = new Dictionary<int, IList<SelectListItem>>();

            AvailableVendors = new List<SelectListItem>();

            SelectedStoreIds = new List<int>();
            AvailableStores = new List<SelectListItem>();

            SelectedManufacturerIds = new List<int>();
            AvailableManufacturers = new List<SelectListItem>();

            SelectedCategoryIds = new List<int>();
            AvailableCategories = new List<SelectListItem>();

            SelectedUserRoleIds = new List<int>();
            AvailableUserRoles = new List<SelectListItem>();

            SelectedDiscountIds = new List<int>();
            AvailableDiscounts = new List<SelectListItem>();

            RelatedTvChannelSearchModel = new RelatedTvChannelSearchModel();
            CrossSellTvChannelSearchModel = new CrossSellTvChannelSearchModel();
            AssociatedTvChannelSearchModel = new AssociatedTvChannelSearchModel();
            TvChannelPictureSearchModel = new TvChannelPictureSearchModel();
            TvChannelVideoSearchModel = new TvChannelVideoSearchModel();
            TvChannelSpecificationAttributeSearchModel = new TvChannelSpecificationAttributeSearchModel();
            TvChannelOrderSearchModel = new TvChannelOrderSearchModel();
            TierPriceSearchModel = new TierPriceSearchModel();
            StockQuantityHistorySearchModel = new StockQuantityHistorySearchModel();
            TvChannelAttributeMappingSearchModel = new TvChannelAttributeMappingSearchModel();
            TvChannelAttributeCombinationSearchModel = new TvChannelAttributeCombinationSearchModel();
        }

        #endregion

        #region Properties
        
        //picture thumbnail
        [TvProgResourceDisplayName("Admin.Catalog.TvChannels.Fields.PictureThumbnailUrl")]
        public string PictureThumbnailUrl { get; set; }

        [TvProgResourceDisplayName("Admin.Catalog.TvChannels.Fields.TvChannelType")]
        public int TvChannelTypeId { get; set; }

        [TvProgResourceDisplayName("Admin.Catalog.TvChannels.Fields.TvChannelType")]
        public string TvChannelTypeName { get; set; }

        [TvProgResourceDisplayName("Admin.Catalog.TvChannels.Fields.AssociatedToTvChannelName")]
        public int AssociatedToTvChannelId { get; set; }

        [TvProgResourceDisplayName("Admin.Catalog.TvChannels.Fields.AssociatedToTvChannelName")]
        public string AssociatedToTvChannelName { get; set; }

        [TvProgResourceDisplayName("Admin.Catalog.TvChannels.Fields.VisibleIndividually")]
        public bool VisibleIndividually { get; set; }

        [TvProgResourceDisplayName("Admin.Catalog.TvChannels.Fields.TvChannelTemplate")]
        public int TvChannelTemplateId { get; set; }
        public IList<SelectListItem> AvailableTvChannelTemplates { get; set; }

        //<tvchannel type ID, list of supported tvchannel template IDs>
        public Dictionary<int, IList<SelectListItem>> TvChannelsTypesSupportedByTvChannelTemplates { get; set; }

        [TvProgResourceDisplayName("Admin.Catalog.TvChannels.Fields.Name")]
        public string Name { get; set; }

        [TvProgResourceDisplayName("Admin.Catalog.TvChannels.Fields.ShortDescription")]
        public string ShortDescription { get; set; }

        [TvProgResourceDisplayName("Admin.Catalog.TvChannels.Fields.FullDescription")]
        public string FullDescription { get; set; }

        [TvProgResourceDisplayName("Admin.Catalog.TvChannels.Fields.AdminComment")]
        public string AdminComment { get; set; }

        [TvProgResourceDisplayName("Admin.Catalog.TvChannels.Fields.ShowOnHomepage")]
        public bool ShowOnHomepage { get; set; }

        [TvProgResourceDisplayName("Admin.Catalog.TvChannels.Fields.MetaKeywords")]
        public string MetaKeywords { get; set; }

        [TvProgResourceDisplayName("Admin.Catalog.TvChannels.Fields.MetaDescription")]
        public string MetaDescription { get; set; }

        [TvProgResourceDisplayName("Admin.Catalog.TvChannels.Fields.MetaTitle")]
        public string MetaTitle { get; set; }

        [TvProgResourceDisplayName("Admin.Catalog.TvChannels.Fields.SeName")]
        public string SeName { get; set; }

        [TvProgResourceDisplayName("Admin.Catalog.TvChannels.Fields.AllowUserReviews")]
        public bool AllowUserReviews { get; set; }

        [TvProgResourceDisplayName("Admin.Catalog.TvChannels.Fields.TvChannelTags")]
        public string TvChannelTags { get; set; }

        public string InitialTvChannelTags { get; set; }

        [TvProgResourceDisplayName("Admin.Catalog.TvChannels.Fields.Sku")]
        public string Sku { get; set; }

        [TvProgResourceDisplayName("Admin.Catalog.TvChannels.Fields.ManufacturerPartNumber")]
        public string ManufacturerPartNumber { get; set; }

        [TvProgResourceDisplayName("Admin.Catalog.TvChannels.Fields.GTIN")]
        public virtual string Gtin { get; set; }

        [TvProgResourceDisplayName("Admin.Catalog.TvChannels.Fields.IsGiftCard")]
        public bool IsGiftCard { get; set; }

        [TvProgResourceDisplayName("Admin.Catalog.TvChannels.Fields.GiftCardType")]
        public int GiftCardTypeId { get; set; }

        [TvProgResourceDisplayName("Admin.Catalog.TvChannels.Fields.OverriddenGiftCardAmount")]
        [UIHint("DecimalNullable")]
        public decimal? OverriddenGiftCardAmount { get; set; }

        [TvProgResourceDisplayName("Admin.Catalog.TvChannels.Fields.RequireOtherTvChannels")]
        public bool RequireOtherTvChannels { get; set; }

        [TvProgResourceDisplayName("Admin.Catalog.TvChannels.Fields.RequiredTvChannelIds")]
        public string RequiredTvChannelIds { get; set; }

        [TvProgResourceDisplayName("Admin.Catalog.TvChannels.Fields.AutomaticallyAddRequiredTvChannels")]
        public bool AutomaticallyAddRequiredTvChannels { get; set; }

        [TvProgResourceDisplayName("Admin.Catalog.TvChannels.Fields.IsDownload")]
        public bool IsDownload { get; set; }

        [TvProgResourceDisplayName("Admin.Catalog.TvChannels.Fields.Download")]
        [UIHint("Download")]
        public int DownloadId { get; set; }

        [TvProgResourceDisplayName("Admin.Catalog.TvChannels.Fields.UnlimitedDownloads")]
        public bool UnlimitedDownloads { get; set; }

        [TvProgResourceDisplayName("Admin.Catalog.TvChannels.Fields.MaxNumberOfDownloads")]
        public int MaxNumberOfDownloads { get; set; }

        [TvProgResourceDisplayName("Admin.Catalog.TvChannels.Fields.DownloadExpirationDays")]
        [UIHint("Int32Nullable")]
        public int? DownloadExpirationDays { get; set; }

        [TvProgResourceDisplayName("Admin.Catalog.TvChannels.Fields.DownloadActivationType")]
        public int DownloadActivationTypeId { get; set; }

        [TvProgResourceDisplayName("Admin.Catalog.TvChannels.Fields.HasSampleDownload")]
        public bool HasSampleDownload { get; set; }

        [TvProgResourceDisplayName("Admin.Catalog.TvChannels.Fields.SampleDownload")]
        [UIHint("Download")]
        public int SampleDownloadId { get; set; }

        [TvProgResourceDisplayName("Admin.Catalog.TvChannels.Fields.HasUserAgreement")]
        public bool HasUserAgreement { get; set; }

        [TvProgResourceDisplayName("Admin.Catalog.TvChannels.Fields.UserAgreementText")]
        public string UserAgreementText { get; set; }

        [TvProgResourceDisplayName("Admin.Catalog.TvChannels.Fields.IsRecurring")]
        public bool IsRecurring { get; set; }

        [TvProgResourceDisplayName("Admin.Catalog.TvChannels.Fields.RecurringCycleLength")]
        public int RecurringCycleLength { get; set; }

        [TvProgResourceDisplayName("Admin.Catalog.TvChannels.Fields.RecurringCyclePeriod")]
        public int RecurringCyclePeriodId { get; set; }

        [TvProgResourceDisplayName("Admin.Catalog.TvChannels.Fields.RecurringTotalCycles")]
        public int RecurringTotalCycles { get; set; }

        [TvProgResourceDisplayName("Admin.Catalog.TvChannels.Fields.IsRental")]
        public bool IsRental { get; set; }

        [TvProgResourceDisplayName("Admin.Catalog.TvChannels.Fields.RentalPriceLength")]
        public int RentalPriceLength { get; set; }

        [TvProgResourceDisplayName("Admin.Catalog.TvChannels.Fields.RentalPricePeriod")]
        public int RentalPricePeriodId { get; set; }

        [TvProgResourceDisplayName("Admin.Catalog.TvChannels.Fields.IsShipEnabled")]
        public bool IsShipEnabled { get; set; }

        [TvProgResourceDisplayName("Admin.Catalog.TvChannels.Fields.IsFreeShipping")]
        public bool IsFreeShipping { get; set; }

        [TvProgResourceDisplayName("Admin.Catalog.TvChannels.Fields.ShipSeparately")]
        public bool ShipSeparately { get; set; }

        [TvProgResourceDisplayName("Admin.Catalog.TvChannels.Fields.AdditionalShippingCharge")]
        public decimal AdditionalShippingCharge { get; set; }

        [TvProgResourceDisplayName("Admin.Catalog.TvChannels.Fields.DeliveryDate")]
        public int DeliveryDateId { get; set; }
        public IList<SelectListItem> AvailableDeliveryDates { get; set; }

        [TvProgResourceDisplayName("Admin.Catalog.TvChannels.Fields.IsTaxExempt")]
        public bool IsTaxExempt { get; set; }

        [TvProgResourceDisplayName("Admin.Catalog.TvChannels.Fields.TaxCategory")]
        public int TaxCategoryId { get; set; }
        public IList<SelectListItem> AvailableTaxCategories { get; set; }

        [TvProgResourceDisplayName("Admin.Catalog.TvChannels.Fields.IsTelecommunicationsOrBroadcastingOrElectronicServices")]
        public bool IsTelecommunicationsOrBroadcastingOrElectronicServices { get; set; }

        [TvProgResourceDisplayName("Admin.Catalog.TvChannels.Fields.ManageInventoryMethod")]
        public int ManageInventoryMethodId { get; set; }

        [TvProgResourceDisplayName("Admin.Catalog.TvChannels.Fields.TvChannelAvailabilityRange")]
        public int TvChannelAvailabilityRangeId { get; set; }
        public IList<SelectListItem> AvailableTvChannelAvailabilityRanges { get; set; }

        [TvProgResourceDisplayName("Admin.Catalog.TvChannels.Fields.UseMultipleWarehouses")]
        public bool UseMultipleWarehouses { get; set; }

        [TvProgResourceDisplayName("Admin.Catalog.TvChannels.Fields.Warehouse")]
        public int WarehouseId { get; set; }
        public IList<SelectListItem> AvailableWarehouses { get; set; }

        [TvProgResourceDisplayName("Admin.Catalog.TvChannels.Fields.StockQuantity")]
        public int StockQuantity { get; set; }

        public int LastStockQuantity { get; set; }

        [TvProgResourceDisplayName("Admin.Catalog.TvChannels.Fields.StockQuantity")]
        public string StockQuantityStr { get; set; }

        [TvProgResourceDisplayName("Admin.Catalog.TvChannels.Fields.DisplayStockAvailability")]
        public bool DisplayStockAvailability { get; set; }

        [TvProgResourceDisplayName("Admin.Catalog.TvChannels.Fields.DisplayStockQuantity")]
        public bool DisplayStockQuantity { get; set; }

        [TvProgResourceDisplayName("Admin.Catalog.TvChannels.Fields.MinStockQuantity")]
        public int MinStockQuantity { get; set; }

        [TvProgResourceDisplayName("Admin.Catalog.TvChannels.Fields.LowStockActivity")]
        public int LowStockActivityId { get; set; }

        [TvProgResourceDisplayName("Admin.Catalog.TvChannels.Fields.NotifyAdminForQuantityBelow")]
        public int NotifyAdminForQuantityBelow { get; set; }

        [TvProgResourceDisplayName("Admin.Catalog.TvChannels.Fields.BackorderMode")]
        public int BackorderModeId { get; set; }

        [TvProgResourceDisplayName("Admin.Catalog.TvChannels.Fields.AllowBackInStockSubscriptions")]
        public bool AllowBackInStockSubscriptions { get; set; }

        [TvProgResourceDisplayName("Admin.Catalog.TvChannels.Fields.OrderMinimumQuantity")]
        public int OrderMinimumQuantity { get; set; }

        [TvProgResourceDisplayName("Admin.Catalog.TvChannels.Fields.OrderMaximumQuantity")]
        public int OrderMaximumQuantity { get; set; }

        [TvProgResourceDisplayName("Admin.Catalog.TvChannels.Fields.AllowedQuantities")]
        public string AllowedQuantities { get; set; }

        [TvProgResourceDisplayName("Admin.Catalog.TvChannels.Fields.AllowAddingOnlyExistingAttributeCombinations")]
        public bool AllowAddingOnlyExistingAttributeCombinations { get; set; }

        [TvProgResourceDisplayName("Admin.Catalog.TvChannels.Fields.NotReturnable")]
        public bool NotReturnable { get; set; }

        [TvProgResourceDisplayName("Admin.Catalog.TvChannels.Fields.DisableBuyButton")]
        public bool DisableBuyButton { get; set; }

        [TvProgResourceDisplayName("Admin.Catalog.TvChannels.Fields.DisableWishlistButton")]
        public bool DisableWishlistButton { get; set; }

        [TvProgResourceDisplayName("Admin.Catalog.TvChannels.Fields.AvailableForPreOrder")]
        public bool AvailableForPreOrder { get; set; }

        [TvProgResourceDisplayName("Admin.Catalog.TvChannels.Fields.PreOrderAvailabilityStartDateTimeUtc")]
        [UIHint("DateTimeNullable")]
        public DateTime? PreOrderAvailabilityStartDateTimeUtc { get; set; }

        [TvProgResourceDisplayName("Admin.Catalog.TvChannels.Fields.CallForPrice")]
        public bool CallForPrice { get; set; }

        [TvProgResourceDisplayName("Admin.Catalog.TvChannels.Fields.Price")]
        public decimal Price { get; set; }

        [TvProgResourceDisplayName("Admin.Catalog.TvChannels.Fields.OldPrice")]
        public decimal OldPrice { get; set; }

        [TvProgResourceDisplayName("Admin.Catalog.TvChannels.Fields.TvChannelCost")]
        public decimal TvChannelCost { get; set; }

        [TvProgResourceDisplayName("Admin.Catalog.TvChannels.Fields.UserEntersPrice")]
        public bool UserEntersPrice { get; set; }

        [TvProgResourceDisplayName("Admin.Catalog.TvChannels.Fields.MinimumUserEnteredPrice")]
        public decimal MinimumUserEnteredPrice { get; set; }

        [TvProgResourceDisplayName("Admin.Catalog.TvChannels.Fields.MaximumUserEnteredPrice")]
        public decimal MaximumUserEnteredPrice { get; set; }

        [TvProgResourceDisplayName("Admin.Catalog.TvChannels.Fields.BasepriceEnabled")]
        public bool BasepriceEnabled { get; set; }

        [TvProgResourceDisplayName("Admin.Catalog.TvChannels.Fields.BasepriceAmount")]
        public decimal BasepriceAmount { get; set; }

        [TvProgResourceDisplayName("Admin.Catalog.TvChannels.Fields.BasepriceUnit")]
        public int BasepriceUnitId { get; set; }
        public IList<SelectListItem> AvailableBasepriceUnits { get; set; }

        [TvProgResourceDisplayName("Admin.Catalog.TvChannels.Fields.BasepriceBaseAmount")]
        public decimal BasepriceBaseAmount { get; set; }

        [TvProgResourceDisplayName("Admin.Catalog.TvChannels.Fields.BasepriceBaseUnit")]
        public int BasepriceBaseUnitId { get; set; }
        public IList<SelectListItem> AvailableBasepriceBaseUnits { get; set; }

        [TvProgResourceDisplayName("Admin.Catalog.TvChannels.Fields.MarkAsNew")]
        public bool MarkAsNew { get; set; }

        [TvProgResourceDisplayName("Admin.Catalog.TvChannels.Fields.MarkAsNewStartDateTimeUtc")]
        [UIHint("DateTimeNullable")]
        public DateTime? MarkAsNewStartDateTimeUtc { get; set; }

        [TvProgResourceDisplayName("Admin.Catalog.TvChannels.Fields.MarkAsNewEndDateTimeUtc")]
        [UIHint("DateTimeNullable")]
        public DateTime? MarkAsNewEndDateTimeUtc { get; set; }

        [TvProgResourceDisplayName("Admin.Catalog.TvChannels.Fields.Weight")]
        public decimal Weight { get; set; }

        [TvProgResourceDisplayName("Admin.Catalog.TvChannels.Fields.Length")]
        public decimal Length { get; set; }

        [TvProgResourceDisplayName("Admin.Catalog.TvChannels.Fields.Width")]
        public decimal Width { get; set; }

        [TvProgResourceDisplayName("Admin.Catalog.TvChannels.Fields.Height")]
        public decimal Height { get; set; }

        [TvProgResourceDisplayName("Admin.Catalog.TvChannels.Fields.AvailableStartDateTime")]
        [UIHint("DateTimeNullable")]
        public DateTime? AvailableStartDateTimeUtc { get; set; }

        [TvProgResourceDisplayName("Admin.Catalog.TvChannels.Fields.AvailableEndDateTime")]
        [UIHint("DateTimeNullable")]
        public DateTime? AvailableEndDateTimeUtc { get; set; }

        [TvProgResourceDisplayName("Admin.Catalog.TvChannels.Fields.DisplayOrder")]
        public int DisplayOrder { get; set; }

        [TvProgResourceDisplayName("Admin.Catalog.TvChannels.Fields.Published")]
        public bool Published { get; set; }

        public string PrimaryStoreCurrencyCode { get; set; }

        public string BaseDimensionIn { get; set; }

        public string BaseWeightIn { get; set; }

        public IList<TvChannelLocalizedModel> Locales { get; set; }

        //ACL (user roles)
        [TvProgResourceDisplayName("Admin.Catalog.TvChannels.Fields.AclUserRoles")]
        public IList<int> SelectedUserRoleIds { get; set; }
        public IList<SelectListItem> AvailableUserRoles { get; set; }

        //store mapping
        [TvProgResourceDisplayName("Admin.Catalog.TvChannels.Fields.LimitedToStores")]
        public IList<int> SelectedStoreIds { get; set; }
        public IList<SelectListItem> AvailableStores { get; set; }

        //categories
        [TvProgResourceDisplayName("Admin.Catalog.TvChannels.Fields.Categories")]
        public IList<int> SelectedCategoryIds { get; set; }
        public IList<SelectListItem> AvailableCategories { get; set; }

        //manufacturers
        [TvProgResourceDisplayName("Admin.Catalog.TvChannels.Fields.Manufacturers")]
        public IList<int> SelectedManufacturerIds { get; set; }
        public IList<SelectListItem> AvailableManufacturers { get; set; }

        //vendors
        [TvProgResourceDisplayName("Admin.Catalog.TvChannels.Fields.Vendor")]
        public int VendorId { get; set; }
        public IList<SelectListItem> AvailableVendors { get; set; }

        //discounts
        [TvProgResourceDisplayName("Admin.Catalog.TvChannels.Fields.Discounts")]
        public IList<int> SelectedDiscountIds { get; set; }
        public IList<SelectListItem> AvailableDiscounts { get; set; }

        //vendor
        public bool IsLoggedInAsVendor { get; set; }

        //pictures
        public TvChannelPictureModel AddPictureModel { get; set; }
        public IList<TvChannelPictureModel> TvChannelPictureModels { get; set; }

        //videos
        public TvChannelVideoModel AddVideoModel { get; set; }
        public IList<TvChannelVideoModel> TvChannelVideoModels { get; set; }

        //tvchannel attributes
        public bool TvChannelAttributesExist { get; set; }
        public bool CanCreateCombinations { get; set; }

        //multiple warehouses
        [TvProgResourceDisplayName("Admin.Catalog.TvChannels.TvChannelWarehouseInventory")]
        public IList<TvChannelWarehouseInventoryModel> TvChannelWarehouseInventoryModels { get; set; }

        //specification attributes
        public bool HasAvailableSpecificationAttributes { get; set; }

        //copy tvchannel
        public CopyTvChannelModel CopyTvChannelModel { get; set; }

        //editor settings
        public TvChannelEditorSettingsModel TvChannelEditorSettingsModel { get; set; }

        //stock quantity history
        public StockQuantityHistoryModel StockQuantityHistory { get; set; }

        public RelatedTvChannelSearchModel RelatedTvChannelSearchModel { get; set; }

        public CrossSellTvChannelSearchModel CrossSellTvChannelSearchModel { get; set; }

        public AssociatedTvChannelSearchModel AssociatedTvChannelSearchModel { get; set; }

        public TvChannelPictureSearchModel TvChannelPictureSearchModel { get; set; }

        public TvChannelVideoSearchModel TvChannelVideoSearchModel { get; set; }

        public TvChannelSpecificationAttributeSearchModel TvChannelSpecificationAttributeSearchModel { get; set; }

        public TvChannelOrderSearchModel TvChannelOrderSearchModel { get; set; }

        public TierPriceSearchModel TierPriceSearchModel { get; set; }

        public StockQuantityHistorySearchModel StockQuantityHistorySearchModel { get; set; }

        public TvChannelAttributeMappingSearchModel TvChannelAttributeMappingSearchModel { get; set; }

        public TvChannelAttributeCombinationSearchModel TvChannelAttributeCombinationSearchModel { get; set; }

        #endregion
    }

    public partial record TvChannelLocalizedModel : ILocalizedLocaleModel
    {
        public int LanguageId { get; set; }

        [TvProgResourceDisplayName("Admin.Catalog.TvChannels.Fields.Name")]
        public string Name { get; set; }

        [TvProgResourceDisplayName("Admin.Catalog.TvChannels.Fields.ShortDescription")]
        public string ShortDescription { get; set; }

        [TvProgResourceDisplayName("Admin.Catalog.TvChannels.Fields.FullDescription")]
        public string FullDescription { get; set; }

        [TvProgResourceDisplayName("Admin.Catalog.TvChannels.Fields.MetaKeywords")]
        public string MetaKeywords { get; set; }

        [TvProgResourceDisplayName("Admin.Catalog.TvChannels.Fields.MetaDescription")]
        public string MetaDescription { get; set; }

        [TvProgResourceDisplayName("Admin.Catalog.TvChannels.Fields.MetaTitle")]
        public string MetaTitle { get; set; }

        [TvProgResourceDisplayName("Admin.Catalog.TvChannels.Fields.SeName")]
        public string SeName { get; set; }
    }
}