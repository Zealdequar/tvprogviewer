﻿using System.Collections.Generic;
using System.Threading.Tasks;
using TvProgViewer.Core.Domain.Catalog;
using TvProgViewer.Core.Domain.Users;
using TvProgViewer.Core.Domain.Directory;
using TvProgViewer.Core.Domain.Messages;
using TvProgViewer.Core.Domain.Orders;

namespace TvProgViewer.Services.ExportImport
{
    /// <summary>
    /// Export manager interface
    /// </summary>
    public partial interface IExportManager
    {
        /// <summary>
        /// Export manufacturer list to XML
        /// </summary>
        /// <param name="manufacturers">Manufacturers</param>
        /// <returns>
        /// Задача представляет асинхронную операцию
        /// The task result contains the result in XML format
        /// </returns>
        Task<string> ExportManufacturersToXmlAsync(IList<Manufacturer> manufacturers);

        /// <summary>
        /// Export manufacturers to XLSX
        /// </summary>
        /// <param name="manufacturers">Manufactures</param>
        /// <returns>Задача представляет асинхронную операцию</returns>
        Task<byte[]> ExportManufacturersToXlsxAsync(IEnumerable<Manufacturer> manufacturers);

        /// <summary>
        /// Export category list to XML
        /// </summary>
        /// <returns>
        /// Задача представляет асинхронную операцию
        /// The task result contains the result in XML format
        /// </returns>
        Task<string> ExportCategoriesToXmlAsync();

        /// <summary>
        /// Export categories to XLSX
        /// </summary>
        /// <param name="categories">Categories</param>
        /// <returns>Задача представляет асинхронную операцию</returns>
        Task<byte[]> ExportCategoriesToXlsxAsync(IList<Category> categories);

        /// <summary>
        /// Export tvChannel list to XML
        /// </summary>
        /// <param name="tvChannels">TvChannels</param>
        /// <returns>
        /// Задача представляет асинхронную операцию
        /// The task result contains the result in XML format
        /// </returns>
        Task<string> ExportTvChannelsToXmlAsync(IList<TvChannel> tvChannels);

        /// <summary>
        /// Export tvChannels to XLSX
        /// </summary>
        /// <param name="tvChannels">TvChannels</param>
        /// <returns>Задача представляет асинхронную операцию</returns>
        Task<byte[]> ExportTvChannelsToXlsxAsync(IEnumerable<TvChannel> tvChannels);

        /// <summary>
        /// Export order list to XML
        /// </summary>
        /// <param name="orders">Orders</param>
        /// <returns>
        /// Задача представляет асинхронную операцию
        /// The task result contains the result in XML format
        /// </returns>
        Task<string> ExportOrdersToXmlAsync(IList<Order> orders);

        /// <summary>
        /// Export orders to XLSX
        /// </summary>
        /// <param name="orders">Orders</param>
        /// <returns>Задача представляет асинхронную операцию</returns>
        Task<byte[]> ExportOrdersToXlsxAsync(IList<Order> orders);

        /// <summary>
        /// Export user list to XLSX
        /// </summary>
        /// <param name="users">Users</param>
        /// <returns>Задача представляет асинхронную операцию</returns>
        Task<byte[]> ExportUsersToXlsxAsync(IList<User> users);

        /// <summary>
        /// Export user list to XML
        /// </summary>
        /// <param name="users">Users</param>
        /// <returns>
        /// Задача представляет асинхронную операцию
        /// The task result contains the result in XML format
        /// </returns>
        Task<string> ExportUsersToXmlAsync(IList<User> users);

        /// <summary>
        /// Export newsletter subscribers to TXT
        /// </summary>
        /// <param name="subscriptions">Subscriptions</param>
        /// <returns>
        /// Задача представляет асинхронную операцию
        /// The task result contains the result in TXT (string) format
        /// </returns>
        Task<string> ExportNewsletterSubscribersToTxtAsync(IList<NewsLetterSubscription> subscriptions);

        /// <summary>
        /// Export states to TXT
        /// </summary>
        /// <param name="states">States</param>
        /// <returns>
        /// Задача представляет асинхронную операцию
        /// The task result contains the result in TXT (string) format
        /// </returns>
        Task<string> ExportStatesToTxtAsync(IList<StateProvince> states);

        /// <summary>
        /// Export user info (GDPR request) to XLSX 
        /// </summary>
        /// <param name="user">User</param>
        /// <param name="storeId">Store identifier</param>
        /// <returns>
        /// Задача представляет асинхронную операцию
        /// The task result contains the user GDPR info
        /// </returns>
        Task<byte[]> ExportUserGdprInfoToXlsxAsync(User user, int storeId);
    }
}
