using System;
using TvProgViewer.Web.Framework.Models;

namespace TvProgViewer.WebUI.Models.Checkout
{
    public partial record CheckoutPaymentInfoModel : BaseTvProgModel
    {
        public Type PaymentViewComponent { get; set; }

        /// <summary>
        /// Used on one-page checkout page
        /// </summary>
        public bool DisplayOrderTotals { get; set; }
    }
}