using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using TVProgViewer.Core.Domain.Users;

namespace TVProgViewer.Services.Users
{
    /// <summary>
    /// User registration interface
    /// </summary>
    public partial interface IUserRegistrationService
    {
        /// <summary>
        /// Validate User
        /// </summary>
        /// <param name="usernameOrEmail">Username or email</param>
        /// <param name="password">Password</param>
        /// <returns>Result</returns>
        Task<UserLoginResults> ValidateUserAsync(string usernameOrEmail, string password);

        /// <summary>
        /// Register User
        /// </summary>
        /// <param name="request">Request</param>
        /// <returns>Result</returns>
        Task<UserRegistrationResult> RegisterUserAsync(UserRegistrationRequest request);

        /// <summary>
        /// Change password
        /// </summary>
        /// <param name="request">Request</param>
        /// <returns>Result</returns>
        Task<ChangePasswordResult> ChangePasswordAsync(ChangePasswordRequest request);

        /// <summary>
        /// Login passed user
        /// </summary>
        /// <param name="user">User to login</param>
        /// <param name="returnUrl">URL to which the user will return after authentication</param>
        /// <param name="isPersist">Is remember me</param>
        /// <returns>Result of an authentication</returns>
        Task<IActionResult> SignInUserAsync(User user, string returnUrl, bool isPersist = false);

        /// <summary>
        /// Sets a user email
        /// </summary>
        /// <param name="user">User</param>
        /// <param name="newEmail">New email</param>
        /// <param name="requireValidation">Require validation of new email address</param>
        Task SetEmailAsync(User user, string newEmail, bool requireValidation);

        /// <summary>
        /// Sets a user username
        /// </summary>
        /// <param name="user">User</param>
        /// <param name="newUsername">New Username</param>
        Task SetUsernameAsync(User user, string newUsername);
    }
}