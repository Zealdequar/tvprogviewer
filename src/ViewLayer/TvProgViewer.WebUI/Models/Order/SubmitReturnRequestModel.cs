using System;
using System.Collections.Generic;
using TvProgViewer.Web.Framework.Mvc.ModelBinding;
using TvProgViewer.Web.Framework.Models;

namespace TvProgViewer.WebUI.Models.Order
{
    public partial record SubmitReturnRequestModel : BaseTvProgModel
    {
        public SubmitReturnRequestModel()
        {
            Items = new List<OrderItemModel>();
            AvailableReturnReasons = new List<ReturnRequestReasonModel>();
            AvailableReturnActions= new List<ReturnRequestActionModel>();
        }
        
        public int OrderId { get; set; }
        public string CustomOrderNumber { get; set; }

        public IList<OrderItemModel> Items { get; set; }
        
        [TvProgResourceDisplayName("ReturnRequests.ReturnReason")]
        public int ReturnRequestReasonId { get; set; }
        public IList<ReturnRequestReasonModel> AvailableReturnReasons { get; set; }
        
        [TvProgResourceDisplayName("ReturnRequests.ReturnAction")]
        public int ReturnRequestActionId { get; set; }
        public IList<ReturnRequestActionModel> AvailableReturnActions { get; set; }
        
        [TvProgResourceDisplayName("ReturnRequests.Comments")]
        public string Comments { get; set; }

        public bool AllowFiles { get; set; }
        [TvProgResourceDisplayName("ReturnRequests.UploadedFile")]
        public Guid UploadedFileGuid { get; set; }

        public string Result { get; set; }
        
        #region Nested classes

        public partial record OrderItemModel : BaseTvProgEntityModel
        {
            public int TvChannelId { get; set; }

            public string TvChannelName { get; set; }

            public string TvChannelSeName { get; set; }

            public string AttributeInfo { get; set; }

            public string UnitPrice { get; set; }

            public int Quantity { get; set; }
        }

        public partial record ReturnRequestReasonModel : BaseTvProgEntityModel
        {
            public string Name { get; set; }
        }

        public partial record ReturnRequestActionModel : BaseTvProgEntityModel
        {
            public string Name { get; set; }
        }

        #endregion
    }

}