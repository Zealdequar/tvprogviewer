using FluentMigrator.Builders.Create.Table;
using TVProgViewer.Core.Domain.Vendors;
using TVProgViewer.Data.Extensions;

namespace TVProgViewer.Data.Mapping.Builders.Vendors
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