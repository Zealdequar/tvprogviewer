﻿using FluentMigrator.Builders.Create.Table;
using TvProgViewer.Core.Domain.Seo;

namespace TvProgViewer.Data.Mapping.Builders.Seo
{
    /// <summary>
    /// Represents a url record entity builder
    /// </summary>
    public partial class UrlRecordBuilder : TvProgEntityBuilder<UrlRecord>
    {
        #region Methods

        /// <summary>
        /// Apply entity configuration
        /// </summary>
        /// <param name="table">Create table expression builder</param>
        public override void MapEntity(CreateTableExpressionBuilder table)
        {
            table
                .WithColumn(nameof(UrlRecord.EntityName)).AsString(400).NotNullable()
                .WithColumn(nameof(UrlRecord.Slug)).AsString(400).NotNullable();
        }

        #endregion
    }
}