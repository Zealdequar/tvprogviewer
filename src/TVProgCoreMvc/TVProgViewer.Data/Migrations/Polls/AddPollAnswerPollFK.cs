using System.Data;
using FluentMigrator;
using TVProgViewer.Core.Domain.Polls;
using TVProgViewer.Data.Extensions;

namespace TVProgViewer.Data.Migrations.Polls
{
    [TvProgMigration("2019/11/19 05:32:28:7520229")]
    public class AddPollAnswerPollFK : AutoReversingMigration
    {
        #region Methods

        public override void Up()
        {
            this.AddForeignKey(nameof(PollAnswer),
                nameof(PollAnswer.PollId),
                nameof(Poll),
                nameof(Poll.Id),
                Rule.Cascade);
        }

        #endregion
    }
}