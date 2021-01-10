using TVProgViewer.Core.Domain.Orders;

namespace TVProgViewer.Services.Payments
{
    /// <summary>
    /// Represents a PostProcessPaymentRequest
    /// </summary>
    public partial class PostProcessPaymentRequest
    {
        /// <summary>
        /// Gets or sets an order. Used when order is already saved (payment gateways that redirect a User to a third-party URL)
        /// </summary>
        public Order Order { get; set; }
    }
}
