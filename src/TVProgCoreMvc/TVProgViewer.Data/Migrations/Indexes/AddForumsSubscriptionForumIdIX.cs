using FluentMigrator;
using TVProgViewer.Core.Domain.Forums;
using TVProgViewer.Data.Extensions;
using TVProgViewer.Data.Mapping;

namespace TVProgViewer.Data.Migrations.Indexes
{
    [TvProgMigration("2019/12/19 09:36:08:9037700")]
    public class AddForumsSubscriptionForumIdIX : AutoReversingMigration
    {
        #region Methods          

        public override void Up()
        {
            this.AddIndex("IX_Forums_Subscription_ForumId", TvProgMappingDefaults.ForumsSubscriptionTable,
                i => i.Ascending(), nameof(ForumSubscription.ForumId));
        }

        #endregion
    }
}