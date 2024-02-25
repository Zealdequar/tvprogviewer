﻿using System;
using System.Threading.Tasks;
using TvProgViewer.Core;
using TvProgViewer.Core.Domain.Users;
using TvProgViewer.Core.Domain.Orders;
using TvProgViewer.Core.Domain.Payments;
using TvProgViewer.Core.Domain.Shipping;
using TvProgViewer.Services.Orders;

namespace TvProgViewer.Services.Users
{
    /// <summary>
    /// User report service interface
    /// </summary>
    public partial interface IUserReportService
    {
        /// <summary>
        /// Get best users
        /// </summary>
        /// <param name="createdFromUtc">Order created date from (UTC); null to load all records</param>
        /// <param name="createdToUtc">Order created date to (UTC); null to load all records</param>
        /// <param name="os">Order status; null to load all records</param>
        /// <param name="ps">Order payment status; null to load all records</param>
        /// <param name="ss">Order shipment status; null to load all records</param>
        /// <param name="orderBy">1 - order by order total, 2 - order by number of orders</param>
        /// <param name="pageIndex">Page index</param>
        /// <param name="pageSize">Page size</param>
        /// <returns>
        /// Задача представляет асинхронную операцию
        /// The task result contains the report
        /// </returns>
        Task<IPagedList<BestUserReportLine>> GetBestUsersReportAsync(DateTime? createdFromUtc,
            DateTime? createdToUtc, OrderStatus? os, PaymentStatus? ps, ShippingStatus? ss, OrderByEnum orderBy,
            int pageIndex = 0, int pageSize = 214748364);

        /// <summary>
        /// Gets a report of users registered in the last days
        /// </summary>
        /// <param name="days">Users registered in the last days</param>
        /// <returns>
        /// Задача представляет асинхронную операцию
        /// The task result contains the number of registered users
        /// </returns>
        Task<int> GetRegisteredUsersReportAsync(int days);
    }
}