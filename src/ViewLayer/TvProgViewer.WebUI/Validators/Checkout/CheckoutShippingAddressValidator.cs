using FluentValidation;
using TvProgViewer.Core.Domain.Common;
using TvProgViewer.Core.Domain.Users;
using TvProgViewer.Services.Directory;
using TvProgViewer.Services.Localization;
using TvProgViewer.WebUI.Models.Checkout;
using TvProgViewer.WebUI.Validators.Common;

namespace TvProgViewer.WebUI.Validators.Checkout
{
    public partial class CheckoutShippingAddressValidator : AbstractValidator<CheckoutShippingAddressModel>
    {
        public CheckoutShippingAddressValidator(ILocalizationService localizationService,
            IStateProvinceService stateProvinceService,
            AddressSettings addressSettings,
            UserSettings userSettings)
        {
            RuleFor(shippingAdress => shippingAdress.ShippingNewAddress).SetValidator(
                new AddressValidator(localizationService, stateProvinceService, addressSettings, userSettings));
        }
    }
}