using System.Collections.Generic;
using TvProgViewer.Web.Framework.Models;
using TvProgViewer.Web.Framework.Mvc.ModelBinding;
using TvProgViewer.WebUI.Models.Common;

namespace TvProgViewer.WebUI.Models.Checkout
{
    public partial record CheckoutBillingAddressModel : BaseTvProgModel
    {
        public CheckoutBillingAddressModel()
        {
            ExistingAddresses = new List<AddressModel>();
            InvalidExistingAddresses = new List<AddressModel>();
            BillingNewAddress = new AddressModel();
        }

        public IList<AddressModel> ExistingAddresses { get; set; }
        public IList<AddressModel> InvalidExistingAddresses { get; set; }

        public AddressModel BillingNewAddress { get; set; }

        public bool ShipToSameAddress { get; set; }
        public bool ShipToSameAddressAllowed { get; set; }

        /// <summary>
        /// Used on one-page checkout page
        /// </summary>
        public bool NewAddressPreselected { get; set; }

        public bool EuVatEnabled { get; set; }
        public bool EuVatEnabledForGuests { get; set; }

        [TvProgResourceDisplayName("Checkout.VatNumber")]
        public string VatNumber { get; set; }
    }
}