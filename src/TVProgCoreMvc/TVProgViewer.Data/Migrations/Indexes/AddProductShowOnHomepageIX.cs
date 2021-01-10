using FluentMigrator;
using TVProgViewer.Core.Domain.Catalog;
using TVProgViewer.Data.Extensions;

namespace TVProgViewer.Data.Migrations.Indexes
{
    [TvProgMigration("2019/12/19 09:36:08:9037704")]
    public class AddProductShowOnHomepageIX : AutoReversingMigration
    {
        #region Methods          

        public override void Up()
        {
            this.AddIndex("IX_Product_ShowOnHomepage", nameof(Product), i => i.Ascending(),
                nameof(Product.ShowOnHomepage));
        }

        #endregion
    }
}