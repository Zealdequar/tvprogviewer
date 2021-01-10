using LinqToDB.Mapping;
using TVProgViewer.Core.Domain.Orders;

namespace TVProgViewer.Data.Mapping.Orders
{
    /// <summary>
    /// Represents a recurring payment history mapping configuration
    /// </summary>
    public partial class RecurringPaymentHistoryMap : TvProgEntityTypeConfiguration<RecurringPaymentHistory>
    {
        #region Methods

        /// <summary>
        /// Configures the entity
        /// </summary>
        /// <param name="builder">The builder to be used to configure the entity</param>
        public override void Configure(EntityMappingBuilder<RecurringPaymentHistory> builder)
        {
            builder.HasTableName(nameof(RecurringPaymentHistory));

            builder.Property(historyEntry => historyEntry.RecurringPaymentId);
            builder.Property(historyEntry => historyEntry.OrderId);
            builder.Property(historyEntry => historyEntry.CreatedOnUtc);
        }

        #endregion
    }
}