using FluentMigrator;
using FluentMigrator.SqlServer;
using TVProgViewer.Core.Domain.Catalog;
using TVProgViewer.Data.Extensions;
using TVProgViewer.Data.Mapping;

namespace TVProgViewer.Data.Migrations.Indexes
{
    [TvProgMigration("2019/12/19 12:02:35:9280389")]
    public class AddPCMProductIdExtendedIX : AutoReversingMigration
    {
        #region Methods          

        public override void Up()
        {
            this.AddIndex("IX_PCM_ProductId_Extended", TvProgMappingDefaults.ProductCategoryTable, i => i.Ascending(),
                    nameof(ProductCategory.ProductId), nameof(ProductCategory.IsFeaturedProduct))
                .Include(nameof(ProductCategory.CategoryId));
        }

        #endregion
    }
}