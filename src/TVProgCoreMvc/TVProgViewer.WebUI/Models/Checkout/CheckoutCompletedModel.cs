using TVProgViewer.Web.Framework.Models;

namespace TVProgViewer.WebUI.Models.Checkout
{
    public partial record CheckoutCompletedModel : BaseTvProgModel
    {
        public int OrderId { get; set; }
        public string CustomOrderNumber { get; set; }
        public bool OnePageCheckoutEnabled { get; set; }
    }
}