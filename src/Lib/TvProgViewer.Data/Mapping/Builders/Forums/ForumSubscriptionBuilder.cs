using System.Data;
using FluentMigrator.Builders.Create.Table;
using TvProgViewer.Core.Domain.Users;
using TvProgViewer.Core.Domain.Forums;
using TvProgViewer.Data.Extensions;

namespace TvProgViewer.Data.Mapping.Builders.Forums
{
    /// <summary>
    /// Represents a forum subscription entity builder
    /// </summary>
    public partial class ForumSubscriptionBuilder : TvProgEntityBuilder<ForumSubscription>
    {
        #region Methods

        /// <summary>
        /// Apply entity configuration
        /// </summary>
        /// <param name="table">Create table expression builder</param>
        public override void MapEntity(CreateTableExpressionBuilder table)
        {
            table.WithColumn(nameof(ForumSubscription.UserId)).AsInt32().ForeignKey<User>(onDelete: Rule.None);
        }

        #endregion
    }
}