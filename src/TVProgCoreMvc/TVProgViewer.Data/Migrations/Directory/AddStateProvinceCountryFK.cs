using System.Data;
using FluentMigrator;
using TVProgViewer.Core.Domain.Directory;
using TVProgViewer.Data.Extensions;

namespace TVProgViewer.Data.Migrations.Directory
{
    [TvProgMigration("2019/11/19 02:42:23:3797964")]
    public class AddStateProvinceCountryFK : AutoReversingMigration
    {
        #region Methods

        public override void Up()
        {
            this.AddForeignKey(nameof(StateProvince),
                nameof(StateProvince.CountryId),
                nameof(Country),
                nameof(Country.Id),
                Rule.Cascade);
        }

        #endregion
    }
}