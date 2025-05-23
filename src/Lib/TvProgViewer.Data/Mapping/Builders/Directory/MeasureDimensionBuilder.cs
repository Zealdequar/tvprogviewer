﻿using FluentMigrator.Builders.Create.Table;
using TvProgViewer.Core.Domain.Directory;

namespace TvProgViewer.Data.Mapping.Builders.Directory
{
    /// <summary>
    /// Represents a measure dimension entity builder
    /// </summary>
    public partial class MeasureDimensionBuilder : TvProgEntityBuilder<MeasureDimension>
    {
        #region Methods

        /// <summary>
        /// Apply entity configuration
        /// </summary>
        /// <param name="table">Create table expression builder</param>
        public override void MapEntity(CreateTableExpressionBuilder table)
        {
            table
                .WithColumn(nameof(MeasureDimension.Name)).AsString(100).NotNullable()
                .WithColumn(nameof(MeasureDimension.SystemKeyword)).AsString(100).NotNullable()
                .WithColumn(nameof(MeasureDimension.Ratio)).AsDecimal(18, 8);
        }

        #endregion
    }
}