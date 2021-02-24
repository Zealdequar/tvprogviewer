using FluentMigrator;
using TVProgViewer.Core.Domain.Orders;

namespace TVProgViewer.Data.Migrations.Indexes
{
    [TvProgMigration("2020/03/13 09:36:08:9037691")]
    public class AddShoppingCartItemShoppingCartTypeIdUserIdIX : AutoReversingMigration
    {
        #region Methods          

        public override void Up()
        {
            Create.Index("IX_ShoppingCartItem_ShoppingCartTypeId_UserId").OnTable(nameof(ShoppingCartItem))
                .OnColumn(nameof(ShoppingCartItem.ShoppingCartTypeId)).Ascending()
                .OnColumn(nameof(ShoppingCartItem.UserId)).Ascending()
                .WithOptions().NonClustered();
        }

        #endregion
    }
}