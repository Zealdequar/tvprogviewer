using System.Data;
using FluentMigrator;
using TVProgViewer.Core.Domain.Users;
using TVProgViewer.Data.Extensions;
using TVProgViewer.Data.Mapping;

namespace TVProgViewer.Data.Migrations.Users
{
    [TvProgMigration("2019/11/19 02:25:23:7489897")]
    public class AddUserUserRoleUserRoleFK : AutoReversingMigration
    {
        #region Methods

        public override void Up()
        {
            this.AddForeignKey(TvProgMappingDefaults.UserUserRoleTable,
                "UserRole_Id",
                nameof(UserRole),
                nameof(UserRole.Id),
                Rule.Cascade);
        }

        #endregion
    }
}