using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TvProgViewer.Data.TvProgMain.ProgObjs;
using TvProgViewer.Data;
using TvProgViewer.Core.Domain.TvProgMain;
using TvProgViewer.Core.Caching;
using System.Globalization;
using LinqToDB;
using TvProgViewer.Data.TvProgMain;
using System.Threading;
using Newtonsoft.Json;
using TvProgViewer.Core.Domain.Users;
using System.Text.RegularExpressions;
using TvProgViewer.Services.Seo;
using TvProgViewer.Services.Catalog;
using DocumentFormat.OpenXml.Office.Word;
using TvProgViewer.Core.Domain.Catalog;
using LinqToDB.Linq;

namespace TvProgViewer.Services.TvProgMain
{
    /// <summary>
    /// Обеспечивает выборку телеканалов
    /// </summary>
    public class ChannelService: IChannelService
    {
        #region Поля

        private readonly IRepository<Channels> _channelsRepository;
        private readonly IRepository<MediaPic> _mediaPicRepository;
        private readonly IRepository<Programmes> _programmesRepository;
        private readonly IRepository<UserChannelMapping> _userChannelMappingRepository;
        private readonly IUrlRecordService _urlRecordService;
        private readonly ITvChannelService _tvChannelService;
        private readonly List<string> _firstMultiplex = new List<string> {
            "Первый канал", "Россия 1", "Матч!", "НТВ", "Пятый Канал", "Культура", "Россия 24", "Карусель", "ОТР", "ТВ Центр",
            "РЕН ТВ", "Спас ТВ", "СТС", "Домашний", "ТВ-3", "Пятница", "Звезда", "МИР", "ТНТ", "МУЗ-ТВ" }; 
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
            KeyValuePair<int, List<SystemChannel>> listSystemChannels = new KeyValuePair<int, List<SystemChannel>>();

            List<SystemChannel> systemChannel = new List<SystemChannel>();
            var sc = await (from ch in _channelsRepository.Table
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
                              .OrderBy(o => o.SysOrderCol).ThenBy(o => o.InternalId)
                              .ToListAsync();
            SetUrls(sc);
            
            int count = sc.Count();
            
            systemChannel = await sc.AsQueryable().OrderBy(!(sidx == null || sidx.Trim() == string.Empty) ? sidx : "SysOrderCol", sord)
                                        .Skip((page - 1) * rows).Take(rows)
                                        .ToListAsync<SystemChannel>();
            return new KeyValuePair<int, List<SystemChannel>>(count, systemChannel);
        }

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
        }
        #endregion
    }
}