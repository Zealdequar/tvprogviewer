using System.Data;
using FluentMigrator;
using TVProgViewer.Core.Domain.Users;
using TVProgViewer.Core.Domain.Orders;
using TVProgViewer.Data.Extensions;

namespace TVProgViewer.Data.Migrations.Orders
{
    [TvProgMigration("2019/11/19 05:27:09:1248082")]
    public class AddReturnRequestUserFK : AutoReversingMigration
    {
        #region Methods

        public override void Up()
        {
            
        }

        #endregion
    }
}