﻿using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using TvProgViewer.Core.Domain.Media;
using TvProgViewer.Data;

namespace TvProgViewer.Services.Media
{
    /// <summary>
    /// Download service
    /// </summary>
    public partial class DownloadService : IDownloadService
    {
        #region Fields

        private readonly IRepository<Download> _downloadRepository;

        #endregion

        #region Ctor

        public DownloadService(IRepository<Download> downloadRepository)
        {
            _downloadRepository = downloadRepository;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Gets a download
        /// </summary>
        /// <param name="downloadId">Download identifier</param>
        /// <returns>
        /// Задача представляет асинхронную операцию
        /// The task result contains the download
        /// </returns>
        public virtual async Task<Download> GetDownloadByIdAsync(int downloadId)
        {
            return await _downloadRepository.GetByIdAsync(downloadId);
        }

        /// <summary>
        /// Gets a download by GUID
        /// </summary>
        /// <param name="downloadGuid">Download GUID</param>
        /// <returns>
        /// Задача представляет асинхронную операцию
        /// The task result contains the download
        /// </returns>
        public virtual async Task<Download> GetDownloadByGuidAsync(Guid downloadGuid)
        {
            if (downloadGuid == Guid.Empty)
                return null;

            var query = from o in _downloadRepository.Table
                        where o.DownloadGuid == downloadGuid
                        select o;

            return await query.FirstOrDefaultAsync();
        }

        /// <summary>
        /// Deletes a download
        /// </summary>
        /// <param name="download">Download</param>
        /// <returns>Задача представляет асинхронную операцию</returns>
        public virtual async Task DeleteDownloadAsync(Download download)
        {
            await _downloadRepository.DeleteAsync(download);
        }

        /// <summary>
        /// Inserts a download
        /// </summary>
        /// <param name="download">Download</param>
        /// <returns>Задача представляет асинхронную операцию</returns>
        public virtual async Task InsertDownloadAsync(Download download)
        {
            await _downloadRepository.InsertAsync(download);
        }

        /// <summary>
        /// Gets the download binary array
        /// </summary>
        /// <param name="file">File</param>
        /// <returns>
        /// Задача представляет асинхронную операцию
        /// The task result contains the download binary array
        /// </returns>
        public virtual async Task<byte[]> GetDownloadBitsAsync(IFormFile file)
        {
            await using var fileStream = file.OpenReadStream();
            await using var ms = new MemoryStream();
            await fileStream.CopyToAsync(ms);
            var fileBytes = ms.ToArray();

            return fileBytes;
        }

        #endregion
    }
}