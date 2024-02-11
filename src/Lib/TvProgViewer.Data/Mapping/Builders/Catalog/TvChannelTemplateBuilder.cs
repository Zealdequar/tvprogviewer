using FluentMigrator.Builders.Create.Table;
using TvProgViewer.Core.Domain.Catalog;

namespace TvProgViewer.Data.Mapping.Builders.Catalog
{
    /// <summary>
    /// Represents a tvchannel template entity builder
    /// </summary>
    public partial class TvChannelTemplateBuilder : TvProgEntityBuilder<TvChannelTemplate>
    {
        #region Methods

        /// <summary>
        /// Apply entity configuration
        /// </summary>
        /// <param name="table">Create table expression builder</param>
        public override void MapEntity(CreateTableExpressionBuilder table)
        {
            table
                .WithColumn(nameof(TvChannelTemplate.Name)).AsString(400).NotNullable()
                .WithColumn(nameof(TvChannelTemplate.ViewPath)).AsString(400).NotNullable();
        }

        #endregion
    }
}