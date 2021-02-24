using FluentMigrator.Builders.Create.Table;
using TVProgViewer.Core.Domain.Catalog;

namespace TVProgViewer.Data.Mapping.Builders.Catalog
{
    /// <summary>
    /// Represents a review type entity builder
    /// </summary>
    public partial class ReviewTypeBuilder : TvProgEntityBuilder<ReviewType>
    {
        #region Methods

        /// <summary>
        /// Apply entity configuration
        /// </summary>
        /// <param name="table">Create table expression builder</param>
        public override void MapEntity(CreateTableExpressionBuilder table)
        {
            table
                .WithColumn(nameof(ReviewType.Name)).AsString(400).NotNullable()
                .WithColumn(nameof(ReviewType.Description)).AsString(400).NotNullable();
        }

        #endregion
    }
}
