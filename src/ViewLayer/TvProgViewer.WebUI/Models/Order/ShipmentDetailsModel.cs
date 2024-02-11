using System;
using System.Collections.Generic;
using TvProgViewer.Web.Framework.Models;

namespace TvProgViewer.WebUI.Models.Order
{
    public partial record ShipmentDetailsModel : BaseTvProgEntityModel
    {
        public ShipmentDetailsModel()
        {
            ShipmentStatusEvents = new List<ShipmentStatusEventModel>();
            Items = new List<ShipmentItemModel>();
        }

        public string TrackingNumber { get; set; }
        public string TrackingNumberUrl { get; set; }
        public DateTime? ShippedDate { get; set; }
        public DateTime? ReadyForPickupDate { get; set; }
        public DateTime? DeliveryDate { get; set; }
        public IList<ShipmentStatusEventModel> ShipmentStatusEvents { get; set; }
        public bool ShowSku { get; set; }
        public IList<ShipmentItemModel> Items { get; set; }

        public OrderDetailsModel Order { get; set; }

		#region Nested Classes

        public partial record ShipmentItemModel : BaseTvProgEntityModel
        {
            public string Sku { get; set; }
            public int TvChannelId { get; set; }
            public string TvChannelName { get; set; }
            public string TvChannelSeName { get; set; }
            public string AttributeInfo { get; set; }
            public string RentalInfo { get; set; }

            public int QuantityOrdered { get; set; }
            public int QuantityShipped { get; set; }
        }

        public partial record ShipmentStatusEventModel : BaseTvProgModel
        {
            public string Status { get; set; }
            public string EventName { get; set; }
            public string Location { get; set; }
            public string Country { get; set; }
            public DateTime? Date { get; set; }
        }

		#endregion
    }
}