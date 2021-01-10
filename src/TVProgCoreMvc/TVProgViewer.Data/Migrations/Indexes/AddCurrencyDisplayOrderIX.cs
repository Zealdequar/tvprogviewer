using FluentMigrator;
using TVProgViewer.Core.Domain.Directory;
using TVProgViewer.Data.Extensions;

namespace TVProgViewer.Data.Migrations.Indexes
{
    [TvProgMigration("2019/12/19 09:36:08:9037708")]
    public class AddCurrencyDisplayOrderIX : AutoReversingMigration
    {
        #region Methods          

        public override void Up()
        {
            this.AddIndex("IX_Currency_DisplayOrder", nameof(Currency), i => i.Ascending(), nameof(Currency.DisplayOrder));
        }

        #endregion
    }
}