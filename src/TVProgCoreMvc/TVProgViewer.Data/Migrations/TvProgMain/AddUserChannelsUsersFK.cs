

using FluentMigrator;
using System.Runtime.CompilerServices;
using TVProgViewer.Core.Domain.TvProgMain;
using TVProgViewer.Core.Domain.Users;
using TVProgViewer.Data.Extensions;
using TVProgViewer.Data.TvProgMain.ProgObjs;

namespace TVProgViewer.Data.Migrations.TvProgMain
{
    [TvProgMigration("2020/06/28 22:45:14:9127679")]
    public class AddUserChannelsUsersFK: AutoReversingMigration
    {
        #region Методы

        public override void Up()
        {
            this.AddForeignKey(nameof(UserChannels),
                nameof(UserChannels.UserId),
                nameof(User),
                nameof(User.Id));
        }
        #endregion
    }
}
