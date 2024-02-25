using System.Threading.Tasks;
using TvProgViewer.Core.Domain.Orders;
using TvProgViewer.Core.Domain.Shipping;
using TvProgViewer.WebUI.Models.Order;

namespace TvProgViewer.WebUI.Factories
{
    /// <summary>
    /// Represents the interface of the order model factory
    /// </summary>
    public partial interface IOrderModelFactory
    {
        /// <summary>
        /// Prepare the user order list model
        /// </summary>
        /// <returns>
        /// Задача представляет асинхронную операцию
        /// The task result contains the user order list model
        /// </returns>
        Task<UserOrderListModel> PrepareUserOrderListModelAsync();

        /// <summary>
        /// Prepare the order details model
        /// </summary>
        /// <param name="order">Order</param>
        /// <returns>
        /// Задача представляет асинхронную операцию
        /// The task result contains the order details model
        /// </returns>
        Task<OrderDetailsModel> PrepareOrderDetailsModelAsync(Order order);

        /// <summary>
        /// Prepare the shipment details model
        /// </summary>
        /// <param name="shipment">Shipment</param>
        /// <returns>
        /// Задача представляет асинхронную операцию
        /// The task result contains the shipment details model
        /// </returns>
        Task<ShipmentDetailsModel> PrepareShipmentDetailsModelAsync(Shipment shipment);

        /// <summary>
        /// Prepare the user reward points model
        /// </summary>
        /// <param name="page">Number of items page; pass null to load the first page</param>
        /// <returns>
        /// Задача представляет асинхронную операцию
        /// The task result contains the user reward points model
        /// </returns>
        Task<UserRewardPointsModel> PrepareUserRewardPointsAsync(int? page);
    }
}
