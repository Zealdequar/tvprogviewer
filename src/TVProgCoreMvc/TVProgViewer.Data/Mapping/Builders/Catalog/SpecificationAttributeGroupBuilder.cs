using FluentMigrator.Builders.Create.Table;
using TVProgViewer.Core.Domain.Catalog;

namespace TVProgViewer.Data.Mapping.Builders.Catalog
{
    /// <summary>
    /// Represents a specification attribute group entity builder
    /// </summary>
    public partial class SpecificationAttributeGroupBuilder : TvProgEntityBuilder<SpecificationAttributeGroup>
    {
        #region Methods

        /// <summary>
        /// Apply entity configuration
        /// </summary>
        /// <param name="table">Create table expression builder</param>
        public override void MapEntity(CreateTableExpressionBuilder table)
        {
            table.WithColumn(nameof(SpecificationAttributeGroup.Name)).AsString(int.MaxValue).NotNullable();
        }

        #endregion
    }
}
