using FluentValidation;
using TVProgViewer.WebUI.Areas.Admin.Models.Catalog;
using TVProgViewer.Services.Localization;
using TVProgViewer.Web.Framework.Validators;
using TVProgViewer.Core.Domain.Catalog;
using TVProgViewer.Data;

namespace TVProgViewer.WebUI.Areas.Admin.Validators.Catalog
{
    public partial class SpecificationAttributeOptionValidator : BaseTvProgValidator<SpecificationAttributeOptionModel>
    {
        public SpecificationAttributeOptionValidator(ILocalizationService localizationService, ITvProgDataProvider dataProvider)
        {
            RuleFor(x => x.Name).NotEmpty().WithMessageAwait(localizationService.GetResourceAsync("Admin.Catalog.Attributes.SpecificationAttributes.SpecificationAttribute.Options.Fields.Name.Required"));

            SetDatabaseValidationRules<SpecificationAttributeOption>(dataProvider);
        }
    }
}