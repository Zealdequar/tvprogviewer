using System.Data;
using FluentMigrator.Builders.Create.Table;
using TVProgViewer.Core.Domain.Affiliates;
using TVProgViewer.Core.Domain.Common;
using TVProgViewer.Data.Extensions;

namespace TVProgViewer.Data.Mapping.Builders.Affiliates
{
    /// <summary>
    /// Represents a affiliate entity builder
    /// </summary>
    public class AffiliateBuilder : TvProgEntityBuilder<Affiliate>
    {
        #region Methods

        /// <summary>
        /// Apply entity configuration
        /// </summary>
        /// <param name="table">Create table expression builder</param>
        public override void MapEntity(CreateTableExpressionBuilder table)
        {
            table
                .WithColumn(nameof(Affiliate.AddressId)).AsInt32().ForeignKey<Address>().OnDelete(Rule.None);
        }

        #endregion
    }
}