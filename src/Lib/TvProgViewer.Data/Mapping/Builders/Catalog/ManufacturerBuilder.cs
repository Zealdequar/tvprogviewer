﻿using FluentMigrator.Builders.Create.Table;
using TvProgViewer.Core.Domain.Catalog;

namespace TvProgViewer.Data.Mapping.Builders.Catalog
{
    /// <summary>
    /// Represents a manufacturer entity builder
    /// </summary>
    public partial class ManufacturerBuilder : TvProgEntityBuilder<Manufacturer>
    {
        #region Methods

        /// <summary>
        /// Apply entity configuration
        /// </summary>
        /// <param name="table">Create table expression builder</param>
        public override void MapEntity(CreateTableExpressionBuilder table)
        {
            table
                .WithColumn(nameof(Manufacturer.Name)).AsString(400).NotNullable()
                .WithColumn(nameof(Manufacturer.MetaKeywords)).AsString(400).Nullable()
                .WithColumn(nameof(Manufacturer.MetaTitle)).AsString(400).Nullable()
                .WithColumn(nameof(Manufacturer.PageSizeOptions)).AsString(200).Nullable();
        }

        #endregion
    }
}