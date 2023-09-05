using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;
using TvProgViewer.Plugin.Tax.Avalara.Models.Log;
using TvProgViewer.WebUI.Areas.Admin.Models.Common;
using TvProgViewer.Web.Framework.Models;
using TvProgViewer.Web.Framework.Mvc.ModelBinding;

namespace TvProgViewer.Plugin.Tax.Avalara.Models.Configuration
{
    /// <summary>
    /// Represents a configuration model
    /// </summary>
    public record ConfigurationModel : BaseTvProgModel, IAclSupportedModel
    {
        #region Ctor

        public ConfigurationModel()
        {
            TestAddress = new AddressModel();
            Companies = new List<SelectListItem>();
            TaxOriginAddressTypes = new List<SelectListItem>();
            TaxTransactionLogSearchModel = new TaxTransactionLogSearchModel();
            SelectedUserRoleIds = new List<int>();
            AvailableUserRoles = new List<SelectListItem>();
        }

        #endregion

        #region Properties

        #region Common

        public bool IsConfigured { get; set; }

        [TvProgResourceDisplayName("Plugins.Tax.Avalara.Fields.AccountId")]
        public string AccountId { get; set; }

        [TvProgResourceDisplayName("Plugins.Tax.Avalara.Fields.LicenseKey")]
        [DataType(DataType.Password)]
        public string LicenseKey { get; set; }

        [TvProgResourceDisplayName("Plugins.Tax.Avalara.Fields.Company")]
        public string CompanyCode { get; set; }
        public IList<SelectListItem> Companies { get; set; }

        [TvProgResourceDisplayName("Plugins.Tax.Avalara.Fields.UseSandbox")]
        public bool UseSandbox { get; set; }

        [TvProgResourceDisplayName("Plugins.Tax.Avalara.Fields.EnableLogging")]
        public bool EnableLogging { get; set; }

        public AddressModel TestAddress { get; set; }

        public string TestTaxResult { get; set; }

        public TaxTransactionLogSearchModel TaxTransactionLogSearchModel { get; set; }

        public bool HideGeneralBlock { get; set; }

        public bool HideLogBlock { get; set; }

        #endregion

        #region Tax calculation

        [TvProgResourceDisplayName("Plugins.Tax.Avalara.Fields.CommitTransactions")]
        public bool CommitTransactions { get; set; }

        [TvProgResourceDisplayName("Plugins.Tax.Avalara.Fields.ValidateAddress")]
        public bool ValidateAddress { get; set; }

        [TvProgResourceDisplayName("Plugins.Tax.Avalara.Fields.TaxOriginAddressType")]
        public int TaxOriginAddressTypeId { get; set; }
        public IList<SelectListItem> TaxOriginAddressTypes { get; set; }

        [TvProgResourceDisplayName("Plugins.Tax.Avalara.Fields.UseTaxRateTables")]
        public bool UseTaxRateTables { get; set; }

        [TvProgResourceDisplayName("Plugins.Tax.Avalara.Fields.GetTaxRateByAddressOnly")]
        public bool GetTaxRateByAddressOnly { get; set; }

        #endregion

        #region Certificates

        [TvProgResourceDisplayName("Plugins.Tax.Avalara.Fields.EnableCertificates")]
        public bool EnableCertificates { get; set; }

        [TvProgResourceDisplayName("Plugins.Tax.Avalara.Fields.AutoValidateCertificate")]
        public bool AutoValidateCertificate { get; set; }

        [TvProgResourceDisplayName("Plugins.Tax.Avalara.Fields.AllowEditUser")]
        public bool AllowEditUser { get; set; }

        [TvProgResourceDisplayName("Plugins.Tax.Avalara.Fields.DisplayNoValidCertificatesMessage")]
        public bool DisplayNoValidCertificatesMessage { get; set; }

        //ACL (user roles)
        [TvProgResourceDisplayName("Plugins.Tax.Avalara.Fields.UserRoles")]
        public IList<int> SelectedUserRoleIds { get; set; }
        public IList<SelectListItem> AvailableUserRoles { get; set; }

        #endregion

        #endregion
    }
}