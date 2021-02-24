using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;
using TVProgViewer.Web.Framework.Mvc.ModelBinding;
using TVProgViewer.Web.Framework.Models;

namespace TVProgViewer.WebUI.Models.User
{
    public partial record RegisterModel : BaseTvProgModel
    {
        public RegisterModel()
        {
            AvailableTimeZones = new List<SelectListItem>();
            AvailableCountries = new List<SelectListItem>();
            AvailableStates = new List<SelectListItem>();
            UserAttributes = new List<UserAttributeModel>();
            GdprConsents = new List<GdprConsentModel>();
        }
        
        [DataType(DataType.EmailAddress)]
        [TvProgResourceDisplayName("Account.Fields.Email")]
        public string Email { get; set; }
        
        public bool EnteringEmailTwice { get; set; }
        [DataType(DataType.EmailAddress)]
        [TvProgResourceDisplayName("Account.Fields.ConfirmEmail")]
        public string ConfirmEmail { get; set; }

        public bool UsernamesEnabled { get; set; }
        [TvProgResourceDisplayName("Account.Fields.Username")]
        public string Username { get; set; }

        public bool CheckUsernameAvailabilityEnabled { get; set; }

        [DataType(DataType.Password)]
        [TvProgResourceDisplayName("Account.Fields.Password")]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [TvProgResourceDisplayName("Account.Fields.ConfirmPassword")]
        public string ConfirmPassword { get; set; }

        //form fields & properties
        public bool GenderEnabled { get; set; }
        [TvProgResourceDisplayName("Account.Fields.Gender")]
        public string Gender { get; set; }

        public bool FirstNameEnabled { get; set; }
        [TvProgResourceDisplayName("Account.Fields.FirstName")]
        public string FirstName { get; set; }
        public bool FirstNameRequired { get; set; }
        public bool LastNameEnabled { get; set; }
        [TvProgResourceDisplayName("Account.Fields.LastName")]
        public string LastName { get; set; }
        public bool LastNameRequired { get; set; }

        public bool DateOfBirthEnabled { get; set; }
        [TvProgResourceDisplayName("Account.Fields.DateOfBirth")]
        public int? DateOfBirthDay { get; set; }
        [TvProgResourceDisplayName("Account.Fields.DateOfBirth")]
        public int? DateOfBirthMonth { get; set; }
        [TvProgResourceDisplayName("Account.Fields.DateOfBirth")]
        public int? DateOfBirthYear { get; set; }
        public bool DateOfBirthRequired { get; set; }
        public DateTime? ParseDateOfBirth()
        {
            if (!DateOfBirthYear.HasValue || !DateOfBirthMonth.HasValue || !DateOfBirthDay.HasValue)
                return null;

            DateTime? dateOfBirth = null;
            try
            {
                dateOfBirth = new DateTime(DateOfBirthYear.Value, DateOfBirthMonth.Value, DateOfBirthDay.Value);
            }
            catch { }
            return dateOfBirth;
        }

        public bool CompanyEnabled { get; set; }
        public bool CompanyRequired { get; set; }
        [TvProgResourceDisplayName("Account.Fields.Company")]
        public string Company { get; set; }

        public bool StreetAddressEnabled { get; set; }
        public bool StreetAddressRequired { get; set; }
        [TvProgResourceDisplayName("Account.Fields.StreetAddress")]
        public string StreetAddress { get; set; }

        public bool StreetAddress2Enabled { get; set; }
        public bool StreetAddress2Required { get; set; }
        [TvProgResourceDisplayName("Account.Fields.StreetAddress2")]
        public string StreetAddress2 { get; set; }

        public bool ZipPostalCodeEnabled { get; set; }
        public bool ZipPostalCodeRequired { get; set; }
        [TvProgResourceDisplayName("Account.Fields.ZipPostalCode")]
        public string ZipPostalCode { get; set; }

        public bool CityEnabled { get; set; }
        public bool CityRequired { get; set; }
        [TvProgResourceDisplayName("Account.Fields.City")]
        public string City { get; set; }

        public bool CountyEnabled { get; set; }
        public bool CountyRequired { get; set; }
        [TvProgResourceDisplayName("Account.Fields.County")]
        public string County { get; set; }

        public bool CountryEnabled { get; set; }
        public bool CountryRequired { get; set; }
        [TvProgResourceDisplayName("Account.Fields.Country")]
        public int CountryId { get; set; }
        public IList<SelectListItem> AvailableCountries { get; set; }

        public bool StateProvinceEnabled { get; set; }
        public bool StateProvinceRequired { get; set; }
        [TvProgResourceDisplayName("Account.Fields.StateProvince")]
        public int StateProvinceId { get; set; }
        public IList<SelectListItem> AvailableStates { get; set; }

        public bool PhoneEnabled { get; set; }
        public bool PhoneRequired { get; set; }
        [DataType(DataType.PhoneNumber)]
        [TvProgResourceDisplayName("Account.Fields.Phone")]
        public string Phone { get; set; }

        public bool FaxEnabled { get; set; }
        public bool FaxRequired { get; set; }
        [DataType(DataType.PhoneNumber)]
        [TvProgResourceDisplayName("Account.Fields.Fax")]
        public string Fax { get; set; }
        
        public bool NewsletterEnabled { get; set; }
        [TvProgResourceDisplayName("Account.Fields.Newsletter")]
        public bool Newsletter { get; set; }
        
        public bool AcceptPrivacyPolicyEnabled { get; set; }
        public bool AcceptPrivacyPolicyPopup { get; set; }

        //time zone
        [TvProgResourceDisplayName("Account.Fields.TimeZone")]
        public string TimeZoneId { get; set; }
        public bool AllowUsersToSetTimeZone { get; set; }
        public IList<SelectListItem> AvailableTimeZones { get; set; }

        //EU VAT
        [TvProgResourceDisplayName("Account.Fields.VatNumber")]
        public string VatNumber { get; set; }
        public bool DisplayVatNumber { get; set; }

        public bool HoneypotEnabled { get; set; }
        public bool DisplayCaptcha { get; set; }

        public IList<UserAttributeModel> UserAttributes { get; set; }

        public IList<GdprConsentModel> GdprConsents { get; set; }
    }
}