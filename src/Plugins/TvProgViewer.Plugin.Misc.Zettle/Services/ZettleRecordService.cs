using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TvProgViewer.Core;
using TvProgViewer.Core.Domain.Catalog;
using TvProgViewer.Data;
using TvProgViewer.Plugin.Misc.Zettle.Domain;
using TvProgViewer.Services.Catalog;

namespace TvProgViewer.Plugin.Misc.Zettle.Services
{
    /// <summary>
    /// Represents the service to manage synchronization records
    /// </summary>
    public class ZettleRecordService
    {
        #region Fields

        private readonly IRepository<Category> _categoryRepository;
        private readonly IRepository<TvChannelAttributeCombination> _tvChannelAttributeCombinationRepository;
        private readonly IRepository<TvChannelCategory> _tvChannelCategoryRepository;
        private readonly IRepository<TvChannel> _tvChannelRepository;
        private readonly IRepository<ZettleRecord> _repository;
        private readonly ZettleSettings _zettleSettings;

        #endregion

        #region Ctor

        public ZettleRecordService(IRepository<Category> categoryRepository,
            IRepository<TvChannelAttributeCombination> tvChannelAttributeCombinationRepository,
            IRepository<TvChannelCategory> tvChannelCategoryRepository,
            IRepository<TvChannel> tvChannelRepository,
            IRepository<ZettleRecord> repository,
            ZettleSettings zettleSettings)
        {
            _categoryRepository = categoryRepository;
            _tvChannelAttributeCombinationRepository = tvChannelAttributeCombinationRepository;
            _tvChannelCategoryRepository = tvChannelCategoryRepository;
            _tvChannelRepository = tvChannelRepository;
            _repository = repository;
            _zettleSettings = zettleSettings;
        }

        #endregion

        #region Utilities

        /// <summary>
        /// Prepare records to add
        /// </summary>
        /// <param name="tvChannelIds">TvChannel identifiers</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the prepared records; the number of tvChannels that were not added
        /// </returns>
        private async Task<(List<ZettleRecord> Records, int InvalidTvChannels)> PrepareRecordsToAddAsync(List<int> tvChannelIds)
        {
            var tvChannels = await _tvChannelRepository.GetByIdsAsync(tvChannelIds, null, false);
            var tvChannelsWithSku = tvChannels.Where(tvChannel => !string.IsNullOrEmpty(tvChannel.Sku)).ToList();
            var invalidTvChannels = tvChannels.Where(tvChannel => string.IsNullOrEmpty(tvChannel.Sku)).Count();
            var records = await tvChannelsWithSku.SelectManyAwait(async tvChannel =>
            {
                var uuid = GuidGenerator.GenerateTimeBasedGuid().ToString();
                var tvChannelRecord = new ZettleRecord
                {
                    Active = _zettleSettings.SyncEnabled,
                    TvChannelId = tvChannel.Id,
                    Uuid = uuid,
                    VariantUuid = GuidGenerator.GenerateTimeBasedGuid().ToString(),
                    PriceSyncEnabled = _zettleSettings.PriceSyncEnabled,
                    ImageSyncEnabled = _zettleSettings.ImageSyncEnabled,
                    InventoryTrackingEnabled = _zettleSettings.InventoryTrackingEnabled,
                    OperationType = OperationType.Create
                };

                var combinations = await _tvChannelAttributeCombinationRepository
                    .GetAllAsync(query => query.Where(combination => combination.TvChannelId == tvChannel.Id && !string.IsNullOrEmpty(combination.Sku)), null);
                var combinationsRecords = combinations.Select(combination => new ZettleRecord
                {
                    Active = _zettleSettings.SyncEnabled,
                    TvChannelId = tvChannel.Id,
                    CombinationId = combination.Id,
                    Uuid = uuid,
                    VariantUuid = GuidGenerator.GenerateTimeBasedGuid().ToString(),
                    PriceSyncEnabled = _zettleSettings.PriceSyncEnabled,
                    ImageSyncEnabled = _zettleSettings.ImageSyncEnabled,
                    InventoryTrackingEnabled = _zettleSettings.InventoryTrackingEnabled,
                    OperationType = OperationType.Create
                }).ToList();

                return new List<ZettleRecord> { tvChannelRecord }.Union(combinationsRecords);
            }).ToListAsync();

            return (records, invalidTvChannels);
        }

        #endregion

        #region Methods

        /// <summary>
        /// Get a record by the identifier
        /// </summary>
        /// <param name="id">Record identifier</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the record for synchronization
        /// </returns>
        public async Task<ZettleRecord> GetRecordByIdAsync(int id)
        {
            return await _repository.GetByIdAsync(id, null);
        }

        /// <summary>
        /// Insert the record
        /// </summary>
        /// <param name="record">Record</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        public async Task InsertRecordAsync(ZettleRecord record)
        {
            await _repository.InsertAsync(record, false);
        }

        /// <summary>
        /// Insert records
        /// </summary>
        /// <param name="records">Records</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        public async Task InsertRecordsAsync(List<ZettleRecord> records)
        {
            await _repository.InsertAsync(records, false);
        }

        /// <summary>
        /// Update the record
        /// </summary>
        /// <param name="record">Record</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        public async Task UpdateRecordAsync(ZettleRecord record)
        {
            await _repository.UpdateAsync(record, false);
        }

        /// <summary>
        /// Update records
        /// </summary>
        /// <param name="records">Records</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        public async Task UpdateRecordsAsync(List<ZettleRecord> records)
        {
            await _repository.UpdateAsync(records, false);
        }

        /// <summary>
        /// Delete the record
        /// </summary>
        /// <param name="record">Record</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        public async Task DeleteRecordAsync(ZettleRecord record)
        {
            await _repository.DeleteAsync(record, false);
        }

        /// <summary>
        /// Delete records
        /// </summary>
        /// <param name="ids">Records identifiers</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        public async Task DeleteRecordsAsync(List<int> ids)
        {
            await _repository.DeleteAsync(record => ids.Contains(record.Id));
        }

        /// <summary>
        /// Clear all records
        /// </summary>
        /// <returns>A task that represents the asynchronous operation</returns>
        public async Task ClearRecordsAsync()
        {
            if (_zettleSettings.ClearRecordsOnChangeCredentials)
                await _repository.TruncateAsync();
            else
            {
                var records = (await GetAllRecordsAsync()).ToList();
                foreach (var record in records)
                {
                    record.ImageUrl = string.Empty;
                    record.UpdatedOnUtc = null;
                    record.OperationType = OperationType.Create;
                }
                await UpdateRecordsAsync(records);
            }
        }

        /// <summary>
        /// Get all records for synchronization
        /// </summary>
        /// <param name="tvChannelOnly">Whether to load only tvChannel records</param>
        /// <param name="active">Whether to load only active records; true - active only, false - inactive only, null - all records</param>
        /// <param name="operationTypes">Operation types; pass null to load all records</param>
        /// <param name="tvChannelUuid">TvChannel unique identifier; pass null to load all records</param>
        /// <param name="pageIndex">Page index</param>
        /// <param name="pageSize">Page size</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the records for synchronization
        /// </returns>
        public async Task<IPagedList<ZettleRecord>> GetAllRecordsAsync(bool tvChannelOnly = false,
            bool? active = null, List<OperationType> operationTypes = null, string tvChannelUuid = null,
            int pageIndex = 0, int pageSize = int.MaxValue)
        {
            return await _repository.GetAllPagedAsync(query =>
            {
                if (tvChannelOnly)
                    query = query.Where(record => record.TvChannelId > 0 && record.CombinationId == 0);

                if (active.HasValue)
                    query = query.Where(record => record.Active == active.Value);

                if (operationTypes?.Any() ?? false)
                    query = query.Where(record => operationTypes.Contains((OperationType)record.OperationTypeId));

                if (!string.IsNullOrEmpty(tvChannelUuid))
                    query = query.Where(record => record.Uuid == tvChannelUuid);

                query = query.OrderBy(record => record.Id);

                return query;
            }, pageIndex, pageSize);
        }

        /// <summary>
        /// Create or update a record for synchronization
        /// </summary>
        /// <param name="operationType">Operation type</param>
        /// <param name="tvChannelId">TvChannel identifier</param>
        /// <param name="attributeCombinationId">TvChannel attribute combination identifier</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        public async Task CreateOrUpdateRecordAsync(OperationType operationType, int tvChannelId, int attributeCombinationId = 0)
        {
            if (tvChannelId == 0 && attributeCombinationId == 0)
                return;

            var existingRecord = _repository.Table.
                FirstOrDefault(record => record.TvChannelId == tvChannelId && record.CombinationId == attributeCombinationId);

            if (existingRecord is null)
            {
                if (operationType != OperationType.Create)
                    return;

                if (!_zettleSettings.AutoAddRecordsEnabled)
                    return;

                if (attributeCombinationId == 0)
                {
                    var (records, _) = await PrepareRecordsToAddAsync(new List<int> { tvChannelId });
                    await InsertRecordsAsync(records);
                }
                else
                {
                    var tvChannelRecord = _repository.Table.FirstOrDefault(record => record.TvChannelId == tvChannelId);
                    await InsertRecordAsync(new()
                    {
                        Active = _zettleSettings.SyncEnabled,
                        TvChannelId = tvChannelId,
                        CombinationId = attributeCombinationId,
                        Uuid = tvChannelRecord.Uuid,
                        VariantUuid = GuidGenerator.GenerateTimeBasedGuid().ToString(),
                        PriceSyncEnabled = _zettleSettings.PriceSyncEnabled,
                        ImageSyncEnabled = _zettleSettings.ImageSyncEnabled,
                        InventoryTrackingEnabled = _zettleSettings.InventoryTrackingEnabled,
                        OperationType = operationType
                    });
                }

                return;
            }

            switch (existingRecord.OperationType)
            {
                case OperationType.Create:
                    if (operationType == OperationType.Delete)
                        await DeleteRecordAsync(existingRecord);
                    return;

                case OperationType.Update:
                    if (operationType == OperationType.Delete)
                    {
                        existingRecord.OperationType = OperationType.Delete;
                        await UpdateRecordAsync(existingRecord);
                    }
                    return;

                case OperationType.Delete:
                    if (operationType == OperationType.Create)
                    {
                        existingRecord.OperationType = OperationType.Update;
                        await UpdateRecordAsync(existingRecord);
                    }
                    return;

                case OperationType.ImageChanged:
                case OperationType.None:
                    existingRecord.OperationType = operationType;
                    await UpdateRecordAsync(existingRecord);
                    return;
            }
        }

        /// <summary>
        /// Add records for synchronization
        /// </summary>
        /// <param name="tvChannelIds">TvChannel identifiers</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the number of tvChannels that were not added
        /// </returns>
        public async Task<int?> AddRecordsAsync(List<int> tvChannelIds)
        {
            if (!tvChannelIds?.Any() ?? true)
                return null;

            var newTvChannelIds = tvChannelIds.Except(_repository.Table.Select(record => record.TvChannelId)).ToList();
            if (!newTvChannelIds.Any())
                return null;

            var (records, invalidTvChannels) = await PrepareRecordsToAddAsync(newTvChannelIds);
            await InsertRecordsAsync(records);

            return invalidTvChannels;
        }

        /// <summary>
        /// Prepare records for synchronization
        /// </summary>
        /// <param name="records">Records</param>
        /// <returns>Records ready for synchronization</returns>
        public List<TvChannelToSync> PrepareToSyncRecords(List<ZettleRecord> records)
        {
            var recordIds = records.Select(record => record.Id).ToList();
            var tvChannelToSync = _repository.Table
                .Where(record => recordIds.Contains(record.Id))
                .Join(_tvChannelCategoryRepository.Table,
                    record => record.TvChannelId,
                    pc => pc.TvChannelId,
                    (record, pc) => new { Record = record, TvChannelCategory = pc })
                .Join(_tvChannelRepository.Table,
                    item => item.TvChannelCategory.TvChannelId,
                    tvChannel => tvChannel.Id,
                    (item, tvChannel) => new { TvChannel = tvChannel, Record = item.Record, TvChannelCategory = item.TvChannelCategory })
                .Join(_categoryRepository.Table,
                    item => item.TvChannelCategory.CategoryId,
                    category => category.Id,
                    (item, category) => new { Category = category, TvChannel = item.TvChannel, Record = item.Record, TvChannelCategory = item.TvChannelCategory })
                .Select(item => new
                {
                    Id = item.TvChannel.Id,
                    Uuid = item.Record.Uuid,
                    VariantUuid = item.Record.VariantUuid,
                    Name = item.TvChannel.Name,
                    Sku = item.TvChannel.Sku,
                    Description = item.TvChannel.ShortDescription,
                    Price = item.TvChannel.Price,
                    CategoryName = item.Category.Name,
                    ImageUrl = item.Record.ImageUrl,
                    ImageSyncEnabled = item.Record.ImageSyncEnabled,
                    PriceSyncEnabled = item.Record.PriceSyncEnabled,
                    TvChannelCategoryId = item.TvChannelCategory.Id,
                    TvChannelCategoryDisplayOrder = item.TvChannelCategory.DisplayOrder
                })
                .GroupBy(item => item.Id)
                .Select(group => new TvChannelToSync
                {
                    Id = group.Key,
                    Uuid = group.FirstOrDefault().Uuid,
                    VariantUuid = group.FirstOrDefault().VariantUuid,
                    Name = group.FirstOrDefault().Name,
                    Sku = group.FirstOrDefault().Sku,
                    Description = group.FirstOrDefault().Description,
                    Price = group.FirstOrDefault().Price,
                    CategoryName = group
                        .OrderBy(item => item.TvChannelCategoryDisplayOrder)
                        .ThenBy(item => item.TvChannelCategoryId)
                        .Select(item => item.CategoryName)
                        .FirstOrDefault(),
                    ImageUrl = group.FirstOrDefault().ImageUrl,
                    ImageSyncEnabled = group.FirstOrDefault().ImageSyncEnabled,
                    PriceSyncEnabled = group.FirstOrDefault().PriceSyncEnabled
                })
                .ToList();

            return tvChannelToSync;
        }

        #endregion
    }
}