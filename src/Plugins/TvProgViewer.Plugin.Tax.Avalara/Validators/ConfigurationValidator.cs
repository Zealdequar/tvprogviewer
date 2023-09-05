using FluentValidation;
using TvProgViewer.Plugin.Tax.Avalara.Models.Configuration;
using TvProgViewer.Services.Localization;
using TvProgViewer.Web.Framework.Validators;

namespace TvProgViewer.Plugin.Tax.Avalara.Validators
{
    /// <summary>
    /// Represents configuration model validator
    /// </summary>
    public class ConfigurationValidator : BaseTvProgValidator<ConfigurationModel>
    {
        #region Ctor

        public ConfigurationValidator(ILocalizationService localizationService)
        {
            RuleFor(model => model.AccountId)
                .NotEmpty()
                .WithMessageAwait(localizationService.GetResourceAsync("Plugins.Tax.Avalara.Fields.AccountId.Required"))
                .When(model => !model.UseSandbox);
            RuleFor(model => model.LicenseKey)
                .NotEmpty()
                .WithMessageAwait(localizationService.GetResourceAsync("Plugins.Tax.Avalara.Fields.LicenseKey.Required"))
                .When(model => !model.UseSandbox);
        }

        #endregion
    }
}