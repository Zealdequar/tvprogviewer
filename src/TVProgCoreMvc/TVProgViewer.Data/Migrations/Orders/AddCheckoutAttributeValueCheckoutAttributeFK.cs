using System.Data;
using FluentMigrator;
using TVProgViewer.Core.Domain.Orders;
using TVProgViewer.Data.Extensions;

namespace TVProgViewer.Data.Migrations.Orders
{
    [TvProgMigration("2019/11/19 05:08:27:8553212")]
    public class AddCheckoutAttributeValueCheckoutAttributeFK : AutoReversingMigration
    {
        #region Methods

        public override void Up()
        {
            this.AddForeignKey(nameof(CheckoutAttributeValue),
                nameof(CheckoutAttributeValue.CheckoutAttributeId),
                nameof(CheckoutAttribute),
                nameof(CheckoutAttribute.Id),
                Rule.Cascade);
        }

        #endregion
    }
}