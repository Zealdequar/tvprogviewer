
using FluentMigrator;
using TVProgViewer.Core.Domain.TvProgMain;
using TVProgViewer.Data.Extensions;

namespace TVProgViewer.Data.Migrations.TvProgMain
{
    [TvProgMigration("2020/06/28 21:26:59:7701895")]
    public class AddProgrammesTypeProgFK: AutoReversingMigration
    {
        #region Методы

        public override void Up()
        {
            this.AddForeignKey(nameof(Programmes),
                nameof(Programmes.TypeProgId),
                nameof(TypeProg),
                nameof(TypeProg.Id));
        }
        #endregion
    }
}
