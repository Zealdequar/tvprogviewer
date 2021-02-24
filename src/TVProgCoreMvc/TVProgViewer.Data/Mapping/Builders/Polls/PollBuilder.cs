using FluentMigrator.Builders.Create.Table;
using TVProgViewer.Core.Domain.Localization;
using TVProgViewer.Core.Domain.Polls;
using TVProgViewer.Data.Extensions;

namespace TVProgViewer.Data.Mapping.Builders.Polls
{
    /// <summary>
    /// Represents a poll entity builder
    /// </summary>
    public partial class PollBuilder : TvProgEntityBuilder<Poll>
    {
        #region Methods

        /// <summary>
        /// Apply entity configuration
        /// </summary>
        /// <param name="table">Create table expression builder</param>
        public override void MapEntity(CreateTableExpressionBuilder table)
        {
            table
                .WithColumn(nameof(Poll.Name)).AsString(int.MaxValue).NotNullable()
                .WithColumn(nameof(Poll.LanguageId)).AsInt32().ForeignKey<Language>();
        }

        #endregion
    }
}