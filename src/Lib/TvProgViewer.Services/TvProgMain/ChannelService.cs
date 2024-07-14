using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TvProgViewer.Data.TvProgMain.ProgObjs;
using TvProgViewer.Data;
using TvProgViewer.Core.Domain.TvProgMain;
using LinqToDB;
using TvProgViewer.Data.TvProgMain;
using System.Threading;
using Newtonsoft.Json;
using TvProgViewer.Core.Domain.Users;
using TvProgViewer.Services.Seo;
using TvProgViewer.Services.Catalog;
using TvProgViewer.Core.Domain.Catalog;

namespace TvProgViewer.Services.TvProgMain
{
    /// <summary>
    /// Обеспечивает выборку телеканалов
    /// </summary>
    public class ChannelService: IChannelService
    {
        private const string SYS_ORDER_COL = "SysOrderCol";
        #region Поля

        private readonly IRepository<Channels> _channelsRepository;
        private readonly IRepository<MediaPic> _mediaPicRepository;
        private readonly IRepository<Programmes> _programmesRepository;
        private readonly IRepository<UserChannelMapping> _userChannelMappingRepository;
        private readonly IUrlRecordService _urlRecordService;
        private readonly ITvChannelService _tvChannelService;
        private readonly List<string> _firstMultiplex =
        [
            "Первый канал", "Россия 1", "Матч!", "НТВ", "Пятый Канал", "Культура", "Россия 24", "Карусель", "ОТР", "ТВ Центр",
            "РЕН ТВ", "Спас ТВ", "СТС", "Домашний", "ТВ-3", "Пятница", "Звезда", "МИР", "ТНТ", "МУЗ-ТВ" 
        ]; 
        #endregion

        #region Конструктор

        public ChannelService(IRepository<Channels> channelsRepositry,
                              IRepository<MediaPic> mediaPicRepository,
                              IRepository<Programmes> programmesRepository,
                              IRepository<UserChannelMapping> userChannelMappingRepository,
                              IUrlRecordService urlRecordService,
                              ITvChannelService tvChannelService)
        {
            _channelsRepository = channelsRepositry;
            _mediaPicRepository = mediaPicRepository;
            _programmesRepository = programmesRepository;
            _userChannelMappingRepository = userChannelMappingRepository;   
            _urlRecordService = urlRecordService;
            _tvChannelService = tvChannelService;
        }

        /// <summary>
        /// Получение идентификатора телеканала по внутреннему идентификатору
        /// </summary>
        /// <param name="internalId">Внутренний идентификатор</param>
        /// <returns>Идентификатор телеканала</returns>
        public async Task<int?> GetChannelIdByInternalIdAsync(int internalId)
        {
            var channel = await _channelsRepository.Table.FirstOrDefaultAsync(q => q.InternalId == internalId && q.Deleted == null);
            return channel?.Id;
        }

        #endregion

        #region Методы
        /// <summary>
        /// Получение системных телеканалов
        /// </summary>
        /// <param name="tvProgProviderId">Идентификатор провайдера телеканалов</param>
        public virtual async Task<KeyValuePair<int, List<SystemChannel>>> GetSystemChannelsAsync(int tvProgProviderId, string filtData, string sidx, string sord, int page, int rows)
        {
            var scCount = await (from ch in _channelsRepository.Table
                            join mp in _mediaPicRepository.Table on ch.IconId equals mp.Id into chmp
                            from mp in chmp.DefaultIfEmpty()
                            where ch.TvProgProviderId == tvProgProviderId && ch.Deleted == null &&
                                        (filtData == null || ch.TitleChannel.Contains(filtData)) &&
                                        (from pr in _programmesRepository.Table select pr.ChannelId).Contains(ch.Id)
                            select ch.Id).CountAsync(CancellationToken.None);
            sidx = !string.IsNullOrWhiteSpace(sidx) ? sidx.Replace("SystemTitle", "Title") : SYS_ORDER_COL;         // - поле для сортировки
            var scRows = await (from ch in _channelsRepository.Table
                            join mp in _mediaPicRepository.Table on ch.IconId equals mp.Id into chmp
                            from mp in chmp.DefaultIfEmpty()
                            where ch.TvProgProviderId == tvProgProviderId && ch.Deleted == null &&
                                        (filtData == null || ch.TitleChannel.Contains(filtData))
                            select new SystemChannel
                            {
                                ChannelId = ch.Id,
                                TvProgViewerId = ch.TvProgProviderId,
                                InternalId = ch.InternalId,
                                IconId = ch.IconId,
                                FileNameOrig = mp.PathOrig + mp.FileName,
                                FileName25 = mp.Path25 + mp.FileName,
                                Title = ch.TitleChannel,
                                ImageWebSrc = ch.IconWebSrc,
                                SysOrderCol = ch.SysOrderCol
                            }).Where(ch => (from pr in _programmesRepository.Table select pr.ChannelId).Contains(ch.ChannelId))
                              .LimitAndOrderBy(page, rows, sidx, sord).ToListAsync();
        
            SetUrls(scRows);
            
            return new KeyValuePair<int, List<SystemChannel>>(scCount, scRows);
        }

        /// <summary>
        /// Установить ссылки на странички с детализацией
        /// </summary>
        /// <param name="systemChannel">Системные каналы</param>
        private void SetUrls(List<SystemChannel> systemChannel)
        {
            Parallel.ForEach(systemChannel, sp =>
            {
                sp.UrlDetails = Task.Run(async() => await GetSeNameByInternalIdAsync(sp.InternalId)).Result;
            });
        }

        /// <summary>
        /// Получение SEO наименования
        /// </summary>
        /// <param name="internalId">Внутренний идентификатор телеканала</param>
        /// <returns></returns>
        public virtual async Task<string> GetSeNameByInternalIdAsync(int? internalId)
        {
           var tvChannel = (await _tvChannelService.GetTvChannelsBySkuAsync([internalId.Value.ToString()])).FirstOrDefault();
           if (tvChannel == null)
                return string.Empty;
           return await _urlRecordService.GetSeNameAsync(tvChannel, 0, true, false);
            
        }

        /// <summary>
        /// Получение пользовательских телеканалов
        /// </summary>
        /// <param name="tvProgProvder">Идентификатор провайдера телеканалов</param>
        /// <param name="jsonChannels">Данные из localStorage</param>
        public virtual async Task<List<UserChannel>> GetUserChannelsByLocalStorageAsync(int tvProgProvder, string jsonChannels)
        {
            List<LocalStorageOptChans> optChans = JsonConvert.DeserializeObject<List<LocalStorageOptChans>>(jsonChannels);
            List<UserChannel> uch = await (from ch in _channelsRepository.Table
                                           join mp in _mediaPicRepository.Table on ch.IconId equals mp.Id into chmp
                                           from mp in chmp.DefaultIfEmpty()
                                           where ch.TvProgProviderId == tvProgProvder && ch.Deleted == null
                                           select new UserChannel
                                           {
                                               ChannelId = ch.Id,
                                               TvProgViewerId = ch.TvProgProviderId,
                                               InternalId = ch.InternalId,
                                               IconId = ch.IconId,
                                               FileNameOrig = mp.PathOrig + mp.FileName,
                                               FileName25 = mp.Path25 + mp.FileName,
                                               Title = ch.TitleChannel,
                                               ImageWebSrc = ch.IconWebSrc,
                                               SysOrderCol = ch.SysOrderCol
                                           }).ToListAsync();
            return await uch.Where(x => optChans.Any(y => y.ChannelId == x.ChannelId))
                                        .OrderBy(o => o.SysOrderCol)
                                        .ThenBy(o => o.InternalId)
                                        .ToListAsync();
        }

        /// <summary>
        /// Перерасчёт пользовательского рейтинга телеканалов
        /// </summary>
        public async Task RecalculateChannelUserRatingAsync()
        {
            // Группировка по идентификаторам телеканалов:
            var grouped = await _userChannelMappingRepository.Table.GroupBy(ucm => ucm.ChannelId)
                                                     .Select(group => new {
                                                         ChannelId = group.Key,
                                                         Qty = group.Count()
                                                     }).ToListAsync();

            // Получение телеканалов, попавших в выборку:
            var channels = await _channelsRepository.GetByIdsAsync(await grouped.Select(gr => gr.ChannelId).ToListAsync());

            // Установка количества повторений соответствующим телеканалам:
            foreach (var (channel, gr) in from channel in channels
                                          from gr in grouped
                                          where channel.Id == gr.ChannelId
                                          select (channel, gr))
            {
                channel.UserRating = gr.Qty;
            }

            // Обновление телеканалов:
            await _channelsRepository.UpdateAsync(channels);
        }

        /// <summary>
        /// Пересортировка телеканалов в соответствии с пользовательским рейтингом
        /// </summary>
        public async Task ReorderChannelAsync()
        {
            // Получение всех неудалённых телеканалов и не входящих в первый мультиплекс,
            // у которых есть пользовательский рейтинг:
            var channels = await _channelsRepository.GetAllAsync(query =>
            {
                query = query.Where(q => q.Deleted == null && q.UserRating != null);
                query = query.Where(q => !_firstMultiplex.Contains(q.TitleChannel));
                return Task.FromResult(query);
            });
            
            // Установка сортировки в соответствии с рейтингом
            int i = 21;
            foreach (var channel in channels.OrderByDescending(o => o.UserRating))
            {
                channel.SysOrderCol = i++;
            }

            // Обновление телеканалов:
            await _channelsRepository.UpdateAsync(channels);

            // Установка рейтинга:
            Dictionary<int, int> idsDict = [];
            foreach (var channel in channels)
            {
                var tvChannel = (await _tvChannelService.GetTvChannelsBySkuAsync([channel.InternalId.Value.ToString()])).FirstOrDefault();
                if (tvChannel != null)
                {
                    idsDict[tvChannel.Id] = channel.SysOrderCol;
                }
            }

            // Добавление в обновляемые:
            List<TvChannel> tvChannelList = [];
            foreach (var tvChannel in (await _tvChannelService.GetAllTvChannelsAsync())
                                                              .Where(q => !_firstMultiplex.Contains(q.Name)))
            {
                tvChannel.DisplayOrder = idsDict.TryGetValue(tvChannel.Id, out int value) ? value : 1000000000;
                tvChannel.UpdatedOnUtc = DateTime.UtcNow;
                tvChannelList.Add(tvChannel);
            }

            // Обновление телеканалов деталей:
            await _tvChannelService.UpdateTvChannelListAsync(tvChannelList);
        }

        /// <summary>
        /// Получение всех действующих каналов
        /// </summary>
        /// <returns></returns>
        public virtual async Task<List<UserChannel>> GetAllChannels()
        {
            List<UserChannel> uch = await (from ch in _channelsRepository.Table
                                           join mp in _mediaPicRepository.Table on ch.IconId equals mp.Id into chmp
                                           from mp in chmp.DefaultIfEmpty()
                                           where ch.TvProgProviderId == 1 && ch.Deleted == null && !string.IsNullOrWhiteSpace(ch.TitleChannel)
                                           select new UserChannel
                                           {
                                               InternalId = ch.InternalId,
                                               Title = ch.TitleChannel,
                                               SysOrderCol = ch.SysOrderCol,
                                               UserRating = ch.UserRating == null ? 0 : ch.UserRating,
                                           }).ToListAsync();
            return uch;
        }
        #endregion
    }
}