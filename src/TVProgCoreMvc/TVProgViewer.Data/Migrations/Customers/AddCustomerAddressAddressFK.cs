using System.Data;
using FluentMigrator;
using TVProgViewer.Core.Domain.Common;
using TVProgViewer.Data.Extensions;
using TVProgViewer.Data.Mapping;

namespace TVProgViewer.Data.Migrations.Users
{
    [TvProgMigration("2019/11/19 02:17:39:5245359")]
    public class AddUserAddressAddressFK : AutoReversingMigration
    {
        #region Methods

        public override void Up()
        {
            this.AddForeignKey(TvProgMappingDefaults.UserAddressesTable,
                "Address_Id",
                nameof(Address),
                nameof(Address.Id),
                Rule.Cascade);
        }

        #endregion
    }
}