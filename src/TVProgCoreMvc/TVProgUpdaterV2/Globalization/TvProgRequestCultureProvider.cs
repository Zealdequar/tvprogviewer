using System;
using System.Globalization;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Localization;
using TVProgViewer.Core;
using TVProgViewer.Core.Infrastructure;
using TVProgViewer.Data;

namespace TVProgViewer.TVProgUpdaterV2.Globalization
{
    /// <summary>
    /// Represents middleware that set current culture based on request
    /// </summary>
    public class TvProgRequestCultureProvider : RequestCultureProvider
    {
        public TvProgRequestCultureProvider(RequestLocalizationOptions options)
        {
            Options = options;
        }

        public override async Task<ProviderCultureResult> DetermineProviderCultureResult(HttpContext httpContext)
        {
            //set working language culture
            var culture = (await EngineContext.Current.Resolve<IWorkContext>().GetWorkingLanguageAsync()).LanguageCulture;

            return new ProviderCultureResult(culture, culture);
        }
    }
}
