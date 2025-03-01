﻿﻿using System.Threading.Tasks;
﻿using Microsoft.AspNetCore.Mvc;
using TvProgViewer.Core.Domain.Users;

namespace TvProgViewer.Services.Users
{
    /// <summary>
    /// User registration interface
    /// </summary>
    public partial interface IUserRegistrationService
    {
        /// <summary>
        /// Validate user
        /// </summary>
        /// <param name="usernameOrEmail">Username or email</param>
        /// <param name="password">Password</param>
        /// <returns>
        /// Задача представляет асинхронную операцию
        /// The task result contains the result
        /// </returns>
        Task<UserLoginResults> ValidateUserAsync(string usernameOrEmail, string password);

        /// <summary>
        /// Register user
        /// </summary>
        /// <param name="request">Request</param>
        /// <returns>
        /// Задача представляет асинхронную операцию
        /// The task result contains the result
        /// </returns>
        Task<UserRegistrationResult> RegisterUserAsync(UserRegistrationRequest request);

        /// <summary>
        /// Change password
        /// </summary>
        /// <param name="request">Request</param>
        /// <returns>
        /// Задача представляет асинхронную операцию
        /// The task result contains the result
        /// </returns>
        Task<ChangePasswordResult> ChangePasswordAsync(ChangePasswordRequest request);

        /// <summary>
        /// Login passed user
        /// </summary>
        /// <param name="user">User to login</param>
        /// <param name="returnUrl">URL to which the user will return after authentication</param>
        /// <param name="isPersist">Is remember me</param>
        /// <returns>
        /// Задача представляет асинхронную операцию
        /// The task result contains the result of an authentication
        /// </returns>
        Task<IActionResult> SignInUserAsync(User user, string returnUrl, bool isPersist = false);

        /// <summary>
        /// Sets a user email
        /// </summary>
        /// <param name="user">User</param>
        /// <param name="newEmail">New email</param>
        /// <param name="requireValidation">Require validation of new email address</param>
        /// <returns>Задача представляет асинхронную операцию</returns>
        Task SetEmailAsync(User user, string newEmail, bool requireValidation);

        /// <summary>
        /// Sets a user username
        /// </summary>
        /// <param name="user">User</param>
        /// <param name="newUsername">New Username</param>
        /// <returns>Задача представляет асинхронную операцию</returns>
        Task SetUsernameAsync(User user, string newUsername);
    }
}