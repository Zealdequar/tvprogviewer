using FluentMigrator.Builders.Create.Table;
using TVProgViewer.Core.Domain.Catalog;

namespace TVProgViewer.Data.Mapping.Builders.Catalog
{
    /// <summary>
    /// Represents a category template entity builder
    /// </summary>
    public class CategoryTemplateBuilder : TvProgEntityBuilder<CategoryTemplate>
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