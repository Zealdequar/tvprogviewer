using FluentMigrator;
using TVProgViewer.Data.Extensions;
using TVProgViewer.Data.Mapping;

namespace TVProgViewer.Data.Migrations.Indexes
{
    [TvProgMigration("2019/12/19 11:35:09:1647939")]
    public class AddUserUserRoleMappingUserIdIX : AutoReversingMigration
    {
        #region Methods

        public override void Up()
        {
            this.AddIndex("IX_User_UserRole_Mapping_User_Id", TvProgMappingDefaults.UserUserRoleTable,
                i => i.Ascending(), "User_Id");
        }

        #endregion
    }
}