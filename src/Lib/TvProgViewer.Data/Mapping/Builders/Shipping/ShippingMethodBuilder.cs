using FluentMigrator.Builders.Create.Table;
using TvProgViewer.Core.Domain.Shipping;

namespace TvProgViewer.Data.Mapping.Builders.Shipping
{
    /// <summary>
    /// Represents a shipping method entity builder
    /// </summary>
    public partial class ShippingMethodBuilder : TvProgEntityBuilder<ShippingMethod>
    {
        #region Methods

        /// <summary>
        /// Apply entity configuration
        /// </summary>
        /// <param name="table">Create table expression builder</param>
        public override void MapEntity(CreateTableExpressionBuilder table)
        {
            table.WithColumn(nameof(ShippingMethod.Name)).AsString(400).NotNullable();
        }

        #endregion
    }
}