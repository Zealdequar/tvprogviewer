using FluentValidation;
using TvProgViewer.Plugin.Misc.Zettle.Models;
using TvProgViewer.Services.Localization;
using TvProgViewer.Web.Framework.Validators;

namespace TvProgViewer.Plugin.Misc.Zettle.Validators
{
    /// <summary>
    /// Represents configuration model validator
    /// </summary>
    public class ConfigurationValidator : BaseTvProgValidator<ConfigurationModel>
    {
        #region Ctor

        public ConfigurationValidator(ILocalizationService localizationService)
        {
            RuleFor(model => model.ApiKey)
                .NotEmpty()
                .WithMessageAwait(localizationService.GetResourceAsync("Plugins.Misc.Zettle.Configuration.Fields.ApiKey.Required"))
                .When(model => !string.IsNullOrEmpty(model.ClientId));

            RuleFor(model => model.AutoSyncPeriod)
                .NotEmpty()
                .GreaterThan(0)
                .WithMessageAwait(localizationService.GetResourceAsync("Plugins.Misc.Zettle.Configuration.Fields.AutoSyncPeriod.Invalid"))
                .When(model => model.AutoSyncEnabled);
        }

        #endregion
    }
}