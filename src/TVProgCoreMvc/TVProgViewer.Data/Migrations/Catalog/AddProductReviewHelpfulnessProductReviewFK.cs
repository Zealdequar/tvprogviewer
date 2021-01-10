using System.Data;
using FluentMigrator;
using TVProgViewer.Core.Domain.Catalog;
using TVProgViewer.Data.Extensions;

namespace TVProgViewer.Data.Migrations.Catalog
{
    [TvProgMigration("2019/11/19 12:39:15:8603530")]
    public class AddProductReviewHelpfulnessProductReviewFK : AutoReversingMigration
    {
        #region Methods

        public override void Up()
        {
            this.AddForeignKey(nameof(ProductReviewHelpfulness),
                nameof(ProductReviewHelpfulness.ProductReviewId),
                nameof(ProductReview),
                nameof(ProductReview.Id),
                Rule.Cascade);
        }

        #endregion
    }
}