using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;
using TvProgViewer.Core.Domain.Catalog;
using TvProgViewer.Core.Domain.Orders;
using TvProgViewer.Data.TvProgMain;
using TvProgViewer.Web.Framework.Models;
using TvProgViewer.Web.Framework.Mvc.ModelBinding;
using TvProgViewer.WebUI.Models.Common;
using TvProgViewer.WebUI.Models.Media;
using TvProgViewer.WebUI.Models.ShoppingCart;

namespace TvProgViewer.WebUI.Models.Catalog
{
    public partial record TvChannelDetailsModel : BaseTvProgEntityModel
    {
        public TvChannelDetailsModel()
        {
            DefaultPictureModel = new PictureModel();
            PictureModels = new List<PictureModel>();
            VideoModels = new List<VideoModel>();
            GiftCard = new GiftCardModel();
            TvChannelPrice = new TvChannelPriceModel();
            AddToCart = new AddToCartModel();
            TvChannelAttributes = new List<TvChannelAttributeModel>();
            AssociatedTvChannels = new List<TvChannelDetailsModel>();
            VendorModel = new VendorBriefInfoModel();
            Breadcrumb = new TvChannelBreadcrumbModel();
            TvChannelTags = new List<TvChannelTagModel>();
            TvChannelSpecificationModel = new TvChannelSpecificationModel();
            TvChannelManufacturers = new List<ManufacturerBriefInfoModel>();
            TvChannelReviewOverview = new TvChannelReviewOverviewModel();
            TierPrices = new List<TierPriceModel>();
            TvChannelEstimateShipping = new TvChannelEstimateShippingModel();
            TvChannelDays = new List<DaysItem>();
        }

        //picture(s)
        public bool DefaultPictureZoomEnabled { get; set; }
        public PictureModel DefaultPictureModel { get; set; }
        public IList<PictureModel> PictureModels { get; set; }

        //videos
        public IList<VideoModel> VideoModels { get; set; }

        public string Name { get; set; }
        public string ShortDescription { get; set; }
        public string FullDescription { get; set; }
        public string MetaKeywords { get; set; }
        public string MetaDescription { get; set; }
        public string MetaTitle { get; set; }
        public string SeName { get; set; }
        public bool VisibleIndividually { get; set; }

        public TvChannelType TvChannelType { get; set; }

        public bool ShowSku { get; set; }
        public string Sku { get; set; }

        public bool ShowManufacturerPartNumber { get; set; }
        public string ManufacturerPartNumber { get; set; }

        public bool ShowGtin { get; set; }
        public string Gtin { get; set; }

        public bool ShowVendor { get; set; }
        public VendorBriefInfoModel VendorModel { get; set; }

        public bool HasSampleDownload { get; set; }

        public GiftCardModel GiftCard { get; set; }

        public bool IsShipEnabled { get; set; }
        public bool IsFreeShipping { get; set; }
        public bool FreeShippingNotificationEnabled { get; set; }
        public string DeliveryDate { get; set; }

        public bool IsRental { get; set; }
        public DateTime? RentalStartDate { get; set; }
        public DateTime? RentalEndDate { get; set; }

        public DateTime? AvailableEndDate { get; set; }

        public ManageInventoryMethod ManageInventoryMethod { get; set; }

        public string StockAvailability { get; set; }

        public bool DisplayBackInStockSubscription { get; set; }

        public bool EmailAFriendEnabled { get; set; }
        public bool TvChannelLiveUrlEnabled { get; set; }
        public string TvChannelLiveUrl { get; set; }
        public bool CompareTvChannelsEnabled { get; set; }

        public string PageShareCode { get; set; }

        public TvChannelPriceModel TvChannelPrice { get; set; }

        public AddToCartModel AddToCart { get; set; }

        public TvChannelBreadcrumbModel Breadcrumb { get; set; }

        public IList<TvChannelTagModel> TvChannelTags { get; set; }

        public IList<TvChannelAttributeModel> TvChannelAttributes { get; set; }

        public TvChannelSpecificationModel TvChannelSpecificationModel { get; set; }

        public IList<ManufacturerBriefInfoModel> TvChannelManufacturers { get; set; }

        public TvChannelReviewOverviewModel TvChannelReviewOverview { get; set; }

        public TvChannelEstimateShippingModel TvChannelEstimateShipping { get; set; }

        public IList<TierPriceModel> TierPrices { get; set; }

        //a list of associated tvChannels. For example, "Grouped" tvChannels could have several child "simple" tvChannels
        public IList<TvChannelDetailsModel> AssociatedTvChannels { get; set; }

        public bool DisplayDiscontinuedMessage { get; set; }

        public string CurrentStoreName { get; set; }

        public bool InStock { get; set; }

        public bool AllowAddingOnlyExistingAttributeCombinations { get; set; }
        public IEnumerable<DaysItem> TvChannelDays {get; set;}
       
        #region Nested Classes

        public partial record TvChannelBreadcrumbModel : BaseTvProgModel
        {
            public TvChannelBreadcrumbModel()
            {
                CategoryBreadcrumb = new List<CategorySimpleModel>();
            }

            public bool Enabled { get; set; }
            public int TvChannelId { get; set; }
            public string TvChannelName { get; set; }
            public string TvChannelSeName { get; set; }
            public IList<CategorySimpleModel> CategoryBreadcrumb { get; set; }
        }

        public partial record AddToCartModel : BaseTvProgModel
        {
            public AddToCartModel()
            {
                AllowedQuantities = new List<SelectListItem>();
            }
            public int TvChannelId { get; set; }

            //qty
            [TvProgResourceDisplayName("TvChannels.Qty")]
            public int EnteredQuantity { get; set; }
            public string MinimumQuantityNotification { get; set; }
            public List<SelectListItem> AllowedQuantities { get; set; }

            //price entered by users
            [TvProgResourceDisplayName("TvChannels.EnterTvChannelPrice")]
            public bool UserEntersPrice { get; set; }
            [TvProgResourceDisplayName("TvChannels.EnterTvChannelPrice")]
            public decimal UserEnteredPrice { get; set; }
            public string UserEnteredPriceRange { get; set; }

            public bool DisableBuyButton { get; set; }
            public bool DisableWishlistButton { get; set; }

            //rental
            public bool IsRental { get; set; }

            //pre-order
            public bool AvailableForPreOrder { get; set; }
            public DateTime? PreOrderAvailabilityStartDateTimeUtc { get; set; }
            public string PreOrderAvailabilityStartDateTimeUserTime { get; set; }

            //updating existing shopping cart or wishlist item?
            public int UpdatedShoppingCartItemId { get; set; }
            public ShoppingCartType? UpdateShoppingCartItemType { get; set; }
        }

        public partial record TvChannelPriceModel : BaseTvProgModel
        {
            /// <summary>
            /// The currency (in 3-letter ISO 4217 format) of the offer price 
            /// </summary>
            public string CurrencyCode { get; set; }

            public string OldPrice { get; set; }
            public decimal? OldPriceValue { get; set; }

            public string Price { get; set; }
            public decimal PriceValue { get; set; }
            public string PriceWithDiscount { get; set; }
            public decimal? PriceWithDiscountValue { get; set; }

            public bool UserEntersPrice { get; set; }

            public bool CallForPrice { get; set; }

            public int TvChannelId { get; set; }

            public bool HidePrices { get; set; }

            //rental
            public bool IsRental { get; set; }
            public string RentalPrice { get; set; }
            public decimal? RentalPriceValue { get; set; }

            /// <summary>
            /// A value indicating whether we should display tax/shipping info (used in Germany)
            /// </summary>
            public bool DisplayTaxShippingInfo { get; set; }
            /// <summary>
            /// PAngV baseprice (used in Germany)
            /// </summary>
            public string BasePricePAngV { get; set; }
            public decimal? BasePricePAngVValue { get; set; }
        }

        public partial record GiftCardModel : BaseTvProgModel
        {
            public bool IsGiftCard { get; set; }

            [TvProgResourceDisplayName("TvChannels.GiftCard.RecipientName")]
            public string RecipientName { get; set; }

            [TvProgResourceDisplayName("TvChannels.GiftCard.RecipientEmail")]
            [DataType(DataType.EmailAddress)]
            public string RecipientEmail { get; set; }

            [TvProgResourceDisplayName("TvChannels.GiftCard.SenderName")]
            public string SenderName { get; set; }

            [TvProgResourceDisplayName("TvChannels.GiftCard.SenderEmail")]
            [DataType(DataType.EmailAddress)]
            public string SenderEmail { get; set; }

            [TvProgResourceDisplayName("TvChannels.GiftCard.Message")]
            public string Message { get; set; }

            public GiftCardType GiftCardType { get; set; }
        }

        public partial record TierPriceModel : BaseTvProgModel
        {
            public string Price { get; set; }
            public decimal PriceValue { get; set; }

            public int Quantity { get; set; }
        }

        public partial record TvChannelAttributeModel : BaseTvProgEntityModel
        {
            public TvChannelAttributeModel()
            {
                AllowedFileExtensions = new List<string>();
                Values = new List<TvChannelAttributeValueModel>();
            }

            public int TvChannelId { get; set; }

            public int TvChannelAttributeId { get; set; }

            public string Name { get; set; }

            public string Description { get; set; }

            public string TextPrompt { get; set; }

            public bool IsRequired { get; set; }

            /// <summary>
            /// Default value for textboxes
            /// </summary>
            public string DefaultValue { get; set; }
            /// <summary>
            /// Selected day value for datepicker
            /// </summary>
            public int? SelectedDay { get; set; }
            /// <summary>
            /// Selected month value for datepicker
            /// </summary>
            public int? SelectedMonth { get; set; }
            /// <summary>
            /// Selected year value for datepicker
            /// </summary>
            public int? SelectedYear { get; set; }

            /// <summary>
            /// A value indicating whether this attribute depends on some other attribute
            /// </summary>
            public bool HasCondition { get; set; }

            /// <summary>
            /// Allowed file extensions for user uploaded files
            /// </summary>
            public IList<string> AllowedFileExtensions { get; set; }

            public AttributeControlType AttributeControlType { get; set; }

            public IList<TvChannelAttributeValueModel> Values { get; set; }
        }

        public partial record TvChannelAttributeValueModel : BaseTvProgEntityModel
        {
            public TvChannelAttributeValueModel()
            {
                ImageSquaresPictureModel = new PictureModel();
            }

            public string Name { get; set; }

            public string ColorSquaresRgb { get; set; }

            //picture model is used with "image square" attribute type
            public PictureModel ImageSquaresPictureModel { get; set; }

            public string PriceAdjustment { get; set; }

            public bool PriceAdjustmentUsePercentage { get; set; }

            public decimal PriceAdjustmentValue { get; set; }

            public bool IsPreSelected { get; set; }

            //tvChannel picture ID (associated to this value)
            public int PictureId { get; set; }

            public bool UserEntersQty { get; set; }

            public int Quantity { get; set; }
        }

        public partial record TvChannelEstimateShippingModel : EstimateShippingModel
        {
            public int TvChannelId { get; set; }
        }

        #endregion
    }
}