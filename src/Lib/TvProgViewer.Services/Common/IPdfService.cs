using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using TvProgViewer.Core.Domain.Catalog;
using TvProgViewer.Core.Domain.Localization;
using TvProgViewer.Core.Domain.Orders;
using TvProgViewer.Core.Domain.Shipping;
using TvProgViewer.Core.Domain.Stores;
using TvProgViewer.Core.Domain.Vendors;

namespace TvProgViewer.Services.Common
{
    /// <summary>
    /// User service interface
    /// </summary>
    public partial interface IPdfService
    {
        /// <summary>
        /// Write PDF invoice to the specified stream
        /// </summary>
        /// <param name="stream">Stream to save PDF</param>
        /// <param name="order">Order</param>
        /// <param name="language">Language; null to use a language used when placing an order</param>
        /// <param name="store">Store</param>
        /// <param name="vendor">Vendor to limit tvchannels; null to print all tvchannels. If specified, then totals won't be printed</param>
        /// <returns>
        /// Задача представляет асинхронную операцию
        /// </returns>
        Task PrintOrderToPdfAsync(Stream stream, Order order, Language language = null, Store store = null, Vendor vendor = null);

        /// <summary>
        /// Write ZIP archive with invoices to the specified stream
        /// </summary>
        /// <param name="stream">Stream</param>
        /// <param name="orders">Orders</param>
        /// <param name="language">Language; null to use a language used when placing an order</param>
        /// <param name="vendor">Vendor to limit tvchannels; null to print all tvchannels. If specified, then totals won't be printed</param>
        /// <returns>Задача представляет асинхронную операцию</returns>
        Task PrintOrdersToPdfAsync(Stream stream, IList<Order> orders, Language language = null, Vendor vendor = null);

        /// <summary>
        /// Write packaging slip to the specified stream
        /// </summary>
        /// <param name="stream">Stream</param>
        /// <param name="shipment">Shipment</param>
        /// <param name="language">Language; null to use a language used when placing an order</param>
        /// <returns>Задача представляет асинхронную операцию</returns>
        Task PrintPackagingSlipToPdfAsync(Stream stream, Shipment shipment, Language language = null);

        /// <summary>
        /// Write ZIP archive with packaging slips to the specified stream
        /// </summary>
        /// <param name="stream">Stream</param>
        /// <param name="shipments">Shipments</param>
        /// <param name="language">Language; null to use a language used when placing an order</param>
        /// <returns>Задача представляет асинхронную операцию</returns>
        Task PrintPackagingSlipsToPdfAsync(Stream stream, IList<Shipment> shipments, Language language = null);

        /// <summary>
        /// Write PDF catalog to the specified stream
        /// </summary>
        /// <param name="stream">Stream</param>
        /// <param name="tvchannels">TvChannels</param>
        /// <returns>Задача представляет асинхронную операцию</returns>
        Task PrintTvChannelsToPdfAsync(Stream stream, IList<TvChannel> tvchannels);

        /// <summary>
        /// Export an order to PDF and save to disk
        /// </summary>
        /// <param name="order">Order</param>
        /// <param name="language">Language identifier; null to use a language used when placing an order</param>
        /// <param name="vendor">Vendor to limit tvchannels; null to print all tvchannels. If specified, then totals won't be printed</param>
        /// <returns>
        /// The task result contains a path of generated file
        /// </returns>
        Task<string> SaveOrderPdfToDiskAsync(Order order, Language language = null, Vendor vendor = null);
    }
}