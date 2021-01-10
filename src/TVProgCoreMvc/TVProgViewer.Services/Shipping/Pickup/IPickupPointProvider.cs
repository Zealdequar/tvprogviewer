using TVProgViewer.Core.Domain.Common;
using TVProgViewer.Services.Plugins;
using TVProgViewer.Services.Shipping.Tracking;

namespace TVProgViewer.Services.Shipping.Pickup
{
    /// <summary>
    /// Represents an interface of pickup point provider
    /// </summary>
    public partial interface IPickupPointProvider : IPlugin
    {
        #region Properties

        /// <summary>
        /// Gets a shipment tracker
        /// </summary>
        IShipmentTracker ShipmentTracker { get; }

        #endregion

        #region Methods

        /// <summary>
        /// Get pickup points for the address
        /// </summary>
        /// <param name="address">Address</param>
        /// <returns>Represents a response of getting pickup points</returns>
        GetPickupPointsResponse GetPickupPoints(Address address);
        
        #endregion
    }
}