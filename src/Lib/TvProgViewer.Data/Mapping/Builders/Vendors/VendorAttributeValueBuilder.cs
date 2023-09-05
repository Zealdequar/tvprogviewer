using FluentMigrator.Builders.Create.Table;
using TvProgViewer.Core.Domain.Vendors;
using TvProgViewer.Data.Extensions;

namespace TvProgViewer.Data.Mapping.Builders.Vendors
{
    /// <summary>
    /// Represents a vendor attribute value entity builder
    /// </summary>
    public partial class VendorAttributeValueBuilder : TvProgEntityBuilder<VendorAttributeValue>
    {
        #region Methods

        /// <summary>
        /// Apply entity configuration
        /// </summary>
        /// <param name="table">Create table expression builder</param>
        public override void MapEntity(CreateTableExpressionBuilder table)
        {
            table
                .WithColumn(nameof(VendorAttributeValue.Name)).AsString(400).NotNullable()
                .WithColumn(nameof(VendorAttributeValue.VendorAttributeId)).AsInt32().ForeignKey<VendorAttribute>();
        }

        #endregion
    }
}