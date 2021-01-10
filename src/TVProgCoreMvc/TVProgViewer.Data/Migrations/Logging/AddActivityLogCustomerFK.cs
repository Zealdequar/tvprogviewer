using System.Data;
using FluentMigrator;
using TVProgViewer.Core.Domain.Users;
using TVProgViewer.Core.Domain.Logging;
using TVProgViewer.Data.Extensions;

namespace TVProgViewer.Data.Migrations.Logging
{
    [TvProgMigration("2019/11/19 04:57:30:8380330")]
    public class AddActivityLogUserFK : AutoReversingMigration
    {
        #region Methods

        public override void Up()
        {
          
        }

        #endregion
    }
}