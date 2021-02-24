using FluentValidation;
using TVProgViewer.WebUI.Areas.Admin.Models.Vendors;
using TVProgViewer.Core.Domain.Vendors;
using TVProgViewer.Data;
using TVProgViewer.Services.Localization;
using TVProgViewer.Web.Framework.Validators;
using TVProgViewer.Services.Seo;

namespace TVProgViewer.WebUI.Areas.Admin.Validators.Vendors
{
    public partial class VendorValidator : BaseTvProgValidator<VendorModel>
    {
        public VendorValidator(ILocalizationService localizationService, ITvProgDataProvider dataProvider)
        {
            RuleFor(x => x.Name).NotEmpty().WithMessageAwait(localizationService.GetResourceAsync("Admin.Vendors.Fields.Name.Required"));

            RuleFor(x => x.Email).NotEmpty().WithMessageAwait(localizationService.GetResourceAsync("Admin.Vendors.Fields.Email.Required"));
            RuleFor(x => x.Email).EmailAddress().WithMessageAwait(localizationService.GetResourceAsync("Admin.Common.WrongEmail"));
            RuleFor(x => x.PageSizeOptions).Must(ValidatorUtilities.PageSizeOptionsValidator).WithMessageAwait(localizationService.GetResourceAsync("Admin.Vendors.Fields.PageSizeOptions.ShouldHaveUniqueItems"));
            RuleFor(x => x.PageSize).Must((x, context) =>
            {
                if (!x.AllowUsersToSelectPageSize && x.PageSize <= 0)
                    return false;

                return true;
            }).WithMessageAwait(localizationService.GetResourceAsync("Admin.Vendors.Fields.PageSize.Positive"));
            RuleFor(x => x.SeName).Length(0, TvProgSeoDefaults.SearchEngineNameLength)
                .WithMessageAwait(localizationService.GetResourceAsync("Admin.SEO.SeName.MaxLengthValidation"), TvProgSeoDefaults.SearchEngineNameLength);

            SetDatabaseValidationRules<Vendor>(dataProvider);
        }
    }
}