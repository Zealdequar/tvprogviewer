using FluentMigrator;
using TVProgViewer.Core.Domain.Localization;
using TVProgViewer.Data.Extensions;

namespace TVProgViewer.Data.Migrations.Indexes
{
    [TvProgMigration("2019/12/19 09:36:08:9037689")]
    public class AddLanguageDisplayOrderIX : AutoReversingMigration
    {
        #region Methods          

        public override void Up()
        {
            this.AddIndex("IX_Language_DisplayOrder", nameof(Language), i => i.Ascending(),
                nameof(Language.DisplayOrder));
        }

        #endregion
    }
}