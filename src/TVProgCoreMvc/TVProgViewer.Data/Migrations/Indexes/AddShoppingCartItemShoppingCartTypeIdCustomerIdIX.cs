using FluentMigrator;
using TVProgViewer.Core.Domain.Orders;
using TVProgViewer.Data.Extensions;

namespace TVProgViewer.Data.Migrations.Indexes
{
    [TvProgMigration("2019/12/19 09:36:08:9037691")]
    public class AddShoppingCartItemShoppingCartTypeIdUserIdIX : AutoReversingMigration
    {
        #region Methods          

        public override void Up()
        {
            this.AddIndex("IX_ShoppingCartItem_ShoppingCartTypeId_UserId", nameof(ShoppingCartItem),
                i => i.Ascending(), nameof(ShoppingCartItem.ShoppingCartTypeId), nameof(ShoppingCartItem.UserId));
        }

        #endregion
    }
}