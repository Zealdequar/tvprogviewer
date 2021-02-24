using FluentMigrator;

namespace TVProgViewer.Data.Migrations.Indexes
{
    [TvProgMigration("2020/03/13 11:35:09:1647939")]
    public class AddUserUserRoleMappingUserIdIX : AutoReversingMigration
    {
        #region Methods

        public override void Up()
        {
            //Create.Index("IX_User_UserRole_Mapping_User_Id").OnTable(NameCompatibilityManager.GetTableName(typeof(UserUserRoleMapping)))
            //    .OnColumn(NameCompatibilityManager.GetColumnName(typeof(UserUserRoleMapping), nameof(UserUserRoleMapping.UserId))).Ascending()
            //    .WithOptions().NonClustered();
        }

        #endregion
    }
}