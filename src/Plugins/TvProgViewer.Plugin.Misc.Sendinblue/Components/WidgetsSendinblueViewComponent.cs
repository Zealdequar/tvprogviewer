using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using TvProgViewer.Core;
using TvProgViewer.Services.Users;
using TvProgViewer.Web.Framework.Components;

namespace TvProgViewer.Plugin.Misc.Sendinblue.Components
{
    /// <summary>
    /// Represents view component to embed tracking script on pages
    /// </summary>
    public class WidgetsSendinblueViewComponent : TvProgViewComponent
    {
        #region Fields

        private readonly IUserService _userService;
        private readonly IWorkContext _workContext;
        private readonly SendinblueSettings _sendinblueSettings;

        #endregion

        #region Ctor

        public WidgetsSendinblueViewComponent(IUserService userService,
            IWorkContext workContext,
            SendinblueSettings sendinblueSettings)
        {
            _userService = userService;
            _workContext = workContext;
            _sendinblueSettings = sendinblueSettings;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Invoke view component
        /// </summary>
        /// <param name="widgetZone">Widget zone name</param>
        /// <param name="additionalData">Additional data</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the view component result
        /// </returns>
        public async Task<IViewComponentResult> InvokeAsync(string widgetZone, object additionalData)
        {
            var trackingScript = string.Empty;

            //ensure Marketing Automation is enabled
            if (!_sendinblueSettings.UseMarketingAutomation)
                return Content(trackingScript);

            //get user email
            var user = await _workContext.GetCurrentUserAsync();
            var userEmail = !await _userService.IsGuestAsync(user)
                ? user.Email?.Replace("'", "\\'")
                : string.Empty;

            //prepare tracking script
            trackingScript = $"{_sendinblueSettings.TrackingScript}{Environment.NewLine}"
                .Replace(SendinblueDefaults.TrackingScriptId, _sendinblueSettings.MarketingAutomationKey)
                .Replace(SendinblueDefaults.TrackingScriptUserEmail, userEmail);

            return View("~/Plugins/Misc.Sendinblue/Views/PublicInfo.cshtml", trackingScript);
        }

        #endregion
    }
}