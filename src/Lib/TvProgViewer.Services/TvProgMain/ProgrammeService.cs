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

namespace TvProgViewer.Services.TvProgMain
{
    /// <summary>
    /// Обеспечивает выборку телепрограммы
    /// </summary>
    public class ProgrammeService: IProgrammeService
    {
        #region Поля

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
            long Gid;
            return await systemProgramme.Where(x => genres.Split(new[] { ';' }, StringSplitOptions.RemoveEmptyEntries)
                                .Any(g => long.TryParse(g, out Gid) && long.Parse(g) == x.GenreId)).ToListAsync();
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
            long Cid;
            return await systemProgramme.Where(x => channels.Split(new char[] { ';' }, StringSplitOptions.RemoveEmptyEntries)
                                    .Any(c => long.TryParse(c, out Cid) && long.Parse(c) == x.Cid)).ToListAsync();
        }


        private async Task<List<SystemProgramme>> FilterExcludeDates (List<SystemProgramme> systemProgramme, List<DateTime> rawDates)
        {
            DateTime minDateTime = rawDates.Min();
            DateTime maxDateTime = rawDates.Max().AddDays(1);
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
                    select g.GenreName).FirstAsync(CancellationToken.None);
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
        /// Выборка телепрограммы
        /// </summary>
        /// <param name="TypeProgId">Тип программы телепередач</param>
        /// <param name="dateTimeOffset">Время</param>
        /// <param name="mode">режимы выборки: 1 - сейчас; 2 - следом</param>
        public virtual async Task<KeyValuePair<int, List<SystemProgramme>>> GetSystemProgrammesAsync(int TypeProgId, DateTimeOffset dateTimeOffset, int mode, string category,
                                                         string sidx, string sord, int page, int rows, string genres, string channels)
        {
            List<SystemProgramme> systemProgramme = new List<SystemProgramme>();
            DateTime minDate = new DateTime(1800, 1, 1);
            DateTime dateTime = dateTimeOffset.DateTime;
            int count = 0;
            var intListChannels = string.IsNullOrWhiteSpace(channels) ? new List<int>() : await channels.Split(';', StringSplitOptions.RemoveEmptyEntries).Select(int.Parse).ToListAsync();
            switch (mode)
            {
                case 1:

                    var sp = await (from pr in _programmesRepository.Table
                                    where pr.TypeProgId == TypeProgId && pr.TsStartMo <= dateTime &&
                                    dateTime < pr.TsStopMo && pr.Category != "Для взрослых" &&
                                    !pr.Title.Contains("(18+)") &&
                                    (category == null || pr.Category == category) &&
                                    ((intListChannels.Any() && intListChannels.Contains(pr.ChannelId)) || intListChannels.Count == 0)
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
                                        AnonsContent = ((pr.Descr != null && pr.Descr != string.Empty) ?
                                           _mediaPicRepository.Table.FirstOrDefault(mp => mp.FileName == "GreenAnons.png").Path25 +
                                           _mediaPicRepository.Table.FirstOrDefault(mp => mp.FileName == "GreenAnons.png").FileName :
                                           null),
                                        Category = pr.Category,
                                        Remain = (int?)(Sql.DateDiff(Sql.DateParts.Second, pr.TsStopMo, dateTime) * 1.0 / (Sql.DateDiff(Sql.DateParts.Second, pr.TsStopMo, pr.TsStartMo) * 1.0) * 100.0),
                                        OrderCol = _channelsRepository.Table.FirstOrDefault(ch => ch.Id == pr.ChannelId).SysOrderCol
                                    }).OrderBy(o => o.OrderCol).ThenBy(o => o.InternalChanId).ToListAsync();
                    SetGenres(sp, null);
                    sp = await ChannelIconJoinAsync(sp, TypeProgId, intListChannels);
                    sp = await FilterGenresAsync(sp, genres);
                    sp = await FilterChannelsAsync(sp, channels);
                    count = sp.Count();
                    systemProgramme = await sp.AsQueryable().OrderBy(!(sidx == null || sidx.Trim() == string.Empty) ? sidx : "OrderCol", sord)
                                        .Skip((page - 1) * rows).Take(rows)
                                        .ToListAsync<SystemProgramme>();
                    break;
                case 2:
                    if (dateTime == minDate)
                    {
                        List<TsStopForRemain> stopAfter = await (from pr2 in _programmesRepository.Table
                                                                 where pr2.TsStartMo <= DateTime.Now && DateTime.Now < pr2.TsStopMo && pr2.TypeProgId == TypeProgId
                                                                   && pr2.Category != "Для взрослых" && !pr2.Title.Contains("(18+)")
                                                                   && ((intListChannels.Any() && intListChannels.Contains(pr2.ChannelId)) || intListChannels.Count == 0)
                                                                 select new TsStopForRemain
                                                                 {
                                                                     TsStopMoAfter = pr2.TsStopMo.AddSeconds(1),
                                                                     Cid = pr2.ChannelId
                                                                 }).ToListAsync();
                        DateTime afterTwoDays = DateTime.Now.AddDays(2);
                        var sp2 = await (from pr3 in _programmesRepository.Table
                                         where pr3.TypeProgId == TypeProgId
                                         && pr3.TsStartMo >= DateTime.Now && afterTwoDays > pr3.TsStopMo
                                         && pr3.Category != "Для взрослых" && !pr3.Title.Contains("(18+)")
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
                                             AnonsContent = ((pr3.Descr != null && pr3.Descr != string.Empty) ?
                                                _mediaPicRepository.Table.FirstOrDefault(mp => mp.FileName == "GreenAnons.png").Path25 +
                                                _mediaPicRepository.Table.FirstOrDefault(mp => mp.FileName == "GreenAnons.png").FileName :
                                            null),
                                             Category = pr3.Category,
                                             Remain = Sql.DateDiff(Sql.DateParts.Second, DateTime.Now, pr3.TsStartMo),
                                             OrderCol = _channelsRepository.Table.FirstOrDefault(ch => ch.Id == pr3.ChannelId).SysOrderCol
                                         }).ToListAsync();

                        sp2 = await ChannelIconJoinAsync(sp2, TypeProgId, intListChannels);

                        var sp3 = await (from pr in sp2.ToList()
                                         join prin in stopAfter on pr.Cid equals prin.Cid
                                         where pr.TsStartMo <= prin.TsStopMoAfter && prin.TsStopMoAfter < pr.TsStopMo
                                         select new SystemProgramme()
                                         {
                                             ProgrammesId = pr.ProgrammesId,
                                             Cid = pr.Cid,
                                             ChannelName = pr.ChannelName,
                                             ChannelContent = pr.ChannelContent,
                                             InternalChanId = pr.InternalChanId,
                                             Start = pr.Start,
                                             Stop = pr.Stop,
                                             TsStartMo = pr.TsStartMo,
                                             TsStopMo = pr.TsStopMo,
                                             TelecastTitle = pr.TelecastTitle,
                                             TelecastDescr = pr.TelecastDescr,
                                             AnonsContent = pr.AnonsContent,
                                             Category = pr.Category,
                                             Remain = pr.Remain,
                                             OrderCol = pr.OrderCol
                                         }).OrderBy(o => o.OrderCol).ThenBy(o => o.OrderCol).ToListAsync();
                        SetGenres(sp3, null);
                        sp3 = await FilterGenresAsync(sp3, genres);
                        sp3 = await FilterChannelsAsync(sp3, channels);
                        count = sp3.Count();

                        systemProgramme = await sp3.AsQueryable().OrderBy(!(sidx == null || sidx.Trim() == string.Empty) ? sidx : "OrderCol", sord)
                                        .Skip((page - 1) * rows).Take(rows)
                                        .ToListAsync();
                    }
                    else if (dateTime > minDate)
                    {
                        var sp4 = await (from pr in _programmesRepository.Table
                                         where pr.TypeProgId == TypeProgId && DateTime.Now < pr.TsStartMo && pr.TsStartMo <= dateTime
                                         && pr.Category != "Для взрослых" && !pr.Title.Contains("(18+)")
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
                                             AnonsContent = (pr.Descr != null && pr.Descr != string.Empty) ?
                                                _mediaPicRepository.Table.FirstOrDefault(mp => mp.FileName == "GreenAnons.png").Path25 +
                                                _mediaPicRepository.Table.FirstOrDefault(mp => mp.FileName == "GreenAnons.png").FileName :
                                             null,
                                             Category = pr.Category,
                                             Remain = Sql.DateDiff(Sql.DateParts.Second, DateTime.Now, pr.TsStartMo),
                                             OrderCol = _channelsRepository.Table.FirstOrDefault(ch => ch.Id == pr.ChannelId).SysOrderCol
                                         }).ToListAsync();
                        sp4 = await ChannelIconJoinAsync(sp4, TypeProgId, intListChannels);
                        SetGenres(sp4, null);
                        sp4 = await FilterGenresAsync(sp4, genres);
                        sp4 = await FilterChannelsAsync(sp4, channels);
                        count = sp4.Count();

                        systemProgramme = await sp4.AsQueryable().OrderBy(!(sidx == null || sidx.Trim() == string.Empty) ? sidx : "OrderCol", sord)
                                        .Skip((page - 1) * rows).Take(rows)
                                        .ToListAsync<SystemProgramme>();
                    }
                    break;
            }
            return new KeyValuePair<int, List<SystemProgramme>>(count, systemProgramme);
        }

        /// <summary>
        /// Поиск телепередачи
        /// </summary>
        /// <param name="TypeProgId">Тип программы телепередач</param>
        /// <param name="findTitle">Поисковая подстрока</param>
        public virtual async Task<KeyValuePair<int, List<SystemProgramme>>> SearchProgrammeAsync(int typeProgId, string findTitle, string category
                                                               , string sidx, string sord, int page, int rows, string genres, string dates, string channels)

        {
            int count = 0;
            var intListChannels = string.IsNullOrWhiteSpace(channels) ? new List<int>() : await channels.Split(';', StringSplitOptions.RemoveEmptyEntries).Select(int.Parse).ToListAsync();
            if (string.IsNullOrWhiteSpace(dates))
                return new KeyValuePair<int, List<SystemProgramme>>();
            DateTime date;
            CultureInfo ci = new CultureInfo("Ru-ru");
            var rawDates = dates.Split(new[] { ';' }, StringSplitOptions.RemoveEmptyEntries).Select(x => DateTime.TryParseExact(x, "yyyyMMdd", ci, DateTimeStyles.None, out date) ? DateTime.ParseExact(x, "yyyyMMdd", ci) : new DateTime()).ToList();
            DateTime minDateTime = rawDates.Min();
            DateTime maxDateTime = rawDates.Max().AddDays(1);

            var systemProgramme = await (from pr in _programmesRepository.Table
                                         where pr.TypeProgId == typeProgId
                                    && pr.Category != "Для взрослых" && !pr.Title.Contains("(18+)")
                                    && (category == null || pr.Category == category)
                                    && pr.Title.Contains(findTitle)
                                    && minDateTime <= pr.TsStartMo && pr.TsStartMo <= maxDateTime
                                    && ((intListChannels.Any() && intListChannels.Contains(pr.ChannelId)) || intListChannels.Count == 0)
                                         orderby pr.TsStartMo, pr.TsStopMo
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
                                             AnonsContent = ((pr.Descr != null && pr.Descr != string.Empty) ?
                                                _mediaPicRepository.Table.FirstOrDefault(mp => mp.FileName == "GreenAnons.png").Path25 +
                                                _mediaPicRepository.Table.FirstOrDefault(mp => mp.FileName == "GreenAnons.png").FileName : null),
                                             Category = pr.Category,
                                             Remain = 1
                                         }).ToListAsync();
            systemProgramme = await ChannelIconJoinAsync(systemProgramme, typeProgId, intListChannels);
            Parallel.ForEach(systemProgramme, pr =>
            {
                pr.DayMonth = pr.TsStartMo.ToString("ddd", new CultureInfo("ru-Ru")) +
           String.Format("({0:D2}.{1:D2})", pr.TsStartMo.Day, pr.TsStartMo.Month);
            });
            SetGenres(systemProgramme, null);
            systemProgramme = await FilterGenresAsync(systemProgramme, genres);
            systemProgramme = await FilterChannelsAsync(systemProgramme, channels);
            systemProgramme = await FilterExcludeDates(systemProgramme, rawDates);
            count = systemProgramme.Count();

            systemProgramme = await systemProgramme.AsQueryable().OrderBy(!(sidx == null || sidx.Trim() == string.Empty) ? sidx : "TsStartMo", sord)
                                         .Skip((page - 1) * rows).Take(rows)
                                         .ToListAsync<SystemProgramme>();
            return new KeyValuePair<int, List<SystemProgramme>>(count, systemProgramme);
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
        public virtual async Task<List<SystemProgramme>> GetUserProgrammesOfDayListAsync(long? uid, int typeProgId, int channelId, DateTime tsStart, DateTime tsEnd, string category)
        {
            List<SystemProgramme> systemProgrammes = new List<SystemProgramme>();
            systemProgrammes = await (from pr in _programmesRepository.Table
                                      join ch in _channelsRepository.Table on pr.ChannelId equals ch.Id
                                      join mp in _mediaPicRepository.Table on ch.IconId equals mp.Id into chmp
                                      from mp in chmp.DefaultIfEmpty()
                                      where pr.TypeProgId == typeProgId &&
                                      ch.Id == channelId &&
                                      pr.TsStartMo >= tsStart &&
                                      pr.TsStopMo <= tsEnd &&
                                      pr.Category != "Для взрослых" && !pr.Title.Contains("(18+)")
                                      && ch.Deleted == null
                                      select new SystemProgramme
                                      {
                                          ProgrammesId = pr.Id,
                                          Cid = pr.ChannelId,
                                          ChannelName = ch.TitleChannel,
                                          InternalChanId = pr.InternalChanId,
                                          Start = pr.TsStart,
                                          Stop = pr.TsStop,
                                          TsStartMo = pr.TsStartMo,
                                          TsStopMo = pr.TsStopMo,
                                          TelecastTitle = pr.Title,
                                          TelecastDescr = pr.Descr,
                                          AnonsContent = ((pr.Descr != null && pr.Descr != string.Empty) ?
                                          _mediaPicRepository.Table.FirstOrDefault(mp => mp.FileName == "GreenAnons.png").Path25 +
                                          _mediaPicRepository.Table.FirstOrDefault(mp => mp.FileName == "GreenAnons.png").FileName :
                                                     null),
                                          Category = pr.Category,
                                      }).ToListAsync<SystemProgramme>();
            SetGenres(systemProgrammes, null);
            return systemProgrammes;
        }

        #endregion
    }
}
