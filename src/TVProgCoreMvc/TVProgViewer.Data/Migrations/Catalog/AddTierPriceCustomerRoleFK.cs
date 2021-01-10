using System.Data;
using FluentMigrator;
using TVProgViewer.Core.Domain.Catalog;
using TVProgViewer.Core.Domain.Users;
using TVProgViewer.Data.Extensions;

namespace TVProgViewer.Data.Migrations.Catalog
{
    [TvProgMigration("2019/11/19 01:09:03:8051845")]
    public class AddTierPriceUserRoleFK : AutoReversingMigration
    {
        #region Methods

        public override void Up()
        {
            this.AddForeignKey(nameof(TierPrice), 
                nameof(TierPrice.UserRoleId), 
                nameof(UserRole), 
                nameof(UserRole.Id), 
                Rule.Cascade);
        }
        
        #endregion
    }
}