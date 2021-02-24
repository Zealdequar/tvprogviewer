using FluentMigrator.Builders.Create.Table;
using TVProgViewer.Core.Domain.Users;

namespace TVProgViewer.Data.Mapping.Builders.Users
{
    /// <summary>
    /// Represents a user attribute entity builder
    /// </summary>
    public partial class UserAttributeBuilder : TvProgEntityBuilder<UserAttribute>
    {
        #region Methods

        /// <summary>
        /// Apply entity configuration
        /// </summary>
        /// <param name="table">Create table expression builder</param>
        public override void MapEntity(CreateTableExpressionBuilder table)
        {
            table.WithColumn(nameof(UserAttribute.Name)).AsString(400).NotNullable();
        }

        #endregion
    }
}