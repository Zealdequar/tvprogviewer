using System;
using System.Collections.Generic;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using TVProgViewer.Services.Users;
using TVProgViewer.Core.Domain.Users;

namespace TVProgViewer.Services.Authentication
{
    /// <summary>
    /// Represents service using cookie middleware for the authentication
    /// </summary>
    public partial class CookieAuthenticationService : IAuthenticationService
    {
        #region Fields

        private readonly UserSettings _userSettings;
        private readonly IUserService _userService;
        private readonly IHttpContextAccessor _httpContextAccessor;

        private User _cachedUser;

        #endregion

        #region Ctor

        public CookieAuthenticationService(UserSettings UserSettings,
            IUserService UserService,
            IHttpContextAccessor httpContextAccessor)
        {
            _userSettings = UserSettings;
            _userService = UserService;
            _httpContextAccessor = httpContextAccessor;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Вход в Систему
        /// </summary>
        /// <param name="User">Пользователь</param>
        /// <param name="isPersistent">Сохранять ли сеанс аутентификации в нескольких запросах</param>
        public virtual async void SignIn(User User, bool isPersistent)
        {
            if (User == null)
                throw new ArgumentNullException(nameof(User));

            // Создание заявки на имя пользователя и адрес электронной почты:
            var claims = new List<Claim>();

            if (!string.IsNullOrEmpty(User.UserName))
                claims.Add(new Claim(ClaimTypes.Name, User.UserName, ClaimValueTypes.String, TvProgAuthenticationDefaults.ClaimsIssuer));

            if (!string.IsNullOrEmpty(User.Email))
                claims.Add(new Claim(ClaimTypes.Email, User.Email, ClaimValueTypes.Email, TvProgAuthenticationDefaults.ClaimsIssuer));

            // Создание принципала для текущей аутентфикационной схемы:
            var userIdentity = new ClaimsIdentity(claims, TvProgAuthenticationDefaults.AuthenticationScheme);
            var userPrincipal = new ClaimsPrincipal(userIdentity);

            // Установка значения, показывающая должна ли сессися сохраняться во время, которое было вызован аутентификацией
            var authenticationProperties = new AuthenticationProperties
            {
                IsPersistent = isPersistent,
                IssuedUtc = DateTime.UtcNow
            };

            // Вход
            await _httpContextAccessor.HttpContext.SignInAsync(TvProgAuthenticationDefaults.AuthenticationScheme, userPrincipal, authenticationProperties);

            // Кэширование аутентифицирванного пользователя
            _cachedUser = User;
        }

        /// <summary>
        /// Sign out
        /// </summary>
        public virtual async void SignOut()
        {
            //reset cached User
            _cachedUser = null;

            //and sign out from the current authentication scheme
            await _httpContextAccessor.HttpContext.SignOutAsync(TvProgAuthenticationDefaults.AuthenticationScheme);
        }

        /// <summary>
        /// Get authenticated User
        /// </summary>
        /// <returns>User</returns>
        public virtual User GetAuthenticatedUser()
        {
            //whether there is a cached User
            if (_cachedUser != null)
                return _cachedUser;

            //try to get authenticated user identity
            var authenticateResult = _httpContextAccessor.HttpContext.AuthenticateAsync(TvProgAuthenticationDefaults.AuthenticationScheme).Result;
            if (!authenticateResult.Succeeded)
                return null;

            User User = null;
            if (_userSettings.UsernamesEnabled)
            {
                //try to get User by username
                var usernameClaim = authenticateResult.Principal.FindFirst(claim => claim.Type == ClaimTypes.Name
                    && claim.Issuer.Equals(TvProgAuthenticationDefaults.ClaimsIssuer, StringComparison.InvariantCultureIgnoreCase));
                if (usernameClaim != null)
                    User = _userService.GetUserByUsername(usernameClaim.Value);
            }
            else
            {
                //try to get User by email
                var emailClaim = authenticateResult.Principal.FindFirst(claim => claim.Type == ClaimTypes.Email
                    && claim.Issuer.Equals(TvProgAuthenticationDefaults.ClaimsIssuer, StringComparison.InvariantCultureIgnoreCase));
                if (emailClaim != null)
                    User = _userService.GetUserByEmail(emailClaim.Value);
            }

            // Доступен ли найденный пользователь
            if (User == null || !User.Active || User.RequireReLogin || User.Deleted != null || !_userService.IsRegistered(User))
                return null;

            //cache authenticated User
            _cachedUser = User;

            return _cachedUser;
        }

        #endregion
    }
}