using System.Data;
using FluentMigrator;
using TVProgViewer.Core.Domain.Users;
using TVProgViewer.Core.Domain.Polls;
using TVProgViewer.Data.Extensions;

namespace TVProgViewer.Data.Migrations.Polls
{
    [TvProgMigration("2019/11/19 05:35:03:6693384")]
    public class AddPollVotingRecordUserFK : AutoReversingMigration
    {
        #region Methods

        public override void Up()
        {

        }

        #endregion
    }
}