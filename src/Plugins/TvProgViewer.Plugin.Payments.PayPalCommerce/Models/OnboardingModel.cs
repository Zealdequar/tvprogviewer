using TvProgViewer.Web.Framework.Models;

namespace TvProgViewer.Plugin.Payments.PayPalViewer.Models
{
    /// <summary>
    /// Represents an onboarding model
    /// </summary>
    public record OnboardingModel : BaseTvProgModel
    {
        #region Properties

        public string MerchantGuid { get; set; }

        public string Email { get; set; }
        public bool Email_OverrideForStore { get; set; }

        public string SignUpUrl { get; set; }

        public string SharedId { get; set; }

        public string AuthCode { get; set; }

        public bool AccountCreated { get; set; }

        public bool EmailConfirmed { get; set; }

        public bool PaymentsReceivable { get; set; }

        public bool PermissionGranted { get; set; }

        public bool DisplayStatus { get; set; }

        #endregion
    }
}