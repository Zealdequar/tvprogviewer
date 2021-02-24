using FluentValidation;
using TVProgViewer.WebUI.Areas.Admin.Models.Vendors;
using TVProgViewer.Core.Domain.Vendors;
using TVProgViewer.Data;
using TVProgViewer.Services.Localization;
using TVProgViewer.Web.Framework.Validators;

namespace TVProgViewer.WebUI.Areas.Admin.Validators.Vendors
{
    public partial class VendorAttributeValidator : BaseTvProgValidator<VendorAttributeModel>
    {
        public VendorAttributeValidator(ILocalizationService localizationService, ITvProgDataProvider dataProvider)
        {
            RuleFor(x => x.Name).NotEmpty().WithMessageAwait(localizationService.GetResourceAsync("Admin.Vendors.VendorAttributes.Fields.Name.Required"));

            SetDatabaseValidationRules<VendorAttribute>(dataProvider);
        }
    }
}