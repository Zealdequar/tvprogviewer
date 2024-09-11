using System.Threading.Tasks;
using TvProgViewer.Core.Domain.Users;
using TvProgViewer.WebUI.Areas.Admin.Models.Users;

namespace TvProgViewer.WebUI.Areas.Admin.Factories
{
    /// <summary>
    /// Represents the user role model factory
    /// </summary>
    public partial interface IUserRoleModelFactory
    {
        /// <summary>
        /// Prepare user role search model
        /// </summary>
        /// <param name="searchModel">User role search model</param>
        /// <returns>
        /// Задача представляет асинхронную операцию
        /// The task result contains the user role search model
        /// </returns>
        Task<UserRoleSearchModel> PrepareUserRoleSearchModelAsync(UserRoleSearchModel searchModel);

        /// <summary>
        /// Prepare paged user role list model
        /// </summary>
        /// <param name="searchModel">User role search model</param>
        /// <returns>
        /// Задача представляет асинхронную операцию
        /// The task result contains the user role list model
        /// </returns>
        Task<UserRoleListModel> PrepareUserRoleListModelAsync(UserRoleSearchModel searchModel);

        /// <summary>
        /// Prepare user role model
        /// </summary>
        /// <param name="model">User role model</param>
        /// <param name="userRole">User role</param>
        /// <param name="excludeProperties">Whether to exclude populating of some properties of model</param>
        /// <returns>
        /// Задача представляет асинхронную операцию
        /// The task result contains the user role model
        /// </returns>
        Task<UserRoleModel> PrepareUserRoleModelAsync(UserRoleModel model, UserRole userRole, bool excludeProperties = false);

        /// <summary>
        /// Prepare user role tvChannel search model
        /// </summary>
        /// <param name="searchModel">User role tvChannel search model</param>
        /// <returns>
        /// Задача представляет асинхронную операцию
        /// The task result contains the user role tvChannel search model
        /// </returns>
        Task<UserRoleTvChannelSearchModel> PrepareUserRoleTvChannelSearchModelAsync(UserRoleTvChannelSearchModel searchModel);

        /// <summary>
        /// Prepare paged user role tvChannel list model
        /// </summary>
        /// <param name="searchModel">User role tvChannel search model</param>
        /// <returns>
        /// Задача представляет асинхронную операцию
        /// The task result contains the user role tvChannel list model
        /// </returns>
        Task<UserRoleTvChannelListModel> PrepareUserRoleTvChannelListModelAsync(UserRoleTvChannelSearchModel searchModel);
    }
}