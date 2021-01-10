using System.Data;
using FluentMigrator;
using TVProgViewer.Core.Domain.News;
using TVProgViewer.Core.Domain.Stores;
using TVProgViewer.Data.Extensions;

namespace TVProgViewer.Data.Migrations.News
{
    [TvProgMigration("2019/11/19 05:03:56:2530774")]
    public class AddNewsCommentStoreFK : AutoReversingMigration
    {
        #region Methods

        public override void Up()
        {
            this.AddForeignKey(nameof(NewsComment),
                nameof(NewsComment.StoreId),
                nameof(Store),
                nameof(Store.Id),
                Rule.Cascade);
        }

        #endregion
    }
}