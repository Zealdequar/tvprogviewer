using System.Data;
using FluentMigrator;
using TVProgViewer.Core.Domain.Users;
using TVProgViewer.Core.Domain.Orders;
using TVProgViewer.Data.Extensions;

namespace TVProgViewer.Data.Migrations.Orders
{
    [TvProgMigration("2019/11/19 05:16:29:6028942")]
    public class AddOrderUserFK : AutoReversingMigration
    {
        #region Methods

        public override void Up()
        {
            
        }

        #endregion
    }
}