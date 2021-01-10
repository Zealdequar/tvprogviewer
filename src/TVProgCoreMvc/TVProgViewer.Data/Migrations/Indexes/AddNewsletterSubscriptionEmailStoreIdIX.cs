using FluentMigrator;
using TVProgViewer.Core.Domain.Messages;
using TVProgViewer.Data.Extensions;

namespace TVProgViewer.Data.Migrations.Indexes
{
    [TvProgMigration("2019/12/19 09:36:08:9037690")]
    public class AddNewsletterSubscriptionEmailStoreIdIX : AutoReversingMigration
    {
        #region Methods          

        public override void Up()
        {
            this.AddIndex("IX_NewsletterSubscription_Email_StoreId", nameof(NewsLetterSubscription), i => i.Ascending(),
                nameof(NewsLetterSubscription.Email), nameof(NewsLetterSubscription.StoreId));
        }

        #endregion
    }
}