using FluentMigrator.Builders.Create.Table;
using TVProgViewer.Core.Domain.Stores;
using TVProgViewer.Data.Extensions;

namespace TVProgViewer.Data.Mapping.Builders.Stores
{
    /// <summary>
    /// Represents a store mapping entity builder
    /// </summary>
    public partial class StoreMappingBuilder : TvProgEntityBuilder<StoreMapping>
    {
        #region Methods

        /// <summary>
        /// Apply entity configuration
        /// </summary>
        /// <param name="table">Create table expression builder</param>
        public override void MapEntity(CreateTableExpressionBuilder table)
        {
            table
                .WithColumn(nameof(StoreMapping.EntityName)).AsString(400).NotNullable()
                .WithColumn(nameof(StoreMapping.StoreId)).AsInt32().ForeignKey<Store>();
        }

        #endregion
    }
}