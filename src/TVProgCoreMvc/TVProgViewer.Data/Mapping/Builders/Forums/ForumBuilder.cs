using FluentMigrator.Builders.Create.Table;
using TVProgViewer.Core.Domain.Forums;
using TVProgViewer.Data.Extensions;

namespace TVProgViewer.Data.Mapping.Builders.Forums
{
    /// <summary>
    /// Represents a forum buil entity builder
    /// </summary>
    public partial class ForumBuilder : TvProgEntityBuilder<Forum>
    {
        #region Methods

        /// <summary>
        /// Apply entity configuration
        /// </summary>
        /// <param name="table">Create table expression builder</param>
        public override void MapEntity(CreateTableExpressionBuilder table)
        {
            table
                .WithColumn(nameof(Forum.Name)).AsString(200).NotNullable()
                .WithColumn(nameof(Forum.ForumGroupId)).AsInt32().ForeignKey<ForumGroup>();
        }

        #endregion
    }
}