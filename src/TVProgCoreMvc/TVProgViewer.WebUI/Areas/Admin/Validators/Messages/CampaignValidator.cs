using FluentValidation;
using TVProgViewer.WebUI.Areas.Admin.Models.Messages;
using TVProgViewer.Services.Localization;
using TVProgViewer.Web.Framework.Validators;
using TVProgViewer.Data;
using TVProgViewer.Core.Domain.Messages;

namespace TVProgViewer.WebUI.Areas.Admin.Validators.Messages
{
    public partial class CampaignValidator : BaseTvProgValidator<CampaignModel>
    {
        public CampaignValidator(ILocalizationService localizationService, ITvProgDataProvider dataProvider)
        {
            RuleFor(x => x.Name).NotEmpty().WithMessageAwait(localizationService.GetResourceAsync("Admin.Promotions.Campaigns.Fields.Name.Required"));

            RuleFor(x => x.Subject).NotEmpty().WithMessageAwait(localizationService.GetResourceAsync("Admin.Promotions.Campaigns.Fields.Subject.Required"));

            RuleFor(x => x.Body).NotEmpty().WithMessageAwait(localizationService.GetResourceAsync("Admin.Promotions.Campaigns.Fields.Body.Required"));

            SetDatabaseValidationRules<Campaign>(dataProvider);
        }
    }
}