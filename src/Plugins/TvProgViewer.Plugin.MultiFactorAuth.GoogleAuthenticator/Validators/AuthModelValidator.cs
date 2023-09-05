using FluentValidation;
using TvProgViewer.Plugin.MultiFactorAuth.GoogleAuthenticator.Models;
using TvProgViewer.Services.Localization;
using TvProgViewer.Web.Framework.Validators;

namespace TvProgViewer.Plugin.MultiFactorAuth.GoogleAuthenticator.Validators
{
    /// <summary>
    /// Represents an <see cref="AuthModel"/> validator.
    /// </summary>
    public class AuthModelValidator : BaseTvProgValidator<AuthModel>
    {
        public AuthModelValidator(ILocalizationService localizationService)
        {
            RuleFor(model => model.Code).NotEmpty()
                .WithMessageAwait(localizationService.GetResourceAsync("Plugins.MultiFactorAuth.GoogleAuthenticator.Fields.Code.Required"));
            RuleFor(model => model.Code).Matches(@"^[0-9]{6,6}$")
                .WithMessageAwait(localizationService.GetResourceAsync("Plugins.MultiFactorAuth.GoogleAuthenticator.Fields.Code.Wrong"));
        }
    }
}
