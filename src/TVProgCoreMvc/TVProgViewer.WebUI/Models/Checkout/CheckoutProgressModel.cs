using TVProgViewer.Web.Framework.Models;

namespace TVProgViewer.WebUI.Models.Checkout
{
    public partial record CheckoutProgressModel : BaseTvProgModel
    {
        public CheckoutProgressStep CheckoutProgressStep { get; set; }
    }

    public enum CheckoutProgressStep
    {
        Cart,
        Address,
        Shipping,
        Payment,
        Confirm,
        Complete
    }
}