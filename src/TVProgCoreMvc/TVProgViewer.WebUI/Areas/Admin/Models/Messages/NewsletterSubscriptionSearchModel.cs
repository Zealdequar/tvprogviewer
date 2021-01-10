using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;
using TVProgViewer.Web.Framework.Mvc.ModelBinding;
using TVProgViewer.Web.Framework.Models;

namespace TVProgViewer.WebUI.Areas.Admin.Models.Messages
{
    /// <summary>
    /// Represents a newsletter subscription search model
    /// </summary>
    public partial record NewsletterSubscriptionSearchModel : BaseSearchModel
    {
        #region Ctor

        public NewsletterSubscriptionSearchModel()
        {
            AvailableStores = new List<SelectListItem>();
            ActiveList = new List<SelectListItem>();
            AvailableUserRoles = new List<SelectListItem>();
        }

        #endregion

        #region Properties

        [DataType(DataType.EmailAddress)]
        [TvProgResourceDisplayName("Admin.Promotions.NewsLetterSubscriptions.List.SearchEmail")]
        public string SearchEmail { get; set; }

        [TvProgResourceDisplayName("Admin.Promotions.NewsLetterSubscriptions.List.SearchStore")]
        public int StoreId { get; set; }

        public IList<SelectListItem> AvailableStores { get; set; }

        [TvProgResourceDisplayName("Admin.Promotions.NewsLetterSubscriptions.List.SearchActive")]
        public int ActiveId { get; set; }

        [TvProgResourceDisplayName("Admin.Promotions.NewsLetterSubscriptions.List.SearchActive")]
        public IList<SelectListItem> ActiveList { get; set; }

        [TvProgResourceDisplayName("Admin.Promotions.NewsLetterSubscriptions.List.UserRoles")]
        public int UserRoleId { get; set; }

        public IList<SelectListItem> AvailableUserRoles { get; set; }

        [TvProgResourceDisplayName("Admin.Promotions.NewsLetterSubscriptions.List.StartDate")]
        [UIHint("DateNullable")]
        public DateTime? StartDate { get; set; }

        [TvProgResourceDisplayName("Admin.Promotions.NewsLetterSubscriptions.List.EndDate")]
        [UIHint("DateNullable")]
        public DateTime? EndDate { get; set; }

        public bool HideStoresList { get; set; }

        #endregion
    }
}