using System.ComponentModel.DataAnnotations;
using TvProgViewer.Web.Framework.Models;
using TvProgViewer.Web.Framework.Mvc.ModelBinding;

namespace TvProgViewer.WebUI.Areas.Admin.Models.Settings
{
    /// <summary>
    /// Represents a user settings model
    /// </summary>
    public partial record UserSettingsModel : BaseTvProgModel, ISettingsModel
    {
        #region Properties

        public int ActiveStoreScopeConfiguration { get; set; }

        [TvProgResourceDisplayName("Admin.Configuration.Settings.UserUser.UsernamesEnabled")]
        public bool UsernamesEnabled { get; set; }

        [TvProgResourceDisplayName("Admin.Configuration.Settings.UserUser.AllowUsersToChangeUsernames")]
        public bool AllowUsersToChangeUsernames { get; set; }

        [TvProgResourceDisplayName("Admin.Configuration.Settings.UserUser.CheckUsernameAvailabilityEnabled")]
        public bool CheckUsernameAvailabilityEnabled { get; set; }

        [TvProgResourceDisplayName("Admin.Configuration.Settings.UserUser.UsernameValidationEnabled")]
        public bool UsernameValidationEnabled { get; set; }

        [TvProgResourceDisplayName("Admin.Configuration.Settings.UserUser.UsernameValidationUseRegex")]
        public bool UsernameValidationUseRegex { get; set; }

        [TvProgResourceDisplayName("Admin.Configuration.Settings.UserUser.UsernameValidationRule")]
        public string UsernameValidationRule { get; set; }       

        [TvProgResourceDisplayName("Admin.Configuration.Settings.UserUser.UserRegistrationType")]
        public int UserRegistrationType { get; set; }

        [TvProgResourceDisplayName("Admin.Configuration.Settings.UserUser.AllowUsersToUploadAvatars")]
        public bool AllowUsersToUploadAvatars { get; set; }

        [TvProgResourceDisplayName("Admin.Configuration.Settings.UserUser.DefaultAvatarEnabled")]
        public bool DefaultAvatarEnabled { get; set; }

        [TvProgResourceDisplayName("Admin.Configuration.Settings.UserUser.ShowUsersLocation")]
        public bool ShowUsersLocation { get; set; }

        [TvProgResourceDisplayName("Admin.Configuration.Settings.UserUser.ShowUsersJoinDate")]
        public bool ShowUsersJoinDate { get; set; }

        [TvProgResourceDisplayName("Admin.Configuration.Settings.UserUser.AllowViewingProfiles")]
        public bool AllowViewingProfiles { get; set; }

        [TvProgResourceDisplayName("Admin.Configuration.Settings.UserUser.NotifyNewUserRegistration")]
        public bool NotifyNewUserRegistration { get; set; }

        [TvProgResourceDisplayName("Admin.Configuration.Settings.UserUser.RequireRegistrationForDownloadableTvChannels")]
        public bool RequireRegistrationForDownloadableTvChannels { get; set; }

        [TvProgResourceDisplayName("Admin.Configuration.Settings.UserUser.AllowUsersToCheckGiftCardBalance")]
        public bool AllowUsersToCheckGiftCardBalance { get; set; }

        [TvProgResourceDisplayName("Admin.Configuration.Settings.UserUser.HideDownloadableTvChannelsTab")]
        public bool HideDownloadableTvChannelsTab { get; set; }

        [TvProgResourceDisplayName("Admin.Configuration.Settings.UserUser.HideBackInStockSubscriptionsTab")]
        public bool HideBackInStockSubscriptionsTab { get; set; }

        [TvProgResourceDisplayName("Admin.Configuration.Settings.UserUser.UserNameFormat")]
        public int UserNameFormat { get; set; }

        [TvProgResourceDisplayName("Admin.Configuration.Settings.UserUser.PasswordMinLength")]
        public int PasswordMinLength { get; set; }

        [TvProgResourceDisplayName("Admin.Configuration.Settings.UserUser.PasswordRequireLowercase")]
        public bool PasswordRequireLowercase { get; set; }

        [TvProgResourceDisplayName("Admin.Configuration.Settings.UserUser.PasswordRequireUppercase")]
        public bool PasswordRequireUppercase { get; set; }

        [TvProgResourceDisplayName("Admin.Configuration.Settings.UserUser.PasswordRequireNonAlphanumeric")]
        public bool PasswordRequireNonAlphanumeric { get; set; }

        [TvProgResourceDisplayName("Admin.Configuration.Settings.UserUser.PasswordRequireDigit")]
        public bool PasswordRequireDigit { get; set; }

        [TvProgResourceDisplayName("Admin.Configuration.Settings.UserUser.UnduplicatedPasswordsNumber")]
        public int UnduplicatedPasswordsNumber { get; set; }

        [TvProgResourceDisplayName("Admin.Configuration.Settings.UserUser.PasswordRecoveryLinkDaysValid")]
        public int PasswordRecoveryLinkDaysValid { get; set; }

        [TvProgResourceDisplayName("Admin.Configuration.Settings.UserUser.DefaultPasswordFormat")]
        public int DefaultPasswordFormat { get; set; }

        [TvProgResourceDisplayName("Admin.Configuration.Settings.UserUser.PasswordLifetime")]
        public int PasswordLifetime { get; set; }

        [TvProgResourceDisplayName("Admin.Configuration.Settings.UserUser.FailedPasswordAllowedAttempts")]
        public int FailedPasswordAllowedAttempts { get; set; }

        [TvProgResourceDisplayName("Admin.Configuration.Settings.UserUser.FailedPasswordLockoutMinutes")]
        public int FailedPasswordLockoutMinutes { get; set; }

        [TvProgResourceDisplayName("Admin.Configuration.Settings.UserUser.NewsletterEnabled")]
        public bool NewsletterEnabled { get; set; }

        [TvProgResourceDisplayName("Admin.Configuration.Settings.UserUser.NewsletterTickedByDefault")]
        public bool NewsletterTickedByDefault { get; set; }

        [TvProgResourceDisplayName("Admin.Configuration.Settings.UserUser.HideNewsletterBlock")]
        public bool HideNewsletterBlock { get; set; }

        [TvProgResourceDisplayName("Admin.Configuration.Settings.UserUser.NewsletterBlockAllowToUnsubscribe")]
        public bool NewsletterBlockAllowToUnsubscribe { get; set; }

        [TvProgResourceDisplayName("Admin.Configuration.Settings.UserUser.StoreLastVisitedPage")]
        public bool StoreLastVisitedPage { get; set; }

        [TvProgResourceDisplayName("Admin.Configuration.Settings.UserUser.StoreIpAddresses")]
        public bool StoreIpAddresses { get; set; }

        [TvProgResourceDisplayName("Admin.Configuration.Settings.UserUser.EnteringEmailTwice")]
        public bool EnteringEmailTwice { get; set; }        

        [TvProgResourceDisplayName("Admin.Configuration.Settings.UserUser.GenderEnabled")]
        public bool GenderEnabled { get; set; }

        [TvProgResourceDisplayName("Admin.Configuration.Settings.UserUser.FirstNameEnabled")]
        public bool FirstNameEnabled { get; set; }

        [TvProgResourceDisplayName("Admin.Configuration.Settings.UserUser.FirstNameRequired")]
        public bool FirstNameRequired { get; set; }

        [TvProgResourceDisplayName("Admin.Configuration.Settings.UserUser.LastNameEnabled")]
        public bool LastNameEnabled { get; set; }

        [TvProgResourceDisplayName("Admin.Configuration.Settings.UserUser.LastNameRequired")]
        public bool LastNameRequired { get; set; }

        [TvProgResourceDisplayName("Admin.Configuration.Settings.UserUser.MiddleNameEnabled")]
        public bool MiddleNameEnabled { get; set; }

        [TvProgResourceDisplayName("Admin.Configuration.Settings.UserUser.BirthDateEnabled")]
        public bool BirthDateEnabled { get; set; }

        [TvProgResourceDisplayName("Admin.Configuration.Settings.UserUser.BirthDateRequired")]
        public bool BirthDateRequired { get; set; }

        [TvProgResourceDisplayName("Admin.Configuration.Settings.UserUser.BirthDateMinimumAge")]
        [UIHint("Int32Nullable")]
        public int? BirthDateMinimumAge { get; set; }

        [TvProgResourceDisplayName("Admin.Configuration.Settings.UserUser.CompanyEnabled")]
        public bool CompanyEnabled { get; set; }

        [TvProgResourceDisplayName("Admin.Configuration.Settings.UserUser.CompanyRequired")]
        public bool CompanyRequired { get; set; }

        [TvProgResourceDisplayName("Admin.Configuration.Settings.UserUser.StreetAddressEnabled")]
        public bool StreetAddressEnabled { get; set; }

        [TvProgResourceDisplayName("Admin.Configuration.Settings.UserUser.StreetAddressRequired")]
        public bool StreetAddressRequired { get; set; }

        [TvProgResourceDisplayName("Admin.Configuration.Settings.UserUser.StreetAddress2Enabled")]
        public bool StreetAddress2Enabled { get; set; }

        [TvProgResourceDisplayName("Admin.Configuration.Settings.UserUser.StreetAddress2Required")]
        public bool StreetAddress2Required { get; set; }

        [TvProgResourceDisplayName("Admin.Configuration.Settings.UserUser.ZipPostalCodeEnabled")]
        public bool ZipPostalCodeEnabled { get; set; }

        [TvProgResourceDisplayName("Admin.Configuration.Settings.UserUser.ZipPostalCodeRequired")]
        public bool ZipPostalCodeRequired { get; set; }

        [TvProgResourceDisplayName("Admin.Configuration.Settings.UserUser.CityEnabled")]
        public bool CityEnabled { get; set; }

        [TvProgResourceDisplayName("Admin.Configuration.Settings.UserUser.CityRequired")]
        public bool CityRequired { get; set; }

        [TvProgResourceDisplayName("Admin.Configuration.Settings.UserUser.CountyEnabled")]
        public bool CountyEnabled { get; set; }

        [TvProgResourceDisplayName("Admin.Configuration.Settings.UserUser.CountyRequired")]
        public bool CountyRequired { get; set; }

        [TvProgResourceDisplayName("Admin.Configuration.Settings.UserUser.CountryEnabled")]
        public bool CountryEnabled { get; set; }

        [TvProgResourceDisplayName("Admin.Configuration.Settings.UserUser.CountryRequired")]
        public bool CountryRequired { get; set; }

        [TvProgResourceDisplayName("Admin.Configuration.Settings.UserUser.StateProvinceEnabled")]
        public bool StateProvinceEnabled { get; set; }

        [TvProgResourceDisplayName("Admin.Configuration.Settings.UserUser.StateProvinceRequired")]
        public bool StateProvinceRequired { get; set; }

        [TvProgResourceDisplayName("Admin.Configuration.Settings.UserUser.SmartPhoneEnabled")]
        public bool SmartPhoneEnabled { get; set; }

        [TvProgResourceDisplayName("Admin.Configuration.Settings.UserUser.SmartPhoneRequired")]
        public bool SmartPhoneRequired { get; set; }

        [TvProgResourceDisplayName("Admin.Configuration.Settings.UserUser.PhoneNumberValidationEnabled")]
        public bool PhoneNumberValidationEnabled { get; set; }

        [TvProgResourceDisplayName("Admin.Configuration.Settings.UserUser.PhoneNumberValidationUseRegex")]
        public bool PhoneNumberValidationUseRegex { get; set; }

        [TvProgResourceDisplayName("Admin.Configuration.Settings.UserUser.PhoneNumberValidationRule")]
        public string PhoneNumberValidationRule { get; set; }

        [TvProgResourceDisplayName("Admin.Configuration.Settings.UserUser.FaxEnabled")]
        public bool FaxEnabled { get; set; }

        [TvProgResourceDisplayName("Admin.Configuration.Settings.UserUser.FaxRequired")]
        public bool FaxRequired { get; set; }

        [TvProgResourceDisplayName("Admin.Configuration.Settings.UserUser.AcceptPrivacyPolicyEnabled")]
        public bool AcceptPrivacyPolicyEnabled { get; set; }

        [TvProgResourceDisplayName("Admin.Users.Users.Fields.AcceptPersonalDataAggreement")]
        public bool AcceptPersonalDataAggreement { get; set; }

        [TvProgResourceDisplayName("Admin.Configuration.Settings.UserUser.AcceptPersonalDataAgreementEnabled")]
        public bool AcceptPersonalDataAgreementEnabled { get; set; }
        #endregion
    }
}