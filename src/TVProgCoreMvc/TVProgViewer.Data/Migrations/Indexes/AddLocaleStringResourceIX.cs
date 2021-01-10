using FluentMigrator;
using TVProgViewer.Core.Domain.Localization;
using TVProgViewer.Data.Extensions;

namespace TVProgViewer.Data.Migrations.Indexes
{
    [TvProgMigration("2019/12/19 09:36:08:9037677")]
    public class AddLocaleStringResourceIX : AutoReversingMigration
    {
        #region Methods

        public override void Up()
        {
            this.AddIndex("IX_LocaleStringResource", nameof(LocaleStringResource), i => i.Ascending(),
                nameof(LocaleStringResource.ResourceName), nameof(LocaleStringResource.LanguageId));
        }

        #endregion
    }
}
