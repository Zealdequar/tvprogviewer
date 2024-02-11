using FluentMigrator.Builders.Create.Table;
using TvProgViewer.Core.Domain.Catalog;
using TvProgViewer.Data.Extensions;

namespace TvProgViewer.Data.Mapping.Builders.Catalog
{
    /// <summary>
    /// Represents a tvchannel attribute mapping entity builder
    /// </summary>
    public partial class TvChannelAttributeMappingBuilder : TvProgEntityBuilder<TvChannelAttributeMapping>
    {
        #region Methods

        /// <summary>
        /// Apply entity configuration
        /// </summary>
        /// <param name="table">Create table expression builder</param>
        public override void MapEntity(CreateTableExpressionBuilder table)
        {
            table
                .WithColumn(nameof(TvChannelAttributeMapping.TvChannelAttributeId)).AsInt32().ForeignKey<TvChannelAttribute>()
                .WithColumn(nameof(TvChannelAttributeMapping.TvChannelId)).AsInt32().ForeignKey<TvChannel>();
        }

        #endregion
    }
}