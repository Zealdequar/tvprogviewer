using FluentValidation;
using TvProgViewer.Plugin.Payments.PayPalViewer.Models;
using TvProgViewer.Services.Localization;
using TvProgViewer.Web.Framework.Validators;

namespace TvProgViewer.Plugin.Payments.PayPalViewer.Validators
{
    /// <summary>
    /// Represents onboarding model validator
    /// </summary>
    public class OnboardingValidator : BaseTvProgValidator<OnboardingModel>
    {
        #region Ctor

        public OnboardingValidator(ILocalizationService localizationService)
        {
            RuleFor(model => model.Email)
                .NotEmpty()
                .EmailAddress()
                .WithMessageAwait(localizationService.GetResourceAsync("Admin.Common.WrongEmail"));
        }

        #endregion
    }
}