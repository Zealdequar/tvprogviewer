using FluentMigrator.Builders.Create.Table;
using TvProgViewer.Core.Domain.Catalog;
using TvProgViewer.Data.Extensions;

namespace TvProgViewer.Data.Mapping.Builders.Catalog
{
    /// <summary>
    /// Represents a tvChannel attribute combination entity builder
    /// </summary>
    public partial class TvChannelAttributeCombinationBuilder : TvProgEntityBuilder<TvChannelAttributeCombination>
    {
        #region Methods

        /// <summary>
        /// Apply entity configuration
        /// </summary>
        /// <param name="table">Create table expression builder</param>
        public override void MapEntity(CreateTableExpressionBuilder table)
        {
            table
                .WithColumn(nameof(TvChannelAttributeCombination.Sku)).AsString(400).Nullable()
                .WithColumn(nameof(TvChannelAttributeCombination.ManufacturerPartNumber)).AsString(400).Nullable()
                .WithColumn(nameof(TvChannelAttributeCombination.Gtin)).AsString(400).Nullable()
                .WithColumn(nameof(TvChannelAttributeCombination.TvChannelId)).AsInt32().ForeignKey<TvChannel>();
        }

        #endregion
    }
}