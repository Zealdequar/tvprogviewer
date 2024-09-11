using FluentMigrator.Builders.Create.Table;
using TvProgViewer.Core.Domain.Catalog;
using TvProgViewer.Data.Extensions;

namespace TvProgViewer.Data.Mapping.Builders.Catalog
{
    /// <summary>
    /// Represents a tvChannel manufacturer entity builder
    /// </summary>
    public partial class TvChannelManufacturerBuilder : TvProgEntityBuilder<TvChannelManufacturer>
    {
        #region Methods

        /// <summary>
        /// Apply entity configuration
        /// </summary>
        /// <param name="table">Create table expression builder</param>
        public override void MapEntity(CreateTableExpressionBuilder table)
        {
            table
                .WithColumn(nameof(TvChannelManufacturer.ManufacturerId)).AsInt32().ForeignKey<Manufacturer>()
                .WithColumn(nameof(TvChannelManufacturer.TvChannelId)).AsInt32().ForeignKey<TvChannel>();
        }

        #endregion
    }
}