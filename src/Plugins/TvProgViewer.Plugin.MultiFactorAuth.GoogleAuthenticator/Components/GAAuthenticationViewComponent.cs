using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using TvProgViewer.Plugin.MultiFactorAuth.GoogleAuthenticator.Factories;
using TvProgViewer.Plugin.MultiFactorAuth.GoogleAuthenticator.Models;
using TvProgViewer.Web.Framework.Components;

namespace TvProgViewer.Plugin.MultiFactorAuth.GoogleAuthenticator.Components
{
    /// <summary>
    /// Represents view component for setting GoogleAuthenticator
    /// </summary>
    public class GAAuthenticationViewComponent : TvProgViewComponent
    {
        #region Fields

        private readonly AuthenticationModelFactory _authenticationModelFactory;

        #endregion

        #region Ctor

        public GAAuthenticationViewComponent(AuthenticationModelFactory authenticationModelFactory)
        {
            _authenticationModelFactory = authenticationModelFactory;
        }

        #endregion

        #region Methods

        /// <summary>
        ///  Invoke view component
        /// </summary>
        /// <param name="widgetZone">Widget zone name</param>
        /// <param name="additionalData">Additional data</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the view component result
        /// </returns>
        public async Task<IViewComponentResult> InvokeAsync(string widgetZone, object additionalData)
        {
            var model = new AuthModel();
            model = await _authenticationModelFactory.PrepareAuthModel(model);

            return View("~/Plugins/MultiFactorAuth.GoogleAuthenticator/Views/User/GAAuthentication.cshtml", model);
        }

        #endregion
    }
}
