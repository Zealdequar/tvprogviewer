using FluentValidation;
using TvProgViewer.Core.Domain.Common;
using TvProgViewer.Core.Domain.Users;
using TvProgViewer.Services.Directory;
using TvProgViewer.Services.Localization;
using TvProgViewer.WebUI.Models.Checkout;
using TvProgViewer.WebUI.Validators.Common;

namespace TvProgViewer.WebUI.Validators.Checkout
{
    public partial class CheckoutBillingAddressValidator : AbstractValidator<CheckoutBillingAddressModel>
    {
        public CheckoutBillingAddressValidator(ILocalizationService localizationService,
            IStateProvinceService stateProvinceService,
            AddressSettings addressSettings,
            UserSettings userSettings)
        {
            RuleFor(billingAddress => billingAddress.BillingNewAddress).SetValidator(new AddressValidator(localizationService, stateProvinceService, addressSettings, userSettings));
        }
    }
}
