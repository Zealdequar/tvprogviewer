using FluentMigrator.Builders.Create.Table;
using TVProgViewer.Core.Domain.Catalog;
using TVProgViewer.Data.Extensions;

namespace TVProgViewer.Data.Mapping.Builders.Catalog
{
    /// <summary>
    /// Represents a product specification attribute entity builder
    /// </summary>
    public partial class ProductSpecificationAttributeBuilder : TvProgEntityBuilder<ProductSpecificationAttribute>
    {
        #region Methods

        /// <summary>
        /// Apply entity configuration
        /// </summary>
        /// <param name="table">Create table expression builder</param>
        public override void MapEntity(CreateTableExpressionBuilder table)
        {
            table
                .WithColumn(nameof(ProductSpecificationAttribute.CustomValue)).AsString(4000).Nullable()
                .WithColumn(nameof(ProductSpecificationAttribute.ProductId)).AsInt32().ForeignKey<Product>()
                .WithColumn(nameof(ProductSpecificationAttribute.SpecificationAttributeOptionId)).AsInt32().ForeignKey<SpecificationAttributeOption>();
        }

        #endregion
    }
}