using FluentMigrator;
using FluentMigrator.SqlServer;
using TVProgViewer.Core.Domain.Messages;
using TVProgViewer.Data.Extensions;

namespace TVProgViewer.Data.Migrations.Indexes
{
    [TvProgMigration("2019/12/19 12:02:35:9280393")]
    public class AddQueuedEmailSentOnUtcDontSendBeforeDateUtcExtendedIX : AutoReversingMigration
    {
        #region Methods          

        public override void Up()
        {
            this.AddIndex("IX_QueuedEmail_SentOnUtc_DontSendBeforeDateUtc_Extended", nameof(QueuedEmail),
                    i => i.Ascending(), nameof(QueuedEmail.SentOnUtc), nameof(QueuedEmail.DontSendBeforeDateUtc))
                .Include(nameof(QueuedEmail.SentTries));
        }

        #endregion
    }
}