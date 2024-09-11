using System.Linq;
using System.Threading.Tasks;
using TvProgViewer.Core.Domain.Catalog;
using TvProgViewer.Core.Events;
using TvProgViewer.Plugin.Misc.Zettle.Domain;
using TvProgViewer.Services.Catalog;
using TvProgViewer.Services.Events;

namespace TvProgViewer.Plugin.Misc.Zettle.Services
{
    /// <summary>
    /// Represents plugin event consumer
    /// </summary>
    public class EventConsumer :
        IConsumer<EntityInsertedEvent<TvChannel>>,
        IConsumer<EntityUpdatedEvent<TvChannel>>,
        IConsumer<EntityDeletedEvent<TvChannel>>,
        IConsumer<EntityInsertedEvent<TvChannelCategory>>,
        IConsumer<EntityDeletedEvent<TvChannelCategory>>,
        IConsumer<EntityUpdatedEvent<Category>>,
        IConsumer<EntityDeletedEvent<Category>>,
        IConsumer<EntityInsertedEvent<TvChannelPicture>>,
        IConsumer<EntityUpdatedEvent<TvChannelPicture>>,
        IConsumer<EntityDeletedEvent<TvChannelPicture>>,
        IConsumer<EntityUpdatedEvent<TvChannelAttribute>>,
        IConsumer<EntityDeletedEvent<TvChannelAttribute>>,
        IConsumer<EntityUpdatedEvent<TvChannelAttributeValue>>,
        IConsumer<EntityDeletedEvent<TvChannelAttributeValue>>,
        IConsumer<EntityInsertedEvent<TvChannelAttributeCombination>>,
        IConsumer<EntityUpdatedEvent<TvChannelAttributeCombination>>,
        IConsumer<EntityDeletedEvent<TvChannelAttributeCombination>>,
        IConsumer<EntityInsertedEvent<StockQuantityHistory>>

    {
        #region Fields

        private readonly ICategoryService _categoryService;
        private readonly ITvChannelAttributeParser _tvChannelAttributeParser;
        private readonly ITvChannelAttributeService _tvChannelAttributeService;
        private readonly ITvChannelService _tvChannelService;
        private readonly ZettleRecordService _zettleRecordService;
        private readonly ZettleService _zettleService;
        private readonly ZettleSettings _zettleSettings;

        #endregion

        #region Ctor

        public EventConsumer(ICategoryService categoryService,
            ITvChannelAttributeParser tvChannelAttributeParser,
            ITvChannelAttributeService tvChannelAttributeService,
            ITvChannelService tvChannelService,
            ZettleRecordService zettleRecordService,
            ZettleService zettleService,
            ZettleSettings zettleSettings)
        {
            _categoryService = categoryService;
            _tvChannelAttributeParser = tvChannelAttributeParser;
            _tvChannelAttributeService = tvChannelAttributeService;
            _tvChannelService = tvChannelService;
            _zettleRecordService = zettleRecordService;
            _zettleService = zettleService;
            _zettleSettings = zettleSettings;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Handle entity created event
        /// </summary>
        /// <param name="eventMessage">Event message</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        public async Task HandleEventAsync(EntityInsertedEvent<TvChannel> eventMessage)
        {
            if (eventMessage.Entity is null)
                return;

            if (!ZettleService.IsConfigured(_zettleSettings))
                return;

            await _zettleRecordService.CreateOrUpdateRecordAsync(OperationType.Create, eventMessage.Entity.Id);
        }

        /// <summary>
        /// Handle entity updated event
        /// </summary>
        /// <param name="eventMessage">Event message</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        public async Task HandleEventAsync(EntityUpdatedEvent<TvChannel> eventMessage)
        {
            if (eventMessage.Entity is null)
                return;

            if (!ZettleService.IsConfigured(_zettleSettings))
                return;

            if (!eventMessage.Entity.Deleted)
                await _zettleRecordService.CreateOrUpdateRecordAsync(OperationType.Update, eventMessage.Entity.Id);
            else
            {
                await _zettleRecordService.CreateOrUpdateRecordAsync(OperationType.Delete, eventMessage.Entity.Id);
                var combinations = await _tvChannelAttributeService.GetAllTvChannelAttributeCombinationsAsync(eventMessage.Entity.Id);
                foreach (var combination in combinations)
                {
                    await _zettleRecordService.CreateOrUpdateRecordAsync(OperationType.Delete, combination.TvChannelId, combination.Id);
                }
            }
        }

        /// <summary>
        /// Handle entity deleted event
        /// </summary>
        /// <param name="eventMessage">Event message</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        public async Task HandleEventAsync(EntityDeletedEvent<TvChannel> eventMessage)
        {
            if (eventMessage.Entity is null)
                return;

            if (!ZettleService.IsConfigured(_zettleSettings))
                return;

            await _zettleRecordService.CreateOrUpdateRecordAsync(OperationType.Delete, eventMessage.Entity.Id);
            var combinations = await _tvChannelAttributeService.GetAllTvChannelAttributeCombinationsAsync(eventMessage.Entity.Id);
            foreach (var combination in combinations)
            {
                await _zettleRecordService.CreateOrUpdateRecordAsync(OperationType.Delete, combination.TvChannelId, combination.Id);
            }
        }

        /// <summary>
        /// Handle entity created event
        /// </summary>
        /// <param name="eventMessage">Event message</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        public async Task HandleEventAsync(EntityInsertedEvent<TvChannelCategory> eventMessage)
        {
            if (eventMessage.Entity is null)
                return;

            if (!ZettleService.IsConfigured(_zettleSettings))
                return;

            if (!_zettleSettings.CategorySyncEnabled)
                return;

            await _zettleRecordService.CreateOrUpdateRecordAsync(OperationType.Update, eventMessage.Entity.TvChannelId);
        }

        /// <summary>
        /// Handle entity deleted event
        /// </summary>
        /// <param name="eventMessage">Event message</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        public async Task HandleEventAsync(EntityDeletedEvent<TvChannelCategory> eventMessage)
        {
            if (eventMessage.Entity is null)
                return;

            if (!ZettleService.IsConfigured(_zettleSettings))
                return;

            if (!_zettleSettings.CategorySyncEnabled)
                return;

            await _zettleRecordService.CreateOrUpdateRecordAsync(OperationType.Update, eventMessage.Entity.TvChannelId);
        }

        /// <summary>
        /// Handle entity updated event
        /// </summary>
        /// <param name="eventMessage">Event message</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        public async Task HandleEventAsync(EntityUpdatedEvent<Category> eventMessage)
        {
            if (eventMessage.Entity is null)
                return;

            if (!ZettleService.IsConfigured(_zettleSettings))
                return;

            if (!_zettleSettings.CategorySyncEnabled)
                return;

            var mappings = await _categoryService.GetTvChannelCategoriesByCategoryIdAsync(eventMessage.Entity.Id, showHidden: true);
            foreach (var mapping in mappings)
            {
                await _zettleRecordService.CreateOrUpdateRecordAsync(OperationType.Update, mapping.TvChannelId);
            }
        }

        /// <summary>
        /// Handle entity deleted event
        /// </summary>
        /// <param name="eventMessage">Event message</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        public async Task HandleEventAsync(EntityDeletedEvent<Category> eventMessage)
        {
            if (eventMessage.Entity is null)
                return;

            if (!ZettleService.IsConfigured(_zettleSettings))
                return;

            if (!_zettleSettings.CategorySyncEnabled)
                return;

            var mappings = await _categoryService.GetTvChannelCategoriesByCategoryIdAsync(eventMessage.Entity.Id, showHidden: true);
            foreach (var mapping in mappings)
            {
                await _zettleRecordService.CreateOrUpdateRecordAsync(OperationType.Update, mapping.TvChannelId);
            }
        }

        /// <summary>
        /// Handle entity created event
        /// </summary>
        /// <param name="eventMessage">Event message</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        public async Task HandleEventAsync(EntityInsertedEvent<TvChannelPicture> eventMessage)
        {
            if (eventMessage.Entity is null)
                return;

            if (!ZettleService.IsConfigured(_zettleSettings))
                return;

            var pictures = await _tvChannelService.GetTvChannelPicturesByTvChannelIdAsync(eventMessage.Entity.TvChannelId);
            if (eventMessage.Entity.DisplayOrder <= pictures.Min(picture => picture.DisplayOrder))
                await _zettleRecordService.CreateOrUpdateRecordAsync(OperationType.ImageChanged, eventMessage.Entity.TvChannelId);
        }

        /// <summary>
        /// Handle entity updated event
        /// </summary>
        /// <param name="eventMessage">Event message</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        public async Task HandleEventAsync(EntityUpdatedEvent<TvChannelPicture> eventMessage)
        {
            if (eventMessage.Entity is null)
                return;

            if (!ZettleService.IsConfigured(_zettleSettings))
                return;

            var pictures = await _tvChannelService.GetTvChannelPicturesByTvChannelIdAsync(eventMessage.Entity.TvChannelId);
            if (eventMessage.Entity.DisplayOrder <= pictures.Min(picture => picture.DisplayOrder))
                await _zettleRecordService.CreateOrUpdateRecordAsync(OperationType.ImageChanged, eventMessage.Entity.TvChannelId);
        }

        /// <summary>
        /// Handle entity deleted event
        /// </summary>
        /// <param name="eventMessage">Event message</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        public async Task HandleEventAsync(EntityDeletedEvent<TvChannelPicture> eventMessage)
        {
            if (eventMessage.Entity is null)
                return;

            if (!ZettleService.IsConfigured(_zettleSettings))
                return;

            await _zettleRecordService.CreateOrUpdateRecordAsync(OperationType.ImageChanged, eventMessage.Entity.TvChannelId);
        }

        /// <summary>
        /// Handle entity updated event
        /// </summary>
        /// <param name="eventMessage">Event message</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        public async Task HandleEventAsync(EntityUpdatedEvent<TvChannelAttribute> eventMessage)
        {
            if (eventMessage.Entity is null)
                return;

            if (!ZettleService.IsConfigured(_zettleSettings))
                return;

            var tvChannels = await _tvChannelService.GetTvChannelsByTvChannelAttributeIdAsync(eventMessage.Entity.Id);
            foreach (var tvChannel in tvChannels)
            {
                await _zettleRecordService.CreateOrUpdateRecordAsync(OperationType.Update, tvChannel.Id);
                var combinations = await _tvChannelAttributeService.GetAllTvChannelAttributeCombinationsAsync(tvChannel.Id);
                foreach (var combination in combinations)
                {
                    await _zettleRecordService.CreateOrUpdateRecordAsync(OperationType.Update, combination.TvChannelId, combination.Id);
                }
            }
        }

        /// <summary>
        /// Handle entity deleted event
        /// </summary>
        /// <param name="eventMessage">Event message</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        public async Task HandleEventAsync(EntityDeletedEvent<TvChannelAttribute> eventMessage)
        {
            if (eventMessage.Entity is null)
                return;

            if (!ZettleService.IsConfigured(_zettleSettings))
                return;

            var tvChannels = await _tvChannelService.GetTvChannelsByTvChannelAttributeIdAsync(eventMessage.Entity.Id);
            foreach (var tvChannel in tvChannels)
            {
                await _zettleRecordService.CreateOrUpdateRecordAsync(OperationType.Update, tvChannel.Id);
                var combinations = await _tvChannelAttributeService.GetAllTvChannelAttributeCombinationsAsync(tvChannel.Id);
                foreach (var combination in combinations)
                {
                    var mappings = await _tvChannelAttributeParser.ParseTvChannelAttributeMappingsAsync(combination.AttributesXml);
                    if (mappings.Any(mapping => mapping.TvChannelAttributeId == eventMessage.Entity.Id))
                        await _zettleRecordService.CreateOrUpdateRecordAsync(OperationType.Delete, combination.TvChannelId, combination.Id);
                    else
                        await _zettleRecordService.CreateOrUpdateRecordAsync(OperationType.Update, combination.TvChannelId, combination.Id);
                }
            }
        }

        /// <summary>
        /// Handle entity updated event
        /// </summary>
        /// <param name="eventMessage">Event message</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        public async Task HandleEventAsync(EntityUpdatedEvent<TvChannelAttributeValue> eventMessage)
        {
            if (eventMessage.Entity is null)
                return;

            if (!ZettleService.IsConfigured(_zettleSettings))
                return;

            var mapping = await _tvChannelAttributeService.GetTvChannelAttributeMappingByIdAsync(eventMessage.Entity.TvChannelAttributeMappingId);
            if (mapping is null)
                return;

            await _zettleRecordService.CreateOrUpdateRecordAsync(OperationType.Update, mapping.TvChannelId);
            var combinations = await _tvChannelAttributeService.GetAllTvChannelAttributeCombinationsAsync(mapping.TvChannelId);
            foreach (var combination in combinations)
            {
                await _zettleRecordService.CreateOrUpdateRecordAsync(OperationType.Update, combination.TvChannelId, combination.Id);
            }
        }

        /// <summary>
        /// Handle entity deleted event
        /// </summary>
        /// <param name="eventMessage">Event message</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        public async Task HandleEventAsync(EntityDeletedEvent<TvChannelAttributeValue> eventMessage)
        {
            if (eventMessage.Entity is null)
                return;

            if (!ZettleService.IsConfigured(_zettleSettings))
                return;

            var mapping = await _tvChannelAttributeService.GetTvChannelAttributeMappingByIdAsync(eventMessage.Entity.TvChannelAttributeMappingId);
            if (mapping is null)
                return;

            await _zettleRecordService.CreateOrUpdateRecordAsync(OperationType.Update, mapping.TvChannelId);
            var combinations = await _tvChannelAttributeService.GetAllTvChannelAttributeCombinationsAsync(mapping.TvChannelId);
            foreach (var combination in combinations)
            {
                await _zettleRecordService.CreateOrUpdateRecordAsync(OperationType.Update, combination.TvChannelId, combination.Id);
            }
        }

        /// <summary>
        /// Handle entity created event
        /// </summary>
        /// <param name="eventMessage">Event message</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        public async Task HandleEventAsync(EntityInsertedEvent<TvChannelAttributeCombination> eventMessage)
        {
            if (eventMessage.Entity is null)
                return;

            if (!ZettleService.IsConfigured(_zettleSettings))
                return;

            await _zettleRecordService.CreateOrUpdateRecordAsync(OperationType.Update, eventMessage.Entity.TvChannelId);
            await _zettleRecordService.CreateOrUpdateRecordAsync(OperationType.Create, eventMessage.Entity.TvChannelId, eventMessage.Entity.Id);
            var combinations = await _tvChannelAttributeService.GetAllTvChannelAttributeCombinationsAsync(eventMessage.Entity.TvChannelId);
            foreach (var combination in combinations)
            {
                await _zettleRecordService.CreateOrUpdateRecordAsync(OperationType.Update, combination.TvChannelId, combination.Id);
            }
        }

        /// <summary>
        /// Handle entity updated event
        /// </summary>
        /// <param name="eventMessage">Event message</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        public async Task HandleEventAsync(EntityUpdatedEvent<TvChannelAttributeCombination> eventMessage)
        {
            if (eventMessage.Entity is null)
                return;

            if (!ZettleService.IsConfigured(_zettleSettings))
                return;

            await _zettleRecordService.CreateOrUpdateRecordAsync(OperationType.Update, eventMessage.Entity.TvChannelId);
            var combinations = await _tvChannelAttributeService.GetAllTvChannelAttributeCombinationsAsync(eventMessage.Entity.TvChannelId);
            foreach (var combination in combinations)
            {
                await _zettleRecordService.CreateOrUpdateRecordAsync(OperationType.Update, combination.TvChannelId, combination.Id);
            }
        }

        /// <summary>
        /// Handle entity deleted event
        /// </summary>
        /// <param name="eventMessage">Event message</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        public async Task HandleEventAsync(EntityDeletedEvent<TvChannelAttributeCombination> eventMessage)
        {
            if (eventMessage.Entity is null)
                return;

            if (!ZettleService.IsConfigured(_zettleSettings))
                return;

            await _zettleRecordService.CreateOrUpdateRecordAsync(OperationType.Update, eventMessage.Entity.TvChannelId);
            await _zettleRecordService.CreateOrUpdateRecordAsync(OperationType.Delete, eventMessage.Entity.TvChannelId, eventMessage.Entity.Id);
            var combinations = await _tvChannelAttributeService.GetAllTvChannelAttributeCombinationsAsync(eventMessage.Entity.TvChannelId);
            foreach (var combination in combinations)
            {
                await _zettleRecordService.CreateOrUpdateRecordAsync(OperationType.Update, combination.TvChannelId, combination.Id);
            }
        }

        /// <summary>
        /// Handle entity created event
        /// </summary>
        /// <param name="eventMessage">Event message</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        public async Task HandleEventAsync(EntityInsertedEvent<StockQuantityHistory> eventMessage)
        {
            if (eventMessage.Entity is null)
                return;

            if (!ZettleService.IsConfigured(_zettleSettings))
                return;

            if (eventMessage.Entity.QuantityAdjustment == 0)
                return;

            if (eventMessage.Entity.Message.StartsWith(ZettleDefaults.SystemName))
                return;

            await _zettleService.ChangeInventoryBalanceAsync(eventMessage.Entity.TvChannelId,
                eventMessage.Entity.CombinationId ?? 0, eventMessage.Entity.QuantityAdjustment);
        }

        #endregion
    }
}