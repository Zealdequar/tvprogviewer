using TvProgViewer.Web.Framework.Models;

namespace TvProgViewer.WebUI.Models.Checkout
{
    public partial record CheckoutCompletedModel : BaseTvProgModel
    {
        public int OrderId { get; set; }
        public string CustomOrderNumber { get; set; }
        public bool OnePageCheckoutEnabled { get; set; }
    }
}