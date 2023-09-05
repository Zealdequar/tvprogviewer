using System;
using System.Collections.Generic;
using System.Data;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using MoreLinq;
using System.Globalization;
using TvProgViewer.DataAccess.Models;
using TvProgViewer.Data.TvProgMain.ProgObjs;
using TvProgViewer.Data.TvProgMain;

namespace TvProgViewer.DataAccess.Adapters
{
    /// <summary>
    /// Адаптер для работы с телеканалами и телепрограмой
    /// </summary>
    public class TvProgAdapter : AdapterBase
    {
        #region Служебные методы
        private static readonly NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();
        /// <summary>
        /// Получение списка типов телепрограммы
        /// </summary>
        /// <returns></returns>
        public List<ProviderType> GetTvProviderTypes()
        {
            try
            {
                return (from tpp in dataContext.TvprogProviders.AsNoTracking()
                        join tp in dataContext.TypeProg.AsNoTracking() on tpp.TvprogProviderId equals tp.TvprogProviderId
                        select new
                        {
                            TvprogProviderId = tpp.TvprogProviderId,
                            ProviderName = tpp.ProviderName,
                            ProviderText = tpp.ProviderWebSite,
                            TypeProgId = tp.TypeProgId,
                            TypeName = tp.TypeName,
                            TypeEnum = (tp.TypeName == "Формат XMLTV") ? Enums.TypeProg.XMLTV : Enums.TypeProg.InterTV,
                            FileFormat = tp.FileFormat
                        }).ToList().Select(mapper.Map<ProviderType>).ToList();
            }
            catch (Exception ex)
            {
            }
            return new List<ProviderType>();
        }

        /*/// <summary>
        /// Получение периода действия программы телепередач
        /// </summary>
        /// <param name="TypeProgId">Идентификатор тип программы</param>
        /// <returns></returns>
        public ProgPeriod GetSystemProgrammePeriod(int TypeProgId)
        {
            ProgPeriod progPeriod = new ProgPeriod()
            {
                dtStart = dataContext.Programmes.Where(x => x.Tid == TypeProgId).Min(x => x.TsStart),
                dtEnd = dataContext.Programmes.Where(x => x.Tid == TypeProgId).Max(x => x.TsStop)
            };

            return progPeriod;
        }

        /// <summary>
        /// Получение категорий телепередач
        /// </summary>
        public List<string> GetCategories()
        {
            List<string> categories = new List<string>();
            try
            {
                categories = dataContext.Programmes.AsNoTracking().Select(x => x.Category).Distinct().OrderBy(o => o).ToList<string>();
            }
            catch (Exception ex)
            {
            }

            return categories;
        }

        #endregion

        #region Работа с телеканалами

        /// <summary>
        /// Получение системных каналов
        /// </summary>
        /// <param name="TvprogProviderId">Провайдер телепрограммы</param>
        public List<SystemChannel> GetSystemChannels(int TvprogProviderId)
        {

            List<SystemChannel> listSystemChannels = new List<SystemChannel>();
            try
            {
                listSystemChannels = (from ch in dataContext.Channels.AsNoTracking()
                                      join mp in dataContext.MediaPic.AsNoTracking() on ch.IconId equals mp.IconId into chmp
                                      from mp in chmp.DefaultIfEmpty()
                                      where ch.TvprogProviderId == TvprogProviderId && ch.Deleted == null
                                      select new
                                      {
                                          ChannelId = ch.ChannelId,
                                          TVProgVieweRid = ch.TvprogProviderId,
                                          InternalID = ch.InternalId,
                                          IconId = ch.IconId,
                                          FileNameOrig = mp.PathOrig + mp.FileName,
                                          FileName25 = mp.Path25 + mp.FileName,
                                          Title = ch.TitleChannel,
                                          ImageWebSrc = ch.IconWebSrc,
                                          OrderCol = 0
                                      }).AsNoTracking().Select(mapper.Map<SystemChannel>).OrderBy(o => o.InternalId).ToList();

            }
            catch (Exception ex)
            {
            }
            return listSystemChannels;
        }

        /// <summary>
        /// Получение пользовательских телеканалов
        /// </summary>
        /// <param name="Uid">Идентификатор пользователя</param>
        /// <param name="TypeProgId">Тип программы телепередач</param>
        public List<UserChannel> GetUserChannels(long Uid, int TypeProgId)
        {

            List<UserChannel> listUserChannels = new List<UserChannel>();
            try
            {
                listUserChannels = (from ch in dataContext.Channels.AsNoTracking()
                                    join uch in dataContext.UserChannels.AsNoTracking() on ch.ChannelId equals uch.Cid
                                    join mp in dataContext.MediaPic.AsNoTracking() on uch.IconId equals mp.IconId into chmp
                                    from mp in chmp.DefaultIfEmpty()
                                    where uch.Uid == Uid &&
                                    ch.TvprogProviderId == TypeProgId
                                    && ch.Deleted == null
                                    orderby uch.OrderCol
                                    select new
                                    {
                                        UserChannelId = uch.UserChannelId,
                                        ChannelId = uch.Cid,
                                        TVProgVieweRid = ch.TvprogProviderId,
                                        ChannelName = ch.TitleChannel,
                                        InternalID = ch.InternalId,
                                        IconId = uch.IconId,
                                        FileNameOrig = mp.PathOrig + mp.FileName,
                                        FileName25 = mp.Path25 + mp.FileName,
                                        TelecastTitle = ch.TitleChannel,
                                        UserTitle = uch.DisplayName,
                                        ImageWebSrc = ch.IconWebSrc,
                                        OrderCol = uch.OrderCol
                                    }).AsNoTracking().Select(mapper.Map<UserChannel>).OrderBy(o => o.OrderCol).ToList<UserChannel>();
            }
            catch (Exception ex)
            {
                Logger.Error(ex, ex.Message);
            }
            return listUserChannels;
        }

        /// <summary>
        /// Получение карты пользовательских телеканалов по системной
        /// </summary>
        /// <param name="Uid">Идентификатор пользователя</param>
        /// <param name="TvprogProviderId">Идентификатор провайдера программы телепередач</param>
        /// <param name="TypeProgId">Тип программы телепередач</param>
        public List<SystemChannel> GetUserInSystemChannels(long Uid, int TvprogProviderId, int TypeProgId)
        {
            List<SystemChannel> systemChannels = GetSystemChannels(TvprogProviderId).ToList();
            List<UserChannel> userChannels = GetUserChannels(Uid, TypeProgId).ToList();
            systemChannels.ForEach(sch =>
            {
                UserChannel userChannel = userChannels.Find(uch => uch.ChannelId == sch.ChannelId);
                sch.Visible = userChannel != null;
                if (sch.Visible)
                {
                    sch.UserChannelId = userChannel.UserChannelId;
                    sch.FileName25 = userChannel.FileName25;
                    sch.FileNameOrig = userChannel.FileNameOrig;
                    sch.OrderCol = userChannel.OrderCol;
                    sch.UserTitle = userChannel.Title;
                }
            });
            return systemChannels;
        }

        /// <summary>
        /// Добавление/обновление пользовательского телеканала
        /// </summary>
        /// <param name="UserChannelId">Идентификатор телеканала</param>
        /// <param name="Uid">Идентификатор пользователя</param>
        /// <param name="TvprogProviderId">Идентификатор источника программы телепередач</param>
        /// <param name="Cid">Идентификатор системного телеканала</param>
        /// <param name="displayName">Пользовательское название</param>
        /// <param name="orderCol">Порядковый номер</param>
        public void InsertUserChannel(int UserChannelId, long Uid,
                            int TvprogProviderId, int Cid, string displayName, int orderCol)
        {
            try
            {
                int qty = dataContext.UserChannels.Count(x => x.Uid == Uid && x.Cid == Cid);
                if (qty == 0)
                {
                    Channels channel = dataContext.Channels.AsNoTracking().Single(ch => ch.TvprogProviderId == TvprogProviderId && ch.ChannelId == Cid && ch.Deleted == null);
                    UserChannels userChannel = new UserChannels()
                    {
                        Uid = Uid,
                        Cid = channel.ChannelId,
                        IconId = channel.IconId,
                        DisplayName = string.IsNullOrWhiteSpace(displayName) ? channel.TitleChannel : displayName,
                        OrderCol = orderCol
                    };
                    dataContext.UserChannels.Add(userChannel);
                    dataContext.SaveChanges();
                }
                else
                    UpdateUserChannel(UserChannelId, Cid, displayName, orderCol);
            }
            catch (Exception ex)
            {

            }
        }

        /// <summary>
        /// Удаление пользовательского телеканала
        /// </summary>
        /// <param name="Uid">Идентификатор пользователя</param>
        /// <param name="Cid">Системный идентификатор телеканала</param>
        public void DeleteUserChannel(long Uid, int Cid)
        {
            try
            {
                UserChannels userChannel = dataContext.UserChannels.SingleOrDefault(x => x.Uid == Uid && x.Cid == Cid);
                if (userChannel != null)
                {
                    dataContext.UserChannels.Remove(userChannel);
                    dataContext.SaveChanges();
                }
            }
            catch (Exception ex)
            {

            }
        }

        /// <summary>
        /// Обновление пользовательского телеканала
        /// </summary>
        /// <param name="UserChannelId">Идентификатор телеканала</param>
        /// <param name="Cid">Системный идентификатор телеканала</param>
        /// <param name="displayName">Пользовательское название телеканала</param>
        /// <param name="orderCol">Порядковый номер телеканала</param>
        public void UpdateUserChannel(int UserChannelId, int Cid, string displayName, int orderCol)
        {
            try
            {
                UserChannels channel = dataContext.UserChannels.Single(x => x.UserChannelId == UserChannelId);
                channel.Cid = Cid;
                channel.DisplayName = displayName;
                channel.OrderCol = orderCol;
                dataContext.SaveChanges();
            }
            catch (Exception ex)
            {

            }
        }


        /// <summary>
        /// Изменение пиктограммы телеканала
        /// </summary>
        /// <param name="Uid">Идентификатор пользователя</param>
        /// <param name="UserChannelId">Идентификатор пользовательского телеканала</param>
        /// <param name="filename">Название файла</param>
        /// <param name="contentType">Тип содержимого</param>
        /// <param name="length">Размер большой пиктограммы в байтах</param>
        /// <param name="length25">Размер маленькой пиктограммы в байтах</param>
        /// <param name="pathOrig">Путь к большой пиктограмме</param>
        /// <param name="path25">Путь к маленькой пиктограмме</param>
        public void ChangeChannelImage(long Uid, int UserChannelId, string filename, string contentType,
            int length, int length25, string pathOrig, string path25)
        {
            try
            {
                MediaPic mp = new MediaPic()
                {
                    FileName = filename,
                    ContentType = contentType,
                    ContentCoding = "gzip",
                    Length = length,
                    Length25 = length25,
                    IsSystem = false,
                    PathOrig = pathOrig,
                    Path25 = path25
                };
                dataContext.MediaPic.Add(mp);

                UserChannels channel = dataContext.UserChannels.Single(x => x.UserChannelId == UserChannelId);
                channel.IconId = mp.IconId;
                dataContext.SaveChanges();
            }
            catch (Exception ex)
            {

            }
        }

        #endregion

        #region Работа с телепередачами
        private void SetRatings(List<SystemProgramme> systemProgramme, long? uid)
        {
            if (!uid.HasValue)
                return;

            foreach (SystemProgramme sp in systemProgramme)
            {
                var ratingAttrs = (from rc in dataContext.RatingClassificator.AsNoTracking()
                                   join r in dataContext.Ratings.AsNoTracking() on rc.Rid equals r.RatingId
                                   join mp2 in dataContext.MediaPic.AsNoTracking() on r.IconId equals mp2.IconId into rmp2
                                   from mp2 in rmp2.DefaultIfEmpty()
                                   where r.Uid == uid && rc.Uid == uid && r.Visible && r.DeleteDate == null &&
                                      (rc.DeleteAfterDate == null || rc.DeleteAfterDate > DateTime.Now)
                                   orderby rc.OrderCol
                                   select new { r.RatingId, r.RatingName, mp2.Path25, mp2.FileName, rc.ContainPhrases, rc.NonContainPhrases })
                                                 .ToList()
                                                 .Where(rc => sp.TelecastTitle.ToUpper().ContainsAny(rc.ContainPhrases.ToUpper().Split(';'))
                                                        && (string.IsNullOrWhiteSpace(rc.NonContainPhrases) ||
                                                        (!string.IsNullOrWhiteSpace(rc.NonContainPhrases) &&
                                                        !sp.TelecastTitle.ToUpper().ContainsAny(rc.NonContainPhrases.ToUpper().Split(';')))))
                                                 .Select(mp2 => new { mp2.RatingId, mp2.RatingName, RatingContent = mp2.Path25 + mp2.FileName }).FirstOrDefault();
                sp.RatingId = ratingAttrs?.RatingId ?? (from r in dataContext.Ratings.AsNoTracking()
                                                        where r.Uid == uid && r.RatingName == "Без рейтинга" && r.Visible && r.DeleteDate == null
                                                        select r.RatingId).FirstOrDefault();
                sp.RatingName = ratingAttrs?.RatingName ?? "Без рейтинга";
                sp.RatingContent = ratingAttrs?.RatingContent ?? (from m in dataContext.MediaPic.AsNoTracking()
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
                var genreCategoryAttrs = (from gc in dataContext.GenreClassificator.AsNoTracking()
                                          join g in dataContext.Genres.AsNoTracking() on gc.Gid equals g.GenreId
                                          join mp2 in dataContext.MediaPic.AsNoTracking() on g.IconId equals mp2.IconId into gmp2
                                          from mp2 in gmp2.DefaultIfEmpty()
                                          where sp.Category == g.GenreName && g.Uid == uid && gc.Uid == uid && g.Visible && g.DeleteDate == null &&
                                           (gc.DeleteAfterDate == null || gc.DeleteAfterDate > DateTime.Now)
                                          orderby gc.OrderCol
                                          select new {g.GenreId, g.GenreName, GenreContent = mp2.Path25 + mp2.FileName }).FirstOrDefault();
                var genreClassifAttrs = (from gc in dataContext.GenreClassificator.AsNoTracking()
                                         join g in dataContext.Genres.AsNoTracking() on gc.Gid equals g.GenreId
                                         join mp2 in dataContext.MediaPic.AsNoTracking() on g.IconId equals mp2.IconId into gmp2
                                         from mp2 in gmp2.DefaultIfEmpty()
                                         where g.Uid == uid && gc.Uid == uid && g.Visible && g.DeleteDate == null &&
                                         (gc.DeleteAfterDate == null || gc.DeleteAfterDate > DateTime.Now)
                                         orderby gc.OrderCol
                                         select new { g.GenreId, g.GenreName, mp2.Path25, mp2.FileName, gc.ContainPhrases, gc.NonContainPhrases })
                                                .ToList()
                                                .Where(gc => sp.TelecastTitle.ToUpper().ContainsAny(gc.ContainPhrases.ToUpper().Split(';'))
                                                       && (string.IsNullOrWhiteSpace(gc.NonContainPhrases) ||
                                                       (!string.IsNullOrWhiteSpace(gc.NonContainPhrases) &&
                                                       !sp.TelecastTitle.ToUpper().ContainsAny(gc.NonContainPhrases.ToUpper().Split(';')))))
                                                .Select(mp2 => new { mp2.GenreId, mp2.GenreName, GenreContent = mp2.Path25 + mp2.FileName }).FirstOrDefault();
                sp.GenreId = genreCategoryAttrs?.GenreId ?? genreClassifAttrs?.GenreId ?? 1;
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
            return systemProgramme.Where(x => genres.Split(new[] { ';' }, StringSplitOptions.RemoveEmptyEntries).Any(g => long.TryParse(g, out Gid) && long.Parse(g)  == x.GenreId)).ToList();
        }

        private List<SystemProgramme> FilterDates(List<SystemProgramme> systemProgramme, string dates)
        {
            if (string.IsNullOrWhiteSpace(dates))
                return systemProgramme;
            DateTime date;
            CultureInfo ci = new CultureInfo("Ru-ru");
            List<DateTime> rawDates = dates.Split(new[] { ';' }, StringSplitOptions.RemoveEmptyEntries).Select(x => DateTime.TryParseExact(x, "yyyyMMdd", ci, DateTimeStyles.None, out date) ? DateTime.ParseExact(x, "yyyyMMdd", ci) : new DateTime()).ToList();
            return systemProgramme.Where(x => rawDates.Any(rd => rd <= x.TsStartMo && x.TsStartMo < rd.AddDays(1))).ToList();
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

            return (from g in dataContext.Genres.AsNoTracking()
                    where g.GenreId == id && g.Uid == Uid && g.Visible && g.DeleteDate == null
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
            int[] splittedGenres = !string.IsNullOrWhiteSpace(genres) ? Array.ConvertAll(genres.Split(';'), int.Parse) : new[] { 0 };
            try
            {
                switch (mode)
                {
                    case 1:
               var sp = (from pr in dataContext.Programmes.AsNoTracking()
                         join ch in dataContext.Channels.AsNoTracking() on pr.Cid equals ch.ChannelId
                         join mp in dataContext.MediaPic.AsNoTracking() on ch.IconId equals mp.IconId into chmp
                         from mp in chmp.DefaultIfEmpty()
                         where pr.Tid == TypeProgId && pr.TsStartMo <= dateTime &&
                               dateTime < pr.TsStopMo && pr.Category != "Для взрослых" &&
                               !pr.Title.Contains("(18+)") && ch.Deleted == null &&
                               (category == null || pr.Category == category)
                         select new
                         {
                             ProgrammesId = pr.ProgrammesId,
                             Cid = pr.Cid,
                             ChannelName = ch.TitleChannel,
                             ChannelContent = (from ch2 in dataContext.Channels
                                               join mp2 in dataContext.MediaPic on ch2.IconId equals mp2.IconId into chmp2
                                               from mp2 in chmp.DefaultIfEmpty()
                                               where ch.ChannelId == ch2.ChannelId
                                               select mp2.Path25 + mp2.FileName).FirstOrDefault(),
                             InternalChanId = pr.InternalChanId ?? 0,
                             Start = pr.TsStart,
                             Stop = pr.TsStop,
                             TsStartMo = pr.TsStartMo,
                             TsStopMo = pr.TsStopMo,
                             TelecastTitle = pr.Title,
                             TelecastDescr = pr.Descr,
                             AnonsContent = ((pr.Descr != null && pr.Descr != string.Empty) ?
                                      dataContext.MediaPic.FirstOrDefault(mp => mp.FileName == "GreenAnons.png").Path25 +
                                      dataContext.MediaPic.FirstOrDefault(mp => mp.FileName == "GreenAnons.png").FileName :
                                      null),
                             Category = pr.Category,
                             Remain = (int)(EF.Functions.DateDiffSecond(pr.TsStopMo, dateTime) * 1.0 / (EF.Functions.DateDiffSecond(pr.TsStopMo, pr.TsStartMo) * 1.0) * 100.0),

                         }).Select(mapper.Map<SystemProgramme>).ToList();
                        
                        SetGenres(sp, null);
                        sp = FilterGenres(sp, genres);
                        count = sp.Count();
                        systemProgramme = sp.AsQueryable().OrderBy(!(sidx == null || sidx.Trim() == string.Empty) ? sidx : "InternalChanId", sord)
                                            .Skip((page - 1) * rows).Take(rows)
                                            .Select(mapper.Map<SystemProgramme>)
                                            .ToList<SystemProgramme>();
                        break;
                    case 2:
                        if (dateTime == minDate)
                        {
                            List<TsStopForRemain> stopAfter = (from pr2 in dataContext.Programmes.AsNoTracking()
                                                               where pr2.TsStartMo <= DateTime.Now && DateTime.Now < pr2.TsStopMo && pr2.Tid == TypeProgId
                                                                 && pr2.Category != "Для взрослых" && !pr2.Title.Contains("(18+)")
                                                               select new
                                                               {
                                                                   TsStopMoAfter = pr2.TsStopMo.AddSeconds(1),
                                                                   Cid = pr2.Cid
                                                               }).Select(mapper.Map<TsStopForRemain>).ToList();
                            DateTime afterTwoDays = DateTime.Now.AddDays(2);
                            var sp2 = (from pr3 in dataContext.Programmes.AsNoTracking()
                                       join ch in dataContext.Channels.AsNoTracking() on pr3.Cid equals ch.ChannelId
                                       join mp in dataContext.MediaPic.AsNoTracking() on ch.IconId equals mp.IconId into chmp
                                       from mp in chmp.DefaultIfEmpty()
                                       where pr3.Tid == TypeProgId
                                       && pr3.TsStartMo >= DateTime.Now && afterTwoDays > pr3.TsStopMo
                                       && pr3.Category != "Для взрослых" && !pr3.Title.Contains("(18+)")
                                       && ch.Deleted == null &&
                                         (category == null || pr3.Category == category)
                                       orderby pr3.InternalChanId, pr3.TsStartMo
                                       select new
                                       {
                                           ProgrammesId = pr3.ProgrammesId,
                                           Cid = pr3.Cid,
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
                                           dataContext.MediaPic.FirstOrDefault(mp => mp.FileName == "GreenAnons.png").Path25 + dataContext.MediaPic.FirstOrDefault(mp => mp.FileName == "GreenAnons.png").FileName :
                                           null),
                                           Category = pr3.Category,
                                           Remain = EF.Functions.DateDiffSecond(DateTime.Now, pr3.TsStartMo)
                                       }
                                               ).AsNoTracking();

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
                                       }).Select(mapper.Map<SystemProgramme>).ToList();
                            SetGenres(sp3, null);
                            sp3 = FilterGenres(sp3, genres);
                            count = sp3.Count();
                           
                            systemProgramme = sp3.AsQueryable().OrderBy(!(sidx == null || sidx.Trim() == string.Empty) ? sidx : "InternalChanId", sord)
                                            .Skip((page - 1) * rows).Take(rows)
                                            .Select(mapper.Map<SystemProgramme>)
                                            .ToList<SystemProgramme>();
                        }
                        else if (dateTime > minDate)
                        {
                            var sp4 = (from pr in dataContext.Programmes.AsNoTracking()
                                       join ch in dataContext.Channels.AsNoTracking() on pr.Cid equals ch.ChannelId
                                       join mp in dataContext.MediaPic.AsNoTracking() on ch.IconId equals mp.IconId into chmp
                                       from mp in chmp.DefaultIfEmpty()
                                       where pr.Tid == TypeProgId && DateTime.Now < pr.TsStartMo && pr.TsStartMo <= dateTime
                                       && pr.Category != "Для взрослых" && !pr.Title.Contains("(18+)")
                                       && ch.Deleted == null &&
                                         (category == null || pr.Category == category)
                                       orderby pr.InternalChanId, pr.TsStart
                                       select new
                                       {
                                           ProgrammesId = pr.ProgrammesId,
                                           Cid = pr.Cid,
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
                                           dataContext.MediaPic.FirstOrDefault(mp => mp.FileName == "GreenAnons.png").Path25 +
                                           dataContext.MediaPic.FirstOrDefault(mp => mp.FileName == "GreenAnons.png").FileName :
                                           null),
                                           Category = pr.Category,
                                           Remain = EF.Functions.DateDiffSecond(DateTime.Now, pr.TsStartMo)
                                       }).Select(mapper.Map<SystemProgramme>).ToList<SystemProgramme>();
                            SetGenres(sp4, null);
                            sp4 = FilterGenres(sp4, genres);
                            count = sp4.Count();
                            
                            systemProgramme = sp4.AsQueryable().OrderBy(!(sidx == null || sidx.Trim() == string.Empty) ? sidx : "InternalChanId", sord)
                                            .Skip((page - 1) * rows).Take(rows)
                                            .Select(mapper.Map<SystemProgramme>)
                                            .ToList<SystemProgramme>();
                        }
                        break;
                }
            }
            catch (Exception ex)
            {
            }
            return new KeyValuePair<int, List<SystemProgramme>>(count, systemProgramme);
        }

        /// <summary>
        /// Получение телепередач за день
        /// </summary>
        /// <param name="TypeProgId">Тип телепередач</param>
        /// <param name="Cid">Идентификатор канала</param>
        /// <param name="tsStart">Время начала</param>
        /// <param name="tsStop">Время окончания</param>
        public List<SystemProgramme> GetProgrammeOfDay(int TypeProgId, int Cid, DateTime tsStart, DateTime tsStop)
        {
            List<SystemProgramme> systemProgrammes = new List<SystemProgramme>();
            try
            {
                systemProgrammes = (from pr in dataContext.Programmes.AsNoTracking()
                                    join ch in dataContext.Channels.AsNoTracking() on pr.Cid equals ch.ChannelId
                                    join mp in dataContext.MediaPic.AsNoTracking() on ch.IconId equals mp.IconId into chmp
                                    from mp in chmp.DefaultIfEmpty()
                                    where pr.Tid == TypeProgId &&
                                    ch.ChannelId == Cid &&
                                    pr.TsStartMo >= tsStart &&
                                    pr.TsStopMo <= tsStop &&
                                    pr.Category != "Для взрослых" && !pr.Title.Contains("(18+)")
                                    && ch.Deleted == null
                                    select new
                                    {
                                        ProgrammesId = pr.ProgrammesId,
                                        Cid = pr.Cid,
                                        ChannelName = ch.TitleChannel,
                                        InternalChanId = pr.InternalChanId,
                                        Start = pr.TsStart,
                                        Stop = pr.TsStop,
                                        TsStartMo = pr.TsStartMo,
                                        TsStopMo = pr.TsStopMo,
                                        TelecastTitle = pr.Title,
                                        TelecastDescr = pr.Descr,
                                        AnonsContent = ((pr.Descr != null && pr.Descr != string.Empty) ?
                                        dataContext.MediaPic.FirstOrDefault(mp => mp.FileName == "GreenAnons.png").Path25 +
                                        dataContext.MediaPic.FirstOrDefault(mp => mp.FileName == "GreenAnons.png").FileName:
                                                   null),
                                        Category = pr.Category,
                                    }).Select(mapper.Map<SystemProgramme>).ToList<SystemProgramme>();
            }
            catch (Exception ex)
            {
                Logger.Error(ex, ex.Message);
            }
            return systemProgrammes;
        }

        /// <summary>
        /// Получение пользовательской телепрограммы
        /// </summary>
        /// <param name="Uid">Идентификатор пользователя</param>
        /// <param name="TypeProgId">Тип программы телепередач</param>
        /// <param name="dateTimeOffset">Время</param>
        /// <param name="mode">Режим: 1 - сейчас, 2 - затем</param>
        /// <param name="category">Категория телепередач</param>
        public KeyValuePair<int, List<SystemProgramme>> GetUserProgrammes(long Uid, int TypeProgId, DateTimeOffset dateTimeOffset, int mode, string category,
                                                         string sidx, string sord, int page, int rows, string genres)
        {
            List<SystemProgramme> systemProgramme = new List<SystemProgramme>();
            DateTime minDate = new DateTime(1800, 1, 1);
            DateTime dateTime = dateTimeOffset.DateTime;
            int count = 0;
            try
            {
                switch (mode)
                {
                    case 1:
                        systemProgramme = (from pr in dataContext.Programmes.AsNoTracking()
                                           join ch in dataContext.Channels.AsNoTracking() on pr.Cid equals ch.ChannelId
                                           join uch in dataContext.UserChannels.AsNoTracking() on ch.ChannelId equals uch.Cid
                                           join mp in dataContext.MediaPic.AsNoTracking() on uch.IconId equals mp.IconId into mpch
                                           from mp in mpch.DefaultIfEmpty()
                                           where pr.Tid == TypeProgId && pr.TsStartMo <= dateTime &&
                                           dateTime < pr.TsStopMo && uch.Uid == Uid
                                           && ch.Deleted == null
                                           && (category == null || pr.Category == category)
                                           orderby pr.InternalChanId, pr.TsStartMo
                                           select new
                                           {
                                               ProgrammesId = pr.ProgrammesId,
                                               Cid = pr.Cid,
                                               ChannelName = (string.IsNullOrEmpty(uch.DisplayName) ? ch.TitleChannel : uch.DisplayName),
                                               InternalChanId = pr.InternalChanId ?? 0,
                                               ChannelContent = mp.Path25 + mp.FileName,
                                               Start = pr.TsStart,
                                               Stop = pr.TsStop,
                                               TsStartMo = pr.TsStartMo,
                                               TsStopMo = pr.TsStopMo,
                                               TelecastTitle = pr.Title,
                                               TelecastDescr = pr.Descr,
                                               OrderCol = uch.OrderCol,
                                               AnonsContent = ((pr.Descr != null && pr.Descr != string.Empty) ?
                                               dataContext.MediaPic.FirstOrDefault(mp => mp.FileName == "GreenAnons.png").Path25 +
                                               dataContext.MediaPic.FirstOrDefault(mp => mp.FileName == "GreenAnons.png").FileName :
                                               null),
                                               Category = pr.Category,
                                               Remain = (int)(EF.Functions.DateDiffSecond(pr.TsStopMo, dateTime) * 1.0 / (EF.Functions.DateDiffSecond(pr.TsStopMo, pr.TsStartMo) * 1.0) * 100.0),
                                           }).ToList().DistinctBy(x => x.ProgrammesId).OrderBy(x => x.OrderCol).Select(mapper.Map<SystemProgramme>).ToList();
                        SetGenres(systemProgramme, Uid);
                        systemProgramme = FilterGenres(systemProgramme, genres);
                        count = systemProgramme.Count();
                        SetRatings(systemProgramme, Uid);
                        systemProgramme = systemProgramme.AsQueryable().OrderBy(!(sidx == null || sidx.Trim() == string.Empty) ? sidx : "InternalChanId", sord)
                                            .Skip((page - 1) * rows).Take(rows)
                                            .Select(mapper.Map<SystemProgramme>)
                                            .ToList<SystemProgramme>();
                        break;
                    case 2:
                        if (dateTime == minDate)
                        {
                            List<TsStopForRemain> stopAfter = (from pr2 in dataContext.Programmes.AsNoTracking()
                                                               join ch2 in dataContext.Channels.AsNoTracking() on pr2.Cid equals ch2.ChannelId
                                                               join uch2 in dataContext.UserChannels.AsNoTracking() on ch2.ChannelId equals uch2.Cid
                                                               where pr2.TsStartMo <= DateTime.Now && DateTime.Now < pr2.TsStopMo && pr2.Tid == TypeProgId
                                                               select new
                                                               {
                                                                   TsStopMoAfter = pr2.TsStopMo.AddSeconds(1),
                                                                   Cid = pr2.Cid
                                                               }).Select(mapper.Map<TsStopForRemain>).ToList();
                            DateTime afterTwoDays = DateTime.Now.AddDays(2);
                            systemProgramme = (from pr in dataContext.Programmes.AsNoTracking()
                                               join ch in dataContext.Channels.AsNoTracking() on pr.Cid equals ch.ChannelId
                                               join uch in dataContext.UserChannels.AsNoTracking() on ch.ChannelId equals uch.Cid
                                               join mp in dataContext.MediaPic.AsNoTracking() on uch.IconId equals mp.IconId into mpch
                                               from mp in mpch.DefaultIfEmpty()
                                               where pr.Tid == TypeProgId && pr.TsStartMo >= DateTime.Now && afterTwoDays > pr.TsStopMo
                                               && uch.Uid == Uid
                                               && ch.Deleted == null
                                               && (category == null || pr.Category == category)
                                               orderby pr.InternalChanId, pr.TsStartMo
                                               select new
                                               {
                                                   ProgrammesId = pr.ProgrammesId,
                                                   Cid = pr.Cid,
                                                   ChannelName = (string.IsNullOrEmpty(uch.DisplayName) ? ch.TitleChannel : uch.DisplayName),
                                                   ChannelContent = mp.Path25 + mp.FileName,
                                                   InternalChanId = pr.InternalChanId ?? 0,
                                                   Start = pr.TsStart,
                                                   Stop = pr.TsStop,
                                                   TsStartMo = pr.TsStartMo,
                                                   TsStopMo = pr.TsStopMo,
                                                   TelecastTitle = pr.Title,
                                                   Title = pr.Title,
                                                   TelecastDescr = pr.Descr,
                                                   AnonsContent = ((pr.Descr != null && pr.Descr != string.Empty) ?
                                                   dataContext.MediaPic.FirstOrDefault(mp => mp.FileName == "GreenAnons.png").Path25 +
                                                   dataContext.MediaPic.FirstOrDefault(mp => mp.FileName == "GreenAnons.png").FileName :
                                                   null),
                                                   OrderCol = uch.OrderCol,
                                                   Category = pr.Category,
                                                   Remain = EF.Functions.DateDiffSecond(DateTime.Now, pr.TsStartMo),

                                               }).AsNoTracking().OrderBy(x => x.OrderCol).ToList().Select(mapper.Map<SystemProgramme>).ToList();
                            systemProgramme = (from pr in systemProgramme
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
                                               }).ToList().DistinctBy(x => x.ProgrammesId).Select(mapper.Map<SystemProgramme>).ToList(); ;
                            SetGenres(systemProgramme, Uid);
                            systemProgramme = FilterGenres(systemProgramme, genres);
                            SetRatings(systemProgramme, Uid);
                            count = systemProgramme.Count();
                            systemProgramme = systemProgramme.AsQueryable().OrderBy(!(sidx == null || sidx.Trim() == string.Empty) ? sidx : "InternalChanId", sord)
                                            .Skip((page - 1) * rows).Take(rows)
                                            .Select(mapper.Map<SystemProgramme>)
                                            .ToList<SystemProgramme>();
                        }
                        else if (dateTime > minDate)
                        {
                            systemProgramme = (from pr in dataContext.Programmes.AsNoTracking()
                                               join ch in dataContext.Channels.AsNoTracking() on pr.Cid equals ch.ChannelId
                                               join uch in dataContext.UserChannels.AsNoTracking() on ch.ChannelId equals uch.Cid
                                               join mp in dataContext.MediaPic.AsNoTracking() on uch.IconId equals mp.IconId into mpch
                                               from mp in mpch.DefaultIfEmpty()
                                               where DateTime.Now < pr.TsStartMo && pr.TsStartMo <= dateTime
                                               && uch.Uid == Uid
                                               && ch.Deleted == null &&
                                               (category == null || pr.Category == category)
                                               orderby pr.InternalChanId, pr.TsStartMo
                                               select new
                                               {
                                                   ProgrammesId = pr.ProgrammesId,
                                                   Cid = pr.Cid,
                                                   ChannelName = (string.IsNullOrEmpty(uch.DisplayName) ? ch.TitleChannel : uch.DisplayName),
                                                   ChannelContent = mp.Path25 + mp.FileName,
                                                   InternalChanId = pr.InternalChanId ?? 0,
                                                   Start = pr.TsStart,
                                                   Stop = pr.TsStop,
                                                   TsStartMo = pr.TsStartMo,
                                                   TsStopMo = pr.TsStopMo,
                                                   TelecastTitle = pr.Title,
                                                   TelecastDescr = pr.Descr,
                                                   AnonsContent = ((pr.Descr != null && pr.Descr != string.Empty) ?
                                                   dataContext.MediaPic.FirstOrDefault(mp => mp.FileName == "GreenAnons.png").Path25 +
                                                   dataContext.MediaPic.FirstOrDefault(mp => mp.FileName == "GreenAnons.png").FileName :
                                                   null),
                                                   OrderCol = uch.OrderCol,
                                                   Category = pr.Category,
                                                   Remain = EF.Functions.DateDiffSecond(DateTime.Now, pr.TsStartMo),
                                               }
                                               ).AsNoTracking().OrderBy(o => o.OrderCol).ToList().Select(mapper.Map<SystemProgramme>).ToList();
                            SetGenres(systemProgramme, Uid);
                            systemProgramme = FilterGenres(systemProgramme, genres);
                            count = systemProgramme.Count();
                            SetRatings(systemProgramme, Uid);
                            systemProgramme = systemProgramme.AsQueryable().OrderBy(!(sidx == null || sidx.Trim() == string.Empty) ? sidx : "InternalChanId", sord)
                                            .Skip((page - 1) * rows).Take(rows)
                                            .Select(mapper.Map<SystemProgramme>)
                                            .ToList<SystemProgramme>();
                        }
                        break;
                }
            }
            catch (Exception ex)
            {
                Logger.Error(ex, ex.Message);
            }
            return new KeyValuePair<int, List<SystemProgramme>>(count, systemProgramme);
        }
        
        /// <summary>
        /// Получение пользовательских телепередач за день
        /// </summary>
        /// <param name="Uid">Идентификатор пользователя</param>
        /// <param name="TypeProgId">Тип программы телепередач</param>
        /// <param name="Cid">Идентификатор телеканала</param>
        /// <param name="tsStart">Время начала выборки телепередач</param>
        /// <param name="tsStop">Время окончания выборки телепередач</param>
        /// <param name="category">Категория телепередач</param>
        public List<SystemProgramme> GetUserProgrammeOfDay(long Uid, int TypeProgId, int Cid, DateTime tsStart, DateTime tsStop, string category)
        {
            List<SystemProgramme> systemProgramme = null;

            try
            {
                systemProgramme = (from pr in dataContext.Programmes.AsNoTracking()
                                   join ch in dataContext.Channels.AsNoTracking() on pr.Cid equals ch.ChannelId
                                   join uch in dataContext.UserChannels.AsNoTracking() on ch.ChannelId equals uch.Cid
                                   join mp in dataContext.MediaPic.AsNoTracking() on uch.IconId equals mp.IconId into mpch
                                   from mp in mpch.DefaultIfEmpty()
                                   where pr.Tid == TypeProgId &&
                                         ch.ChannelId == Cid &&
                                         pr.TsStartMo >= tsStart &&
                                         pr.TsStopMo < tsStop &&
                                         uch.Uid == Uid
                                         && ch.Deleted == null
                                         && (category == null || pr.Category == category)
                                   orderby uch.OrderCol, pr.TsStartMo
                                   select new
                                   {
                                       ProgrammesId = pr.ProgrammesId,
                                       Cid = pr.Cid,
                                       ChannelName = ch.TitleChannel,
                                       InternalChanId = pr.InternalChanId,
                                       ChannelContent = mp.Path25 + mp.FileName,
                                       Start = pr.TsStart,
                                       TsStartMo = pr.TsStartMo,
                                       Stop = pr.TsStop,
                                       TsStopMo = pr.TsStopMo,
                                       TelecastTitle = pr.Title,
                                       TelecastDescr = pr.Descr,
                                       AnonsContent = ((pr.Descr != null && pr.Descr != string.Empty) ?
                                       dataContext.MediaPic.FirstOrDefault(mp => mp.FileName == "GreenAnons.png").Path25 +
                                       dataContext.MediaPic.FirstOrDefault(mp => mp.FileName == "GreenAnons.png").FileName :
                                                                   null),
                                       OrderCol = uch.OrderCol,
                                       Category = pr.Category,
                                   }).ToList().DistinctBy(x => x.ProgrammesId).Select(mapper.Map<SystemProgramme>).ToList();
                SetGenres(systemProgramme, Uid);
                SetRatings(systemProgramme, Uid);
            }
            catch(Exception ex)
            {
                Logger.Error(ex, ex.Message);
            }
            return systemProgramme;
        }

        /// <summary>
        /// Поиск телепередачи
        /// </summary>
        /// <param name="TypeProgId">Тип программы телепередач</param>
        /// <param name="findTitle">Поисковая подстрока</param>
        public KeyValuePair<int, List<SystemProgramme>> SearchProgramme(int TypeProgId, string findTitle, string category,
                                                         string sidx, string sord, int page, int rows, string genres, string dates)
        {
            List<SystemProgramme> systemProgramme = new List<SystemProgramme>();
            int count = 0;
            try
            {
                systemProgramme = (from pr in dataContext.Programmes.AsNoTracking()
                                   join ch in dataContext.Channels.AsNoTracking() on pr.Cid equals ch.ChannelId
                                   join mp in dataContext.MediaPic.AsNoTracking() on ch.IconId equals mp.IconId into chmp
                                   from mp in chmp.DefaultIfEmpty()
                                   where pr.Tid == TypeProgId
                                        && pr.Category != "Для взрослых" && !pr.Title.Contains("(18+)")
                                  //      && (category == null || pr.Category == category)
                                        && ch.Deleted == null
                                        && pr.Title.Contains(findTitle)
                                   orderby pr.TsStartMo, pr.TsStopMo
                                   select new
                                   {
                                       ProgrammesId = pr.ProgrammesId,
                                       Cid = pr.Cid,
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
                                       dataContext.MediaPic.FirstOrDefault(mp => mp.FileName == "GreenAnons.png").Path25 + 
                                       dataContext.MediaPic.FirstOrDefault(mp => mp.FileName == "GreenAnons.png").FileName :
                                 null),
                                       Category = pr.Category,
                                       Remain = 1
                                   }).Select(mapper.Map<SystemProgramme>).ToList();
                systemProgramme.ForEach(pr =>
                {
                    pr.DayMonth = pr.TsStartMo.ToString("ddd", new CultureInfo("ru-Ru")) +
               String.Format("({0:D2}.{1:D2})", pr.TsStartMo.Day, pr.TsStartMo.Month);
                });
                SetGenres(systemProgramme, null);
                systemProgramme = FilterGenres(systemProgramme, genres);
                systemProgramme = FilterDates(systemProgramme, dates);
                count = systemProgramme.Count();

                systemProgramme = systemProgramme.AsQueryable().OrderBy(!(sidx == null || sidx.Trim() == string.Empty) ? sidx : "TsStartMo", sord)
                                             .Skip((page - 1) * rows).Take(rows)
                                             .Select(mapper.Map<SystemProgramme>)
                                             .ToList<SystemProgramme>();

            }
            catch (Exception ex)
            {
            }
            return new KeyValuePair<int, List<SystemProgramme>>(count, systemProgramme); 
        }

        /// <summary>
        /// Поиск телепередач по пользовательской (кастомизированной) программе
        /// </summary>
        /// <param name="Uid">Идентификатор пользователя</param>
        /// <param name="TypeProgId">Тип программы телепередач</param>
        /// <param name="findTitle">Поисковая подстрока</param>
        public KeyValuePair<int, List<SystemProgramme>> SearchUserProgramme(long Uid, int TypeProgId, string findTitle, string category,
                                                         string sidx, string sord, int page, int rows, string genres, string dates)
        {
            List<SystemProgramme> systemProgramme = new List<SystemProgramme>();
            int count = 0;
            try
            {
                systemProgramme = (from pr in dataContext.Programmes.AsNoTracking()
                                   join ch in dataContext.Channels.AsNoTracking() on pr.Cid equals ch.ChannelId
                                   join uch in dataContext.UserChannels.AsNoTracking() on ch.ChannelId equals uch.Cid
                                   join mp in dataContext.MediaPic.AsNoTracking() on uch.IconId equals mp.IconId into chmp
                                   from mp in chmp.DefaultIfEmpty()
                                   where uch.Uid == Uid 
                                        && pr.Tid == TypeProgId
                                        && ch.Deleted == null
                                        && pr.Title.Contains(findTitle)
                                   orderby pr.TsStartMo, pr.TsStopMo
                                   select new
                                   {
                                       ProgrammesId = pr.ProgrammesId,
                                       Cid = pr.Cid,
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
                                       dataContext.MediaPic.FirstOrDefault(mp => mp.FileName == "GreenAnons.png").Path25 + 
                                       dataContext.MediaPic.FirstOrDefault(mp => mp.FileName == "GreenAnons.png").FileName :
                                 null),
                                       Category = pr.Category,
                                       Remain = 1
                                   }).Select(mapper.Map<SystemProgramme>).ToList();
                systemProgramme.ForEach(pr =>
                {
                    pr.DayMonth = pr.TsStartMo.ToString("ddd", new CultureInfo("ru-Ru")) +
                    String.Format("({0:D2}.{1:D2})", pr.TsStartMo.Day, pr.TsStartMo.Month);
                });

                SetGenres(systemProgramme, Uid);
                SetRatings(systemProgramme, Uid);
                systemProgramme = FilterGenres(systemProgramme, genres);
                systemProgramme = FilterDates(systemProgramme, dates);
                count = systemProgramme.Count();

                systemProgramme = systemProgramme.AsQueryable().OrderBy(!(sidx == null || sidx.Trim() == string.Empty) ? sidx : "TsStartMo", sord)
                                             .Skip((page - 1) * rows).Take(rows)
                                             .Select(mapper.Map<SystemProgramme>)
                                             .ToList<SystemProgramme>();
            }
            catch (Exception ex)
            {
            }
            return new KeyValuePair<int, List<SystemProgramme>>(count, systemProgramme); 
        }*/
        
        #endregion
    }
    
}