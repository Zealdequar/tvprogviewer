using FluentValidation;
using TvProgViewer.Core.Domain.Catalog;
using TvProgViewer.Data.Mapping;
using TvProgViewer.Services.Localization;
using TvProgViewer.WebUI.Areas.Admin.Models.Catalog;
using TvProgViewer.Web.Framework.Validators;

namespace TvProgViewer.WebUI.Areas.Admin.Validators.Catalog
{
    /// <summary>
    /// Represents a validator for <see cref="SpecificationAttributeGroupModel"/>
    /// </summary>
    public partial class SpecificationAttributeGroupValidator : BaseTvProgValidator<SpecificationAttributeGroupModel>
    {
        public SpecificationAttributeGroupValidator(ILocalizationService localizationService, IMappingEntityAccessor mappingEntityAccessor)
        {
            RuleFor(x => x.Name)
                .NotEmpty()
                .WithMessageAwait(localizationService.GetResourceAsync("Admin.Catalog.Attributes.SpecificationAttributes.SpecificationAttributeGroup.Fields.Name.Required"));

            SetDatabaseValidationRules<SpecificationAttributeGroup>(mappingEntityAccessor);
        }
    }
}
