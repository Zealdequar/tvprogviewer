using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using TvProgViewer.Core;
using TvProgViewer.Core.Domain.Tax;
using TvProgViewer.Services.Common;
using TvProgViewer.Services.Users;
using TvProgViewer.Web.Framework.Controllers;

namespace TvProgViewer.Plugin.Tax.Avalara.Controllers
{
    [AutoValidateAntiforgeryToken]
    public class AddressValidationController : BaseController
    {
        #region Fields

        private readonly IAddressService _addressService;
        private readonly IUserService _userService;
        private readonly IWorkContext _workContext;
        private readonly TaxSettings _taxSettings;

        #endregion

        #region Ctor

        public AddressValidationController(IAddressService addressService,
            IUserService userService,
            IWorkContext workContext,
            TaxSettings taxSettings)
        {
            _addressService = addressService;
            _userService = userService;
            _workContext = workContext;
            _taxSettings = taxSettings;
        }

        #endregion

        #region Methods

        [HttpPost]
        public async Task<IActionResult> UseValidatedAddress(int addressId, bool isNewAddress)
        {
            //try to get an address by the passed identifier
            var address = await _addressService.GetAddressByIdAsync(addressId);
            if (address != null)
            {
                var user = await _workContext.GetCurrentUserAsync();
                //add address to user collection if it's a new
                if (isNewAddress) await _userService.InsertUserAddressAsync(user, address);

                //and update appropriate user address
                if (_taxSettings.TaxBasedOn == TaxBasedOn.BillingAddress)
                    (user).BillingAddressId = address.Id;
                if (_taxSettings.TaxBasedOn == TaxBasedOn.ShippingAddress)
                    (user).ShippingAddressId = address.Id;
                await _userService.UpdateUserAsync(user);
            }

            //nothing to return
            return Content(string.Empty);
        }

        #endregion
    }
}