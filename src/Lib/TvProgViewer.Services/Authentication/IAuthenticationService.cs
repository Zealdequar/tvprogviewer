using System.Threading.Tasks;
using TvProgViewer.Core.Domain.Users;

namespace TvProgViewer.Services.Authentication
{
    /// <summary>
    /// Authentication service interface
    /// </summary>
    public partial interface IAuthenticationService 
    {
        /// <summary>
        /// Sign in
        /// </summary>
        /// <param name="user">User</param>
        /// <param name="isPersistent">Whether the authentication session is persisted across multiple requests</param>
        /// <returns>Задача представляет асинхронную операцию</returns>
        Task SignInAsync(User user, bool isPersistent);

        /// <summary>
        /// Sign out
        /// </summary>
        /// <returns>Задача представляет асинхронную операцию</returns>
        Task SignOutAsync();

        /// <summary>
        /// Get authenticated user
        /// </summary>
        /// <returns>
        /// Задача представляет асинхронную операцию
        /// The task result contains the user
        /// </returns>
        Task<User> GetAuthenticatedUserAsync();
    }
}