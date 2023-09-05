using Microsoft.AspNetCore.Mvc;
using TvProgViewer.Web.Framework.Components;

namespace TvProgViewer.Plugin.ExternalAuth.Facebook.Components
{
    /// <summary>
    /// Represents view component to display login button
    /// </summary>
    public class FacebookAuthenticationViewComponent : TvProgViewComponent
    {
        /// <summary>
        /// Invoke view component
        /// </summary>
        /// <param name="widgetZone">Widget zone name</param>
        /// <param name="additionalData">Additional data</param>
        /// <returns>View component result</returns>
        public IViewComponentResult Invoke(string widgetZone, object additionalData)
        {
            return View("~/Plugins/ExternalAuth.Facebook/Views/PublicInfo.cshtml");
        }
    }
}