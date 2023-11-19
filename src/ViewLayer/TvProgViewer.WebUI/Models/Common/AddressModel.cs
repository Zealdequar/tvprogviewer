using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;
using TvProgViewer.Web.Framework.Mvc.ModelBinding;
using TvProgViewer.Web.Framework.Models;

namespace TvProgViewer.WebUI.Models.Common
{
    public partial record AddressModel : BaseTvProgEntityModel
    {
        public AddressModel()
        {
            AvailableCountries = new List<SelectListItem>();
            AvailableStates = new List<SelectListItem>();
            CustomAddressAttributes = new List<AddressAttributeModel>();
        }

        [TvProgResourceDisplayName("Address.Fields.FirstName")]
        public string FirstName { get; set; }
        [TvProgResourceDisplayName("Address.Fields.LastName")]
        public string LastName { get; set; }
        [TvProgResourceDisplayName("Address.Fields.MiddleName")]
        public string MiddleName { get; set; }
        [DataType(DataType.EmailAddress)]
        [TvProgResourceDisplayName("Address.Fields.Email")]
        public string Email { get; set; }


        public bool CompanyEnabled { get; set; }
        public bool CompanyRequired { get; set; }
        [TvProgResourceDisplayName("Address.Fields.Company")]
        public string Company { get; set; }

        public bool CountryEnabled { get; set; }
        [TvProgResourceDisplayName("Address.Fields.Country")]
        public int? CountryId { get; set; }
        [TvProgResourceDisplayName("Address.Fields.Country")]
        public string CountryName { get; set; }

        public bool StateProvinceEnabled { get; set; }
        [TvProgResourceDisplayName("Address.Fields.StateProvince")]
        public int? StateProvinceId { get; set; }
        [TvProgResourceDisplayName("Address.Fields.StateProvince")]
        public string StateProvinceName { get; set; }

        public bool CountyEnabled { get; set; }
        public bool CountyRequired { get; set; }
        [TvProgResourceDisplayName("Address.Fields.County")]
        public string County { get; set; }

        public bool CityEnabled { get; set; }
        public bool CityRequired { get; set; }
        [TvProgResourceDisplayName("Address.Fields.City")]
        public string City { get; set; }

        public bool StreetAddressEnabled { get; set; }
        public bool StreetAddressRequired { get; set; }
        [TvProgResourceDisplayName("Address.Fields.Address1")]
        public string Address1 { get; set; }

        public bool StreetAddress2Enabled { get; set; }
        public bool StreetAddress2Required { get; set; }
        [TvProgResourceDisplayName("Address.Fields.Address2")]
        public string Address2 { get; set; }

        public bool ZipPostalCodeEnabled { get; set; }
        public bool ZipPostalCodeRequired { get; set; }
        [TvProgResourceDisplayName("Address.Fields.ZipPostalCode")]
        public string ZipPostalCode { get; set; }

        public bool SmartPhoneEnabled { get; set; }
        public bool SmartPhoneRequired { get; set; }
        [DataType(DataType.PhoneNumber)]
        [TvProgResourceDisplayName("Address.Fields.PhoneNumber")]
        public string PhoneNumber { get; set; }

        public bool FaxEnabled { get; set; }
        public bool FaxRequired { get; set; }
        [TvProgResourceDisplayName("Address.Fields.FaxNumber")]
        public string FaxNumber { get; set; }

        public IList<SelectListItem> AvailableCountries { get; set; }
        public IList<SelectListItem> AvailableStates { get; set; }

        public string FormattedCustomAddressAttributes { get; set; }
        public IList<AddressAttributeModel> CustomAddressAttributes { get; set; }
    }
}