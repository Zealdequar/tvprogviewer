using TVProgViewer.Web.Framework.Models;
using TVProgViewer.Web.Framework.Mvc.ModelBinding;
using System.ComponentModel.DataAnnotations;

namespace TVProgViewer.WebUI.Models.User
{
    public partial record CheckGiftCardBalanceModel : BaseTvProgModel
    {
        public string Result { get; set; }

        public string Message { get; set; }
        
        [TvProgResourceDisplayName("ShoppingCart.GiftCardCouponCode.Tooltip")]
        public string GiftCardCode { get; set; }
    }
}
