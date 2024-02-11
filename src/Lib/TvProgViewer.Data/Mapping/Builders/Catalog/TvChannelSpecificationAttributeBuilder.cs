using FluentMigrator.Builders.Create.Table;
using TvProgViewer.Core.Domain.Catalog;
using TvProgViewer.Data.Extensions;

namespace TvProgViewer.Data.Mapping.Builders.Catalog
{
    /// <summary>
    /// Represents a tvchannel specification attribute entity builder
    /// </summary>
    public partial class TvChannelSpecificationAttributeBuilder : TvProgEntityBuilder<TvChannelSpecificationAttribute>
    {
        #region Methods

        /// <summary>
        /// Apply entity configuration
        /// </summary>
        /// <param name="table">Create table expression builder</param>
        public override void MapEntity(CreateTableExpressionBuilder table)
        {
            table
                .WithColumn(nameof(TvChannelSpecificationAttribute.CustomValue)).AsString(4000).Nullable()
                .WithColumn(nameof(TvChannelSpecificationAttribute.TvChannelId)).AsInt32().ForeignKey<TvChannel>()
                .WithColumn(nameof(TvChannelSpecificationAttribute.SpecificationAttributeOptionId)).AsInt32().ForeignKey<SpecificationAttributeOption>();
        }

        #endregion
    }
}