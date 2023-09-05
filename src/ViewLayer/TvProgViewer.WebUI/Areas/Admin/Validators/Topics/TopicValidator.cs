using FluentValidation;
using TvProgViewer.Core.Domain.Topics;
using TvProgViewer.Data.Mapping;
using TvProgViewer.Services.Localization;
using TvProgViewer.Services.Seo;
using TvProgViewer.WebUI.Areas.Admin.Models.Topics;
using TvProgViewer.Web.Framework.Validators;

namespace TvProgViewer.WebUI.Areas.Admin.Validators.Topics
{
    public partial class TopicValidator : BaseTvProgValidator<TopicModel>
    {
        public TopicValidator(ILocalizationService localizationService, IMappingEntityAccessor mappingEntityAccessor)
        {
            RuleFor(x => x.SeName)
                .Length(0, TvProgSeoDefaults.ForumTopicLength)
                .WithMessageAwait(localizationService.GetResourceAsync("Admin.SEO.SeName.MaxLengthValidation"), TvProgSeoDefaults.ForumTopicLength);

            RuleFor(x => x.Password)
                .NotEmpty()
                .When(x => x.IsPasswordProtected)
                .WithMessageAwait(localizationService.GetResourceAsync("Validation.Password.IsNotEmpty"));

            SetDatabaseValidationRules<Topic>(mappingEntityAccessor);
        }
    }
}
