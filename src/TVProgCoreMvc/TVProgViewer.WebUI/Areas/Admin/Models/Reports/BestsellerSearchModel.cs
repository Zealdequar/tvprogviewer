using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;
using TVProgViewer.Web.Framework.Mvc.ModelBinding;
using TVProgViewer.Web.Framework.Models;

namespace TVProgViewer.WebUI.Areas.Admin.Models.Reports
{
    /// <summary>
    /// Represents a bestseller search model
    /// </summary>
    public partial record BestsellerSearchModel : BaseSearchModel
    {
        #region Ctor

        public BestsellerSearchModel()
        {
            AvailableStores = new List<SelectListItem>();
            AvailableOrderStatuses = new List<SelectListItem>();
            AvailablePaymentStatuses = new List<SelectListItem>();
            AvailableCategories = new List<SelectListItem>();
            AvailableManufacturers = new List<SelectListItem>();
            AvailableCountries = new List<SelectListItem>();
            AvailableVendors = new List<SelectListItem>();
        }

        #endregion

        #region Properties

        [TvProgResourceDisplayName("Admin.Reports.Sales.Bestsellers.StartDate")]
        [UIHint("DateNullable")]
        public DateTime? StartDate { get; set; }

        [TvProgResourceDisplayName("Admin.Reports.Sales.Bestsellers.EndDate")]
        [UIHint("DateNullable")]
        public DateTime? EndDate { get; set; }

        [TvProgResourceDisplayName("Admin.Reports.Sales.Bestsellers.Store")]
        public int StoreId { get; set; }

        [TvProgResourceDisplayName("Admin.Reports.Sales.Bestsellers.OrderStatus")]
        public int OrderStatusId { get; set; }

        [TvProgResourceDisplayName("Admin.Reports.Sales.Bestsellers.PaymentStatus")]
        public int PaymentStatusId { get; set; }

        [TvProgResourceDisplayName("Admin.Reports.Sales.Bestsellers.Category")]
        public int CategoryId { get; set; }

        [TvProgResourceDisplayName("Admin.Reports.Sales.Bestsellers.Manufacturer")]
        public int ManufacturerId { get; set; }

        [TvProgResourceDisplayName("Admin.Reports.Sales.Bestsellers.BillingCountry")]
        public int BillingCountryId { get; set; }

        [TvProgResourceDisplayName("Admin.Reports.Sales.Bestsellers.Vendor")]
        public int VendorId { get; set; }

        public IList<SelectListItem> AvailableStores { get; set; }

        public IList<SelectListItem> AvailableOrderStatuses { get; set; }

        public IList<SelectListItem> AvailablePaymentStatuses { get; set; }

        public IList<SelectListItem> AvailableCategories { get; set; }

        public IList<SelectListItem> AvailableManufacturers { get; set; }

        public IList<SelectListItem> AvailableCountries { get; set; }

        public IList<SelectListItem> AvailableVendors { get; set; }

        public bool IsLoggedInAsVendor { get; set; }

        #endregion
    }
}