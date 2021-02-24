using FluentMigrator.Builders.Create.Table;
using TVProgViewer.Core.Domain.Common;

namespace TVProgViewer.Data.Mapping.Builders.Common
{
    /// <summary>
    /// Represents a generic attribute entity builder
    /// </summary>
    public partial class GenericAttributeBuilder : TvProgEntityBuilder<GenericAttribute>
    {
        #region Methods

        /// <summary>
        /// Apply entity configuration
        /// </summary>
        /// <param name="table">Create table expression builder</param>
        public override void MapEntity(CreateTableExpressionBuilder table)
        {
            table
                .WithColumn(nameof(GenericAttribute.KeyGroup)).AsString(400).NotNullable()
                .WithColumn(nameof(GenericAttribute.Key)).AsString(400).NotNullable()
                .WithColumn(nameof(GenericAttribute.Value)).AsString(int.MaxValue).NotNullable();
        }

        #endregion
    }
}