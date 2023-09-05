using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;
using TvProgViewer.Core.Domain.Catalog;
using TvProgViewer.Web.Framework.Mvc.ModelBinding;
using TvProgViewer.Web.Framework.Models;

namespace TvProgViewer.WebUI.Areas.Admin.Models.Users
{
    /// <summary>
    /// Represents a user model
    /// </summary>
    public partial record UserModel : BaseTvProgEntityModel, IAclSupportedModel
    {
        #region Ctor

        public UserModel()
        {
            AvailableTimeZones = new List<SelectListItem>();
            SendEmail = new SendEmailModel() { SendImmediately = true };
            SendPm = new SendPmModel();

            SelectedUserRoleIds = new List<int>();
            AvailableUserRoles = new List<SelectListItem>();

            AvailableCountries = new List<SelectListItem>();
            AvailableStates = new List<SelectListItem>();
            AvailableVendors = new List<SelectListItem>();
            UserAttributes = new List<UserAttributeModel>();
            AvailableNewsletterSubscriptionStores = new List<SelectListItem>();
            SelectedNewsletterSubscriptionStoreIds = new List<int>();
            AddRewardPoints = new AddRewardPointsToUserModel();
            UserRewardPointsSearchModel = new UserRewardPointsSearchModel();
            UserAddressSearchModel = new UserAddressSearchModel();
            UserOrderSearchModel = new UserOrderSearchModel();
            UserShoppingCartSearchModel = new UserShoppingCartSearchModel();
            UserActivityLogSearchModel = new UserActivityLogSearchModel();
            UserBackInStockSubscriptionSearchModel = new UserBackInStockSubscriptionSearchModel();
            UserAssociatedExternalAuthRecordsSearchModel = new UserAssociatedExternalAuthRecordsSearchModel();
        }

        #endregion

        #region Properties

        public bool UsernamesEnabled { get; set; }

        [TvProgResourceDisplayName("Admin.Users.Users.Fields.Username")]
        public string Username { get; set; }

        [DataType(DataType.EmailAddress)]
        [TvProgResourceDisplayName("Admin.Users.Users.Fields.Email")]
        public string Email { get; set; }

        [TvProgResourceDisplayName("Admin.Users.Users.Fields.Password")]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [TvProgResourceDisplayName("Admin.Users.Users.Fields.Vendor")]
        public int VendorId { get; set; }

        public IList<SelectListItem> AvailableVendors { get; set; }

        //form fields & properties
        public bool GenderEnabled { get; set; }

        [TvProgResourceDisplayName("Admin.Users.Users.Fields.Gender")]
        public string Gender { get; set; }

        public bool FirstNameEnabled { get; set; }
        [TvProgResourceDisplayName("Admin.Users.Users.Fields.FirstName")]
        public string FirstName { get; set; }

        public bool LastNameEnabled { get; set; }
        [TvProgResourceDisplayName("Admin.Users.Users.Fields.LastName")]
        public string LastName { get; set; }

        [TvProgResourceDisplayName("Admin.Users.Users.Fields.FullName")]
        public string FullName { get; set; }

        public bool BirthDateEnabled { get; set; }

        [UIHint("DateNullable")]
        [TvProgResourceDisplayName("Admin.Users.Users.Fields.BirthDate")]
        public DateTime? BirthDate { get; set; }

        public bool CompanyEnabled { get; set; }

        [TvProgResourceDisplayName("Admin.Users.Users.Fields.Company")]
        public string Company { get; set; }

        public bool StreetAddressEnabled { get; set; }

        [TvProgResourceDisplayName("Admin.Users.Users.Fields.StreetAddress")]
        public string StreetAddress { get; set; }

        public bool StreetAddress2Enabled { get; set; }

        [TvProgResourceDisplayName("Admin.Users.Users.Fields.StreetAddress2")]
        public string StreetAddress2 { get; set; }

        public bool ZipPostalCodeEnabled { get; set; }

        [TvProgResourceDisplayName("Admin.Users.Users.Fields.ZipPostalCode")]
        public string ZipPostalCode { get; set; }

        public bool CityEnabled { get; set; }

        [TvProgResourceDisplayName("Admin.Users.Users.Fields.City")]
        public string City { get; set; }

        public bool CountyEnabled { get; set; }

        [TvProgResourceDisplayName("Admin.Users.Users.Fields.County")]
        public string County { get; set; }

        public bool CountryEnabled { get; set; }

        [TvProgResourceDisplayName("Admin.Users.Users.Fields.Country")]
        public int CountryId { get; set; }

        public IList<SelectListItem> AvailableCountries { get; set; }

        public bool StateProvinceEnabled { get; set; }

        [TvProgResourceDisplayName("Admin.Users.Users.Fields.StateProvince")]
        public int StateProvinceId { get; set; }

        public IList<SelectListItem> AvailableStates { get; set; }

        public bool SmartPhoneEnabled { get; set; }

        [DataType(DataType.PhoneNumber)]
        [TvProgResourceDisplayName("Admin.Users.Users.Fields.SmartPhone")]
        public string SmartPhone { get; set; }

        public bool FaxEnabled { get; set; }

        [DataType(DataType.PhoneNumber)]
        [TvProgResourceDisplayName("Admin.Users.Users.Fields.Fax")]
        public string Fax { get; set; }

        public List<UserAttributeModel> UserAttributes { get; set; }

        [TvProgResourceDisplayName("Admin.Users.Users.Fields.RegisteredInStore")]
        public string RegisteredInStore { get; set; }

        [TvProgResourceDisplayName("Admin.Users.Users.Fields.AdminComment")]
        public string AdminComment { get; set; }

        [TvProgResourceDisplayName("Admin.Users.Users.Fields.IsTaxExempt")]
        public bool IsTaxExempt { get; set; }

        [TvProgResourceDisplayName("Admin.Users.Users.Fields.Active")]
        public bool Active { get; set; }

        [TvProgResourceDisplayName("Admin.Users.Users.Fields.Affiliate")]
        public int AffiliateId { get; set; }

        [TvProgResourceDisplayName("Admin.Users.Users.Fields.Affiliate")]
        public string AffiliateName { get; set; }

        //time zone
        [TvProgResourceDisplayName("Admin.Users.Users.Fields.GmtZone")]
        public string GmtZone { get; set; }

        public bool AllowUsersToSetTimeZone { get; set; }

        public IList<SelectListItem> AvailableTimeZones { get; set; }

        //EU VAT
        [TvProgResourceDisplayName("Admin.Users.Users.Fields.VatNumber")]
        public string VatNumber { get; set; }

        public string VatNumberStatusNote { get; set; }

        public bool DisplayVatNumber { get; set; }

        public bool DisplayRegisteredInStore { get; set; }

        //registration date
        [TvProgResourceDisplayName("Admin.Users.Users.Fields.CreatedOn")]
        public DateTime CreatedOn { get; set; }

        [TvProgResourceDisplayName("Admin.Users.Users.Fields.LastActivityDate")]
        public DateTime LastActivityDate { get; set; }

        //IP address
        [TvProgResourceDisplayName("Admin.Users.Users.Fields.IPAddress")]
        public string LastIpAddress { get; set; }

        [TvProgResourceDisplayName("Admin.Users.Users.Fields.LastVisitedPage")]
        public string LastVisitedPage { get; set; }

        //user roles
        [TvProgResourceDisplayName("Admin.Users.Users.Fields.UserRoles")]
        public string UserRoleNames { get; set; }

        //binding with multi-factor authentication provider
        [TvProgResourceDisplayName("Admin.Users.Users.Fields.MultiFactorAuthenticationProvider")]
        public string MultiFactorAuthenticationProvider { get; set; }

        public IList<SelectListItem> AvailableUserRoles { get; set; }

        [TvProgResourceDisplayName("Admin.Users.Users.Fields.UserRoles")]
        public IList<int> SelectedUserRoleIds { get; set; }

        //newsletter subscriptions (per store)
        [TvProgResourceDisplayName("Admin.Users.Users.Fields.Newsletter")]
        public IList<SelectListItem> AvailableNewsletterSubscriptionStores { get; set; }

        [TvProgResourceDisplayName("Admin.Users.Users.Fields.Newsletter")]
        public IList<int> SelectedNewsletterSubscriptionStoreIds { get; set; }

        //reward points history
        public bool DisplayRewardPointsHistory { get; set; }

        public AddRewardPointsToUserModel AddRewardPoints { get; set; }

        public UserRewardPointsSearchModel UserRewardPointsSearchModel { get; set; }

        //send email model
        public SendEmailModel SendEmail { get; set; }

        //send PM model
        public SendPmModel SendPm { get; set; }

        //send a private message
        public bool AllowSendingOfPrivateMessage { get; set; }

        //send the welcome message
        public bool AllowSendingOfWelcomeMessage { get; set; }

        //re-send the activation message
        public bool AllowReSendingOfActivationMessage { get; set; }

        //GDPR enabled
        public bool GdprEnabled { get; set; }
        
        public string AvatarUrl { get; internal set; }

        public UserAddressSearchModel UserAddressSearchModel { get; set; }

        public UserOrderSearchModel UserOrderSearchModel { get; set; }

        public UserShoppingCartSearchModel UserShoppingCartSearchModel { get; set; }

        public UserActivityLogSearchModel UserActivityLogSearchModel { get; set; }

        public UserBackInStockSubscriptionSearchModel UserBackInStockSubscriptionSearchModel { get; set; }

        public UserAssociatedExternalAuthRecordsSearchModel UserAssociatedExternalAuthRecordsSearchModel { get; set; }

        #endregion

        #region Nested classes

        public partial record SendEmailModel : BaseTvProgModel
        {
            [TvProgResourceDisplayName("Admin.Users.Users.SendEmail.Subject")]
            public string Subject { get; set; }

            [TvProgResourceDisplayName("Admin.Users.Users.SendEmail.Body")]
            public string Body { get; set; }

            [TvProgResourceDisplayName("Admin.Users.Users.SendEmail.SendImmediately")]
            public bool SendImmediately { get; set; }

            [TvProgResourceDisplayName("Admin.Users.Users.SendEmail.DontSendBeforeDate")]
            [UIHint("DateTimeNullable")]
            public DateTime? DontSendBeforeDate { get; set; }
        }

        public partial record SendPmModel : BaseTvProgModel
        {
            [TvProgResourceDisplayName("Admin.Users.Users.SendPM.Subject")]
            public string Subject { get; set; }

            [TvProgResourceDisplayName("Admin.Users.Users.SendPM.Message")]
            public string Message { get; set; }
        }

        public partial record UserAttributeModel : BaseTvProgEntityModel
        {
            public UserAttributeModel()
            {
                Values = new List<UserAttributeValueModel>();
            }

            public string Name { get; set; }

            public bool IsRequired { get; set; }

            /// <summary>
            /// Default value for textboxes
            /// </summary>
            public string DefaultValue { get; set; }

            public AttributeControlType AttributeControlType { get; set; }

            public IList<UserAttributeValueModel> Values { get; set; }
        }

        public partial record UserAttributeValueModel : BaseTvProgEntityModel
        {
            public string Name { get; set; }

            public bool IsPreSelected { get; set; }
        }

        #endregion
    }
}