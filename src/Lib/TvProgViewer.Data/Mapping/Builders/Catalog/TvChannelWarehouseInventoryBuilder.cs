using FluentMigrator.Builders.Create.Table;
using TvProgViewer.Core.Domain.Catalog;
using TvProgViewer.Core.Domain.Shipping;
using TvProgViewer.Data.Extensions;

namespace TvProgViewer.Data.Mapping.Builders.Catalog
{
    /// <summary>
    /// Represents a tvChannel warehouse inventory entity builder
    /// </summary>
    public partial class TvChannelWarehouseInventoryBuilder : TvProgEntityBuilder<TvChannelWarehouseInventory>
    {
        #region Methods

        /// <summary>
        /// Apply entity configuration
        /// </summary>
        /// <param name="table">Create table expression builder</param>
        public override void MapEntity(CreateTableExpressionBuilder table)
        {
            table
                .WithColumn(nameof(TvChannelWarehouseInventory.TvChannelId)).AsInt32().ForeignKey<TvChannel>()
                .WithColumn(nameof(TvChannelWarehouseInventory.WarehouseId)).AsInt32().ForeignKey<Warehouse>();
        }

        #endregion
    }
}