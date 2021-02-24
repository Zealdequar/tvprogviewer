using FluentValidation;
using TVProgViewer.WebUI.Areas.Admin.Models.Templates;
using TVProgViewer.Services.Localization;
using TVProgViewer.Web.Framework.Validators;
using TVProgViewer.Data;
using TVProgViewer.Core.Domain.Topics;

namespace TVProgViewer.WebUI.Areas.Admin.Validators.Templates
{
    public partial class TopicTemplateValidator : BaseTvProgValidator<TopicTemplateModel>
    {
        public TopicTemplateValidator(ILocalizationService localizationService, ITvProgDataProvider dataProvider)
        {
            RuleFor(x => x.Name).NotEmpty().WithMessageAwait(localizationService.GetResourceAsync("Admin.System.Templates.Topic.Name.Required"));
            RuleFor(x => x.ViewPath).NotEmpty().WithMessageAwait(localizationService.GetResourceAsync("Admin.System.Templates.Topic.ViewPath.Required"));

            SetDatabaseValidationRules<TopicTemplate>(dataProvider);
        }
    }
}