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
        public VendorAttributeValidator(IDataProvider dataProvider, ILocalizationService localizationService)
        {
            RuleFor(x => x.Name).NotEmpty().WithMessage(localizationService.GetResource("Admin.Vendors.VendorAttributes.Fields.Name.Required"));

            SetDatabaseValidationRules<VendorAttribute>(dataProvider);
        }
    }
}