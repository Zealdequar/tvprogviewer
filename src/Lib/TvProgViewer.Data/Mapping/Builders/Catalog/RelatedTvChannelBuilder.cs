using FluentMigrator.Builders.Create.Table;
using TvProgViewer.Core.Domain.Catalog;

namespace TvProgViewer.Data.Mapping.Builders.Catalog
{
    /// <summary>
    /// Represents a related tvChannel entity builder
    /// </summary>
    public partial class RelatedTvChannelBuilder : TvProgEntityBuilder<RelatedTvChannel>
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