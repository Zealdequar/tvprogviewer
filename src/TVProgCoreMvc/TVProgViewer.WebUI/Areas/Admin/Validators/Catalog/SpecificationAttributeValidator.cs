using FluentValidation;
using TVProgViewer.WebUI.Areas.Admin.Models.Catalog;
using TVProgViewer.Services.Localization;
using TVProgViewer.Web.Framework.Validators;
using TVProgViewer.Data;
using TVProgViewer.Core.Domain.Catalog;

namespace TVProgViewer.WebUI.Areas.Admin.Validators.Catalog
{
    public partial class SpecificationAttributeValidator : BaseTvProgValidator<SpecificationAttributeModel>
    {
        public SpecificationAttributeValidator(ILocalizationService localizationService, ITvProgDataProvider dataProvider)
        {
            RuleFor(x => x.Name).NotEmpty().WithMessageAwait(localizationService.GetResourceAsync("Admin.Catalog.Attributes.SpecificationAttributes.SpecificationAttribute.Fields.Name.Required"));

            SetDatabaseValidationRules<SpecificationAttribute>(dataProvider);
        }
    }
}