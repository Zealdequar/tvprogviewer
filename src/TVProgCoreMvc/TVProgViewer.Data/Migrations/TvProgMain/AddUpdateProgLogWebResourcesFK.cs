
using FluentMigrator;
using TVProgViewer.Core.Domain.TvProgMain;
using TVProgViewer.Data.Extensions;

namespace TVProgViewer.Data.Migrations.TvProgMain
{
    [TvProgMigration("2020/06/28 22:42:16:5367010")]
    public class AddUpdateProgLogWebResourcesFK: AutoReversingMigration
    {
        #region Методы

        public override void Up()
        {
            this.AddForeignKey(nameof(UpdateProgLog),
                nameof(UpdateProgLog.WebResourceId),
                nameof(WebResources),
                nameof(WebResources.Id));
        }
        #endregion
    }
}
