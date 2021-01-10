using System.Data;
using FluentMigrator;
using TVProgViewer.Core.Domain.Catalog;
using TVProgViewer.Data.Extensions;
using TVProgViewer.Data.Mapping;

namespace TVProgViewer.Data.Migrations.Catalog
{
    [TvProgMigration("2019/11/19 12:49:06:2261986")]
    public class AddProductSpecificationAttributeProductFK : AutoReversingMigration
    {
        #region Methods

        public override void Up()
        {
            this.AddForeignKey(TvProgMappingDefaults.ProductSpecificationAttributeTable, 
                nameof(ProductSpecificationAttribute.ProductId), 
                nameof(Product), 
                nameof(Product.Id), 
                Rule.Cascade);
        }

        #endregion
    }
}