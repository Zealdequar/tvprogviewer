using FluentMigrator;
using TVProgViewer.Core.Domain.Catalog;
using TVProgViewer.Data.Extensions;
using TVProgViewer.Data.Mapping;

namespace TVProgViewer.Data.Migrations.Indexes
{
    [TvProgMigration("2019/12/19 09:36:08:9037707")]
    public class AddPCMProductCategoryIX : AutoReversingMigration
    {
        #region Methods          

        public override void Up()
        {
            this.AddIndex("IX_PCM_Product_and_Category", TvProgMappingDefaults.ProductCategoryTable, i => i.Ascending(),
                nameof(ProductCategory.CategoryId), nameof(ProductCategory.ProductId));
        }

        #endregion
    }
}