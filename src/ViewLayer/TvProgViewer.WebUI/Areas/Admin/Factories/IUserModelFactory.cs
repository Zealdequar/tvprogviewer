using System.Threading.Tasks;
using TvProgViewer.Core.Domain.Common;
using TvProgViewer.Core.Domain.Users;
using TvProgViewer.WebUI.Areas.Admin.Models.Users;

namespace TvProgViewer.WebUI.Areas.Admin.Factories
{
    /// <summary>
    /// Represents the user model factory
    /// </summary>
    public partial interface IUserModelFactory
    {
        /// <summary>
        /// Prepare user search model
        /// </summary>
        /// <param name="searchModel">User search model</param>
        /// <returns>
        /// Задача представляет асинхронную операцию
        /// The task result contains the user search model
        /// </returns>
        Task<UserSearchModel> PrepareUserSearchModelAsync(UserSearchModel searchModel);

        /// <summary>
        /// Prepare paged user list model
        /// </summary>
        /// <param name="searchModel">User search model</param>
        /// <returns>
        /// Задача представляет асинхронную операцию
        /// The task result contains the user list model
        /// </returns>
        Task<UserListModel> PrepareUserListModelAsync(UserSearchModel searchModel);

        /// <summary>
        /// Prepare user model
        /// </summary>
        /// <param name="model">User model</param>
        /// <param name="user">User</param>
        /// <param name="excludeProperties">Whether to exclude populating of some properties of model</param>
        /// <returns>
        /// Задача представляет асинхронную операцию
        /// The task result contains the user model
        /// </returns>
        Task<UserModel> PrepareUserModelAsync(UserModel model, User user, bool excludeProperties = false);

        /// <summary>
        /// Prepare paged reward points list model
        /// </summary>
        /// <param name="searchModel">Reward points search model</param>
        /// <param name="user">User</param>
        /// <returns>
        /// Задача представляет асинхронную операцию
        /// The task result contains the reward points list model
        /// </returns>
        Task<UserRewardPointsListModel> PrepareRewardPointsListModelAsync(UserRewardPointsSearchModel searchModel, User user);

        /// <summary>
        /// Prepare paged user address list model
        /// </summary>
        /// <param name="searchModel">User address search model</param>
        /// <param name="user">User</param>
        /// <returns>
        /// Задача представляет асинхронную операцию
        /// The task result contains the user address list model
        /// </returns>
        Task<UserAddressListModel> PrepareUserAddressListModelAsync(UserAddressSearchModel searchModel, User user);

        /// <summary>
        /// Prepare user address model
        /// </summary>
        /// <param name="model">User address model</param>
        /// <param name="user">User</param>
        /// <param name="address">Address</param>
        /// <param name="excludeProperties">Whether to exclude populating of some properties of model</param>
        /// <returns>
        /// Задача представляет асинхронную операцию
        /// The task result contains the user address model
        /// </returns>
        Task<UserAddressModel> PrepareUserAddressModelAsync(UserAddressModel model,
            User user, Address address, bool excludeProperties = false);

        /// <summary>
        /// Prepare paged user order list model
        /// </summary>
        /// <param name="searchModel">User order search model</param>
        /// <param name="user">User</param>
        /// <returns>
        /// Задача представляет асинхронную операцию
        /// The task result contains the user order list model
        /// </returns>
        Task<UserOrderListModel> PrepareUserOrderListModelAsync(UserOrderSearchModel searchModel, User user);

        /// <summary>
        /// Prepare paged user shopping cart list model
        /// </summary>
        /// <param name="searchModel">User shopping cart search model</param>
        /// <param name="user">User</param>
        /// <returns>
        /// Задача представляет асинхронную операцию
        /// The task result contains the user shopping cart list model
        /// </returns>
        Task<UserShoppingCartListModel> PrepareUserShoppingCartListModelAsync(UserShoppingCartSearchModel searchModel,
            User user);

        /// <summary>
        /// Prepare paged user activity log list model
        /// </summary>
        /// <param name="searchModel">User activity log search model</param>
        /// <param name="user">User</param>
        /// <returns>
        /// Задача представляет асинхронную операцию
        /// The task result contains the user activity log list model
        /// </returns>
        Task<UserActivityLogListModel> PrepareUserActivityLogListModelAsync(UserActivityLogSearchModel searchModel, User user);

        /// <summary>
        /// Prepare paged user back in stock subscriptions list model
        /// </summary>
        /// <param name="searchModel">User back in stock subscriptions search model</param>
        /// <param name="user">User</param>
        /// <returns>
        /// Задача представляет асинхронную операцию
        /// The task result contains the user back in stock subscriptions list model
        /// </returns>
        Task<UserBackInStockSubscriptionListModel> PrepareUserBackInStockSubscriptionListModelAsync(
            UserBackInStockSubscriptionSearchModel searchModel, User user);

        /// <summary>
        /// Prepare online user search model
        /// </summary>
        /// <param name="searchModel">Online user search model</param>
        /// <returns>
        /// Задача представляет асинхронную операцию
        /// The task result contains the online user search model
        /// </returns>
        Task<OnlineUserSearchModel> PrepareOnlineUserSearchModelAsync(OnlineUserSearchModel searchModel);

        /// <summary>
        /// Prepare paged online user list model
        /// </summary>
        /// <param name="searchModel">Online user search model</param>
        /// <returns>
        /// Задача представляет асинхронную операцию
        /// The task result contains the online user list model
        /// </returns>
        Task<OnlineUserListModel> PrepareOnlineUserListModelAsync(OnlineUserSearchModel searchModel);

        /// <summary>
        /// Prepare GDPR request (log) search model
        /// </summary>
        /// <param name="searchModel">GDPR request search model</param>
        /// <returns>
        /// Задача представляет асинхронную операцию
        /// The task result contains the gDPR request search model
        /// </returns>
        Task<GdprLogSearchModel> PrepareGdprLogSearchModelAsync(GdprLogSearchModel searchModel);

        /// <summary>
        /// Prepare paged GDPR request list model
        /// </summary>
        /// <param name="searchModel">GDPR request search model</param>
        /// <returns>
        /// Задача представляет асинхронную операцию
        /// The task result contains the gDPR request list model
        /// </returns>
        Task<GdprLogListModel> PrepareGdprLogListModelAsync(GdprLogSearchModel searchModel);
    }
}