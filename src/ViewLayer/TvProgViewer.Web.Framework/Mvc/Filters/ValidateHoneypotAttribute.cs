﻿using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Primitives;
using TvProgViewer.Core;
using TvProgViewer.Core.Domain.Security;
using TvProgViewer.Data;
using TvProgViewer.Services.Logging;

namespace TvProgViewer.Web.Framework.Mvc.Filters
{
    /// <summary>
    /// Represents a filter attribute enabling honeypot validation
    /// </summary>
    public sealed class ValidateHoneypotAttribute : TypeFilterAttribute
    {
        #region Ctor

        /// <summary>
        /// Create instance of the filter attribute
        /// </summary>
        public ValidateHoneypotAttribute() : base(typeof(ValidateHoneypotFilter))
        {
        }

        #endregion

        #region Nested filter

        /// <summary>
        /// Represents a filter enabling honeypot validation
        /// </summary>
        private class ValidateHoneypotFilter : IAsyncAuthorizationFilter
        {
            #region Fields

            private readonly ILogger _logger;
            private readonly IWebHelper _webHelper;
            private readonly SecuritySettings _securitySettings;

            #endregion

            #region Ctor

            public ValidateHoneypotFilter(ILogger logger,
                IWebHelper webHelper,
                SecuritySettings securitySettings)
            {
                _logger = logger;
                _webHelper = webHelper;
                _securitySettings = securitySettings;
            }

            #endregion

            #region Utilities

            /// <summary>
            /// Called early in the filter pipeline to confirm request is authorized
            /// </summary>
            /// <param name="context">Authorization filter context</param>
            /// <returns>Задача представляет асинхронную операцию</returns>
            private async Task ValidateHoneypotAsync(AuthorizationFilterContext context)
            {
                if (context == null)
                    throw new ArgumentNullException(nameof(context));

                if (context.HttpContext.Request == null)
                    return;

                if (!DataSettingsManager.IsDatabaseInstalled())
                    return;

                //whether honeypot is enabled
                if (!_securitySettings.HoneypotEnabled)
                    return;

                //try get honeypot input value 
                var inputValue = context.HttpContext.Request.Form[_securitySettings.HoneypotInputName];

                //if exists, bot is caught
                if (!StringValues.IsNullOrEmpty(inputValue))
                {
                    //warning admin about it
                    await _logger.WarningAsync("A bot detected. Honeypot.");

                    //and redirect to the original page
                    var page = _webHelper.GetThisPageUrl(true);
                    context.Result = new RedirectResult(page);
                }
            }

            #endregion

            #region Methods

            /// <summary>
            /// Called early in the filter pipeline to confirm request is authorized
            /// </summary>
            /// <param name="context">Authorization filter context</param>
            /// <returns>Задача представляет асинхронную операцию</returns>
            public async Task OnAuthorizationAsync(AuthorizationFilterContext context)
            {
                await ValidateHoneypotAsync(context);
            }

            #endregion
        }

        #endregion
    }
}