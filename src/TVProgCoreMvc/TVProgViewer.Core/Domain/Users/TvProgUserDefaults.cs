﻿namespace TVProgViewer.Core.Domain.Users
{
    /// <summary>
    /// Represents default values related to Users data
    /// </summary>
    public static partial class TvProgUserDefaults
    {
        #region System User roles

        /// <summary>
        /// Gets a system name of 'administrators' User role
        /// </summary>
        public static string AdministratorsRoleName => "Administrators";

        /// <summary>
        /// Gets a system name of 'forum moderators' User role
        /// </summary>
        public static string ForumModeratorsRoleName => "ForumModerators";

        /// <summary>
        /// Gets a system name of 'registered' User role
        /// </summary>
        public static string RegisteredRoleName => "Registered";

        /// <summary>
        /// Gets a system name of 'guests' User role
        /// </summary>
        public static string GuestsRoleName => "Guests";

        /// <summary>
        /// Gets a system name of 'vendors' User role
        /// </summary>
        public static string VendorsRoleName => "Vendors";

        #endregion

        #region System Users

        /// <summary>
        /// Gets a system name of 'search engine' User object
        /// </summary>
        public static string SearchEngineUserName => "SearchEngine";

        /// <summary>
        /// Gets a system name of 'background task' User object
        /// </summary>
        public static string BackgroundTaskUserName => "BackgroundTask";

        #endregion

        #region User attributes

        /// <summary>
        /// Gets a name of generic attribute to store the value of 'FirstName'
        /// </summary>
        public static string FirstNameAttribute => "FirstName";

        /// <summary>
        /// Gets a name of generic attribute to store the value of 'LastName'
        /// </summary>
        public static string LastNameAttribute => "LastName";

        /// <summary>
        /// Gets a name of generic attribute to store the value of 'Gender'
        /// </summary>
        public static string GenderAttribute => "Gender";

        /// <summary>
        /// Gets a name of generic attribute to store the value of 'DateOfBirth'
        /// </summary>
        public static string DateOfBirthAttribute => "DateOfBirth";

        /// <summary>
        /// Gets a name of generic attribute to store the value of 'Company'
        /// </summary>
        public static string CompanyAttribute => "Company";

        /// <summary>
        /// Gets a name of generic attribute to store the value of 'StreetAddress'
        /// </summary>
        public static string StreetAddressAttribute => "StreetAddress";

        /// <summary>
        /// Gets a name of generic attribute to store the value of 'StreetAddress2'
        /// </summary>
        public static string StreetAddress2Attribute => "StreetAddress2";

        /// <summary>
        /// Gets a name of generic attribute to store the value of 'ZipPostalCode'
        /// </summary>
        public static string ZipPostalCodeAttribute => "ZipPostalCode";

        /// <summary>
        /// Gets a name of generic attribute to store the value of 'City'
        /// </summary>
        public static string CityAttribute => "City";

        /// <summary>
        /// Gets a name of generic attribute to store the value of 'County'
        /// </summary>
        public static string CountyAttribute => "County";

        /// <summary>
        /// Gets a name of generic attribute to store the value of 'CountryId'
        /// </summary>
        public static string CountryIdAttribute => "CountryId";

        /// <summary>
        /// Gets a name of generic attribute to store the value of 'StateProvinceId'
        /// </summary>
        public static string StateProvinceIdAttribute => "StateProvinceId";

        /// <summary>
        /// Gets a name of generic attribute to store the value of 'Phone'
        /// </summary>
        public static string PhoneAttribute => "Phone";

        /// <summary>
        /// Gets a name of generic attribute to store the value of 'Fax'
        /// </summary>
        public static string FaxAttribute => "Fax";

        /// <summary>
        /// Gets a name of generic attribute to store the value of 'VatNumber'
        /// </summary>
        public static string VatNumberAttribute => "VatNumber";

        /// <summary>
        /// Gets a name of generic attribute to store the value of 'VatNumberStatusId'
        /// </summary>
        public static string VatNumberStatusIdAttribute => "VatNumberStatusId";

        /// <summary>
        /// Gets a name of generic attribute to store the value of 'TimeZoneId'
        /// </summary>
        public static string TimeZoneIdAttribute => "TimeZoneId";

        /// <summary>
        /// Gets a name of generic attribute to store the value of 'CustomUserAttributes'
        /// </summary>
        public static string CustomUserAttributes => "CustomUserAttributes";

        /// <summary>
        /// Gets a name of generic attribute to store the value of 'DiscountCouponCode'
        /// </summary>
        public static string DiscountCouponCodeAttribute => "DiscountCouponCode";

        /// <summary>
        /// Gets a name of generic attribute to store the value of 'GiftCardCouponCodes'
        /// </summary>
        public static string GiftCardCouponCodesAttribute => "GiftCardCouponCodes";

        /// <summary>
        /// Gets a name of generic attribute to store the value of 'AvatarPictureId'
        /// </summary>
        public static string AvatarPictureIdAttribute => "AvatarPictureId";

        /// <summary>
        /// Gets a name of generic attribute to store the value of 'ForumPostCount'
        /// </summary>
        public static string ForumPostCountAttribute => "ForumPostCount";

        /// <summary>
        /// Gets a name of generic attribute to store the value of 'Signature'
        /// </summary>
        public static string SignatureAttribute => "Signature";

        /// <summary>
        /// Gets a name of generic attribute to store the value of 'PasswordRecoveryToken'
        /// </summary>
        public static string PasswordRecoveryTokenAttribute => "PasswordRecoveryToken";

        /// <summary>
        /// Gets a name of generic attribute to store the value of 'PasswordRecoveryTokenDateGenerated'
        /// </summary>
        public static string PasswordRecoveryTokenDateGeneratedAttribute => "PasswordRecoveryTokenDateGenerated";

        /// <summary>
        /// Gets a name of generic attribute to store the value of 'AccountActivationToken'
        /// </summary>
        public static string AccountActivationTokenAttribute => "AccountActivationToken";

        /// <summary>
        /// Gets a name of generic attribute to store the value of 'EmailRevalidationToken'
        /// </summary>
        public static string EmailRevalidationTokenAttribute => "EmailRevalidationToken";

        /// <summary>
        /// Gets a name of generic attribute to store the value of 'LastVisitedPage'
        /// </summary>
        public static string LastVisitedPageAttribute => "LastVisitedPage";

        /// <summary>
        /// Gets a name of generic attribute to store the value of 'ImpersonatedUserId'
        /// </summary>
        public static string ImpersonatedUserIdAttribute => "ImpersonatedUserId";

        /// <summary>
        /// Gets a name of generic attribute to store the value of 'AdminAreaStoreScopeConfiguration'
        /// </summary>
        public static string AdminAreaStoreScopeConfigurationAttribute => "AdminAreaStoreScopeConfiguration";

        /// <summary>
        /// Gets a name of generic attribute to store the value of 'CurrencyId'
        /// </summary>
        public static string CurrencyIdAttribute => "CurrencyId";

        /// <summary>
        /// Получает название списка атрибутов в Систему со значением 'ProviderId'
        /// </summary>
        public static string ProviderIdAttribute => "ProviderId";

        /// <summary>
        /// Получает название списка атрибутов в Систему со значением 'TypeProgId'
        /// </summary>
        public static string TypeProgIdAttribute => "TypeProgId";

        /// <summary>
        /// Получает название списка атрибутов в Систему со значением 'Category'
        /// </summary>
        public static string CategoryAttribute => "Category";

        /// <summary>
        /// Gets a name of generic attribute to store the value of 'LanguageId'
        /// </summary>
        public static string LanguageIdAttribute => "LanguageId";

        /// <summary>
        /// Gets a name of generic attribute to store the value of 'LanguageAutomaticallyDetected'
        /// </summary>
        public static string LanguageAutomaticallyDetectedAttribute => "LanguageAutomaticallyDetected";

        /// <summary>
        /// Gets a name of generic attribute to store the value of 'SelectedPaymentMethod'
        /// </summary>
        public static string SelectedPaymentMethodAttribute => "SelectedPaymentMethod";

        /// <summary>
        /// Gets a name of generic attribute to store the value of 'SelectedShippingOption'
        /// </summary>
        public static string SelectedShippingOptionAttribute => "SelectedShippingOption";

        /// <summary>
        /// Gets a name of generic attribute to store the value of 'SelectedPickupPoint'
        /// </summary>
        public static string SelectedPickupPointAttribute => "SelectedPickupPoint";

        /// <summary>
        /// Gets a name of generic attribute to store the value of 'CheckoutAttributes'
        /// </summary>
        public static string CheckoutAttributes => "CheckoutAttributes";

        /// <summary>
        /// Gets a name of generic attribute to store the value of 'OfferedShippingOptions'
        /// </summary>
        public static string OfferedShippingOptionsAttribute => "OfferedShippingOptions";

        /// <summary>
        /// Gets a name of generic attribute to store the value of 'LastContinueShoppingPage'
        /// </summary>
        public static string LastContinueShoppingPageAttribute => "LastContinueShoppingPage";

        /// <summary>
        /// Gets a name of generic attribute to store the value of 'NotifiedAboutNewPrivateMessages'
        /// </summary>
        public static string NotifiedAboutNewPrivateMessagesAttribute => "NotifiedAboutNewPrivateMessages";

        /// <summary>
        /// Gets a name of generic attribute to store the value of 'WorkingThemeName'
        /// </summary>
        public static string WorkingThemeNameAttribute => "WorkingThemeName";

        /// <summary>
        /// Gets a name of generic attribute to store the value of 'TaxDisplayTypeId'
        /// </summary>
        public static string TaxDisplayTypeIdAttribute => "TaxDisplayTypeId";

        /// <summary>
        /// Gets a name of generic attribute to store the value of 'UseRewardPointsDuringCheckout'
        /// </summary>
        public static string UseRewardPointsDuringCheckoutAttribute => "UseRewardPointsDuringCheckout";

        /// <summary>
        /// Gets a name of generic attribute to store the value of 'EuCookieLawAccepted'
        /// </summary>
        public static string EuCookieLawAcceptedAttribute => "EuCookieLaw.Accepted";

        /// <summary>
        /// Gets a name of generic attribute to store the value of 'SelectedMultiFactorAuthProvider'
        /// </summary>
        public static string SelectedMultiFactorAuthenticationProviderAttribute => "SelectedMultiFactorAuthProvider";

        /// <summary>
        /// Gets a name of session key
        /// </summary>
        public static string UserMultiFactorAuthenticationInfo => "UserMultiFactorAuthenticationInfo";
        #endregion
    }
}