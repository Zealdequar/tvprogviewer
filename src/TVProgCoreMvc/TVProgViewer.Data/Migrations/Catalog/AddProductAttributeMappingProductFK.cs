using System.Data;
using FluentMigrator;
using TVProgViewer.Core.Domain.Catalog;
using TVProgViewer.Data.Extensions;
using TVProgViewer.Data.Mapping;

namespace TVProgViewer.Data.Migrations.Catalog
{
    [TvProgMigration("2019/11/19 11:58:58:6806324")]
    public class AddProductAttributeMappingProductFK : AutoReversingMigration
    {
        #region Methods

        public override void Up()
        {
            this.AddForeignKey(TvProgMappingDefaults.ProductProductAttributeTable,
                nameof(ProductAttributeMapping.ProductId),
                nameof(Product),
                nameof(Product.Id),
                Rule.Cascade);
        }

        #endregion
    }
}