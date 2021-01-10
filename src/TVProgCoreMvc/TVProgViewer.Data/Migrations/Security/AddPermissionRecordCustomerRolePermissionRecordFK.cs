using System.Data;
using FluentMigrator;
using TVProgViewer.Core.Domain.Security;
using TVProgViewer.Data.Extensions;
using TVProgViewer.Data.Mapping;

namespace TVProgViewer.Data.Migrations.Security
{
    [TvProgMigration("2019/11/19 05:38:30:7801302")]
    public class AddPermissionRecordUserRolePermissionRecordFK : AutoReversingMigration
    {
        #region Methods

        public override void Up()
        {
            this.AddForeignKey(TvProgMappingDefaults.PermissionRecordRoleTable,
                "PermissionRecord_Id",
                nameof(PermissionRecord),
                nameof(PermissionRecord.Id),
                Rule.Cascade);
        }

        #endregion
    }
}