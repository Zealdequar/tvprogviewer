using FluentMigrator;
using TVProgViewer.Core.Domain.Catalog;
using TVProgViewer.Data.Extensions;

namespace TVProgViewer.Data.Migrations.Indexes
{
    [TvProgMigration("2019/12/19 11:35:09:1647934")]
    public class AddCategorSubjectToAclIX : AutoReversingMigration
    {
        #region Methods

        public override void Up()
        {
            this.AddIndex("IX_Category_SubjectToAcl", nameof(Category), i => i.Ascending(),
                nameof(Category.SubjectToAcl));
        }

        #endregion
    }
}