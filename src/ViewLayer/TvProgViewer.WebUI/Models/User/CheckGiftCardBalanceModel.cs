using TvProgViewer.Web.Framework.Models;
using TvProgViewer.Web.Framework.Mvc.ModelBinding;

namespace TvProgViewer.WebUI.Models.User
{
    public partial record CheckGiftCardBalanceModel : BaseTvProgModel
    {
        public string Result { get; set; }

        public string Message { get; set; }
        
        [TvProgResourceDisplayName("ShoppingCart.GiftCardCouponCode.Tooltip")]
        public string GiftCardCode { get; set; }
    }
}
