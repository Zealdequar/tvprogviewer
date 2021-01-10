
using FluentMigrator;
using TVProgViewer.Core.Domain.TvProgMain;
using TVProgViewer.Data.Extensions;

namespace TVProgViewer.Data.Migrations.TvProgMain
{
    [TvProgMigration("2020/06/28 23:10:37:2638325")]
    public class AddWebResourcesTypeProgFK: AutoReversingMigration
    {
        #region Методы

        public override void Up()
        {
            this.AddForeignKey(nameof(WebResources),
                nameof(WebResources.TypeProgId),
                nameof(TypeProg),
                nameof(TypeProg.Id));
        }
        #endregion
    }
}
