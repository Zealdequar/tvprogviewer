using System.Data;
using FluentMigrator;
using TVProgViewer.Core.Domain.Localization;
using TVProgViewer.Core.Domain.Polls;
using TVProgViewer.Data.Extensions;

namespace TVProgViewer.Data.Migrations.Polls
{
    /// <summary>
    /// Represents a poll mapping configuration
    /// </summary>
    [TvProgMigration("2019/11/19 05:33:22:5962851")]
    public class AddPollLanguageFK : AutoReversingMigration
    {
        #region Methods

        public override void Up()
        {
            this.AddForeignKey(nameof(Poll),
                nameof(Poll.LanguageId),
                nameof(Language),
                nameof(Language.Id),
                Rule.Cascade);
        }

        #endregion
    }
}