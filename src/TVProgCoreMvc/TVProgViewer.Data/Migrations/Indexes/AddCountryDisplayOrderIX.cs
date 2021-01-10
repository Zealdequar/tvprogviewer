using FluentMigrator;
using TVProgViewer.Core.Domain.Directory;
using TVProgViewer.Data.Extensions;

namespace TVProgViewer.Data.Migrations.Indexes
{
    [TvProgMigration("2019/12/19 09:36:08:9037679")]
    public class AddCountryDisplayOrderIX : AutoReversingMigration
    {
        #region Methods

        public override void Up()
        {
            this.AddIndex("IX_Country_DisplayOrder", nameof(Country), i => i.Ascending(), nameof(Country.DisplayOrder));
        }

        #endregion
    }
}