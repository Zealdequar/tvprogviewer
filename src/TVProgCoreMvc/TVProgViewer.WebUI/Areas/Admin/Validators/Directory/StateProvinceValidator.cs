using FluentValidation;
using TVProgViewer.WebUI.Areas.Admin.Models.Directory;
using TVProgViewer.Core.Domain.Directory;
using TVProgViewer.Data;
using TVProgViewer.Services.Localization;
using TVProgViewer.Web.Framework.Validators;

namespace TVProgViewer.WebUI.Areas.Admin.Validators.Directory
{
    public partial class StateProvinceValidator : BaseTvProgValidator<StateProvinceModel>
    {
        public StateProvinceValidator(ILocalizationService localizationService, ITvProgDataProvider dataProvider)
        {
            RuleFor(x => x.Name).NotEmpty().WithMessageAwait(localizationService.GetResourceAsync("Admin.Configuration.Countries.States.Fields.Name.Required"));

            SetDatabaseValidationRules<StateProvince>(dataProvider);
        }
    }
}