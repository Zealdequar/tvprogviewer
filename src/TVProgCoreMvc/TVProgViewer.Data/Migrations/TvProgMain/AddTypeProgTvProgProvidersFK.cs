
using FluentMigrator;
using TVProgViewer.Core.Domain.TvProgMain;
using TVProgViewer.Data.Extensions;

namespace TVProgViewer.Data.Migrations.TvProgMain
{
    [TvProgMigration("2020/06/28 22:39:19:4965735")]
    public class AddTypeProgTvProgProvidersFK: AutoReversingMigration
    {
        #region Методы

        public override void Up()
        {
            this.AddForeignKey(nameof(TypeProg),
                nameof(TypeProg.TvProgProviderId),
                nameof(TvProgProviders),
                nameof(TvProgProviders.Id));
        }
        #endregion
    }
}
