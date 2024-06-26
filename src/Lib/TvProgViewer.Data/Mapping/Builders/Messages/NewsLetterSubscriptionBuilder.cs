﻿using FluentMigrator.Builders.Create.Table;
using TvProgViewer.Core.Domain.Messages;

namespace TvProgViewer.Data.Mapping.Builders.Messages
{
    /// <summary>
    /// Represents a news letter subscription entity builder
    /// </summary>
    public partial class NewsLetterSubscriptionBuilder : TvProgEntityBuilder<NewsLetterSubscription>
    {
        #region Methods

        /// <summary>
        /// Apply entity configuration
        /// </summary>
        /// <param name="table">Create table expression builder</param>
        public override void MapEntity(CreateTableExpressionBuilder table)
        {
            table.WithColumn(nameof(NewsLetterSubscription.Email)).AsString(255).NotNullable();
        }

        #endregion
    }
}