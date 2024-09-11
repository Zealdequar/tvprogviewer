using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;
using TvProgViewer.Web.Framework.Models;
using TvProgViewer.Web.Framework.Mvc.ModelBinding;

namespace TvProgViewer.Plugin.Payments.PayPalViewer.Models
{
    /// <summary>
    /// Represents configuration model
    /// </summary>
    public record ConfigurationModel : BaseTvProgModel
    {
        #region Ctor

        public ConfigurationModel()
        {
            PaymentTypes = new List<SelectListItem>();
            OnboardingModel = new OnboardingModel();
        }

        #endregion

        #region Properties

        public bool IsConfigured { get; set; }

        public int ActiveStoreScopeConfiguration { get; set; }

        [TvProgResourceDisplayName("Plugins.Payments.PayPalViewer.Fields.Email")]
        [EmailAddress]
        public string Email { get; set; }
        public bool Email_OverrideForStore { get; set; }

        [TvProgResourceDisplayName("Plugins.Payments.PayPalViewer.Fields.SetCredentialsManually")]
        public bool SetCredentialsManually { get; set; }
        public bool SetCredentialsManually_OverrideForStore { get; set; }

        [TvProgResourceDisplayName("Plugins.Payments.PayPalViewer.Fields.UseSandbox")]
        public bool UseSandbox { get; set; }
        public bool UseSandbox_OverrideForStore { get; set; }

        [TvProgResourceDisplayName("Plugins.Payments.PayPalViewer.Fields.ClientId")]
        public string ClientId { get; set; }
        public bool ClientId_OverrideForStore { get; set; }

        [TvProgResourceDisplayName("Plugins.Payments.PayPalViewer.Fields.SecretKey")]
        [DataType(DataType.Password)]
        public string SecretKey { get; set; }
        public bool SecretKey_OverrideForStore { get; set; }

        [TvProgResourceDisplayName("Plugins.Payments.PayPalViewer.Fields.PaymentType")]
        public int PaymentTypeId { get; set; }
        public bool PaymentTypeId_OverrideForStore { get; set; }
        public IList<SelectListItem> PaymentTypes { get; set; }

        [TvProgResourceDisplayName("Plugins.Payments.PayPalViewer.Fields.DisplayButtonsOnShoppingCart")]
        public bool DisplayButtonsOnShoppingCart { get; set; }
        public bool DisplayButtonsOnShoppingCart_OverrideForStore { get; set; }

        [TvProgResourceDisplayName("Plugins.Payments.PayPalViewer.Fields.DisplayButtonsOnTvChannelDetails")]
        public bool DisplayButtonsOnTvChannelDetails { get; set; }
        public bool DisplayButtonsOnTvChannelDetails_OverrideForStore { get; set; }

        [TvProgResourceDisplayName("Plugins.Payments.PayPalViewer.Fields.DisplayLogoInHeaderLinks")]
        public bool DisplayLogoInHeaderLinks { get; set; }
        public bool DisplayLogoInHeaderLinks_OverrideForStore { get; set; }

        [TvProgResourceDisplayName("Plugins.Payments.PayPalViewer.Fields.LogoInHeaderLinks")]
        public string LogoInHeaderLinks { get; set; }
        public bool LogoInHeaderLinks_OverrideForStore { get; set; }

        [TvProgResourceDisplayName("Plugins.Payments.PayPalViewer.Fields.DisplayLogoInFooter")]
        public bool DisplayLogoInFooter { get; set; }
        public bool DisplayLogoInFooter_OverrideForStore { get; set; }

        [TvProgResourceDisplayName("Plugins.Payments.PayPalViewer.Fields.LogoInFooter")]
        public string LogoInFooter { get; set; }
        public bool LogoInFooter_OverrideForStore { get; set; }

        [TvProgResourceDisplayName("Plugins.Payments.PayPalViewer.Fields.DisplayPayLaterMessages")]
        public bool DisplayPayLaterMessages { get; set; }
        public bool DisplayPayLaterMessages_OverrideForStore { get; set; }

        public OnboardingModel OnboardingModel { get; set; }

        #endregion
    }
}