﻿using FluentMigrator.Builders.Create.Table;
using TvProgViewer.Core.Domain.Localization;

namespace TvProgViewer.Data.Mapping.Builders.Localization
{
    /// <summary>
    /// Represents a language entity builder
    /// </summary>
    public partial class LanguageBuilder : TvProgEntityBuilder<Language>
    {
        #region Methods

        /// <summary>
        /// Apply entity configuration
        /// </summary>
        /// <param name="table">Create table expression builder</param>
        public override void MapEntity(CreateTableExpressionBuilder table)
        {
            table
                .WithColumn(nameof(Language.Name)).AsString(100).NotNullable()
                .WithColumn(nameof(Language.LanguageCulture)).AsString(20).NotNullable()
                .WithColumn(nameof(Language.UniqueSeoCode)).AsString(2).Nullable()
                .WithColumn(nameof(Language.FlagImageFileName)).AsString(50).Nullable();
        }

        #endregion
    }
}