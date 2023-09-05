using FluentMigrator.Builders.Create.Table;
using TvProgViewer.Core.Domain.Users;
using TvProgViewer.Core.Domain.Security;
using TvProgViewer.Data.Extensions;

namespace TvProgViewer.Data.Mapping.Builders.Security
{
    /// <summary>
    /// Represents a ACL record entity builder
    /// </summary>
    public partial class AclRecordBuilder : TvProgEntityBuilder<AclRecord>
    {
        #region Methods

        /// <summary>
        /// Apply entity configuration
        /// </summary>
        /// <param name="table">Create table expression builder</param>
        public override void MapEntity(CreateTableExpressionBuilder table)
        {
            table
                .WithColumn(nameof(AclRecord.EntityName)).AsString(400).NotNullable()
                .WithColumn(nameof(AclRecord.UserRoleId)).AsInt32().ForeignKey<UserRole>();
        }

        #endregion
    }
}