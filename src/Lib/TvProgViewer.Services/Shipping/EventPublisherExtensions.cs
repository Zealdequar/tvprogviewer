﻿﻿using System.Threading.Tasks;
using TvProgViewer.Core.Domain.Shipping;
 using TvProgViewer.Core.Events;

namespace TvProgViewer.Services.Shipping
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
        /// <returns>Задача представляет асинхронную операцию</returns>
        public static async Task PublishShipmentSentAsync(this IEventPublisher eventPublisher, Shipment shipment)
        {
            await eventPublisher.PublishAsync(new ShipmentSentEvent(shipment));
        }

        /// <summary>
        /// Publishes the shipment ready for pickup event.
        /// </summary>
        /// <param name="eventPublisher">The event publisher.</param>
        /// <param name="shipment">The shipment.</param>
        /// <returns>Задача представляет асинхронную операцию</returns>
        public static async Task PublishShipmentReadyForPickupAsync(this IEventPublisher eventPublisher, Shipment shipment)
        {
            await eventPublisher.PublishAsync(new ShipmentReadyForPickupEvent(shipment));
        }

        /// <summary>
        /// Publishes the shipment delivered event.
        /// </summary>
        /// <param name="eventPublisher">The event publisher.</param>
        /// <param name="shipment">The shipment.</param>
        /// <returns>Задача представляет асинхронную операцию</returns>
        public static async Task PublishShipmentDeliveredAsync(this IEventPublisher eventPublisher, Shipment shipment)
        {
            await eventPublisher.PublishAsync(new ShipmentDeliveredEvent(shipment));
        }
    }
}