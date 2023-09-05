using System.Collections.Generic;
using FluentMigrator;
using TvProgViewer.Data;
using TvProgViewer.Data.Migrations;
using TvProgViewer.Services.Configuration;
using TvProgViewer.Services.Localization;
using TvProgViewer.Web.Framework.Extensions;

namespace TvProgViewer.Plugin.Payments.PayPalViewer.Data
{
    [TvProgMigration("2021-12-01 00:00:00", "Payments.PayPalViewer 1.07. Add Pay Later message", MigrationProcessType.Update)]
    internal class PayLaterMessageMigration : MigrationBase
    {
        #region Fields

        private readonly PayPalViewerSettings _payPalViewerSettings;
        private readonly ILanguageService _languageService;
        private readonly ILocalizationService _localizationService;
        private readonly ISettingService _settingService;

        #endregion

        #region Ctor

        public PayLaterMessageMigration(PayPalViewerSettings payPalViewerSettings,
            ILanguageService languageService,
            ILocalizationService localizationService,
            ISettingService settingService)
        {
            _payPalViewerSettings = payPalViewerSettings;
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
                ["Plugins.Payments.PayPalViewer.Fields.DisplayPayLaterMessages"] = "Display Pay Later messages",
                ["Plugins.Payments.PayPalViewer.Fields.DisplayPayLaterMessages.Hint"] = "Determine whether to display Pay Later messages. This message displays how much the user pays in four payments. The message will be shown next to the PayPal buttons.",
            }, languageId);


            //settings
            if (!_settingService.SettingExists(_payPalViewerSettings, settings => settings.DisplayPayLaterMessages))
                _payPalViewerSettings.DisplayPayLaterMessages = false;
            
            _settingService.SaveSetting(_payPalViewerSettings);
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
