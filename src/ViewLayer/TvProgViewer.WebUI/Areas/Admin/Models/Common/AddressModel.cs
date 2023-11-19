using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;
using TvProgViewer.Core.Domain.Catalog;
using TvProgViewer.Web.Framework.Mvc.ModelBinding;
using TvProgViewer.Web.Framework.Models;

namespace TvProgViewer.WebUI.Areas.Admin.Models.Common
{
    public partial record AddressModel : BaseTvProgEntityModel
    {
        public AddressModel()
        {
            AvailableCountries = new List<SelectListItem>();
            AvailableStates = new List<SelectListItem>();
            CustomAddressAttributes = new List<AddressAttributeModel>();
        }

        [TvProgResourceDisplayName("Admin.Address.Fields.FirstName")]
        public string FirstName { get; set; }

        [TvProgResourceDisplayName("Admin.Address.Fields.LastName")]
        public string LastName { get; set; }

        [TvProgResourceDisplayName("Admin.Address.Fields.MiddleName")]
        public string MiddleName { get; set; }

        [DataType(DataType.EmailAddress)]
        [TvProgResourceDisplayName("Admin.Address.Fields.Email")]
        public string Email { get; set; }

        [TvProgResourceDisplayName("Admin.Address.Fields.Company")]
        public string Company { get; set; }

        [TvProgResourceDisplayName("Admin.Address.Fields.Country")]
        public int? CountryId { get; set; }

        [TvProgResourceDisplayName("Admin.Address.Fields.Country")]
        public string CountryName { get; set; }

        [TvProgResourceDisplayName("Admin.Address.Fields.StateProvince")]
        public int? StateProvinceId { get; set; }

        [TvProgResourceDisplayName("Admin.Address.Fields.StateProvince")]
        public string StateProvinceName { get; set; }

        [TvProgResourceDisplayName("Admin.Address.Fields.Address1")]
        public string Address1 { get; set; }

        [TvProgResourceDisplayName("Admin.Address.Fields.City")]
        public string City { get; set; }

        [TvProgResourceDisplayName("Admin.Address.Fields.County")]
        public string County { get; set; }

        [TvProgResourceDisplayName("Admin.Address.Fields.Address2")]
        public string Address2 { get; set; }

        [TvProgResourceDisplayName("Admin.Address.Fields.ZipPostalCode")]
        public string ZipPostalCode { get; set; }

        [DataType(DataType.PhoneNumber)]
        [TvProgResourceDisplayName("Admin.Address.Fields.PhoneNumber")]
        public string PhoneNumber { get; set; }

        [DataType(DataType.PhoneNumber)]
        [TvProgResourceDisplayName("Admin.Address.Fields.FaxNumber")]
        public string FaxNumber { get; set; }

        //address in HTML format (usually used in grids)
        [TvProgResourceDisplayName("Admin.Address")]
        public string AddressHtml { get; set; }

        //formatted custom address attributes
        public string FormattedCustomAddressAttributes { get; set; }
        public IList<AddressAttributeModel> CustomAddressAttributes { get; set; }

        public IList<SelectListItem> AvailableCountries { get; set; }
        public IList<SelectListItem> AvailableStates { get; set; }
        
        public bool FirstNameRequired { get; set; }
        public bool LastNameRequired { get; set; }
        public bool EmailRequired { get; set; }
        public bool CompanyRequired { get; set; }
        public bool CountryRequired { get; set; }
        public bool CityRequired { get; set; }
        public bool CountyRequired { get; set; }
        public bool StreetAddressRequired { get; set; }
        public bool StreetAddress2Required { get; set; }
        public bool ZipPostalCodeRequired { get; set; }
        public bool SmartPhoneRequired { get; set; }
        public bool FaxRequired { get; set; }

        #region Nested classes

        public partial record AddressAttributeModel : BaseTvProgEntityModel
        {
            public AddressAttributeModel()
            {
                Values = new List<AddressAttributeValueModel>();
            }

            public string Name { get; set; }

            public bool IsRequired { get; set; }

            /// <summary>
            /// Selected value for textboxes
            /// </summary>
            public string DefaultValue { get; set; }

            public AttributeControlType AttributeControlType { get; set; }

            public IList<AddressAttributeValueModel> Values { get; set; }
        }

        public partial record AddressAttributeValueModel : BaseTvProgEntityModel
        {
            public string Name { get; set; }

            public bool IsPreSelected { get; set; }
        }

        #endregion
    }
}