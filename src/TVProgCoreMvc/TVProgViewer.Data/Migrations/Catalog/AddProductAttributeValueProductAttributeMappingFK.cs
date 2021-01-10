using System.Data;
using FluentMigrator;
using TVProgViewer.Core.Domain.Catalog;
using TVProgViewer.Data.Extensions;
using TVProgViewer.Data.Mapping;

namespace TVProgViewer.Data.Migrations.Catalog
{
    [TvProgMigration("2019/11/19 12:00:50:7544540")]
    public class AddProductAttributeValueProductAttributeMappingFK : AutoReversingMigration
    {
        #region Methods

        public override void Up()
        {
            this.AddForeignKey(nameof(ProductAttributeValue),
                nameof(ProductAttributeValue.ProductAttributeMappingId),
                TvProgMappingDefaults.ProductProductAttributeTable,
                nameof(ProductAttributeMapping.Id),
                Rule.Cascade);
        }

        #endregion
    }
}