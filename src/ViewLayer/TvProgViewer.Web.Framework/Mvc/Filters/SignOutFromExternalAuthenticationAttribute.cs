﻿using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using TvProgViewer.Services.Authentication;

namespace TvProgViewer.Web.Framework.Mvc.Filters
{
    /// <summary>
    /// Represents filter attribute that sign out from the external authentication scheme
    /// </summary>
    public sealed class SignOutFromExternalAuthenticationAttribute : TypeFilterAttribute
    {
        #region Ctor

        /// <summary>
        /// Create instance of the filter attribute
        /// </summary>
        public SignOutFromExternalAuthenticationAttribute() : base(typeof(SignOutFromExternalAuthenticationFilter))
        {
        }

        #endregion

        #region Nested filter

        /// <summary>
        /// Represents a filter that sign out from the external authentication scheme
        /// </summary>
        private class SignOutFromExternalAuthenticationFilter : IAsyncAuthorizationFilter
        {
            #region Utilities

            /// <summary>
            /// Called early in the filter pipeline to confirm request is authorized
            /// </summary>
            /// <param name="context">Authorization filter context</param>
            /// <returns>Задача представляет асинхронную операцию</returns>
            private async Task SignOutFromExternalAuthenticationAsync(AuthorizationFilterContext context)
            {
                if (context == null)
                    throw new ArgumentNullException(nameof(context));

                //sign out from the external authentication scheme
                var authenticateResult = await context.HttpContext.AuthenticateAsync(TvProgAuthenticationDefaults.ExternalAuthenticationScheme);
                if (authenticateResult.Succeeded)
                    await context.HttpContext.SignOutAsync(TvProgAuthenticationDefaults.ExternalAuthenticationScheme);
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
                await SignOutFromExternalAuthenticationAsync(context);
            }

            #endregion
        }

        #endregion
    }
}