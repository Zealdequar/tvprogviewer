using TvProgViewer.Web.Framework.Models;

namespace TvProgViewer.Plugin.Payments.CheckMoneyOrder.Models
{
    public record PaymentInfoModel : BaseTvProgModel
    {
        public string DescriptionText { get; set; }
    }
}