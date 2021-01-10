using System;
using System.Net;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using TVProgViewer.Core;
using TVProgViewer.Core.Domain.Users;
using TVProgViewer.Data;
using TVProgViewer.Services.Users;

namespace TVProgViewer.Web.Framework.Mvc.Filters
{
    /// <summary>
    /// Represents filter attribute that saves last IP address of User
    /// </summary>
    public sealed class SaveIpAddressAttribute : TypeFilterAttribute
    {
        #region Ctor

        /// <summary>
        /// Create instance of the filter attribute
        /// </summary>
        public SaveIpAddressAttribute() : base(typeof(SaveIpAddressFilter))
        {
        }

        #endregion

        #region Nested filter

        /// <summary>
        /// Represents a filter that saves last IP address of User
        /// </summary>
        private class SaveIpAddressFilter : IActionFilter
        {
            #region Fields

            private readonly IUserService _UserService;
            private readonly IWebHelper _webHelper;
            private readonly IWorkContext _workContext;
            private readonly UserSettings _UserSettings;

            #endregion

            #region Ctor

            public SaveIpAddressFilter(IUserService UserService,
                IWebHelper webHelper,
                IWorkContext workContext,
                UserSettings UserSettings)
            {
                _UserService = UserService;
                _webHelper = webHelper;
                _workContext = workContext;
                _UserSettings = UserSettings;
            }

            #endregion

            #region Methods

            /// <summary>
            /// Called before the action executes, after model binding is complete
            /// </summary>
            /// <param name="context">A context for action filters</param>
            public void OnActionExecuting(ActionExecutingContext context)
            {
                if (context == null)
                    throw new ArgumentNullException(nameof(context));

                if (context.HttpContext.Request == null)
                    return;

                //only in GET requests
                if (!context.HttpContext.Request.Method.Equals(WebRequestMethods.Http.Get, StringComparison.InvariantCultureIgnoreCase))
                    return;

                if (!DataSettingsManager.IsDatabaseInstalled())
                    return;

                //check whether we store IP addresses
                if (!_UserSettings.StoreIpAddresses)
                    return;

                //get current IP address
                var currentIpAddress = _webHelper.GetCurrentIpAddress();
                if (string.IsNullOrEmpty(currentIpAddress))
                    return;

                //update User's IP address
                if (_workContext.OriginalUserIfImpersonated == null &&
                     !currentIpAddress.Equals(_workContext.CurrentUser.LastIpAddress, StringComparison.InvariantCultureIgnoreCase))
                {
                    _workContext.CurrentUser.LastIpAddress = currentIpAddress;
                    _UserService.UpdateUser(_workContext.CurrentUser);
                }
            }

            /// <summary>
            /// Called after the action executes, before the action result
            /// </summary>
            /// <param name="context">A context for action filters</param>
            public void OnActionExecuted(ActionExecutedContext context)
            {
                //do nothing
            }

            #endregion
        }

        #endregion
    }
}