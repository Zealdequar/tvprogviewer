
using FluentMigrator;
using TVProgViewer.Core.Domain.TvProgMain;
using TVProgViewer.Data.Extensions;

namespace TVProgViewer.Data.Migrations.TvProgMain
{
    [TvProgMigration("2020/06/28 23:01:48:3129623")]
    public class AddUsersProgramsProgrammesFK: AutoReversingMigration
    {
        #region Методы

        public override void Up()
        {
            this.AddForeignKey(nameof(UsersPrograms),
                nameof(UsersPrograms.ProgrammesId),
                nameof(Programmes),
                nameof(Programmes.Id));
        }
        #endregion
    }
}
