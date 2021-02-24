using FluentMigrator.Builders.Create.Table;
using TVProgViewer.Core.Domain.Users;
using TVProgViewer.Data.Extensions;

namespace TVProgViewer.Data.Mapping.Builders.Users
{
    /// <summary>
    /// Represents a external authentication record entity builder
    /// </summary>
    public partial class ExternalAuthenticationRecordBuilder : TvProgEntityBuilder<ExternalAuthenticationRecord>
    {
        #region Methods

        /// <summary>
        /// Apply entity configuration
        /// </summary>
        /// <param name="table">Create table expression builder</param>
        public override void MapEntity(CreateTableExpressionBuilder table)
        {
            table.WithColumn(nameof(ExternalAuthenticationRecord.UserId)).AsInt32().ForeignKey<User>();
        }

        #endregion
    }
}