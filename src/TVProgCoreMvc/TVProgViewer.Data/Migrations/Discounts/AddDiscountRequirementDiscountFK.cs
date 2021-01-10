using System.Data;
using FluentMigrator;
using TVProgViewer.Core.Domain.Discounts;
using TVProgViewer.Data.Extensions;

namespace TVProgViewer.Data.Migrations.Discounts
{
    [TvProgMigration("2019/12/13 01:04:12:0043560")]
    public class AddDiscountRequirementDiscountFK : AutoReversingMigration
    {
        #region Methods

        public override void Up()
        {
            this.AddForeignKey(nameof(DiscountRequirement),
                nameof(DiscountRequirement.DiscountId),
                nameof(Discount),
                nameof(Discount.Id),
                Rule.Cascade);
        }

        #endregion
    }
}
