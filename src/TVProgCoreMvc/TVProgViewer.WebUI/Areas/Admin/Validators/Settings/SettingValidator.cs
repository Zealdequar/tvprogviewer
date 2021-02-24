using FluentValidation;
using TVProgViewer.WebUI.Areas.Admin.Models.Settings;
using TVProgViewer.Services.Localization;
using TVProgViewer.Web.Framework.Validators;
using TVProgViewer.Data;
using TVProgViewer.Core.Domain.Configuration;

namespace TVProgViewer.WebUI.Areas.Admin.Validators.Settings
{
    public partial class SettingValidator : BaseTvProgValidator<SettingModel>
    {
        public SettingValidator(ILocalizationService localizationService, ITvProgDataProvider dataProvider)
        {
            RuleFor(x => x.Name).NotEmpty().WithMessageAwait(localizationService.GetResourceAsync("Admin.Configuration.Settings.AllSettings.Fields.Name.Required"));

            SetDatabaseValidationRules<Setting>(dataProvider);
        }
    }
}