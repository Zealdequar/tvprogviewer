using System.Data;
using FluentMigrator;
using TVProgViewer.Core.Domain.Catalog;
using TVProgViewer.Data.Extensions;
using TVProgViewer.Data.Mapping;

namespace TVProgViewer.Data.Migrations.Catalog
{
    [TvProgMigration("2019/11/19 12:26:28:0193450")]
    public class AddAddProductProductTagProductFK : AutoReversingMigration
    {
        #region Methods

        public override void Up()
        {
            this.AddForeignKey(TvProgMappingDefaults.ProductProductTagTable,
                "Product_Id",
                nameof(Product),
                nameof(Product.Id),
                Rule.Cascade);
        }

        #endregion
    }
}