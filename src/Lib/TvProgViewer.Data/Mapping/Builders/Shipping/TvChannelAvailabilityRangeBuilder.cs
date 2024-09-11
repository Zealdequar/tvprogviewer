using FluentMigrator.Builders.Create.Table;
using TvProgViewer.Core.Domain.Shipping;

namespace TvProgViewer.Data.Mapping.Builders.Shipping
{
    /// <summary>
    /// Represents a tvChannel availability range entity builder
    /// </summary>
    public partial class TvChannelAvailabilityRangeBuilder : TvProgEntityBuilder<TvChannelAvailabilityRange>
    {
        #region Methods

        /// <summary>
        /// Apply entity configuration
        /// </summary>
        /// <param name="table">Create table expression builder</param>
        public override void MapEntity(CreateTableExpressionBuilder table)
        {
            table.WithColumn(nameof(TvChannelAvailabilityRange.Name)).AsString(400).NotNullable();
        }

        #endregion
    }
}