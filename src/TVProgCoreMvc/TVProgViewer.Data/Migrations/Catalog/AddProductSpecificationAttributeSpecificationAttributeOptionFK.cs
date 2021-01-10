using System.Data;
using FluentMigrator;
using TVProgViewer.Core.Domain.Catalog;
using TVProgViewer.Data.Extensions;
using TVProgViewer.Data.Mapping;

namespace TVProgViewer.Data.Migrations.Catalog
{
    [TvProgMigration("2019/11/19 12:49:06:2261985")]
    public class AddProductSpecificationAttributeSpecificationAttributeOptionFK : AutoReversingMigration
    {
        #region Methods

        public override void Up()
        {
            this.AddForeignKey(TvProgMappingDefaults.ProductSpecificationAttributeTable, 
                nameof(ProductSpecificationAttribute.SpecificationAttributeOptionId), 
                nameof(SpecificationAttributeOption), 
                nameof(SpecificationAttributeOption.Id), 
                Rule.Cascade);
        }

        #endregion
    }
}