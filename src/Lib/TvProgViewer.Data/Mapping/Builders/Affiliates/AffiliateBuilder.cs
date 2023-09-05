using System.Data;
using FluentMigrator.Builders.Create.Table;
using TvProgViewer.Core.Domain.Affiliates;
using TvProgViewer.Core.Domain.Common;
using TvProgViewer.Data.Extensions;

namespace TvProgViewer.Data.Mapping.Builders.Affiliates
{
    /// <summary>
    /// Represents a affiliate entity builder
    /// </summary>
    public partial class AffiliateBuilder : TvProgEntityBuilder<Affiliate>
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