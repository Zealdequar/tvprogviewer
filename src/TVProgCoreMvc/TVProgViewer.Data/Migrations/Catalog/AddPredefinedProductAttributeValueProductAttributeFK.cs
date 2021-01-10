using System.Data;
using FluentMigrator;
using TVProgViewer.Core.Domain.Catalog;
using TVProgViewer.Data.Extensions;

namespace TVProgViewer.Data.Migrations.Catalog
{
    [TvProgMigration("2019/11/19 11:52:39:0754490")]
    public class AddPredefinedProductAttributeValueProductAttributeFK : AutoReversingMigration
    {
        #region Methods

        public override void Up()
        {
            this.AddForeignKey(nameof(PredefinedProductAttributeValue),
                nameof(PredefinedProductAttributeValue.ProductAttributeId),
                nameof(ProductAttribute),
                nameof(ProductAttribute.Id),
                Rule.Cascade);
        }

        #endregion
    }
}