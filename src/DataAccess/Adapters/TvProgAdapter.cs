using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using TVProgViewer.BusinessLogic.ProgObjs;
using TVProgViewer.Common;
using MoreLinq;
using System.Globalization;



namespace TVProgViewer.DataAccess.Adapters
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
                return (from tpp in dataContext.TVProgProviders.AsNoTracking()
                        join tp in dataContext.TypeProg.AsNoTracking() on tpp.TVProgProviderID equals tp.TVProgProviderID
                        select new
                        {
                            TVProgProviderID = tpp.TVProgProviderID,
                            ProviderName = tpp.ProviderName,
                            ProviderText = tpp.ProviderWebSite,
                            TypeProgID = tp.TypeProgID,
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

        /// <summary>
        /// Получение периода действия программы телепередач
        /// </summary>
        /// <param name="typeProgID">Идентификатор тип программы</param>
        /// <returns></returns>
        public ProgPeriod GetSystemProgrammePeriod(int typeProgID)
        {
            ProgPeriod progPeriod = new ProgPeriod()
            {
                dtStart = dataContext.Programmes.Where(x => x.TID == typeProgID).Min(x => x.TsStart),
                dtEnd = dataContext.Programmes.Where(x => x.TID == typeProgID).Max(x => x.TsStop)
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
        /// <param name="tvProgProviderID">Провайдер телепрограммы</param>
        public List<SystemChannel> GetSystemChannels(int tvProgProviderID)
        {

            List<SystemChannel> listSystemChannels = new List<SystemChannel>();
            try
            {
                listSystemChannels = (from ch in dataContext.Channels.AsNoTracking()
                                      join mp in dataContext.MediaPic.AsNoTracking() on ch.IconID equals mp.IconID into chmp
                                      from mp in chmp.DefaultIfEmpty()
                                      where ch.TVProgProviderID == tvProgProviderID && ch.Deleted == null
                                      select new
                                      {
                                          ChannelID = ch.ChannelID,
                                          TVProgViewerID = ch.TVProgProviderID,
                                          InternalID = ch.InternalID,
                                          IconID = ch.IconID,
                                          FileNameOrig = mp.PathOrig + mp.FileName,
                                          FileName25 = mp.Path25 + mp.FileName,
                                          Title = ch.TitleChannel,
                                          ImageWebSrc = ch.IconWebSrc,
                                          OrderCol = 0
                                      }).AsNoTracking().Select(mapper.Map<SystemChannel>).OrderBy(o => o.InternalID).ToList();

            }
            catch (Exception ex)
            {
            }
            return listSystemChannels;
        }

        /// <summary>
        /// Получение пользовательских телеканалов
        /// </summary>
        /// <param name="uid">Идентификатор пользователя</param>
        /// <param name="typeProgID">Тип программы телепередач</param>
        public List<UserChannel> GetUserChannels(long uid, int typeProgID)
        {

            List<UserChannel> listUserChannels = new List<UserChannel>();
            try
            {
                listUserChannels = (from ch in dataContext.Channels.AsNoTracking()
                                    join uch in dataContext.UserChannels.AsNoTracking() on ch.ChannelID equals uch.CID
                                    join mp in dataContext.MediaPic.AsNoTracking() on uch.IconID equals mp.IconID into chmp
                                    from mp in chmp.DefaultIfEmpty()
                                    where uch.UID == uid &&
                                    ch.TVProgProviderID == typeProgID
                                    && ch.Deleted == null
                                    orderby uch.OrderCol
                                    select new
                                    {
                                        UserChannelID = uch.UserChannelID,
                                        ChannelID = uch.CID,
                                        TVProgViewerID = ch.TVProgProviderID,
                                        ChannelName = ch.TitleChannel,
                                        InternalID = ch.InternalID,
                                        IconID = uch.IconID,
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
        /// <param name="uid">Идентификатор пользователя</param>
        /// <param name="tvProgProviderID">Идентификатор провайдера программы телепередач</param>
        /// <param name="typeProgID">Тип программы телепередач</param>
        public List<SystemChannel> GetUserInSystemChannels(long uid, int tvProgProviderID, int typeProgID)
        {
            List<SystemChannel> systemChannels = GetSystemChannels(tvProgProviderID).ToList();
            List<UserChannel> userChannels = GetUserChannels(uid, typeProgID).ToList();
            systemChannels.ForEach(sch =>
            {
                UserChannel userChannel = userChannels.Find(uch => uch.ChannelID == sch.ChannelID);
                sch.Visible = userChannel != null;
                if (sch.Visible)
                {
                    sch.UserChannelID = userChannel.UserChannelID;
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
        /// <param name="userChannelID">Идентификатор телеканала</param>
        /// <param name="uid">Идентификатор пользователя</param>
        /// <param name="tvProgProviderID">Идентификатор источника программы телепередач</param>
        /// <param name="cid">Идентификатор системного телеканала</param>
        /// <param name="displayName">Пользовательское название</param>
        /// <param name="orderCol">Порядковый номер</param>
        public void InsertUserChannel(int userChannelID, long uid,
                            int tvProgProviderID, int cid, string displayName, int orderCol)
        {
            try
            {
                int qty = dataContext.UserChannels.Count(x => x.UID == uid && x.CID == cid);
                if (qty == 0)
                {
                    Channels channel = dataContext.Channels.AsNoTracking().Single(ch => ch.TVProgProviderID == tvProgProviderID && ch.ChannelID == cid && ch.Deleted == null);
                    UserChannels userChannel = new UserChannels()
                    {
                        UID = uid,
                        CID = channel.ChannelID,
                        IconID = channel.IconID,
                        DisplayName = string.IsNullOrWhiteSpace(displayName) ? channel.TitleChannel : displayName,
                        OrderCol = orderCol
                    };
                    dataContext.UserChannels.Add(userChannel);
                    dataContext.SaveChanges();
                }
                else
                    UpdateUserChannel(userChannelID, cid, displayName, orderCol);
            }
            catch (Exception ex)
            {

            }
        }

        /// <summary>
        /// Удаление пользовательского телеканала
        /// </summary>
        /// <param name="uid">Идентификатор пользователя</param>
        /// <param name="cid">Системный идентификатор телеканала</param>
        public void DeleteUserChannel(long uid, int cid)
        {
            try
            {
                UserChannels userChannel = dataContext.UserChannels.SingleOrDefault(x => x.UID == uid && x.CID == cid);
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
        /// <param name="userChannelID">Идентификатор телеканала</param>
        /// <param name="cid">Системный идентификатор телеканала</param>
        /// <param name="displayName">Пользовательское название телеканала</param>
        /// <param name="orderCol">Порядковый номер телеканала</param>
        public void UpdateUserChannel(int userChannelID, int cid, string displayName, int orderCol)
        {
            try
            {
                UserChannels channel = dataContext.UserChannels.Single(x => x.UserChannelID == userChannelID);
                channel.CID = cid;
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
        /// <param name="uid">Идентификатор пользователя</param>
        /// <param name="userChannelId">Идентификатор пользовательского телеканала</param>
        /// <param name="filename">Название файла</param>
        /// <param name="contentType">Тип содержимого</param>
        /// <param name="length">Размер большой пиктограммы в байтах</param>
        /// <param name="length25">Размер маленькой пиктограммы в байтах</param>
        /// <param name="pathOrig">Путь к большой пиктограмме</param>
        /// <param name="path25">Путь к маленькой пиктограмме</param>
        public void ChangeChannelImage(long uid, int userChannelId, string filename, string contentType,
            int length, int length25, string pathOrig, string path25)
        {
            try
            {
                MediaPic mp = dataContext.MediaPic.Add(new MediaPic()
                {
                    FileName = filename,
                    ContentType = contentType,
                    ContentCoding = "gzip",
                    Length = length,
                    Length25 = length25,
                    IsSystem = false,
                    PathOrig = pathOrig,
                    Path25 = path25
                });

                UserChannels channel = dataContext.UserChannels.Single(x => x.UserChannelID == userChannelId);
                channel.IconID = mp.IconID;
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
                sp.RatingName = (from rc in dataContext.RatingClassificator.AsNoTracking()
                                 join r in dataContext.Ratings.AsNoTracking() on rc.RID equals r.RatingID
                                 where r.UID == uid && rc.UID == uid && r.Visible && r.DeleteDate == null &&
                                 (rc.DeleteAfterDate == null || rc.DeleteAfterDate > DateTime.Now)
                                 orderby rc.OrderCol
                                 select new { r.RatingName, rc.ContainPhrases, rc.NonContainPhrases })
                                 .ToList()
                                 .Where(rc => sp.TelecastTitle.ContainsAny(rc.ContainPhrases.Split(';')) &&
                                        (string.IsNullOrWhiteSpace(rc.NonContainPhrases) ||
                                         (!string.IsNullOrWhiteSpace(rc.NonContainPhrases) &&
                                         !sp.TelecastTitle.ContainsAny(rc.NonContainPhrases.Split(';')))))
                                  .Select(r => r.RatingName).FirstOrDefault() ?? "Без рейтинга";
                sp.RatingContent = (from rc in dataContext.RatingClassificator.AsNoTracking()
                                    join r in dataContext.Ratings.AsNoTracking() on rc.RID equals r.RatingID
                                    join mp2 in dataContext.MediaPic.AsNoTracking() on r.IconID equals mp2.IconID into rmp2
                                    from mp2 in rmp2.DefaultIfEmpty()
                                    where r.UID == uid && rc.UID == uid && r.Visible && r.DeleteDate == null &&
                                       (rc.DeleteAfterDate == null || rc.DeleteAfterDate > DateTime.Now)
                                    orderby rc.OrderCol
                                    select new { mp2.Path25, mp2.FileName, rc.ContainPhrases, rc.NonContainPhrases })
                                                 .ToList()
                                                 .Where(rc => sp.TelecastTitle.ContainsAny(rc.ContainPhrases.Split(';'))
                                                        && (string.IsNullOrWhiteSpace(rc.NonContainPhrases) ||
                                                        (!string.IsNullOrWhiteSpace(rc.NonContainPhrases) &&
                                                        !sp.TelecastTitle.ContainsAny(rc.NonContainPhrases.Split(';')))))
                                                 .Select(mp2 => mp2.Path25 + mp2.FileName).FirstOrDefault() ?? (from m in dataContext.MediaPic.AsNoTracking()
                                                                                                                where m.FileName == "favempty.png" && m.IsSystem
                                                                                                                select m.Path25 + m.FileName).Single();
            }
        }

        private void SetGenres(List<SystemProgramme> systemProgramme, long? uid)
        {
            foreach (SystemProgramme sp in systemProgramme)
            {
                sp.GenreName = (from gc in dataContext.GenreClassificator.AsNoTracking()
                                join g in dataContext.Genres.AsNoTracking() on gc.GID equals g.GenreID
                                where sp.Category == g.GenreName && g.UID == uid && gc.UID == uid && g.Visible && g.DeleteDate == null &&
                                (gc.DeleteAfterDate == null || gc.DeleteAfterDate > DateTime.Now)
                                orderby gc.OrderCol
                                select g.GenreName).FirstOrDefault() ??
                                               (from gc in dataContext.GenreClassificator.AsNoTracking()
                                                join g in dataContext.Genres.AsNoTracking() on gc.GID equals g.GenreID
                                                where g.UID == uid && gc.UID == uid && g.Visible && g.DeleteDate == null &&
                                                (gc.DeleteAfterDate == null || gc.DeleteAfterDate > DateTime.Now)
                                                orderby gc.OrderCol
                                                select new { g.GenreName, gc.ContainPhrases, gc.NonContainPhrases })
                                                .ToList()
                                                .Where(gc => sp.TelecastTitle.ContainsAny(gc.ContainPhrases.Split(';')) &&
                                                            (string.IsNullOrWhiteSpace(gc.NonContainPhrases) ||
                                                            (!string.IsNullOrWhiteSpace(gc.NonContainPhrases) &&
                                                            !sp.TelecastTitle.ContainsAny(gc.NonContainPhrases.Split(';')))))
                                                .Select(g => g.GenreName).FirstOrDefault() ?? string.Empty;
                sp.GenreContent = (from gc in dataContext.GenreClassificator.AsNoTracking()
                                   join g in dataContext.Genres.AsNoTracking() on gc.GID equals g.GenreID
                                   join mp2 in dataContext.MediaPic.AsNoTracking() on g.IconID equals mp2.IconID into gmp2
                                   from mp2 in gmp2.DefaultIfEmpty()
                                   where sp.Category == g.GenreName && g.UID == uid && gc.UID == uid && g.Visible && g.DeleteDate == null &&
                                    (gc.DeleteAfterDate == null || gc.DeleteAfterDate > DateTime.Now)
                                   orderby gc.OrderCol
                                   select mp2.Path25 + mp2.FileName).FirstOrDefault() ??
                                               (from gc in dataContext.GenreClassificator.AsNoTracking()
                                                join g in dataContext.Genres.AsNoTracking() on gc.GID equals g.GenreID
                                                join mp2 in dataContext.MediaPic.AsNoTracking() on g.IconID equals mp2.IconID into gmp2
                                                from mp2 in gmp2.DefaultIfEmpty()
                                                where g.UID == uid && gc.UID == uid && g.Visible && g.DeleteDate == null &&
                                                (gc.DeleteAfterDate == null || gc.DeleteAfterDate > DateTime.Now)
                                                orderby gc.OrderCol
                                                select new { mp2.Path25, mp2.FileName, gc.ContainPhrases, gc.NonContainPhrases })
                                                .ToList()
                                                .Where(gc => sp.TelecastTitle.ContainsAny(gc.ContainPhrases.Split(';'))
                                                       && (string.IsNullOrWhiteSpace(gc.NonContainPhrases) ||
                                                       (!string.IsNullOrWhiteSpace(gc.NonContainPhrases) &&
                                                       !sp.TelecastTitle.ContainsAny(gc.NonContainPhrases.Split(';')))))
                                                .Select(mp2 => mp2.Path25 + mp2.FileName).FirstOrDefault() ?? null;
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

            return systemProgramme.Where(x => genres.Split(';').Any(g => GetNameByIdGenre(g) == x.GenreName)).ToList();
        }

       /// <summary>
       /// Получение названия жанра через идентификатор
       /// </summary>
       /// <param name="idStr">Идентификатор жанра</param>
        private string GetNameByIdGenre(string idStr)
        {  
            long id;

            if (!long.TryParse(idStr, out id))
                return string.Empty;

            return (from g in dataContext.Genres.AsNoTracking()
                    where g.GenreID == id && g.Visible && g.DeleteDate == null
                    select g.GenreName).First(); 
        }
        /// <summary>
        /// Выборка телепрограммы
        /// </summary>
        /// <param name="typeProgID">Тип программы телепередач</param>
        /// <param name="dateTimeOffset">Время</param>
        /// <param name="mode">режимы выборки: 1 - сейчас; 2 - следом</param>
        public KeyValuePair<int, List<SystemProgramme>> GetSystemProgrammes(int typeProgID, DateTimeOffset dateTimeOffset, int mode, string category, 
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
                         var sp = (from pr in dataContext.Programmes.AsNoTracking()
                                  join ch in dataContext.Channels.AsNoTracking() on pr.CID equals ch.ChannelID
                                  join mp in dataContext.MediaPic.AsNoTracking() on ch.IconID equals mp.IconID into chmp
                                  from mp in chmp.DefaultIfEmpty()
                                  where pr.TID == typeProgID && pr.TsStartMO <= dateTime &&
                                  dateTime < pr.TsStopMO && pr.Category != "Для взрослых" &&
                                  !pr.Title.Contains("(18+)") && ch.Deleted == null &&
                                  (category == null || pr.Category == category)
                                  orderby pr.InternalChanID, pr.TsStartMO
                                  select new
                                         {
                                            ProgrammesID = pr.ProgrammesID,
                                            CID = pr.CID,
                                            ChannelName = ch.TitleChannel,
                                            ChannelContent = (from ch2 in dataContext.Channels
                                                              join mp2 in dataContext.MediaPic on ch2.IconID equals mp2.IconID into chmp2
                                                              from mp2 in chmp.DefaultIfEmpty()
                                                              where ch.ChannelID == ch2.ChannelID
                                                              select mp2.Path25 + mp2.FileName).FirstOrDefault(),
                                              
                                            InternalChanID = pr.InternalChanID ?? 0,
                                            Start = pr.TsStart,
                                            Stop = pr.TsStop,
                                            TsStartMO = pr.TsStartMO,
                                            TsStopMO = pr.TsStopMO,
                                            TelecastTitle = pr.Title,
                                            TelecastDescr = pr.Descr,
                                            AnonsContent = ((pr.Descr != null && pr.Descr != string.Empty) ?
                                            dataContext.MediaPic.FirstOrDefault(mp => mp.FileName == "GreenAnons.png").Path25 +
                                            dataContext.MediaPic.FirstOrDefault(mp => mp.FileName == "GreenAnons.png").FileName :
                                            null),
                                            Category = pr.Category,
                                            Remain = (int)(DbFunctions.DiffSeconds(pr.TsStopMO, dateTime) * 1.0 / (DbFunctions.DiffSeconds(pr.TsStopMO, pr.TsStartMO) * 1.0) * 100.0)
                                          });

                        count = sp.Count();

                        if (!string.IsNullOrWhiteSpace(sidx))
                             systemProgramme = LinqExtensions.OrderBy(sp.AsQueryable(), sidx, sord).Skip((page - 1) * rows).Take(rows).Select(mapper.Map<SystemProgramme>).ToList<SystemProgramme>();
                        else systemProgramme = sp.Skip((page - 1) * rows).Take(rows).Select(mapper.Map<SystemProgramme>).ToList<SystemProgramme>();

                        SetGenres(systemProgramme, null);

                        systemProgramme = FilterGenres(systemProgramme, genres);

                        break;
                    case 2:
                        if (dateTime == minDate)
                        {
                            List<TsStopForRemain> stopAfter = (from pr2 in dataContext.Programmes.AsNoTracking()
                             where pr2.TsStartMO <= DateTime.Now && DateTime.Now < pr2.TsStopMO && pr2.TID == typeProgID 
                               && pr2.Category != "Для взрослых" && !pr2.Title.Contains("(18+)")
                             select new
                             {
                                 TsStopMOAfter = DbFunctions.AddSeconds(pr2.TsStopMO, 1)
                               , CID = pr2.CID
                             }).Select(mapper.Map<TsStopForRemain>).ToList();
                            DateTime afterTwoDays = DateTime.Now.AddDays(2);
                            var sp2 = (from pr3 in dataContext.Programmes.AsNoTracking()
                                               join ch in dataContext.Channels.AsNoTracking() on pr3.CID equals ch.ChannelID
                                               join mp in dataContext.MediaPic.AsNoTracking() on ch.IconID equals mp.IconID into chmp
                                               from mp in chmp.DefaultIfEmpty()
                                               where pr3.TID == typeProgID
                                               && pr3.TsStartMO >= DateTime.Now && afterTwoDays > pr3.TsStopMO
                                               && pr3.Category != "Для взрослых"  && !pr3.Title.Contains("(18+)")
                                               && ch.Deleted == null &&
                                                 (category == null || pr3.Category == category)
                                               orderby pr3.InternalChanID, pr3.TsStartMO
                                               select new
                                               {
                                                   ProgrammesID = pr3.ProgrammesID,
                                                   CID = pr3.CID,
                                                   ChannelName = ch.TitleChannel,
                                                   ChannelContent = mp.Path25 + mp.FileName,
                                                   InternalChanID = pr3.InternalChanID,
                                                   Start = pr3.TsStart,
                                                   Stop = pr3.TsStop,
                                                   TsStartMO = pr3.TsStartMO,
                                                   TsStopMO = pr3.TsStopMO,
                                                   TelecastTitle = pr3.Title,
                                                   TelecastDescr = pr3.Descr,
                                                   AnonsContent = ((pr3.Descr != null && pr3.Descr != string.Empty) ?
                                                   dataContext.MediaPic.FirstOrDefault(mp => mp.FileName == "GreenAnons.png").Path25 + dataContext.MediaPic.FirstOrDefault(mp => mp.FileName == "GreenAnons.png").FileName :
                                                   null),
                                                   Category = pr3.Category,
                                                   Remain = (int)DbFunctions.DiffSeconds(DateTime.Now, pr3.TsStartMO)
                                               }
                                               ).AsNoTracking();

                            var sp3 = (from pr in sp2.ToList()
                                  join prin in stopAfter on pr.CID equals prin.CID
                                  where pr.TsStartMO <= prin.TsStopMOAfter && prin.TsStopMOAfter < pr.TsStopMO
                                  select new SystemProgramme()
                                  {
                                      ProgrammesID = pr.ProgrammesID,
                                      CID = pr.CID,
                                      ChannelName = pr.ChannelName,
                                      ChannelContent = pr.ChannelContent,
                                      InternalChanID = pr.InternalChanID,
                                      Start = pr.Start,
                                      Stop = pr.Stop,
                                      TsStartMO = pr.TsStartMO,
                                      TsStopMO = pr.TsStopMO,
                                      TelecastTitle = pr.TelecastTitle,
                                      TelecastDescr = pr.TelecastDescr,
                                      AnonsContent = pr.AnonsContent,
                                      Category = pr.Category,
                                      Remain = pr.Remain
                                  }).Select(mapper.Map<SystemProgramme>).ToList();
                            count = sp3.Count();
                            if (!string.IsNullOrWhiteSpace(sidx))
                                systemProgramme = LinqExtensions.OrderBy(sp3.AsQueryable(), sidx, sord).Skip((page - 1) * rows).Take(rows).ToList<SystemProgramme>();
                            else systemProgramme = sp3.Skip((page - 1) * rows).Take(rows).ToList<SystemProgramme>();
                            SetGenres(systemProgramme, null);
                            systemProgramme = FilterGenres(systemProgramme, genres);
                        }
                        else if (dateTime > minDate)
                        {
                            var sp4 = (from pr in dataContext.Programmes.AsNoTracking()
                                       join ch in dataContext.Channels.AsNoTracking() on pr.CID equals ch.ChannelID
                                       join mp in dataContext.MediaPic.AsNoTracking() on ch.IconID equals mp.IconID into chmp
                                       from mp in chmp.DefaultIfEmpty()
                                       where pr.TID == typeProgID && DateTime.Now < pr.TsStartMO && pr.TsStartMO <= dateTime
                                       && pr.Category != "Для взрослых" && !pr.Title.Contains("(18+)")
                                       && ch.Deleted == null &&
                                         (category == null || pr.Category == category)
                                       orderby pr.InternalChanID, pr.TsStart
                                       select new
                                       {
                                           ProgrammesID = pr.ProgrammesID,
                                           CID = pr.CID,
                                           ChannelName = ch.TitleChannel,
                                           ChannelContent = mp.Path25 + mp.FileName,
                                           InternalChanID = pr.InternalChanID ?? 0,
                                           Start = pr.TsStart,
                                           Stop = pr.TsStop,
                                           TsStartMO = pr.TsStartMO,
                                           TsStopMO = pr.TsStopMO,
                                           TelecastTitle = pr.Title,
                                           TelecastDescr = pr.Descr,
                                           AnonsContent = ((pr.Descr != null && pr.Descr != string.Empty) ?
                                           dataContext.MediaPic.FirstOrDefault(mp => mp.FileName == "GreenAnons.png").Path25 +
                                           dataContext.MediaPic.FirstOrDefault(mp => mp.FileName == "GreenAnons.png").FileName :
                                           null),
                                           Category = pr.Category,
                                           Remain = (int)DbFunctions.DiffSeconds(DateTime.Now, pr.TsStartMO)
                                       });
                            count = sp4.Count();
                            if (!string.IsNullOrWhiteSpace(sidx))
                                systemProgramme = LinqExtensions.OrderBy(sp4.AsQueryable(), sidx, sord).Skip((page - 1) * rows).Take(rows).Select(mapper.Map<SystemProgramme>).ToList<SystemProgramme>();
                            else systemProgramme = sp4.Skip((page - 1) * rows).Take(rows).Select(mapper.Map<SystemProgramme>).ToList<SystemProgramme>();

                            SetGenres(systemProgramme, null);
                            systemProgramme = FilterGenres(systemProgramme, genres);
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
        /// <param name="typeProgID">Тип телепередач</param>
        /// <param name="cid">Идентификатор канала</param>
        /// <param name="tsStart">Время начала</param>
        /// <param name="tsStop">Время окончания</param>
        public List<SystemProgramme> GetProgrammeOfDay(int typeProgID, int cid, DateTime tsStart, DateTime tsStop)
        {
            List<SystemProgramme> systemProgrammes = new List<SystemProgramme>();
            try
            {
                systemProgrammes = (from pr in dataContext.Programmes.AsNoTracking()
                                    join ch in dataContext.Channels.AsNoTracking() on pr.CID equals ch.ChannelID
                                    join mp in dataContext.MediaPic.AsNoTracking() on ch.IconID equals mp.IconID into chmp
                                    from mp in chmp.DefaultIfEmpty()
                                    where pr.TID == typeProgID &&
                                    ch.ChannelID == cid &&
                                    pr.TsStartMO >= tsStart &&
                                    pr.TsStopMO <= tsStop &&
                                    pr.Category != "Для взрослых" && !pr.Title.Contains("(18+)")
                                    && ch.Deleted == null
                                    select new
                                    {
                                        ProgrammesID = pr.ProgrammesID,
                                        CID = pr.CID,
                                        ChannelName = ch.TitleChannel,
                                        InternalChanID = pr.InternalChanID,
                                        Start = pr.TsStart,
                                        Stop = pr.TsStop,
                                        TsStartMO = pr.TsStartMO,
                                        TsStopMO = pr.TsStopMO,
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
        /// <param name="uid">Идентификатор пользователя</param>
        /// <param name="typeProgID">Тип программы телепередач</param>
        /// <param name="dateTimeOffset">Время</param>
        /// <param name="mode">Режим: 1 - сейчас, 2 - затем</param>
        /// <param name="category">Категория телепередач</param>
        public List<SystemProgramme> GetUserProgrammes(long uid, int typeProgID, DateTimeOffset dateTimeOffset, int mode, string category)
        {
            List<SystemProgramme> systemProgramme = new List<SystemProgramme>();
            DateTime minDate = new DateTime(1800, 1, 1);
            DateTime dateTime = dateTimeOffset.DateTime;
            try
            {
                switch (mode)
                {
                    case 1:
                        systemProgramme = (from pr in dataContext.Programmes.AsNoTracking()
                                           join ch in dataContext.Channels.AsNoTracking() on pr.CID equals ch.ChannelID
                                           join uch in dataContext.UserChannels.AsNoTracking() on ch.ChannelID equals uch.CID
                                           join mp in dataContext.MediaPic.AsNoTracking() on uch.IconID equals mp.IconID into mpch
                                           from mp in mpch.DefaultIfEmpty()
                                           where pr.TID == typeProgID && pr.TsStartMO <= dateTime &&
                                           dateTime < pr.TsStopMO && uch.UID == uid
                                           && ch.Deleted == null
                                           && (category == null || pr.Category == category)
                                           orderby pr.InternalChanID, pr.TsStartMO
                                           select new
                                           {
                                               ProgrammesID = pr.ProgrammesID,
                                               CID = pr.CID,
                                               ChannelName = (string.IsNullOrEmpty(uch.DisplayName) ? ch.TitleChannel : uch.DisplayName),
                                               InternalChanID = pr.InternalChanID ?? 0,
                                               ChannelContent = mp.Path25 + mp.FileName,
                                               Start = pr.TsStart,
                                               Stop = pr.TsStop,
                                               TsStartMO = pr.TsStartMO,
                                               TsStopMO = pr.TsStopMO,
                                               TelecastTitle = pr.Title,
                                               TelecastDescr = pr.Descr,
                                               OrderCol = uch.OrderCol,
                                               AnonsContent = ((pr.Descr != null && pr.Descr != string.Empty) ?
                                               dataContext.MediaPic.FirstOrDefault(mp => mp.FileName == "GreenAnons.png").Path25 +
                                               dataContext.MediaPic.FirstOrDefault(mp => mp.FileName == "GreenAnons.png").FileName :
                                               null),
                                               Category = pr.Category,
                                               Remain = (int)(DbFunctions.DiffSeconds(pr.TsStopMO, dateTime) * 1.0 / (DbFunctions.DiffSeconds(pr.TsStopMO, pr.TsStartMO) * 1.0) * 100.0),
                                           }).ToList().DistinctBy(x => x.ProgrammesID).OrderBy(x => x.OrderCol).Select(mapper.Map<SystemProgramme>).ToList();
                        SetGenres(systemProgramme, uid);
                        SetRatings(systemProgramme, uid);
                        break;
                    case 2:
                        if (dateTime == minDate)
                        {
                            List<TsStopForRemain> stopAfter = (from pr2 in dataContext.Programmes.AsNoTracking()
                                                               join ch2 in dataContext.Channels.AsNoTracking() on pr2.CID equals ch2.ChannelID
                                                               join uch2 in dataContext.UserChannels.AsNoTracking() on ch2.ChannelID equals uch2.CID
                                                               where pr2.TsStartMO <= DateTime.Now && DateTime.Now < pr2.TsStopMO && pr2.TID == typeProgID
                                                               select new
                                                               {
                                                                   TsStopMOAfter = DbFunctions.AddSeconds(pr2.TsStopMO, 1),
                                                                   CID = pr2.CID
                                                               }).Select(mapper.Map<TsStopForRemain>).ToList();
                            DateTime afterTwoDays = DateTime.Now.AddDays(2);
                            systemProgramme = (from pr in dataContext.Programmes.AsNoTracking()
                                               join ch in dataContext.Channels.AsNoTracking() on pr.CID equals ch.ChannelID
                                               join uch in dataContext.UserChannels.AsNoTracking() on ch.ChannelID equals uch.CID
                                               join mp in dataContext.MediaPic.AsNoTracking() on uch.IconID equals mp.IconID into mpch
                                               from mp in mpch.DefaultIfEmpty()
                                               where pr.TID == typeProgID && pr.TsStartMO >= DateTime.Now && afterTwoDays > pr.TsStopMO
                                               && uch.UID == uid
                                               && ch.Deleted == null
                                               && (category == null || pr.Category == category)
                                               orderby pr.InternalChanID, pr.TsStartMO
                                               select new
                                               {
                                                   ProgrammesID = pr.ProgrammesID,
                                                   CID = pr.CID,
                                                   ChannelName = (string.IsNullOrEmpty(uch.DisplayName) ? ch.TitleChannel : uch.DisplayName),
                                                   ChannelContent = mp.Path25 + mp.FileName,
                                                   InternalChanID = pr.InternalChanID ?? 0,
                                                   Start = pr.TsStart,
                                                   Stop = pr.TsStop,
                                                   TsStartMO = pr.TsStartMO,
                                                   TsStopMO = pr.TsStopMO,
                                                   TelecastTitle = pr.Title,
                                                   Title = pr.Title,
                                                   TelecastDescr = pr.Descr,
                                                   AnonsContent = ((pr.Descr != null && pr.Descr != string.Empty) ?
                                                   dataContext.MediaPic.FirstOrDefault(mp => mp.FileName == "GreenAnons.png").Path25 +
                                                   dataContext.MediaPic.FirstOrDefault(mp => mp.FileName == "GreenAnons.png").FileName :
                                                   null),
                                                   OrderCol = uch.OrderCol,
                                                   Category = pr.Category,
                                                   Remain = DbFunctions.DiffSeconds(DateTime.Now, pr.TsStartMO),

                                               }).AsNoTracking().OrderBy(x => x.OrderCol).ToList().Select(mapper.Map<SystemProgramme>).ToList();
                            systemProgramme = (from pr in systemProgramme
                                               join prin in stopAfter on pr.CID equals prin.CID
                                               where pr.TsStartMO <= prin.TsStopMOAfter && prin.TsStopMOAfter < pr.TsStopMO
                                               select new SystemProgramme()
                                               {
                                                   ProgrammesID = pr.ProgrammesID,
                                                   CID = pr.CID,
                                                   ChannelName = pr.ChannelName,
                                                   ChannelContent = pr.ChannelContent,
                                                   InternalChanID = pr.InternalChanID,
                                                   Start = pr.Start,
                                                   Stop = pr.Stop,
                                                   TsStartMO = pr.TsStartMO,
                                                   TsStopMO = pr.TsStopMO,
                                                   TelecastTitle = pr.TelecastTitle,
                                                   TelecastDescr = pr.TelecastDescr,
                                                   AnonsContent = pr.AnonsContent,
                                                   Category = pr.Category,
                                                   Remain = pr.Remain
                                               }).ToList().DistinctBy(x => x.ProgrammesID).Select(mapper.Map<SystemProgramme>).ToList(); ;
                            SetGenres(systemProgramme, uid);
                            SetRatings(systemProgramme, uid);
                        }
                        else if (dateTime > minDate)
                        {
                            systemProgramme = (from pr in dataContext.Programmes.AsNoTracking()
                                               join ch in dataContext.Channels.AsNoTracking() on pr.CID equals ch.ChannelID
                                               join uch in dataContext.UserChannels.AsNoTracking() on ch.ChannelID equals uch.CID
                                               join mp in dataContext.MediaPic.AsNoTracking() on uch.IconID equals mp.IconID into mpch
                                               from mp in mpch.DefaultIfEmpty()
                                               where DateTime.Now < pr.TsStartMO && pr.TsStartMO <= dateTime
                                               && uch.UID == uid
                                               && ch.Deleted == null &&
                                               (category == null || pr.Category == category)
                                               orderby pr.InternalChanID, pr.TsStartMO
                                               select new
                                               {
                                                   ProgrammesID = pr.ProgrammesID,
                                                   CID = pr.CID,
                                                   ChannelName = (string.IsNullOrEmpty(uch.DisplayName) ? ch.TitleChannel : uch.DisplayName),
                                                   ChannelContent = mp.Path25 + mp.FileName,
                                                   InternalChanID = pr.InternalChanID ?? 0,
                                                   Start = pr.TsStart,
                                                   Stop = pr.TsStop,
                                                   TsStartMO = pr.TsStartMO,
                                                   TsStopMO = pr.TsStopMO,
                                                   TelecastTitle = pr.Title,
                                                   TelecastDescr = pr.Descr,
                                                   AnonsContent = ((pr.Descr != null && pr.Descr != string.Empty) ?
                                                   dataContext.MediaPic.FirstOrDefault(mp => mp.FileName == "GreenAnons.png").Path25 +
                                                   dataContext.MediaPic.FirstOrDefault(mp => mp.FileName == "GreenAnons.png").FileName :
                                                   null),
                                                   OrderCol = uch.OrderCol,
                                                   Category = pr.Category,
                                                   Remain = (int)DbFunctions.DiffSeconds(DateTime.Now, pr.TsStartMO),
                                               }
                                               ).AsNoTracking().OrderBy(o => o.OrderCol).ToList().Select(mapper.Map<SystemProgramme>).ToList();
                            SetGenres(systemProgramme, uid);
                            SetRatings(systemProgramme, uid);
                        }
                        break;
                }
            }
            catch (Exception ex)
            {
                Logger.Error(ex, ex.Message);
            }
            return systemProgramme;
        }
        
        /// <summary>
        /// Получение пользовательских телепередач за день
        /// </summary>
        /// <param name="uid">Идентификатор пользователя</param>
        /// <param name="typeProgID">Тип программы телепередач</param>
        /// <param name="cid">Идентификатор телеканала</param>
        /// <param name="tsStart">Время начала выборки телепередач</param>
        /// <param name="tsStop">Время окончания выборки телепередач</param>
        /// <param name="category">Категория телепередач</param>
        public List<SystemProgramme> GetUserProgrammeOfDay(long uid, int typeProgID, int cid, DateTime tsStart, DateTime tsStop, string category)
        {
            List<SystemProgramme> systemProgramme = null;

            try
            {
                systemProgramme = (from pr in dataContext.Programmes.AsNoTracking()
                                   join ch in dataContext.Channels.AsNoTracking() on pr.CID equals ch.ChannelID
                                   join uch in dataContext.UserChannels.AsNoTracking() on ch.ChannelID equals uch.CID
                                   join mp in dataContext.MediaPic.AsNoTracking() on uch.IconID equals mp.IconID into mpch
                                   from mp in mpch.DefaultIfEmpty()
                                   where pr.TID == typeProgID &&
                                         ch.ChannelID == cid &&
                                         pr.TsStartMO >= tsStart &&
                                         pr.TsStopMO < tsStop &&
                                         uch.UID == uid
                                         && ch.Deleted == null
                                         && (category == null || pr.Category == category)
                                   orderby uch.OrderCol, pr.TsStartMO
                                   select new
                                   {
                                       ProgrammesID = pr.ProgrammesID,
                                       CID = pr.CID,
                                       ChannelName = ch.TitleChannel,
                                       InternalChanID = pr.InternalChanID,
                                       ChannelContent = mp.Path25 + mp.FileName,
                                       Start = pr.TsStart,
                                       TsStartMO = pr.TsStartMO,
                                       Stop = pr.TsStop,
                                       TsStopMO = pr.TsStopMO,
                                       TelecastTitle = pr.Title,
                                       TelecastDescr = pr.Descr,
                                       AnonsContent = ((pr.Descr != null && pr.Descr != string.Empty) ?
                                       dataContext.MediaPic.FirstOrDefault(mp => mp.FileName == "GreenAnons.png").Path25 +
                                       dataContext.MediaPic.FirstOrDefault(mp => mp.FileName == "GreenAnons.png").FileName :
                                                                   null),
                                       OrderCol = uch.OrderCol,
                                       Category = pr.Category,
                                   }).ToList().DistinctBy(x => x.ProgrammesID).Select(mapper.Map<SystemProgramme>).ToList();
                SetGenres(systemProgramme, uid);
                SetRatings(systemProgramme, uid);
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
        /// <param name="typeProgID">Тип программы телепередач</param>
        /// <param name="findTitle">Поисковая подстрока</param>
        public List<SystemProgramme> SearchProgramme(int typeProgID, string findTitle)
        {
            List<SystemProgramme> systemProgramme = new List<SystemProgramme>();
            try
            {
                systemProgramme = (from pr in dataContext.Programmes.AsNoTracking()
                                   join ch in dataContext.Channels.AsNoTracking() on pr.CID equals ch.ChannelID
                                   join mp in dataContext.MediaPic.AsNoTracking() on ch.IconID equals mp.IconID into chmp
                                   from mp in chmp.DefaultIfEmpty()
                                   where pr.TID == typeProgID
                                        && pr.Category != "Для взрослых" && !pr.Title.Contains("(18+)")
                                        && ch.Deleted == null
                                        && pr.Title.Contains(findTitle)
                                   orderby pr.TsStartMO, pr.TsStopMO
                                   select new
                                   {
                                       ProgrammesID = pr.ProgrammesID,
                                       CID = pr.CID,
                                       ChannelName = ch.TitleChannel,
                                       ChannelContent = mp.Path25 + mp.FileName,
                                       InternalChanID = pr.InternalChanID ?? 0,
                                       Start = pr.TsStart,
                                       Stop = pr.TsStop,
                                       TsStartMO = pr.TsStartMO,
                                       TsStopMO = pr.TsStopMO,
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
                    pr.DayMonth = pr.TsStartMO.ToString("ddd", new CultureInfo("ru-Ru")) +
               String.Format("({0:D2}.{1:D2})", pr.TsStartMO.Day, pr.TsStartMO.Month);
                });
                SetGenres(systemProgramme, null);
            }
            catch (Exception ex)
            {
            }
            return systemProgramme;
        }

        /// <summary>
        /// Поиск телепередач по пользовательской (кастомизированной) программе
        /// </summary>
        /// <param name="uid">Идентификатор пользователя</param>
        /// <param name="typeProgID">Тип программы телепередач</param>
        /// <param name="findTitle">Поисковая подстрока</param>
        public List<SystemProgramme> SearchUserProgramme(long uid, int typeProgID, string findTitle)
        {
            List<SystemProgramme> systemProgramme = new List<SystemProgramme>();
            try
            {
                systemProgramme = (from pr in dataContext.Programmes.AsNoTracking()
                                   join ch in dataContext.Channels.AsNoTracking() on pr.CID equals ch.ChannelID
                                   join uch in dataContext.UserChannels.AsNoTracking() on ch.ChannelID equals uch.CID
                                   join mp in dataContext.MediaPic.AsNoTracking() on uch.IconID equals mp.IconID into chmp
                                   from mp in chmp.DefaultIfEmpty()
                                   where uch.UID == uid 
                                        && pr.TID == typeProgID
                                        && ch.Deleted == null
                                        && pr.Title.Contains(findTitle)
                                   orderby pr.TsStartMO, pr.TsStopMO
                                   select new
                                   {
                                       ProgrammesID = pr.ProgrammesID,
                                       CID = pr.CID,
                                       ChannelName = ch.TitleChannel,
                                       ChannelContent = mp.Path25 + mp.FileName,
                                       InternalChanID = pr.InternalChanID ?? 0,
                                       Start = pr.TsStart,
                                       Stop = pr.TsStop,
                                       TsStartMO = pr.TsStartMO,
                                       TsStopMO = pr.TsStopMO,
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
                    pr.DayMonth = pr.TsStartMO.ToString("ddd", new CultureInfo("ru-Ru")) +
                    String.Format("({0:D2}.{1:D2})", pr.TsStartMO.Day, pr.TsStartMO.Month);
                });
                SetGenres(systemProgramme, uid);
                SetRatings(systemProgramme, uid);
            }
            catch (Exception ex)
            {
            }
            return systemProgramme;
        }

        #endregion
    }
}