﻿using FluentMigrator.Builders.Create.Table;
using TvProgViewer.Core.Domain.Stores;
using TvProgViewer.Data.Extensions;

namespace TvProgViewer.Data.Mapping.Builders.Stores
{
    /// <summary>
    /// Represents a store mapping entity builder
    /// </summary>
    public partial class StoreMappingBuilder : TvProgEntityBuilder<StoreMapping>
    {
        #region Methods

        /// <summary>
        /// Apply entity configuration
        /// </summary>
        /// <param name="table">Create table expression builder</param>
        public override void MapEntity(CreateTableExpressionBuilder table)
        {
            table
                .WithColumn(nameof(StoreMapping.EntityName)).AsString(400).NotNullable()
                .WithColumn(nameof(StoreMapping.StoreId)).AsInt32().ForeignKey<Store>();
        }

        #endregion
    }
}