using System.Threading.Tasks;
using TVProgViewer.Core.Domain.Users;

namespace TVProgViewer.Services.Authentication
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
        Task SignInAsync(User user, bool isPersistent);

        /// <summary>
        /// Sign out
        /// </summary>
        Task SignOutAsync();

        /// <summary>
        /// Get authenticated user
        /// </summary>
        /// <returns>User</returns>
        Task<User> GetAuthenticatedUserAsync();
    }
}