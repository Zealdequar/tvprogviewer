using FluentValidation;
using TVProgViewer.WebUI.Areas.Admin.Models.Forums;
using TVProgViewer.Core.Domain.Forums;
using TVProgViewer.Data;
using TVProgViewer.Services.Localization;
using TVProgViewer.Web.Framework.Validators;

namespace TVProgViewer.WebUI.Areas.Admin.Validators.Forums
{
    public partial class ForumGroupValidator : BaseTvProgValidator<ForumGroupModel>
    {
        public ForumGroupValidator(ILocalizationService localizationService, ITvProgDataProvider dataProvider)
        {
            RuleFor(x => x.Name).NotEmpty().WithMessageAwait(localizationService.GetResourceAsync("Admin.ContentManagement.Forums.ForumGroup.Fields.Name.Required"));

            SetDatabaseValidationRules<ForumGroup>(dataProvider);
        }
    }
}