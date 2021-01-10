using FluentValidation;
using TVProgViewer.Services.Localization;
using TVProgViewer.Web.Framework.Validators;
using TVProgViewer.WebUI.Models.Vendors;

namespace TVProgViewer.WebUI.Validators.Vendors
{
    public partial class VendorInfoValidator : BaseTvProgValidator<VendorInfoModel>
    {
        public VendorInfoValidator(ILocalizationService localizationService)
        {
            RuleFor(x => x.Name).NotEmpty().WithMessage(localizationService.GetResource("Account.VendorInfo.Name.Required"));

            RuleFor(x => x.Email).NotEmpty().WithMessage(localizationService.GetResource("Account.VendorInfo.Email.Required"));
            RuleFor(x => x.Email).EmailAddress().WithMessage(localizationService.GetResource("Common.WrongEmail"));
        }
    }
}