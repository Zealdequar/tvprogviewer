﻿using FluentValidation;
using TvProgViewer.Core.Domain.Catalog;
using TvProgViewer.Data.Mapping;
using TvProgViewer.Services.Localization;
using TvProgViewer.WebUI.Areas.Admin.Models.Catalog;
using TvProgViewer.Web.Framework.Validators;

namespace TvProgViewer.WebUI.Areas.Admin.Validators.Catalog
{
    public partial class SpecificationAttributeValidator : BaseTvProgValidator<SpecificationAttributeModel>
    {
        public SpecificationAttributeValidator(ILocalizationService localizationService, IMappingEntityAccessor mappingEntityAccessor)
        {
            RuleFor(x => x.Name).NotEmpty().WithMessageAwait(localizationService.GetResourceAsync("Admin.Catalog.Attributes.SpecificationAttributes.SpecificationAttribute.Fields.Name.Required"));

            SetDatabaseValidationRules<SpecificationAttribute>(mappingEntityAccessor);
        }
    }
}