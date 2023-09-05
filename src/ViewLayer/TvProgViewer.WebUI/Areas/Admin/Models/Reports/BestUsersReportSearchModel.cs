using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;
using TvProgViewer.Web.Framework.Mvc.ModelBinding;
using TvProgViewer.Web.Framework.Models;
using TvProgViewer.Services.Orders;

namespace TvProgViewer.WebUI.Areas.Admin.Models.Reports
{
    /// <summary>
    /// Represents a best users report search model
    /// </summary>
    public partial record BestUsersReportSearchModel : BaseSearchModel
    {
        #region Ctor

        public BestUsersReportSearchModel()
        {
            AvailableOrderStatuses = new List<SelectListItem>();
            AvailablePaymentStatuses = new List<SelectListItem>();
            AvailableShippingStatuses = new List<SelectListItem>();
        }

        #endregion

        #region Properties

        public OrderByEnum OrderBy { get; set; }

        [TvProgResourceDisplayName("Admin.Reports.Users.BestBy.StartDate")]
        [UIHint("DateNullable")]
        public DateTime? StartDate { get; set; }

        [TvProgResourceDisplayName("Admin.Reports.Users.BestBy.EndDate")]
        [UIHint("DateNullable")]
        public DateTime? EndDate { get; set; }

        [TvProgResourceDisplayName("Admin.Reports.Users.BestBy.OrderStatus")]
        public int OrderStatusId { get; set; }

        [TvProgResourceDisplayName("Admin.Reports.Users.BestBy.PaymentStatus")]
        public int PaymentStatusId { get; set; }

        [TvProgResourceDisplayName("Admin.Reports.Users.BestBy.ShippingStatus")]
        public int ShippingStatusId { get; set; }

        public IList<SelectListItem> AvailableOrderStatuses { get; set; }

        public IList<SelectListItem> AvailablePaymentStatuses { get; set; }

        public IList<SelectListItem> AvailableShippingStatuses { get; set; }

        #endregion
    }
}