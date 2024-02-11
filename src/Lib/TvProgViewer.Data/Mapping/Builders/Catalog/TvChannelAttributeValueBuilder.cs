using FluentMigrator.Builders.Create.Table;
using TvProgViewer.Core.Domain.Catalog;
using TvProgViewer.Data.Extensions;

namespace TvProgViewer.Data.Mapping.Builders.Catalog
{
    /// <summary>
    /// Represents a tvchannel attribute value entity builder
    /// </summary>
    public partial class TvChannelAttributeValueBuilder : TvProgEntityBuilder<TvChannelAttributeValue>
    {
        #region Methods

        /// <summary>
        /// Apply entity configuration
        /// </summary>
        /// <param name="table">Create table expression builder</param>
        public override void MapEntity(CreateTableExpressionBuilder table)
        {
            table
                .WithColumn(nameof(TvChannelAttributeValue.Name)).AsString(400).NotNullable()
                .WithColumn(nameof(TvChannelAttributeValue.ColorSquaresRgb)).AsString(100).Nullable()
                .WithColumn(nameof(TvChannelAttributeValue.TvChannelAttributeMappingId)).AsInt32().ForeignKey<TvChannelAttributeMapping>();
        }

        #endregion
    }
}