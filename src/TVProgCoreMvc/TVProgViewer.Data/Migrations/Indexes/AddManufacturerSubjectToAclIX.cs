using FluentMigrator;
using TVProgViewer.Core.Domain.Catalog;
using TVProgViewer.Data.Extensions;

namespace TVProgViewer.Data.Migrations.Indexes
{
    [TvProgMigration("2019/12/19 11:35:09:1647935")]
    public class AddManufacturerSubjectToAclIX : AutoReversingMigration
    {
        #region Methods

        public override void Up()
        {
            this.AddIndex("IX_Manufacturer_SubjectToAcl", nameof(Manufacturer), i => i.Ascending(),
                nameof(Manufacturer.SubjectToAcl));
        }

        #endregion
    }
}