using System.Data;
using FluentMigrator.Builders.Create.Table;
using TVProgViewer.Core.Domain.Users;
using TVProgViewer.Core.Domain.Forums;
using TVProgViewer.Data.Extensions;

namespace TVProgViewer.Data.Mapping.Builders.Forums
{
    /// <summary>
    /// Represents a forum topic entity builder
    /// </summary>
    public partial class ForumTopicBuilder : TvProgEntityBuilder<ForumTopic>
    {
        #region Methods

        /// <summary>
        /// Apply entity configuration
        /// </summary>
        /// <param name="table">Create table expression builder</param>
        public override void MapEntity(CreateTableExpressionBuilder table)
        {
            table
                .WithColumn(nameof(ForumTopic.Subject)).AsString(450).NotNullable()
                .WithColumn(nameof(ForumTopic.UserId)).AsInt32().ForeignKey<User>(onDelete: Rule.None)
                .WithColumn(nameof(ForumTopic.ForumId)).AsInt32().ForeignKey<Forum>();
        }

        #endregion
    }
}