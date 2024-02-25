using System.Threading.Tasks;
using TvProgViewer.Core.Domain.Orders;
using TvProgViewer.WebUI.Models.Order;

namespace TvProgViewer.WebUI.Factories
{
    /// <summary>
    /// Represents the interface of the return request model factory
    /// </summary>
    public partial interface IReturnRequestModelFactory
    {
        /// <summary>
        /// Prepare the submit return request model
        /// </summary>
        /// <param name="model">Submit return request model</param>
        /// <param name="order">Order</param>
        /// <returns>
        /// Задача представляет асинхронную операцию
        /// The task result contains the submit return request model
        /// </returns>
        Task<SubmitReturnRequestModel> PrepareSubmitReturnRequestModelAsync(SubmitReturnRequestModel model, Order order);

        /// <summary>
        /// Prepare the user return requests model
        /// </summary>
        /// <returns>
        /// Задача представляет асинхронную операцию
        /// The task result contains the user return requests model
        /// </returns>
        Task<UserReturnRequestsModel> PrepareUserReturnRequestsModelAsync();
    }
}
