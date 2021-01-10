using FluentValidation;
using TVProgViewer.WebUI.Areas.Admin.Models.Topics;
using TVProgViewer.Core.Domain.Topics;
using TVProgViewer.Data;
using TVProgViewer.Services.Defaults;
using TVProgViewer.Services.Localization;
using TVProgViewer.Web.Framework.Validators;

namespace TVProgViewer.WebUI.Areas.Admin.Validators.Topics
{
    public partial class TopicValidator : BaseTvProgValidator<TopicModel>
    {
        public TopicValidator(IDataProvider dataProvider, ILocalizationService localizationService)
        {
            RuleFor(x => x.SeName).Length(0, TvProgSeoDefaults.ForumTopicLength)
                .WithMessage(string.Format(localizationService.GetResource("Admin.SEO.SeName.MaxLengthValidation"), TvProgSeoDefaults.ForumTopicLength));

            SetDatabaseValidationRules<Topic>(dataProvider);
        }
    }
}
