using FluentMigrator.Builders.Create.Table;
using TVProgViewer.Core.Domain.Catalog;
using TVProgViewer.Data.Extensions;

namespace TVProgViewer.Data.Mapping.Builders.Catalog
{
    /// <summary>
    /// Represents a predefined product attribute value entity builder
    /// </summary>
    public partial class PredefinedProductAttributeValueBuilder : TvProgEntityBuilder<PredefinedProductAttributeValue>
    {
        #region Methods

        /// <summary>
        /// Apply entity configuration
        /// </summary>
        /// <param name="table">Create table expression builder</param>
        public override void MapEntity(CreateTableExpressionBuilder table)
        {
            table
                .WithColumn(nameof(PredefinedProductAttributeValue.Name)).AsString(400).NotNullable()
                .WithColumn(nameof(PredefinedProductAttributeValue.ProductAttributeId)).AsInt32().ForeignKey<ProductAttribute>();
        }

        #endregion
    }
}