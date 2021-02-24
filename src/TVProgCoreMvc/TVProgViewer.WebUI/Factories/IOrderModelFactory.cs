using System.Threading.Tasks;
using TVProgViewer.Core.Domain.Orders;
using TVProgViewer.Core.Domain.Shipping;
using TVProgViewer.WebUI.Models.Order;

namespace TVProgViewer.WebUI.Factories
{
    /// <summary>
    /// Represents the interface of the order model factory
    /// </summary>
    public partial interface IOrderModelFactory
    {
        /// <summary>
        /// Prepare the user order list model
        /// </summary>
        /// <returns>User order list model</returns>
        Task<UserOrderListModel> PrepareUserOrderListModelAsync();

        /// <summary>
        /// Prepare the order details model
        /// </summary>
        /// <param name="order">Order</param>
        /// <returns>Order details model</returns>
        Task<OrderDetailsModel> PrepareOrderDetailsModelAsync(Order order);

        /// <summary>
        /// Prepare the shipment details model
        /// </summary>
        /// <param name="shipment">Shipment</param>
        /// <returns>Shipment details model</returns>
        Task<ShipmentDetailsModel> PrepareShipmentDetailsModelAsync(Shipment shipment);

        /// <summary>
        /// Prepare the user reward points model
        /// </summary>
        /// <param name="page">Number of items page; pass null to load the first page</param>
        /// <returns>User reward points model</returns>
        Task<UserRewardPointsModel> PrepareUserRewardPointsAsync(int? page);
    }
}
