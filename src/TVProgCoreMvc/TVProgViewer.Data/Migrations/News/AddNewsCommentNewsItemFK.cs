using System.Data;
using FluentMigrator;
using TVProgViewer.Core.Domain.News;
using TVProgViewer.Data.Extensions;
using TVProgViewer.Data.Mapping;

namespace TVProgViewer.Data.Migrations.News
{
    [TvProgMigration("2019/11/19 05:03:56:2530772")]
    public class AddNewsCommentNewsItemFK : AutoReversingMigration
    {
        #region Methods

        public override void Up()
        {
            this.AddForeignKey(nameof(NewsComment),
                nameof(NewsComment.NewsItemId),
                TvProgMappingDefaults.NewsItemTable,
                nameof(NewsItem.Id),
                Rule.Cascade);
        }

        #endregion
    }
}