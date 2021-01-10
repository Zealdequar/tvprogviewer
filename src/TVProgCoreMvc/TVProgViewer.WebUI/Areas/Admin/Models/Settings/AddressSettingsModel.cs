using TVProgViewer.Web.Framework.Models;
using TVProgViewer.Web.Framework.Mvc.ModelBinding;

namespace TVProgViewer.WebUI.Areas.Admin.Models.Settings
{
    /// <summary>
    /// Represents an address settings model
    /// </summary>
    public partial record AddressSettingsModel : BaseTvProgModel, ISettingsModel
    {
        #region Properties

        public int ActiveStoreScopeConfiguration { get; set; }

        [TvProgResourceDisplayName("Admin.Configuration.Settings.UserUser.AddressFormFields.CompanyEnabled")]
        public bool CompanyEnabled { get; set; }

        [TvProgResourceDisplayName("Admin.Configuration.Settings.UserUser.AddressFormFields.CompanyRequired")]
        public bool CompanyRequired { get; set; }

        [TvProgResourceDisplayName("Admin.Configuration.Settings.UserUser.AddressFormFields.StreetAddressEnabled")]
        public bool StreetAddressEnabled { get; set; }

        [TvProgResourceDisplayName("Admin.Configuration.Settings.UserUser.AddressFormFields.StreetAddressRequired")]
        public bool StreetAddressRequired { get; set; }

        [TvProgResourceDisplayName("Admin.Configuration.Settings.UserUser.AddressFormFields.StreetAddress2Enabled")]
        public bool StreetAddress2Enabled { get; set; }

        [TvProgResourceDisplayName("Admin.Configuration.Settings.UserUser.AddressFormFields.StreetAddress2Required")]
        public bool StreetAddress2Required { get; set; }

        [TvProgResourceDisplayName("Admin.Configuration.Settings.UserUser.AddressFormFields.ZipPostalCodeEnabled")]
        public bool ZipPostalCodeEnabled { get; set; }

        [TvProgResourceDisplayName("Admin.Configuration.Settings.UserUser.AddressFormFields.ZipPostalCodeRequired")]
        public bool ZipPostalCodeRequired { get; set; }

        [TvProgResourceDisplayName("Admin.Configuration.Settings.UserUser.AddressFormFields.CityEnabled")]
        public bool CityEnabled { get; set; }

        [TvProgResourceDisplayName("Admin.Configuration.Settings.UserUser.AddressFormFields.CityRequired")]
        public bool CityRequired { get; set; }

        [TvProgResourceDisplayName("Admin.Configuration.Settings.UserUser.AddressFormFields.CountyEnabled")]
        public bool CountyEnabled { get; set; }

        [TvProgResourceDisplayName("Admin.Configuration.Settings.UserUser.AddressFormFields.CountyRequired")]
        public bool CountyRequired { get; set; }

        [TvProgResourceDisplayName("Admin.Configuration.Settings.UserUser.AddressFormFields.CountryEnabled")]
        public bool CountryEnabled { get; set; }

        [TvProgResourceDisplayName("Admin.Configuration.Settings.UserUser.AddressFormFields.StateProvinceEnabled")]
        public bool StateProvinceEnabled { get; set; }

        [TvProgResourceDisplayName("Admin.Configuration.Settings.UserUser.AddressFormFields.PhoneEnabled")]
        public bool PhoneEnabled { get; set; }

        [TvProgResourceDisplayName("Admin.Configuration.Settings.UserUser.AddressFormFields.PhoneRequired")]
        public bool PhoneRequired { get; set; }

        [TvProgResourceDisplayName("Admin.Configuration.Settings.UserUser.AddressFormFields.FaxEnabled")]
        public bool FaxEnabled { get; set; }

        [TvProgResourceDisplayName("Admin.Configuration.Settings.UserUser.AddressFormFields.FaxRequired")]
        public bool FaxRequired { get; set; }

        #endregion
    }
}