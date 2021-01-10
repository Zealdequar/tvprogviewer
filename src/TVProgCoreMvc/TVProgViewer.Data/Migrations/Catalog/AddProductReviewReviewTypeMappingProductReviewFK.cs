using System.Data;
using FluentMigrator;
using TVProgViewer.Core.Domain.Catalog;
using TVProgViewer.Data.Extensions;
using TVProgViewer.Data.Mapping;

namespace TVProgViewer.Data.Migrations.Catalog
{
    [TvProgMigration("2019/11/19 12:46:00:2513441")]
    public class AddProductReviewReviewTypeMappingProductReviewFK : AutoReversingMigration
    {
        #region Methods

        public override void Up()
        {
            this.AddForeignKey(TvProgMappingDefaults.ProductReviewReviewTypeTable, 
                nameof(ProductReviewReviewTypeMapping.ProductReviewId), 
                nameof(ProductReview), 
                nameof(ProductReview.Id), 
                Rule.Cascade);
        }

        #endregion
    }
}