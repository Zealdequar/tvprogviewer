using System;
using TvProgViewer.Core.Domain.Orders;
using TvProgViewer.Web.Framework.Models;

namespace TvProgViewer.WebUI.Areas.Admin.Models.ShoppingCart
{
    /// <summary>
    /// Represents a shopping cart item search model
    /// </summary>
    public partial record ShoppingCartItemSearchModel : BaseSearchModel
    {
        #region Properties

        public int UserId { get; set; }

        public ShoppingCartType ShoppingCartType { get; set; }

        public DateTime? StartDate { get; set; }

        public DateTime? EndDate { get; set; }

        public int TvChannelId { get; set; }

        public int BillingCountryId { get; set; }

        public int StoreId { get; set; }

        #endregion
    }
}