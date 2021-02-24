using FluentValidation;
using TVProgViewer.Services.Localization;
using TVProgViewer.WebUI.Areas.Admin.Models.Polls;
using TVProgViewer.Web.Framework.Validators;
using TVProgViewer.Data;
using TVProgViewer.Core.Domain.Polls;

namespace TVProgViewer.WebUI.Areas.Admin.Validators.Polls
{
    public partial class PollAnswerValidator : BaseTvProgValidator<PollAnswerModel>
    {
        public PollAnswerValidator(ILocalizationService localizationService, ITvProgDataProvider dataProvider)
        {
            //if validation without this set rule is applied, in this case nothing will be validated
            //it's used to prevent auto-validation of child models
            RuleSet(TvProgValidatorDefaults.ValidationRuleSet, () =>
            {
                RuleFor(model => model.Name)
                    .NotEmpty()
                    .WithMessageAwait(localizationService.GetResourceAsync("Admin.ContentManagement.Polls.Answers.Fields.Name.Required"));

                SetDatabaseValidationRules<PollAnswer>(dataProvider);
            });
        }
    }
}