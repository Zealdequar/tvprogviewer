using FluentMigrator;
using TVProgViewer.Core.Domain.Catalog;
using TVProgViewer.Data.Extensions;
using TVProgViewer.Data.Mapping;

namespace TVProgViewer.Data.Migrations.Indexes
{
    [TvProgMigration("2019/12/19 11:35:09:1647937")]
    public class AddProductCategoryMappingIsFeaturedProductIX : AutoReversingMigration
    {
        #region Methods

        public override void Up()
        {
            this.AddIndex("IX_Product_Category_Mapping_IsFeaturedProduct", TvProgMappingDefaults.ProductCategoryTable,
                i => i.Ascending(), nameof(ProductCategory.IsFeaturedProduct));
        }

        #endregion
    }
}