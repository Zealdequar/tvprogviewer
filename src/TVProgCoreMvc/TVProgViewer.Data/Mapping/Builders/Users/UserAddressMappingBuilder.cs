using FluentMigrator.Builders.Create.Table;
using TVProgViewer.Core.Domain.Common;
using TVProgViewer.Core.Domain.Users;
using TVProgViewer.Data.Extensions;

namespace TVProgViewer.Data.Mapping.Builders.Users
{
    /// <summary>
    /// Represents a user address mapping entity builder
    /// </summary>
    public partial class UserAddressMappingBuilder : TvProgEntityBuilder<UserAddressMapping>
    {
        #region Methods

        /// <summary>
        /// Apply entity configuration
        /// </summary>
        /// <param name="table">Create table expression builder</param>
        public override void MapEntity(CreateTableExpressionBuilder table)
        {
            table
                .WithColumn(NameCompatibilityManager.GetColumnName(typeof(UserAddressMapping), nameof(UserAddressMapping.AddressId)))
                    .AsInt32().ForeignKey<Address>().PrimaryKey()
                .WithColumn(NameCompatibilityManager.GetColumnName(typeof(UserAddressMapping), nameof(UserAddressMapping.UserId)))
                    .AsInt32().ForeignKey<User>().PrimaryKey();
        }

        #endregion
    }
}