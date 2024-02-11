using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using TvProgViewer.Core.Domain.Tax;
using TvProgViewer.WebUI.Areas.Admin.Models.Common;
using TvProgViewer.Web.Framework.Models;
using TvProgViewer.Web.Framework.Mvc.ModelBinding;

namespace TvProgViewer.WebUI.Areas.Admin.Models.Orders
{
    /// <summary>
    /// Represents an order model
    /// </summary>
    public partial record OrderModel : BaseTvProgEntityModel
    {
        #region Ctor

        public OrderModel()
        {
            CustomValues = new Dictionary<string, object>();
            TaxRates = new List<TaxRate>();
            GiftCards = new List<GiftCard>();
            Items = new List<OrderItemModel>();
            UsedDiscounts = new List<UsedDiscountModel>();
            OrderShipmentSearchModel = new OrderShipmentSearchModel();
            OrderNoteSearchModel = new OrderNoteSearchModel();
            BillingAddress = new AddressModel();
            ShippingAddress = new AddressModel();
            PickupAddress = new AddressModel();
        }

        #endregion

        #region Properties

        public bool IsLoggedInAsVendor { get; set; }

        //identifiers
        public override int Id { get; set; }
        [TvProgResourceDisplayName("Admin.Orders.Fields.OrderGuid")]
        public Guid OrderGuid { get; set; }
        [TvProgResourceDisplayName("Admin.Orders.Fields.CustomOrderNumber")]
        public string CustomOrderNumber { get; set; }
        
        //store
        [TvProgResourceDisplayName("Admin.Orders.Fields.Store")]
        public string StoreName { get; set; }

        //user info
        [TvProgResourceDisplayName("Admin.Orders.Fields.User")]
        public int UserId { get; set; }
        [TvProgResourceDisplayName("Admin.Orders.Fields.User")]
        public string UserInfo { get; set; }
        [TvProgResourceDisplayName("Admin.Orders.Fields.UserEmail")]
        public string UserEmail { get; set; }
        public string UserFullName { get; set; }
        [TvProgResourceDisplayName("Admin.Orders.Fields.UserIP")]
        public string UserIp { get; set; }

        [TvProgResourceDisplayName("Admin.Orders.Fields.CustomValues")]
        public Dictionary<string, object> CustomValues { get; set; }

        [TvProgResourceDisplayName("Admin.Orders.Fields.Affiliate")]
        public int AffiliateId { get; set; }
        [TvProgResourceDisplayName("Admin.Orders.Fields.Affiliate")]
        public string AffiliateName { get; set; }

        //Used discounts
        [TvProgResourceDisplayName("Admin.Orders.Fields.UsedDiscounts")]
        public IList<UsedDiscountModel> UsedDiscounts { get; set; }

        //totals
        public bool AllowUsersToSelectTaxDisplayType { get; set; }
        public TaxDisplayType TaxDisplayType { get; set; }
        [TvProgResourceDisplayName("Admin.Orders.Fields.OrderSubtotalInclTax")]
        public string OrderSubtotalInclTax { get; set; }
        [TvProgResourceDisplayName("Admin.Orders.Fields.OrderSubtotalExclTax")]
        public string OrderSubtotalExclTax { get; set; }
        [TvProgResourceDisplayName("Admin.Orders.Fields.OrderSubTotalDiscountInclTax")]
        public string OrderSubTotalDiscountInclTax { get; set; }
        [TvProgResourceDisplayName("Admin.Orders.Fields.OrderSubTotalDiscountExclTax")]
        public string OrderSubTotalDiscountExclTax { get; set; }
        [TvProgResourceDisplayName("Admin.Orders.Fields.OrderShippingInclTax")]
        public string OrderShippingInclTax { get; set; }
        [TvProgResourceDisplayName("Admin.Orders.Fields.OrderShippingExclTax")]
        public string OrderShippingExclTax { get; set; }
        [TvProgResourceDisplayName("Admin.Orders.Fields.PaymentMethodAdditionalFeeInclTax")]
        public string PaymentMethodAdditionalFeeInclTax { get; set; }
        [TvProgResourceDisplayName("Admin.Orders.Fields.PaymentMethodAdditionalFeeExclTax")]
        public string PaymentMethodAdditionalFeeExclTax { get; set; }
        [TvProgResourceDisplayName("Admin.Orders.Fields.Tax")]
        public string Tax { get; set; }
        public IList<TaxRate> TaxRates { get; set; }
        public bool DisplayTax { get; set; }
        public bool DisplayTaxRates { get; set; }
        [TvProgResourceDisplayName("Admin.Orders.Fields.OrderTotalDiscount")]
        public string OrderTotalDiscount { get; set; }
        [TvProgResourceDisplayName("Admin.Orders.Fields.RedeemedRewardPoints")]
        public int RedeemedRewardPoints { get; set; }
        [TvProgResourceDisplayName("Admin.Orders.Fields.RedeemedRewardPoints")]
        public string RedeemedRewardPointsAmount { get; set; }
        [TvProgResourceDisplayName("Admin.Orders.Fields.OrderTotal")]
        public string OrderTotal { get; set; }
        [TvProgResourceDisplayName("Admin.Orders.Fields.RefundedAmount")]
        public string RefundedAmount { get; set; }
        [TvProgResourceDisplayName("Admin.Orders.Fields.Profit")]
        public string Profit { get; set; }

        //edit totals
        [TvProgResourceDisplayName("Admin.Orders.Fields.Edit.OrderSubtotal")]
        public decimal OrderSubtotalInclTaxValue { get; set; }
        [TvProgResourceDisplayName("Admin.Orders.Fields.Edit.OrderSubtotal")]
        public decimal OrderSubtotalExclTaxValue { get; set; }
        [TvProgResourceDisplayName("Admin.Orders.Fields.Edit.OrderSubTotalDiscount")]
        public decimal OrderSubTotalDiscountInclTaxValue { get; set; }
        [TvProgResourceDisplayName("Admin.Orders.Fields.Edit.OrderSubTotalDiscount")]
        public decimal OrderSubTotalDiscountExclTaxValue { get; set; }
        [TvProgResourceDisplayName("Admin.Orders.Fields.Edit.OrderShipping")]
        public decimal OrderShippingInclTaxValue { get; set; }
        [TvProgResourceDisplayName("Admin.Orders.Fields.Edit.OrderShipping")]
        public decimal OrderShippingExclTaxValue { get; set; }
        [TvProgResourceDisplayName("Admin.Orders.Fields.Edit.PaymentMethodAdditionalFee")]
        public decimal PaymentMethodAdditionalFeeInclTaxValue { get; set; }
        [TvProgResourceDisplayName("Admin.Orders.Fields.Edit.PaymentMethodAdditionalFee")]
        public decimal PaymentMethodAdditionalFeeExclTaxValue { get; set; }
        [TvProgResourceDisplayName("Admin.Orders.Fields.Edit.Tax")]
        public decimal TaxValue { get; set; }
        [TvProgResourceDisplayName("Admin.Orders.Fields.Edit.TaxRates")]
        public string TaxRatesValue { get; set; }
        [TvProgResourceDisplayName("Admin.Orders.Fields.Edit.OrderTotalDiscount")]
        public decimal OrderTotalDiscountValue { get; set; }
        [TvProgResourceDisplayName("Admin.Orders.Fields.Edit.OrderTotal")]
        public decimal OrderTotalValue { get; set; }

        //associated recurring payment id
        [TvProgResourceDisplayName("Admin.Orders.Fields.RecurringPayment")]
        public int RecurringPaymentId { get; set; }

        //order status
        [TvProgResourceDisplayName("Admin.Orders.Fields.OrderStatus")]
        public string OrderStatus { get; set; }
        [TvProgResourceDisplayName("Admin.Orders.Fields.OrderStatus")]
        public int OrderStatusId { get; set; }

        //payment info
        [TvProgResourceDisplayName("Admin.Orders.Fields.PaymentStatus")]
        public string PaymentStatus { get; set; }
        [TvProgResourceDisplayName("Admin.Orders.Fields.PaymentStatus")]
        public int PaymentStatusId { get; set; }
        [TvProgResourceDisplayName("Admin.Orders.Fields.PaymentMethod")]
        public string PaymentMethod { get; set; }

        //credit card info
        public bool AllowStoringCreditCardNumber { get; set; }
        [TvProgResourceDisplayName("Admin.Orders.Fields.CardType")]
        public string CardType { get; set; }
        [TvProgResourceDisplayName("Admin.Orders.Fields.CardName")]
        public string CardName { get; set; }
        [TvProgResourceDisplayName("Admin.Orders.Fields.CardNumber")]
        public string CardNumber { get; set; }
        [TvProgResourceDisplayName("Admin.Orders.Fields.CardCVV2")]
        public string CardCvv2 { get; set; }
        [TvProgResourceDisplayName("Admin.Orders.Fields.CardExpirationMonth")]
        public string CardExpirationMonth { get; set; }
        [TvProgResourceDisplayName("Admin.Orders.Fields.CardExpirationYear")]
        public string CardExpirationYear { get; set; }

        //misc payment info
        [TvProgResourceDisplayName("Admin.Orders.Fields.AuthorizationTransactionID")]
        public string AuthorizationTransactionId { get; set; }
        [TvProgResourceDisplayName("Admin.Orders.Fields.CaptureTransactionID")]
        public string CaptureTransactionId { get; set; }
        [TvProgResourceDisplayName("Admin.Orders.Fields.SubscriptionTransactionID")]
        public string SubscriptionTransactionId { get; set; }

        //shipping info
        public bool IsShippable { get; set; }
        public bool PickupInStore { get; set; }
        [TvProgResourceDisplayName("Admin.Orders.Fields.PickupAddress")]
        public AddressModel PickupAddress { get; set; }
        public string PickupAddressGoogleMapsUrl { get; set; }
        [TvProgResourceDisplayName("Admin.Orders.Fields.ShippingStatus")]
        public string ShippingStatus { get; set; }
        [TvProgResourceDisplayName("Admin.Orders.Fields.ShippingStatus")]
        public int ShippingStatusId { get; set; }
        [TvProgResourceDisplayName("Admin.Orders.Fields.ShippingAddress")]
        public AddressModel ShippingAddress { get; set; }
        [TvProgResourceDisplayName("Admin.Orders.Fields.ShippingMethod")]
        public string ShippingMethod { get; set; }
        public string ShippingAddressGoogleMapsUrl { get; set; }
        public bool CanAddNewShipments { get; set; }

        //billing info
        [TvProgResourceDisplayName("Admin.Orders.Fields.BillingAddress")]
        public AddressModel BillingAddress { get; set; }
        [TvProgResourceDisplayName("Admin.Orders.Fields.VatNumber")]
        public string VatNumber { get; set; }
        
        //gift cards
        public IList<GiftCard> GiftCards { get; set; }

        //items
        public bool HasDownloadableTvChannels { get; set; }
        public IList<OrderItemModel> Items { get; set; }

        //creation date
        [TvProgResourceDisplayName("Admin.Orders.Fields.CreatedOn")]
        public DateTime CreatedOn { get; set; }

        //checkout attributes
        public string CheckoutAttributeInfo { get; set; }

        //order notes
        [TvProgResourceDisplayName("Admin.Orders.OrderNotes.Fields.DisplayToUser")]
        public bool AddOrderNoteDisplayToUser { get; set; }
        [TvProgResourceDisplayName("Admin.Orders.OrderNotes.Fields.Note")]
        public string AddOrderNoteMessage { get; set; }
        public bool AddOrderNoteHasDownload { get; set; }
        [TvProgResourceDisplayName("Admin.Orders.OrderNotes.Fields.Download")]
        [UIHint("Download")]
        public int AddOrderNoteDownloadId { get; set; }

        //refund info
        [TvProgResourceDisplayName("Admin.Orders.Fields.PartialRefund.AmountToRefund")]
        public decimal AmountToRefund { get; set; }
        public decimal MaxAmountToRefund { get; set; }
        public string PrimaryStoreCurrencyCode { get; set; }

        //workflow info
        public bool CanCancelOrder { get; set; }
        public bool CanCapture { get; set; }
        public bool CanMarkOrderAsPaid { get; set; }
        public bool CanRefund { get; set; }
        public bool CanRefundOffline { get; set; }
        public bool CanPartiallyRefund { get; set; }
        public bool CanPartiallyRefundOffline { get; set; }
        public bool CanVoid { get; set; }
        public bool CanVoidOffline { get; set; }

        public OrderShipmentSearchModel OrderShipmentSearchModel { get; set; }

        public OrderNoteSearchModel OrderNoteSearchModel { get; set; }

        #endregion

        #region Nested Classes

        public partial record TaxRate : BaseTvProgModel
        {
            public string Rate { get; set; }
            public string Value { get; set; }
        }

        public partial record GiftCard : BaseTvProgModel
        {
            [TvProgResourceDisplayName("Admin.Orders.Fields.GiftCardInfo")]
            public string CouponCode { get; set; }
            public string Amount { get; set; }
        }               
        
        public partial record UsedDiscountModel:BaseTvProgModel
        {
            public int DiscountId { get; set; }
            public string DiscountName { get; set; }
        }

        #endregion
    }

    public partial record OrderAggreratorModel : BaseTvProgModel
    {
        //aggergator properties
        public string aggregatorprofit { get; set; }
        public string aggregatorshipping { get; set; }
        public string aggregatortax { get; set; }
        public string aggregatortotal { get; set; }
    }
}