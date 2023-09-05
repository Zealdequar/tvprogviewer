using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using TvProgViewer.Services.Authentication.External;
using TvProgViewer.Services.Users;
using TvProgViewer.Services.Events;

namespace TvProgViewer.Plugin.ExternalAuth.Facebook.Infrastructure
{
    /// <summary>
    /// Facebook authentication event consumer (used for saving user fields on registration)
    /// </summary>
    public class FacebookAuthenticationEventConsumer : IConsumer<UserAutoRegisteredByExternalMethodEvent>
    {
        #region Fields

        private readonly IUserService _userService;

        #endregion

        #region Ctor

        public FacebookAuthenticationEventConsumer(IUserService userService)
        {
            _userService = userService;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Handle event
        /// </summary>
        /// <param name="eventMessage">Event message</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        public async Task HandleEventAsync(UserAutoRegisteredByExternalMethodEvent eventMessage)
        {
            if (eventMessage?.User == null || eventMessage.AuthenticationParameters == null)
                return;

            //handle event only for this authentication method
            if (!eventMessage.AuthenticationParameters.ProviderSystemName.Equals(FacebookAuthenticationDefaults.SystemName))
                return;

            var user = eventMessage.User;
            //store some of the user fields
            var firstName = eventMessage.AuthenticationParameters.Claims?.FirstOrDefault(claim => claim.Type == ClaimTypes.GivenName)?.Value;
            if (!string.IsNullOrEmpty(firstName))
                user.FirstName = firstName;

            var lastName = eventMessage.AuthenticationParameters.Claims?.FirstOrDefault(claim => claim.Type == ClaimTypes.Surname)?.Value;
            if (!string.IsNullOrEmpty(lastName))
                user.LastName = lastName;

            await _userService.UpdateUserAsync(user);
        }

        #endregion
    }
}