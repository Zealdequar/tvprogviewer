using System.Data;
using FluentMigrator;
using TVProgViewer.Core.Domain.Vendors;
using TVProgViewer.Data.Extensions;

namespace TVProgViewer.Data.Migrations.Vendors
{
    [TvProgMigration("2019/11/19 05:47:14:6411077")]
    public class AddVendorAttributeValueVendorAttributeFK : AutoReversingMigration
    {
        #region Methods

        public override void Up()
        {
            this.AddForeignKey(nameof(VendorAttributeValue),
                nameof(VendorAttributeValue.VendorAttributeId),
                nameof(VendorAttribute),
                nameof(VendorAttribute.Id),
                Rule.Cascade);
        }

        #endregion
    }
}