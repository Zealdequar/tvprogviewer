using LinqToDB;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Linq.Expressions;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using DynamicExpression.Entities;
using DynamicExpression.Extensions;
using TvProgViewer.Core.Caching;
using TvProgViewer.Core.Domain.TvProgMain;
using TvProgViewer.Core.Domain.Users;
using TvProgViewer.Data;
using TvProgViewer.Data.TvProgMain;
using TvProgViewer.Data.TvProgMain.ProgObjs;

namespace TvProgViewer.Services.TvProgMain
{
    /// <summary>
    /// Обеспечивает выборку телепрограммы
    /// </summary>
    public partial class ProgrammeService : IProgrammeService
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
        private const string CATEGORY_CHILDREN = "Детям";
        private const string CATEGORY_MOVIE = "Художественный фильм";
        private const string CATEGORY_SERIES = "Сериал";
        private const string CATEGORY_SPORT = "Спорт";
        private const string CATEGORY_ADULT = "Для взрослых";
        private const string CATEGORY_NONE = "Без категории";
        private const string NO_GENRE_NAME = "Без типа";
        private const string NO_RATING_NAME = "Без рейтинга";
        private const string XMLTV_TYPE_NAME = "Формат XMLTV";

        private static readonly char[] separator = new[] { ';' };

        /// <summary>
        /// Русская культура для форматирования дат
        /// </summary>
        private static readonly CultureInfo RussianCulture = CultureInfo.GetCultureInfo("ru-RU");

        /// <summary>
        /// Предопределённые системные категории
        /// </summary>
        private static readonly HashSet<string> PredefinedCategories = new(StringComparer.Ordinal)
        {
            CATEGORY_CHILDREN,
            CATEGORY_MOVIE,
            CATEGORY_SERIES,
            CATEGORY_SPORT,
            CATEGORY_ADULT,
            CATEGORY_NONE
        };

        /// <summary>
        /// Соответствие предопределённых категорий их системным идентификаторам
        /// </summary>
        private static readonly IReadOnlyDictionary<string, string> PredefinedCategoryIdents =
            new Dictionary<string, string>(StringComparer.Ordinal)
            {
                [CATEGORY_CHILDREN] = TvProgUserDefaults.GetChildTvProgCategory,
                [CATEGORY_MOVIE] = TvProgUserDefaults.GetFeatureFilmCategory,
                [CATEGORY_SERIES] = TvProgUserDefaults.GetSerialCategory,
                [CATEGORY_SPORT] = TvProgUserDefaults.GetSportCategory,
                [CATEGORY_NONE] = TvProgUserDefaults.GetNonCategory
            };

        /// <summary>
        /// Шаблон возрастной метки "(X+)" в названии передачи
        /// </summary>
        private static readonly Regex AgeFromTitleRegex = new(@"\((\d+)\+\)", RegexOptions.IgnoreCase | RegexOptions.Compiled);

        private readonly IRepository<TvProgProviders> _providerTypeRepository;
        private readonly IRepository<TypeProg> _typeProgRepository;
        private readonly IRepository<Programmes> _programmesRepository;
        private readonly IRepository<Channels> _channelsRepository;
        private readonly IRepository<RatingClassificator> _ratingClassificatorRepository;
        private readonly IRepository<Ratings> _ratingsRepository;
        private readonly IRepository<MediaPic> _mediaPicRepository;
        private readonly IRepository<GenreClassificator> _genreClassificatorRepository;
        private readonly IRepository<Genres> _genresRepository;
        private readonly IRepository<TvProgCategory> _tvProgCategoryRepository;
        private readonly IRepository<ProgrammesTvProgCategoryMapping> _programmesTvProgCategoryMappingRepository;
        private readonly IStaticCacheManager _cacheManager;

        /// <summary>
        /// Московская часовая зона (кроссплатформенный идентификатор)
        /// </summary>
        private static readonly TimeZoneInfo MoscowTz =
            TimeZoneInfo.FindSystemTimeZoneById(
                RuntimeInformation.IsOSPlatform(OSPlatform.Windows) ?
                "Russian Standard Time" : "Europe/Moscow");

        #endregion

        #region Конструктор

        public ProgrammeService(IRepository<TvProgProviders> providerTypeRepository,
                         IRepository<TypeProg> typeProgRepository,
                         IRepository<Programmes> programmesRepository,
                         IRepository<Channels> channelsRepository,
                         IRepository<RatingClassificator> ratingClassificatorRepository,
                         IRepository<Ratings> ratingsRepository,
                         IRepository<MediaPic> mediaPicRepository,
                         IRepository<GenreClassificator> genreClassificatorRepository,
                         IRepository<Genres> genresRepository,
                         IRepository<TvProgCategory> tvProgCategoryRepository,
                         IRepository<ProgrammesTvProgCategoryMapping> programmesTvProgCategoryMappingRepository,
                         IStaticCacheManager cacheManager)
        {
            _providerTypeRepository = providerTypeRepository;
            _typeProgRepository = typeProgRepository;
            _programmesRepository = programmesRepository;
            _channelsRepository = channelsRepository;
            _ratingClassificatorRepository = ratingClassificatorRepository;
            _ratingsRepository = ratingsRepository;
            _mediaPicRepository = mediaPicRepository;
            _genreClassificatorRepository = genreClassificatorRepository;
            _genresRepository = genresRepository;
            _tvProgCategoryRepository = tvProgCategoryRepository;
            _programmesTvProgCategoryMappingRepository = programmesTvProgCategoryMappingRepository;
            _cacheManager = cacheManager;
        }

        #endregion

        #region Провайдеры и типы программ

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
            return await _providerTypeRepository.GetAllAsync(query => query,
                cache => cache.PrepareKeyForDefaultCache(TvProgProgrammeDefaults.ProvidersAllCacheKey));
        }

        /// <summary>
        /// Получение типа программы по его идентификатору
        /// </summary>
        /// <param name="typeProgId">Идентификатор типа программы</param>
        public virtual async Task<TypeProg> GetTypeProgByIdAsync(int typeProgId)
        {
            if (typeProgId == 0)
                return null;

            return await _typeProgRepository.GetByIdAsync(typeProgId);
        }

        /// <summary>
        /// Получение всех типов ТВ-программы
        /// </summary>
        public virtual async Task<IList<TypeProg>> GetAllTypeProgsAsync(bool showHidden = false)
        {
            return await _typeProgRepository.GetAllAsync(query => query,
                cache => cache.PrepareKeyForDefaultCache(TvProgProgrammeDefaults.TypeProgAllCacheKey, showHidden));
        }

        /// <summary>
        /// Получение списка провайдеров телепрограммы вместе с их типами программ
        /// </summary>
        public virtual async Task<IList<ProviderType>> GetProviderTypeListAsync()
        {
            return await (from tpp in _providerTypeRepository.Table
                          join tp in _typeProgRepository.Table on tpp.Id equals tp.TvProgProviderId
                          select new ProviderType
                          {
                              TvProgProviderId = tpp.Id,
                              ProviderName = tpp.ProviderName,
                              ProviderText = tpp.ProviderWebSite,
                              TypeProgId = tp.Id,
                              TypeName = tp.TypeName,
                              TypeEnum = (tp.TypeName == XMLTV_TYPE_NAME) ? Enums.TypeProg.XMLTV : Enums.TypeProg.InterTV,
                              FileFormat = tp.FileFormat
                          }).ToListAsync();
        }

        #endregion

        #region Период и дни программы

        /// <summary>
        /// Получение периода действия программы телепередач (минимальное и максимальное время показа одним запросом)
        /// </summary>
        /// <param name="typeProgId">Идентификатор типа программы</param>
        public virtual async Task<ProgPeriod> GetSystemProgrammePeriodAsync(int typeProgId)
        {
            // Пустая программа возвращает структуру с null-значениями границ
            return await _programmesRepository.Table
                .Where(x => x.TypeProgId == typeProgId)
                .GroupBy(x => x.TypeProgId)
                .Select(g => new ProgPeriod
                {
                    dtStart = g.Min(x => (DateTimeOffset?)x.TsStartMo),
                    dtEnd = g.Max(x => (DateTimeOffset?)x.TsStopMo)
                })
                .FirstOrDefaultAsync(CancellationToken.None);
        }

        /// <summary>
        /// Получение дней доступной программы
        /// </summary>
        /// <param name="typeProgId">Идентификатор типа программы</param>
        /// <returns>Список дней</returns>
        public virtual async Task<List<DaysItem>> GetDaysAsync(int typeProgId)
        {
            var bounds = (await GetSystemProgrammePeriodAsync(typeProgId)).GetPeriodBounds();
            var todayUtc = DateTime.UtcNow.Date;
            var daysList = new List<DaysItem>();
            for (DateTime tsCurDate = bounds.Middle1.AddDays(1); tsCurDate <= bounds.TsMax; tsCurDate = tsCurDate.AddDays(1))
            {
                daysList.Add(new DaysItem
                {
                    Name = tsCurDate.ToString("dd.MM.yyyy", RussianCulture),
                    DayOfWeek = tsCurDate.DayOfWeek.ToString(),
                    IconSrc = $"./images/i/{Extensions.GetDayIconName(tsCurDate.DayOfWeek)}.png",
                    Color = (tsCurDate.Date < todayUtc) ? GRAY_COLOR : (tsCurDate.Date == todayUtc) ? BLACK_COLOR : GREEN_COLOR
                });
            }

            return daysList;
        }

        /// <summary>
        /// Получение категорий телепередач
        /// </summary>
        /// <remarks>
        /// Категории берутся из справочника <see cref="TvProgCategory"/> (включая предопределённые
        /// системные категории). Порядок вывода определяется полем <see cref="TvProgCategory.Order"/>.
        /// Список запрашивается практически на каждой странице (компонент выбора категории),
        /// поэтому результат кешируется; сброс выполняется потребителем событий изменения категорий
        /// </remarks>
        public virtual async Task<IList<string>> GetCategoriesAsync()
        {
            // Защитная копия: вызывающий код (например, SetCategory) мутирует список,
            // а закешированный экземпляр должен оставаться неизменным
            var cachedCategories = await _cacheManager.GetAsync(TvProgProgrammeDefaults.CategoriesAllCacheKey, async () =>
            {
                IList<string> categories = await _tvProgCategoryRepository.Table
                    .Where(c => c.Active)
                    .Select(c => new { c.Name, Order = c.Order ?? int.MaxValue })
                    .Where(x => !string.IsNullOrEmpty(x.Name))
                    .Distinct()
                    .OrderBy(x => x.Order)
                    .ThenBy(x => x.Name)
                    .Select(x => x.Name)
                    .ToListAsync<string>();

                return categories;
            });

            return cachedCategories?.ToList() ?? [];
        }

        #endregion

        #region Выборка телепрограммы (сейчас / следом)

        /// <summary>
        /// Выборка телепрограммы
        /// </summary>
        /// <param name="typeProgId">Тип программы телепередач</param>
        /// <param name="dateTimeOffset">Время</param>
        /// <param name="mode">Режимы выборки: 1 - сейчас; 2 - следом</param>
        public virtual Task<KeyValuePair<int, List<SystemProgramme>>> GetSystemProgrammesAsync(int typeProgId, DateTimeOffset dateTimeOffset, int mode, string category,
                                                         string sidx, string sord, int page, int rows, string genres, string channels, bool isAuthorized)
        {
            return GetProgrammesCoreAsync(typeProgId, dateTimeOffset, mode, category,
                sidx, sord, page, rows, genres, channels, isAuthorized, includeAdult: false);
        }

        /// <summary>
        /// Выборка телепрограммы для совершеннолетних
        /// </summary>
        /// <param name="typeProgId">Тип программы телепередач</param>
        /// <param name="dateTimeOffset">Время</param>
        /// <param name="mode">Режимы выборки: 1 - сейчас; 2 - следом</param>
        public virtual Task<KeyValuePair<int, List<SystemProgramme>>> GetUserAdultProgrammesAsync(int typeProgId, DateTimeOffset dateTimeOffset, int mode, string category,
                                                         string sidx, string sord, int page, int rows, string genres, string channels, bool isAuthorized)
        {
            return GetProgrammesCoreAsync(typeProgId, dateTimeOffset, mode, category,
                sidx, sord, page, rows, genres, channels, isAuthorized, includeAdult: true);
        }

        /// <summary>
        /// Общая выборка телепрограммы для обычных пользователей и совершеннолетних
        /// </summary>
        /// <param name="includeAdult">Включать ли передачи для взрослых</param>
        private async Task<KeyValuePair<int, List<SystemProgramme>>> GetProgrammesCoreAsync(int typeProgId, DateTimeOffset dateTimeOffset, int mode, string category,
                                                         string sidx, string sord, int page, int rows, string genres, string channels, bool isAuthorized, bool includeAdult)
        {
            var spRows = new List<SystemProgramme>();
            DateTime minDate = new(1800, 1, 1);
            DateTime dateTime = dateTimeOffset.DateTime;
            int spCount = 0;
            var channelIds = ParseChannelIds(channels);

            // Путь к изображению анонса получается один раз (кешируется)
            var anonsImagePath = await GetAnonsImagePathAsync();

            switch (mode)
            {
                case 1:
                    dateTime = dateTime.ToUniversalTime();
                    var baseQuery = GetFilteredProgrammesQuery(typeProgId, category, channelIds, includeAdult);

                    spCount = await baseQuery
                        .Where(pr => pr.TsStartMo <= dateTime && dateTime < pr.TsStopMo)
                        .Select(pr => pr.Id)
                        .CountAsync(CancellationToken.None);

                    sidx = !string.IsNullOrWhiteSpace(sidx) ? sidx : ORDER_COL;

                    // Порядковый номер канала берётся левым соединением вместо подзапроса на каждую строку
                    spRows = await (from pr in baseQuery
                                    where pr.TsStartMo <= dateTime && dateTime < pr.TsStopMo
                                    join ch in _channelsRepository.Table on pr.ChannelId equals ch.Id into prch
                                    from ch in prch.DefaultIfEmpty()
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
                                        AnonsContent = !string.IsNullOrWhiteSpace(pr.Descr) ? anonsImagePath : null,
                                        Category = pr.Category,
                                        Remain = CalculateRemainPercentage(pr.TsStartMo, pr.TsStopMo, dateTime),
                                        OrderCol = ch.SysOrderCol
                                    }).LimitAndOrderBy(page, rows, sidx, sord).ToListAsync();

                    await ApplyPostFiltersAsync(spRows, typeProgId, category, isAuthorized, channelIds, genres, channels);
                    break;
                case 2:
                    if (dateTime == minDate)
                    {
                        var nowUtc = DateTime.UtcNow;
                        var afterTwoDays = nowUtc.AddDays(2);

                        // Текущие передачи: конец каждой из них определяет точку отсчёта "следующей" передачи канала
                        var nextPointsByChannel = (await (from pr2 in GetFilteredProgrammesQuery(typeProgId, null, channelIds, includeAdult)
                                                          where pr2.TsStartMo <= nowUtc && nowUtc < pr2.TsStopMo
                                                          select new
                                                          {
                                                              Cid = pr2.ChannelId,
                                                              NextPoint = pr2.TsStopMo.AddSeconds(1)
                                                          }).ToListAsync())
                                                  .ToLookup(x => x.Cid, x => x.NextPoint);

                        sidx = !string.IsNullOrWhiteSpace(sidx) ? sidx : ORDER_COL;

                        // Данные выбираются без преобразования времени; порядковый номер канала берётся левым соединением вместо подзапроса на каждую строку
                        var candidates = await (from pr3 in GetFilteredProgrammesQuery(typeProgId, category, channelIds, includeAdult)
                                                where pr3.TsStartMo >= nowUtc && afterTwoDays > pr3.TsStopMo
                                                join ch in _channelsRepository.Table on pr3.ChannelId equals ch.Id into prch
                                                from ch in prch.DefaultIfEmpty()
                                                orderby pr3.InternalChanId, pr3.TsStartMo
                                                select new
                                                {
                                                    pr3.Id,
                                                    Cid = pr3.ChannelId,
                                                    pr3.InternalChanId,
                                                    pr3.TsStart,
                                                    pr3.TsStop,
                                                    pr3.TsStartMo,
                                                    pr3.TsStopMo,
                                                    pr3.Title,
                                                    pr3.Descr,
                                                    pr3.Category,
                                                    OrderCol = ch.SysOrderCol
                                                }).ToListAsync();

                        // Передачи, идущие следом за текущими по каждому каналу (поиск по каналу за O(1))
                        var following = candidates
                            .Where(pr => nextPointsByChannel[pr.Cid]
                                .Any(nextPoint => pr.TsStartMo <= nextPoint && nextPoint < pr.TsStopMo))
                            .ToList();

                        spCount = following.Count;

                        // Сортировка и постраничное ограничение выполняются до преобразования времени (конвертируется только страница)
                        spRows = await following.AsQueryable()
                            .LimitAndOrderBy(page, rows, sidx, sord)
                            .Select(pr => new SystemProgramme
                            {
                                ProgrammesId = pr.Id,
                                Cid = pr.Cid,
                                InternalChanId = pr.InternalChanId,
                                Start = TimeZoneInfo.ConvertTimeFromUtc(pr.TsStart, MoscowTz),
                                Stop = TimeZoneInfo.ConvertTimeFromUtc(pr.TsStop, MoscowTz),
                                TsStartMo = TimeZoneInfo.ConvertTimeFromUtc(pr.TsStartMo, MoscowTz),
                                TsStopMo = TimeZoneInfo.ConvertTimeFromUtc(pr.TsStopMo, MoscowTz),
                                TelecastTitle = pr.Title,
                                TelecastDescr = pr.Descr,
                                AnonsContent = !string.IsNullOrEmpty(pr.Descr) ? anonsImagePath : null,
                                Category = pr.Category,
                                Remain = (int)(pr.TsStartMo - nowUtc).TotalSeconds,
                                OrderCol = pr.OrderCol
                            })
                            .ToListAsync();

                        await ApplyPostFiltersAsync(spRows, typeProgId, category, isAuthorized, channelIds, genres, channels);
                    }
                    else if (dateTime > minDate)
                    {
                        var nowUtc = DateTime.UtcNow;
                        var nextQuery = GetFilteredProgrammesQuery(typeProgId, category, channelIds, includeAdult)
                            .Where(pr => nowUtc < pr.TsStartMo && pr.TsStartMo <= dateTime);

                        spCount = await nextQuery.Select(pr => pr.Id).CountAsync(CancellationToken.None);
                        sidx = !string.IsNullOrWhiteSpace(sidx) ? sidx : ORDER_COL;

                        spRows = await (from pr in nextQuery
                                        join ch in _channelsRepository.Table on pr.ChannelId equals ch.Id into prch
                                        from ch in prch.DefaultIfEmpty()
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
                                            AnonsContent = !string.IsNullOrEmpty(pr.Descr) ? anonsImagePath : null,
                                            Category = pr.Category,
                                            Remain = (int)(pr.TsStartMo - DateTime.UtcNow).TotalSeconds,
                                            OrderCol = ch.SysOrderCol
                                        }).LimitAndOrderBy(page, rows, sidx, sord).ToListAsync();

                        await ApplyPostFiltersAsync(spRows, typeProgId, category, isAuthorized, channelIds, genres, channels);
                    }
                    break;
            }

            return new KeyValuePair<int, List<SystemProgramme>>(spCount, spRows);
        }

        #endregion

        #region Поиск телепередач

        /// <summary>
        /// Поиск телепередачи
        /// </summary>
        /// <param name="typeProgId">Тип программы телепередач</param>
        /// <param name="findTitle">Поисковая подстрока</param>
        public virtual Task<KeyValuePair<int, List<SystemProgramme>>> SearchProgrammeAsync(int typeProgId, string findTitle, string category,
                                                         string sidx, string sord, int page, int rows, string genres, string dates, string channels, bool isAuthorized)
        {
            return SearchProgrammesCoreAsync(typeProgId, findTitle, category,
                sidx, sord, page, rows, genres, dates, channels, isAuthorized, includeAdult: false);
        }

        /// <summary>
        /// Поиск телепередачи для совершеннолетних
        /// </summary>
        /// <param name="typeProgId">Тип программы телепередач</param>
        /// <param name="findTitle">Поисковая подстрока</param>
        public virtual Task<KeyValuePair<int, List<SystemProgramme>>> SearchAdultProgrammeAsync(int typeProgId, string findTitle, string category,
                                                         string sidx, string sord, int page, int rows, string genres, string dates, string channels, bool isAuthorized)
        {
            return SearchProgrammesCoreAsync(typeProgId, findTitle, category,
                sidx, sord, page, rows, genres, dates, channels, isAuthorized, includeAdult: true);
        }

        /// <summary>
        /// Общий поиск телепередач по датам для обычных пользователей и совершеннолетних
        /// </summary>
        /// <param name="includeAdult">Включать ли передачи для взрослых</param>
        private async Task<KeyValuePair<int, List<SystemProgramme>>> SearchProgrammesCoreAsync(int typeProgId, string findTitle, string category,
                                                         string sidx, string sord, int page, int rows, string genres, string dates, string channels, bool isAuthorized, bool includeAdult)
        {
            if (string.IsNullOrWhiteSpace(dates))
                return new KeyValuePair<int, List<SystemProgramme>>();

            // Без поисковой подстроки поиск имеет смысл только при явном выборе жанров
            if (string.IsNullOrWhiteSpace(findTitle) && string.IsNullOrWhiteSpace(genres))
                return new KeyValuePair<int, List<SystemProgramme>>();

            if (string.IsNullOrWhiteSpace(findTitle))
            {
                // Если выбранные жанры совпадают с системными - фильтрация не даст результата
                var repoGenreIds = (await _genresRepository.Table
                    .Where(x => x.Visible && x.UserId == null)
                    .Select(x => (long)x.Id)
                    .ToListAsync(CancellationToken.None))
                    .ToHashSet();
                if (repoGenreIds.SetEquals(ParseLongIds(genres)))
                    return new KeyValuePair<int, List<SystemProgramme>>();
            }

            var rawDates = dates.Split(separator, StringSplitOptions.RemoveEmptyEntries)
                .Select(x => DateTime.TryParseExact(x, "yyyyMMdd", RussianCulture, DateTimeStyles.None, out var date) ? date : default)
                .ToList();
            var minDateTime = rawDates.Min();
            var maxDateTime = rawDates.Max().AddDays(1);

            // Путь к изображению анонса получается один раз (кешируется)
            var anonsImagePath = await GetAnonsImagePathAsync();

            var channelIds = ParseChannelIds(channels);
            var systemProgramme = await (from pr in GetFilteredProgrammesQuery(typeProgId, category, channelIds, includeAdult)
                                         where (string.IsNullOrWhiteSpace(findTitle) || pr.Title.Contains(findTitle))
                                           && minDateTime.ToUniversalTime() <= pr.TsStartMo && pr.TsStartMo <= maxDateTime.ToUniversalTime()
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
                                             AnonsContent = !string.IsNullOrEmpty(pr.Descr) ? anonsImagePath : null,
                                             Category = pr.Category,
                                             Remain = 1
                                         }).ToListAsync();

            await JoinChannelIcons(systemProgramme, typeProgId, channelIds);
            SetDayMonth(systemProgramme);
            systemProgramme = FilterByIds(systemProgramme, channels, x => x.Cid);
            systemProgramme = FilterByDates(systemProgramme, rawDates);
            SetGenres(systemProgramme);

            // Пост-фильтрация по предопределённым категориям
            if (IsPredefinedCategory(category))
                ReplaceList(systemProgramme, systemProgramme.Where(x => MatchesPredefinedCategory(x.TelecastTitle, x.GenreName, category, isAuthorized)));
            ReplaceList(systemProgramme, FilterByIds(systemProgramme, genres, x => x.GenreId.GetValueOrDefault()));

            int spCount = systemProgramme.Count;
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
        public virtual Task<KeyValuePair<int, List<SystemProgramme>>> SearchGlobalProgrammeAsync(int typeProgId, string findTitle, string category,
                                                         string sidx, string sord, int page, int rows, string genres, string channels, bool isAuthorized)
        {
            return SearchGlobalProgrammesCoreAsync(typeProgId, findTitle, category,
                sidx, sord, page, rows, genres, channels, isAuthorized, includeAdult: false);
        }

        /// <summary>
        /// Глобальный поиск телепередачи для совершеннолетних
        /// </summary>
        /// <param name="typeProgId">Тип программы телепередач</param>
        /// <param name="findTitle">Поисковая подстрока</param>
        /// <param name="category">Категория</param>
        /// <param name="genres">Жанры</param>
        /// <param name="channels">Каналы</param>
        public virtual Task<KeyValuePair<int, List<SystemProgramme>>> SearchAdultGlobalProgrammeAsync(int typeProgId, string findTitle, string category,
                                                         string sidx, string sord, int page, int rows, string genres, string channels, bool isAuthorized)
        {
            return SearchGlobalProgrammesCoreAsync(typeProgId, findTitle, category,
                sidx, sord, page, rows, genres, channels, isAuthorized, includeAdult: true);
        }

        /// <summary>
        /// Общий глобальный поиск для обычных пользователей и совершеннолетних
        /// </summary>
        /// <param name="includeAdult">Включать ли передачи для взрослых</param>
        private async Task<KeyValuePair<int, List<SystemProgramme>>> SearchGlobalProgrammesCoreAsync(int typeProgId, string findTitle, string category,
                                                         string sidx, string sord, int page, int rows, string genres, string channels, bool isAuthorized, bool includeAdult)
        {
            var channelIds = ParseChannelIds(channels);
            var baseQuery = GetFilteredProgrammesQuery(typeProgId, category, channelIds, includeAdult)
                .Where(pr => pr.Title.Contains(findTitle));

            int spCount = await baseQuery.Select(pr => pr.Id).CountAsync(CancellationToken.None);
            sidx = !string.IsNullOrWhiteSpace(sidx) ? sidx : TS_START_MO;

            // Сначала получаем данные из БД без преобразования времени
            var programmes = await (from pr in baseQuery
                                    orderby pr.TsStartMo, pr.TsStopMo
                                    select new
                                    {
                                        pr.Id,
                                        pr.ChannelId,
                                        pr.InternalChanId,
                                        pr.TsStart,
                                        pr.TsStop,
                                        pr.TsStartMo,
                                        pr.TsStopMo,
                                        pr.Title,
                                        pr.Descr,
                                        pr.Category
                                    }).AsQueryable()
                                      .LimitAndOrderBy(page, rows, sidx, sord)
                                      .ToListAsync();

            // Путь к изображению анонса получается один раз (кешируется)
            var anonsImagePath = await GetAnonsImagePathAsync();

            // Затем преобразуем время в памяти
            var systemProgramme = programmes.Select(x => new SystemProgramme
            {
                ProgrammesId = x.Id,
                Cid = x.ChannelId,
                InternalChanId = x.InternalChanId ?? 0,
                Start = TimeZoneInfo.ConvertTimeFromUtc(x.TsStart, MoscowTz),
                Stop = TimeZoneInfo.ConvertTimeFromUtc(x.TsStop, MoscowTz),
                TsStartMo = TimeZoneInfo.ConvertTimeFromUtc(x.TsStartMo, MoscowTz),
                TsStopMo = TimeZoneInfo.ConvertTimeFromUtc(x.TsStopMo, MoscowTz),
                TelecastTitle = x.Title,
                TelecastDescr = x.Descr,
                AnonsContent = !string.IsNullOrEmpty(x.Descr) ? anonsImagePath : null,
                Category = x.Category,
                Remain = 1
            }).ToList();

            await JoinChannelIcons(systemProgramme, typeProgId, channelIds);
            SetDayMonth(systemProgramme);
            SetGenres(systemProgramme);

            // Пост-фильтрация по предопределённым категориям
            if (IsPredefinedCategory(category))
                ReplaceList(systemProgramme, systemProgramme.Where(x => MatchesPredefinedCategory(x.TelecastTitle, x.GenreName, category, isAuthorized)));
            systemProgramme = FilterByIds(systemProgramme, genres, x => x.GenreId.GetValueOrDefault());
            systemProgramme = FilterByIds(systemProgramme, channels, x => x.Cid);

            return new KeyValuePair<int, List<SystemProgramme>>(spCount, systemProgramme);
        }

        #endregion

        #region Телепередачи за день

        /// <summary>
        /// Получение пользовательских телепередач за день
        /// </summary>
        /// <param name="typeProgId">Идентификатор типа программы телепередач</param>
        /// <param name="channelId">Код канала</param>
        /// <param name="tsStart">Время начала выборки</param>
        /// <param name="tsEnd">Время завершения выборки</param>
        /// <param name="category">Категория</param>
        public virtual Task<List<SystemProgramme>> GetUserProgrammesOfDayListAsync(int typeProgId, int channelId, DateTime tsStart, DateTime tsEnd, string category, bool isAuthorized)
        {
            return GetUserProgrammesOfDayCoreAsync(typeProgId, channelId, tsStart, tsEnd, category, isAuthorized, includeAdult: false);
        }

        /// <summary>
        /// Получение пользовательских телепередач за день для совершеннолетних
        /// </summary>
        /// <param name="typeProgId">Идентификатор типа программы телепередач</param>
        /// <param name="channelId">Код канала</param>
        /// <param name="tsStart">Время начала выборки</param>
        /// <param name="tsEnd">Время завершения выборки</param>
        /// <param name="category">Категория</param>
        public virtual Task<List<SystemProgramme>> GetUserAdultProgrammesOfDayListAsync(int typeProgId, int channelId, DateTime tsStart, DateTime tsEnd, string category, bool isAuthorized)
        {
            return GetUserProgrammesOfDayCoreAsync(typeProgId, channelId, tsStart, tsEnd, category, isAuthorized, includeAdult: true);
        }

        /// <summary>
        /// Общая выборка телепередач за день для обычных пользователей и совершеннолетних
        /// </summary>
        /// <param name="includeAdult">Включать ли передачи для взрослых</param>
        private async Task<List<SystemProgramme>> GetUserProgrammesOfDayCoreAsync(int typeProgId, int channelId, DateTime tsStart, DateTime tsEnd, string category, bool isAuthorized, bool includeAdult)
        {
            var baseQuery = GetFilteredProgrammesQuery(typeProgId, category, [], includeAdult)
                .Where(pr => pr.TsStartMo >= tsStart.ToUniversalTime() && pr.TsStopMo <= tsEnd.ToUniversalTime());

            // Сначала получаем данные из БД без преобразования времени
            // (фильтр по каналу применён до соединения, неиспользуемое соединение с пиктограммами удалено)
            var programmes = await (from ch in _channelsRepository.Table
                                    join pr in baseQuery on ch.Id equals pr.ChannelId
                                    where ch.Id == channelId && ch.Deleted == null
                                    select new
                                    {
                                        Programme = pr,
                                        Channel = ch,
                                        HasDescription = !string.IsNullOrEmpty(pr.Descr)
                                    })
                                  .OrderBy(x => x.Programme.TsStartMo)
                                  .ToListAsync();

            // Путь к изображению анонса получается один раз (кешируется)
            var anonsImagePath = await GetAnonsImagePathAsync();

            // Затем преобразуем время в памяти
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

            SetGenres(systemProgrammes);

            // Пост-фильтрация по предопределённым категориям
            if (IsPredefinedCategory(category))
                ReplaceList(systemProgrammes, systemProgrammes.Where(x => MatchesPredefinedCategory(x.TelecastTitle, x.GenreName, category, isAuthorized)));

            return systemProgrammes;
        }

        #endregion

        #region Служебные методы

        /// <summary>
        /// Разбирает строку идентификаторов каналов, разделённых точкой с запятой
        /// </summary>
        private static List<int> ParseChannelIds(string channels)
        {
            if (string.IsNullOrWhiteSpace(channels))
                return [];

            return [.. channels.Split(separator, StringSplitOptions.RemoveEmptyEntries)
                .Where(ch => int.TryParse(ch, out _))
                .Select(int.Parse)];
        }

        /// <summary>
        /// Разбирает строку идентификаторов, разделённых точкой с запятой
        /// </summary>
        private static HashSet<long> ParseLongIds(string source)
        {
            if (string.IsNullOrWhiteSpace(source))
                return [];

            return [.. source.Split(separator, StringSplitOptions.RemoveEmptyEntries)
                .Where(item => long.TryParse(item, out _))
                .Select(long.Parse)];
        }

        /// <summary>
        /// Строит базовый запрос телепрограммы с фильтрами по типу, категории, каналам и взрослым передачам
        /// </summary>
        /// <param name="typeProgId">Идентификатор типа программы</param>
        /// <param name="category">Категория</param>
        /// <param name="channelIds">Идентификаторы каналов (пустой список - все каналы)</param>
        /// <param name="includeAdult">Включать ли передачи для взрослых</param>
        private IQueryable<Programmes> GetFilteredProgrammesQuery(int typeProgId, string category, IReadOnlyCollection<int> channelIds, bool includeAdult)
        {
            var query = _programmesRepository.Table
                .Where(pr => pr.TypeProgId == typeProgId)
                .Where(CreateChannelFilter(channelIds))
                .Where(CreateCategoryFilter(category));

            if (!includeAdult)
                query = query.Where(pr => pr.Category != ADULT_USERS && !pr.Title.Contains(AGE_18_PLUS));

            return query;
        }

        /// <summary>
        /// Создаёт фильтр по каналам (пустой список означает отсутствие ограничения)
        /// </summary>
        private static Expression<Func<Programmes, bool>> CreateChannelFilter(IReadOnlyCollection<int> channelIds)
        {
            if (channelIds.Count == 0)
                return pr => true;

            return pr => channelIds.Contains(pr.ChannelId);
        }

        /// <summary>
        /// Создаёт фильтр по категории: точное совпадение либо совпадение через справочник категорий.
        /// Для предопределённых категорий фильтрация выполняется в памяти после выборки
        /// (<see cref="MatchesPredefinedCategory"/>), поэтому ограничение не накладывается
        /// </summary>
        /// <param name="category">Категория</param>
        private Expression<Func<Programmes, bool>> CreateCategoryFilter(string category)
        {
            if (category == null || IsPredefinedCategory(category))
                return pr => true;

            var loweredCategory = category.ToLower();
            return pr => pr.Category == category ||
                _programmesTvProgCategoryMappingRepository.Table.Any(m => m.ProgrammesId == pr.Id &&
                    _tvProgCategoryRepository.Table.Any(c => c.Id == m.TvProgCategoryId && c.Active &&
                        c.Name.ToLower() == loweredCategory));
        }

        /// <summary>
        /// Применяет постобработку выбранных телепередач: жанры, предопределённые категории, пиктограммы каналов и фильтры
        /// </summary>
        /// <param name="systemProgramme">Выбранный список телепередач</param>
        /// <param name="typeProgId">Идентификатор типа программы</param>
        /// <param name="category">Категория</param>
        /// <param name="isAuthorized">Авторизован ли пользователь</param>
        /// <param name="channelIds">Идентификаторы каналов</param>
        /// <param name="genres">Идентификаторы жанров через ;</param>
        /// <param name="channels">Идентификаторы каналов через ;</param>
        private async Task ApplyPostFiltersAsync(List<SystemProgramme> systemProgramme, int typeProgId, string category,
            bool isAuthorized, IReadOnlyCollection<int> channelIds, string genres, string channels)
        {
            SetGenres(systemProgramme);

            // Пост-фильтрация по предопределённым категориям
            if (IsPredefinedCategory(category))
                ReplaceList(systemProgramme, systemProgramme.Where(x => MatchesPredefinedCategory(x.TelecastTitle, x.GenreName, category, isAuthorized)));

            await JoinChannelIcons(systemProgramme, typeProgId, channelIds);
            ReplaceList(systemProgramme, FilterByIds(systemProgramme, genres, x => x.GenreId.GetValueOrDefault()));
            ReplaceList(systemProgramme, FilterByIds(systemProgramme, channels, x => x.Cid));
        }

        /// <summary>
        /// Заменяет содержимое списка отфильтрованной последовательностью.
        /// Источник предварительно копируется в новый список: он может быть
        /// ленивой выборкой по target либо самим target, который очищается
        /// </summary>
        private static void ReplaceList<T>(List<T> target, IEnumerable<T> source)
        {
            var snapshot = source.ToList();
            target.Clear();
            target.AddRange(snapshot);
        }

        /// <summary>
        /// Установка жанров. Данные справочников загружаются одним запросом,
        /// сопоставление выполняется в памяти без N+1 запросов
        /// </summary>
        /// <param name="systemProgramme">Системная телепрограмма</param>
        private void SetGenres(List<SystemProgramme> systemProgramme)
        {
            if (systemProgramme.Count == 0)
                return;

            var genreItems = LoadGenreMatchItems();

            foreach (var sp in systemProgramme)
            {
                var genreMatch = ResolveGenreMatch(sp.TelecastTitle, sp.Category, genreItems);
                sp.GenreId = genreMatch?.Id ?? 1;
                sp.GenreName = genreMatch?.GenreName ?? NO_GENRE_NAME;
                sp.GenreContent = genreMatch?.GenreContent;
            }
        }

        /// <summary>
        /// Загрузка классификаторов жанров одним запросом с подготовкой фраз для сопоставления в памяти.
        /// Классификаторы вместе со справочником жанров и пиктограммами (пользовательские жанры не используются)
        /// </summary>
        private List<GenreMatchItem> LoadGenreMatchItems()
        {
            var nowUtc = DateTime.UtcNow;
            long? uid = null;

            var genreItems = (from gc in _genreClassificatorRepository.Table
                              join g in _genresRepository.Table on gc.GenreId equals g.Id
                              join mp2 in _mediaPicRepository.Table on g.IconId equals mp2.Id into gmp2
                              from mp2 in gmp2.DefaultIfEmpty()
                              where g.UserId == uid && gc.UserId == uid && g.Visible && g.DeleteDate == null &&
                                (gc.DeleteAfterDate == null || gc.DeleteAfterDate > nowUtc)
                              orderby gc.OrderCol
                              select new GenreMatchItem
                              {
                                  Id = g.Id,
                                  GenreName = g.GenreName,
                                  GenreContent = mp2.Path25 + mp2.FileName,
                                  ContainPhrases = gc.ContainPhrases,
                                  NonContainPhrases = gc.NonContainPhrases
                              }).ToList();

            foreach (var item in genreItems)
            {
                item.ContainPhrasesUpper = item.ContainPhrases.ToUpper().Split(';');
                item.NonContainPhrasesUpper = string.IsNullOrWhiteSpace(item.NonContainPhrases)
                    ? []
                    : item.NonContainPhrases.ToUpper().Split(';');
            }

            return genreItems;
        }

        /// <summary>
        /// Определяет жанр телепередачи: по названию категории либо по фразам-классификаторам в названии
        /// </summary>
        /// <param name="title">Название телепередачи</param>
        /// <param name="category">Категория (устаревшее поле)</param>
        /// <param name="genreItems">Классификаторы жанров</param>
        private static GenreMatchItem ResolveGenreMatch(string title, string category, List<GenreMatchItem> genreItems)
        {
            var titleUpper = (title ?? string.Empty).ToUpper();

            // Жанр, назначенный по названию категории
            var byCategory = genreItems.FirstOrDefault(item => item.GenreName == category);

            // Жанр, назначенный по фразам-классификаторам в названии передачи
            var byPhrases = genreItems.FirstOrDefault(item =>
                titleUpper.ContainsAny(item.ContainPhrasesUpper) &&
                (item.NonContainPhrasesUpper.Length == 0 ||
                 !titleUpper.ContainsAny(item.NonContainPhrasesUpper)));

            return byCategory ?? byPhrases;
        }

        /// <summary>
        /// Заполняет подпись "день(число.месяц)" для выбранных телепередач
        /// </summary>
        /// <param name="systemProgramme">Системный список телепрограммы</param>
        private static void SetDayMonth(IEnumerable<SystemProgramme> systemProgramme)
        {
            foreach (var pr in systemProgramme)
            {
                pr.DayMonth = $"{pr.TsStartMo.ToString("ddd", RussianCulture)}({pr.TsStartMo.Day:D2}.{pr.TsStartMo.Month:D2})";
            }
        }

        /// <summary>
        /// Фильтрует список телепередач по набору идентификаторов (строка через ;)
        /// </summary>
        /// <param name="systemProgramme">Системная программа</param>
        /// <param name="ids">Идентификаторы через ;</param>
        /// <param name="idSelector">Селектор идентификатора телепередачи</param>
        private static List<SystemProgramme> FilterByIds(List<SystemProgramme> systemProgramme, string ids, Func<SystemProgramme, long> idSelector)
        {
            if (string.IsNullOrWhiteSpace(ids))
                return systemProgramme;

            var idSet = ParseLongIds(ids);
            return [.. systemProgramme.Where(x => idSet.Contains(idSelector(x)))];
        }

        /// <summary>
        /// Фильтрует список телепередач по датам показа (включая день, следующий за последней выбранной датой)
        /// </summary>
        /// <param name="systemProgramme">Системная программа</param>
        /// <param name="rawDates">Даты показа</param>
        private static List<SystemProgramme> FilterByDates(List<SystemProgramme> systemProgramme, List<DateTime> rawDates)
        {
            if (rawDates.Count == 0)
                return systemProgramme;

            var allowedDates = rawDates.ToHashSet();
            allowedDates.Add(rawDates.Max().AddDays(1));

            return [.. systemProgramme.Where(x => allowedDates.Contains(x.TsStartMo.Date))];
        }

        /// <summary>
        /// Заполняет названия телеканалов и их иконки одним запросом со словарём вместо перебора
        /// </summary>
        /// <param name="systemProgramme">Системный список телепрограммы</param>
        /// <param name="typeProgId">Идентификатор типа программы</param>
        /// <param name="channelIds">Ограничение по каналам (пустой список - все)</param>
        private async Task JoinChannelIcons(List<SystemProgramme> systemProgramme, int typeProgId, IReadOnlyCollection<int> channelIds)
        {
            if (systemProgramme.Count == 0)
                return;

            var channelsQuery = from ch in _channelsRepository.Table
                                join pt in _providerTypeRepository.Table on ch.TvProgProviderId equals pt.Id
                                join tp in _typeProgRepository.Table on pt.Id equals tp.TvProgProviderId
                                join mp in _mediaPicRepository.Table on ch.IconId equals mp.Id into chmp
                                from mp in chmp.DefaultIfEmpty()
                                where tp.Id == typeProgId && ch.Deleted == null
                                select new { ChannelId = ch.Id, ChannelName = ch.TitleChannel, ChannelContent = mp.Path25 + mp.FileName };

            if (channelIds.Count > 0)
                channelsQuery = channelsQuery.Where(x => channelIds.Contains(x.ChannelId));

            var channelMap = new Dictionary<int, (string Name, string Content)>();
            foreach (var channel in await channelsQuery.ToListAsync())
                channelMap.TryAdd(channel.ChannelId, (channel.ChannelName, channel.ChannelContent));

            foreach (var sp in systemProgramme)
            {
                if (channelMap.TryGetValue(sp.Cid, out var channelItem))
                {
                    sp.ChannelName = channelItem.Name;
                    sp.ChannelContent = channelItem.Content;
                }
            }
        }

        /// <summary>
        /// Получение пути к изображению анонса (с кешированием)
        /// </summary>
        private Task<string> GetAnonsImagePathAsync() =>
            _cacheManager.GetAsync(TvProgProgrammeDefaults.AnonsImagePathCacheKey, async () =>
            {
                var image = await _mediaPicRepository.Table
                    .FirstOrDefaultAsync(mp => mp.FileName == GREEN_ANONS_PNG, CancellationToken.None);
                return image != null ? image.Path25 + image.FileName : null;
            });

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

            var elapsed = (endTime - currentTime).TotalSeconds;
            return (int)(elapsed / totalDuration * 100);
        }

        #endregion

        #region Предопределённые категории

        /// <summary>
        /// Проверяет, является ли категория предопределённой системной категорией
        /// </summary>
        private static bool IsPredefinedCategory(string category)
        {
            return category != null && PredefinedCategories.Contains(category);
        }

        /// <summary>
        /// Извлекает возрастную метку из названия передачи (например, "(12+)", "(18+)")
        /// </summary>
        /// <param name="title">Название передачи</param>
        /// <returns>Возрастная метка или null, если она не указана</returns>
        private static int? ExtractAgeFromTitle(string title)
        {
            if (string.IsNullOrWhiteSpace(title))
                return null;

            // Ищем шаблон "(X+)" — X это одна или несколько цифр
            var match = AgeFromTitleRegex.Match(title);
            if (match.Success && int.TryParse(match.Groups[1].Value, out var age))
                return age;

            return null;
        }

        /// <summary>
        /// Проверяет, соответствует ли передача запрошенной предопределённой категории
        /// </summary>
        /// <param name="title">Название передачи</param>
        /// <param name="genreName">Название жанра</param>
        /// <param name="requestedCategory">Запрошенная категория</param>
        /// <param name="isAuthorized">Авторизован ли пользователь</param>
        private static bool MatchesPredefinedCategory(string title, string genreName, string requestedCategory, bool isAuthorized)
        {
            var age = ExtractAgeFromTitle(title);

            switch (requestedCategory)
            {
                case CATEGORY_CHILDREN:
                    // Дети — возраст 0+, 6+, 12+ (т.е. < 16)
                    return age.HasValue && age.Value < 16;

                case CATEGORY_MOVIE:
                    // Художественный фильм — по жанру
                    return !string.IsNullOrWhiteSpace(genreName) &&
                        (genreName.IndexOf("художественный", StringComparison.OrdinalIgnoreCase) >= 0 ||
                         genreName.IndexOf("фильм", StringComparison.OrdinalIgnoreCase) >= 0);

                case CATEGORY_SERIES:
                    // Сериал — по жанру или по названию
                    if (!string.IsNullOrWhiteSpace(genreName) &&
                        (genreName.IndexOf("сериал", StringComparison.OrdinalIgnoreCase) >= 0 ||
                         genreName.IndexOf("сезон", StringComparison.OrdinalIgnoreCase) >= 0))
                        return true;
                    return !string.IsNullOrWhiteSpace(title) &&
                        (title.IndexOf("сериал", StringComparison.OrdinalIgnoreCase) >= 0 ||
                         title.IndexOf("сезон", StringComparison.OrdinalIgnoreCase) >= 0);

                case CATEGORY_SPORT:
                    // Спорт — по жанру
                    return !string.IsNullOrWhiteSpace(genreName) &&
                        genreName.IndexOf("спорт", StringComparison.OrdinalIgnoreCase) >= 0;

                case CATEGORY_ADULT:
                    // Для взрослых — 18+ И пользователь авторизован
                    return isAuthorized && age.HasValue && age.Value >= 18;

                case CATEGORY_NONE:
                    // Без категории — передача не относится ни к одной другой предопределённой категории
                    return !MatchesAnyOtherPredefinedCategory(title, genreName, isAuthorized);

                default:
                    return false;
            }
        }

        /// <summary>
        /// Проверяет, соответствует ли передача какой-либо другой предопределённой категории (кроме «Без категории»).
        /// ADULT учитывается только для авторизованных пользователей
        /// </summary>
        private static bool MatchesAnyOtherPredefinedCategory(string title, string genreName, bool isAuthorized)
        {
            return MatchesPredefinedCategory(title, genreName, CATEGORY_CHILDREN, isAuthorized) ||
                MatchesPredefinedCategory(title, genreName, CATEGORY_MOVIE, isAuthorized) ||
                MatchesPredefinedCategory(title, genreName, CATEGORY_SERIES, isAuthorized) ||
                MatchesPredefinedCategory(title, genreName, CATEGORY_SPORT, isAuthorized) ||
                MatchesPredefinedCategory(title, genreName, CATEGORY_ADULT, isAuthorized);
        }

        #endregion

        #region Вся телепрограмма

        /// <summary>
        /// Получение всей телепрограммы
        /// </summary>
        /// <param name="page">Страница</param>
        /// <param name="rows">Строки</param>
public virtual async Task<List<GdProgramme>> GetAllProgrammes(int page, int rows)
        {
            Pagination pagination = new()
            {
                Number = page,
                Count = rows
            };

            var programmeItems = await (from pr in _programmesRepository.Table
                                        join ch in _channelsRepository.Table on pr.ChannelId equals ch.Id into prch
                                        from ch in prch.DefaultIfEmpty()
                                        orderby ch.SysOrderCol, pr.TsStartMo
                                        select new
                                        {
                                            pr.Id,
                                            pr.InternalChanId,
                                            pr.TsStartMo,
                                            pr.TsStopMo,
                                            TelecastTitle = pr.Title,
                                            Category = pr.Category
                                        }).Limit(pagination).ToListAsync();

            var programmeIds = programmeItems.Select(p => p.Id).ToList();

            // Категории из связующей таблицы одним запросом
            var categoriesLookup = (await (from m in _programmesTvProgCategoryMappingRepository.Table
                                           join c in _tvProgCategoryRepository.Table on m.TvProgCategoryId equals c.Id
                                           where programmeIds.Contains(m.ProgrammesId) && c.Ident != null
                                           select new { m.ProgrammesId, c.Ident }).ToListAsync())
                .GroupBy(x => x.ProgrammesId)
                .ToDictionary(g => g.Key, g => g.Select(x => x.Ident).ToArray());

            // Классификаторы жанров одним запросом для определения предопределённых категорий
            var genreItems = LoadGenreMatchItems();

            List<GdProgramme> gdProgrammeList = new(programmeItems.Count);
            foreach (var p in programmeItems)
            {
                var categories = categoriesLookup.TryGetValue(p.Id, out var mapped)
                    ? mapped.ToList()
                    : new List<string>();

                // Предопределённые категории (кроме ADULT и NON_CATEGORY) по заложенной логике отбора
                var genreName = ResolveGenreMatch(p.TelecastTitle, p.Category, genreItems)?.GenreName ?? NO_GENRE_NAME;
                foreach (var categoryName in PredefinedCategoryIdents.Keys)
                {
                    if (categoryName == CATEGORY_NONE)
                        continue;

                    if (MatchesPredefinedCategory(p.TelecastTitle, genreName, categoryName, false) &&
                        PredefinedCategoryIdents.TryGetValue(categoryName, out var ident))
                    {
                        categories.Add(ident);
                    }
                }

                // «Без категории» добавляется только если у передачи не оказалось ни одной другой категории
                if (categories.Count == 0 &&
                    PredefinedCategoryIdents.TryGetValue(CATEGORY_NONE, out var noneIdent))
                {
                    categories.Add(noneIdent);
                }

                gdProgrammeList.Add(new GdProgramme
                {
                    InternalChanId = p.InternalChanId,
                    TsStartMo = p.TsStartMo,
                    TsStopMo = p.TsStopMo,
                    TelecastTitle = p.TelecastTitle,
                    Categories = categories.Distinct().ToArray()
                });
            }

            return gdProgrammeList;
        }

        /// <summary>
        /// Получение состояния телепрограммы
        /// </summary>
        public virtual async Task<GdProgrammeStatus> GetProgrammeStatus()
        {
            GdProgrammeStatus status = new GdProgrammeStatus
            {
                Qty = await _programmesRepository.Table.CountAsync(CancellationToken.None),
                MaxDateTime = await _programmesRepository.Table.MaxAsync(x => (DateTime?)x.TsStopMo)
            };
            return status;
        }
        #endregion

        #region Вложенные классы

        /// <summary>
        /// Элемент классификатора жанров, подготовленный к сопоставлению в памяти
        /// </summary>
        private sealed class GenreMatchItem
        {
            public long Id { get; init; }
            public string GenreName { get; init; }
            public string GenreContent { get; init; }
            public string ContainPhrases { get; init; }
            public string NonContainPhrases { get; init; }
            public string[] ContainPhrasesUpper { get; set; }
            public string[] NonContainPhrasesUpper { get; set; }
        }

        #endregion
    }
}
