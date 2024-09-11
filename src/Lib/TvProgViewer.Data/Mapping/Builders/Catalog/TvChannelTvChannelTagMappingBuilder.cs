using FluentMigrator.Builders.Create.Table;
using TvProgViewer.Core.Domain.Catalog;
using TvProgViewer.Data.Extensions;

namespace TvProgViewer.Data.Mapping.Builders.Catalog
{
    /// <summary>
    /// Represents a tvChannel tvChannel tag mapping entity builder
    /// </summary>
    public partial class TvChannelTvChannelTagMappingBuilder : TvProgEntityBuilder<TvChannelTvChannelTagMapping>
    {
        #region Methods

        /// <summary>
        /// Apply entity configuration
        /// </summary>
        /// <param name="table">Create table expression builder</param>
        public override void MapEntity(CreateTableExpressionBuilder table)
        {
            table
                .WithColumn(NameCompatibilityManager.GetColumnName(typeof(TvChannelTvChannelTagMapping), nameof(TvChannelTvChannelTagMapping.TvChannelId)))
                    .AsInt32().PrimaryKey().ForeignKey<TvChannel>()
                .WithColumn(NameCompatibilityManager.GetColumnName(typeof(TvChannelTvChannelTagMapping), nameof(TvChannelTvChannelTagMapping.TvChannelTagId)))
                    .AsInt32().PrimaryKey().ForeignKey<TvChannelTag>();
        }

        #endregion
    }
}