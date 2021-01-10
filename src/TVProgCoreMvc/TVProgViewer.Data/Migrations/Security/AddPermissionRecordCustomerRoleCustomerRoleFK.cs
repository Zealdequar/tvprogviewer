using System.Data;
using FluentMigrator;
using TVProgViewer.Core.Domain.Users;
using TVProgViewer.Data.Extensions;
using TVProgViewer.Data.Mapping;

namespace TVProgViewer.Data.Migrations.Security
{
    [TvProgMigration("2019/11/19 05:38:30:7801301")]
    public class AddPermissionRecordUserRoleUserRoleFK : AutoReversingMigration
    {
        #region Methods

        public override void Up()
        {
            this.AddForeignKey(TvProgMappingDefaults.PermissionRecordRoleTable,
                "UserRole_Id",
                nameof(UserRole),
                nameof(UserRole.Id),
                Rule.Cascade);
        }

        #endregion
    }
}