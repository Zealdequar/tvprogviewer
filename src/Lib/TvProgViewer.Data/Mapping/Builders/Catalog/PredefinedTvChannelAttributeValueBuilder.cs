using FluentMigrator.Builders.Create.Table;
using TvProgViewer.Core.Domain.Catalog;
using TvProgViewer.Data.Extensions;

namespace TvProgViewer.Data.Mapping.Builders.Catalog
{
    /// <summary>
    /// Represents a predefined tvchannel attribute value entity builder
    /// </summary>
    public partial class PredefinedTvChannelAttributeValueBuilder : TvProgEntityBuilder<PredefinedTvChannelAttributeValue>
    {
        #region Methods

        /// <summary>
        /// Apply entity configuration
        /// </summary>
        /// <param name="table">Create table expression builder</param>
        public override void MapEntity(CreateTableExpressionBuilder table)
        {
            table
                .WithColumn(nameof(PredefinedTvChannelAttributeValue.Name)).AsString(400).NotNullable()
                .WithColumn(nameof(PredefinedTvChannelAttributeValue.TvChannelAttributeId)).AsInt32().ForeignKey<TvChannelAttribute>();
        }

        #endregion
    }
}