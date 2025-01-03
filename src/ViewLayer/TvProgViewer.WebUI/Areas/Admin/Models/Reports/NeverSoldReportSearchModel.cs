﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;
using TvProgViewer.Web.Framework.Mvc.ModelBinding;
using TvProgViewer.Web.Framework.Models;

namespace TvProgViewer.WebUI.Areas.Admin.Models.Reports
{
    /// <summary>
    /// Represents a never sold tvChannels report search model
    /// </summary>
    public partial record NeverSoldReportSearchModel : BaseSearchModel
    {
        #region Ctor

        public NeverSoldReportSearchModel()
        {
            AvailableCategories = new List<SelectListItem>();
            AvailableManufacturers = new List<SelectListItem>();
            AvailableStores = new List<SelectListItem>();
            AvailableVendors = new List<SelectListItem>();
        }

        #endregion

        #region Properties

        [TvProgResourceDisplayName("Admin.Reports.Sales.NeverSold.StartDate")]
        [UIHint("DateNullable")]
        public DateTime? StartDate { get; set; }

        [TvProgResourceDisplayName("Admin.Reports.Sales.NeverSold.EndDate")]
        [UIHint("DateNullable")]
        public DateTime? EndDate { get; set; }

        [TvProgResourceDisplayName("Admin.Reports.Sales.NeverSold.SearchCategory")]
        public int SearchCategoryId { get; set; }

        public IList<SelectListItem> AvailableCategories { get; set; }

        [TvProgResourceDisplayName("Admin.Reports.Sales.NeverSold.SearchManufacturer")]
        public int SearchManufacturerId { get; set; }

        public IList<SelectListItem> AvailableManufacturers { get; set; }

        [TvProgResourceDisplayName("Admin.Reports.Sales.NeverSold.SearchStore")]
        public int SearchStoreId { get; set; }

        public IList<SelectListItem> AvailableStores { get; set; }
        
        [TvProgResourceDisplayName("Admin.Reports.Sales.NeverSold.SearchVendor")]
        public int SearchVendorId { get; set; }

        public IList<SelectListItem> AvailableVendors { get; set; }

        public bool IsLoggedInAsVendor { get; set; }

        #endregion
    }
}