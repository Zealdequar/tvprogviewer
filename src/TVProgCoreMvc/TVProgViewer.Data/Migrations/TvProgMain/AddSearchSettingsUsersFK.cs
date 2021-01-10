
using FluentMigrator;
using TVProgViewer.Core.Domain.TvProgMain;
using TVProgViewer.Core.Domain.Users;
using TVProgViewer.Data.Extensions;

namespace TVProgViewer.Data.Migrations.TvProgMain
{
    [TvProgMigration("2020/06/28 22:36:13:7363662")]
    public class AddSearchSettingsUsersFK: AutoReversingMigration
    {
        #region Методы

        public override void Up()
        {
            this.AddForeignKey(nameof(SearchSettings),
                nameof(SearchSettings.UserId),
                nameof(User),
                nameof(User.Id));
        }
        #endregion
    }
}
