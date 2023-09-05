using FluentValidation;
using TvProgViewer.Plugin.Widgets.AccessiBe.Models;
using TvProgViewer.Services.Localization;
using TvProgViewer.Web.Framework.Validators;

namespace TvProgViewer.Plugin.Widgets.AccessiBe.Validators
{
    /// <summary>
    /// Represents configuration model validator
    /// </summary>
    public class ConfigurationValidator : BaseTvProgValidator<ConfigurationModel>
    {
        #region Ctor

        public ConfigurationValidator(ILocalizationService localizationService)
        {
            RuleFor(model => model.Script)
                .NotEmpty()
                .WithMessageAwait(localizationService.GetResourceAsync("Plugins.Widgets.AccessiBe.Fields.Script.Required"))
                .When(model => model.Enabled);
        }

        #endregion
    }
}