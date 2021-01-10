using System.Data;
using FluentMigrator;
using TVProgViewer.Core.Domain.Catalog;
using TVProgViewer.Data.Extensions;
using TVProgViewer.Data.Mapping;

namespace TVProgViewer.Data.Migrations.Catalog
{
    [TvProgMigration("2019/11/19 12:26:28:0193451")]
    public class AddAddProductProductTagProductTagFK : AutoReversingMigration
    {
        #region Methods

        public override void Up()
        {
            this.AddForeignKey(TvProgMappingDefaults.ProductProductTagTable,
                "ProductTag_Id",
                nameof(ProductTag),
                nameof(ProductTag.Id),
                Rule.Cascade);
        }

        #endregion
    }
}