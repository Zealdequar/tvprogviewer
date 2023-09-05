using Microsoft.AspNetCore.Mvc;
using TvProgViewer.Plugin.MultiFactorAuth.GoogleAuthenticator.Models;
using TvProgViewer.Web.Framework.Components;

namespace TvProgViewer.Plugin.MultiFactorAuth.GoogleAuthenticator.Components
{
    /// <summary>
    /// Represents view component for verification GoogleAuthenticator
    /// </summary>
    public class GAVerificationViewComponent : TvProgViewComponent
    {
        #region Fields

        #endregion

        #region Ctor

        public GAVerificationViewComponent()
        {

        }

        #endregion

        #region Methods

        /// <summary>
        ///  Invoke view component
        /// </summary>
        /// <param name="widgetZone">Widget zone name</param>
        /// <param name="additionalData">Additional data</param>
        /// <returns>View component result</returns>
        public IViewComponentResult Invoke(string widgetZone, object additionalData)
        {
            var model = new TokenModel();
            return View("~/Plugins/MultiFactorAuth.GoogleAuthenticator/Views/User/GAVefification.cshtml", model);
        }

        #endregion
    }
}
