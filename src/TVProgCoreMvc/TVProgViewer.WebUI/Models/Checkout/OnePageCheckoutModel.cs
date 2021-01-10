using TVProgViewer.Web.Framework.Models;

namespace TVProgViewer.WebUI.Models.Checkout
{
    public partial record OnePageCheckoutModel : BaseTvProgModel
    {
        public bool ShippingRequired { get; set; }
        public bool DisableBillingAddressCheckoutStep { get; set; }

        public CheckoutBillingAddressModel BillingAddress { get; set; }
    }
}