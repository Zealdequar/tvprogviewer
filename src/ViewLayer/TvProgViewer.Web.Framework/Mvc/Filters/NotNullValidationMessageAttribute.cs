using System;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using TvProgViewer.Data;
using TvProgViewer.Services.Localization;
using TvProgViewer.Web.Framework.Models;
using TvProgViewer.Web.Framework.Mvc.ModelBinding;
using TvProgViewer.Web.Framework.Validators;

namespace TvProgViewer.Web.Framework.Mvc.Filters
{
    /// <summary>
    /// Represents filter attribute that adds a detailed validation message that a value cannot be empty
    /// </summary>
    public sealed class NotNullValidationMessageAttribute : TypeFilterAttribute
    {
        #region Ctor

        /// <summary>
        /// Create instance of the filter attribute
        /// </summary>
        public NotNullValidationMessageAttribute() : base(typeof(NotNullValidationMessageFilter))
        {
        }

        #endregion

        #region Nested filter

        /// <summary>
        /// Represents a filter that adds a detailed validation message that a value cannot be empty
        /// </summary>
        private class NotNullValidationMessageFilter : IAsyncActionFilter
        {
            #region Fields

            private readonly ILocalizationService _localizationService;

            #endregion

            #region Ctor

            public NotNullValidationMessageFilter(ILocalizationService localizationService)
            {
                _localizationService = localizationService;
            }

            #endregion

            #region Utilities

            /// <summary>
            /// Called asynchronously before the action, after model binding is complete.
            /// </summary>
            /// <param name="context">A context for action filters</param>
            /// <returns>Задача представляет асинхронную операцию</returns>
            private async Task CheckNotNullValidationAsync(ActionExecutingContext context)
            {
                if (context == null)
                    throw new ArgumentNullException(nameof(context));

                if (context.HttpContext.Request == null)
                    return;

                //only in POST requests
                if (!context.HttpContext.Request.Method.Equals(WebRequestMethods.Http.Post, StringComparison.InvariantCultureIgnoreCase))
                    return;

                if (!DataSettingsManager.IsDatabaseInstalled())
                    return;

                //whether the model state is invalid
                if (context.ModelState.ErrorCount == 0)
                    return;

                var nullModelValues = context.ModelState
                    .Where(modelState => modelState.Value.ValidationState == ModelValidationState.Invalid
                        && modelState.Value.Errors.Any(error => error.ErrorMessage.Equals(TvProgValidationDefaults.NotNullValidationLocaleName)))
                    .ToList();
                if (!nullModelValues.Any())
                    return;

                //get model passed to the action
                var model = context.ActionArguments.Values.OfType<BaseTvProgModel>().FirstOrDefault();
                if (model is null)
                    return;

                //get model properties that failed validation
                var properties = model.GetType().GetProperties();
                var locale = await _localizationService.GetResourceAsync(TvProgValidationDefaults.NotNullValidationLocaleName);
                foreach (var modelState in nullModelValues)
                {
                    var property = properties
                        .FirstOrDefault(propertyInfo => propertyInfo.Name.Equals(modelState.Key, StringComparison.InvariantCultureIgnoreCase));
                    if (property is null)
                        continue;

                    var displayName = property
                        .GetCustomAttributes(typeof(TvProgResourceDisplayNameAttribute), true)
                        .OfType<TvProgResourceDisplayNameAttribute>()
                        .FirstOrDefault()
                        ?.DisplayName
                        ?? property.Name;

                    //set localized error message
                    modelState.Value.Errors.Clear();
                    modelState.Value.Errors.Add(string.Format(locale, displayName));
                }
            }

            #endregion

            #region Methods

            /// <summary>
            /// Called asynchronously before the action, after model binding is complete.
            /// </summary>
            /// <param name="context">A context for action filters</param>
            /// <param name="next">A delegate invoked to execute the next action filter or the action itself</param>
            /// <returns>Задача представляет асинхронную операцию</returns>
            public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
            {
                await CheckNotNullValidationAsync(context);
                if (context.Result == null)
                    await next();
            }

            #endregion
        }

        #endregion
    }
}