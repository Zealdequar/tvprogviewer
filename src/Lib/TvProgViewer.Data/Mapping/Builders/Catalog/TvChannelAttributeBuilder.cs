﻿using FluentMigrator.Builders.Create.Table;
using TvProgViewer.Core.Domain.Catalog;

namespace TvProgViewer.Data.Mapping.Builders.Catalog
{
    /// <summary>
    /// Represents a tvChannel attribute entity builder
    /// </summary>
    public partial class TvChannelAttributeBuilder : TvProgEntityBuilder<TvChannelAttribute>
    {
        #region Methods

        /// <summary>
        /// Apply entity configuration
        /// </summary>
        /// <param name="table">Create table expression builder</param>
        public override void MapEntity(CreateTableExpressionBuilder table)
        {
            table.WithColumn(nameof(TvChannelAttribute.Name)).AsString(int.MaxValue).NotNullable();
        }

        #endregion
    }
}