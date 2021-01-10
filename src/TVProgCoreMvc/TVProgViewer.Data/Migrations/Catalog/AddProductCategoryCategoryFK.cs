using System.Data;
using FluentMigrator;
using TVProgViewer.Core.Domain.Catalog;
using TVProgViewer.Data.Extensions;
using TVProgViewer.Data.Mapping;

namespace TVProgViewer.Data.Migrations.Catalog
{
    [TvProgMigration("2019/11/19 12:04:22:5689396")]
    public class AddProductCategoryCategoryFK : AutoReversingMigration
    {
        #region Methods

        public override void Up()
        {
            this.AddForeignKey(TvProgMappingDefaults.ProductCategoryTable,
                nameof(ProductCategory.CategoryId),
                nameof(Category),
                nameof(Category.Id),
                Rule.Cascade);
        }
        
        #endregion
    }
}