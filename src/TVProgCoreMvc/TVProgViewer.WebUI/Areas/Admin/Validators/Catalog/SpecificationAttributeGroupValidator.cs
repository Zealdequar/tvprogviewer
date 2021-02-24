using FluentValidation;
using TVProgViewer.Core.Domain.Catalog;
using TVProgViewer.Data;
using TVProgViewer.Services.Localization;
using TVProgViewer.WebUI.Areas.Admin.Models.Catalog;
using TVProgViewer.Web.Framework.Validators;

namespace TVProgViewer.WebUI.Areas.Admin.Validators.Catalog
{
    /// <summary>
    /// Represents a validator for <see cref="SpecificationAttributeGroupModel"/>
    /// </summary>
    public partial class SpecificationAttributeGroupValidator : BaseTvProgValidator<SpecificationAttributeGroupModel>
    {
        public SpecificationAttributeGroupValidator(ILocalizationService localizationService, ITvProgDataProvider dataProvider)
        {
            RuleFor(x => x.Name)
                .NotEmpty()
                .WithMessageAwait(localizationService.GetResourceAsync("Admin.Catalog.Attributes.SpecificationAttributes.SpecificationAttributeGroup.Fields.Name.Required"));

            SetDatabaseValidationRules<SpecificationAttributeGroup>(dataProvider);
        }
    }
}
