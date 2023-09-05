﻿using System.Collections.Generic;
using TvProgViewer.Core.Domain.Shipping;
using TvProgViewer.Web.Framework.Models;

namespace TvProgViewer.WebUI.Models.Checkout
{
    public partial record CheckoutShippingMethodModel : BaseTvProgModel
    {
        public CheckoutShippingMethodModel()
        {
            ShippingMethods = new List<ShippingMethodModel>();
            Warnings = new List<string>();
        }

        public IList<ShippingMethodModel> ShippingMethods { get; set; }

        public bool NotifyUserAboutShippingFromMultipleLocations { get; set; }

        public IList<string> Warnings { get; set; }

        public bool DisplayPickupInStore { get; set; }
        public CheckoutPickupPointsModel PickupPointsModel { get; set; }

        #region Nested classes

        public partial record ShippingMethodModel : BaseTvProgModel
        {
            public string ShippingRateComputationMethodSystemName { get; set; }
            public string Name { get; set; }
            public string Description { get; set; }
            public string Fee { get; set; }
            public decimal Rate { get; set; }
            public int DisplayOrder { get; set; }
            public bool Selected { get; set; }
            public ShippingOption ShippingOption { get; set; }
        }

        #endregion
    }
}