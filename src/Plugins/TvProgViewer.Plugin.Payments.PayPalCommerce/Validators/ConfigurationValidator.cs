using FluentValidation;
using TvProgViewer.Plugin.Payments.PayPalViewer.Models;
using TvProgViewer.Services.Localization;
using TvProgViewer.Web.Framework.Validators;

namespace TvProgViewer.Plugin.Payments.PayPalViewer.Validators
{
    /// <summary>
    /// Represents configuration model validator
    /// </summary>
    public class ConfigurationValidator : BaseTvProgValidator<ConfigurationModel>
    {
        #region Ctor

        public ConfigurationValidator(ILocalizationService localizationService)
        {
            RuleFor(model => model.ClientId)
                .NotEmpty()
                .WithMessageAwait(localizationService.GetResourceAsync("Plugins.Payments.PayPalViewer.Fields.ClientId.Required"))
                .When(model => !model.UseSandbox && model.SetCredentialsManually);

            RuleFor(model => model.SecretKey)
                .NotEmpty()
                .WithMessageAwait(localizationService.GetResourceAsync("Plugins.Payments.PayPalViewer.Fields.SecretKey.Required"))
                .When(model => !model.UseSandbox && model.SetCredentialsManually);
        }

        #endregion
    }
}