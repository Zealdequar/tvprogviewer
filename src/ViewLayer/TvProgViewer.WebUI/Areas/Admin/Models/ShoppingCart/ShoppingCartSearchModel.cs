using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;
using TvProgViewer.Core.Domain.Orders;
using TvProgViewer.Web.Framework.Models;
using TvProgViewer.Web.Framework.Mvc.ModelBinding;

namespace TvProgViewer.WebUI.Areas.Admin.Models.ShoppingCart
{
    /// <summary>
    /// Represents a shopping cart search model
    /// </summary>
    public partial record ShoppingCartSearchModel : BaseSearchModel
    {
        #region Ctor

        public ShoppingCartSearchModel()
        {
            AvailableShoppingCartTypes = new List<SelectListItem>();
            ShoppingCartItemSearchModel = new ShoppingCartItemSearchModel();
            AvailableStores = new List<SelectListItem>();
            AvailableCountries = new List<SelectListItem>();
        }

        #endregion

        #region Properties

        [TvProgResourceDisplayName("Admin.ShoppingCartType.ShoppingCartType")]
        public ShoppingCartType ShoppingCartType { get; set; }

        [TvProgResourceDisplayName("Admin.ShoppingCartType.StartDate")]
        [UIHint("DateNullable")]
        public DateTime? StartDate { get; set; }

        [TvProgResourceDisplayName("Admin.ShoppingCartType.EndDate")]
        [UIHint("DateNullable")]
        public DateTime? EndDate { get; set; }

        [TvProgResourceDisplayName("Admin.ShoppingCartType.TvChannel")]
        public int TvChannelId { get; set; }

        [TvProgResourceDisplayName("Admin.ShoppingCartType.BillingCountry")]
        public int BillingCountryId { get; set; }

        [TvProgResourceDisplayName("Admin.ShoppingCartType.Store")]
        public int StoreId { get; set; }

        public IList<SelectListItem> AvailableShoppingCartTypes { get; set; }

        public ShoppingCartItemSearchModel ShoppingCartItemSearchModel { get; set; }

        public IList<SelectListItem> AvailableStores { get; set; }

        public IList<SelectListItem> AvailableCountries { get; set; }

        public bool HideStoresList { get; set; }

        #endregion
    }
}