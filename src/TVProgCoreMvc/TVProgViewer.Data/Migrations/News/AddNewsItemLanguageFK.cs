using System.Data;
using FluentMigrator;
using TVProgViewer.Core.Domain.Localization;
using TVProgViewer.Core.Domain.News;
using TVProgViewer.Data.Extensions;
using TVProgViewer.Data.Mapping;

namespace TVProgViewer.Data.Migrations.News
{
    [TvProgMigration("2019/11/19 05:06:49:4361423")]
    public class AddNewsItemLanguageFK : AutoReversingMigration
    {
        #region Methods

        public override void Up()
        {
            this.AddForeignKey(TvProgMappingDefaults.NewsItemTable,
                nameof(NewsItem.LanguageId),
                nameof(Language),
                nameof(Language.Id),
                Rule.Cascade);
        }

        #endregion
    }
}