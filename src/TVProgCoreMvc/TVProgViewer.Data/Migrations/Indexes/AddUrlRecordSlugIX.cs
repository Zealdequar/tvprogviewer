using FluentMigrator;
using TVProgViewer.Core.Domain.Seo;
using TVProgViewer.Data.Extensions;

namespace TVProgViewer.Data.Migrations.Indexes
{
    [TvProgMigration("2019/12/19 11:35:09:1647927")]
    public class AddUrlRecordSlugIX : AutoReversingMigration
    {
        #region Methods

        public override void Up()
        {
            this.AddIndex("IX_UrlRecord_Slug", nameof(UrlRecord), i => i.Ascending(), nameof(UrlRecord.Slug));
        }

        #endregion
    }
}