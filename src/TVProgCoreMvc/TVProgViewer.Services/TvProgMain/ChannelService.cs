using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TVProgViewer.Data.TvProgMain.ProgObjs;
using TVProgViewer.Data;
using TVProgViewer.Core.Domain.TvProgMain;
using TVProgViewer.Core.Caching;
using System.Globalization;
using LinqToDB;
using TVProgViewer.Data.TvProgMain;
using System.Threading;
using Newtonsoft.Json;

namespace TVProgViewer.Services.TvProgMain
{
    /// <summary>
    /// Обеспечивает выборку телеканалов
    /// </summary>
    public class ChannelService: IChannelService
    {
        #region Поля

        private readonly IRepository<Channels> _channelsRepository;
        private readonly IRepository<MediaPic> _mediaPicRepository;

        #endregion

        #region Конструктор

        public ChannelService(IRepository<Channels> channelsRepositry,
                              IRepository<MediaPic> mediaPicRepository)
        {
            _channelsRepository = channelsRepositry;
            _mediaPicRepository = mediaPicRepository;
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
                            where ch.TvProgProviderId == tvProgProviderId && ch.Deleted == null && (filtData == null || ch.TitleChannel.Contains(filtData))
                            select new SystemChannel
                            {
                                ChannelId = ch.Id,
                                TVProgViewerId = ch.TvProgProviderId,
                                InternalId = ch.InternalId,
                                IconId = ch.IconId,
                                FileNameOrig = mp.PathOrig + mp.FileName,
                                FileName25 = mp.Path25 + mp.FileName,
                                Title = ch.TitleChannel,
                                ImageWebSrc = ch.IconWebSrc,
                                OrderCol = 0
                            }).OrderBy(o => o.InternalId)
                              .ToListAsync();
            int count = sc.Count();
            systemChannel = await sc.AsQueryable().OrderBy(!(sidx == null || sidx.Trim() == string.Empty) ? sidx : "InternalId", sord)
                                        .Skip((page - 1) * rows).Take(rows)
                                        .ToListAsync<SystemChannel>();
            return new KeyValuePair<int, List<SystemChannel>>(count, systemChannel);
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
                                               TVProgViewerId = ch.TvProgProviderId,
                                               InternalId = ch.InternalId,
                                               IconId = ch.IconId,
                                               FileNameOrig = mp.PathOrig + mp.FileName,
                                               FileName25 = mp.Path25 + mp.FileName,
                                               Title = ch.TitleChannel,
                                               ImageWebSrc = ch.IconWebSrc,
                                               OrderCol = 0
                                           }).OrderBy(o => o.InternalId)
                                             .ToListAsync();
            return await uch.Where(x => optChans.Any(y => y.ChannelId == x.ChannelId)).ToListAsync();
        }
        #endregion
    }
}