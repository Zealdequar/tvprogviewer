using System.Collections.Generic;
using TVProgViewer.Web.Framework.Models;
using TVProgViewer.WebUI.Models.Common;

namespace TVProgViewer.WebUI.Models.Checkout
{
    public partial record CheckoutShippingAddressModel : BaseTvProgModel
    {
        public CheckoutShippingAddressModel()
        {
            Warnings = new List<string>();
            ExistingAddresses = new List<AddressModel>();
            InvalidExistingAddresses = new List<AddressModel>();
            ShippingNewAddress = new AddressModel();
            PickupPoints = new List<CheckoutPickupPointModel>();
        }
        
        public IList<string> Warnings { get; set; }

        public IList<AddressModel> ExistingAddresses { get; set; }
        public IList<AddressModel> InvalidExistingAddresses { get; set; }
        public AddressModel ShippingNewAddress { get; set; }
        public bool NewAddressPreselected { get; set; }

        public IList<CheckoutPickupPointModel> PickupPoints { get; set; }
        public bool AllowPickupInStore { get; set; }
        public bool PickupInStore { get; set; }
        public bool PickupInStoreOnly { get; set; }
        public bool DisplayPickupPointsOnMap { get; set; }
        public string GoogleMapsApiKey { get; set; }
    }
}