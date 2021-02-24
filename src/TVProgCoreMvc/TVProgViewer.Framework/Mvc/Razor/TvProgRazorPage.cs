﻿using System.Threading.Tasks;
using TVProgViewer.Core;
using TVProgViewer.Core.Infrastructure;
using TVProgViewer.Services.Localization;
using TVProgViewer.Services.Themes;
using TVProgViewer.Web.Framework.Localization;
using TVProgViewer.Web.Framework.Themes;

namespace TVProgViewer.Web.Framework.Mvc.Razor
{
    /// <summary>
    /// Web view page
    /// </summary>
    /// <typeparam name="TModel">Model</typeparam>
    public abstract class TvProgRazorPage<TModel> : Microsoft.AspNetCore.Mvc.Razor.RazorPage<TModel>
    {
        private ILocalizationService _localizationService;
        private Localizer _localizer;

        /// <summary>
        /// Get a localized resources
        /// </summary>
        public Localizer T
        {
            get
            {
                if (_localizationService == null)
                    _localizationService = EngineContext.Current.Resolve<ILocalizationService>();

                if (_localizer == null)
                {
                    _localizer = (format, args) =>
                    {
                        var resFormat = _localizationService.GetResourceAsync(format).Result;
                        if (string.IsNullOrEmpty(resFormat))
                        {
                            return new LocalizedString(format);
                        }
                        return new LocalizedString((args == null || args.Length == 0)
                            ? resFormat
                            : string.Format(resFormat, args));
                    };
                }
                return _localizer;
            }
        }

        /// <summary>
        /// Return a value indicating whether the working language and theme support RTL (right-to-left)
        /// </summary>
        /// <returns></returns>
        public async Task<bool> ShouldUseRtlThemeAsync()
        {
            var workContext = EngineContext.Current.Resolve<IWorkContext>();
            var supportRtl = (await workContext.GetWorkingLanguageAsync()).Rtl;
            if (supportRtl)
            {
                //ensure that the active theme also supports it
                var themeProvider = EngineContext.Current.Resolve<IThemeProvider>();
                var themeContext = EngineContext.Current.Resolve<IThemeContext>();
                supportRtl = (await themeProvider.GetThemeBySystemNameAsync(await themeContext.GetWorkingThemeNameAsync()))?.SupportRtl ?? false;
            }

            return supportRtl;
        }
    }

    /// <summary>
    /// Web view page
    /// </summary>
    public abstract class TvProgRazorPage : TvProgRazorPage<dynamic>
    {
    }
}