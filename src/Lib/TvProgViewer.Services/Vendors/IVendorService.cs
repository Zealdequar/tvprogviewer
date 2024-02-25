﻿using System.Collections.Generic;
using System.Threading.Tasks;
using TvProgViewer.Core;
using TvProgViewer.Core.Domain.Vendors;

namespace TvProgViewer.Services.Vendors
{
    /// <summary>
    /// Vendor service interface
    /// </summary>
    public partial interface IVendorService
    {
        /// <summary>
        /// Gets a vendor by vendor identifier
        /// </summary>
        /// <param name="vendorId">Vendor identifier</param>
        /// <returns>
        /// Задача представляет асинхронную операцию
        /// The task result contains the vendor
        /// </returns>
        Task<Vendor> GetVendorByIdAsync(int vendorId);

        /// <summary>
        /// Gets a vendors by tvchannel identifiers
        /// </summary>
        /// <param name="tvchannelIds">Array of tvchannel identifiers</param>
        /// <returns>
        /// Задача представляет асинхронную операцию
        /// The task result contains the vendors
        /// </returns>
        Task<IList<Vendor>> GetVendorsByTvChannelIdsAsync(int[] tvchannelIds);

        /// <summary>
        /// Gets a vendors by users identifiers
        /// </summary>
        /// <param name="userIds">Array of user identifiers</param>
        /// <returns>
        /// Задача представляет асинхронную операцию
        /// The task result contains the vendors
        /// </returns>
        Task<IList<Vendor>> GetVendorsByUserIdsAsync(int[] userIds);

        /// <summary>
        /// Gets a vendor by tvchannel identifier
        /// </summary>
        /// <param name="tvchannelId">TvChannel identifier</param>
        /// <returns>
        /// Задача представляет асинхронную операцию
        /// The task result contains the vendor
        /// </returns>
        Task<Vendor> GetVendorByTvChannelIdAsync(int tvchannelId);

        /// <summary>
        /// Delete a vendor
        /// </summary>
        /// <param name="vendor">Vendor</param>
        /// <returns>Задача представляет асинхронную операцию</returns>
        Task DeleteVendorAsync(Vendor vendor);

        /// <summary>
        /// Gets all vendors
        /// </summary>
        /// <param name="name">Vendor name</param>
        /// <param name="email">Vendor email</param>
        /// <param name="pageIndex">Page index</param>
        /// <param name="pageSize">Page size</param>
        /// <param name="showHidden">A value indicating whether to show hidden records</param>
        /// <returns>
        /// Задача представляет асинхронную операцию
        /// The task result contains the vendors
        /// </returns>
        Task<IPagedList<Vendor>> GetAllVendorsAsync(string name = "", string email = "", int pageIndex = 0, int pageSize = int.MaxValue, bool showHidden = false);

        /// <summary>
        /// Inserts a vendor
        /// </summary>
        /// <param name="vendor">Vendor</param>
        /// <returns>Задача представляет асинхронную операцию</returns>
        Task InsertVendorAsync(Vendor vendor);

        /// <summary>
        /// Updates the vendor
        /// </summary>
        /// <param name="vendor">Vendor</param>
        /// <returns>Задача представляет асинхронную операцию</returns>
        Task UpdateVendorAsync(Vendor vendor);

        /// <summary>
        /// Gets a vendor note
        /// </summary>
        /// <param name="vendorNoteId">The vendor note identifier</param>
        /// <returns>
        /// Задача представляет асинхронную операцию
        /// The task result contains the vendor note
        /// </returns>
        Task<VendorNote> GetVendorNoteByIdAsync(int vendorNoteId);

        /// <summary>
        /// Gets all vendor notes
        /// </summary>
        /// <param name="vendorId">Vendor identifier</param>
        /// <param name="pageIndex">Page index</param>
        /// <param name="pageSize">Page size</param>
        /// <returns>
        /// Задача представляет асинхронную операцию
        /// The task result contains the vendor notes
        /// </returns>
        Task<IPagedList<VendorNote>> GetVendorNotesByVendorAsync(int vendorId, int pageIndex = 0, int pageSize = int.MaxValue);

        /// <summary>
        /// Deletes a vendor note
        /// </summary>
        /// <param name="vendorNote">The vendor note</param>
        /// <returns>Задача представляет асинхронную операцию</returns>
        Task DeleteVendorNoteAsync(VendorNote vendorNote);

        /// <summary>
        /// Inserts a vendor note
        /// </summary>
        /// <param name="vendorNote">Vendor note</param>
        /// <returns>Задача представляет асинхронную операцию</returns>
        Task InsertVendorNoteAsync(VendorNote vendorNote);

        /// <summary>
        /// Formats the vendor note text
        /// </summary>
        /// <param name="vendorNote">Vendor note</param>
        /// <returns>Formatted text</returns>
        string FormatVendorNoteText(VendorNote vendorNote);
    }
}