﻿using System.Collections.Generic;
using FluentMigrator;
using TvProgViewer.Data;
using TvProgViewer.Data.Migrations;
using TvProgViewer.Services.Configuration;
using TvProgViewer.Services.Localization;
using TvProgViewer.Web.Framework.Extensions;

namespace TvProgViewer.Plugin.Tax.Avalara.Data
{
    [TvProgMigration("2021-09-06 00:00:00", "Tax.Avalara 2.50. Add certificates feature", MigrationProcessType.Update)]
    public class CertificatesMigration : MigrationBase
    {
        #region Fields

        private readonly AvalaraTaxSettings _avalaraTaxSettings;
        private readonly ILanguageService _languageService;
        private readonly ILocalizationService _localizationService;
        private readonly ISettingService _settingService;

        #endregion

        #region Ctor

        public CertificatesMigration(AvalaraTaxSettings avalaraTaxSettings,
            ILanguageService languageService,
            ILocalizationService localizationService,
            ISettingService settingService)
        {
            _avalaraTaxSettings = avalaraTaxSettings;
            _languageService = languageService;
            _localizationService = localizationService;
            _settingService = settingService;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Collect the UP migration expressions
        /// </summary>
        public override void Up()
        {
            if (!DataSettingsManager.IsDatabaseInstalled())
                return;

            //locales
            var (languageId, languages) = this.GetLanguageData();

            _localizationService.AddOrUpdateLocaleResource(new Dictionary<string, string>
            {
                ["Plugins.Tax.Avalara.Configuration.Certificates"] = "Exemption certificates",
                ["Plugins.Tax.Avalara.Configuration.Certificates.InProgress"] = "Exemption certificates",
                ["Plugins.Tax.Avalara.Configuration.Certificates.NotProvisioned"] = "The selected company isn't configured to use exemption certificates, use the button 'Request certificate setup' below to access this feature",
                ["Plugins.Tax.Avalara.Configuration.Certificates.Provisioned"] = "The selected company is configured to use exemption certificates",
                ["Plugins.Tax.Avalara.Configuration.Certificates.Button"] = "Request certificate setup",
                ["Plugins.Tax.Avalara.Configuration.Common"] = "Common settings",
                ["Plugins.Tax.Avalara.Configuration.Credentials.Button"] = "Check connection",
                ["Plugins.Tax.Avalara.Configuration.Credentials.Declined"] = "Credentials declined",
                ["Plugins.Tax.Avalara.Configuration.Credentials.Verified"] = "Credentials verified",
                ["Plugins.Tax.Avalara.Configuration.TaxCalculation"] = "Tax calculation",
                ["Plugins.Tax.Avalara.ExemptionCertificates"] = "Tax exemption certificates",
                ["Plugins.Tax.Avalara.ExemptionCertificates.Add.ExposureZone"] = "State",
                ["Plugins.Tax.Avalara.ExemptionCertificates.Add.Fail"] = "An error occurred while adding a certificate",
                ["Plugins.Tax.Avalara.ExemptionCertificates.Add.Success"] = "Certificate added successfully",
                ["Plugins.Tax.Avalara.ExemptionCertificates.Description"] = @"
                    <h3>Here you can view and manage your certificates.</h3>
                    <p>
                        The certificate document contains information about a user's eligibility for exemption from sales.<br />
                        When you add a certificate, it will be processed and become available for use in calculating tax exemptions.<br />
                    </p>
                    <p>
                        You can also go to <a href=""{0}"" target=""_blank"">CertExpress website</a> where you can follow a step-by-step guide to enter information about your exemption certificates.
                    </p>
                    <p>
                        The certificates entered will be recorded and automatically linked to your account.
                    </p>
                    <p>If you have any questions, please <a href=""{1}"" target=""_blank"">contact us</a>.</p>",
                ["Plugins.Tax.Avalara.ExemptionCertificates.ExpirationDate"] = "Expiration date",
                ["Plugins.Tax.Avalara.ExemptionCertificates.ExposureZone"] = "State",
                ["Plugins.Tax.Avalara.ExemptionCertificates.None"] = "No downloaded certificates yet",
                ["Plugins.Tax.Avalara.ExemptionCertificates.OrderReview"] = "Tax",
                ["Plugins.Tax.Avalara.ExemptionCertificates.OrderReview.Applied"] = "Exemption certificate applied",
                ["Plugins.Tax.Avalara.ExemptionCertificates.OrderReview.None"] = @"You have no valid certificates in the selected region. You can add them in your account on <a href=""{0}"" target=""_blank"" style=""color: #4ab2f1;"">this page</a>.",
                ["Plugins.Tax.Avalara.ExemptionCertificates.SignedDate"] = "Signed date",
                ["Plugins.Tax.Avalara.ExemptionCertificates.Status"] = "Status",
                ["Plugins.Tax.Avalara.ExemptionCertificates.View"] = "View",
                ["Plugins.Tax.Avalara.Fields.AllowEditUser"] = "Allow edit info",
                ["Plugins.Tax.Avalara.Fields.AllowEditUser.Hint"] = "Determine whether to allow users to edit their info (name, phone, address, etc) when managing certificates. If disabled, the info will be auto updated when users change details in their accounts.",
                ["Plugins.Tax.Avalara.Fields.AutoValidateCertificate"] = "Auto validate certificates",
                ["Plugins.Tax.Avalara.Fields.AutoValidateCertificate.Hint"] = "Determine whether the new certificates are automatically valid, this allows your users to make exempt purchases right away, otherwise a user is not treated as exempt until you validate the document.",
                ["Plugins.Tax.Avalara.Fields.UserRoles"] = "Limited to user roles",
                ["Plugins.Tax.Avalara.Fields.UserRoles.Hint"] = "Select user roles for which exemption certificates will be available. Leave empty if you want this feature to be available to all users.",
                ["Plugins.Tax.Avalara.Fields.DisplayNoValidCertificatesMessage"] = "Display 'No valid certificates' message",
                ["Plugins.Tax.Avalara.Fields.DisplayNoValidCertificatesMessage.Hint"] = "Determine whether to display a message that there are no valid certificates for the user on the order confirmation page.",
                ["Plugins.Tax.Avalara.Fields.EnableCertificates"] = "Enable exemption certificates",
                ["Plugins.Tax.Avalara.Fields.EnableCertificates.Hint"] = "Determine whether to enable this feature. In this case, a new page will be added in the account section, so users can manage their exemption certificates before making a purchase.",
                ["Plugins.Tax.Avalara.Fields.EnableCertificates.Warning"] = "To use this feature, you need the following information from users: name, country, state, city, address, postal code. Ensure that the appropriate User form fields are enabled under <a href=\"{0}\" target=\"_blank\">User settings</a>",
                ["Plugins.Tax.Avalara.TestTax.Button"] = "Submit",
            }, languageId);

            _localizationService.DeleteLocaleResources(new List<string>
            {
                "Enums.TvProg.Plugin.Tax.Avalara.Domain.LogType.Create",
                "Enums.TvProg.Plugin.Tax.Avalara.Domain.LogType.CreateResponse",
                "Enums.TvProg.Plugin.Tax.Avalara.Domain.LogType.Error",
                "Enums.TvProg.Plugin.Tax.Avalara.Domain.LogType.Refund",
                "Enums.TvProg.Plugin.Tax.Avalara.Domain.LogType.RefundResponse",
                "Enums.TvProg.Plugin.Tax.Avalara.Domain.LogType.Void",
                "Enums.TvProg.Plugin.Tax.Avalara.Domain.LogType.VoidResponse",
                "Plugins.Tax.Avalara.VerifyCredentials",
                "Plugins.Tax.Avalara.VerifyCredentials.Declined",
                "Plugins.Tax.Avalara.VerifyCredentials.Verified",
            });

            //settings
            if (!_settingService.SettingExists(_avalaraTaxSettings, settings => settings.CompanyId))
                _avalaraTaxSettings.CompanyId = null;
            if (!_settingService.SettingExists(_avalaraTaxSettings, settings => settings.EnableCertificates))
                _avalaraTaxSettings.EnableCertificates = false;
            if (!_settingService.SettingExists(_avalaraTaxSettings, settings => settings.AutoValidateCertificate))
                _avalaraTaxSettings.AutoValidateCertificate = true;
            if (!_settingService.SettingExists(_avalaraTaxSettings, settings => settings.AllowEditUser))
                _avalaraTaxSettings.AllowEditUser = true;
            if (!_settingService.SettingExists(_avalaraTaxSettings, settings => settings.DisplayNoValidCertificatesMessage))
                _avalaraTaxSettings.DisplayNoValidCertificatesMessage = true;
            if (!_settingService.SettingExists(_avalaraTaxSettings, settings => settings.UserRoleIds))
                _avalaraTaxSettings.UserRoleIds = null;
            if (!_settingService.SettingExists(_avalaraTaxSettings, settings => settings.PreviewCertificate))
                _avalaraTaxSettings.PreviewCertificate = false;
            if (!_settingService.SettingExists(_avalaraTaxSettings, settings => settings.UploadOnly))
                _avalaraTaxSettings.UploadOnly = false;
            if (!_settingService.SettingExists(_avalaraTaxSettings, settings => settings.FillOnly))
                _avalaraTaxSettings.FillOnly = false;
            _settingService.SaveSetting(_avalaraTaxSettings);
        }

        /// <summary>
        /// Collects the DOWN migration expressions
        /// </summary>
        public override void Down()
        {
            //nothing
        }

        #endregion
    }
}