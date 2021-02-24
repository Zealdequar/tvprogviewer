using System.Data;
using FluentMigrator.Builders.Create.Table;
using TVProgViewer.Core.Domain.Common;
using TVProgViewer.Core.Domain.Users;
using TVProgViewer.Data.Extensions;

namespace TVProgViewer.Data.Mapping.Builders.Users
{
    /// <summary>
    /// Represents a User entity builder
    /// </summary>
    public partial class UserBuilder : TvProgEntityBuilder<User>
    {
        #region Methods

        /// <summary>
        /// Apply entity configuration
        /// </summary>
        /// <param name="table">Create table expression builder</param>
        public override void MapEntity(CreateTableExpressionBuilder table)
        {
            table
                .WithColumn(nameof(User.Username)).AsString(1000).Nullable()
                .WithColumn(nameof(User.Email)).AsString(1000).Nullable()
                .WithColumn(nameof(User.EmailToRevalidate)).AsString(1000).Nullable()
                .WithColumn(nameof(User.SystemName)).AsString(400).Nullable()
                .WithColumn(NameCompatibilityManager.GetColumnName(typeof(User), nameof(User.BillingAddressId))).AsInt32().ForeignKey<Address>(onDelete: Rule.None).Nullable()
                .WithColumn(NameCompatibilityManager.GetColumnName(typeof(User), nameof(User.ShippingAddressId))).AsInt32().ForeignKey<Address>(onDelete: Rule.None).Nullable();
        }

        #endregion
    }
}