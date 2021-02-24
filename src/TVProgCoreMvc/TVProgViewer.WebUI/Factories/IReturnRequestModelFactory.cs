using System.Threading.Tasks;
using TVProgViewer.Core.Domain.Orders;
using TVProgViewer.WebUI.Models.Order;

namespace TVProgViewer.WebUI.Factories
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
        /// <returns>Submit return request model</returns>
        Task<SubmitReturnRequestModel> PrepareSubmitReturnRequestModelAsync(SubmitReturnRequestModel model, Order order);

        /// <summary>
        /// Prepare the user return requests model
        /// </summary>
        /// <returns>User return requests model</returns>
        Task<UserReturnRequestsModel> PrepareUserReturnRequestsModelAsync();
    }
}
