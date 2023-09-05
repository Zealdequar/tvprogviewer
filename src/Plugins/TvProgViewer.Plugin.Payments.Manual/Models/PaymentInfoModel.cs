using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc.Rendering;
using TvProgViewer.Web.Framework.Mvc.ModelBinding;
using TvProgViewer.Web.Framework.Models;

namespace TvProgViewer.Plugin.Payments.Manual.Models
{
    public record PaymentInfoModel : BaseTvProgModel
    {
        public PaymentInfoModel()
        {
            CreditCardTypes = new List<SelectListItem>();
            ExpireMonths = new List<SelectListItem>();
            ExpireYears = new List<SelectListItem>();
        }

        [TvProgResourceDisplayName("Payment.SelectCreditCard")]
        public string CreditCardType { get; set; }

        [TvProgResourceDisplayName("Payment.SelectCreditCard")]
        public IList<SelectListItem> CreditCardTypes { get; set; }

        [TvProgResourceDisplayName("Payment.CardholderName")]
        public string CardholderName { get; set; }

        [TvProgResourceDisplayName("Payment.CardNumber")]
        public string CardNumber { get; set; }

        [TvProgResourceDisplayName("Payment.ExpirationDate")]
        public string ExpireMonth { get; set; }

        [TvProgResourceDisplayName("Payment.ExpirationDate")]
        public string ExpireYear { get; set; }

        public IList<SelectListItem> ExpireMonths { get; set; }

        public IList<SelectListItem> ExpireYears { get; set; }

        [TvProgResourceDisplayName("Payment.CardCode")]
        public string CardCode { get; set; }
    }
}