using System.Data;
using FluentMigrator;
using TVProgViewer.Core.Domain.Directory;
using TVProgViewer.Data.Extensions;
using TVProgViewer.Data.Mapping;

namespace TVProgViewer.Data.Migrations.Shipping
{
    [TvProgMigration("2019/11/19 05:44:01:0528356")]
    public class AddShippingMethodCountryCountryFK : AutoReversingMigration
    {
        #region Methods

        public override void Up()
        {
            this.AddForeignKey(TvProgMappingDefaults.ShippingMethodRestrictionsTable,
                "Country_Id",
                nameof(Country),
                nameof(Country.Id),
                Rule.Cascade);
        }

        #endregion
    }
}