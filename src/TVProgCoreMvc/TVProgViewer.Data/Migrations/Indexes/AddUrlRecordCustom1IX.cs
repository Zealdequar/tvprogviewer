using FluentMigrator;
using TVProgViewer.Core.Domain.Seo;
using TVProgViewer.Data.Extensions;

namespace TVProgViewer.Data.Migrations.Indexes
{
    [TvProgMigration("2019/12/19 11:35:09:1647928")]
    public class AddUrlRecordCustom1IX : AutoReversingMigration
    {
        #region Methods

        public override void Up()
        {
            this.AddIndex("IX_UrlRecord_Custom_1", nameof(UrlRecord), i => i.Ascending(), nameof(UrlRecord.EntityId),
                nameof(UrlRecord.EntityName), nameof(UrlRecord.LanguageId), nameof(UrlRecord.IsActive));
        }

        #endregion
    }
}