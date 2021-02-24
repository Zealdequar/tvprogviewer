using FluentValidation;
using TVProgViewer.Core.Domain.Gdpr;
using TVProgViewer.Data;
using TVProgViewer.Services.Localization;
using TVProgViewer.WebUI.Areas.Admin.Models.Settings;
using TVProgViewer.Web.Framework.Validators;

namespace TVProgViewer.WebUI.Areas.Admin.Validators.Settings
{
    public partial class GdprConsentValidator : BaseTvProgValidator<GdprConsentModel>
    {
        public GdprConsentValidator(ILocalizationService localizationService, ITvProgDataProvider dataProvider)
        {
            RuleFor(x => x.Message).NotEmpty().WithMessageAwait(localizationService.GetResourceAsync("Admin.Configuration.Settings.Gdpr.Consent.Message.Required"));
            RuleFor(x => x.RequiredMessage)
                .NotEmpty()
                .WithMessageAwait(localizationService.GetResourceAsync("Admin.Configuration.Settings.Gdpr.Consent.RequiredMessage.Required"))
                .When(x => x.IsRequired);

            SetDatabaseValidationRules<GdprConsent>(dataProvider);
        }
    }
}