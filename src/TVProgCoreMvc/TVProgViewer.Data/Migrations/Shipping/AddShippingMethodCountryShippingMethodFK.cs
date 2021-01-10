using System.Data;
using FluentMigrator;
using TVProgViewer.Core.Domain.Shipping;
using TVProgViewer.Data.Extensions;
using TVProgViewer.Data.Mapping;

namespace TVProgViewer.Data.Migrations.Shipping
{
    [TvProgMigration("2019/11/19 05:44:01:0528357")]
    public class AddShippingMethodCountryShippingMethodFK : AutoReversingMigration
    {
        #region Methods

        public override void Up()
        {
            this.AddForeignKey(TvProgMappingDefaults.ShippingMethodRestrictionsTable,
                "ShippingMethod_Id",
                nameof(ShippingMethod),
                nameof(ShippingMethod.Id),
                Rule.Cascade);
        }

        #endregion
    }
}