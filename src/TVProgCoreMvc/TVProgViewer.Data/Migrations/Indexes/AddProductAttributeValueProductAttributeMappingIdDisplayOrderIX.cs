using FluentMigrator;
using TVProgViewer.Core.Domain.Catalog;
using TVProgViewer.Data.Extensions;

namespace TVProgViewer.Data.Migrations.Indexes
{
    [TvProgMigration("2019/12/19 09:36:08:9037693")]
    public class AddProductAttributeValueProductAttributeMappingIdDisplayOrderIX : AutoReversingMigration
    {
        #region Methods          

        public override void Up()
        {
            this.AddIndex("IX_ProductAttributeValue_ProductAttributeMappingId_DisplayOrder",
                nameof(ProductAttributeValue), i => i.Ascending(),
                nameof(ProductAttributeValue.ProductAttributeMappingId), nameof(ProductAttributeValue.DisplayOrder));
        }

        #endregion
    }
}