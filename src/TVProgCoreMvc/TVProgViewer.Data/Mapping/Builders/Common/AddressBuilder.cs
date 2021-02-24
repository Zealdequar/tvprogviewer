using System.Data;
using FluentMigrator.Builders.Create.Table;
using TVProgViewer.Core.Domain.Common;
using TVProgViewer.Core.Domain.Directory;
using TVProgViewer.Data.Extensions;

namespace TVProgViewer.Data.Mapping.Builders.Common
{
    /// <summary>
    /// Represents a address entity builder
    /// </summary>
    public partial class AddressBuilder : TvProgEntityBuilder<Address>
    {
        #region Methods

        /// <summary>
        /// Apply entity configuration
        /// </summary>
        /// <param name="table">Create table expression builder</param>
        public override void MapEntity(CreateTableExpressionBuilder table)
        {
            table
                .WithColumn(nameof(Address.CountryId)).AsInt32().Nullable().ForeignKey<Country>(onDelete: Rule.None)
                .WithColumn(nameof(Address.StateProvinceId)).AsInt32().Nullable().ForeignKey<StateProvince>(onDelete: Rule.None);
        }

        #endregion
    }
}