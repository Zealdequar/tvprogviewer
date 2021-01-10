using FluentMigrator;
using System.Runtime.CompilerServices;
using TVProgViewer.Core.Domain.TvProgMain;
using TVProgViewer.Data.Extensions;

namespace TVProgViewer.Data.Migrations.TvProgMain
{
    [TvProgMigration("2020/06/28 21:09:25:7450073")]
    public class AddExtUserSettingsTvProgProviderFK: AutoReversingMigration
    {
        #region Методы
        
        public override void Up()
        {
            this.AddForeignKey(nameof(ExtUserSettings),
                nameof(ExtUserSettings.TvProgProviderId),
                nameof(TvProgProviders),
                nameof(TvProgProviders.Id));
        }
        #endregion
    }
}
