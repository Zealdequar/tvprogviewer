using FluentMigrator;
using TVProgViewer.Core.Domain.Common;
using TVProgViewer.Core.Domain.Directory;
using TVProgViewer.Data.Extensions;

namespace TVProgViewer.Data.Migrations.Common
{
    [TvProgMigration("2019/11/19 02:13:44:0659481")]
    public class AddAddressStateProvinceFK : AutoReversingMigration
    {
        #region Methods

        public override void Up()
        {
            this.AddForeignKey(nameof(Address),
                nameof(Address.StateProvinceId),
                nameof(StateProvince),
                nameof(StateProvince.Id));
        }

        #endregion
    }
}