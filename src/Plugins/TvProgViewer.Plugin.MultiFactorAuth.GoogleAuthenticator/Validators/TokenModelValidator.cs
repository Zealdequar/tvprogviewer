using FluentValidation;
using TvProgViewer.Plugin.MultiFactorAuth.GoogleAuthenticator.Models;
using TvProgViewer.Services.Localization;
using TvProgViewer.Web.Framework.Validators;

namespace TvProgViewer.Plugin.MultiFactorAuth.GoogleAuthenticator.Validators
{
    /// <summary>
    /// Represents an <see cref="TokenModel"/> validator.
    /// </summary>
    public class TokenModelValidator : BaseTvProgValidator<TokenModel>
    {
        public TokenModelValidator(ILocalizationService localizationService)
        {
            RuleFor(model => model.Token).NotEmpty()
                .WithMessageAwait(localizationService.GetResourceAsync("Plugins.MultiFactorAuth.GoogleAuthenticator.Fields.Code.Required"));
            RuleFor(model => model.Token).Matches(@"^[0-9]{6,6}$")
                .WithMessageAwait(localizationService.GetResourceAsync("Plugins.MultiFactorAuth.GoogleAuthenticator.Fields.Code.Wrong"));
        }
    }
}
