using FluentValidation;
using TVProgViewer.WebUI.Areas.Admin.Models.Catalog;
using TVProgViewer.Core.Domain.Catalog;
using TVProgViewer.Data;
using TVProgViewer.Services.Defaults;
using TVProgViewer.Services.Localization;
using TVProgViewer.Web.Framework.Validators;

namespace TVProgViewer.WebUI.Areas.Admin.Validators.Catalog
{
    public partial class ManufacturerValidator : BaseTvProgValidator<ManufacturerModel>
    {
        public ManufacturerValidator(IDataProvider dataProvider, ILocalizationService localizationService)
        {
            RuleFor(x => x.Name).NotEmpty().WithMessage(localizationService.GetResource("Admin.Catalog.Manufacturers.Fields.Name.Required"));
            RuleFor(x => x.PageSizeOptions).Must(ValidatorUtilities.PageSizeOptionsValidator).WithMessage(localizationService.GetResource("Admin.Catalog.Manufacturers.Fields.PageSizeOptions.ShouldHaveUniqueItems"));
            RuleFor(x => x.PageSize).Must((x, context) =>
            {
                if (!x.AllowUsersToSelectPageSize && x.PageSize <= 0)
                    return false;

                return true;
            }).WithMessage(localizationService.GetResource("Admin.Catalog.Manufacturers.Fields.PageSize.Positive"));
            RuleFor(x => x.SeName).Length(0, TvProgSeoDefaults.SearchEngineNameLength)
                .WithMessage(string.Format(localizationService.GetResource("Admin.SEO.SeName.MaxLengthValidation"), TvProgSeoDefaults.SearchEngineNameLength));

            SetDatabaseValidationRules<Manufacturer>(dataProvider);
        }
    }
}