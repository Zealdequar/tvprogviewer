using System.Collections.Generic;
using TVProgViewer.Core.Domain.Catalog;
using TVProgViewer.Core.Domain.Users;
using TVProgViewer.Core.Domain.Directory;
using TVProgViewer.Core.Domain.Messages;
using TVProgViewer.Core.Domain.Orders;

namespace TVProgViewer.Services.ExportImport
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
        /// <returns>Result in XML format</returns>
        string ExportManufacturersToXml(IList<Manufacturer> manufacturers);

        /// <summary>
        /// Export manufacturers to XLSX
        /// </summary>
        /// <param name="manufacturers">Manufactures</param>
        byte[] ExportManufacturersToXlsx(IEnumerable<Manufacturer> manufacturers);

        /// <summary>
        /// Export category list to XML
        /// </summary>
        /// <returns>Result in XML format</returns>
        string ExportCategoriesToXml();

        /// <summary>
        /// Export categories to XLSX
        /// </summary>
        /// <param name="categories">Categories</param>
        byte[] ExportCategoriesToXlsx(IList<Category> categories);

        /// <summary>
        /// Export product list to XML
        /// </summary>
        /// <param name="products">Products</param>
        /// <returns>Result in XML format</returns>
        string ExportProductsToXml(IList<Product> products);

        /// <summary>
        /// Export products to XLSX
        /// </summary>
        /// <param name="products">Products</param>
        byte[] ExportProductsToXlsx(IEnumerable<Product> products);

        /// <summary>
        /// Export order list to XML
        /// </summary>
        /// <param name="orders">Orders</param>
        /// <returns>Result in XML format</returns>
        string ExportOrdersToXml(IList<Order> orders);

        /// <summary>
        /// Export orders to XLSX
        /// </summary>
        /// <param name="orders">Orders</param>
        byte[] ExportOrdersToXlsx(IList<Order> orders);

        /// <summary>
        /// Export User list to XLSX
        /// </summary>
        /// <param name="Users">Users</param>
        byte[] ExportUsersToXlsx(IList<User> Users);

        /// <summary>
        /// Export User list to XML
        /// </summary>
        /// <param name="Users">Users</param>
        /// <returns>Result in XML format</returns>
        string ExportUsersToXml(IList<User> Users);

        /// <summary>
        /// Export newsletter subscribers to TXT
        /// </summary>
        /// <param name="subscriptions">Subscriptions</param>
        /// <returns>Result in TXT (string) format</returns>
        string ExportNewsletterSubscribersToTxt(IList<NewsLetterSubscription> subscriptions);

        /// <summary>
        /// Export states to TXT
        /// </summary>
        /// <param name="states">States</param>
        /// <returns>Result in TXT (string) format</returns>
        string ExportStatesToTxt(IList<StateProvince> states);

        /// <summary>
        /// Export User info (GDPR request) to XLSX 
        /// </summary>
        /// <param name="User">User</param>
        /// <param name="storeId">Store identifier</param>
        /// <returns>User GDPR info</returns>
        byte[] ExportUserGdprInfoToXlsx(User User, int storeId);
    }
}
