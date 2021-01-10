using System.Data;
using FluentMigrator;
using TVProgViewer.Core.Domain.Localization;
using TVProgViewer.Data.Extensions;

namespace TVProgViewer.Data.Migrations.Localization
{
    [TvProgMigration("2019/11/19 04:54:55:1964555")]
    public class AddLocaleStringResourceLanguageFK : AutoReversingMigration
    {
        #region Methods

        public override void Up()
        {
            this.AddForeignKey(nameof(LocaleStringResource),
                nameof(LocaleStringResource.LanguageId),
                nameof(Language),
                nameof(Language.Id),
                Rule.Cascade);
        }

        #endregion
    }
}