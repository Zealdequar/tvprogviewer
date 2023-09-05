﻿using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using TvProgViewer.Core;
using TvProgViewer.Data;
using TvProgViewer.Services.Users;

namespace TvProgViewer.TvProgUpdaterV3.Mvc.Filters
{
    /// <summary>
    /// Represents a filter attribute confirming that user with "Vendor" user role has appropriate vendor account associated (and active)
    /// </summary>
    public sealed class ValidateVendorAttribute : TypeFilterAttribute
    {
        #region Ctor

        /// <summary>
        /// Create instance of the filter attribute
        /// </summary>
        /// <param name="ignore">Whether to ignore the execution of filter actions</param>
        public ValidateVendorAttribute(bool ignore = false) : base(typeof(ValidateVendorFilter))
        {
            IgnoreFilter = ignore;
            Arguments = new object[] { ignore };
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets a value indicating whether to ignore the execution of filter actions
        /// </summary>
        public bool IgnoreFilter { get; }

        #endregion

        #region Nested filter

        /// <summary>
        /// Represents a filter confirming that user with "Vendor" user role has appropriate vendor account associated (and active)
        /// </summary>
        private class ValidateVendorFilter : IAsyncAuthorizationFilter
        {
            #region Fields

            private readonly bool _ignoreFilter;
            private readonly IUserService _userService;
            private readonly IWorkContext _workContext;

            #endregion

            #region Ctor

            public ValidateVendorFilter(bool ignoreFilter, IWorkContext workContext, IUserService userService)
            {
                _ignoreFilter = ignoreFilter;
                _userService = userService;
                _workContext = workContext;
            }

            #endregion

            #region Utilities

            /// <summary>
            /// Called early in the filter pipeline to confirm request is authorized
            /// </summary>
            /// <param name="context">Authorization filter context</param>
            /// <returns>A task that represents the asynchronous operation</returns>
            private async Task ValidateVendorAsync(AuthorizationFilterContext context)
            {
                if (context == null)
                    throw new ArgumentNullException(nameof(context));

                if (!DataSettingsManager.IsDatabaseInstalled())
                    return;

                //check whether this filter has been overridden for the Action
                var actionFilter = context.ActionDescriptor.FilterDescriptors
                    .Where(filterDescriptor => filterDescriptor.Scope == FilterScope.Action)
                    .Select(filterDescriptor => filterDescriptor.Filter)
                    .OfType<ValidateVendorAttribute>()
                    .FirstOrDefault();

                //ignore filter (the action is available even if the current user isn't a vendor)
                if (actionFilter?.IgnoreFilter ?? _ignoreFilter)
                    return;

                //whether current user is vendor
                var user = await _workContext.GetCurrentUserAsync();
                if (!await _userService.IsVendorAsync(user))
                    return;

                //ensure that this user has active vendor record associated
                var vendor = await _workContext.GetCurrentVendorAsync();
                if (vendor == null)
                    context.Result = new ChallengeResult();
            }

            #endregion

            #region Methods

            /// <summary>
            /// Called early in the filter pipeline to confirm request is authorized
            /// </summary>
            /// <param name="context">Authorization filter context</param>
            /// <returns>A task that represents the asynchronous operation</returns>
            public async Task OnAuthorizationAsync(AuthorizationFilterContext context)
            {
                await ValidateVendorAsync(context);
            }

            #endregion
        }

        #endregion
    }
}