using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using TVProgViewer.Data.TvProgMain.ProgObjs;
using TVProgViewer.Data;
using TVProgViewer.Core.Domain.TvProgMain;
using TVProgViewer.Core.Caching;
using TVProgViewer.Services.Caching.Extensions;
using System.Globalization;
using LinqToDB;
using TVProgViewer.Data.TvProgMain;

namespace TVProgViewer.Services.TvProgMain
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
        public virtual TvProgProviders GetProviderById(int providerId)
        {
            if (providerId == 0)
                return null;

            return _providerTypeRepository.ToCachedGetById(providerId);
        }

        /// <summary>
        /// Получение всех ТВ-провайдеров
        /// </summary>
        public virtual List<TvProgProviders> GetAllProviders()
        {
            return _providerTypeRepository.Table.ToList();
        }

        /// <summary>
        /// Получение типа программы по его идентификатору
        /// </summary>
        /// <param name="typeProgId">Идентификатор типа программы</param>
        public virtual TypeProg GetTypeProgById(int typeProgId)
        {
            if (typeProgId == 0)
                return null;

            return _typeProgRespository.ToCachedGetById(typeProgId);
        }

        /// <summary>
        /// Получение всех типов ТВ-программы
        /// </summary>
        public virtual List<TypeProg> GetAllTypeProgs()
        {
            return _typeProgRespository.Table.ToList();
        }

        /// <summary>
        /// Получение списка провайдеров телепрограммы
        /// </summary>
        /// <returns></returns>
        public List<ProviderType> GetProviderTypeList()
        {
            return (from tpp in _providerTypeRepository.Table
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
                    }).ToList();
        }

        

        /// <summary>
        /// Получение периода действия программы телепередач
        /// </summary>
        /// <param name="TypeProgId">Идентификатор тип программы</param>
        /// <returns></returns>
        public ProgPeriod GetSystemProgrammePeriod(int typeProgId)
        {
            ProgPeriod progPeriod = new ProgPeriod()
            {
                dtStart = _programmesRepository.Table.Where(x => x.TypeProgId == typeProgId).Min(x => (DateTimeOffset?)x.TsStartMo),
                dtEnd = _programmesRepository.Table.Where(x => x.TypeProgId == typeProgId).Max(x => (DateTimeOffset?)x.TsStopMo)
            };

            return progPeriod;
        }

        /// <summary>
        /// Получение категорий телепередач
        /// </summary>
        public List<string> GetCategories()
        {
            List<string> categories = new List<string>();
            categories = _programmesRepository.Table.Select(x => x.Category).Distinct().OrderBy(o => o).ToList<string>();
            
            return categories;
        }

        private void SetRatings(List<SystemProgramme> systemProgramme, long? uid)
        {
            if (!uid.HasValue)
                return;

            foreach (SystemProgramme sp in systemProgramme)
            {
                var ratingAttrs = (from rc in _ratingClassificatorRepository.Table
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
            foreach (SystemProgramme sp in systemProgramme)
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
            }
        }

        /// <summary>
        /// Фильтрация по жанрам
        /// </summary>
        /// <param name="systemProgramme">Системная программа</param>
        /// <param name="genres">Идентификаторы жанров через ;</param>
        private List<SystemProgramme> FilterGenres(List<SystemProgramme> systemProgramme, string genres)
        {
            if (string.IsNullOrWhiteSpace(genres))
                return systemProgramme;
            long Gid;
            return systemProgramme.Where(x => genres.Split(new[] { ';' }, StringSplitOptions.RemoveEmptyEntries).Any(g => long.TryParse(g, out Gid) && long.Parse(g) == x.GenreId)).ToList();
        }

        /// <summary>
        /// Получение названия жанра через идентификатор
        /// </summary>
        /// <param name="idStr">Идентификатор жанра</param>
        private string GetNameByIdGenre(string idStr, long? Uid)
        {
            long id;

            if (!long.TryParse(idStr, out id))
                return string.Empty;

            return (from g in _genresRepository.Table
                    where g.Id == id && g.UserId == Uid && g.Visible && g.DeleteDate == null
                    select g.GenreName).First();
        }
        /// <summary>
        /// Выборка телепрограммы
        /// </summary>
        /// <param name="TypeProgId">Тип программы телепередач</param>
        /// <param name="dateTimeOffset">Время</param>
        /// <param name="mode">режимы выборки: 1 - сейчас; 2 - следом</param>
        public KeyValuePair<int, List<SystemProgramme>> GetSystemProgrammes(int TypeProgId, DateTimeOffset dateTimeOffset, int mode, string category,
                                                         string sidx, string sord, int page, int rows, string genres)
        {
            List<SystemProgramme> systemProgramme = new List<SystemProgramme>();
            DateTime minDate = new DateTime(1800, 1, 1);
            DateTime dateTime = dateTimeOffset.DateTime;
            int count = 0;
            
            int[] splittedGenres = !string.IsNullOrWhiteSpace(genres) && !genres.Contains("undefined") ? Array.ConvertAll(genres.Split(';'), int.Parse) : new[] { 0 };
            switch (mode)
            {
                case 1:

                    var sp = (from pr in _programmesRepository.Table
                              join ch in _channelsRepository.Table on pr.ChannelId equals ch.Id
                              join mp in _mediaPicRepository.Table on ch.IconId equals mp.Id into chmp
                              from mp in chmp.DefaultIfEmpty()
                              where pr.TypeProgId == TypeProgId && pr.TsStartMo <= dateTime &&
                                    dateTime < pr.TsStopMo && pr.Category != "Для взрослых" &&
                                    !pr.Title.Contains("(18+)") && ch.Deleted == null &&
                                    (category == null || pr.Category == category)
                              select new SystemProgramme
                              {
                                  ProgrammesId = pr.Id,
                                  Cid = pr.ChannelId,
                                  ChannelName = ch.TitleChannel,
                                  ChannelContent = mp.Path25 + mp.FileName,
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
                              }).ToList();
                    SetGenres(sp, null);
                    sp = FilterGenres(sp, genres);
                    count = sp.Count();
                    systemProgramme = sp.AsQueryable().OrderBy(!(sidx == null || sidx.Trim() == string.Empty) ? sidx : "InternalChanId", sord)
                                        .Skip((page - 1) * rows).Take(rows)
                                        .ToList<SystemProgramme>();
                    break;
                case 2:
                    if (dateTime == minDate)
                    {
                        List<TsStopForRemain> stopAfter = (from pr2 in _programmesRepository.Table
                                                           where pr2.TsStartMo <= DateTime.Now && DateTime.Now < pr2.TsStopMo && pr2.TypeProgId == TypeProgId
                                                             && pr2.Category != "Для взрослых" && !pr2.Title.Contains("(18+)")
                                                           select new TsStopForRemain
                                                           {
                                                               TsStopMoAfter = pr2.TsStopMo.AddSeconds(1),
                                                               Cid = pr2.ChannelId
                                                           }).ToList();
                        DateTime afterTwoDays = DateTime.Now.AddDays(2);
                        var sp2 = (from pr3 in _programmesRepository.Table
                                   join ch in _channelsRepository.Table on pr3.ChannelId equals ch.Id
                                   join mp in _mediaPicRepository.Table on ch.IconId equals mp.Id into chmp
                                   from mp in chmp.DefaultIfEmpty()
                                   where pr3.TypeProgId == TypeProgId
                                   && pr3.TsStartMo >= DateTime.Now && afterTwoDays > pr3.TsStopMo
                                   && pr3.Category != "Для взрослых" && !pr3.Title.Contains("(18+)")
                                   && ch.Deleted == null &&
                                     (category == null || pr3.Category == category)
                                   orderby pr3.InternalChanId, pr3.TsStartMo
                                   select new SystemProgramme
                                   {
                                       ProgrammesId = pr3.Id,
                                       Cid = pr3.ChannelId,
                                       ChannelName = ch.TitleChannel,
                                       ChannelContent = mp.Path25 + mp.FileName,
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
                                       Remain = Sql.DateDiff(Sql.DateParts.Second, DateTime.Now, pr3.TsStartMo)
                                   }
                                           );

                        var sp3 = (from pr in sp2.ToList()
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
                                       Remain = pr.Remain
                                   }).ToList();
                        SetGenres(sp3, null);
                        sp3 = FilterGenres(sp3, genres);
                        count = sp3.Count();

                        systemProgramme = sp3.AsQueryable().OrderBy(!(sidx == null || sidx.Trim() == string.Empty) ? sidx : "InternalChanId", sord)
                                        .Skip((page - 1) * rows).Take(rows)
                                        .ToList<SystemProgramme>();
                    }
                    else if (dateTime > minDate)
                    {
                        var sp4 = (from pr in _programmesRepository.Table
                                   join ch in _channelsRepository.Table on pr.ChannelId equals ch.Id
                                   join mp in _mediaPicRepository.Table on ch.IconId equals mp.Id into chmp
                                   from mp in chmp.DefaultIfEmpty()
                                   where pr.TypeProgId == TypeProgId && DateTime.Now < pr.TsStartMo && pr.TsStartMo <= dateTime
                                   && pr.Category != "Для взрослых" && !pr.Title.Contains("(18+)")
                                   && ch.Deleted == null &&
                                     (category == null || pr.Category == category)
                                   orderby pr.InternalChanId, pr.TsStart
                                   select new SystemProgramme
                                   {
                                       ProgrammesId = pr.Id,
                                       Cid = pr.ChannelId,
                                       ChannelName = ch.TitleChannel,
                                       ChannelContent = mp.Path25 + mp.FileName,
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
                                       Remain = Sql.DateDiff(Sql.DateParts.Second, DateTime.Now, pr.TsStartMo)
                                   }).ToList<SystemProgramme>();
                        SetGenres(sp4, null);
                        sp4 = FilterGenres(sp4, genres);
                        count = sp4.Count();

                        systemProgramme = sp4.AsQueryable().OrderBy(!(sidx == null || sidx.Trim() == string.Empty) ? sidx : "InternalChanId", sord)
                                        .Skip((page - 1) * rows).Take(rows)
                                        .ToList<SystemProgramme>();
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
        public KeyValuePair<int, List<SystemProgramme>> SearchProgramme(int typeProgId, string findTitle, string category,
                                                         string sidx, string sord, int page, int rows, string genres, string dates)
        {
            List<SystemProgramme> systemProgramme = new List<SystemProgramme>();
            int count = 0;
            int[] splittedGenres = !string.IsNullOrWhiteSpace(genres) && !genres.Contains("undefined") ? Array.ConvertAll(genres.Split(';'), int.Parse) : new[] { 0 };
            if (string.IsNullOrWhiteSpace(dates))
                return new KeyValuePair<int, List<SystemProgramme>>();
            DateTime date;
            CultureInfo ci = new CultureInfo("Ru-ru");
            var rawDates = dates.Split(new[] { ';' }, StringSplitOptions.RemoveEmptyEntries).Select(x => DateTime.TryParseExact(x, "yyyyMMdd", ci, DateTimeStyles.None, out date) ? DateTime.ParseExact(x, "yyyyMMdd", ci) : new DateTime()).ToList();
            DateTime minDateTime = rawDates.Min();   
            DateTime maxDateTime = rawDates.Max().AddDays(1);

            systemProgramme = (from pr in _programmesRepository.Table
                               join ch in _channelsRepository.Table on pr.ChannelId equals ch.Id
                               join mp in _mediaPicRepository.Table on ch.IconId equals mp.Id into chmp
                               from mp in chmp.DefaultIfEmpty()
                               where pr.TypeProgId == typeProgId
                                    && pr.Category != "Для взрослых" && !pr.Title.Contains("(18+)")
                                    && (category == null || pr.Category == category)
                                    && ch.Deleted == null
                                    && pr.Title.Contains(findTitle)
                                    && minDateTime <= pr.TsStartMo && pr.TsStartMo <= maxDateTime  
                               orderby pr.TsStartMo, pr.TsStopMo
                               select new SystemProgramme
                               {
                                   ProgrammesId = pr.Id,
                                   Cid = pr.ChannelId,
                                   ChannelName = ch.TitleChannel,
                                   ChannelContent = mp.Path25 + mp.FileName,
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
                               }).ToList();
            systemProgramme.ForEach(pr =>
            {
                pr.DayMonth = pr.TsStartMo.ToString("ddd", new CultureInfo("ru-Ru")) +
           String.Format("({0:D2}.{1:D2})", pr.TsStartMo.Day, pr.TsStartMo.Month);
            });
            SetGenres(systemProgramme, null);
            systemProgramme = FilterGenres(systemProgramme, genres);
            count = systemProgramme.Count();

            systemProgramme = systemProgramme.AsQueryable().OrderBy(!(sidx == null || sidx.Trim() == string.Empty) ? sidx : "TsStartMo", sord)
                                         .Skip((page - 1) * rows).Take(rows)
                                         .ToList<SystemProgramme>();
            return new KeyValuePair<int, List<SystemProgramme>>(count, systemProgramme);
        }
        #endregion
    }
}
