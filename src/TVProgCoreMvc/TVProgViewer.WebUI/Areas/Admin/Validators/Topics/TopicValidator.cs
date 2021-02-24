using FluentValidation;
using TVProgViewer.WebUI.Areas.Admin.Models.Topics;
using TVProgViewer.Core.Domain.Topics;
using TVProgViewer.Data;
using TVProgViewer.Services.Localization;
using TVProgViewer.Web.Framework.Validators;
using TVProgViewer.Services.Seo;

namespace TVProgViewer.WebUI.Areas.Admin.Validators.Topics
{
    public partial class TopicValidator : BaseTvProgValidator<TopicModel>
    {
        public TopicValidator(ILocalizationService localizationService, ITvProgDataProvider dataProvider)
        {
            RuleFor(x => x.SeName)
                .Length(0, TvProgSeoDefaults.ForumTopicLength)
                .WithMessageAwait(localizationService.GetResourceAsync("Admin.SEO.SeName.MaxLengthValidation"), TvProgSeoDefaults.ForumTopicLength);

            RuleFor(x => x.Password)
                .NotEmpty()
                .When(x => x.IsPasswordProtected)
                .WithMessageAwait(localizationService.GetResourceAsync("Validation.Password.IsNotEmpty"));

            SetDatabaseValidationRules<Topic>(dataProvider);
        }
    }
}
