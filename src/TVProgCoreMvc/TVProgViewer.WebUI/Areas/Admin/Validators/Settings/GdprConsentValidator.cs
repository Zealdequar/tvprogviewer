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
        public GdprConsentValidator(IDataProvider dataProvider, ILocalizationService localizationService)
        {
            RuleFor(x => x.Message).NotEmpty().WithMessage(localizationService.GetResource("Admin.Configuration.Settings.Gdpr.Consent.Message.Required"));
            RuleFor(x => x.RequiredMessage)
                .NotEmpty()
                .WithMessage(localizationService.GetResource("Admin.Configuration.Settings.Gdpr.Consent.RequiredMessage.Required"))
                .When(x => x.IsRequired);

            SetDatabaseValidationRules<GdprConsent>(dataProvider);
        }
    }
}