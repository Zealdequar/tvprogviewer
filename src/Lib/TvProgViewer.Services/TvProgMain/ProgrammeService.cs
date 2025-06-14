using DocumentFormat.OpenXml.Bibliography;
using DocumentFormat.OpenXml.Spreadsheet;
using LinqToDB;
using Microsoft.Extensions.Azure;
using System;
using System.Collections.Generic;
using System.Diagnostics.Eventing.Reader;
using System.Globalization;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using TvProgViewer.Core.Caching;
using TvProgViewer.Core.Domain.TvProgMain;
using TvProgViewer.Data;
using TvProgViewer.Data.TvProgMain;
using TvProgViewer.Data.TvProgMain.ProgObjs;

namespace TvProgViewer.Services.TvProgMain
{
    /// <summary>
    /// Обеспечивает выборку телепрограммы
    /// </summary>
    public class ProgrammeService: IProgrammeService
    {
        #region Поля
        private const string GRAY_COLOR = "grayColor";
        private const string BLACK_COLOR = "blackColor";
        private const string GREEN_COLOR = "greenColor";
        private const string ORDER_COL = "OrderCol";
        private const string ADULT_USERS = "Для взрослых";
        private const string AGE_18_PLUS = "(18+)";
        private const string TS_START_MO = "TsStartMo";
        private const string GREEN_ANONS_PNG = "GreenAnons.png";
        private readonly IRepository<TvProgProviders> _providerTypeRepository;
        private readonly IRepository<TypeProg> _typeProgRespository;
        private readonly IRepository<Programmes> _programmesRepository;
        private readonly IRepository<Channels> _channelsRepository;
        private readonly IRepository<RatingClassificator> _ratingClassificatorRepository;
        private readonly IRepository<Ratings> _ratingsRepository;
        private readonly IRepository<MediaPic> _mediaPicRepository;
        private readonly IRepository<GenreClassificator> _genreClassificatorRepository;
        private readonly IRepository<Genres> _genresRepository;
        private readonly IStaticCacheManager _cacheManager;
        private static readonly char[] separator = new[] { ';' };
        private readonly Dictionary<string, string> dictWeek = new Dictionary<string, string>()
                                                       {
                                                           {"monday", "Mon"},
                                                           {"tuesday", "Tue"},
                                                           {"wednesday", "Wen"},
                                                           {"thursday", "Ths" },
                                                           {"friday", "Fri" },
                                                           {"saturday", "Sat"},
                                                           {"sunday", "Sun"}
                                                       };
        private static readonly TimeZoneInfo MoscowTz =
        TimeZoneInfo.FindSystemTimeZoneById(
            RuntimeInformation.IsOSPlatform(OSPlatform.Windows) ?
            "Russian Standard Time" : "Europe/Moscow");
        #endregion

        #region Конструктор

        public ProgrammeService( IRepository<TvProgProviders> providerTypeRepository,
                                 IRepository<TypeProg> typeProgRepository,
                                 IRepository<Programmes> programmesRepository,
                                 IRepository<Channels> channelsRepository,
                                 IRepository<RatingClassificator> ratingClassificatorRepository,
                                 IRepository<Ratings> ratingsRepository,
                                 IRepository<MediaPic> mediaPicRepository,
                                 IRepository<GenreClassificator> genreClassificatorRepository,
                                 IRepository<Genres> genresRepository,
                                 IStaticCacheManager cacheManager)
        {
            _providerTypeRepository = providerTypeRepository;
            _typeProgRespository = typeProgRepository;
            _programmesRepository = programmesRepository;
            _channelsRepository = channelsRepository;
            _ratingClassificatorRepository = ratingClassificatorRepository;   
            _ratingsRepository = ratingsRepository;
            _mediaPicRepository = mediaPicRepository;
            _genreClassificatorRepository = genreClassificatorRepository;
            _genresRepository = genresRepository;
            _cacheManager = cacheManager;
        }

        #endregion

        #region Методы

        /// <summary>
        /// Получение ТВ-провайдера по его идентификатору
        /// </summary>
        /// <param name="providerId">Идентификатор ТВ-провайдера</param>
        public virtual async Task<TvProgProviders> GetProviderByIdAsync(int providerId)
        {
            if (providerId == 0)
                return null;

            return await _providerTypeRepository.GetByIdAsync(providerId, cache => default);
        }

        /// <summary>
        /// Получение всех ТВ-провайдеров
        /// </summary>
        public virtual async Task<IList<TvProgProviders>> GetAllProvidersAsync()
        {
            var providers = await _providerTypeRepository.GetAllAsync(query =>
            {
                return query;
            }, cache => cache.PrepareKeyForDefaultCache(TvProgProgrammeDefaults.ProvidersAllCacheKey));
            return await providers.ToListAsync();
        }

        /// <summary>
        /// Получение типа программы по его идентификатору
        /// </summary>
        /// <param name="typeProgId">Идентификатор типа программы</param>
        public virtual async Task<TypeProg> GetTypeProgByIdAsync(int typeProgId)
        {
            if (typeProgId == 0)
                return null;

            return await _typeProgRespository.GetByIdAsync(typeProgId);
        }

        /// <summary>
        /// Получение всех типов ТВ-программы
        /// </summary>
        public virtual async Task<IList<TypeProg>> GetAllTypeProgsAsync(bool showHidden = false)
        {
            var types = await _typeProgRespository.GetAllAsync(query =>
            {
                return query;
            }, cache => cache.PrepareKeyForDefaultCache(TvProgProgrammeDefaults.TypeProgAllCacheKey, showHidden));

            return await types.ToListAsync();
        }

        /// <summary>
        /// Получение списка провайдеров телепрограммы
        /// </summary>
        /// <returns></returns>
        public virtual async Task<IList<ProviderType>> GetProviderTypeListAsync()
        {
            return await (from tpp in _providerTypeRepository.Table
                    join tp in _typeProgRespository.Table on tpp.Id equals tp.TvProgProviderId
                    select new ProviderType
                    {
                        TvProgProviderId = tpp.Id,
                        ProviderName = tpp.ProviderName,
                        ProviderText = tpp.ProviderWebSite,
                        TypeProgId = tp.Id,
                        TypeName = tp.TypeName,
                        TypeEnum = (tp.TypeName == "Формат XMLTV") ? Enums.TypeProg.XMLTV : Enums.TypeProg.InterTV,
                        FileFormat = tp.FileFormat
                    }).ToListAsync();
        }

        

        /// <summary>
        /// Получение периода действия программы телепередач
        /// </summary>
        /// <param name="TypeProgId">Идентификатор тип программы</param>
        public virtual async Task<ProgPeriod> GetSystemProgrammePeriodAsync(int typeProgId)
        {
            ProgPeriod progPeriod = new ProgPeriod() { 
                dtStart = await _programmesRepository.Table.Where(x => x.TypeProgId == typeProgId).MinAsync(x => (DateTimeOffset?)x.TsStartMo),
                dtEnd = await _programmesRepository.Table.Where(x => x.TypeProgId == typeProgId).MaxAsync(x => (DateTimeOffset?)x.TsStopMo) 
            };
            
            return progPeriod;
        }

        /// <summary>
        /// Получение дней доступной программы
        /// </summary>
        /// <param name="typeProg">Идентификатор типа программы</param>
        /// <returns>Список дней</returns>
        public virtual async Task<List<DaysItem>> GetDaysAsync(int typeProg)
        {
            ProgPeriod periodMinMax = await GetSystemProgrammePeriodAsync(typeProg);
            DateTime tsMin = periodMinMax.dtStart.HasValue ? periodMinMax.dtStart.Value.DateTime : DateTime.UtcNow;
            tsMin = new DateTime(tsMin.Year, tsMin.Month, tsMin.Day, 5, 45, 0);
            DateTime tsMiddle = tsMin.AddDays(6);
            DateTime tsMax = periodMinMax.dtEnd.HasValue ? periodMinMax.dtEnd.Value.DateTime : DateTime.UtcNow;
            DateTime dtMax = tsMax.AddDays(-1);
            tsMax = new DateTime(dtMax.Year, dtMax.Month, dtMax.Day, 5, 45, 0, Calendar.CurrentEra);
            IFormatProvider provider = CultureInfo.GetCultureInfo("ru-RU");
            List<DaysItem> daysList = [];
            for (DateTime tsCurDate = tsMiddle.AddDays(1); tsCurDate <= tsMax; tsCurDate = tsCurDate.AddDays(1))
            {
                string strKey = dictWeek[tsCurDate.DayOfWeek.ToString().ToLower()];
                
                DaysItem dayItem = new()
                {
                    Name = tsCurDate.ToString("dd.MM.yyyy", provider),
                    DayOfWeek = tsCurDate.DayOfWeek.ToString(),
                    IconSrc = $"./images/i/{strKey}.png",
                    Color = (tsCurDate.Date < DateTime.UtcNow.Date) ? GRAY_COLOR : (tsCurDate.Date == DateTime.UtcNow.Date) ? BLACK_COLOR : GREEN_COLOR
                };
                daysList.Add(dayItem);
            }

            return await daysList.ToListAsync();
        }
        /// <summary>
        /// Получение категорий телепередач
        /// </summary>
        public virtual async Task<IList<string>> GetCategoriesAsync()
        {
            return await _programmesRepository.Table.Select(x => x.Category).Distinct().OrderBy(o => o).ToListAsync<string>();
        }

        /// <summary>
        /// Установка рейтингов
        /// </summary>
        /// <param name="systemProgramme">Программа телепередач</param>
        /// <param name="uid">Идентификатор пользователя</param>
        private void SetRatings(List<SystemProgramme> systemProgramme, long? uid)
        {
            if (!uid.HasValue)
                return;

            foreach (SystemProgramme sp in systemProgramme)
            {
                var ratingAttrs =  (from rc in _ratingClassificatorRepository.Table
                                   join r in _ratingsRepository.Table on rc.RatingId equals r.Id
                                   join mp2 in _mediaPicRepository.Table on r.IconId equals mp2.Id into rmp2
                                   from mp2 in rmp2.DefaultIfEmpty()
                                   where r.UserId == uid && rc.UserId == uid && r.Visible && r.DeleteDate == null &&
                                      (rc.DeleteAfterDate == null || rc.DeleteAfterDate > DateTime.Now)
                                   orderby rc.OrderCol
                                   select new { r.Id, r.RatingName, mp2.Path25, mp2.FileName, rc.ContainPhrases, rc.NonContainPhrases })
                                                .ToList()
                                                .Where(rc => sp.TelecastTitle.ToUpper().ContainsAny(rc.ContainPhrases.ToUpper().Split(';'))
                                                        && (string.IsNullOrWhiteSpace(rc.NonContainPhrases) ||
                                                        (!string.IsNullOrWhiteSpace(rc.NonContainPhrases) &&
                                                        !sp.TelecastTitle.ToUpper().ContainsAny(rc.NonContainPhrases.ToUpper().Split(';')))))
                                                .Select(mp2 => new { mp2.Id, mp2.RatingName, RatingContent = mp2.Path25 + mp2.FileName }).FirstOrDefault();
                sp.RatingId = ratingAttrs?.Id ?? (from r in _ratingsRepository.Table
                                                        where r.UserId == uid && r.RatingName == "Без рейтинга" && r.Visible && r.DeleteDate == null
                                                        select r.Id).FirstOrDefault();
                sp.RatingName = ratingAttrs?.RatingName ?? "Без рейтинга";
                sp.RatingContent = ratingAttrs?.RatingContent ?? (from m in _mediaPicRepository.Table
                                                                  where m.FileName == "favempty.png" && m.IsSystem
                                                                  select m.Path25 + m.FileName).FirstOrDefault();
            }
        }

        /// <summary>
        /// Установка жанров
        /// </summary>
        /// <param name="systemProgramme">Системная телепрограмма</param>
        /// <param name="Uid">Код пользователя</param>
        private void SetGenres(List<SystemProgramme> systemProgramme, long? uid)
        {
            Parallel.ForEach(systemProgramme, sp =>
            {
                var genreCategoryAttrs = (from gc in _genreClassificatorRepository.Table
                                          join g in _genresRepository.Table on gc.GenreId equals g.Id
                                          join mp2 in _mediaPicRepository.Table on g.IconId equals mp2.Id into gmp2
                                          from mp2 in gmp2.DefaultIfEmpty()
                                          where sp.Category == g.GenreName && g.UserId == uid && gc.UserId == uid && g.Visible && g.DeleteDate == null &&
                                           (gc.DeleteAfterDate == null || gc.DeleteAfterDate > DateTime.Now)
                                          orderby gc.OrderCol
                                          select new { g.Id, g.GenreName, GenreContent = mp2.Path25 + mp2.FileName }).FirstOrDefault();
                var genreClassifAttrs = (from gc in _genreClassificatorRepository.Table
                                         join g in _genresRepository.Table on gc.GenreId equals g.Id
                                         join mp2 in _mediaPicRepository.Table on g.IconId equals mp2.Id into gmp2
                                         from mp2 in gmp2.DefaultIfEmpty()
                                         where g.UserId == uid && gc.UserId == uid && g.Visible && g.DeleteDate == null &&
                                         (gc.DeleteAfterDate == null || gc.DeleteAfterDate > DateTime.Now)
                                         orderby gc.OrderCol
                                         select new { g.Id, g.GenreName, mp2.Path25, mp2.FileName, gc.ContainPhrases, gc.NonContainPhrases })
                                                .ToList()
                                                .Where(gc => sp.TelecastTitle.ToUpper().ContainsAny(gc.ContainPhrases.ToUpper().Split(';'))
                                                       && (string.IsNullOrWhiteSpace(gc.NonContainPhrases) ||
                                                       (!string.IsNullOrWhiteSpace(gc.NonContainPhrases) &&
                                                       !sp.TelecastTitle.ToUpper().ContainsAny(gc.NonContainPhrases.ToUpper().Split(';')))))
                                                .Select(mp2 => new { mp2.Id, mp2.GenreName, GenreContent = mp2.Path25 + mp2.FileName }).FirstOrDefault();
                sp.GenreId = genreCategoryAttrs?.Id ?? genreClassifAttrs?.Id ?? 1;
                sp.GenreName = genreCategoryAttrs?.GenreName ?? genreClassifAttrs?.GenreName ?? "Без типа";
                sp.GenreContent = genreCategoryAttrs?.GenreContent ?? genreClassifAttrs?.GenreContent;
            });
        }

        /// <summary>
        /// Фильтрация по жанрам
        /// </summary>
        /// <param name="systemProgramme">Системная программа</param>
        /// <param name="genres">Идентификаторы жанров через ;</param>
        private async Task<List<SystemProgramme>> FilterGenresAsync(List<SystemProgramme> systemProgramme, string genres)
        {
            if (string.IsNullOrWhiteSpace(genres))
                return systemProgramme;
            return await systemProgramme.Where(x => genres.Split(new[] { ';' }, StringSplitOptions.RemoveEmptyEntries)
                                .Any(g => long.TryParse(g, out long Gid) && long.Parse(g) == x.GenreId)).ToListAsync();
        }

        /// <summary>
        /// Фильтрация по каналам
        /// </summary>
        /// <param name="systemProgramme">Системная программа</param>
        /// <param name="channels">Идентификаторы каналов через ;</param>
        private async Task<List<SystemProgramme>> FilterChannelsAsync(List<SystemProgramme> systemProgramme, string channels)
        {
            if (string.IsNullOrWhiteSpace(channels))
                return systemProgramme;
            return await systemProgramme.Where(x => channels.Split(new char[] { ';' }, StringSplitOptions.RemoveEmptyEntries)
                                    .Any(c => long.TryParse(c, out long Cid) && long.Parse(c) == x.Cid)).ToListAsync();
        }


        private async Task<List<SystemProgramme>> FilterExcludeDates (List<SystemProgramme> systemProgramme, List<DateTime> rawDates)
        {
            DateTime minDateTime = await rawDates.AsQueryable().MinAsync(CancellationToken.None);
            DateTime maxDateTime = await rawDates.AsQueryable().MaxAsync(CancellationToken.None);
            maxDateTime = maxDateTime.AddDays(1);
            rawDates.Add(maxDateTime);
            return await systemProgramme.Where(x => rawDates.Contains(x.TsStartMo.Date)).ToListAsync(); 
        }

        /// <summary>
        /// Получение названия жанра через идентификатор
        /// </summary>
        /// <param name="idStr">Идентификатор жанра</param>
        private async Task<string> GetNameByIdGenre(string idStr, long? Uid)
        {
            long id;

            if (!long.TryParse(idStr, out id))
                return string.Empty;

            return await (from g in _genresRepository.Table
                    where g.Id == id && g.UserId == Uid && g.Visible && g.DeleteDate == null
                    select g.GenreName).FirstOrDefaultAsync(CancellationToken.None);
        }

        /// <summary>
        /// Получение телеканалов и их иконок
        /// </summary>
        /// <param name="systemProgramme">Системный список телепрограммы</param>
        private async Task<List<SystemProgramme>> ChannelIconJoinAsync(List<SystemProgramme> systemProgramme, int typeProgId, List<int> intListChannels)
        {
            var resultChannels = await (from ch in _channelsRepository.Table
                                        join mp in _mediaPicRepository.Table on ch.IconId equals mp.Id into chmp
                                        join pt in _providerTypeRepository.Table on ch.TvProgProviderId equals pt.Id
                                        join tp in _typeProgRespository.Table on pt.Id equals tp.TvProgProviderId
                                        from mp in chmp.DefaultIfEmpty()
                                        where tp.Id == typeProgId && ch.Deleted == null && 
                                              ((intListChannels.Any() && intListChannels.Contains(ch.Id)) || intListChannels.Count == 0)
                                        select new
                                        {
                                            ChannelId = ch.Id,
                                            ChannelName = ch.TitleChannel,
                                            ChannelContent = mp.Path25 + mp.FileName,
                                        }).ToListAsync();

            Parallel.ForEach(systemProgramme, x =>
            {
                var channelItem = resultChannels.Where(ch => ch.ChannelId == x.Cid).FirstOrDefault();
                if (channelItem != null)
                {
                    x.ChannelName = channelItem.ChannelName;
                    x.ChannelContent = channelItem.ChannelContent;
                }
            });

            return systemProgramme;
        }
        /// <summary>
        /// Вспомогательный метод для расчета процента оставшегося времени
        /// </summary>
        /// <param name="startTime">Дата начала</param>
        /// <param name="endTime">Дата завершения</param>
        /// <param name="currentTime">Текущее время</param>
        /// <returns>Процент</returns>
        private static int? CalculateRemainPercentage(DateTime startTime, DateTime endTime, DateTime currentTime)
        {
            var totalDuration = (endTime - startTime).TotalSeconds;
            if (totalDuration <= 0) return null;

            var elapsed = (currentTime - startTime).TotalSeconds;
            var percentage = (elapsed / totalDuration) * 100;

            return (int?)percentage;
        }

        /// <summary>
        /// Выборка телепрограммы
        /// </summary>
        /// <param name="TypeProgId">Тип программы телепередач</param>
        /// <param name="dateTimeOffset">Время</param>
        /// <param name="mode">режимы выборки: 1 - сейчас; 2 - следом</param>
        public virtual async Task<KeyValuePair<int, List<SystemProgramme>>> GetSystemProgrammesAsync(int TypeProgId, DateTimeOffset dateTimeOffset, int mode, string category,
                                                         string sidx, string sord, int page, int rows, string genres, string channels)
        {
            var spRows = new List<SystemProgramme>();
            DateTime minDate = new DateTime(1800, 1, 1);
            DateTime dateTime = dateTimeOffset.ToUniversalTime().DateTime;
            int spCount = 0;
            var intListChannels = string.IsNullOrWhiteSpace(channels) ? [] : await channels.Split(';', StringSplitOptions.RemoveEmptyEntries)
                .Select(int.Parse).ToListAsync();
            switch (mode)
            {
                case 1:
                    spCount = await (from pr in _programmesRepository.Table
                                         where pr.TypeProgId == TypeProgId && pr.TsStartMo <= dateTime &&
                                         dateTime < pr.TsStopMo && pr.Category != ADULT_USERS &&
                                         !pr.Title.Contains(AGE_18_PLUS) &&
                                         (category == null || pr.Category == category) &&
                                         ((intListChannels.Any() && intListChannels.Contains(pr.ChannelId)) || intListChannels.Count == 0)
                                         select pr.Id).CountAsync(CancellationToken.None);

                    spRows = await (from pr in _programmesRepository.Table
                                    where pr.TypeProgId == TypeProgId && pr.TsStartMo <= dateTime &&
                                    dateTime < pr.TsStopMo && pr.Category != ADULT_USERS &&
                                    !pr.Title.Contains(AGE_18_PLUS) &&
                                    (category == null || pr.Category == category) &&
                                    ((intListChannels.Any() && intListChannels.Contains(pr.ChannelId)) || intListChannels.Count == 0)
                                    select new SystemProgramme
                                    {
                                        ProgrammesId = pr.Id,
                                        Cid = pr.ChannelId,
                                        InternalChanId = pr.InternalChanId ?? 0,
                                        Start = TimeZoneInfo.ConvertTimeFromUtc(pr.TsStart, MoscowTz),
                                        Stop = TimeZoneInfo.ConvertTimeFromUtc(pr.TsStop, MoscowTz),
                                        TsStartMo = TimeZoneInfo.ConvertTimeFromUtc(pr.TsStartMo, MoscowTz),
                                        TsStopMo = TimeZoneInfo.ConvertTimeFromUtc(pr.TsStopMo, MoscowTz),
                                        TelecastTitle = pr.Title,
                                        TelecastDescr = pr.Descr,
                                        AnonsContent = !string.IsNullOrWhiteSpace(pr.Descr) ? GetAnonsImagePath() : null,
                                        Category = pr.Category,
                                        Remain = CalculateRemainPercentage(pr.TsStartMo, pr.TsStopMo, dateTime),
                                        OrderCol = _channelsRepository.Table.FirstOrDefault(ch => ch.Id == pr.ChannelId).SysOrderCol
                                    }).ToListAsync();
                    sidx = !string.IsNullOrWhiteSpace(sidx) ? sidx : ORDER_COL;
                    spRows = await spRows.AsQueryable()
                                         .LimitAndOrderBy(page, rows, sidx, sord)
                                         .ToListAsync();
                    SetGenres(spRows, null);
                    spRows = await ChannelIconJoinAsync(spRows, TypeProgId, intListChannels);
                    spRows = await FilterGenresAsync(spRows, genres);
                    spRows = await FilterChannelsAsync(spRows, channels);
                    break;
                case 2:
                    if (dateTime == minDate)
                    {
                        List<TsStopForRemain> stopAfter = await (from pr2 in _programmesRepository.Table
                                                                 where pr2.TsStartMo <= DateTime.UtcNow && DateTime.UtcNow < pr2.TsStopMo && pr2.TypeProgId == TypeProgId
                                                                   && pr2.Category != ADULT_USERS && !pr2.Title.Contains(AGE_18_PLUS)
                                                                   && ((intListChannels.Any() && intListChannels.Contains(pr2.ChannelId)) || intListChannels.Count == 0)
                                                                 select new TsStopForRemain
                                                                 {
                                                                     TsStopMoAfter = pr2.TsStopMo.AddSeconds(1),
                                                                     Cid = pr2.ChannelId
                                                                 }).ToListAsync();
                        var nowUtc = DateTime.UtcNow;
                        var afterTwoDays = nowUtc.AddDays(2);
                                                
                        var sp2 = await (from pr3 in _programmesRepository.Table
                                         where pr3.TypeProgId == TypeProgId
                                         && pr3.TsStartMo >= nowUtc && afterTwoDays > pr3.TsStopMo
                                         && pr3.Category != ADULT_USERS && !pr3.Title.Contains(AGE_18_PLUS)
                                         && (category == null || pr3.Category == category)
                                         && ((intListChannels.Any() && intListChannels.Contains(pr3.ChannelId)) || intListChannels.Count == 0)
                                         orderby pr3.InternalChanId, pr3.TsStartMo
                                         select new SystemProgramme
                                         {
                                             ProgrammesId = pr3.Id,
                                             Cid = pr3.ChannelId,
                                             InternalChanId = pr3.InternalChanId,
                                             Start = pr3.TsStart,
                                             Stop = pr3.TsStop,
                                             TsStartMo = pr3.TsStartMo,
                                             TsStopMo = pr3.TsStopMo, 
                                             TelecastTitle = pr3.Title,
                                             TelecastDescr = pr3.Descr,
                                             AnonsContent = (pr3.Descr != null && pr3.Descr != string.Empty) ? GetAnonsImagePath() : null,
                                             Category = pr3.Category,
                                             OrderCol = _channelsRepository.Table.FirstOrDefault(ch => ch.Id == pr3.ChannelId).SysOrderCol
                                         }).ToListAsync();

                        sp2 = await ChannelIconJoinAsync(sp2, TypeProgId, intListChannels);

                        spCount = await (from pr in await sp2.ToListAsync()
                                         join prin in stopAfter on pr.Cid equals prin.Cid
                                         where pr.TsStartMo <= prin.TsStopMoAfter && prin.TsStopMoAfter < pr.TsStopMo
                                         select pr.ProgrammesId
                                         ).AsQueryable().CountAsync(CancellationToken.None);
                        sidx = !string.IsNullOrWhiteSpace(sidx) ? sidx : ORDER_COL;
                        spRows = await (from pr in await sp2.ToListAsync()
                                         join prin in stopAfter on pr.Cid equals prin.Cid
                                         where pr.TsStartMo <= prin.TsStopMoAfter && prin.TsStopMoAfter < pr.TsStopMo
                                         select new SystemProgramme()
                                         {
                                             ProgrammesId = pr.ProgrammesId,
                                             Cid = pr.Cid,
                                             ChannelName = pr.ChannelName,
                                             ChannelContent = pr.ChannelContent,
                                             InternalChanId = pr.InternalChanId,
                                             Start = TimeZoneInfo.ConvertTimeFromUtc(pr.Start.DateTime, MoscowTz),
                                             Stop = TimeZoneInfo.ConvertTimeFromUtc(pr.Stop.DateTime, MoscowTz),
                                             TsStartMo = TimeZoneInfo.ConvertTimeFromUtc(pr.TsStartMo, MoscowTz),
                                             TsStopMo = TimeZoneInfo.ConvertTimeFromUtc(pr.TsStopMo, MoscowTz),
                                             TelecastTitle = pr.TelecastTitle,
                                             TelecastDescr = pr.TelecastDescr,
                                             AnonsContent = pr.AnonsContent,
                                             Category = pr.Category,
                                             Remain = (int) (pr.TsStartMo - DateTime.UtcNow).TotalSeconds,
                                             OrderCol = pr.OrderCol
                                         }).AsQueryable()
                                         .LimitAndOrderBy(page, rows, sidx, sord)
                                         .ToListAsync();
                        SetGenres(spRows, null);
                        spRows = await FilterGenresAsync(spRows, genres);
                        spRows = await FilterChannelsAsync(spRows, channels);
                    }
                    else if (dateTime > minDate)
                    {
                        spCount = await (from pr in _programmesRepository.Table
                                         where pr.TypeProgId == TypeProgId && DateTime.UtcNow < pr.TsStartMo && pr.TsStartMo <= dateTime
                                         && pr.Category != ADULT_USERS && !pr.Title.Contains(AGE_18_PLUS)
                                         && (category == null || pr.Category == category)
                                         && ((intListChannels.Any() && intListChannels.Contains(pr.ChannelId)) || intListChannels.Count == 0)
                                         orderby pr.InternalChanId, pr.TsStart
                                         select new SystemProgramme
                                         {
                                             ProgrammesId = pr.Id,
                                             Cid = pr.ChannelId,
                                             InternalChanId = pr.InternalChanId ?? 0,
                                             Start = TimeZoneInfo.ConvertTimeFromUtc(pr.TsStart, MoscowTz),
                                             Stop = TimeZoneInfo.ConvertTimeFromUtc(pr.TsStop, MoscowTz),
                                             TsStartMo = TimeZoneInfo.ConvertTimeFromUtc(pr.TsStartMo, MoscowTz),
                                             TsStopMo = TimeZoneInfo.ConvertTimeFromUtc(pr.TsStopMo, MoscowTz),
                                             TelecastTitle = pr.Title,
                                             TelecastDescr = pr.Descr,
                                             AnonsContent = (pr.Descr != null && pr.Descr != string.Empty) ? GetAnonsImagePath() : null,
                                             Category = pr.Category,
                                             OrderCol = _channelsRepository.Table.FirstOrDefault(ch => ch.Id == pr.ChannelId).SysOrderCol
                                         }).CountAsync(CancellationToken.None);
                        sidx = !string.IsNullOrWhiteSpace(sidx) ? sidx : ORDER_COL;
                        spRows = await (from pr in _programmesRepository.Table
                                         where pr.TypeProgId == TypeProgId && DateTime.UtcNow < pr.TsStartMo && pr.TsStartMo <= dateTime
                                         && pr.Category != ADULT_USERS && !pr.Title.Contains(AGE_18_PLUS)
                                         && (category == null || pr.Category == category)
                                         && ((intListChannels.Any() && intListChannels.Contains(pr.ChannelId)) || intListChannels.Count == 0)
                                         orderby pr.InternalChanId, pr.TsStart
                                         select new SystemProgramme
                                         {
                                             ProgrammesId = pr.Id,
                                             Cid = pr.ChannelId,
                                             InternalChanId = pr.InternalChanId ?? 0,
                                             Start = TimeZoneInfo.ConvertTimeFromUtc(pr.TsStart, MoscowTz),
                                             Stop = TimeZoneInfo.ConvertTimeFromUtc(pr.TsStop, MoscowTz),
                                             TsStartMo = TimeZoneInfo.ConvertTimeFromUtc(pr.TsStartMo, MoscowTz),
                                             TsStopMo = TimeZoneInfo.ConvertTimeFromUtc(pr.TsStopMo, MoscowTz),
                                             TelecastTitle = pr.Title,
                                             TelecastDescr = pr.Descr,
                                             AnonsContent = (pr.Descr != null && pr.Descr != string.Empty) ? GetAnonsImagePath() : null,
                                             Category = pr.Category,
                                             Remain = (int)(pr.TsStartMo - DateTime.UtcNow).TotalSeconds,
                                             OrderCol = _channelsRepository.Table.FirstOrDefault(ch => ch.Id == pr.ChannelId).SysOrderCol
                                         }).AsQueryable()
                                           .LimitAndOrderBy(page, rows, sidx, sord)
                                           .ToListAsync();
                        spRows = await ChannelIconJoinAsync(spRows, TypeProgId, intListChannels);
                        SetGenres(spRows, null);
                        spRows = await FilterGenresAsync(spRows, genres);
                        spRows = await FilterChannelsAsync(spRows, channels);
                    }
                    break;
            }
            return new KeyValuePair<int, List<SystemProgramme>>(spCount, spRows);
        }

        /// <summary>
        /// Выборка телепрограммы для совершеннолетних
        /// </summary>
        /// <param name="TypeProgId">Тип программы телепередач</param>
        /// <param name="dateTimeOffset">Время</param>
        /// <param name="mode">режимы выборки: 1 - сейчас; 2 - следом</param>
        public virtual async Task<KeyValuePair<int, List<SystemProgramme>>> GetUserAdultProgrammesAsync(int TypeProgId, DateTimeOffset dateTimeOffset, int mode, string category,
                                                         string sidx, string sord, int page, int rows, string genres, string channels)
        {
            var spRows = new List<SystemProgramme>();
            DateTime minDate = new DateTime(1800, 1, 1);
            DateTime dateTime = dateTimeOffset.ToUniversalTime().DateTime;
            int spCount = 0;
            var intListChannels = string.IsNullOrWhiteSpace(channels) ? [] : await channels.Split(';', StringSplitOptions.RemoveEmptyEntries)
                .Select(int.Parse).ToListAsync();
            switch (mode)
            {
                case 1:
                    spCount = await (from pr in _programmesRepository.Table
                                     where pr.TypeProgId == TypeProgId && pr.TsStartMo <= dateTime &&
                                     dateTime < pr.TsStopMo && (category == null || pr.Category == category) &&
                                     ((intListChannels.Any() && intListChannels.Contains(pr.ChannelId)) || intListChannels.Count == 0)
                                     select pr.Id).CountAsync(CancellationToken.None);
                    sidx = !string.IsNullOrWhiteSpace(sidx) ? sidx : ORDER_COL;
                    spRows = await (from pr in _programmesRepository.Table
                                    where pr.TypeProgId == TypeProgId && pr.TsStartMo <= dateTime &&
                                    dateTime < pr.TsStopMo && (category == null || pr.Category == category) &&
                                    ((intListChannels.Any() && intListChannels.Contains(pr.ChannelId)) || intListChannels.Count == 0)
                                    select new SystemProgramme
                                    {
                                        ProgrammesId = pr.Id,
                                        Cid = pr.ChannelId,
                                        InternalChanId = pr.InternalChanId ?? 0,
                                        Start = TimeZoneInfo.ConvertTimeFromUtc(pr.TsStart, MoscowTz),
                                        Stop = TimeZoneInfo.ConvertTimeFromUtc(pr.TsStop, MoscowTz),
                                        TsStartMo = TimeZoneInfo.ConvertTimeFromUtc(pr.TsStartMo, MoscowTz),
                                        TsStopMo = TimeZoneInfo.ConvertTimeFromUtc(pr.TsStopMo, MoscowTz),
                                        TelecastTitle = pr.Title,
                                        TelecastDescr = pr.Descr,
                                        AnonsContent = !string.IsNullOrWhiteSpace(pr.Descr) ? GetAnonsImagePath() : null,
                                        Category = pr.Category,
                                        Remain = CalculateRemainPercentage(pr.TsStartMo, pr.TsStopMo, dateTime),
                                        OrderCol = _channelsRepository.Table.FirstOrDefault(ch => ch.Id == pr.ChannelId).SysOrderCol
                                    }).AsQueryable()
                                      .LimitAndOrderBy(page, rows, sidx, sord)
                                      .ToListAsync();
                    SetGenres(spRows, null);
                    spRows = await ChannelIconJoinAsync(spRows, TypeProgId, intListChannels);
                    spRows = await FilterGenresAsync(spRows, genres);
                    spRows = await FilterChannelsAsync(spRows, channels);
                    break;
                case 2:
                    if (dateTime == minDate)
                    {
                        List<TsStopForRemain> stopAfter = await (from pr2 in _programmesRepository.Table
                                                                 where pr2.TsStartMo <= DateTime.UtcNow && DateTime.UtcNow < pr2.TsStopMo && pr2.TypeProgId == TypeProgId
                                                                    && ((intListChannels.Any() && intListChannels.Contains(pr2.ChannelId)) || intListChannels.Count == 0)
                                                                 select new TsStopForRemain
                                                                 {
                                                                     TsStopMoAfter = pr2.TsStopMo.AddSeconds(1),
                                                                     Cid = pr2.ChannelId
                                                                 }).ToListAsync();
                        DateTime afterTwoDays = DateTime.UtcNow.AddDays(2);

                        var sp2 = await (from pr3 in _programmesRepository.Table
                                         where pr3.TypeProgId == TypeProgId
                                         && pr3.TsStartMo >= DateTime.UtcNow && afterTwoDays > pr3.TsStopMo
                                         && (category == null || pr3.Category == category)
                                         && ((intListChannels.Any() && intListChannels.Contains(pr3.ChannelId)) || intListChannels.Count == 0)
                                         orderby pr3.InternalChanId, pr3.TsStartMo
                                         select new SystemProgramme
                                         {
                                             ProgrammesId = pr3.Id,
                                             Cid = pr3.ChannelId,
                                             InternalChanId = pr3.InternalChanId,
                                             Start = pr3.TsStart,
                                             Stop = pr3.TsStop,
                                             TsStartMo = pr3.TsStartMo,
                                             TsStopMo = pr3.TsStopMo,
                                             TelecastTitle = pr3.Title,
                                             TelecastDescr = pr3.Descr,
                                             AnonsContent = (pr3.Descr != null && pr3.Descr != string.Empty) ? GetAnonsImagePath() : null,
                                             Category = pr3.Category,
                                             OrderCol = _channelsRepository.Table.FirstOrDefault(ch => ch.Id == pr3.ChannelId).SysOrderCol
                                         }).ToListAsync();

                        sp2 = await ChannelIconJoinAsync(sp2, TypeProgId, intListChannels);

                        spCount = await (from pr in await sp2.ToListAsync()
                                         join prin in stopAfter on pr.Cid equals prin.Cid
                                         where pr.TsStartMo <= prin.TsStopMoAfter && prin.TsStopMoAfter < pr.TsStopMo
                                         select pr.ProgrammesId
                                         ).AsQueryable().CountAsync(CancellationToken.None);
                        sidx = !string.IsNullOrWhiteSpace(sidx) ? sidx : ORDER_COL;
                        spRows = await (from pr in await sp2.ToListAsync()
                                        join prin in stopAfter on pr.Cid equals prin.Cid
                                        where pr.TsStartMo <= prin.TsStopMoAfter && prin.TsStopMoAfter < pr.TsStopMo
                                        select new SystemProgramme()
                                        {
                                            ProgrammesId = pr.ProgrammesId,
                                            Cid = pr.Cid,
                                            ChannelName = pr.ChannelName,
                                            ChannelContent = pr.ChannelContent,
                                            InternalChanId = pr.InternalChanId,
                                            Start = TimeZoneInfo.ConvertTimeFromUtc(pr.Start.DateTime, MoscowTz),
                                            Stop = TimeZoneInfo.ConvertTimeFromUtc(pr.Stop.DateTime, MoscowTz),
                                            TsStartMo = TimeZoneInfo.ConvertTimeFromUtc(pr.TsStartMo, MoscowTz),
                                            TsStopMo = TimeZoneInfo.ConvertTimeFromUtc(pr.TsStopMo, MoscowTz),
                                            TelecastTitle = pr.TelecastTitle,
                                            TelecastDescr = pr.TelecastDescr,
                                            AnonsContent = pr.AnonsContent,
                                            Category = pr.Category,
                                            Remain = (int)(pr.TsStartMo - DateTime.UtcNow).TotalSeconds,
                                            OrderCol = pr.OrderCol
                                        }).AsQueryable()
                                          .LimitAndOrderBy(page, rows, sidx, sord)
                                          .ToListAsync();
                        SetGenres(spRows, null);
                        spRows = await FilterGenresAsync(spRows, genres);
                        spRows = await FilterChannelsAsync(spRows, channels);
                    }
                    else if (dateTime > minDate)
                    {
                        spCount = await (from pr in _programmesRepository.Table
                                         where pr.TypeProgId == TypeProgId && DateTime.Now < pr.TsStartMo && pr.TsStartMo <= dateTime
                                         && (category == null || pr.Category == category)
                                         && ((intListChannels.Any() && intListChannels.Contains(pr.ChannelId)) || intListChannels.Count == 0)
                                         orderby pr.InternalChanId, pr.TsStart
                                         select new SystemProgramme
                                         {
                                             ProgrammesId = pr.Id,
                                             Cid = pr.ChannelId,
                                             InternalChanId = pr.InternalChanId ?? 0,
                                             Start = pr.TsStart,
                                             Stop = pr.TsStop,
                                             TsStartMo = pr.TsStartMo,
                                             TsStopMo = pr.TsStopMo,
                                             TelecastTitle = pr.Title,
                                             TelecastDescr = pr.Descr,
                                             AnonsContent = (pr.Descr != null && pr.Descr != string.Empty) ? GetAnonsImagePath() : null,
                                             Category = pr.Category,
                                             OrderCol = _channelsRepository.Table.FirstOrDefault(ch => ch.Id == pr.ChannelId).SysOrderCol
                                         }).CountAsync(CancellationToken.None);
                        sidx = !string.IsNullOrWhiteSpace(sidx) ? sidx : ORDER_COL;
                        spRows = await (from pr in _programmesRepository.Table
                                        where pr.TypeProgId == TypeProgId && DateTime.Now < pr.TsStartMo && pr.TsStartMo <= dateTime
                                        && (category == null || pr.Category == category)
                                        && ((intListChannels.Any() && intListChannels.Contains(pr.ChannelId)) || intListChannels.Count == 0)
                                        orderby pr.InternalChanId, pr.TsStart
                                        select new SystemProgramme
                                        {
                                            ProgrammesId = pr.Id,
                                            Cid = pr.ChannelId,
                                            InternalChanId = pr.InternalChanId ?? 0,
                                            Start = TimeZoneInfo.ConvertTimeFromUtc(pr.TsStart, MoscowTz),
                                            Stop = TimeZoneInfo.ConvertTimeFromUtc(pr.TsStop, MoscowTz),
                                            TsStartMo = TimeZoneInfo.ConvertTimeFromUtc(pr.TsStartMo, MoscowTz),
                                            TsStopMo = TimeZoneInfo.ConvertTimeFromUtc(pr.TsStopMo, MoscowTz),
                                            TelecastTitle = pr.Title,
                                            TelecastDescr = pr.Descr,
                                            AnonsContent = (pr.Descr != null && pr.Descr != string.Empty) ? GetAnonsImagePath() : null,
                                            Category = pr.Category,
                                            Remain = (int)(pr.TsStartMo - DateTime.UtcNow).TotalSeconds,
                                            OrderCol = _channelsRepository.Table.FirstOrDefault(ch => ch.Id == pr.ChannelId).SysOrderCol
                                        }).AsQueryable()
                                          .LimitAndOrderBy(page, rows, sidx, sord)
                                          .ToListAsync();
                        spRows = await ChannelIconJoinAsync(spRows, TypeProgId, intListChannels);
                        SetGenres(spRows, null);
                        spRows = await FilterGenresAsync(spRows, genres);
                        spRows = await FilterChannelsAsync(spRows, channels);
                    }
                    break;
            }
            return new KeyValuePair<int, List<SystemProgramme>>(spCount, spRows);
        }

        /// <summary>
        /// Поиск телепередачи
        /// </summary>
        /// <param name="TypeProgId">Тип программы телепередач</param>
        /// <param name="findTitle">Поисковая подстрока</param>
        public virtual async Task<KeyValuePair<int, List<SystemProgramme>>> SearchProgrammeAsync(int typeProgId, string findTitle, string category
                                                               , string sidx, string sord, int page, int rows, string genres, string dates, string channels)

        {
            int spCount = 0;
            var intListChannels = string.IsNullOrWhiteSpace(channels) ? [] : 
                await channels.Split(';', StringSplitOptions.RemoveEmptyEntries).Select(int.Parse).ToListAsync();
            if (string.IsNullOrWhiteSpace(dates))
                return new KeyValuePair<int, List<SystemProgramme>>();
            
            if (genres == null)
                genres = "";

            List<long> repoGenres = _genresRepository.Table.Where(x => x.Visible && x.UserId == null).Select(x => (long)x.Id).ToList();
            var setGenres = new HashSet<long>(repoGenres);
            List<long> userGenres = genres.Split(new[] { ';' }, StringSplitOptions.RemoveEmptyEntries).Where(g =>
                    long.TryParse(g, out long Gid)).Select(long.Parse).ToList();
            if (string.IsNullOrWhiteSpace(findTitle) && (string.IsNullOrWhiteSpace(genres) || setGenres.SetEquals(userGenres)))
                return new KeyValuePair<int, List<SystemProgramme>>();
            

            DateTime date;
            CultureInfo ci = new("Ru-ru");
            var rawDates = await dates.Split(separator, StringSplitOptions.RemoveEmptyEntries)
                .Select(x => DateTime.TryParseExact(x, "yyyyMMdd", ci, DateTimeStyles.None, out date) ? 
                DateTime.ParseExact(x, "yyyyMMdd", ci) : 
                new DateTime()).ToListAsync();
            var minDateTime = await rawDates.AsQueryable().MinAsync(CancellationToken.None);
            var maxDateTime = await rawDates.AsQueryable().MaxAsync(CancellationToken.None);
            maxDateTime = maxDateTime.AddDays(1);

            var systemProgramme = await (from pr in _programmesRepository.Table
                                         where pr.TypeProgId == typeProgId
                                    && pr.Category != ADULT_USERS && !pr.Title.Contains(AGE_18_PLUS)
                                    && (category == null || pr.Category == category)
                                    && (string.IsNullOrWhiteSpace(findTitle) || pr.Title.Contains(findTitle))
                                    && minDateTime <= pr.TsStartMo && pr.TsStartMo <= maxDateTime
                                    && ((intListChannels.Any() && intListChannels.Contains(pr.ChannelId)) || intListChannels.Count == 0)
                                         orderby pr.TsStartMo, pr.TsStopMo
                                         select new SystemProgramme
                                         {
                                             ProgrammesId = pr.Id,
                                             Cid = pr.ChannelId,
                                             InternalChanId = pr.InternalChanId ?? 0,
                                             Start = TimeZoneInfo.ConvertTimeFromUtc(pr.TsStart, MoscowTz),
                                             Stop = TimeZoneInfo.ConvertTimeFromUtc(pr.TsStop, MoscowTz),
                                             TsStartMo = TimeZoneInfo.ConvertTimeFromUtc(pr.TsStartMo, MoscowTz),
                                             TsStopMo = TimeZoneInfo.ConvertTimeFromUtc(pr.TsStopMo, MoscowTz),
                                             TelecastTitle = pr.Title,
                                             TelecastDescr = pr.Descr,
                                             AnonsContent = (pr.Descr != null && pr.Descr != string.Empty) ? GetAnonsImagePath() : null,
                                             Category = pr.Category,
                                             Remain = 1
                                         }).ToListAsync();
            
            systemProgramme = await ChannelIconJoinAsync(systemProgramme, typeProgId, intListChannels);
            Parallel.ForEach(systemProgramme, pr =>
            {
                pr.DayMonth = pr.TsStartMo.ToString("ddd", new CultureInfo("ru-Ru")) +
               String.Format("({0:D2}.{1:D2})", pr.TsStartMo.Day, pr.TsStartMo.Month);
            });
            systemProgramme = await FilterChannelsAsync(systemProgramme, channels);
            systemProgramme = await FilterExcludeDates(systemProgramme, rawDates);
            SetGenres(systemProgramme, null);
            systemProgramme = await FilterGenresAsync(systemProgramme, genres);
            spCount = systemProgramme.Count;
            sidx = !string.IsNullOrWhiteSpace(sidx) ? sidx : TS_START_MO;
            systemProgramme = await systemProgramme.AsQueryable()
                                                   .LimitAndOrderBy(page, rows, sidx, sord)
                                                   .ToListAsync();
            return new KeyValuePair<int, List<SystemProgramme>>(spCount, systemProgramme);
        }

        /// <summary>
        /// Поиск телепередачи для совершеннолетних
        /// </summary>
        /// <param name="TypeProgId">Тип программы телепередач</param>
        /// <param name="findTitle">Поисковая подстрока</param>
        public virtual async Task<KeyValuePair<int, List<SystemProgramme>>> SearchAdultProgrammeAsync(int typeProgId, string findTitle, string category
                                                               , string sidx, string sord, int page, int rows, string genres, string dates, string channels)

        {
            int spCount = 0;
            var intListChannels = string.IsNullOrWhiteSpace(channels) ? [] :
                await channels.Split(';', StringSplitOptions.RemoveEmptyEntries).Select(int.Parse).ToListAsync();
            if (string.IsNullOrWhiteSpace(dates))
                return new KeyValuePair<int, List<SystemProgramme>>();

            if (genres == null)
                genres = "";

            List<long> repoGenres = _genresRepository.Table.Where(x => x.Visible && x.UserId == null).Select(x => (long)x.Id).ToList();
            var setGenres = new HashSet<long>(repoGenres);
            List<long> userGenres = genres.Split(new[] { ';' }, StringSplitOptions.RemoveEmptyEntries).Where(g =>
                    long.TryParse(g, out long Gid)).Select(long.Parse).ToList();
            if (string.IsNullOrWhiteSpace(findTitle) && (string.IsNullOrWhiteSpace(genres) || setGenres.SetEquals(userGenres)))
                return new KeyValuePair<int, List<SystemProgramme>>();


            DateTime date;
            CultureInfo ci = new("Ru-ru");
            var rawDates = await dates.Split(separator, StringSplitOptions.RemoveEmptyEntries)
                .Select(x => DateTime.TryParseExact(x, "yyyyMMdd", ci, DateTimeStyles.None, out date) ?
                DateTime.ParseExact(x, "yyyyMMdd", ci) :
                new DateTime()).ToListAsync();
            var minDateTime = await rawDates.AsQueryable().MinAsync(CancellationToken.None);
            var maxDateTime = await rawDates.AsQueryable().MaxAsync(CancellationToken.None);
            maxDateTime = maxDateTime.AddDays(1);

            var systemProgramme = await (from pr in _programmesRepository.Table
                                         where pr.TypeProgId == typeProgId
                                    && (category == null || pr.Category == category)
                                    && (string.IsNullOrWhiteSpace(findTitle) || pr.Title.Contains(findTitle))
                                    && minDateTime <= pr.TsStartMo && pr.TsStartMo <= maxDateTime
                                    && ((intListChannels.Any() && intListChannels.Contains(pr.ChannelId)) || intListChannels.Count == 0)
                                         orderby pr.TsStartMo, pr.TsStopMo
                                         select new SystemProgramme
                                         {
                                             ProgrammesId = pr.Id,
                                             Cid = pr.ChannelId,
                                             InternalChanId = pr.InternalChanId ?? 0,
                                             Start = TimeZoneInfo.ConvertTimeFromUtc(pr.TsStart, MoscowTz),
                                             Stop = TimeZoneInfo.ConvertTimeFromUtc(pr.TsStop, MoscowTz),
                                             TsStartMo = TimeZoneInfo.ConvertTimeFromUtc(pr.TsStartMo, MoscowTz),
                                             TsStopMo = TimeZoneInfo.ConvertTimeFromUtc(pr.TsStopMo, MoscowTz),
                                             TelecastTitle = pr.Title,
                                             TelecastDescr = pr.Descr,
                                             AnonsContent = (pr.Descr != null && pr.Descr != string.Empty) ? GetAnonsImagePath() : null,
                                             Category = pr.Category,
                                             Remain = 1
                                         }).ToListAsync();

            systemProgramme = await ChannelIconJoinAsync(systemProgramme, typeProgId, intListChannels);
            Parallel.ForEach(systemProgramme, pr =>
            {
                pr.DayMonth = pr.TsStartMo.ToString("ddd", new CultureInfo("ru-Ru")) +
           String.Format("({0:D2}.{1:D2})", pr.TsStartMo.Day, pr.TsStartMo.Month);
            });
            systemProgramme = await FilterChannelsAsync(systemProgramme, channels);
            systemProgramme = await FilterExcludeDates(systemProgramme, rawDates);
            SetGenres(systemProgramme, null);
            systemProgramme = await FilterGenresAsync(systemProgramme, genres);
            spCount = systemProgramme.Count;
            sidx = !string.IsNullOrWhiteSpace(sidx) ? sidx : TS_START_MO;
            systemProgramme = await systemProgramme.AsQueryable()
                                                   .LimitAndOrderBy(page, rows, sidx, sord)                                     
                                                   .ToListAsync();
            return new KeyValuePair<int, List<SystemProgramme>>(spCount, systemProgramme);
        }

        /// <summary>
        /// Глобальный поиск телепередачи
        /// </summary>
        /// <param name="typeProgId">Тип программы телепередач</param>
        /// <param name="findTitle">Поисковая подстрока</param>
        /// <param name="category">Категория</param>
        /// <param name="genres">Жанры</param>
        /// <param name="channels">Каналы</param>
        public async Task<KeyValuePair<int, List<SystemProgramme>>> SearchGlobalProgrammeAsync(int typeProgId, string findTitle, string category,
                                                         string sidx, string sord, int page, int rows, string genres, string channels)
        {
            int spCount = 0;
            var intListChannels = string.IsNullOrWhiteSpace(channels) ? [] : 
                await channels.Split(';', StringSplitOptions.RemoveEmptyEntries).Select(int.Parse).ToListAsync();
            CultureInfo ci = new("Ru-ru");

            spCount = await (from pr in _programmesRepository.Table
                             where pr.TypeProgId == typeProgId
                        && pr.Category != ADULT_USERS && !pr.Title.Contains(AGE_18_PLUS)
                        && (category == null || pr.Category == category)
                        && pr.Title.Contains(findTitle)
                        && ((intListChannels.Any() && intListChannels.Contains(pr.ChannelId)) || intListChannels.Count == 0)
                             select pr.Id).CountAsync(CancellationToken.None);
            sidx = !string.IsNullOrWhiteSpace(sidx) ? sidx : TS_START_MO;
            var systemProgramme = await (from pr in _programmesRepository.Table
                                         where pr.TypeProgId == typeProgId
                                    && pr.Category != ADULT_USERS && !pr.Title.Contains(AGE_18_PLUS)
                                    && (category == null || pr.Category == category)
                                    && pr.Title.Contains(findTitle)
                                    && ((intListChannels.Any() && intListChannels.Contains(pr.ChannelId)) || intListChannels.Count == 0)
                                         orderby pr.TsStartMo, pr.TsStopMo
                                         select new SystemProgramme
                                         {
                                             ProgrammesId = pr.Id,
                                             Cid = pr.ChannelId,
                                             InternalChanId = pr.InternalChanId ?? 0,
                                             Start = TimeZoneInfo.ConvertTimeFromUtc(pr.TsStart, MoscowTz),
                                             Stop = TimeZoneInfo.ConvertTimeFromUtc(pr.TsStop, MoscowTz),
                                             TsStartMo = TimeZoneInfo.ConvertTimeFromUtc(pr.TsStartMo, MoscowTz),
                                             TsStopMo = TimeZoneInfo.ConvertTimeFromUtc(pr.TsStopMo, MoscowTz),
                                             TelecastTitle = pr.Title,
                                             TelecastDescr = pr.Descr,
                                             AnonsContent = (pr.Descr != null && pr.Descr != string.Empty) ? GetAnonsImagePath() : null,
                                             Category = pr.Category,
                                             Remain = 1
                                         }).AsQueryable()
                                           .LimitAndOrderBy(page, rows, sidx, sord)
                                           .ToListAsync();

            systemProgramme = await ChannelIconJoinAsync(systemProgramme, typeProgId, intListChannels);
            _ = Parallel.ForEach(systemProgramme, pr =>
            {
                pr.DayMonth = $"{pr.TsStartMo.ToString("ddd", new CultureInfo("ru-Ru"))}{string.Format("({0:D2}.{1:D2})", pr.TsStartMo.Day, pr.TsStartMo.Month)}";
            });
            SetGenres(systemProgramme, null);
            systemProgramme = await FilterGenresAsync(systemProgramme, genres);
            systemProgramme = await FilterChannelsAsync(systemProgramme, channels);
                       
            return new KeyValuePair<int, List<SystemProgramme>>(spCount, systemProgramme);
        }

        /// <summary>
        /// Глобальный поиск телепередачи для совершеннолетних
        /// </summary>
        /// <param name="typeProgId">Тип программы телепередач</param>
        /// <param name="findTitle">Поисковая подстрока</param>
        /// <param name="category">Категория</param>
        /// <param name="genres">Жанры</param>
        /// <param name="channels">Каналы</param>
        public async Task<KeyValuePair<int, List<SystemProgramme>>> SearchAdultGlobalProgrammeAsync(int typeProgId, string findTitle, string category,
                                                         string sidx, string sord, int page, int rows, string genres, string channels)
        {
            int spCount = 0;
            var intListChannels = string.IsNullOrWhiteSpace(channels) ? [] :
                await channels.Split(';', StringSplitOptions.RemoveEmptyEntries).Select(int.Parse).ToListAsync();
            CultureInfo ci = new("Ru-ru");

            spCount = await (from pr in _programmesRepository.Table
                             where pr.TypeProgId == typeProgId
                        && (category == null || pr.Category == category)
                        && pr.Title.Contains(findTitle)
                        && ((intListChannels.Any() && intListChannels.Contains(pr.ChannelId)) || intListChannels.Count == 0)
                             select pr.Id).CountAsync(CancellationToken.None);
            sidx = !string.IsNullOrWhiteSpace(sidx) ? sidx : TS_START_MO;
            var systemProgramme = await (from pr in _programmesRepository.Table
                                         where pr.TypeProgId == typeProgId
                                    && (category == null || pr.Category == category)
                                    && pr.Title.Contains(findTitle)
                                    && ((intListChannels.Any() && intListChannels.Contains(pr.ChannelId)) || intListChannels.Count == 0)
                                         orderby pr.TsStartMo, pr.TsStopMo
                                         select new SystemProgramme
                                         {
                                             ProgrammesId = pr.Id,
                                             Cid = pr.ChannelId,
                                             InternalChanId = pr.InternalChanId ?? 0,
                                             Start = TimeZoneInfo.ConvertTimeFromUtc(pr.TsStart, MoscowTz),
                                             Stop = TimeZoneInfo.ConvertTimeFromUtc(pr.TsStop, MoscowTz),
                                             TsStartMo = TimeZoneInfo.ConvertTimeFromUtc(pr.TsStartMo, MoscowTz),
                                             TsStopMo = TimeZoneInfo.ConvertTimeFromUtc(pr.TsStopMo, MoscowTz),
                                             TelecastTitle = pr.Title,
                                             TelecastDescr = pr.Descr,
                                             AnonsContent = (pr.Descr != null && pr.Descr != string.Empty) ? GetAnonsImagePath() : null,
                                             Category = pr.Category,
                                             Remain = 1
                                         }).AsQueryable()
                                           .LimitAndOrderBy(page, rows, sidx, sord)
                                           .ToListAsync();

            systemProgramme = await ChannelIconJoinAsync(systemProgramme, typeProgId, intListChannels);
            _ = Parallel.ForEach(systemProgramme, pr =>
            {
                pr.DayMonth = $"{pr.TsStartMo.ToString("ddd", new CultureInfo("ru-Ru"))}{string.Format("({0:D2}.{1:D2})", pr.TsStartMo.Day, pr.TsStartMo.Month)}";
            });
            SetGenres(systemProgramme, null);
            systemProgramme = await FilterGenresAsync(systemProgramme, genres);
            systemProgramme = await FilterChannelsAsync(systemProgramme, channels);

            return new KeyValuePair<int, List<SystemProgramme>>(spCount, systemProgramme);
        }

        /// <summary>
        /// Получение пользовательских телепередач за день
        /// </summary>
        /// <param name="uid">Идентификатор пользователя</param>
        /// <param name="typeProgID">Идентификатор типа программы телепередач</param>
        /// <param name="channelId">Код канала</param>
        /// <param name="tsStart">Время начала выборки</param>
        /// <param name="tsEnd">Время завершения выборки</param>
        /// <param name="category">Категория</param>
        public virtual async Task<List<SystemProgramme>> GetUserProgrammesOfDayListAsync(int typeProgId, int channelId, DateTime tsStart, DateTime tsEnd, string category)
        {
            // Сначала получаем данные из БД без преобразования времени:
            var programmes = await (from pr in _programmesRepository.Table
                                    join ch in _channelsRepository.Table on pr.ChannelId equals ch.Id
                                    join mp in _mediaPicRepository.Table on ch.IconId equals mp.Id into chmp
                                    from mp in chmp.DefaultIfEmpty()
                                    where pr.TypeProgId == typeProgId &&
                                          ch.Id == channelId &&
                                          pr.TsStartMo >= tsStart &&
                                          pr.TsStopMo <= tsEnd &&
                                          pr.Category != ADULT_USERS &&
                                          !pr.Title.Contains(AGE_18_PLUS) &&
                                          (category == null || pr.Category == category) &&
                                          ch.Deleted == null
                                    select new
                                    {
                                        Programme = pr,
                                        Channel = ch,
                                        Icon = mp
                                    })
                                  .OrderBy(x => x.Programme.TsStartMo)
                                  .ToListAsync();

            // Затем преобразуем время в памяти:
            var systemProgrammes = programmes.Select(x => new SystemProgramme
            {
                ProgrammesId = x.Programme.Id,
                Cid = x.Programme.ChannelId,
                ChannelName = x.Channel.TitleChannel,
                InternalChanId = x.Programme.InternalChanId,
                Start = TimeZoneInfo.ConvertTimeFromUtc(x.Programme.TsStart, MoscowTz),
                Stop = TimeZoneInfo.ConvertTimeFromUtc(x.Programme.TsStop, MoscowTz),
                TsStartMo = TimeZoneInfo.ConvertTimeFromUtc(x.Programme.TsStartMo, MoscowTz),
                TsStopMo = TimeZoneInfo.ConvertTimeFromUtc(x.Programme.TsStopMo, MoscowTz),
                TelecastTitle = x.Programme.Title,
                TelecastDescr = x.Programme.Descr,
                AnonsContent = !string.IsNullOrEmpty(x.Programme.Descr)
                    ? GetAnonsImagePath()
                    : null,
                Category = x.Programme.Category,
            }).ToList();

            SetGenres(systemProgrammes, null);
            return systemProgrammes;
        }

        /// <summary>
        /// Получение картинки анонса
        /// </summary>
        /// <returns></returns>
        private string GetAnonsImagePath()
        {
            var image = _mediaPicRepository.Table.FirstOrDefault(mp => mp.FileName == GREEN_ANONS_PNG);
            return image != null ? image.Path25 + image.FileName : null;
        }

        /// <summary>
        /// Получение пользовательских телепередач за день для совершеннолетних
        /// </summary>
        /// <param name="uid">Идентификатор пользователя</param>
        /// <param name="typeProgID">Идентификатор типа программы телепередач</param>
        /// <param name="channelId">Код канала</param>
        /// <param name="tsStart">Время начала выборки</param>
        /// <param name="tsEnd">Время завершения выборки</param>
        /// <param name="category">Категория</param>
        public virtual async Task<List<SystemProgramme>> GetUserAdultProgrammesOfDayListAsync(int typeProgId, int channelId, DateTime tsStart, DateTime tsEnd, string category)
        {
            // Сначала получаем данные из БД без преобразования времени:
            var programmes = await (from pr in _programmesRepository.Table
                                    join ch in _channelsRepository.Table on pr.ChannelId equals ch.Id
                                    join mp in _mediaPicRepository.Table on ch.IconId equals mp.Id into chmp
                                    from mp in chmp.DefaultIfEmpty()
                                    where pr.TypeProgId == typeProgId &&
                                          ch.Id == channelId &&
                                          pr.TsStartMo >= tsStart &&
                                          pr.TsStopMo <= tsEnd &&
                                          (category == null || pr.Category == category) &&
                                          ch.Deleted == null
                                    select new
                                    {
                                        Programme = pr,
                                        Channel = ch,
                                        HasDescription = !string.IsNullOrEmpty(pr.Descr)
                                    })
                                  .OrderBy(x => x.Programme.TsStartMo)
                                  .ToListAsync();

            // Получаем путь к изображению анонса один раз:
            var anonsImagePath = GetAnonsImagePath();

            // Затем преобразуем время в памяти:
            var systemProgrammes = programmes.Select(x => new SystemProgramme
            {
                ProgrammesId = x.Programme.Id,
                Cid = x.Programme.ChannelId,
                ChannelName = x.Channel.TitleChannel,
                InternalChanId = x.Programme.InternalChanId,
                Start = TimeZoneInfo.ConvertTimeFromUtc(x.Programme.TsStart, MoscowTz),
                Stop = TimeZoneInfo.ConvertTimeFromUtc(x.Programme.TsStop, MoscowTz),
                TsStartMo = TimeZoneInfo.ConvertTimeFromUtc(x.Programme.TsStartMo, MoscowTz),
                TsStopMo = TimeZoneInfo.ConvertTimeFromUtc(x.Programme.TsStopMo, MoscowTz),
                TelecastTitle = x.Programme.Title,
                TelecastDescr = x.Programme.Descr,
                AnonsContent = x.HasDescription ? anonsImagePath : null,
                Category = x.Programme.Category,
            }).ToList();

            SetGenres(systemProgrammes, null);
            return systemProgrammes;
        }



        /// <summary>
        /// Получение всей телепрограммы
        /// </summary>
        /// <param name="page">Страница</param>
        /// <param name="rows">Строки</param>
        public virtual async Task<List<GdProgramme>> GetAllProgrammes(int page, int rows)
        {
            List<GdProgramme> gdProgrammeList = await (from pr in _programmesRepository.Table
                                                               orderby pr.InternalChanId, pr.TsStartMo
                                                               select new GdProgramme
                                                               {
                                                                   InternalChanId = pr.InternalChanId,
                                                                   TsStartMo = pr.TsStartMo,
                                                                   TsStopMo = pr.TsStopMo,
                                                                   TelecastTitle = pr.Title,
                                                                   Category = pr.Category
                                                               }).LimitAndOrderBy(page, rows, "InternalChanId", "asc").ToListAsync();
            return gdProgrammeList;
        }
        #endregion
    }
}
