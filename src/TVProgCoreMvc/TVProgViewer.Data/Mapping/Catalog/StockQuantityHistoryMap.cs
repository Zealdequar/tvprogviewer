using LinqToDB.Mapping;
using TVProgViewer.Core.Domain.Catalog;

namespace TVProgViewer.Data.Mapping.Catalog
{
    /// <summary>
    /// Represents a stock quantity history mapping configuration
    /// </summary>
    public partial class StockQuantityHistoryMap : TvProgEntityTypeConfiguration<StockQuantityHistory>
    {
        #region Methods

        /// <summary>
        /// Configures the entity
        /// </summary>
        /// <param name="builder">The builder to be used to configure the entity</param>
        public override void Configure(EntityMappingBuilder<StockQuantityHistory> builder)
        {
            builder.HasTableName(nameof(StockQuantityHistory));

            builder.Property(stockquantityhistory => stockquantityhistory.QuantityAdjustment);
            builder.Property(stockquantityhistory => stockquantityhistory.StockQuantity);
            builder.Property(stockquantityhistory => stockquantityhistory.Message);
            builder.Property(stockquantityhistory => stockquantityhistory.CreatedOnUtc);
            builder.Property(stockquantityhistory => stockquantityhistory.ProductId);
            builder.Property(stockquantityhistory => stockquantityhistory.CombinationId);
            builder.Property(stockquantityhistory => stockquantityhistory.WarehouseId);
        }

        #endregion
    }
}