using FluentMigrator;
using TVProgViewer.Core.Domain.Security;
using TVProgViewer.Data.Extensions;

namespace TVProgViewer.Data.Migrations.Indexes
{
    [TvProgMigration("2019/12/19 11:35:09:1647929")]
    public class AddAclRecordEntityIdEntityNameIX : AutoReversingMigration
    {
        #region Methods

        public override void Up()
        {
            this.AddIndex("IX_AclRecord_EntityId_EntityName", nameof(AclRecord), i => i.Ascending(),
                nameof(AclRecord.EntityId), nameof(AclRecord.EntityName));
        }

        #endregion
    }
}