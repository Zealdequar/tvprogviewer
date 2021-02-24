using FluentMigrator.Builders.Create.Table;
using TVProgViewer.Core.Domain.Catalog;
using TVProgViewer.Core.Domain.Discounts;
using TVProgViewer.Data.Extensions;

namespace TVProgViewer.Data.Mapping.Builders.Discounts
{
    /// <summary>
    /// Represents a discount product mapping entity builder
    /// </summary>
    public partial class DiscountProductMappingBuilder : TvProgEntityBuilder<DiscountProductMapping>
    {
        #region Methods

        /// <summary>
        /// Apply entity configuration
        /// </summary>
        /// <param name="table">Create table expression builder</param>
        public override void MapEntity(CreateTableExpressionBuilder table)
        {
            table
                .WithColumn(NameCompatibilityManager.GetColumnName(typeof(DiscountProductMapping), nameof(DiscountProductMapping.DiscountId)))
                    .AsInt32().PrimaryKey().ForeignKey<Discount>()
                .WithColumn(NameCompatibilityManager.GetColumnName(typeof(DiscountProductMapping), nameof(DiscountProductMapping.EntityId)))
                    .AsInt32().PrimaryKey().ForeignKey<Product>();
        }

        #endregion
    }
}