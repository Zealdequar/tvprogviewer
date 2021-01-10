using System.Data;
using FluentMigrator;
using TVProgViewer.Core.Domain.Discounts;
using TVProgViewer.Data.Extensions;
using TVProgViewer.Data.Mapping;

namespace TVProgViewer.Data.Migrations.Discounts
{
    [TvProgMigration("2019/11/19 04:31:35:1975256")]
    public class AddDiscountProductDiscountFK : AutoReversingMigration
    {
        #region Methods

        public override void Up()
        {
            this.AddForeignKey(TvProgMappingDefaults.DiscountAppliedToProductsTable,
                "Discount_Id",
                nameof(Discount),
                nameof(Discount.Id),
                Rule.Cascade);
        }

        #endregion
    }
}