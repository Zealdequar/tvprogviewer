using FluentMigrator;
using TVProgViewer.Core.Domain.Catalog;

namespace TVProgViewer.Data.Migrations.Indexes
{
    [TvProgMigration("2020/03/13 11:35:09:1647925")]
    public class AddProductTagNameIX : AutoReversingMigration
    {
        #region Methods

        public override void Up()
        {
            Create.Index("IX_ProductTag_Name").OnTable(nameof(ProductTag))
                .OnColumn(nameof(ProductTag.Name)).Ascending()
                .WithOptions().NonClustered();
        }

        #endregion
    }
}