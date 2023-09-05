using FluentMigrator.Builders.Create.Table;
using TvProgViewer.Core.Domain.Catalog;
using TvProgViewer.Core.Domain.Users;
using TvProgViewer.Data.Extensions;

namespace TvProgViewer.Data.Mapping.Builders.Catalog
{
    /// <summary>
    /// Represents a tier price entity builder
    /// </summary>
    public partial class TierPriceBuilder : TvProgEntityBuilder<TierPrice>
    {
        #region Methods

        /// <summary>
        /// Apply entity configuration
        /// </summary>
        /// <param name="table">Create table expression builder</param>
        public override void MapEntity(CreateTableExpressionBuilder table)
        {
            table
                .WithColumn(nameof(TierPrice.UserRoleId)).AsInt32().Nullable().ForeignKey<UserRole>()
                .WithColumn(nameof(TierPrice.ProductId)).AsInt32().ForeignKey<Product>();
        }

        #endregion
    }
}