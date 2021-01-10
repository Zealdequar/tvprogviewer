using FluentMigrator;
using TVProgViewer.Core.Domain.TvProgMain;
using TVProgViewer.Core.Domain.Users;
using TVProgViewer.Data.Extensions;

namespace TVProgViewer.Data.Migrations.TvProgMain
{
    [TvProgMigration("2020/06/28 21:01:42:2173985")]
    public class AddExtUserSettingsUsersFK: AutoReversingMigration
    {
        #region Методы

        public override void Up()
        {
            this.AddForeignKey(nameof(ExtUserSettings),
                nameof(ExtUserSettings.UserId),
                nameof(User),
                nameof(User.Id));
        }
        #endregion
    }
}
