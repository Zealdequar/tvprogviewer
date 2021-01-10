using FluentMigrator;
using TVProgViewer.Core.Domain.Catalog;
using TVProgViewer.Data.Extensions;

namespace TVProgViewer.Data.Migrations.Indexes
{
    [TvProgMigration("2019/12/19 09:36:08:9037692")]
    public class AddRelatedProductProductId1IX : AutoReversingMigration
    {
        #region Methods          

        public override void Up()
        {
            this.AddIndex("IX_RelatedProduct_ProductId1", nameof(RelatedProduct), i => i.Ascending(),
                nameof(RelatedProduct.ProductId1));
        }

        #endregion
    }
}