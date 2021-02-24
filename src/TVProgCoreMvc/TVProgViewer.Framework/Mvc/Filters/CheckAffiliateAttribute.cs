using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using TVProgViewer.Core;
using TVProgViewer.Core.Domain.Affiliates;
using TVProgViewer.Core.Domain.Users;
using TVProgViewer.Data;
using TVProgViewer.Services.Affiliates;
using TVProgViewer.Services.Users;

namespace TVProgViewer.Web.Framework.Mvc.Filters
{
    /// <summary>
    /// Represents filter attribute that checks and updates affiliate of User
    /// </summary>
    public sealed class CheckAffiliateAttribute : TypeFilterAttribute
    {
        #region Ctor

        /// <summary>
        /// Create instance of the filter attribute
        /// </summary>
        public CheckAffiliateAttribute() : base(typeof(CheckAffiliateFilter))
        {
        }

        #endregion

        #region Nested filter

        /// <summary>
        /// Represents a filter that checks and updates affiliate of user
        /// </summary>
        private class CheckAffiliateFilter : IAsyncActionFilter
        {
            #region Constants

            private const string AFFILIATE_ID_QUERY_PARAMETER_NAME = "affiliateid";
            private const string AFFILIATE_FRIENDLYURLNAME_QUERY_PARAMETER_NAME = "affiliate";

            #endregion

            #region Fields

            private readonly IAffiliateService _affiliateService;
            private readonly IUserService _userService;
            private readonly IWorkContext _workContext;

            #endregion

            #region Ctor

            public CheckAffiliateFilter(IAffiliateService affiliateService,
                IUserService userService,
                IWorkContext workContext)
            {
                _affiliateService = affiliateService;
                _userService = userService;
                _workContext = workContext;
            }

            #endregion

            #region Utilities

            /// <summary>
            /// Set the affiliate identifier of current user
            /// </summary>
            /// <param name="affiliate">Affiliate</param>
            /// <param name="user">User</param>
            private async Task SetUserAffiliateIdAsync(Affiliate affiliate, User user)
            {
                if (affiliate == null || affiliate.Deleted || !affiliate.Active)
                    return;

                if (affiliate.Id == user.AffiliateId)
                    return;

                //ignore search engines
                if (user.IsSearchEngineAccount())
                    return;

                //update affiliate identifier
                user.AffiliateId = affiliate.Id;
                await _userService.UpdateUserAsync(user);
            }

            /// <summary>
            /// Called asynchronously before the action, after model binding is complete.
            /// </summary>
            /// <param name="context">A context for action filters</param>
            /// <returns>A task that on completion indicates the necessary filter actions have been executed</returns>
            private async Task CheckAffiliateAsync(ActionExecutingContext context)
            {
                if (context == null)
                    throw new ArgumentNullException(nameof(context));

                //check request query parameters
                var request = context.HttpContext.Request;
                if (request?.Query == null || !request.Query.Any())
                    return;

                if (!await DataSettingsManager.IsDatabaseInstalledAsync())
                    return;

                //try to find by ID
                var user = await _workContext.GetCurrentUserAsync();
                var affiliateIds = request.Query[AFFILIATE_ID_QUERY_PARAMETER_NAME];

                if (int.TryParse(affiliateIds.FirstOrDefault(), out var affiliateId) && affiliateId > 0 && affiliateId != user.AffiliateId)
                {
                    var affiliate = await _affiliateService.GetAffiliateByIdAsync(affiliateId);
                    await SetUserAffiliateIdAsync(affiliate, user);
                    return;
                }

                //try to find by friendly name
                var affiliateNames = request.Query[AFFILIATE_FRIENDLYURLNAME_QUERY_PARAMETER_NAME];
                var affiliateName = affiliateNames.FirstOrDefault();
                if (!string.IsNullOrEmpty(affiliateName))
                {
                    var affiliate = await _affiliateService.GetAffiliateByFriendlyUrlNameAsync(affiliateName);
                    await SetUserAffiliateIdAsync(affiliate, user);
                }
            }

            #endregion

            #region Methods

            /// <summary>
            /// Called asynchronously before the action, after model binding is complete.
            /// </summary>
            /// <param name="context">A context for action filters</param>
            /// <param name="next">A delegate invoked to execute the next action filter or the action itself</param>
            /// <returns>A task that on completion indicates the filter has executed</returns>
            public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
            {
                await CheckAffiliateAsync(context);
                if (context.Result == null)
                    await next();
            }

            #endregion
        }

        #endregion
    }
}

