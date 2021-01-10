using System.Data;
using FluentMigrator;
using TVProgViewer.Core.Domain.Catalog;
using TVProgViewer.Data.Extensions;
using TVProgViewer.Data.Mapping;

namespace TVProgViewer.Data.Migrations.Catalog
{
    [TvProgMigration("2019/11/19 12:46:00:2513442")]
    public class AddProductReviewReviewTypeMappingReviewTypeFK : AutoReversingMigration
    {
        #region Methods

        public override void Up()
        {
            this.AddForeignKey(TvProgMappingDefaults.ProductReviewReviewTypeTable, 
                nameof(ProductReviewReviewTypeMapping.ReviewTypeId), 
                nameof(ReviewType), 
                nameof(ReviewType.Id), 
                Rule.Cascade);
        }
        
        #endregion
    }
}
