
using FluentMigrator;
using TVProgViewer.Core.Domain.TvProgMain;
using TVProgViewer.Core.Domain.Users;
using TVProgViewer.Data.Extensions;

namespace TVProgViewer.Data.Migrations.TvProgMain
{
    [TvProgMigration("2020/06/28 22:55:49:2958609")]
    public class AddUsersProgramsUsersFK: AutoReversingMigration
    {
        #region Методы

        public override void Up()
        {
            this.AddForeignKey(nameof(UsersPrograms),
                nameof(UsersPrograms.UserId),
                nameof(User),
                nameof(User.Id));
        }

        #endregion
    }
}
