using TVProgViewer.Web.Framework.Models;

namespace TVProgViewer.WebUI.Models.Checkout
{
    public partial record CheckoutPaymentInfoModel : BaseTvProgModel
    {
        public string PaymentViewComponentName { get; set; }

        /// <summary>
        /// Used on one-page checkout page
        /// </summary>
        public bool DisplayOrderTotals { get; set; }
    }
}