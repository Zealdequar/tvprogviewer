using LinqToDB.Mapping;
using TVProgViewer.Core.Domain.Orders;

namespace TVProgViewer.Data.Mapping.Orders
{
    /// <summary>
    /// Represents a return request mapping configuration
    /// </summary>
    public partial class ReturnRequestMap : TvProgEntityTypeConfiguration<ReturnRequest>
    {
        #region Methods

        /// <summary>
        /// Configures the entity
        /// </summary>
        /// <param name="builder">The builder to be used to configure the entity</param>
        public override void Configure(EntityMappingBuilder<ReturnRequest> builder)
        {
            builder.HasTableName(nameof(ReturnRequest));

            builder.Property(returnRequest => returnRequest.ReasonForReturn).IsNullable(false);
            builder.Property(returnRequest => returnRequest.RequestedAction).IsNullable(false);
            builder.Property(returnrequest => returnrequest.CustomNumber);
            builder.Property(returnrequest => returnrequest.StoreId);
            builder.Property(returnrequest => returnrequest.OrderItemId);
            builder.Property(returnrequest => returnrequest.UserId);
            builder.Property(returnrequest => returnrequest.Quantity);
            builder.Property(returnrequest => returnrequest.UserComments);
            builder.Property(returnrequest => returnrequest.UploadedFileId);
            builder.Property(returnrequest => returnrequest.StaffNotes);
            builder.Property(returnrequest => returnrequest.ReturnRequestStatusId);
            builder.Property(returnrequest => returnrequest.CreatedOnUtc);
            builder.Property(returnrequest => returnrequest.UpdatedOnUtc);

            builder.Ignore(returnRequest => returnRequest.ReturnRequestStatus);
        }

        #endregion
    }
}