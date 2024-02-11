using FluentMigrator;
using TvProgViewer.Core.Domain.Blogs;
using TvProgViewer.Core.Domain.Catalog;
using TvProgViewer.Core.Domain.Common;
using TvProgViewer.Core.Domain.Users;
using TvProgViewer.Core.Domain.Directory;
using TvProgViewer.Core.Domain.Discounts;
using TvProgViewer.Core.Domain.Forums;
using TvProgViewer.Core.Domain.Gdpr;
using TvProgViewer.Core.Domain.Logging;
using TvProgViewer.Core.Domain.Messages;
using TvProgViewer.Core.Domain.News;
using TvProgViewer.Core.Domain.Orders;
using TvProgViewer.Core.Domain.Polls;
using TvProgViewer.Core.Domain.ScheduleTasks;
using TvProgViewer.Core.Domain.Shipping;
using TvProgViewer.Core.Domain.Vendors;
using TvProgViewer.Data.Mapping;

namespace TvProgViewer.Data.Migrations.UpgradeTo460
{
    [TvProgMigration("2022-07-13 00:00:00", "Update datetime type precision", MigrationProcessType.Update)]
    public class MySqlDateTimeWithPrecisionMigration : Migration
    {
        public override void Up()
        {
            var dataSettings = DataSettingsManager.LoadSettings();

            //update the types only in MySql 
            if (dataSettings.DataProvider != DataProviderType.MySql)
                return;

            Alter.Table(NameCompatibilityManager.GetTableName(typeof(ActivityLog)))
                 .AlterColumn(NameCompatibilityManager.GetColumnName(typeof(ActivityLog), nameof(ActivityLog.CreatedOnUtc)))
                 .AsCustom("datetime(6)");
            Alter.Table(NameCompatibilityManager.GetTableName(typeof(Address)))
                 .AlterColumn(NameCompatibilityManager.GetColumnName(typeof(Address), nameof(Address.CreatedOnUtc)))
                 .AsCustom("datetime(6)");
            Alter.Table(NameCompatibilityManager.GetTableName(typeof(BackInStockSubscription)))
                 .AlterColumn(NameCompatibilityManager.GetColumnName(typeof(BackInStockSubscription), nameof(BackInStockSubscription.CreatedOnUtc)))
                 .AsCustom("datetime(6)");
            Alter.Table(NameCompatibilityManager.GetTableName(typeof(BlogComment)))
                 .AlterColumn(NameCompatibilityManager.GetColumnName(typeof(BlogComment), nameof(BlogComment.CreatedOnUtc)))
                 .AsCustom("datetime(6)");
            Alter.Table(NameCompatibilityManager.GetTableName(typeof(BlogPost)))
                 .AlterColumn(NameCompatibilityManager.GetColumnName(typeof(BlogPost), nameof(BlogPost.CreatedOnUtc)))
                 .AsCustom("datetime(6)");
            Alter.Table(NameCompatibilityManager.GetTableName(typeof(BlogPost)))
                 .AlterColumn(NameCompatibilityManager.GetColumnName(typeof(BlogPost), nameof(BlogPost.EndDateUtc)))
                 .AsCustom("datetime(6)")
                 .Nullable();
            Alter.Table(NameCompatibilityManager.GetTableName(typeof(BlogPost)))
                 .AlterColumn(NameCompatibilityManager.GetColumnName(typeof(BlogPost), nameof(BlogPost.StartDateUtc)))
                 .AsCustom("datetime(6)")
                 .Nullable();
            Alter.Table(NameCompatibilityManager.GetTableName(typeof(Campaign)))
                 .AlterColumn(NameCompatibilityManager.GetColumnName(typeof(Campaign), nameof(Campaign.CreatedOnUtc)))
                 .AsCustom("datetime(6)");
            Alter.Table(NameCompatibilityManager.GetTableName(typeof(Campaign)))
                 .AlterColumn(NameCompatibilityManager.GetColumnName(typeof(Campaign), nameof(Campaign.DontSendBeforeDateUtc)))
                 .AsCustom("datetime(6)")
                 .Nullable();
            Alter.Table(NameCompatibilityManager.GetTableName(typeof(Category)))
                 .AlterColumn(NameCompatibilityManager.GetColumnName(typeof(Category), nameof(Category.CreatedOnUtc)))
                 .AsCustom("datetime(6)");
            Alter.Table(NameCompatibilityManager.GetTableName(typeof(Category)))
                 .AlterColumn(NameCompatibilityManager.GetColumnName(typeof(Category), nameof(Category.UpdatedOnUtc)))
                 .AsCustom("datetime(6)");
            Alter.Table(NameCompatibilityManager.GetTableName(typeof(Currency)))
                 .AlterColumn(NameCompatibilityManager.GetColumnName(typeof(Currency), nameof(Currency.CreatedOnUtc)))
                 .AsCustom("datetime(6)");
            Alter.Table(NameCompatibilityManager.GetTableName(typeof(Currency)))
                 .AlterColumn(NameCompatibilityManager.GetColumnName(typeof(Currency), nameof(Currency.UpdatedOnUtc)))
                 .AsCustom("datetime(6)");
            Alter.Table(NameCompatibilityManager.GetTableName(typeof(User)))
                 .AlterColumn(NameCompatibilityManager.GetColumnName(typeof(User), nameof(User.CannotLoginUntilDateUtc)))
                 .AsCustom("datetime(6)")
                 .Nullable();
            Alter.Table(NameCompatibilityManager.GetTableName(typeof(User)))
                 .AlterColumn(NameCompatibilityManager.GetColumnName(typeof(User), nameof(User.CreatedOnUtc)))
                 .AsCustom("datetime(6)");
            Alter.Table(NameCompatibilityManager.GetTableName(typeof(User)))
                 .AlterColumn(NameCompatibilityManager.GetColumnName(typeof(User), nameof(User.BirthDate)))
                 .AsCustom("datetime(6)")
                 .Nullable();
            Alter.Table(NameCompatibilityManager.GetTableName(typeof(User)))
                 .AlterColumn(NameCompatibilityManager.GetColumnName(typeof(User), nameof(User.LastActivityDateUtc)))
                 .AsCustom("datetime(6)");
            Alter.Table(NameCompatibilityManager.GetTableName(typeof(User)))
                 .AlterColumn(NameCompatibilityManager.GetColumnName(typeof(User), nameof(User.LastLoginDateUtc)))
                 .AsCustom("datetime(6)")
                 .Nullable();
            Alter.Table(NameCompatibilityManager.GetTableName(typeof(UserPassword)))
                 .AlterColumn(NameCompatibilityManager.GetColumnName(typeof(UserPassword), nameof(UserPassword.CreatedOnUtc)))
                 .AsCustom("datetime(6)");
            Alter.Table(NameCompatibilityManager.GetTableName(typeof(Discount)))
                 .AlterColumn(NameCompatibilityManager.GetColumnName(typeof(Discount), nameof(Discount.EndDateUtc)))
                 .AsCustom("datetime(6)")
                 .Nullable();
            Alter.Table(NameCompatibilityManager.GetTableName(typeof(Discount)))
                 .AlterColumn(NameCompatibilityManager.GetColumnName(typeof(Discount), nameof(Discount.StartDateUtc)))
                 .AsCustom("datetime(6)")
                 .Nullable();
            Alter.Table(NameCompatibilityManager.GetTableName(typeof(DiscountUsageHistory)))
                 .AlterColumn(NameCompatibilityManager.GetColumnName(typeof(DiscountUsageHistory), nameof(DiscountUsageHistory.CreatedOnUtc)))
                 .AsCustom("datetime(6)");
            Alter.Table(NameCompatibilityManager.GetTableName(typeof(Forum)))
                 .AlterColumn(NameCompatibilityManager.GetColumnName(typeof(Forum), nameof(Forum.CreatedOnUtc)))
                 .AsCustom("datetime(6)");
            Alter.Table(NameCompatibilityManager.GetTableName(typeof(Forum)))
                 .AlterColumn(NameCompatibilityManager.GetColumnName(typeof(Forum), nameof(Forum.LastPostTime)))
                 .AsCustom("datetime(6)")
                 .Nullable();
            Alter.Table(NameCompatibilityManager.GetTableName(typeof(Forum)))
                 .AlterColumn(NameCompatibilityManager.GetColumnName(typeof(Forum), nameof(Forum.UpdatedOnUtc)))
                 .AsCustom("datetime(6)");
            Alter.Table(NameCompatibilityManager.GetTableName(typeof(ForumGroup)))
                 .AlterColumn(NameCompatibilityManager.GetColumnName(typeof(ForumGroup), nameof(ForumGroup.CreatedOnUtc)))
                 .AsCustom("datetime(6)");
            Alter.Table(NameCompatibilityManager.GetTableName(typeof(ForumGroup)))
                 .AlterColumn(NameCompatibilityManager.GetColumnName(typeof(ForumGroup), nameof(ForumGroup.UpdatedOnUtc)))
                 .AsCustom("datetime(6)");
            Alter.Table(NameCompatibilityManager.GetTableName(typeof(ForumPost)))
                 .AlterColumn(NameCompatibilityManager.GetColumnName(typeof(ForumPost), nameof(ForumPost.CreatedOnUtc)))
                 .AsCustom("datetime(6)");
            Alter.Table(NameCompatibilityManager.GetTableName(typeof(ForumPost)))
                 .AlterColumn(NameCompatibilityManager.GetColumnName(typeof(ForumPost), nameof(ForumPost.UpdatedOnUtc)))
                 .AsCustom("datetime(6)");
            Alter.Table(NameCompatibilityManager.GetTableName(typeof(ForumPostVote)))
                 .AlterColumn(NameCompatibilityManager.GetColumnName(typeof(ForumPostVote), nameof(ForumPostVote.CreatedOnUtc)))
                 .AsCustom("datetime(6)");
            Alter.Table(NameCompatibilityManager.GetTableName(typeof(PrivateMessage)))
                 .AlterColumn(NameCompatibilityManager.GetColumnName(typeof(PrivateMessage), nameof(PrivateMessage.CreatedOnUtc)))
                 .AsCustom("datetime(6)");
            Alter.Table(NameCompatibilityManager.GetTableName(typeof(ForumSubscription)))
                 .AlterColumn(NameCompatibilityManager.GetColumnName(typeof(ForumSubscription), nameof(ForumSubscription.CreatedOnUtc)))
                 .AsCustom("datetime(6)");
            Alter.Table(NameCompatibilityManager.GetTableName(typeof(ForumTopic)))
                 .AlterColumn(NameCompatibilityManager.GetColumnName(typeof(ForumTopic), nameof(ForumTopic.CreatedOnUtc)))
                 .AsCustom("datetime(6)");
            Alter.Table(NameCompatibilityManager.GetTableName(typeof(ForumTopic)))
                 .AlterColumn(NameCompatibilityManager.GetColumnName(typeof(ForumTopic), nameof(ForumTopic.LastPostTime)))
                 .AsCustom("datetime(6)")
                 .Nullable();
            Alter.Table(NameCompatibilityManager.GetTableName(typeof(ForumTopic)))
                 .AlterColumn(NameCompatibilityManager.GetColumnName(typeof(ForumTopic), nameof(ForumTopic.UpdatedOnUtc)))
                 .AsCustom("datetime(6)");
            Alter.Table(NameCompatibilityManager.GetTableName(typeof(GdprLog)))
                 .AlterColumn(NameCompatibilityManager.GetColumnName(typeof(GdprLog), nameof(GdprLog.CreatedOnUtc)))
                 .AsCustom("datetime(6)");
            Alter.Table(NameCompatibilityManager.GetTableName(typeof(GenericAttribute)))
                 .AlterColumn(NameCompatibilityManager.GetColumnName(typeof(GenericAttribute), nameof(GenericAttribute.CreatedOrUpdatedDateUTC)))
                 .AsCustom("datetime(6)")
                 .Nullable();
            Alter.Table(NameCompatibilityManager.GetTableName(typeof(GiftCard)))
                 .AlterColumn(NameCompatibilityManager.GetColumnName(typeof(GiftCard), nameof(GiftCard.CreatedOnUtc)))
                 .AsCustom("datetime(6)");
            Alter.Table(NameCompatibilityManager.GetTableName(typeof(GiftCardUsageHistory)))
                 .AlterColumn(NameCompatibilityManager.GetColumnName(typeof(GiftCardUsageHistory), nameof(GiftCardUsageHistory.CreatedOnUtc)))
                 .AsCustom("datetime(6)");
            Alter.Table(NameCompatibilityManager.GetTableName(typeof(Log)))
                 .AlterColumn(NameCompatibilityManager.GetColumnName(typeof(Log), nameof(Log.CreatedOnUtc)))
                 .AsCustom("datetime(6)");
            Alter.Table(NameCompatibilityManager.GetTableName(typeof(Manufacturer)))
                 .AlterColumn(NameCompatibilityManager.GetColumnName(typeof(Manufacturer), nameof(Manufacturer.CreatedOnUtc)))
                 .AsCustom("datetime(6)");
            Alter.Table(NameCompatibilityManager.GetTableName(typeof(Manufacturer)))
                 .AlterColumn(NameCompatibilityManager.GetColumnName(typeof(Manufacturer), nameof(Manufacturer.UpdatedOnUtc)))
                 .AsCustom("datetime(6)");
            Alter.Table(NameCompatibilityManager.GetTableName(typeof(MigrationVersionInfo)))
                 .AlterColumn(NameCompatibilityManager.GetColumnName(typeof(MigrationVersionInfo), nameof(MigrationVersionInfo.AppliedOn)))
                 .AsCustom("datetime(6)")
                 .Nullable();
            Alter.Table(NameCompatibilityManager.GetTableName(typeof(NewsItem)))
                 .AlterColumn(NameCompatibilityManager.GetColumnName(typeof(NewsItem), nameof(NewsItem.CreatedOnUtc)))
                 .AsCustom("datetime(6)");
            Alter.Table(NameCompatibilityManager.GetTableName(typeof(NewsItem)))
                 .AlterColumn(NameCompatibilityManager.GetColumnName(typeof(NewsItem), nameof(NewsItem.EndDateUtc)))
                 .AsCustom("datetime(6)")
                 .Nullable();
            Alter.Table(NameCompatibilityManager.GetTableName(typeof(NewsItem)))
                 .AlterColumn(NameCompatibilityManager.GetColumnName(typeof(NewsItem), nameof(NewsItem.StartDateUtc)))
                 .AsCustom("datetime(6)")
                 .Nullable();
            Alter.Table(NameCompatibilityManager.GetTableName(typeof(NewsComment)))
                 .AlterColumn(NameCompatibilityManager.GetColumnName(typeof(NewsComment), nameof(NewsComment.CreatedOnUtc)))
                 .AsCustom("datetime(6)");
            Alter.Table(NameCompatibilityManager.GetTableName(typeof(NewsLetterSubscription)))
                 .AlterColumn(NameCompatibilityManager.GetColumnName(typeof(NewsLetterSubscription), nameof(NewsLetterSubscription.CreatedOnUtc)))
                 .AsCustom("datetime(6)");
            Alter.Table(NameCompatibilityManager.GetTableName(typeof(Order)))
                 .AlterColumn(NameCompatibilityManager.GetColumnName(typeof(Order), nameof(Order.CreatedOnUtc)))
                 .AsCustom("datetime(6)");
            Alter.Table(NameCompatibilityManager.GetTableName(typeof(Order)))
                 .AlterColumn(NameCompatibilityManager.GetColumnName(typeof(Order), nameof(Order.PaidDateUtc)))
                 .AsCustom("datetime(6)")
                 .Nullable();
            Alter.Table(NameCompatibilityManager.GetTableName(typeof(OrderItem)))
                 .AlterColumn(NameCompatibilityManager.GetColumnName(typeof(OrderItem), nameof(OrderItem.RentalEndDateUtc)))
                 .AsCustom("datetime(6)")
                 .Nullable();
            Alter.Table(NameCompatibilityManager.GetTableName(typeof(OrderItem)))
                 .AlterColumn(NameCompatibilityManager.GetColumnName(typeof(OrderItem), nameof(OrderItem.RentalStartDateUtc)))
                 .AsCustom("datetime(6)")
                 .Nullable();
            Alter.Table(NameCompatibilityManager.GetTableName(typeof(OrderNote)))
                 .AlterColumn(NameCompatibilityManager.GetColumnName(typeof(OrderNote), nameof(OrderNote.CreatedOnUtc)))
                 .AsCustom("datetime(6)");
            Alter.Table(NameCompatibilityManager.GetTableName(typeof(Poll)))
                 .AlterColumn(NameCompatibilityManager.GetColumnName(typeof(Poll), nameof(Poll.EndDateUtc)))
                 .AsCustom("datetime(6)")
                 .Nullable();
            Alter.Table(NameCompatibilityManager.GetTableName(typeof(Poll)))
                 .AlterColumn(NameCompatibilityManager.GetColumnName(typeof(Poll), nameof(Poll.StartDateUtc)))
                 .AsCustom("datetime(6)")
                 .Nullable();
            Alter.Table(NameCompatibilityManager.GetTableName(typeof(PollVotingRecord)))
                 .AlterColumn(NameCompatibilityManager.GetColumnName(typeof(PollVotingRecord), nameof(PollVotingRecord.CreatedOnUtc)))
                 .AsCustom("datetime(6)");
            Alter.Table(NameCompatibilityManager.GetTableName(typeof(TvChannel)))
                 .AlterColumn(NameCompatibilityManager.GetColumnName(typeof(TvChannel), nameof(TvChannel.AvailableEndDateTimeUtc)))
                 .AsCustom("datetime(6)")
                 .Nullable();
            Alter.Table(NameCompatibilityManager.GetTableName(typeof(TvChannel)))
                 .AlterColumn(NameCompatibilityManager.GetColumnName(typeof(TvChannel), nameof(TvChannel.AvailableStartDateTimeUtc)))
                 .AsCustom("datetime(6)")
                 .Nullable();
            Alter.Table(NameCompatibilityManager.GetTableName(typeof(TvChannel)))
                 .AlterColumn(NameCompatibilityManager.GetColumnName(typeof(TvChannel), nameof(TvChannel.CreatedOnUtc)))
                 .AsCustom("datetime(6)");
            Alter.Table(NameCompatibilityManager.GetTableName(typeof(TvChannel)))
                 .AlterColumn(NameCompatibilityManager.GetColumnName(typeof(TvChannel), nameof(TvChannel.MarkAsNewEndDateTimeUtc)))
                 .AsCustom("datetime(6)")
                 .Nullable();
            Alter.Table(NameCompatibilityManager.GetTableName(typeof(TvChannel)))
                 .AlterColumn(NameCompatibilityManager.GetColumnName(typeof(TvChannel), nameof(TvChannel.MarkAsNewStartDateTimeUtc)))
                 .AsCustom("datetime(6)")
                 .Nullable();
            Alter.Table(NameCompatibilityManager.GetTableName(typeof(TvChannel)))
                 .AlterColumn(NameCompatibilityManager.GetColumnName(typeof(TvChannel), nameof(TvChannel.PreOrderAvailabilityStartDateTimeUtc)))
                 .AsCustom("datetime(6)")
                 .Nullable();
            Alter.Table(NameCompatibilityManager.GetTableName(typeof(TvChannel)))
                 .AlterColumn(NameCompatibilityManager.GetColumnName(typeof(TvChannel), nameof(TvChannel.UpdatedOnUtc)))
                 .AsCustom("datetime(6)");
            Alter.Table(NameCompatibilityManager.GetTableName(typeof(TvChannelReview)))
                 .AlterColumn(NameCompatibilityManager.GetColumnName(typeof(TvChannelReview), nameof(TvChannelReview.CreatedOnUtc)))
                 .AsCustom("datetime(6)");
            Alter.Table(NameCompatibilityManager.GetTableName(typeof(QueuedEmail)))
                 .AlterColumn(NameCompatibilityManager.GetColumnName(typeof(QueuedEmail), nameof(QueuedEmail.CreatedOnUtc)))
                 .AsCustom("datetime(6)");
            Alter.Table(NameCompatibilityManager.GetTableName(typeof(QueuedEmail)))
                 .AlterColumn(NameCompatibilityManager.GetColumnName(typeof(QueuedEmail), nameof(QueuedEmail.DontSendBeforeDateUtc)))
                 .AsCustom("datetime(6)")
                 .Nullable();
            Alter.Table(NameCompatibilityManager.GetTableName(typeof(QueuedEmail)))
                 .AlterColumn(NameCompatibilityManager.GetColumnName(typeof(QueuedEmail), nameof(QueuedEmail.SentOnUtc)))
                 .AsCustom("datetime(6)")
                 .Nullable();
            Alter.Table(NameCompatibilityManager.GetTableName(typeof(RecurringPayment)))
                 .AlterColumn(NameCompatibilityManager.GetColumnName(typeof(RecurringPayment), nameof(RecurringPayment.CreatedOnUtc)))
                 .AsCustom("datetime(6)");
            Alter.Table(NameCompatibilityManager.GetTableName(typeof(RecurringPayment)))
                 .AlterColumn(NameCompatibilityManager.GetColumnName(typeof(RecurringPayment), nameof(RecurringPayment.StartDateUtc)))
                 .AsCustom("datetime(6)");
            Alter.Table(NameCompatibilityManager.GetTableName(typeof(RecurringPaymentHistory)))
                 .AlterColumn(NameCompatibilityManager.GetColumnName(typeof(RecurringPaymentHistory), nameof(RecurringPaymentHistory.CreatedOnUtc)))
                 .AsCustom("datetime(6)");
            Alter.Table(NameCompatibilityManager.GetTableName(typeof(ReturnRequest)))
                 .AlterColumn(NameCompatibilityManager.GetColumnName(typeof(ReturnRequest), nameof(ReturnRequest.CreatedOnUtc)))
                 .AsCustom("datetime(6)");
            Alter.Table(NameCompatibilityManager.GetTableName(typeof(ReturnRequest)))
                 .AlterColumn(NameCompatibilityManager.GetColumnName(typeof(ReturnRequest), nameof(ReturnRequest.UpdatedOnUtc)))
                 .AsCustom("datetime(6)");
            Alter.Table(NameCompatibilityManager.GetTableName(typeof(RewardPointsHistory)))
                 .AlterColumn(NameCompatibilityManager.GetColumnName(typeof(RewardPointsHistory), nameof(RewardPointsHistory.CreatedOnUtc)))
                 .AsCustom("datetime(6)");
            Alter.Table(NameCompatibilityManager.GetTableName(typeof(RewardPointsHistory)))
                 .AlterColumn(NameCompatibilityManager.GetColumnName(typeof(RewardPointsHistory), nameof(RewardPointsHistory.EndDateUtc)))
                 .AsCustom("datetime(6)")
                 .Nullable();
            Alter.Table(NameCompatibilityManager.GetTableName(typeof(ScheduleTask)))
                 .AlterColumn(NameCompatibilityManager.GetColumnName(typeof(ScheduleTask), nameof(ScheduleTask.LastEnabledUtc)))
                 .AsCustom("datetime(6)")
                 .Nullable();
            Alter.Table(NameCompatibilityManager.GetTableName(typeof(ScheduleTask)))
                 .AlterColumn(NameCompatibilityManager.GetColumnName(typeof(ScheduleTask), nameof(ScheduleTask.LastEndUtc)))
                 .AsCustom("datetime(6)")
                 .Nullable();
            Alter.Table(NameCompatibilityManager.GetTableName(typeof(ScheduleTask)))
                 .AlterColumn(NameCompatibilityManager.GetColumnName(typeof(ScheduleTask), nameof(ScheduleTask.LastStartUtc)))
                 .AsCustom("datetime(6)")
                 .Nullable();
            Alter.Table(NameCompatibilityManager.GetTableName(typeof(ScheduleTask)))
                 .AlterColumn(NameCompatibilityManager.GetColumnName(typeof(ScheduleTask), nameof(ScheduleTask.LastSuccessUtc)))
                 .AsCustom("datetime(6)")
                 .Nullable();
            Alter.Table(NameCompatibilityManager.GetTableName(typeof(Shipment)))
                 .AlterColumn(NameCompatibilityManager.GetColumnName(typeof(Shipment), nameof(Shipment.CreatedOnUtc)))
                 .AsCustom("datetime(6)");
            Alter.Table(NameCompatibilityManager.GetTableName(typeof(Shipment)))
                 .AlterColumn(NameCompatibilityManager.GetColumnName(typeof(Shipment), nameof(Shipment.DeliveryDateUtc)))
                 .AsCustom("datetime(6)")
                 .Nullable();
            Alter.Table(NameCompatibilityManager.GetTableName(typeof(Shipment)))
                 .AlterColumn(NameCompatibilityManager.GetColumnName(typeof(Shipment), nameof(Shipment.ReadyForPickupDateUtc)))
                 .AsCustom("datetime(6)")
                 .Nullable();
            Alter.Table(NameCompatibilityManager.GetTableName(typeof(Shipment)))
                 .AlterColumn(NameCompatibilityManager.GetColumnName(typeof(Shipment), nameof(Shipment.ShippedDateUtc)))
                 .AsCustom("datetime(6)")
                 .Nullable();
            Alter.Table(NameCompatibilityManager.GetTableName(typeof(ShoppingCartItem)))
                 .AlterColumn(NameCompatibilityManager.GetColumnName(typeof(ShoppingCartItem), nameof(ShoppingCartItem.CreatedOnUtc)))
                 .AsCustom("datetime(6)");
            Alter.Table(NameCompatibilityManager.GetTableName(typeof(ShoppingCartItem)))
                 .AlterColumn(NameCompatibilityManager.GetColumnName(typeof(ShoppingCartItem), nameof(ShoppingCartItem.RentalEndDateUtc)))
                 .AsCustom("datetime(6)")
                 .Nullable();
            Alter.Table(NameCompatibilityManager.GetTableName(typeof(ShoppingCartItem)))
                 .AlterColumn(NameCompatibilityManager.GetColumnName(typeof(ShoppingCartItem), nameof(ShoppingCartItem.RentalStartDateUtc)))
                 .AsCustom("datetime(6)")
                 .Nullable();
            Alter.Table(NameCompatibilityManager.GetTableName(typeof(ShoppingCartItem)))
                 .AlterColumn(NameCompatibilityManager.GetColumnName(typeof(ShoppingCartItem), nameof(ShoppingCartItem.UpdatedOnUtc)))
                 .AsCustom("datetime(6)");
            Alter.Table(NameCompatibilityManager.GetTableName(typeof(StockQuantityHistory)))
                 .AlterColumn(NameCompatibilityManager.GetColumnName(typeof(StockQuantityHistory), nameof(StockQuantityHistory.CreatedOnUtc)))
                 .AsCustom("datetime(6)");
            Alter.Table(NameCompatibilityManager.GetTableName(typeof(TierPrice)))
                 .AlterColumn(NameCompatibilityManager.GetColumnName(typeof(TierPrice), nameof(TierPrice.EndDateTimeUtc)))
                 .AsCustom("datetime(6)")
                 .Nullable();
            Alter.Table(NameCompatibilityManager.GetTableName(typeof(TierPrice)))
                 .AlterColumn(NameCompatibilityManager.GetColumnName(typeof(TierPrice), nameof(TierPrice.StartDateTimeUtc)))
                 .AsCustom("datetime(6)")
                 .Nullable();
            Alter.Table(NameCompatibilityManager.GetTableName(typeof(VendorNote)))
                 .AlterColumn(NameCompatibilityManager.GetColumnName(typeof(VendorNote), nameof(VendorNote.CreatedOnUtc)))
                 .AsCustom("datetime(6)");
        }

        public override void Down()
        {
            //add the downgrade Logic if necessary 
        }
    }
}