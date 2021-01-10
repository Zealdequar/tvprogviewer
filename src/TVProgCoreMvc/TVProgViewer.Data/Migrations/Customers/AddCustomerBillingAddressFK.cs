using FluentMigrator;
using TVProgViewer.Core.Domain.Common;
using TVProgViewer.Core.Domain.Users;
using TVProgViewer.Data.Extensions;

namespace TVProgViewer.Data.Migrations.Users
{
    [TvProgMigration("2019/11/19 02:29:25:1641381")]
    public class AddUserBillingAddressFK : AutoReversingMigration
    {
        #region Methods

        public override void Up()
        {
            this.AddForeignKey(nameof(User),
                "BillingAddress_Id",
                nameof(Address),
                nameof(Address.Id));
        }

        #endregion
    }
}