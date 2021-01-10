using FluentMigrator;
using TVProgViewer.Core.Domain.Common;
using TVProgViewer.Core.Domain.Orders;
using TVProgViewer.Data.Extensions;

namespace TVProgViewer.Data.Migrations.Orders
{
    [TvProgMigration("2019/11/19 05:16:29:6028944")]
    public class AddOrderShippingAddressFK : AutoReversingMigration
    {
        #region Methods

        public override void Up()
        {
            this.AddForeignKey(nameof(Order),
                nameof(Order.ShippingAddressId),
                nameof(Address),
                nameof(Address.Id));
        }

        #endregion
    }
}