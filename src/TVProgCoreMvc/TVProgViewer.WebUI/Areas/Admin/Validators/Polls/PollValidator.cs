using FluentValidation;
using TVProgViewer.WebUI.Areas.Admin.Models.Polls;
using TVProgViewer.Services.Localization;
using TVProgViewer.Web.Framework.Validators;
using TVProgViewer.Data;
using TVProgViewer.Core.Domain.Polls;

namespace TVProgViewer.WebUI.Areas.Admin.Validators.Polls
{
    public partial class PollValidator : BaseTvProgValidator<PollModel>
    {
        public PollValidator(ILocalizationService localizationService, ITvProgDataProvider dataProvider)
        {
            RuleFor(x => x.Name).NotEmpty().WithMessageAwait(localizationService.GetResourceAsync("Admin.ContentManagement.Polls.Fields.Name.Required"));

            SetDatabaseValidationRules<Poll>(dataProvider);
        }
    }
}