using LinqToDB.Mapping;
using TVProgViewer.Core.Domain.Orders;

namespace TVProgViewer.Data.Mapping.Orders
{
    /// <summary>
    /// Represents an order note mapping configuration
    /// </summary>
    public partial class OrderNoteMap : TvProgEntityTypeConfiguration<OrderNote>
    {
        #region Methods

        /// <summary>
        /// Configures the entity
        /// </summary>
        /// <param name="builder">The builder to be used to configure the entity</param>
        public override void Configure(EntityMappingBuilder<OrderNote> builder)
        {
            builder.HasTableName(nameof(OrderNote));

            builder.Property(note => note.Note).IsNullable(false);

            builder.Property(note => note.OrderId);
            builder.Property(note => note.DownloadId);
            builder.Property(note => note.DisplayToUser);
            builder.Property(note => note.CreatedOnUtc);
        }

        #endregion
    }
}