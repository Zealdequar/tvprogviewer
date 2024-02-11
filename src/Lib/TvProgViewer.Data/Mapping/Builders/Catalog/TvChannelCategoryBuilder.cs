using FluentMigrator.Builders.Create.Table;
using TvProgViewer.Core.Domain.Catalog;
using TvProgViewer.Data.Extensions;

namespace TvProgViewer.Data.Mapping.Builders.Catalog
{
    /// <summary>
    /// Represents a tvchannel category entity builder
    /// </summary>
    public partial class TvChannelCategoryBuilder : TvProgEntityBuilder<TvChannelCategory>
    {
        #region Methods

        /// <summary>
        /// Apply entity configuration
        /// </summary>
        /// <param name="table">Create table expression builder</param>
        public override void MapEntity(CreateTableExpressionBuilder table)
        {
            table
                .WithColumn(nameof(TvChannelCategory.CategoryId)).AsInt32().ForeignKey<Category>()
                .WithColumn(nameof(TvChannelCategory.TvChannelId)).AsInt32().ForeignKey<TvChannel>();
        }

        #endregion
    }
}