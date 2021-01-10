using System.Data;
using FluentMigrator;
using TVProgViewer.Core.Domain.Catalog;
using TVProgViewer.Data.Extensions;
using TVProgViewer.Data.Mapping;

namespace TVProgViewer.Data.Migrations.Catalog
{
    [TvProgMigration("2019/11/19 12:19:26:2625750")]
    public class AddProductPictureProductProductFK : AutoReversingMigration
    {
        #region Methods

        public override void Up()
        {
            this.AddForeignKey(TvProgMappingDefaults.ProductPictureTable,
                nameof(ProductPicture.ProductId),
                nameof(Product),
                nameof(Product.Id),
                Rule.Cascade);
        }
        
        #endregion
    }
}