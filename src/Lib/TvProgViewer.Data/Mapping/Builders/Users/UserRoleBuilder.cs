﻿using FluentMigrator.Builders.Create.Table;
using TvProgViewer.Core.Domain.Users;

namespace TvProgViewer.Data.Mapping.Builders.Users
{
    /// <summary>
    /// Represents a user role entity builder
    /// </summary>
    public partial class UserRoleBuilder : TvProgEntityBuilder<UserRole>
    {
        #region Methods

        /// <summary>
        /// Apply entity configuration
        /// </summary>
        /// <param name="table">Create table expression builder</param>
        public override void MapEntity(CreateTableExpressionBuilder table)
        {
            table
                .WithColumn(nameof(UserRole.Name)).AsString(255).NotNullable()
                .WithColumn(nameof(UserRole.SystemName)).AsString(255).Nullable();
        }

        #endregion
    }
}