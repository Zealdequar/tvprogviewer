using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using TvProgViewer.Core;
using TvProgViewer.Core.Domain.Directory;
using TvProgViewer.Core.Domain.Discounts;
using TvProgViewer.Core.Domain.Media;
using TvProgViewer.Plugin.Misc.Zettle.Domain;
using TvProgViewer.Plugin.Misc.Zettle.Domain.Api;
using TvProgViewer.Plugin.Misc.Zettle.Domain.Api.Image;
using TvProgViewer.Plugin.Misc.Zettle.Domain.Api.Inventory;
using TvProgViewer.Plugin.Misc.Zettle.Domain.Api.OAuth;
using TvProgViewer.Plugin.Misc.Zettle.Domain.Api.TvChannel;
using TvProgViewer.Plugin.Misc.Zettle.Domain.Api.Pusher;
using TvProgViewer.Plugin.Misc.Zettle.Domain.Api.Secure;
using TvProgViewer.Services.Catalog;
using TvProgViewer.Services.Configuration;
using TvProgViewer.Services.Directory;
using TvProgViewer.Services.Discounts;
using TvProgViewer.Services.Logging;
using TvProgViewer.Services.Media;

namespace TvProgViewer.Plugin.Misc.Zettle.Services
{
    /// <summary>
    /// Represents the plugin service
    /// </summary>
    public class ZettleService
    {
        #region Fields

        private readonly CurrencySettings _currencySettings;
        private readonly ICurrencyService _currencyService;
        private readonly IDiscountService _discountService;
        private readonly ILogger _logger;
        private readonly IPictureService _pictureService;
        private readonly ITvChannelAttributeParser _tvChannelAttributeParser;
        private readonly ITvChannelAttributeService _tvChannelAttributeService;
        private readonly ITvChannelService _tvChannelService;
        private readonly ISettingService _settingService;
        private readonly IWorkContext _workContext;
        private readonly MediaSettings _mediaSettings;
        private readonly ZettleHttpClient _zettleHttpClient;
        private readonly ZettleRecordService _zettleRecordService;
        private readonly ZettleSettings _zettleSettings;

        private Dictionary<string, string> _locations = new();

        #endregion

        #region Ctor

        public ZettleService(CurrencySettings currencySettings,
            ICurrencyService currencyService,
            IDiscountService discountService,
            ILogger logger,
            IPictureService pictureService,
            ITvChannelAttributeParser tvChannelAttributeParser,
            ITvChannelAttributeService tvChannelAttributeService,
            ITvChannelService tvChannelService,
            ISettingService settingService,
            IWorkContext workContext,
            MediaSettings mediaSettings,
            ZettleHttpClient zettleHttpClient,
            ZettleRecordService zettleRecordService,
            ZettleSettings zettleSettings)
        {
            _currencySettings = currencySettings;
            _currencyService = currencyService;
            _discountService = discountService;
            _logger = logger;
            _pictureService = pictureService;
            _tvChannelAttributeParser = tvChannelAttributeParser;
            _tvChannelAttributeService = tvChannelAttributeService;
            _tvChannelService = tvChannelService;
            _settingService = settingService;
            _workContext = workContext;
            _mediaSettings = mediaSettings;
            _zettleHttpClient = zettleHttpClient;
            _zettleRecordService = zettleRecordService;
            _zettleSettings = zettleSettings;
        }

        #endregion

        #region Utilities

        /// <summary>
        /// Handle function and get result
        /// </summary>
        /// <typeparam name="TResult">Result type</typeparam>
        /// <param name="function">Function</param>
        /// <param name="logErrors">Whether to log errors</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the result; error if exists
        /// </returns>
        private async Task<(TResult Result, string Error)> HandleFunctionAsync<TResult>(Func<Task<TResult>> function, bool logErrors = true)
        {
            try
            {
                //ensure that plugin is configured
                if (!IsConfigured(_zettleSettings))
                    throw new TvProgException("Plugin not configured");

                return (await function(), default);
            }
            catch (Exception exception)
            {
                var errorMessage = exception.Message;
                if (logErrors)
                {
                    var logMessage = $"{ZettleDefaults.SystemName} error: {Environment.NewLine}{errorMessage}";
                    await _logger.ErrorAsync(logMessage, exception, await _workContext.GetCurrentUserAsync());
                }

                return (default, errorMessage);
            }
        }

        #region Sync

        /// <summary>
        /// Import discounts to Zettle library
        /// </summary>
        /// <param name="log">Log message</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        private async Task ImportDiscountsAsync(StringBuilder log)
        {
            //if enabled
            if (!_zettleSettings.DiscountSyncEnabled)
                return;

            log.AppendLine("Add discounts...");

            var storeCurrency = await _currencyService.GetCurrencyByIdAsync(_currencySettings.PrimaryStoreCurrencyId);

            //add only assigned to order subtotal discounts
            var existingDiscounts = await _zettleHttpClient.RequestAsync<GetDiscountsRequest, DiscountList>(new());
            var discounts = await _discountService.GetAllDiscountsAsync(DiscountType.AssignedToOrderSubTotal, showHidden: true);
            var discountsToAdd = discounts
                .Where(discount => !existingDiscounts.Any(existingDiscount => existingDiscount.ExternalReference == discount.Id.ToString()))
                .ToList();

            foreach (var discount in discountsToAdd)
            {
                var request = new CreateDiscountRequest
                {
                    Uuid = GuidGenerator.GenerateTimeBasedGuid().ToString(),
                    Name = discount.Name,
                    Description = discount.Name,
                    ExternalReference = discount.Id.ToString()
                };
                if (!discount.UsePercentage)
                {
                    request.Amount = new Domain.Api.TvChannel.Discount.DiscountAmount
                    {
                        CurrencyId = storeCurrency.CurrencyCode.ToUpper(),
                        Amount = storeCurrency.CurrencyCode.ToUpper() switch
                        {
                            "JPY" or "ISK" => Convert.ToInt32(Math.Round(discount.DiscountAmount, 0)),
                            _ => Convert.ToInt32(Math.Round(discount.DiscountAmount * 100, 0))
                        }
                    };
                }
                else
                    request.Percentage = discount.DiscountPercentage;

                log.AppendLine($"\tAdd discount '{discount.Name}'");

                await _zettleHttpClient.RequestAsync<CreateDiscountRequest, ApiResponse>(request);
            }
        }

        /// <summary>
        /// Delete tvChannels from Zettle library
        /// </summary>
        /// <param name="log">Log message</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        private async Task ImportDeletedAsync(StringBuilder log)
        {
            log.AppendLine("Delete tvChannels...");

            //get records to delete
            var records = await _zettleRecordService
                .GetAllRecordsAsync(active: true, operationTypes: new List<OperationType> { OperationType.Delete });
            var idsToDelete = records
                .Where(record => !string.IsNullOrEmpty(record.Uuid) && record.TvChannelId > 0 && record.CombinationId == 0)
                .Select(record => record.Uuid)
                .Distinct()
                .ToList();

            if (idsToDelete.Any())
                log.AppendLine($"\tDelete {idsToDelete.Count} tvChannels (#{string.Join(", #", idsToDelete)})");

            //if needed, also delete all existing tvChannels
            if (_zettleSettings.DeleteBeforeImport)
            {
                var idsToKeep = (await _zettleRecordService.GetAllRecordsAsync(tvChannelOnly: true, active: true))
                    .Where(record => !string.IsNullOrEmpty(record.Uuid))
                    .Select(record => record.Uuid)
                    .Distinct()
                    .Except(idsToDelete)
                    .ToList();

                var tvChannels = await _zettleHttpClient.RequestAsync<GetTvChannelsRequest, TvChannelList>(new());
                var existingIds = tvChannels.Select(tvChannel => tvChannel.Uuid).ToList();

                idsToDelete.AddRange(existingIds.Except(idsToKeep).ToList());

                log.AppendLine($"\tAlso delete all existing library items before importing tvChannels");
            }

            idsToDelete = idsToDelete.Distinct().ToList();
            if (idsToDelete.Any())
                await _zettleHttpClient.RequestAsync<DeleteTvChannelsRequest, ApiResponse>(new DeleteTvChannelsRequest { TvChannelUuids = idsToDelete });

            await _zettleRecordService.DeleteRecordsAsync(records.Select(record => record.Id).ToList());
        }

        /// <summary>
        /// Change tvChannel images in Zettle library
        /// </summary>
        /// <param name="log">Log message</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        private async Task ImportImageChangedAsync(StringBuilder log)
        {
            log.AppendLine("Change images...");

            //upload new images
            var records = (await _zettleRecordService
                .GetAllRecordsAsync(active: true, operationTypes: new List<OperationType> { OperationType.ImageChanged }))
                .Where(record => record.ImageSyncEnabled && !string.IsNullOrEmpty(record.Uuid))
                .ToList();
            await UploadImagesAsync(records, true, log);

            //then update appropriate tvChannels
            var tvChannels = records
                .GroupBy(record => record.TvChannelId)
                .Select(group => new
                {
                    TvChannelRecord = group.FirstOrDefault(record => record.CombinationId == 0),
                    CombinationRecords = group.Where(record => record.CombinationId > 0 && !string.IsNullOrEmpty(record.VariantUuid)).ToList()
                })
                .ToList();
            foreach (var tvChannel in tvChannels)
            {
                var existingTvChannel = await _zettleHttpClient
                    .RequestAsync<GetTvChannelRequest, TvChannel>(new GetTvChannelRequest { Uuid = tvChannel.TvChannelRecord.Uuid });
                var request = new UpdateTvChannelRequest
                {
                    Uuid = existingTvChannel.Uuid,
                    Name = existingTvChannel.Name,
                    ETag = $"\"{existingTvChannel.ETag}\""
                };
                if (!tvChannel.CombinationRecords.Any())
                {
                    request.Presentation = new TvChannel.TvChannelPresentation { ImageUrl = tvChannel.TvChannelRecord?.ImageUrl };
                    request.Variants = new List<TvChannel.TvChannelVariant>
                    {
                        new TvChannel.TvChannelVariant { Uuid = tvChannel.TvChannelRecord.VariantUuid }
                    };
                }
                else
                {
                    request.Variants = tvChannel.CombinationRecords.Select(record => new TvChannel.TvChannelVariant
                    {
                        Uuid = record.VariantUuid,
                        Presentation = new TvChannel.TvChannelPresentation { ImageUrl = record.ImageUrl }
                    }).ToList();
                }

                log.AppendLine($"\tAdd image to tvChannel #{tvChannel.TvChannelRecord.TvChannelId}");

                await _zettleHttpClient.RequestAsync<UpdateTvChannelRequest, ApiResponse>(request);
            }
        }

        /// <summary>
        /// Update inventory tracking balances in Zettle library
        /// </summary>
        /// <param name="log">Log message</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        private async Task ImportInventoryTrackingAsync(StringBuilder log)
        {
            log.AppendLine("Update inventory tracking...");

            var records = (await _zettleRecordService
                .GetAllRecordsAsync(active: true, operationTypes: new List<OperationType> { OperationType.Update }))
                .Where(record => record.InventoryTrackingEnabled && !string.IsNullOrEmpty(record.Uuid))
                .ToList();
            if (!records.Any())
                return;

            var storeBalance = await _zettleHttpClient.RequestAsync<GetLocationInventoryBalanceRequest, LocationInventoryBalance>(new());

            var tvChannels = records
                .GroupBy(record => record.TvChannelId)
                .Select(group => new
                {
                    TvChannelRecord = group.FirstOrDefault(record => record.CombinationId == 0),
                    CombinationRecords = group.Where(record => record.CombinationId > 0 && !string.IsNullOrEmpty(record.VariantUuid)).ToList()
                })
                .Where(tvChannel => !storeBalance.TrackedTvChannels?.Contains(tvChannel.TvChannelRecord.Uuid, StringComparer.InvariantCultureIgnoreCase) ?? true)
                .ToList();
            if (!tvChannels.Any())
                return;

            var tvChannelChanges = new List<CreateTrackingRequest.TvChannelBalanceChange>();
            foreach (var tvChannel in tvChannels)
            {
                log.AppendLine($"\tStart inventory tracking for tvChannel #{tvChannel.TvChannelRecord.TvChannelId}");

                //get current quantity if exists
                var tvChannelQuantity = storeBalance.Variants
                    ?.FirstOrDefault(balance => balance.TvChannelUuid == tvChannel.TvChannelRecord.Uuid && balance.VariantUuid == tvChannel.TvChannelRecord.VariantUuid)
                    ?.Balance ?? 0;
                (ZettleRecord Record, int StockQuantity, int? QuantityAdjustment) tvChannelRecordToStart = (tvChannel.TvChannelRecord, tvChannelQuantity, null);
                var combinationRecordsToStart = new List<(ZettleRecord Record, int StockQuantity, int? QuantityAdjustment)>();
                foreach (var combinationRecord in tvChannel.CombinationRecords)
                {
                    //get current quantity if exists
                    var combinationQuantity = storeBalance.Variants
                        ?.FirstOrDefault(balance => balance.TvChannelUuid == combinationRecord.Uuid && balance.VariantUuid == combinationRecord.VariantUuid)
                        ?.Balance ?? 0;
                    combinationRecordsToStart.Add((combinationRecord, combinationQuantity, null));
                }
                var tvChannelChange = await PrepareInventoryBalanceChangeAsync(InventoryBalanceChangeType.StartTracking,
                    tvChannelRecordToStart, combinationRecordsToStart);
                if (tvChannelChange is not null)
                    tvChannelChanges.Add(tvChannelChange);
            }
            await UpdateInventoryBalanceAsync(InventoryBalanceChangeType.StartTracking, tvChannelChanges);
        }

        /// <summary>
        /// Create or update tvChannels in Zettle library
        /// </summary>
        /// <param name="log">Log message</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        private async Task<Import> ImportCreatedOrUpdatedAsync(StringBuilder log)
        {
            log.AppendLine("Create and update tvChannels...");

            //check currency match
            var storeCurrency = await _currencyService.GetCurrencyByIdAsync(_currencySettings.PrimaryStoreCurrencyId);
            var (accountInfo, _) = await GetAccountInfoAsync();
            var priceSyncAvailable = string.Equals(storeCurrency.CurrencyCode, accountInfo.Currency, StringComparison.InvariantCultureIgnoreCase);

            //prepare price function
            int preparePrice(decimal price) => storeCurrency.CurrencyCode.ToUpper() switch
            {
                "JPY" or "ISK" => Convert.ToInt32(Math.Round(price, 0)),
                _ => Convert.ToInt32(Math.Round(price * 100, 0))
            };

            Import import = null;
            var pageIndex = 0;
            while (true)
            {
                //we can add up to 2000 tvChannels per request, but when uploading images, this may be too much
                var records = await _zettleRecordService.GetAllRecordsAsync(active: true,
                    operationTypes: new List<OperationType> { OperationType.Create, OperationType.Update },
                    pageIndex: pageIndex++,
                    pageSize: _zettleSettings.ImportTvChannelsNumber);
                if (!records.Any())
                    return import;

                log.AppendLine($"\tPrepare {records.Count} records to import");

                //upload images if needed
                await UploadImagesAsync(records.ToList(), false, log);

                //prepare tvChannels to import
                var tvChannels = await _zettleRecordService.PrepareToSyncRecords(records.ToList()).SelectAwait(async tvChannel =>
                {
                    var request = new TvChannel
                    {
                        Uuid = tvChannel.Uuid,
                        ExternalReference = tvChannel.Sku,
                        Name = tvChannel.Name,
                        Id = tvChannel.Id,
                        Description = tvChannel.Description,
                        CreateWithDefaultTax = _zettleSettings.DefaultTaxEnabled,
                        Category = new TvChannel.TvChannelCategory
                        {
                            Name = tvChannel.CategoryName,
                            Uuid = GuidGenerator.GenerateTimeBasedGuid().ToString()
                        },
                        Metadata = new TvChannel.TvChannelMetadata
                        {
                            InPos = true,
                            Source = new TvChannel.TvChannelMetadata.TvChannelSource
                            {
                                External = true,
                                Name = ZettleDefaults.PartnerIdentifier
                            }
                        }
                    };

                    //set image
                    if (tvChannel.ImageSyncEnabled && !string.IsNullOrEmpty(tvChannel.ImageUrl))
                        request.Presentation = new TvChannel.TvChannelPresentation { ImageUrl = tvChannel.ImageUrl };

                    var combinationRecords = records
                        .Where(record => record.TvChannelId == tvChannel.Id && record.CombinationId != 0)
                        .ToList();
                    if (!combinationRecords.Any())
                    {
                        //a single variant
                        request.Variants = new List<TvChannel.TvChannelVariant>
                        {
                            new TvChannel.TvChannelVariant
                            {
                                Uuid = tvChannel.VariantUuid,
                                Name = tvChannel.Name,
                                Sku = tvChannel.Sku,
                                Description = tvChannel.Description,

                                //set the price if available
                                Price = tvChannel.PriceSyncEnabled && priceSyncAvailable
                                    ? new TvChannel.TvChannelVariant.TvChannelPrice
                                    {
                                        Amount = preparePrice(tvChannel.Price),
                                        CurrencyId =  accountInfo.Currency
                                    } : null
                            }
                        };
                    }
                    else
                    {
                        //or multi variants
                        var tvChannelCombinations = await _tvChannelAttributeService.GetAllTvChannelAttributeCombinationsAsync(tvChannel.Id);
                        var tvChannelAttributMappings = await _tvChannelAttributeService.GetTvChannelAttributeMappingsByTvChannelIdAsync(tvChannel.Id);
                        var tvChannelAttributes = await tvChannelAttributMappings.SelectAwait(async mapping =>
                        {
                            var tvChannelAttribute = await _tvChannelAttributeService.GetTvChannelAttributeByIdAsync(mapping.TvChannelAttributeId);
                            var tvChannelAttributeValues = await _tvChannelAttributeService.GetTvChannelAttributeValuesAsync(mapping.Id);
                            return new { Name = tvChannelAttribute.Name, Values = tvChannelAttributeValues.Select(value => value.Name).ToList() };
                        }).ToListAsync();

                        request.VariantOptionDefinitions = new TvChannel.TvChannelVariantDefinitions
                        {
                            Definitions = tvChannelAttributes.Select(attribute => new TvChannel.TvChannelVariantDefinitions.TvChannelVariantOptionDefinition
                            {
                                Name = attribute.Name,
                                Properties = attribute.Values.Select(value => new TvChannel.TvChannelVariantDefinitions.TvChannelVariantOptionDefinition.TvChannelVariantOptionProperty
                                {
                                    Value = value
                                }).ToList()
                            }).ToList()
                        };

                        var combinations = combinationRecords
                            .Join(tvChannelCombinations,
                                record => record.CombinationId,
                                combination => combination.Id,
                                (record, combination) => new { Record = record, Combination = combination })
                            .ToList();
                        request.Variants = await combinations.SelectAwait(async combination =>
                        {
                            var variant = new TvChannel.TvChannelVariant
                            {
                                Uuid = combination.Record.VariantUuid,
                                Name = tvChannel.Name,
                                Sku = combination.Combination.Sku,
                                Description = tvChannel.Description
                            };

                            //set image
                            if (combination.Record.ImageSyncEnabled && !string.IsNullOrEmpty(combination.Record.ImageUrl))
                                variant.Presentation = new TvChannel.TvChannelPresentation { ImageUrl = combination.Record.ImageUrl };

                            //set the price if available
                            if (combination.Record.PriceSyncEnabled && priceSyncAvailable)
                            {
                                variant.Price = new TvChannel.TvChannelVariant.TvChannelPrice
                                {
                                    Amount = preparePrice(combination.Combination.OverriddenPrice ?? tvChannel.Price),
                                    CurrencyId = accountInfo.Currency
                                };
                            }

                            variant.Options = await (await _tvChannelAttributeParser.ParseTvChannelAttributeMappingsAsync(combination.Combination.AttributesXml))
                                .SelectAwait(async mapping =>
                                {
                                    var attribute = await _tvChannelAttributeService.GetTvChannelAttributeByIdAsync(mapping.TvChannelAttributeId);
                                    var values = await _tvChannelAttributeParser.ParseTvChannelAttributeValuesAsync(combination.Combination.AttributesXml, mapping.Id);
                                    return new TvChannel.TvChannelVariant.TvChannelVariantOption { Name = attribute.Name, Value = values.FirstOrDefault()?.Name };
                                })
                                .ToListAsync();

                            return variant;
                        }).ToListAsync();
                    }
                    return request;
                }).ToListAsync();

                log.AppendLine($"\tImport {tvChannels.Count} tvChannels (#{string.Join(", #", tvChannels.Select(tvChannel => tvChannel.Id).ToList())})");

                import = await _zettleHttpClient.RequestAsync<CreateImportRequest, Import>(new CreateImportRequest { TvChannels = tvChannels });

                log.AppendLine($"\t\tImport ({import.Uuid}) created at {import.Created?.ToLongTimeString()}");
            }
        }

        /// <summary>
        /// Upload images for the passed records
        /// </summary>
        /// <param name="records">Records</param>
        /// <param name="update">Whether to update existing images</param>
        /// <param name="log">Log message</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        private async Task UploadImagesAsync(IList<ZettleRecord> records, bool update, StringBuilder log)
        {
            //ensure MediaSettings.UseAbsoluteImagePath is enabled (used for images uploading)
            if (!_mediaSettings.UseAbsoluteImagePath)
                throw new TvProgException("For the correct image uploading need to use absolute pictures path (MediaSettings.UseAbsoluteImagePath setting)");

            //prepare images to upload
            var recordsWithImages = await records
               .Where(record => record.ImageSyncEnabled && (update || string.IsNullOrEmpty(record.ImageUrl)))
               .SelectAwait(async record =>
               {
                   var tvChannel = await _tvChannelService.GetTvChannelByIdAsync(record.TvChannelId);
                   var combination = await _tvChannelAttributeService.GetTvChannelAttributeCombinationByIdAsync(record.CombinationId);
                   var picture = await _pictureService.GetTvChannelPictureAsync(tvChannel, combination?.AttributesXml);
                   var ext = await _pictureService.GetFileExtensionFromMimeTypeAsync(picture.MimeType);
                   var (url, _) = await _pictureService.GetPictureUrlAsync(picture);
                   return new { Record = record, Url = url, Format = ext };
               })
               .ToListAsync();
            var imagesToUpload = recordsWithImages.Select(record => new CreateImageRequest
            {
                ImageFormat = record.Format?.ToUpper().Replace("JPG", "JPEG"),
                ImageUrl = record.Url
            }).ToList();

            if (!imagesToUpload.Any())
                return;

            log.AppendLine($"\tUpload {recordsWithImages.Count} new images");

            //upload images
            var images = await _zettleHttpClient
                .RequestAsync<CreateImagesRequest, ImageList>(new CreateImagesRequest { ImageUploads = imagesToUpload });

            log.AppendLine($"\t{images.Uploaded?.Count ?? 0} images uploaded successfully and {images.Invalid?.Count ?? 0} failed to upload");

            //set uploaded images URLs to records
            var recordsToUpdate = images.Uploaded?
                .SelectMany(image =>
                {
                    var recordsWithUploadedImage = recordsWithImages
                        .Where(record => string.Equals(record.Url, image.Source, StringComparison.InvariantCultureIgnoreCase))
                        .Select(record => record.Record)
                        .ToList();
                    foreach (var record in recordsWithUploadedImage)
                    {
                        record.ImageUrl = image.ImageUrls?.FirstOrDefault();
                    }
                    return recordsWithUploadedImage;
                })
                .Distinct()
                .ToList();
            await _zettleRecordService.UpdateRecordsAsync(recordsToUpdate);
        }

        #region Inventory

        /// <summary>
        /// Prepare tvChannel inventory balance changes
        /// </summary>
        /// <param name="changeType">Inventory balance change type</param>
        /// <param name="tvChannelRecord">TvChannel record with initial stock quantity and qunatity adjustment</param>
        /// <param name="combinationRecords">Combination records with initial stock quantity and qunatity adjustment</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains list of balance changes
        /// </returns>
        private async Task<CreateTrackingRequest.TvChannelBalanceChange> PrepareInventoryBalanceChangeAsync(InventoryBalanceChangeType changeType,
            (ZettleRecord Record, int StockQuantity, int? QuantityAdjustment) tvChannelRecord,
            List<(ZettleRecord Record, int StockQuantity, int? QuantityAdjustment)> combinationRecords)
        {
            //ensure that inventory is tracked for the tvChannel
            var tvChannel = await _tvChannelService.GetTvChannelByIdAsync(tvChannelRecord.Record?.TvChannelId ?? 0);
            if (tvChannel is null || tvChannel.ManageInventoryMethod == Core.Domain.Catalog.ManageInventoryMethod.DontManageStock)
                return null;

            var tvChannelChange = new CreateTrackingRequest.TvChannelBalanceChange
            {
                TvChannelUuid = tvChannelRecord.Record.Uuid,
                TrackingStatusChange = changeType == InventoryBalanceChangeType.StartTracking ? "START_TRACKING" : "NO_CHANGE"
            };

            //Zettle Inventory service keeps track of inventory balances by moving tvChannel items between so-called locations
            var fromLocation = await (changeType switch
            {
                InventoryBalanceChangeType.StartTracking or InventoryBalanceChangeType.Restock => GetLocationAsync("SUPPLIER"),
                InventoryBalanceChangeType.Purchase or InventoryBalanceChangeType.Void => GetLocationAsync("STORE"),
                _ => GetLocationAsync("SUPPLIER")
            });
            var toLocation = await (changeType switch
            {
                InventoryBalanceChangeType.StartTracking or InventoryBalanceChangeType.Restock => GetLocationAsync("STORE"),
                InventoryBalanceChangeType.Purchase => GetLocationAsync("SOLD"),
                InventoryBalanceChangeType.Void => GetLocationAsync("BIN"),
                _ => GetLocationAsync("BIN")
            });

            if (!combinationRecords.Any())
            {
                //get initial quantity
                var quantity = changeType == InventoryBalanceChangeType.StartTracking
                    ? tvChannel.StockQuantity - tvChannelRecord.StockQuantity
                    : tvChannelRecord.QuantityAdjustment ?? 0;
                if (quantity != 0)
                {
                    tvChannelChange.VariantChanges = new List<CreateTrackingRequest.VariantBalanceChange>
                    {
                        new CreateTrackingRequest.VariantBalanceChange
                        {
                            FromLocationUuid = quantity > 0 ? fromLocation : toLocation,
                            ToLocationUuid = quantity > 0 ? toLocation : fromLocation,
                            VariantUuid = tvChannelRecord.Record.VariantUuid,
                            Change = Math.Abs(quantity)
                        }
                    };
                }
            }
            else
            {
                var combinations = await _tvChannelAttributeService.GetAllTvChannelAttributeCombinationsAsync(tvChannel.Id);
                tvChannelChange.VariantChanges = await combinationRecords.SelectAwait(async combinationRecord =>
                {
                    var combination = await _tvChannelAttributeService.GetTvChannelAttributeCombinationByIdAsync(combinationRecord.Record.CombinationId);

                    //get initial quantity
                    var quantity = changeType == InventoryBalanceChangeType.StartTracking
                        ? (tvChannel.ManageInventoryMethod == Core.Domain.Catalog.ManageInventoryMethod.ManageStockByAttributes
                        ? (combination?.StockQuantity ?? 0) - combinationRecord.StockQuantity
                        : (tvChannel.StockQuantity / combinations.Count) - combinationRecord.StockQuantity)
                        : combinationRecord.QuantityAdjustment ?? 0;
                    if (quantity == 0)
                        return null;

                    return new CreateTrackingRequest.VariantBalanceChange
                    {
                        FromLocationUuid = quantity > 0 ? fromLocation : toLocation,
                        ToLocationUuid = quantity > 0 ? toLocation : fromLocation,
                        VariantUuid = combinationRecord.Record.VariantUuid,
                        Change = Math.Abs(quantity)
                    };
                }).Where(variantChange => variantChange is not null).ToListAsync();
            }

            return tvChannelChange;
        }

        /// <summary>
        /// Update inventory balance
        /// </summary>
        /// <param name="changeType">Inventory balance change type</param>
        /// <param name="tvChannelChanges">List of tvChannel changes</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        private async Task UpdateInventoryBalanceAsync(InventoryBalanceChangeType changeType, List<CreateTrackingRequest.TvChannelBalanceChange> tvChannelChanges)
        {
            if (!tvChannelChanges.Any())
                return;

            var inventoryRequest = new CreateTrackingRequest
            {
                ReturnLocationUuid = await GetLocationAsync("STORE"),
                TvChannelChanges = tvChannelChanges,
                ExternalUuid = GuidGenerator.GenerateTimeBasedGuid().ToString()
            };

            //save external id to avoid a double change, we will check it when receive a webhook event
            _zettleSettings.InventoryTrackingIds.Add(inventoryRequest.ExternalUuid);
            await _settingService.SetSettingAsync($"{nameof(ZettleSettings)}.{nameof(ZettleSettings.InventoryTrackingIds)}", _zettleSettings.InventoryTrackingIds);

            //update balances
            await _zettleHttpClient.RequestAsync<CreateTrackingRequest, LocationInventoryBalance>(inventoryRequest);
        }

        /// <summary>
        /// Get location UUID by the passed type
        /// </summary>
        /// <param name="type">Location type</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains location UUID
        /// </returns>
        private async Task<string> GetLocationAsync(string type)
        {
            if (!_locations.TryGetValue(type, out var _))
            {
                var locationList = await _zettleHttpClient.RequestAsync<GetLocationsRequest, LocationList>(new());
                _locations = locationList.ToDictionary(location => location.Type?.ToUpper(), location => location.Uuid);
            }

            return _locations[type];
        }

        #endregion

        #endregion

        #endregion

        #region Methods

        /// <summary>
        /// Check whether the plugin is configured
        /// </summary>
        /// <param name="settings">Plugin settings</param>
        /// <returns>Result</returns>
        public static bool IsConfigured(ZettleSettings settings)
        {
            //Client ID and API Key are required to request services
            return !string.IsNullOrEmpty(settings?.ClientId) && !string.IsNullOrEmpty(settings?.ApiKey);
        }

        #region Account

        /// <summary>
        /// Get the authenticated user info
        /// </summary>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains user details; error message if exists
        /// </returns>
        public async Task<(UserInfo Result, string Error)> GetUserInfoAsync()
        {
            return await HandleFunctionAsync(async () => await _zettleHttpClient.RequestAsync<GetUserInfoRequest, UserInfo>(new()), false);
        }

        /// <summary>
        /// Get the merchant account info
        /// </summary>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains account details; error message if exists
        /// </returns>
        public async Task<(AccountInfo Result, string Error)> GetAccountInfoAsync()
        {
            return await HandleFunctionAsync(async () => await _zettleHttpClient.RequestAsync<GetAccountInfoRequest, AccountInfo>(new()), false);
        }

        /// <summary>
        /// Get the default tax rate
        /// </summary>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the default tax rate; error message if exists
        /// </returns>
        public async Task<(decimal? Result, string Error)> GetDefaultTaxRateAsync()
        {
            return await HandleFunctionAsync(async () =>
            {
                var taxRates = await _zettleHttpClient.RequestAsync<GetTaxRatesRequest, TaxRateList>(new());
                return taxRates.TaxRates?.FirstOrDefault(rate => rate.IsDefault == true)?.Percentage;
            });
        }

        /// <summary>
        /// Disconnect the app from an associated Zettle organisation
        /// </summary>
        /// <returns>A task that represents the asynchronous operation
        /// The task result contains disconnect result; error message if exists
        /// </returns>
        public async Task<(bool Result, string Error)> DisconnectAsync()
        {
            return await HandleFunctionAsync(async () =>
            {
                await _zettleHttpClient.RequestAsync<DeleteAppRequest, ApiResponse>(new());
                return true;
            }, false);
        }

        #endregion

        #region Webhooks

        /// <summary>
        /// Create webhook that receive events for the subscribed event types
        /// </summary>
        /// <param name="webhookUrl">Webhook URL</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the webhook; error message if exists
        /// </returns>
        public async Task<(Subscription Result, string Error)> CreateWebhookAsync(string webhookUrl)
        {
            return await HandleFunctionAsync(async () =>
            {
                //check whether the webhook already exists
                var webhooks = await _zettleHttpClient.RequestAsync<GetSubscriptionsRequest, SubscriptionList>(new());
                var existingWebhook = webhooks
                    ?.FirstOrDefault(webhook => webhook.Destination?.Equals(webhookUrl, StringComparison.InvariantCultureIgnoreCase) ?? false);
                if (existingWebhook is not null)
                    return existingWebhook;

                //or try to create the new one if doesn't
                var (accountInfo, _) = await GetAccountInfoAsync();
                var request = new CreateSubscriptionRequest
                {
                    Uuid = GuidGenerator.GenerateTimeBasedGuid().ToString(),
                    TransportName = "WEBHOOK",
                    EventNames = ZettleDefaults.WebhookEventNames,
                    Destination = webhookUrl,
                    ContactEmail = accountInfo?.ContactEmail
                };

                return await _zettleHttpClient.RequestAsync<CreateSubscriptionRequest, Subscription>(request);
            });
        }

        /// <summary>
        /// Delete webhook
        /// </summary>
        /// <returns>A task that represents the asynchronous operation</returns>
        public async Task DeleteWebhookAsync()
        {
            await HandleFunctionAsync(async () =>
            {
                var webhooks = await _zettleHttpClient.RequestAsync<GetSubscriptionsRequest, SubscriptionList>(new());
                var existingWebhook = webhooks
                    ?.FirstOrDefault(webhook => webhook.Destination?.Equals(_zettleSettings.WebhookUrl, StringComparison.InvariantCultureIgnoreCase) ?? false);
                if (existingWebhook is null)
                    return false;

                var request = new DeleteSubscriptionsRequest { Uuid = existingWebhook.Uuid };
                await _zettleHttpClient.RequestAsync<DeleteSubscriptionsRequest, ApiResponse>(request);

                return true;
            }, false);
        }

        /// <summary>
        /// Handle webhook request
        /// </summary>
        /// <param name="request">HTTP request</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        public async Task HandleWebhookAsync(Microsoft.AspNetCore.Http.HttpRequest request)
        {
            await HandleFunctionAsync(async () =>
            {
                using var streamReader = new StreamReader(request.Body);
                var requestContent = await streamReader.ReadToEndAsync();
                if (string.IsNullOrEmpty(requestContent))
                    throw new TvProgException("Webhook request content is empty");

                //get webhook message
                var message = JsonConvert.DeserializeObject<Message>(requestContent);

                //test message is sent during webhook initialization
                if (message.EventName == "TestMessage")
                    return true;

                if (string.IsNullOrEmpty(_zettleSettings.WebhookKey))
                    throw new TvProgException("Webhook is not set");

                //ensure that request is signed
                if (!request.Headers.TryGetValue(ZettleDefaults.SignatureHeader, out var signatures))
                    throw new TvProgException("Webhook request not signed by a signature header");

                var messageBytes = Encoding.UTF8.GetBytes($"{message.Timestamp}.{message.Payload}");
                var keyBytes = Encoding.UTF8.GetBytes(_zettleSettings.WebhookKey);
                using var cryptographer = new HMACSHA256(keyBytes);
                var hashBytes = cryptographer.ComputeHash(messageBytes);
                var encryptedString = BitConverter.ToString(hashBytes).Replace("-", "").ToLower();
                if (!signatures.Any(signature => signature.Equals(encryptedString, StringComparison.InvariantCultureIgnoreCase)))
                    throw new TvProgException("Webhook request isn't valid");

                switch (message.EventName)
                {
                    case "InventoryBalanceChanged":
                        {
                            var balanceInfo = JsonConvert.DeserializeObject<InventoryBalanceUpdate>(message.Payload);

                            //сhange initiated by the plugin
                            if (_zettleSettings.InventoryTrackingIds.Contains(balanceInfo.ExternalUuid, StringComparer.InvariantCultureIgnoreCase))
                            {
                                //keep external ids for a day in case of errors when processing webhook requests
                                var balanceChangeDate = balanceInfo.UpdateDetails.Timestamp ?? DateTime.UtcNow;
                                if (balanceChangeDate < DateTime.UtcNow.AddDays(-1))
                                {
                                    _zettleSettings.InventoryTrackingIds.Remove(balanceInfo.ExternalUuid);
                                    await _settingService.SetSettingAsync($"{nameof(ZettleSettings)}.{nameof(ZettleSettings.InventoryTrackingIds)}", _zettleSettings.InventoryTrackingIds);
                                }
                                break;
                            }

                            for (var i = 0; i < (balanceInfo.BalanceBefore ?? new()).Count; i++)
                            {
                                var balanceBefore = balanceInfo.BalanceBefore?.ElementAtOrDefault(i);
                                var balanceAfter = balanceInfo.BalanceAfter?.ElementAtOrDefault(i);

                                if (string.IsNullOrEmpty(balanceBefore?.TvChannelUuid) || string.IsNullOrEmpty(balanceAfter?.TvChannelUuid))
                                    continue;

                                if (balanceBefore.TvChannelUuid != balanceAfter.TvChannelUuid || balanceBefore.VariantUuid != balanceAfter.VariantUuid)
                                    continue;

                                if (!balanceBefore.Balance.HasValue || !balanceAfter.Balance.HasValue)
                                    continue;

                                var records = await _zettleRecordService.GetAllRecordsAsync(tvChannelUuid: balanceAfter.TvChannelUuid);
                                var tvChannelRecord = records.FirstOrDefault(record => string.Equals(record.VariantUuid, balanceAfter.VariantUuid, StringComparison.InvariantCultureIgnoreCase));
                                if (tvChannelRecord is null || !tvChannelRecord.Active || !tvChannelRecord.InventoryTrackingEnabled)
                                    continue;

                                //adjust inventory
                                var tvChannel = await _tvChannelService.GetTvChannelByIdAsync(tvChannelRecord.TvChannelId);
                                var combination = await _tvChannelAttributeService.GetTvChannelAttributeCombinationByIdAsync(tvChannelRecord.CombinationId);
                                var quantityToChange = balanceAfter.Balance.Value - balanceBefore.Balance.Value;
                                var logMessage = $"{ZettleDefaults.SystemName} update. Inventory balance changed at {balanceAfter.Created?.ToLongTimeString()}";
                                await _tvChannelService.AdjustInventoryAsync(tvChannel, quantityToChange, combination?.AttributesXml, logMessage);
                            }

                            break;
                        }
                    case "InventoryTrackingStopped":
                        {
                            var inventoryTrackingInfo = JsonConvert.DeserializeAnonymousType(message.Payload, new { TvChannelUuid = string.Empty });
                            if (string.IsNullOrEmpty(inventoryTrackingInfo.TvChannelUuid))
                                break;

                            //stop tracking
                            var records = (await _zettleRecordService.GetAllRecordsAsync(tvChannelUuid: inventoryTrackingInfo.TvChannelUuid)).ToList();
                            foreach (var record in records)
                            {
                                record.InventoryTrackingEnabled = false;
                                record.UpdatedOnUtc = DateTime.UtcNow;
                            }
                            await _zettleRecordService.UpdateRecordsAsync(records);

                            break;
                        }

                    case "TvChannelCreated":
                        {
                            //use this event only to start inventory tracking for tvChannel
                            var tvChannelInfo = JsonConvert.DeserializeObject<TvChannel>(message.Payload);
                            var records = await _zettleRecordService.GetAllRecordsAsync(tvChannelUuid: tvChannelInfo.Uuid);
                            var tvChannelRecord = records.FirstOrDefault(record => record.CombinationId == 0);
                            if (tvChannelRecord is null || !tvChannelRecord.Active || !tvChannelRecord.InventoryTrackingEnabled)
                                break;

                            var storeBalance = await _zettleHttpClient
                                .RequestAsync<GetLocationInventoryBalanceRequest, LocationInventoryBalance>(new());
                            var trackingStarted = storeBalance.TrackedTvChannels
                                ?.Contains(tvChannelRecord.Uuid, StringComparer.InvariantCultureIgnoreCase);
                            if (trackingStarted ?? true)
                                break;

                            var combinationRecords = records.Where(record => record.CombinationId != 0).ToList();
                            var combinationRecordsToStart = new List<(ZettleRecord Record, int StockQuantity, int? QuantityAdjustment)>();
                            foreach (var combinationRecord in combinationRecords)
                            {
                                combinationRecordsToStart.Add((combinationRecord, 0, null));
                            }
                            (ZettleRecord Record, int StockQuantity, int? QuantityAdjustment) tvChannelRecordToStart = (tvChannelRecord, 0, null);
                            var tvChannelChange = await PrepareInventoryBalanceChangeAsync(InventoryBalanceChangeType.StartTracking,
                                tvChannelRecordToStart, combinationRecordsToStart);
                            if (tvChannelChange is null)
                                break;

                            await UpdateInventoryBalanceAsync(InventoryBalanceChangeType.StartTracking, new List<CreateTrackingRequest.TvChannelBalanceChange> { tvChannelChange });

                            break;
                        }

                    case "ApplicationConnectionRemoved":
                        {
                            var applicationInfo = JsonConvert.DeserializeAnonymousType(message.Payload, new { Type = string.Empty });
                            if (string.IsNullOrEmpty(applicationInfo.Type))
                                break;

                            var warning = applicationInfo.Type;
                            if (applicationInfo.Type.Equals("ApplicationConnectionRemoved", StringComparison.InvariantCultureIgnoreCase) ||
                                applicationInfo.Type.Equals("PersonalAssertionDeleted", StringComparison.InvariantCultureIgnoreCase))
                            {
                                warning = "The application was disconnected from PayPal Zettle organization. You need to reconfigure the plugin.";

                                _zettleSettings.ClientId = string.Empty;
                                _zettleSettings.ApiKey = string.Empty;
                                _zettleSettings.WebhookUrl = string.Empty;
                                _zettleSettings.WebhookKey = string.Empty;
                                _zettleSettings.ImportId = string.Empty;
                                _zettleSettings.InventoryTrackingIds = new();
                                await _settingService.SaveSettingAsync(_zettleSettings);
                            }
                            await _logger.WarningAsync($"{ZettleDefaults.SystemName}. {warning}");

                            break;
                        }

                    default:
                        throw new TvProgException($"Unknown webhook resource type '{message.EventName}'");
                }

                return true;
            });
        }

        #endregion

        #region Sync

        /// <summary>
        /// Get last import details
        /// </summary>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the import details; error message if exists
        /// </returns>
        public async Task<(Import Result, string Error)> GetImportAsync()
        {
            return await HandleFunctionAsync(async () =>
            {
                return await _zettleHttpClient.RequestAsync<GetImportRequest, Import>(new() { ImportUuid = _zettleSettings.ImportId });
            }, false);
        }

        /// <summary>
        /// Start tvChannels import
        /// </summary>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the import details; error message if exists
        /// </returns>
        public async Task<(Import Result, string Error)> ImportAsync()
        {
            return await HandleFunctionAsync(async () =>
            {
                var log = new StringBuilder($"{ZettleDefaults.SystemName} information.{Environment.NewLine}");
                log.AppendLine($"Synchronization started at {DateTime.UtcNow.ToLongTimeString()} UTC");

                await ImportDiscountsAsync(log);

                await ImportDeletedAsync(log);

                await ImportImageChangedAsync(log);

                await ImportInventoryTrackingAsync(log);

                var import = await ImportCreatedOrUpdatedAsync(log);

                if (!string.IsNullOrEmpty(import?.Uuid))
                {
                    //save import id for future use
                    await _settingService.SetSettingAsync($"{nameof(ZettleSettings)}.{nameof(ZettleSettings.ImportId)}", import?.Uuid);

                    //refresh records
                    var records = await _zettleRecordService.GetAllRecordsAsync(active: true,
                        operationTypes: new List<OperationType> { OperationType.Create, OperationType.Update, OperationType.ImageChanged });
                    foreach (var record in records)
                    {
                        record.OperationType = OperationType.None;
                        record.UpdatedOnUtc = DateTime.UtcNow;
                    }
                    await _zettleRecordService.UpdateRecordsAsync(records.ToList());
                }

                log.AppendLine($"Synchronization finished at {DateTime.UtcNow.ToLongTimeString()} UTC");

                if (_zettleSettings.LogSyncMessages)
                    await _logger.InformationAsync(log.ToString());

                return import;
            });
        }

        #endregion

        #region Inventory

        /// <summary>
        /// Change inventory balance
        /// </summary>
        /// <param name="tvChannelId">TvChannel identifier</param>
        /// <param name="combinationId">Combination identifier</param>
        /// <param name="quantityAdjustment">Stock quantity adjustment</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        public async Task ChangeInventoryBalanceAsync(int tvChannelId, int combinationId, int quantityAdjustment)
        {
            var records = (await _zettleRecordService.GetAllRecordsAsync(active: true))
                .Where(record => record.TvChannelId == tvChannelId && record.InventoryTrackingEnabled && !string.IsNullOrEmpty(record.Uuid))
                .ToList();
            if (!records.Any())
                return;

            var tvChannelRecord = records.FirstOrDefault(record => record.CombinationId == 0);
            var combinationRecords = combinationId > 0
                ? records.Where(record => record.CombinationId == combinationId && record.InventoryTrackingEnabled && !string.IsNullOrEmpty(record.VariantUuid)).ToList()
                : new List<ZettleRecord>();

            //we cannot know the exact reason of the change, so we will use Purchase for negative adjustments and Re-stock for positive ones
            var changeType = quantityAdjustment < 0 ? InventoryBalanceChangeType.Purchase : InventoryBalanceChangeType.Restock;
            var combinationRecordsToUpdate = new List<(ZettleRecord Record, int StockQuantity, int? QuantityAdjustment)>();
            foreach (var combinationRecord in combinationRecords)
            {
                combinationRecordsToUpdate.Add((combinationRecord, 0, Math.Abs(quantityAdjustment)));
            }
            (ZettleRecord Record, int StockQuantity, int? QuantityAdjustment) tvChannelRecordToUpdate = (tvChannelRecord, 0, Math.Abs(quantityAdjustment));
            var tvChannelChange = await PrepareInventoryBalanceChangeAsync(changeType, tvChannelRecordToUpdate, combinationRecordsToUpdate);
            if (tvChannelChange is null)
                return;

            await UpdateInventoryBalanceAsync(changeType, new List<CreateTrackingRequest.TvChannelBalanceChange> { tvChannelChange });
        }

        #endregion

        #endregion
    }
}