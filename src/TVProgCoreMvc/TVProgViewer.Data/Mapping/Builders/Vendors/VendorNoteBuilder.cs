using FluentMigrator.Builders.Create.Table;
using TVProgViewer.Core.Domain.Vendors;
using TVProgViewer.Data.Extensions;

namespace TVProgViewer.Data.Mapping.Builders.Vendors
{
    /// <summary>
    /// Represents a vendor note entity builder
    /// </summary>
    public partial class VendorNoteBuilder : TvProgEntityBuilder<VendorNote>
    {
        #region Methods

        /// <summary>
        /// Apply entity configuration
        /// </summary>
        /// <param name="table">Create table expression builder</param>
        public override void MapEntity(CreateTableExpressionBuilder table)
        {
            table
                .WithColumn(nameof(VendorNote.Note)).AsString(int.MaxValue).NotNullable()
                .WithColumn(nameof(VendorNote.VendorId)).AsInt32().ForeignKey<Vendor>();
        }

        #endregion
    }
}