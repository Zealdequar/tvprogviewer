using LinqToDB.Mapping;
using TVProgViewer.Core.Domain.Orders;
using TVProgViewer.Data.Extensions;

namespace TVProgViewer.Data.Mapping.Orders
{
    /// <summary>
    /// Represents an order mapping configuration
    /// </summary>
    public partial class OrderMap : TvProgEntityTypeConfiguration<Order>
    {
        #region Methods

        /// <summary>
        /// Configures the entity
        /// </summary>
        /// <param name="builder">The builder to be used to configure the entity</param>
        public override void Configure(EntityMappingBuilder<Order> builder)
        {
            builder.HasTableName(nameof(Order));

            builder.Property(order => order.CurrencyRate).HasDecimal();
            builder.Property(order => order.OrderSubtotalInclTax).HasDecimal();
            builder.Property(order => order.OrderSubtotalExclTax).HasDecimal();
            builder.Property(order => order.OrderSubTotalDiscountInclTax).HasDecimal();
            builder.Property(order => order.OrderSubTotalDiscountExclTax).HasDecimal();
            builder.Property(order => order.OrderShippingInclTax).HasDecimal();
            builder.Property(order => order.OrderShippingExclTax).HasDecimal();
            builder.Property(order => order.PaymentMethodAdditionalFeeInclTax).HasDecimal();
            builder.Property(order => order.PaymentMethodAdditionalFeeExclTax).HasDecimal();
            builder.Property(order => order.OrderTax).HasDecimal();
            builder.Property(order => order.OrderDiscount).HasDecimal();
            builder.Property(order => order.OrderTotal).HasDecimal();
            builder.Property(order => order.RefundedAmount).HasDecimal();
            builder.Property(order => order.CustomOrderNumber).IsNullable(false);
            builder.Property(order => order.OrderGuid);
            builder.Property(order => order.StoreId);
            builder.Property(order => order.UserId);
            builder.Property(order => order.BillingAddressId);
            builder.Property(order => order.ShippingAddressId);
            builder.Property(order => order.PickupAddressId);
            builder.Property(order => order.PickupInStore);
            builder.Property(order => order.OrderStatusId);
            builder.Property(order => order.ShippingStatusId);
            builder.Property(order => order.PaymentStatusId);
            builder.Property(order => order.PaymentMethodSystemName);
            builder.Property(order => order.UserCurrencyCode);
            builder.Property(order => order.UserTaxDisplayTypeId);
            builder.Property(order => order.VatNumber);
            builder.Property(order => order.TaxRates);
            builder.Property(order => order.RewardPointsHistoryEntryId);
            builder.Property(order => order.CheckoutAttributeDescription);
            builder.Property(order => order.CheckoutAttributesXml);
            builder.Property(order => order.UserLanguageId);
            builder.Property(order => order.AffiliateId);
            builder.Property(order => order.UserIp);
            builder.Property(order => order.AllowStoringCreditCardNumber);
            builder.Property(order => order.CardType);
            builder.Property(order => order.CardName);
            builder.Property(order => order.CardNumber);
            builder.Property(order => order.MaskedCreditCardNumber);
            builder.Property(order => order.CardCvv2);
            builder.Property(order => order.CardExpirationMonth);
            builder.Property(order => order.CardExpirationYear);
            builder.Property(order => order.AuthorizationTransactionId);
            builder.Property(order => order.AuthorizationTransactionCode);
            builder.Property(order => order.AuthorizationTransactionResult);
            builder.Property(order => order.CaptureTransactionId);
            builder.Property(order => order.CaptureTransactionResult);
            builder.Property(order => order.SubscriptionTransactionId);
            builder.Property(order => order.PaidDateUtc);
            builder.Property(order => order.ShippingMethod);
            builder.Property(order => order.ShippingRateComputationMethodSystemName);
            builder.Property(order => order.CustomValuesXml);
            builder.Property(order => order.Deleted);
            builder.Property(order => order.CreatedOnUtc);
            builder.Property(order => order.RedeemedRewardPointsEntryId);

            builder.Ignore(order => order.OrderStatus);
            builder.Ignore(order => order.PaymentStatus);
            builder.Ignore(order => order.ShippingStatus);
            builder.Ignore(order => order.UserTaxDisplayType);
        }

        #endregion
    }
}