using FluentValidation;
using TVProgViewer.WebUI.Areas.Admin.Models.Tasks;
using TVProgViewer.Services.Localization;
using TVProgViewer.Web.Framework.Validators;
using TVProgViewer.Data;
using TVProgViewer.Core.Domain.Tasks;

namespace TVProgViewer.WebUI.Areas.Admin.Validators.Tasks
{
    public partial class ScheduleTaskValidator : BaseTvProgValidator<ScheduleTaskModel>
    {
        public ScheduleTaskValidator(ILocalizationService localizationService, ITvProgDataProvider dataProvider)
        {
            RuleFor(x => x.Name).NotEmpty().WithMessageAwait(localizationService.GetResourceAsync("Admin.System.ScheduleTasks.Name.Required"));
            RuleFor(x => x.Seconds).GreaterThan(0).WithMessageAwait(localizationService.GetResourceAsync("Admin.System.ScheduleTasks.Seconds.Positive"));

            SetDatabaseValidationRules<ScheduleTask>(dataProvider);
        }
    }
}