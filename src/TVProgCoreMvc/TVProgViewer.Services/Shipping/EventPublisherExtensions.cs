using System.Threading.Tasks;
using TVProgViewer.Core.Domain.Shipping;
using TVProgViewer.Core.Events;
using TVProgViewer.Services.Events;

namespace TVProgViewer.Services.Shipping
{
    /// <summary>
    /// Event publisher extensions
    /// </summary>
    public static class EventPublisherExtensions
    {
        /// <summary>
        /// Publishes the shipment sent event.
        /// </summary>
        /// <param name="eventPublisher">The event publisher.</param>
        /// <param name="shipment">The shipment.</param>
        public static async Task PublishShipmentSentAsync(this IEventPublisher eventPublisher, Shipment shipment)
        {
            await eventPublisher.PublishAsync(new ShipmentSentEvent(shipment));
        }

        /// <summary>
        /// Publishes the shipment delivered event.
        /// </summary>
        /// <param name="eventPublisher">The event publisher.</param>
        /// <param name="shipment">The shipment.</param>
        public static async Task PublishShipmentDeliveredAsync(this IEventPublisher eventPublisher, Shipment shipment)
        {
            await eventPublisher.PublishAsync(new ShipmentDeliveredEvent(shipment));
        }
    }
}