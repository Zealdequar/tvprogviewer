using FluentValidation;
using TvProgViewer.Plugin.Payments.CyberSource.Models;
using TvProgViewer.Services.Localization;
using TvProgViewer.Web.Framework.Validators;

namespace TvProgViewer.Plugin.Payments.CyberSource.Validators
{
    /// <summary>
    /// Represents configuration model validator
    /// </summary>
    public class ConfigurationValidator : BaseTvProgValidator<ConfigurationModel>
    {
        #region Ctor

        public ConfigurationValidator(ILocalizationService localizationService)
        {
            RuleFor(model => model.MerchantId)
                .NotEmpty()
                .WithMessageAwait(localizationService.GetResourceAsync("Plugins.Payments.CyberSource.Fields.MerchantId.Required"))
                .When(model => !model.UseSandbox);

            RuleFor(model => model.KeyId)
                .NotEmpty()
                .WithMessageAwait(localizationService.GetResourceAsync("Plugins.Payments.CyberSource.Fields.KeyId.Required"))
                .When(model => !model.UseSandbox);

            RuleFor(model => model.SecretKey)
                .NotEmpty()
                .WithMessageAwait(localizationService.GetResourceAsync("Plugins.Payments.CyberSource.Fields.SecretKey.Required"))
                .When(model => !model.UseSandbox);

            RuleFor(model => model.ConversionDetailReportingFrequency)
                .NotEmpty()
                .GreaterThan(0)
                .WithMessageAwait(localizationService.GetResourceAsync("Plugins.Payments.CyberSource.Fields.ConversionDetailReportingFrequency.Invalid"));
        }

        #endregion
    }
}