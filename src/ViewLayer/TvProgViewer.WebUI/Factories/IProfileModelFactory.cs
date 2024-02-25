using System.Threading.Tasks;
using TvProgViewer.Core.Domain.Users;
using TvProgViewer.WebUI.Models.Profile;

namespace TvProgViewer.WebUI.Factories
{
    /// <summary>
    /// Represents the interface of the profile model factory
    /// </summary>
    public partial interface IProfileModelFactory
    {
        /// <summary>
        /// Prepare the profile index model
        /// </summary>
        /// <param name="user">User</param>
        /// <param name="page">Number of posts page; pass null to disable paging</param>
        /// <returns>
        /// Задача представляет асинхронную операцию
        /// The task result contains the profile index model
        /// </returns>
        Task<ProfileIndexModel> PrepareProfileIndexModelAsync(User user, int? page);

        /// <summary>
        /// Prepare the profile info model
        /// </summary>
        /// <param name="user">User</param>
        /// <returns>
        /// Задача представляет асинхронную операцию
        /// The task result contains the profile info model
        /// </returns>
        Task<ProfileInfoModel> PrepareProfileInfoModelAsync(User user);

        /// <summary>
        /// Prepare the profile posts model
        /// </summary>
        /// <param name="user">User</param>
        /// <param name="page">Number of posts page</param>
        /// <returns>
        /// Задача представляет асинхронную операцию
        /// The task result contains the profile posts model  
        /// </returns>
        Task<ProfilePostsModel> PrepareProfilePostsModelAsync(User user, int page);
    }
}
