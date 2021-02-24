using System.Linq;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using TVProgViewer.Core.Domain.Localization;
using TVProgViewer.Data;
using TVProgViewer.Services.Localization;

namespace TVProgViewer.WebUI.Infrastructure
{
    public class BaseRouteProvider
    {
        protected string GetRouterPattern(IEndpointRouteBuilder endpointRouteBuilder, string seoCode = "")
        {
            if (DataSettingsManager.IsDatabaseInstalled())
            {
                var localizationSettings = endpointRouteBuilder.ServiceProvider.GetRequiredService<LocalizationSettings>();
                if (localizationSettings.SeoFriendlyUrlsForLanguagesEnabled)
                {
                    var langservice = endpointRouteBuilder.ServiceProvider.GetRequiredService<ILanguageService>();
                    var languages = langservice.GetAllLanguagesAsync().Result;
                    return "{language:lang=" + languages.FirstOrDefault().UniqueSeoCode + $"}}/{seoCode}";
                }
            }
            return seoCode;
        }
    }
}
