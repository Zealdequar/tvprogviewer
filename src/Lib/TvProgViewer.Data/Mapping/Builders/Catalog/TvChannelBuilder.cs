using FluentMigrator.Builders.Create.Table;
using TvProgViewer.Core.Domain.Catalog;

namespace TvProgViewer.Data.Mapping.Builders.Catalog
{
    /// <summary>
    /// Represents a tvchannel entity builder
    /// </summary>
    public partial class TvChannelBuilder : TvProgEntityBuilder<TvChannel>
    {
        #region Methods

        /// <summary>
        /// Apply entity configuration
        /// </summary>
        /// <param name="table">Create table expression builder</param>
        public override void MapEntity(CreateTableExpressionBuilder table)
        {
            table
                .WithColumn(nameof(TvChannel.Name)).AsString(400).NotNullable()
                .WithColumn(nameof(TvChannel.MetaKeywords)).AsString(400).Nullable()
                .WithColumn(nameof(TvChannel.MetaTitle)).AsString(400).Nullable()
                .WithColumn(nameof(TvChannel.Sku)).AsString(400).Nullable()
                .WithColumn(nameof(TvChannel.ManufacturerPartNumber)).AsString(400).Nullable()
                .WithColumn(nameof(TvChannel.Gtin)).AsString(400).Nullable()
                .WithColumn(nameof(TvChannel.RequiredTvChannelIds)).AsString(1000).Nullable()
                .WithColumn(nameof(TvChannel.AllowedQuantities)).AsString(1000).Nullable();
        }

        #endregion
    }
}