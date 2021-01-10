using FluentMigrator;
using FluentMigrator.SqlServer;
using TVProgViewer.Core.Domain.Catalog;
using TVProgViewer.Data.Extensions;
using TVProgViewer.Data.Mapping;

namespace TVProgViewer.Data.Migrations.Indexes
{
    [TvProgMigration("2019/12/19 12:02:35:9280392")]
    public class AddPSAMSpecificationAttributeOptionIdAllowFilteringIX : AutoReversingMigration
    {
        #region Methods          

        public override void Up()
        {
            this.AddIndex("IX_PSAM_SpecificationAttributeOptionId_AllowFiltering",
                    TvProgMappingDefaults.ProductSpecificationAttributeTable, i => i.Ascending(),
                    nameof(ProductSpecificationAttribute.SpecificationAttributeOptionId),
                    nameof(ProductSpecificationAttribute.AllowFiltering))
                .Include(nameof(ProductSpecificationAttribute.ProductId));
        }

        #endregion
    }
}