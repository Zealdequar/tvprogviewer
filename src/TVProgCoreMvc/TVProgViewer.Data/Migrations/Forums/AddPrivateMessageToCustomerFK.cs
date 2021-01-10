using FluentMigrator;
using TVProgViewer.Core.Domain.Users;
using TVProgViewer.Core.Domain.Forums;
using TVProgViewer.Data.Extensions;

namespace TVProgViewer.Data.Migrations.Forums
{
    [TvProgMigration("2019/11/19 04:50:37:3669696")]
    public class AddPrivateMessageToUserFK : AutoReversingMigration
    {
        #region Methods

        public override void Up()
        {
            
        }

        #endregion
    }
}