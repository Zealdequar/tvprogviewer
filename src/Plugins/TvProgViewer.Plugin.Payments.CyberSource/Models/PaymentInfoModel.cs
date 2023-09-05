using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc.Rendering;
using TvProgViewer.Web.Framework.Models;
using TvProgViewer.Web.Framework.Mvc.ModelBinding;

namespace TvProgViewer.Plugin.Payments.CyberSource.Models
{
    /// <summary>
    /// Represents a payment info model
    /// </summary>
    public record PaymentInfoModel : BaseTvProgModel
    {
        #region Ctor

        public PaymentInfoModel()
        {
            ExpireMonths = new List<SelectListItem>();
            ExpireYears = new List<SelectListItem>();
            ExistingTokens = new List<SelectListItem>();
        }

        #endregion

        #region Properties

        [TvProgResourceDisplayName("Plugins.Payments.CyberSource.Payment.NewCard")]
        public bool NewCard { get; set; }

        [TvProgResourceDisplayName("Plugins.Payments.CyberSource.Payment.SaveCardOnFile")]
        public bool SaveCardOnFile { get; set; }

        [TvProgResourceDisplayName("Payment.CardNumber")]
        public string CardNumber { get; set; }

        [TvProgResourceDisplayName("Payment.ExpirationDate")]
        public string ExpireMonth { get; set; }

        [TvProgResourceDisplayName("Payment.ExpirationDate")]
        public string ExpireYear { get; set; }

        [TvProgResourceDisplayName("Payment.CardCode")]
        public string Cvv { get; set; }

        public bool IsFlexMicroFormEnabled { get; set; }

        public bool TokenizationEnabled { get; set; }

        public bool ShowExistingTokenSection { get; set; }

        public int SelectedTokenId { get; set; }

        public string CaptureContext { get; set; }

        public string TransientToken { get; set; }

        public string AuthenticationTransactionId { get; set; }

        public string ReferenceId { get; set; }

        public string Errors { get; set; }

        public IList<SelectListItem> ExpireMonths { get; set; }

        public IList<SelectListItem> ExpireYears { get; set; }

        public IList<SelectListItem> ExistingTokens { get; set; }

        #endregion
    }
}