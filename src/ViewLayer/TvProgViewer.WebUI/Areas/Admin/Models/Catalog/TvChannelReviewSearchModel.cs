using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;
using TvProgViewer.Web.Framework.Mvc.ModelBinding;
using TvProgViewer.Web.Framework.Models;

namespace TvProgViewer.WebUI.Areas.Admin.Models.Catalog
{
    /// <summary>
    /// Represents a tvChannel review search model
    /// </summary>
    public partial record TvChannelReviewSearchModel : BaseSearchModel
    {
        #region Ctor

        public TvChannelReviewSearchModel()
        {
            AvailableStores = new List<SelectListItem>();
            AvailableApprovedOptions = new List<SelectListItem>();
        }

        #endregion

        #region Properties

        [TvProgResourceDisplayName("Admin.Catalog.TvChannelReviews.List.CreatedOnFrom")]
        [UIHint("DateNullable")]
        public DateTime? CreatedOnFrom { get; set; }

        [TvProgResourceDisplayName("Admin.Catalog.TvChannelReviews.List.CreatedOnTo")]
        [UIHint("DateNullable")]
        public DateTime? CreatedOnTo { get; set; }

        [TvProgResourceDisplayName("Admin.Catalog.TvChannelReviews.List.SearchText")]
        public string SearchText { get; set; }

        [TvProgResourceDisplayName("Admin.Catalog.TvChannelReviews.List.SearchStore")]
        public int SearchStoreId { get; set; }

        [TvProgResourceDisplayName("Admin.Catalog.TvChannelReviews.List.SearchTvChannel")]
        public int SearchTvChannelId { get; set; }

        [TvProgResourceDisplayName("Admin.Catalog.TvChannelReviews.List.SearchApproved")]
        public int SearchApprovedId { get; set; }

        //vendor
        public bool IsLoggedInAsVendor { get; set; }

        public IList<SelectListItem> AvailableStores { get; set; }

        public IList<SelectListItem> AvailableApprovedOptions { get; set; }

        public bool HideStoresList { get; set; }

        #endregion
    }
}