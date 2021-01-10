using FluentValidation;
using TVProgViewer.Services.Localization;
using TVProgViewer.Web.Framework.Validators;
using TVProgViewer.WebUI.Models.Boards;

namespace TVProgViewer.WebUI.Validators.Boards
{
    public partial class EditForumTopicValidator : BaseTvProgValidator<EditForumTopicModel>
    {
        public EditForumTopicValidator(ILocalizationService localizationService)
        {
            RuleFor(x => x.Subject).NotEmpty().WithMessage(localizationService.GetResource("Forum.TopicSubjectCannotBeEmpty"));
            RuleFor(x => x.Text).NotEmpty().WithMessage(localizationService.GetResource("Forum.TextCannotBeEmpty"));
        }
    }
}