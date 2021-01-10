using FluentMigrator;
using TVProgViewer.Core.Domain.Catalog;
using TVProgViewer.Data.Extensions;

namespace TVProgViewer.Data.Migrations.Indexes
{
    [TvProgMigration("2019/12/19 09:36:08:9037697")]
    public class AddCategoryParentCategoryIdIX : AutoReversingMigration
    {
        #region Methods         

        public override void Up()
        {
            this.AddIndex("IX_Category_ParentCategoryId", nameof(Category), i => i.Ascending(),
                nameof(Category.ParentCategoryId));
        }

        #endregion
    }
}