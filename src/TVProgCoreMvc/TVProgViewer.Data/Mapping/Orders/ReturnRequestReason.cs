using LinqToDB.Mapping;
using TVProgViewer.Core.Domain.Orders;

namespace TVProgViewer.Data.Mapping.Orders
{
    /// <summary>
    /// Represents a return request reason mapping configuration
    /// </summary>
    public partial class ReturnRequestReasonMap : TvProgEntityTypeConfiguration<ReturnRequestReason>
    {
        #region Methods

        /// <summary>
        /// Configures the entity
        /// </summary>
        /// <param name="builder">The builder to be used to configure the entity</param>
        public override void Configure(EntityMappingBuilder<ReturnRequestReason> builder)
        {
            builder.HasTableName(nameof(ReturnRequestReason));

            builder.Property(reason => reason.Name).HasLength(400).IsNullable(false);
            builder.Property(returnrequestreason => returnrequestreason.DisplayOrder);
        }

        #endregion
    }
}