using FluentMigrator.Builders.Create.Table;
using TVProgViewer.Core.Domain.Directory;
using TVProgViewer.Data.Extensions;

namespace TVProgViewer.Data.Mapping.Builders.Directory
{
    /// <summary>
    /// Represents a state and province entity builder
    /// </summary>
    public partial class StateProvinceBuilder : TvProgEntityBuilder<StateProvince>
    {
        #region Methods

        /// <summary>
        /// Apply entity configuration
        /// </summary>
        /// <param name="table">Create table expression builder</param>
        public override void MapEntity(CreateTableExpressionBuilder table)
        {
            table
                .WithColumn(nameof(StateProvince.Name)).AsString(100).NotNullable()
                .WithColumn(nameof(StateProvince.Abbreviation)).AsString(100).Nullable()
                .WithColumn(nameof(StateProvince.CountryId)).AsInt32().ForeignKey<Country>();
        }

        #endregion
    }
}