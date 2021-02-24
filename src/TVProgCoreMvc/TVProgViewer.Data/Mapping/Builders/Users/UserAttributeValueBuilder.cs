using FluentMigrator.Builders.Create.Table;
using TVProgViewer.Core.Domain.Users;
using TVProgViewer.Data.Extensions;

namespace TVProgViewer.Data.Mapping.Builders.Users
{
    /// <summary>
    /// Represents a user attribute value entity builder
    /// </summary>
    public partial class UserAttributeValueBuilder : TvProgEntityBuilder<UserAttributeValue>
    {
        #region Methods

        /// <summary>
        /// Apply entity configuration
        /// </summary>
        /// <param name="table">Create table expression builder</param>
        public override void MapEntity(CreateTableExpressionBuilder table)
        {
            table
                .WithColumn(nameof(UserAttributeValue.Name)).AsString(400).NotNullable()
                .WithColumn(nameof(UserAttributeValue.UserAttributeId)).AsInt32().ForeignKey<UserAttribute>();
        }

        #endregion
    }
}