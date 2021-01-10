using FluentMigrator;
using TVProgViewer.Core.Domain.Catalog;
using TVProgViewer.Data.Extensions;

namespace TVProgViewer.Data.Migrations.Indexes
{
    [TvProgMigration("2019/12/19 09:36:08:9037678")]
    public class AddProductPriceDatesEtcIX : AutoReversingMigration
    {
        #region Methods

        public override void Up()
        {
            this.AddIndex("IX_Product_PriceDatesEtc", nameof(Product), i => i.Ascending(),
                nameof(Product.Price), nameof(Product.AvailableStartDateTimeUtc),
                nameof(Product.AvailableEndDateTimeUtc), nameof(Product.Published),
                nameof(Product.Deleted));
        }

        #endregion
    }
}