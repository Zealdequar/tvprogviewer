using FluentMigrator.Builders.Create.Table;
using TvProgViewer.Core.Domain.Forums;
using TvProgViewer.Data.Extensions;

namespace TvProgViewer.Data.Mapping.Builders.Forums
{
    /// <summary>
    /// Represents a forum post vote entity builder
    /// </summary>
    public partial class ForumPostVoteBuilder : TvProgEntityBuilder<ForumPostVote>
    {
        #region Methods

        /// <summary>
        /// Apply entity configuration
        /// </summary>
        /// <param name="table">Create table expression builder</param>
        public override void MapEntity(CreateTableExpressionBuilder table)
        {
            table.WithColumn(nameof(ForumPostVote.ForumPostId)).AsInt32().ForeignKey<ForumPost>();
        }

        #endregion
    }
}