﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Microsoft.AspNetCore.Http;
using TvProgViewer.Core;
using TvProgViewer.Core.Caching;
using TvProgViewer.Core.Configuration;
using TvProgViewer.Core.Domain.Catalog;
using TvProgViewer.Core.Domain.Media;
using TvProgViewer.Core.Infrastructure;
using TvProgViewer.Data;
using TvProgViewer.Services.Catalog;
using TvProgViewer.Services.Configuration;
using TvProgViewer.Services.Logging;
using TvProgViewer.Services.Seo;

namespace TvProgViewer.Services.Media
{
    /// <summary>
    /// Picture service for Windows Azure
    /// </summary>
    public partial class AzurePictureService : PictureService
    {
        #region Fields

        private static BlobContainerClient _blobContainerClient;
        private static BlobServiceClient _blobServiceClient;
        private static bool _azureBlobStorageAppendContainerName;
        private static bool _isInitialized;
        private static string _azureBlobStorageConnectionString;
        private static string _azureBlobStorageContainerName;
        private static string _azureBlobStorageEndPoint;

        private readonly IStaticCacheManager _staticCacheManager;
        private readonly MediaSettings _mediaSettings;

        private readonly object _locker = new();

        #endregion

        #region Ctor

        public AzurePictureService(AppSettings appSettings,
            IDownloadService downloadService,
            IHttpContextAccessor httpContextAccessor,
            ILogger logger,
            ITvProgFileProvider fileProvider,
            ITvChannelAttributeParser tvChannelAttributeParser,
            IRepository<Picture> pictureRepository,
            IRepository<PictureBinary> pictureBinaryRepository,
            IRepository<TvChannelPicture> tvChannelPictureRepository,
            ISettingService settingService,
            IStaticCacheManager staticCacheManager,
            IUrlRecordService urlRecordService,
            IWebHelper webHelper,
            MediaSettings mediaSettings)
            : base(downloadService,
                  httpContextAccessor,
                  logger,
                  fileProvider,
                  tvChannelAttributeParser,
                  pictureRepository,
                  pictureBinaryRepository,
                  tvChannelPictureRepository,
                  settingService,
                  urlRecordService,
                  webHelper,
                  mediaSettings)
        {
            _staticCacheManager = staticCacheManager;
            _mediaSettings = mediaSettings;

            OneTimeInit(appSettings);
        }

        #endregion

        #region Utilities

        /// <summary>
        /// Initialize cloud container
        /// </summary>
        /// <param name="appSettings">App settings</param>
        protected void OneTimeInit(AppSettings appSettings)
        {
            if (_isInitialized)
                return;

            if (string.IsNullOrEmpty(appSettings.Get<AzureBlobConfig>().ConnectionString))
                throw new Exception("Azure connection string for Blob is not specified");

            if (string.IsNullOrEmpty(appSettings.Get<AzureBlobConfig>().ContainerName))
                throw new Exception("Azure container name for Blob is not specified");

            if (string.IsNullOrEmpty(appSettings.Get<AzureBlobConfig>().EndPoint))
                throw new Exception("Azure end point for Blob is not specified");

            lock (_locker)
            {
                if (_isInitialized)
                    return;

                _azureBlobStorageAppendContainerName = appSettings.Get<AzureBlobConfig>().AppendContainerName;
                _azureBlobStorageConnectionString = appSettings.Get<AzureBlobConfig>().ConnectionString;
                _azureBlobStorageContainerName = appSettings.Get<AzureBlobConfig>().ContainerName.Trim().ToLowerInvariant();
                _azureBlobStorageEndPoint = appSettings.Get<AzureBlobConfig>().EndPoint.Trim().ToLowerInvariant().TrimEnd('/');

                _blobServiceClient = new BlobServiceClient(_azureBlobStorageConnectionString);
                _blobContainerClient = _blobServiceClient.GetBlobContainerClient(_azureBlobStorageContainerName);

                CreateCloudBlobContainer().GetAwaiter().GetResult();

                _isInitialized = true;
            }
        }

        /// <summary>
        /// Create cloud Blob container
        /// </summary>
        /// <returns>Задача представляет асинхронную операцию</returns>
        protected virtual async Task CreateCloudBlobContainer()
        {
            await _blobContainerClient.CreateIfNotExistsAsync(PublicAccessType.Blob);
        }

        /// <summary>
        /// Get picture (thumb) local path
        /// </summary>
        /// <param name="thumbFileName">Filename</param>
        /// <returns>
        /// Задача представляет асинхронную операцию
        /// The task result contains the local picture thumb path
        /// </returns>
        protected override Task<string> GetThumbLocalPathAsync(string thumbFileName)
        {
            var path = _azureBlobStorageAppendContainerName ? $"{_azureBlobStorageContainerName}/" : string.Empty;

            return Task.FromResult($"{_azureBlobStorageEndPoint}/{path}{thumbFileName}");
        }

        /// <summary>
        /// Get picture (thumb) URL 
        /// </summary>
        /// <param name="thumbFileName">Filename</param>
        /// <param name="storeLocation">Store location URL; null to use determine the current store location automatically</param>
        /// <returns>
        /// Задача представляет асинхронную операцию
        /// The task result contains the local picture thumb path
        /// </returns>
        protected override async Task<string> GetThumbUrlAsync(string thumbFileName, string storeLocation = null)
        {
            return await GetThumbLocalPathAsync(thumbFileName);
        }

        /// <summary>
        /// Initiates an asynchronous operation to delete picture thumbs
        /// </summary>
        /// <param name="picture">Picture</param>
        /// <returns>Задача представляет асинхронную операцию</returns>
        protected override async Task DeletePictureThumbsAsync(Picture picture)
        {
            //create a string containing the Blob name prefix
            var prefix = $"{picture.Id:0000000}";

            var tasks = new List<Task>();
            await foreach (var blob in _blobContainerClient.GetBlobsAsync(BlobTraits.All, BlobStates.All, prefix))
            {
                tasks.Add(_blobContainerClient.DeleteBlobIfExistsAsync(blob.Name, DeleteSnapshotsOption.IncludeSnapshots));
            }
            await Task.WhenAll(tasks);

            await _staticCacheManager.RemoveByPrefixAsync(TvProgMediaDefaults.ThumbsExistsPrefix);
        }

        /// <summary>
        /// Initiates an asynchronous operation to get a value indicating whether some file (thumb) already exists
        /// </summary>
        /// <param name="thumbFilePath">Thumb file path</param>
        /// <param name="thumbFileName">Thumb file name</param>
        /// <returns>
        /// Задача представляет асинхронную операцию
        /// The task result contains the result
        /// </returns>
        protected override async Task<bool> GeneratedThumbExistsAsync(string thumbFilePath, string thumbFileName)
        {
            try
            {
                var key = _staticCacheManager.PrepareKeyForDefaultCache(TvProgMediaDefaults.ThumbExistsCacheKey, thumbFileName);

                return await _staticCacheManager.GetAsync(key, async () =>
                {
                    return await _blobContainerClient.GetBlobClient(thumbFileName).ExistsAsync();
                });
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// Initiates an asynchronous operation to save a value indicating whether some file (thumb) already exists
        /// </summary>
        /// <param name="thumbFilePath">Thumb file path</param>
        /// <param name="thumbFileName">Thumb file name</param>
        /// <param name="mimeType">MIME type</param>
        /// <param name="binary">Picture binary</param>
        /// <returns>Задача представляет асинхронную операцию</returns>
        protected override async Task SaveThumbAsync(string thumbFilePath, string thumbFileName, string mimeType, byte[] binary)
        {
            var blobClient = _blobContainerClient.GetBlobClient(thumbFileName);
            await using var ms = new MemoryStream(binary);

            //set mime type
            BlobHttpHeaders headers = null;
            if (!string.IsNullOrWhiteSpace(mimeType))
            {
                headers = new BlobHttpHeaders
                {
                    ContentType = mimeType
                };
            }

            //set cache control
            if (!string.IsNullOrWhiteSpace(_mediaSettings.AzureCacheControlHeader))
            {
                headers ??= new BlobHttpHeaders();
                headers.CacheControl = _mediaSettings.AzureCacheControlHeader;
            }

            if (headers is null)
                //We must explicitly indicate through the parameter that the object needs to be overwritten if it already exists
                //See more: https://github.com/Azure/azure-sdk-for-net/issues/9470
                await blobClient.UploadAsync(ms, overwrite: true);
            else
                await blobClient.UploadAsync(ms, new BlobUploadOptions { HttpHeaders = headers });

            await _staticCacheManager.RemoveByPrefixAsync(TvProgMediaDefaults.ThumbsExistsPrefix);
        }

        #endregion
    }
}