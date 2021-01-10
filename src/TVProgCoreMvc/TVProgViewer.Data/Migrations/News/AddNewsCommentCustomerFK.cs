using System.Data;
using FluentMigrator;
using TVProgViewer.Core.Domain.Users;
using TVProgViewer.Core.Domain.News;
using TVProgViewer.Data.Extensions;

namespace TVProgViewer.Data.Migrations.News
{
    [TvProgMigration("2019/11/19 05:03:56:2530773")]
    public class AddNewsCommentUserFK : AutoReversingMigration
    {
        #region Methods

        public override void Up()
        {
            
        }

        #endregion
    }
}