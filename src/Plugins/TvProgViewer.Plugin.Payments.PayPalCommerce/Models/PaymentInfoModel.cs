using TvProgViewer.Web.Framework.Models;

namespace TvProgViewer.Plugin.Payments.PayPalViewer.Models
{
    /// <summary>
    /// Represents a payment info model
    /// </summary>
    public record PaymentInfoModel : BaseTvProgModel
    {
        #region Properties

        public string OrderId { get; set; }

        public string OrderTotal { get; set; }

        public string Errors { get; set; }

        #endregion
    }
}