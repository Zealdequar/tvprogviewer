using System.Data;
using FluentMigrator;
using TVProgViewer.Core.Domain.Messages;
using TVProgViewer.Data.Extensions;

namespace TVProgViewer.Data.Migrations.Messages
{
    [TvProgMigration("2019/11/19 05:01:43:1655781")]
    public class AddQueuedEmailEmailAccountFK : AutoReversingMigration
    {
        #region Methods

        public override void Up()
        {
            this.AddForeignKey(nameof(QueuedEmail),
                nameof(QueuedEmail.EmailAccountId),
                nameof(EmailAccount),
                nameof(EmailAccount.Id),
                Rule.Cascade);
        }

        #endregion
    }
}