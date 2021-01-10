using System.Data;
using FluentMigrator;
using TVProgViewer.Core.Domain.Vendors;
using TVProgViewer.Data.Extensions;

namespace TVProgViewer.Data.Migrations.Vendors
{
    [TvProgMigration("2019/11/19 05:48:19:1868645")]
    public class AddVendorNoteVendorFK : AutoReversingMigration
    {
        #region Methods

        public override void Up()
        {
            this.AddForeignKey(nameof(VendorNote),
                nameof(VendorNote.VendorId),
                nameof(Vendor),
                nameof(Vendor.Id),
                Rule.Cascade);
        }

        #endregion
    }
}