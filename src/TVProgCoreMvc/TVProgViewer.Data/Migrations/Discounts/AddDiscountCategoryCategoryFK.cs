using System.Data;
using FluentMigrator;
using TVProgViewer.Core.Domain.Catalog;
using TVProgViewer.Data.Extensions;
using TVProgViewer.Data.Mapping;

namespace TVProgViewer.Data.Migrations.Discounts
{
    [TvProgMigration("2019/11/19 04:19:29:5936888")]
    public class AddDiscountCategoryCategoryFK : AutoReversingMigration
    {
        #region Methods

        public override void Up()
        {
            this.AddForeignKey(TvProgMappingDefaults.DiscountAppliedToCategoriesTable,
                "Category_Id",
                nameof(Category),
                nameof(Category.Id),
                Rule.Cascade);
        }

        #endregion
    }
}