using FluentMigrator;
using FluentMigrator.SqlServer;
using TvProgViewer.Core.Domain.Catalog;
using TvProgViewer.Core.Domain.Common;
using TvProgViewer.Core.Domain.Users;
using TvProgViewer.Core.Domain.Directory;
using TvProgViewer.Core.Domain.Forums;
using TvProgViewer.Core.Domain.Localization;
using TvProgViewer.Core.Domain.Logging;
using TvProgViewer.Core.Domain.Messages;
using TvProgViewer.Core.Domain.Orders;
using TvProgViewer.Core.Domain.Security;
using TvProgViewer.Core.Domain.Seo;
using TvProgViewer.Core.Domain.Stores;
using TvProgViewer.Data.Mapping;

namespace TvProgViewer.Data.Migrations.Installation
{
    [TvProgMigration("2020/03/13 09:36:08:9037677", "TvProg.Data base indexes", MigrationProcessType.Installation)]
    public class Indexes : AutoReversingMigration
    {
        #region Methods

        public override void Up()
        {
            Create.Index("IX_UrlRecord_Slug")
                .OnTable(nameof(UrlRecord))
                .OnColumn(nameof(UrlRecord.Slug))
                .Ascending()
                .WithOptions()
                .NonClustered();

            Create.Index("IX_UrlRecord_Custom_1").OnTable(nameof(UrlRecord))
                .OnColumn(nameof(UrlRecord.EntityId)).Ascending()
                .OnColumn(nameof(UrlRecord.EntityName)).Ascending()
                .OnColumn(nameof(UrlRecord.LanguageId)).Ascending()
                .OnColumn(nameof(UrlRecord.IsActive)).Ascending()
                .WithOptions().NonClustered();

            Create.Index("IX_StoreMapping_EntityId_EntityName").OnTable(nameof(StoreMapping))
                .OnColumn(nameof(StoreMapping.EntityId)).Ascending()
                .OnColumn(nameof(StoreMapping.EntityName)).Ascending()
                .WithOptions().NonClustered();

            Create.Index("IX_ShoppingCartItem_ShoppingCartTypeId_UserId").OnTable(nameof(ShoppingCartItem))
                .OnColumn(nameof(ShoppingCartItem.ShoppingCartTypeId)).Ascending()
                .OnColumn(nameof(ShoppingCartItem.UserId)).Ascending()
                .WithOptions().NonClustered();

            Create.Index("IX_RelatedTvChannel_TvChannelId1").OnTable(nameof(RelatedTvChannel))
                .OnColumn(nameof(RelatedTvChannel.TvChannelId1)).Ascending()
                .WithOptions().NonClustered();

            Create.Index("IX_QueuedEmail_SentOnUtc_DontSendBeforeDateUtc_Extended").OnTable(nameof(QueuedEmail))
                .OnColumn(nameof(QueuedEmail.SentOnUtc)).Ascending()
                .OnColumn(nameof(QueuedEmail.DontSendBeforeDateUtc)).Ascending()
                .WithOptions().NonClustered()
                .Include(nameof(QueuedEmail.SentTries));

            Create.Index("IX_QueuedEmail_CreatedOnUtc").OnTable(nameof(QueuedEmail))
                .OnColumn(nameof(QueuedEmail.CreatedOnUtc)).Descending()
                .WithOptions().NonClustered();

            Create.Index("IX_PSAM_SpecificationAttributeOptionId_AllowFiltering").OnTable(NameCompatibilityManager.GetTableName(typeof(TvChannelSpecificationAttribute)))
                .OnColumn(nameof(TvChannelSpecificationAttribute.SpecificationAttributeOptionId)).Ascending()
                .OnColumn(nameof(TvChannelSpecificationAttribute.AllowFiltering)).Ascending()
                .WithOptions().NonClustered()
                .Include(nameof(TvChannelSpecificationAttribute.TvChannelId));

            Create.Index("IX_PSAM_AllowFiltering").OnTable(NameCompatibilityManager.GetTableName(typeof(TvChannelSpecificationAttribute)))
                .OnColumn(nameof(TvChannelSpecificationAttribute.AllowFiltering)).Ascending()
                .WithOptions().NonClustered()
                .Include(nameof(TvChannelSpecificationAttribute.TvChannelId))
                .Include(nameof(TvChannelSpecificationAttribute.SpecificationAttributeOptionId));

            Create.Index("IX_TvChannel_VisibleIndividually_Published_Deleted_Extended").OnTable(nameof(TvChannel))
                .OnColumn(nameof(TvChannel.VisibleIndividually)).Ascending()
                .OnColumn(nameof(TvChannel.Published)).Ascending()
                .OnColumn(nameof(TvChannel.Deleted)).Ascending()
                .WithOptions().NonClustered()
                .Include(nameof(TvChannel.Id))
                .Include(nameof(TvChannel.AvailableStartDateTimeUtc))
                .Include(nameof(TvChannel.AvailableEndDateTimeUtc));

            Create.Index("IX_TvChannel_VisibleIndividually").OnTable(nameof(TvChannel))
                .OnColumn(nameof(TvChannel.VisibleIndividually)).Ascending()
                .WithOptions().NonClustered();

            Create.Index("IX_TvChannelTag_Name").OnTable(nameof(TvChannelTag))
                .OnColumn(nameof(TvChannelTag.Name)).Ascending()
                .WithOptions().NonClustered();

            Create.Index("IX_TvChannel_SubjectToAcl").OnTable(nameof(TvChannel))
                .OnColumn(nameof(TvChannel.SubjectToAcl)).Ascending()
                .WithOptions().NonClustered();

            Create.Index("IX_TvChannel_ShowOnHomepage").OnTable(nameof(TvChannel))
                .OnColumn(nameof(TvChannel.ShowOnHomepage)).Ascending()
                .WithOptions().NonClustered();

            Create.Index("IX_TvChannel_Published").OnTable(nameof(TvChannel))
                .OnColumn(nameof(TvChannel.Published)).Ascending()
                .WithOptions().NonClustered();

            Create.Index("IX_TvChannel_TvChannelAttribute_Mapping_TvChannelId_DisplayOrder").OnTable(NameCompatibilityManager.GetTableName(typeof(TvChannelAttributeMapping)))
                .OnColumn(nameof(TvChannelAttributeMapping.TvChannelId)).Ascending()
                .OnColumn(nameof(TvChannelAttributeMapping.DisplayOrder)).Ascending()
                .WithOptions().NonClustered();

            Create.Index("IX_TvChannel_PriceDatesEtc").OnTable(nameof(TvChannel))
                .OnColumn(nameof(TvChannel.Price)).Ascending()
                .OnColumn(nameof(TvChannel.AvailableStartDateTimeUtc)).Ascending()
                .OnColumn(nameof(TvChannel.AvailableEndDateTimeUtc)).Ascending()
                .OnColumn(nameof(TvChannel.Published)).Ascending()
                .OnColumn(nameof(TvChannel.Deleted)).Ascending()
                .WithOptions().NonClustered();

            Create.Index("IX_TvChannel_ParentGroupedTvChannelId").OnTable(nameof(TvChannel))
                .OnColumn(nameof(TvChannel.ParentGroupedTvChannelId)).Ascending()
                .WithOptions().NonClustered();

            Create.Index("IX_TvChannel_Manufacturer_Mapping_IsFeaturedTvChannel")
                .OnTable(NameCompatibilityManager.GetTableName(typeof(TvChannelManufacturer)))
                .OnColumn(nameof(TvChannelManufacturer.IsFeaturedTvChannel)).Ascending()
                .WithOptions().NonClustered();

            Create.Index("IX_TvChannel_LimitedToStores").OnTable(nameof(TvChannel))
                .OnColumn(nameof(TvChannel.LimitedToStores)).Ascending()
                .WithOptions().NonClustered();

            Create.Index("IX_TvChannel_Delete_Id").OnTable(nameof(TvChannel))
                .OnColumn(nameof(TvChannel.Deleted)).Ascending()
                .OnColumn(nameof(TvChannel.Id)).Ascending()
                .WithOptions().NonClustered();

            Create.Index("IX_TvChannel_Deleted_and_Published").OnTable(nameof(TvChannel))
                .OnColumn(nameof(TvChannel.Published)).Ascending()
                .OnColumn(nameof(TvChannel.Deleted)).Ascending()
                .WithOptions().NonClustered();

            Create.Index("IX_TvChannel_Category_Mapping_IsFeaturedTvChannel").OnTable(NameCompatibilityManager.GetTableName(typeof(TvChannelCategory)))
                .OnColumn(nameof(TvChannelCategory.IsFeaturedTvChannel)).Ascending()
                .WithOptions().NonClustered();

            Create.Index("IX_TvChannelAttributeValue_TvChannelAttributeMappingId_DisplayOrder").OnTable(nameof(TvChannelAttributeValue))
                .OnColumn(nameof(TvChannelAttributeValue.TvChannelAttributeMappingId)).Ascending()
                .OnColumn(nameof(TvChannelAttributeValue.DisplayOrder)).Ascending()
                .WithOptions().NonClustered();

            Create.Index("IX_PMM_TvChannel_and_Manufacturer").OnTable(NameCompatibilityManager.GetTableName(typeof(TvChannelManufacturer)))
                .OnColumn(nameof(TvChannelManufacturer.ManufacturerId)).Ascending()
                .OnColumn(nameof(TvChannelManufacturer.TvChannelId)).Ascending()
                .WithOptions().NonClustered();

            Create.Index("IX_PMM_TvChannelId_Extended").OnTable(NameCompatibilityManager.GetTableName(typeof(TvChannelManufacturer)))
                .OnColumn(nameof(TvChannelManufacturer.TvChannelId)).Ascending()
                .OnColumn(nameof(TvChannelManufacturer.IsFeaturedTvChannel)).Ascending()
                .WithOptions().NonClustered()
                .Include(nameof(TvChannelManufacturer.ManufacturerId));

            Create.Index("IX_PCM_TvChannelId_Extended").OnTable(NameCompatibilityManager.GetTableName(typeof(TvChannelCategory)))
                .OnColumn(nameof(TvChannelCategory.TvChannelId)).Ascending()
                .OnColumn(nameof(TvChannelCategory.IsFeaturedTvChannel)).Ascending()
                .WithOptions().NonClustered()
                .Include(nameof(TvChannelCategory.CategoryId));

            Create.Index("IX_PCM_TvChannel_and_Category").OnTable(NameCompatibilityManager.GetTableName(typeof(TvChannelCategory)))
                .OnColumn(nameof(TvChannelCategory.CategoryId)).Ascending()
                .OnColumn(nameof(TvChannelCategory.TvChannelId)).Ascending()
                .WithOptions().NonClustered();

            Create.Index("IX_Order_CreatedOnUtc").OnTable(nameof(Order))
                .OnColumn(nameof(Order.CreatedOnUtc)).Descending()
                .WithOptions().NonClustered();

            Create.Index("IX_NewsletterSubscription_Email_StoreId").OnTable(nameof(NewsLetterSubscription))
                .OnColumn(nameof(NewsLetterSubscription.Email)).Ascending()
                .OnColumn(nameof(NewsLetterSubscription.StoreId)).Ascending()
                .WithOptions().NonClustered();

            Create.Index("IX_Manufacturer_SubjectToAcl").OnTable(nameof(Manufacturer))
                .OnColumn(nameof(Manufacturer.SubjectToAcl)).Ascending()
                .WithOptions().NonClustered();

            Create.Index("IX_Manufacturer_LimitedToStores").OnTable(nameof(Manufacturer))
                .OnColumn(nameof(Manufacturer.LimitedToStores)).Ascending()
                .WithOptions().NonClustered();

            Create.Index("IX_Manufacturer_DisplayOrder").OnTable(nameof(Manufacturer))
                .OnColumn(nameof(Manufacturer.DisplayOrder)).Ascending()
                .WithOptions().NonClustered();

            Create.Index("IX_Log_CreatedOnUtc").OnTable(nameof(Log))
                .OnColumn(nameof(Log.CreatedOnUtc)).Descending()
                .WithOptions().NonClustered();

            Create.Index("IX_LocaleStringResource").OnTable(nameof(LocaleStringResource))
                .OnColumn(nameof(LocaleStringResource.ResourceName)).Ascending()
                .OnColumn(nameof(LocaleStringResource.LanguageId)).Ascending()
                .WithOptions().NonClustered();

            Create.Index("IX_Language_DisplayOrder").OnTable(nameof(Language))
                .OnColumn(nameof(Language.DisplayOrder)).Ascending()
                .WithOptions().NonClustered();

            Create.Index("IX_GetLowStockTvChannels").OnTable(nameof(TvChannel))
                .OnColumn(nameof(TvChannel.Deleted)).Ascending()
                .OnColumn(nameof(TvChannel.VendorId)).Ascending()
                .OnColumn(nameof(TvChannel.TvChannelTypeId)).Ascending()
                .OnColumn(nameof(TvChannel.ManageInventoryMethodId)).Ascending()
                .OnColumn(nameof(TvChannel.MinStockQuantity)).Ascending()
                .OnColumn(nameof(TvChannel.UseMultipleWarehouses)).Ascending()
                .WithOptions().NonClustered();

            Create.Index("IX_GenericAttribute_EntityId_and_KeyGroup").OnTable(nameof(GenericAttribute))
                .OnColumn(nameof(GenericAttribute.EntityId)).Ascending()
                .OnColumn(nameof(GenericAttribute.KeyGroup)).Ascending()
                .WithOptions().NonClustered();

            Create.Index("IX_Forums_Subscription_TopicId").OnTable(NameCompatibilityManager.GetTableName(typeof(ForumSubscription)))
                .OnColumn(nameof(ForumSubscription.TopicId)).Ascending()
                .WithOptions().NonClustered();

            Create.Index("IX_Forums_Subscription_ForumId").OnTable(NameCompatibilityManager.GetTableName(typeof(ForumSubscription)))
                .OnColumn(nameof(ForumSubscription.ForumId)).Ascending()
                .WithOptions().NonClustered();

            Create.Index("IX_Forums_Group_DisplayOrder").OnTable(NameCompatibilityManager.GetTableName(typeof(ForumGroup)))
                .OnColumn(nameof(ForumGroup.DisplayOrder)).Ascending()
                .WithOptions().NonClustered();

            Create.Index("IX_Forums_Forum_DisplayOrder").OnTable(NameCompatibilityManager.GetTableName(typeof(Forum)))
                .OnColumn(nameof(Forum.DisplayOrder)).Ascending()
                .WithOptions().NonClustered();

            Create.Index("IX_User_Username").OnTable(nameof(User))
                .OnColumn(nameof(User.Username)).Ascending()
                .WithOptions().NonClustered();

            Create.Index("IX_User_SystemName").OnTable(nameof(User))
                .OnColumn(nameof(User.SystemName)).Ascending()
                .WithOptions().NonClustered();

            Create.Index("IX_User_Email").OnTable(nameof(User))
                .OnColumn(nameof(User.Email)).Ascending()
                .WithOptions().NonClustered();

            Create.Index("IX_User_UserGuid").OnTable(nameof(User))
                .OnColumn(nameof(User.UserGuid)).Ascending()
                .WithOptions().NonClustered();

            Create.Index("IX_User_CreatedOnUtc").OnTable(nameof(User))
                .OnColumn(nameof(User.CreatedOnUtc)).Descending()
                .WithOptions().NonClustered();

            Create.Index("IX_Currency_DisplayOrder").OnTable(nameof(Currency))
                .OnColumn(nameof(Currency.DisplayOrder)).Ascending()
                .WithOptions().NonClustered();

            Create.Index("IX_Country_DisplayOrder").OnTable(nameof(Country))
                .OnColumn(nameof(Country.DisplayOrder)).Ascending()
                .WithOptions().NonClustered();

            Create.Index("IX_Category_ParentCategoryId").OnTable(nameof(Category))
                .OnColumn(nameof(Category.ParentCategoryId)).Ascending()
                .WithOptions().NonClustered();

            Create.Index("IX_Category_LimitedToStores").OnTable(nameof(Category))
                .OnColumn(nameof(Category.LimitedToStores)).Ascending()
                .WithOptions().NonClustered();

            Create.Index("IX_Category_DisplayOrder").OnTable(nameof(Category))
                .OnColumn(nameof(Category.DisplayOrder)).Ascending()
                .WithOptions().NonClustered();

            Create.Index("IX_Category_Deleted_Extended").OnTable(nameof(Category))
                .OnColumn(nameof(Category.Deleted)).Ascending()
                .WithOptions().NonClustered()
                .Include(nameof(Category.Id))
                .Include(nameof(Category.Name))
                .Include(nameof(Category.SubjectToAcl)).Include(nameof(Category.LimitedToStores))
                .Include(nameof(Category.Published));

            Create.Index("IX_Category_SubjectToAcl").OnTable(nameof(Category))
                .OnColumn(nameof(Category.SubjectToAcl)).Ascending()
                .WithOptions().NonClustered();

            Create.Index("IX_ActivityLog_CreatedOnUtc").OnTable(nameof(ActivityLog))
                .OnColumn(nameof(ActivityLog.CreatedOnUtc)).Descending()
                .WithOptions().NonClustered();

            Create.Index("IX_AclRecord_EntityId_EntityName").OnTable(nameof(AclRecord))
                .OnColumn(nameof(AclRecord.EntityId)).Ascending()
                .OnColumn(nameof(AclRecord.EntityName)).Ascending()
                .WithOptions().NonClustered();
        }

        #endregion
    }
}
