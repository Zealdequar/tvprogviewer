using FluentMigrator.Builders.Create.Table;
using TvProgViewer.Core.Domain.Discounts;

namespace TvProgViewer.Data.Mapping.Builders.Discounts
{
    /// <summary>
    /// Represents a discount entity builder
    /// </summary>
    public partial class DiscountBuilder : TvProgEntityBuilder<Discount>
    {
        #region Methods

        /// <summary>
        /// Apply entity configuration
        /// </summary>
        /// <param name="table">Create table expression builder</param>
        public override void MapEntity(CreateTableExpressionBuilder table)
        {
            table
               .WithColumn(nameof(Discount.Name)).AsString(200).NotNullable()
               .WithColumn(nameof(Discount.CouponCode)).AsString(100).Nullable();
        }

        #endregion
    }
}