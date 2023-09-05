using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;
using TvProgViewer.Web.Framework.Models;
using TvProgViewer.Web.Framework.Mvc.ModelBinding;

namespace TvProgViewer.WebUI.Areas.Admin.Models.Users
{
    /// <summary>
    /// Represents a user search model
    /// </summary>
    public partial record UserSearchModel : BaseSearchModel, IAclSupportedModel
    {
        #region Ctor

        public UserSearchModel()
        {
            SelectedUserRoleIds = new List<int>();
            AvailableUserRoles = new List<SelectListItem>();
        }

        #endregion

        #region Properties

        [TvProgResourceDisplayName("Admin.Users.Users.List.UserRoles")]
        public IList<int> SelectedUserRoleIds { get; set; }

        public IList<SelectListItem> AvailableUserRoles { get; set; }

        [DataType(DataType.EmailAddress)]
        [TvProgResourceDisplayName("Admin.Users.Users.List.SearchEmail")]
        public string SearchEmail { get; set; }

        [TvProgResourceDisplayName("Admin.Users.Users.List.SearchUsername")]
        public string SearchUsername { get; set; }

        public bool UsernamesEnabled { get; set; }

        [TvProgResourceDisplayName("Admin.Users.Users.List.SearchFirstName")]
        public string SearchFirstName { get; set; }
        public bool FirstNameEnabled { get; set; }

        [TvProgResourceDisplayName("Admin.Users.Users.List.SearchLastName")]
        public string SearchLastName { get; set; }
        public bool LastNameEnabled { get; set; }

        [UIHint("DateNullable")]
        [TvProgResourceDisplayName("Admin.Users.Users.List.SearchLastActivityFrom")]
        public DateTime? SearchLastActivityFrom { get; set; }

        [UIHint("DateNullable")]
        [TvProgResourceDisplayName("Admin.Users.Users.List.SearchLastActivityTo")]
        public DateTime? SearchLastActivityTo { get; set; }

        [UIHint("DateNullable")]
        [TvProgResourceDisplayName("Admin.Users.Users.List.SearchRegistrationDateFrom")]
        public DateTime? SearchRegistrationDateFrom { get; set; }

        [UIHint("DateNullable")]
        [TvProgResourceDisplayName("Admin.Users.Users.List.SearchRegistrationDateTo")]
        public DateTime? SearchRegistrationDateTo { get; set; }

        [TvProgResourceDisplayName("Admin.Users.Users.List.SearchBirthDate")]
        public string SearchDayOfBirth { get; set; }

        [TvProgResourceDisplayName("Admin.Users.Users.List.SearchBirthDate")]
        public string SearchMonthOfBirth { get; set; }

        public bool BirthDateEnabled { get; set; }

        [TvProgResourceDisplayName("Admin.Users.Users.List.SearchCompany")]
        public string SearchCompany { get; set; }

        public bool CompanyEnabled { get; set; }

        [DataType(DataType.PhoneNumber)]
        [TvProgResourceDisplayName("Admin.Users.Users.List.SearchSmartPhone")]
        public string SearchSmartPhone { get; set; }

        public bool SmartPhoneEnabled { get; set; }

        [TvProgResourceDisplayName("Admin.Users.Users.List.SearchZipCode")]
        public string SearchZipPostalCode { get; set; }

        public bool ZipPostalCodeEnabled { get; set; }

        [TvProgResourceDisplayName("Admin.Users.Users.List.SearchIpAddress")]
        public string SearchIpAddress { get; set; }

        public bool AvatarEnabled { get; internal set; }

        #endregion
    }
}