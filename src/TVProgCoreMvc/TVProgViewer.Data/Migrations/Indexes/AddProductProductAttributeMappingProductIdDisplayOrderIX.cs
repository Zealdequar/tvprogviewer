using FluentMigrator;
using TVProgViewer.Core.Domain.Catalog;
using TVProgViewer.Data.Extensions;
using TVProgViewer.Data.Mapping;

namespace TVProgViewer.Data.Migrations.Indexes
{
    [TvProgMigration("2019/12/19 09:36:08:9037694")]
    public class AddProductProductAttributeMappingProductIdDisplayOrderIX : AutoReversingMigration
    {
        #region Methods          

        public override void Up()
        {
            this.AddIndex("IX_Product_ProductAttribute_Mapping_ProductId_DisplayOrder",
                TvProgMappingDefaults.ProductProductAttributeTable, i => i.Ascending(),
                nameof(ProductAttributeMapping.ProductId), nameof(ProductAttributeMapping.DisplayOrder));
        }

        #endregion
    }
}