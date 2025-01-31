﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TvProgViewer.Core;
using TvProgViewer.Core.Domain.Catalog;
using TvProgViewer.Core.Domain.Users;
using TvProgViewer.Core.Domain.Vendors;
using TvProgViewer.Data;
using TvProgViewer.Services.Html;

namespace TvProgViewer.Services.Vendors
{
    /// <summary>
    /// Vendor service
    /// </summary>
    public partial class VendorService : IVendorService
    {
        #region Fields

        private readonly IHtmlFormatter _htmlFormatter;
        private readonly IRepository<User> _userRepository;
        private readonly IRepository<TvChannel> _tvChannelRepository;
        private readonly IRepository<Vendor> _vendorRepository;
        private readonly IRepository<VendorNote> _vendorNoteRepository;

        #endregion

        #region Ctor

        public VendorService(IHtmlFormatter htmlFormatter,
            IRepository<User> userRepository,
            IRepository<TvChannel> tvChannelRepository,
            IRepository<Vendor> vendorRepository,
            IRepository<VendorNote> vendorNoteRepository)
        {
            _htmlFormatter = htmlFormatter;
            _userRepository = userRepository;
            _tvChannelRepository = tvChannelRepository;
            _vendorRepository = vendorRepository;
            _vendorNoteRepository = vendorNoteRepository;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Gets a vendor by vendor identifier
        /// </summary>
        /// <param name="vendorId">Vendor identifier</param>
        /// <returns>
        /// Задача представляет асинхронную операцию
        /// The task result contains the vendor
        /// </returns>
        public virtual async Task<Vendor> GetVendorByIdAsync(int vendorId)
        {
            return await _vendorRepository.GetByIdAsync(vendorId, cache => default);
        }

        /// <summary>
        /// Gets a vendor by tvChannel identifier
        /// </summary>
        /// <param name="tvChannelId">TvChannel identifier</param>
        /// <returns>
        /// Задача представляет асинхронную операцию
        /// The task result contains the vendor
        /// </returns>
        public virtual async Task<Vendor> GetVendorByTvChannelIdAsync(int tvChannelId)
        {
            if (tvChannelId == 0)
                return null;

            return await (from v in _vendorRepository.Table
                    join p in _tvChannelRepository.Table on v.Id equals p.VendorId
                    where p.Id == tvChannelId
                    select v).FirstOrDefaultAsync();
        }

        /// <summary>
        /// Gets vendors by tvChannel identifiers
        /// </summary>
        /// <param name="tvChannelIds">Array of tvChannel identifiers</param>
        /// <returns>
        /// Задача представляет асинхронную операцию
        /// The task result contains the vendors
        /// </returns>
        public virtual async Task<IList<Vendor>> GetVendorsByTvChannelIdsAsync(int[] tvChannelIds)
        {
            if (tvChannelIds is null)
                throw new ArgumentNullException(nameof(tvChannelIds));

            return await (from v in _vendorRepository.Table
                    join p in _tvChannelRepository.Table on v.Id equals p.VendorId
                    where tvChannelIds.Contains(p.Id) && !v.Deleted && v.Active
                    select v).Distinct().ToListAsync();
        }

        /// <summary>
        /// Gets a vendors by users identifiers
        /// </summary>
        /// <param name="userIds">Array of user identifiers</param>
        /// <returns>
        /// Задача представляет асинхронную операцию
        /// The task result contains the vendors
        /// </returns>
        public virtual async Task<IList<Vendor>> GetVendorsByUserIdsAsync(int[] userIds)
        {
            if (userIds is null)
                throw new ArgumentNullException(nameof(userIds));

            return await (from v in _vendorRepository.Table
                join c in _userRepository.Table on v.Id equals c.VendorId
                where userIds.Contains(c.Id) && !v.Deleted && v.Active
                select v).Distinct().ToListAsync();
        }

        /// <summary>
        /// Delete a vendor
        /// </summary>
        /// <param name="vendor">Vendor</param>
        /// <returns>Задача представляет асинхронную операцию</returns>
        public virtual async Task DeleteVendorAsync(Vendor vendor)
        {
            await _vendorRepository.DeleteAsync(vendor);
        }

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
        public virtual async Task<IPagedList<Vendor>> GetAllVendorsAsync(string name = "", string email = "", int pageIndex = 0, int pageSize = int.MaxValue, bool showHidden = false)
        {
            var vendors = await _vendorRepository.GetAllPagedAsync(query =>
            {
                if (!string.IsNullOrWhiteSpace(name))
                    query = query.Where(v => v.Name.Contains(name));

                if (!string.IsNullOrWhiteSpace(email))
                    query = query.Where(v => v.Email.Contains(email));

                if (!showHidden)
                    query = query.Where(v => v.Active);

                query = query.Where(v => !v.Deleted);
                query = query.OrderBy(v => v.DisplayOrder).ThenBy(v => v.Name).ThenBy(v => v.Email);

                return query;
            }, pageIndex, pageSize);

            return vendors;
        }

        /// <summary>
        /// Inserts a vendor
        /// </summary>
        /// <param name="vendor">Vendor</param>
        /// <returns>Задача представляет асинхронную операцию</returns>
        public virtual async Task InsertVendorAsync(Vendor vendor)
        {
            await _vendorRepository.InsertAsync(vendor);
        }

        /// <summary>
        /// Updates the vendor
        /// </summary>
        /// <param name="vendor">Vendor</param>
        /// <returns>Задача представляет асинхронную операцию</returns>
        public virtual async Task UpdateVendorAsync(Vendor vendor)
        {
            await _vendorRepository.UpdateAsync(vendor);
        }

        /// <summary>
        /// Gets a vendor note
        /// </summary>
        /// <param name="vendorNoteId">The vendor note identifier</param>
        /// <returns>
        /// Задача представляет асинхронную операцию
        /// The task result contains the vendor note
        /// </returns>
        public virtual async Task<VendorNote> GetVendorNoteByIdAsync(int vendorNoteId)
        {
            return await _vendorNoteRepository.GetByIdAsync(vendorNoteId, cache => default);
        }

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
        public virtual async Task<IPagedList<VendorNote>> GetVendorNotesByVendorAsync(int vendorId, int pageIndex = 0, int pageSize = int.MaxValue)
        {
            var query = _vendorNoteRepository.Table.Where(vn => vn.VendorId == vendorId);

            query = query.OrderBy(v => v.CreatedOnUtc).ThenBy(v => v.Id);

            return await query.ToPagedListAsync(pageIndex, pageSize);
        }

        /// <summary>
        /// Deletes a vendor note
        /// </summary>
        /// <param name="vendorNote">The vendor note</param>
        /// <returns>Задача представляет асинхронную операцию</returns>
        public virtual async Task DeleteVendorNoteAsync(VendorNote vendorNote)
        {
            await _vendorNoteRepository.DeleteAsync(vendorNote);
        }

        /// <summary>
        /// Inserts a vendor note
        /// </summary>
        /// <param name="vendorNote">Vendor note</param>
        /// <returns>Задача представляет асинхронную операцию</returns>
        public virtual async Task InsertVendorNoteAsync(VendorNote vendorNote)
        {
            await _vendorNoteRepository.InsertAsync(vendorNote);
        }

        /// <summary>
        /// Formats the vendor note text
        /// </summary>
        /// <param name="vendorNote">Vendor note</param>
        /// <returns>Formatted text</returns>
        public virtual string FormatVendorNoteText(VendorNote vendorNote)
        {
            if (vendorNote == null)
                throw new ArgumentNullException(nameof(vendorNote));

            var text = vendorNote.Note;

            if (string.IsNullOrEmpty(text))
                return string.Empty;

            text = _htmlFormatter.FormatText(text, false, true, false, false, false, false);

            return text;
        }

        #endregion
    }
}