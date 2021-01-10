using System.Data;
using FluentMigrator;
using TVProgViewer.Core.Domain.Users;
using TVProgViewer.Core.Domain.Security;
using TVProgViewer.Data.Extensions;

namespace TVProgViewer.Data.Migrations.Security
{
    [TvProgMigration("2019/11/19 05:37:23:6073081")]
    public class AddAclRecordUserRoleFK : AutoReversingMigration
    {
        #region Methods

        public override void Up()
        {
            this.AddForeignKey(nameof(AclRecord),
                nameof(AclRecord.UserRoleId),
                nameof(UserRole),
                nameof(UserRole.Id),
                Rule.Cascade);
        }

        #endregion
    }
}