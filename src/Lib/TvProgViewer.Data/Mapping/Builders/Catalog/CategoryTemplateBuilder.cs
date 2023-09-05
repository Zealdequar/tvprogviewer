using FluentMigrator.Builders.Create.Table;
using TvProgViewer.Core.Domain.Catalog;

namespace TvProgViewer.Data.Mapping.Builders.Catalog
{
    /// <summary>
    /// Represents a category template entity builder
    /// </summary>
    public partial class CategoryTemplateBuilder : TvProgEntityBuilder<CategoryTemplate>
    {
        #region Methods

        /// <summary>
        /// Apply entity configuration
        /// </summary>
        /// <param name="table">Create table expression builder</param>
        public override void MapEntity(CreateTableExpressionBuilder table) 
        {
            table
                .WithColumn(nameof(CategoryTemplate.Name)).AsString(400).NotNullable()
                .WithColumn(nameof(CategoryTemplate.ViewPath)).AsString(400).NotNullable();
        }

        #endregion
    }
}