using FluentMigrator;
using TVProgViewer.Core.Domain.Catalog;
using TVProgViewer.Data.Extensions;

namespace TVProgViewer.Data.Migrations.Indexes
{
    [TvProgMigration("2019/12/19 11:35:09:1647925")]
    public class AddProductTagNameIX : AutoReversingMigration
    {
        #region Methods

        public override void Up()
        {
            this.AddIndex("IX_ProductTag_Name", nameof(ProductTag), i => i.Ascending(), nameof(ProductTag.Name));
        }

        #endregion
    }
}