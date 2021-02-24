using FluentMigrator.Builders.Create.Table;
using TVProgViewer.Core.Domain.Catalog;

namespace TVProgViewer.Data.Mapping.Builders.Catalog
{
    /// <summary>
    /// Represents a cross sell product entity builder
    /// </summary>
    public partial class CrossSellProductBuilder : TvProgEntityBuilder<CrossSellProduct>
    {
        #region Methods

        /// <summary>
        /// Apply entity configuration
        /// </summary>
        /// <param name="table">Create table expression builder</param>
        public override void MapEntity(CreateTableExpressionBuilder table)
        {
        }

        #endregion
    }
}