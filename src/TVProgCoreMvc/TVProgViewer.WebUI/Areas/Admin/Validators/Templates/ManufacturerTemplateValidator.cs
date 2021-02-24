using FluentValidation;
using TVProgViewer.WebUI.Areas.Admin.Models.Templates;
using TVProgViewer.Services.Localization;
using TVProgViewer.Web.Framework.Validators;
using TVProgViewer.Data;
using TVProgViewer.Core.Domain.Catalog;

namespace TVProgViewer.WebUI.Areas.Admin.Validators.Templates
{
    public partial class ManufacturerTemplateValidator : BaseTvProgValidator<ManufacturerTemplateModel>
    {
        public ManufacturerTemplateValidator(ILocalizationService localizationService, ITvProgDataProvider dataProvider)
        {
            RuleFor(x => x.Name).NotEmpty().WithMessageAwait(localizationService.GetResourceAsync("Admin.System.Templates.Manufacturer.Name.Required"));
            RuleFor(x => x.ViewPath).NotEmpty().WithMessageAwait(localizationService.GetResourceAsync("Admin.System.Templates.Manufacturer.ViewPath.Required"));

            SetDatabaseValidationRules<ManufacturerTemplate>(dataProvider);
        }
    }
}