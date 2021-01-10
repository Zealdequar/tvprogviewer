using FluentMigrator;
using TVProgViewer.Core.Domain.Catalog;
using TVProgViewer.Data.Extensions;

namespace TVProgViewer.Data.Migrations.Indexes
{
    [TvProgMigration("2019/12/19 11:35:09:1647936")]
    public class AddProductSubjectToAclIX : AutoReversingMigration
    {
        #region Methods

        public override void Up()
        {
            this.AddIndex("IX_Product_SubjectToAcl", nameof(Product), i => i.Ascending(), nameof(Product.SubjectToAcl));
        }

        #endregion
    }
}