﻿using System;
using System.ComponentModel.DataAnnotations;
using TVProgViewer.Web.Framework.Mvc.ModelBinding;
using TVProgViewer.Web.Framework.Models;

namespace TVProgViewer.WebUI.Areas.Admin.Models.Affiliates
{
    /// <summary>
    /// Represents an affiliate search model
    /// </summary>
    public partial record AffiliateSearchModel : BaseSearchModel
    {
        #region Properties

        [TvProgResourceDisplayName("Admin.Affiliates.List.SearchFirstName")]
        public string SearchFirstName { get; set; }

        [TvProgResourceDisplayName("Admin.Affiliates.List.SearchLastName")]
        public string SearchLastName { get; set; }

        [TvProgResourceDisplayName("Admin.Affiliates.List.SearchFriendlyUrlName")]
        public string SearchFriendlyUrlName { get; set; }

        [TvProgResourceDisplayName("Admin.Affiliates.List.LoadOnlyWithOrders")]
        public bool LoadOnlyWithOrders { get; set; }

        [TvProgResourceDisplayName("Admin.Affiliates.List.OrdersCreatedFromUtc")]
        [UIHint("DateNullable")]
        public DateTime? OrdersCreatedFromUtc { get; set; }

        [TvProgResourceDisplayName("Admin.Affiliates.List.OrdersCreatedToUtc")]
        [UIHint("DateNullable")]
        public DateTime? OrdersCreatedToUtc { get; set; }

        #endregion
    }
}