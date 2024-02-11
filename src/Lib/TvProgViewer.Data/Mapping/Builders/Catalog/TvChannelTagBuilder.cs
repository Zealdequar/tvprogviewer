using FluentMigrator.Builders.Create.Table;
using TvProgViewer.Core.Domain.Catalog;

namespace TvProgViewer.Data.Mapping.Builders.Catalog
{
    /// <summary>
    /// Represents a tvchannel tag entity builder
    /// </summary>
    public partial class TvChannelTagBuilder : TvProgEntityBuilder<TvChannelTag>
    {
        #region Methods

        /// <summary>
        /// Apply entity configuration
        /// </summary>
        /// <param name="table">Create table expression builder</param>
        public override void MapEntity(CreateTableExpressionBuilder table)
        {
            table.WithColumn(nameof(TvChannelTag.Name)).AsString(400).NotNullable();
        }

        #endregion
    }
}