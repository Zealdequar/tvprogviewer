using System.Threading.Tasks;
using TvProgViewer.Core.Domain.Catalog;
using TvProgViewer.Core.Domain.Common;
using TvProgViewer.Core.Domain.Orders;
using TvProgViewer.Core.Domain.Shipping;
using TvProgViewer.WebUI.Areas.Admin.Models.Orders;
using TvProgViewer.WebUI.Areas.Admin.Models.Reports;

namespace TvProgViewer.WebUI.Areas.Admin.Factories
{
    /// <summary>
    /// Represents the order model factory
    /// </summary>
    public partial interface IOrderModelFactory
    {
        /// <summary>
        /// Prepare order search model
        /// </summary>
        /// <param name="searchModel">Order search model</param>
        /// <returns>
        /// Задача представляет асинхронную операцию
        /// The task result contains the order search model
        /// </returns>
        Task<OrderSearchModel> PrepareOrderSearchModelAsync(OrderSearchModel searchModel);

        /// <summary>
        /// Prepare paged order list model
        /// </summary>
        /// <param name="searchModel">Order search model</param>
        /// <returns>
        /// Задача представляет асинхронную операцию
        /// The task result contains the order list model
        /// </returns>
        Task<OrderListModel> PrepareOrderListModelAsync(OrderSearchModel searchModel);

        /// <summary>
        /// Prepare order aggregator model
        /// </summary>
        /// <param name="searchModel">Order search model</param>
        /// <returns>
        /// Задача представляет асинхронную операцию
        /// The task result contains the order aggregator model
        /// </returns>
        Task<OrderAggreratorModel> PrepareOrderAggregatorModelAsync(OrderSearchModel searchModel);

        /// <summary>
        /// Prepare order model
        /// </summary>
        /// <param name="model">Order model</param>
        /// <param name="order">Order</param>
        /// <param name="excludeProperties">Whether to exclude populating of some properties of model</param>
        /// <returns>
        /// Задача представляет асинхронную операцию
        /// The task result contains the order model
        /// </returns>
        Task<OrderModel> PrepareOrderModelAsync(OrderModel model, Order order, bool excludeProperties = false);

        /// <summary>
        /// Prepare upload license model
        /// </summary>
        /// <param name="model">Upload license model</param>
        /// <param name="order">Order</param>
        /// <param name="orderItem">Order item</param>
        /// <returns>
        /// Задача представляет асинхронную операцию
        /// The task result contains the upload license model
        /// </returns>
        Task<UploadLicenseModel> PrepareUploadLicenseModelAsync(UploadLicenseModel model, Order order, OrderItem orderItem);

        /// <summary>
        /// Prepare tvchannel search model to add to the order
        /// </summary>
        /// <param name="searchModel">TvChannel search model to add to the order</param>
        /// <param name="order">Order</param>
        /// <returns>
        /// Задача представляет асинхронную операцию
        /// The task result contains the tvchannel search model to add to the order
        /// </returns>
        Task<AddTvChannelToOrderSearchModel> PrepareAddTvChannelToOrderSearchModelAsync(AddTvChannelToOrderSearchModel searchModel, Order order);

        /// <summary>
        /// Prepare paged tvchannel list model to add to the order
        /// </summary>
        /// <param name="searchModel">TvChannel search model to add to the order</param>
        /// <param name="order">Order</param>
        /// <returns>
        /// Задача представляет асинхронную операцию
        /// The task result contains the tvchannel search model to add to the order
        /// </returns>
        Task<AddTvChannelToOrderListModel> PrepareAddTvChannelToOrderListModelAsync(AddTvChannelToOrderSearchModel searchModel, Order order);

        /// <summary>
        /// Prepare tvchannel model to add to the order
        /// </summary>
        /// <param name="model">TvChannel model to add to the order</param>
        /// <param name="order">Order</param>
        /// <param name="tvchannel">TvChannel</param>
        /// <returns>
        /// Задача представляет асинхронную операцию
        /// The task result contains the tvchannel model to add to the order
        /// </returns>
        Task<AddTvChannelToOrderModel> PrepareAddTvChannelToOrderModelAsync(AddTvChannelToOrderModel model, Order order, TvChannel tvchannel);

        /// <summary>
        /// Prepare order address model
        /// </summary>
        /// <param name="model">Order address model</param>
        /// <param name="order">Order</param>
        /// <param name="address">Address</param>
        /// <returns>
        /// Задача представляет асинхронную операцию
        /// The task result contains the order address model
        /// </returns>
        Task<OrderAddressModel> PrepareOrderAddressModelAsync(OrderAddressModel model, Order order, Address address);

        /// <summary>
        /// Prepare shipment search model
        /// </summary>
        /// <param name="searchModel">Shipment search model</param>
        /// <returns>
        /// Задача представляет асинхронную операцию
        /// The task result contains the shipment search model
        /// </returns>
        Task<ShipmentSearchModel> PrepareShipmentSearchModelAsync(ShipmentSearchModel searchModel);

        /// <summary>
        /// Prepare paged shipment list model
        /// </summary>
        /// <param name="searchModel">Shipment search model</param>
        /// <returns>
        /// Задача представляет асинхронную операцию
        /// The task result contains the shipment list model
        /// </returns>
        Task<ShipmentListModel> PrepareShipmentListModelAsync(ShipmentSearchModel searchModel);

        /// <summary>
        /// Prepare shipment model
        /// </summary>
        /// <param name="model">Shipment model</param>
        /// <param name="shipment">Shipment</param>
        /// <param name="order">Order</param>
        /// <param name="excludeProperties">Whether to exclude populating of some properties of model</param>
        /// <returns>
        /// Задача представляет асинхронную операцию
        /// The task result contains the shipment model
        /// </returns>
        Task<ShipmentModel> PrepareShipmentModelAsync(ShipmentModel model, Shipment shipment, Order order, bool excludeProperties = false);

        /// <summary>
        /// Prepare paged order shipment list model
        /// </summary>
        /// <param name="searchModel">Order shipment search model</param>
        /// <param name="order">Order</param>
        /// <returns>
        /// Задача представляет асинхронную операцию
        /// The task result contains the order shipment list model
        /// </returns>
        Task<OrderShipmentListModel> PrepareOrderShipmentListModelAsync(OrderShipmentSearchModel searchModel, Order order);

        /// <summary>
        /// Prepare paged shipment item list model
        /// </summary>
        /// <param name="searchModel">Shipment item search model</param>
        /// <param name="shipment">Shipment</param>
        /// <returns>
        /// Задача представляет асинхронную операцию
        /// The task result contains the shipment item list model
        /// </returns>
        Task<ShipmentItemListModel> PrepareShipmentItemListModelAsync(ShipmentItemSearchModel searchModel, Shipment shipment);

        /// <summary>
        /// Prepare paged order note list model
        /// </summary>
        /// <param name="searchModel">Order note search model</param>
        /// <param name="order">Order</param>
        /// <returns>
        /// Задача представляет асинхронную операцию
        /// The task result contains the order note list model
        /// </returns>
        Task<OrderNoteListModel> PrepareOrderNoteListModelAsync(OrderNoteSearchModel searchModel, Order order);

        /// <summary>
        /// Prepare bestseller brief search model
        /// </summary>
        /// <param name="searchModel">Bestseller brief search model</param>
        /// <returns>
        /// Задача представляет асинхронную операцию
        /// The task result contains the bestseller brief search model
        /// </returns>
        Task<BestsellerBriefSearchModel> PrepareBestsellerBriefSearchModelAsync(BestsellerBriefSearchModel searchModel);

        /// <summary>
        /// Prepare paged bestseller brief list model
        /// </summary>
        /// <param name="searchModel">Bestseller brief search model</param>
        /// <returns>
        /// Задача представляет асинхронную операцию
        /// The task result contains the bestseller brief list model
        /// </returns>
        Task<BestsellerBriefListModel> PrepareBestsellerBriefListModelAsync(BestsellerBriefSearchModel searchModel);

        /// <summary>
        /// Prepare order average line summary report list model
        /// </summary>
        /// <param name="searchModel">Order average line summary report search model</param>
        /// <returns>
        /// Задача представляет асинхронную операцию
        /// The task result contains the order average line summary report list model
        /// </returns>
        Task<OrderAverageReportListModel> PrepareOrderAverageReportListModelAsync(OrderAverageReportSearchModel searchModel);

        /// <summary>
        /// Prepare incomplete order report list model
        /// </summary>
        /// <param name="searchModel">Incomplete order report search model</param>
        /// <returns>
        /// Задача представляет асинхронную операцию
        /// The task result contains the incomplete order report list model
        /// </returns>
        Task<OrderIncompleteReportListModel> PrepareOrderIncompleteReportListModelAsync(OrderIncompleteReportSearchModel searchModel);
    }
}