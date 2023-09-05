using FluentValidation;
using TvProgViewer.Plugin.Widgets.FacebookPixel.Models;
using TvProgViewer.Services.Localization;
using TvProgViewer.Web.Framework.Validators;

namespace TvProgViewer.Plugin.Widgets.FacebookPixel.Validators
{
    /// <summary>
    /// Represents configuration model validator
    /// </summary>
    public class ConfigurationValidator : BaseTvProgValidator<FacebookPixelModel>
    {
        #region Ctor

        public ConfigurationValidator(ILocalizationService localizationService)
        {
            //set validation rules
            RuleFor(model => model.PixelId)
                .NotEmpty()
                .WithMessageAwait(localizationService.GetResourceAsync("Plugins.Widgets.FacebookPixel.Configuration.Fields.PixelId.Required"));
            RuleFor(model => model.AccessToken)
                .NotEmpty()
                .WithMessageAwait(localizationService.GetResourceAsync("Plugins.Widgets.FacebookPixel.Configuration.Fields.AccessToken.Required"))
                .When(model => model.ConversionsApiEnabled);
            RuleFor(model => model.UseAdvancedMatching)
                .NotEqual(true)
                .WithMessageAwait(localizationService.GetResourceAsync("Plugins.Widgets.FacebookPixel.Configuration.Fields.UseAdvancedMatching.Forbidden"))
                .When(model => model.PassUserProperties);
            RuleFor(model => model.PassUserProperties)
                .NotEqual(true)
                .WithMessageAwait(localizationService.GetResourceAsync("Plugins.Widgets.FacebookPixel.Configuration.Fields.PassUserProperties.Forbidden"))
                .When(model => model.UseAdvancedMatching);
        }

        #endregion
    }
}