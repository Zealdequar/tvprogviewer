using System.Threading.Tasks;
using TvProgViewer.Core;
using TvProgViewer.Core.Domain.Catalog;

namespace TvProgViewer.Services.Catalog
{
    /// <summary>
    /// Back in stock subscription service interface
    /// </summary>
    public partial interface IBackInStockSubscriptionService
    {
        /// <summary>
        /// Delete a back in stock subscription
        /// </summary>
        /// <param name="subscription">Subscription</param>
        /// <returns>Задача представляет асинхронную операцию</returns>
        Task DeleteSubscriptionAsync(BackInStockSubscription subscription);

        /// <summary>
        /// Gets all subscriptions
        /// </summary>
        /// <param name="userId">User identifier</param>
        /// <param name="storeId">Store identifier; pass 0 to load all records</param>
        /// <param name="pageIndex">Page index</param>
        /// <param name="pageSize">Page size</param>
        /// <returns>
        /// Задача представляет асинхронную операцию
        /// The task result contains the subscriptions
        /// </returns>
        Task<IPagedList<BackInStockSubscription>> GetAllSubscriptionsByUserIdAsync(int userId,
            int storeId = 0, int pageIndex = 0, int pageSize = int.MaxValue);

        /// <summary>
        /// Gets all subscriptions
        /// </summary>
        /// <param name="userId">User id</param>
        /// <param name="tvchannelId">TvChannel identifier</param>
        /// <param name="storeId">Store identifier</param>
        /// <returns>
        /// Задача представляет асинхронную операцию
        /// The task result contains the subscriptions
        /// </returns>
        Task<BackInStockSubscription> FindSubscriptionAsync(int userId, int tvchannelId, int storeId);

        /// <summary>
        /// Gets a subscription
        /// </summary>
        /// <param name="subscriptionId">Subscription identifier</param>
        /// <returns>
        /// Задача представляет асинхронную операцию
        /// The task result contains the subscription
        /// </returns>
        Task<BackInStockSubscription> GetSubscriptionByIdAsync(int subscriptionId);

        /// <summary>
        /// Inserts subscription
        /// </summary>
        /// <param name="subscription">Subscription</param>
        /// <returns>Задача представляет асинхронную операцию</returns>
        Task InsertSubscriptionAsync(BackInStockSubscription subscription);

        /// <summary>
        /// Send notification to subscribers
        /// </summary>
        /// <param name="tvchannel">TvChannel</param>
        /// <returns>
        /// Задача представляет асинхронную операцию
        /// The task result contains the number of sent email
        /// </returns>
        Task<int> SendNotificationsToSubscribersAsync(TvChannel tvchannel);
        
        /// <summary>
        /// Gets all subscriptions
        /// </summary>
        /// <param name="tvchannelId">TvChannel identifier</param>
        /// <param name="storeId">Store identifier; pass 0 to load all records</param>
        /// <param name="pageIndex">Page index</param>
        /// <param name="pageSize">Page size</param>
        /// <returns>
        /// Задача представляет асинхронную операцию
        /// The task result contains the subscriptions
        /// </returns>
        Task<IPagedList<BackInStockSubscription>> GetAllSubscriptionsByTvChannelIdAsync(int tvchannelId,
            int storeId = 0, int pageIndex = 0, int pageSize = int.MaxValue);
    }
}
