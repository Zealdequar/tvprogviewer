using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using NLog;
using System.Xml.Linq;
using TvProgViewer.Core;
using TvProgViewer.Data;
using TvProgViewer.Core.Domain.TvProgMain;
using System.Linq;
using LinqToDB.Data;
using LinqToDB;
using System.Diagnostics;
using TvProgViewer.Services.TvProgMain;
using System.Xml.XPath;
using TVProgViewer.Services.TvProgMain;
using System.Data;
using MimeKit.Cryptography;
using DocumentFormat.OpenXml.Bibliography;

namespace TvProgViewer.Services.Common
{
    /// <summary>
    /// Represents the HTTP client to request current store
    /// </summary>
    public partial class StoreHttpClient
    {
        #region Fields

        private readonly HttpClient _httpClient;
        private readonly IDataProviderManager _dataProviderManager;
        private readonly IRepository<WebResources> _webResourcesRepository;
        private readonly IRepository<TypeProg> _typeProgRepository;
        private readonly IRepository<Channels> _channelsRepository;
        private readonly IRepository<Programmes> _programmesRepository;
        private readonly IRepository<UpdateProgLog> _updateProgLogRepository;
        private static Logger _logger = LogManager.GetLogger("dbupdate");
        private static Dictionary<string, string> _dictChanCodeOld = new Dictionary<string, string>();
        private static Dictionary<string, string> _dictChanCodeNew = new Dictionary<string, string>();
        private int _channelQty = 0;
        private int _newChannelQty = 0;
        private int _programmeQty = 0;
        private DateTime _minProgDate = DateTime.MinValue;
        private DateTime _maxProgDate = DateTime.MaxValue;

        #endregion

        #region Ctor


        public StoreHttpClient(HttpClient client
        , IWebHelper webHelper
        , IDataProviderManager dataProviderManager
        , IRepository<WebResources> webResourcesRepository
        , IRepository<TypeProg> typeProgRepository
        , IRepository<Channels> channelsRepository
        , IRepository<Programmes> programmesRepository
        , IRepository<UpdateProgLog> updateProgLogRepository)
        {
            //configure client
            client.BaseAddress = new Uri(webHelper.GetStoreLocation());

            _httpClient = client;

            _dataProviderManager = dataProviderManager;
            _webResourcesRepository = webResourcesRepository;
            _typeProgRepository = typeProgRepository;
            _channelsRepository = channelsRepository;
            _programmesRepository = programmesRepository;
            _updateProgLogRepository = updateProgLogRepository;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Keep the current store site alive
        /// </summary>
        /// <returns>
        /// Задача представляет асинхронную операцию
        /// The task result contains the asynchronous task whose result determines that request completed
        /// </returns>
        public virtual async Task KeepAliveAsync()
        {
            await _httpClient.GetStringAsync(TvProgCommonDefaults.KeepAlivePath);
        }

        /// <summary>
        /// Получение всех веб-ресурсов
        /// </summary>
        /// <param name="showHidden">Показывать ли скрытые записи</param>
        public virtual async Task<IList<WebResources>> GetAllWebResourcesAsync(bool showHidden = false)
        {
            var webResources = await _webResourcesRepository.GetAllAsync(query =>
            {
                return query;
            }, default);
            return await webResources.ToListAsync();
        }

        /// <summary>
        /// Получение типов программы передач
        /// </summary>
        /// <param name="showHidden">Показывать ли скрытые записи</param>
        /// <returns></returns>
        public virtual async Task<IList<TypeProg>> GetAllTypeProgAsync(bool showHidden = false)
        {
            var typeProgs = await _typeProgRepository.GetAllAsync(query =>
            {
                return query;
            }, default);
            return await typeProgs.ToListAsync();
        }

        /// <summary>
        /// Получение всех неудалённых каналов
        /// </summary>
        /// <param name="showHidden">Показывать ли скрытые записи</param>
        /// <returns></returns>
        public virtual async Task<IList<Channels>> GetAllChannelsAsync(bool showHidden = false)
        {
            var channels = await _channelsRepository.GetAllAsync(query =>
            {
                return query.Where(x => x.Deleted == null);
            }, default);
            return await channels.ToListAsync();
        }

        /// <summary>
        /// Получение всей телепрограммы
        /// </summary>
        /// <param name="showHidden"></param>
        /// <returns></returns>
        public virtual async Task<IList<Programmes>> GetAllProgrammesAsync(bool showHidden = false)
        {
            var programmes = await _programmesRepository.GetAllAsync(query =>
            {
                return query;
            }, default);
            return await programmes.ToListAsync();
        }

        /// <summary>
        /// Преобразование строки к дате и времени
        /// </summary>
        /// <param name="rawDateTime">Сырая строка</param>
        /// <returns>Дата и время</returns>
        public DateTime GetDateTimeValue(string rawDateTime)
        {
            string strDateTime = rawDateTime;
            int year = int.Parse(strDateTime.Substring(0, 4));
            int month = int.Parse(strDateTime.Substring(4, 2));
            int day = int.Parse(strDateTime.Substring(6, 2));
            int hour = int.Parse(strDateTime.Substring(8, 2));
            int minute = int.Parse(strDateTime.Substring(10, 2));
            int second = int.Parse(strDateTime.Substring(12, 2));
            DateTime tsDateTime = new DateTime(year, month, day, hour, minute, second);
            return tsDateTime;
        }

        /// <summary>
        /// Получение типа программы по идентификатору
        /// </summary>
        /// <param name="typeProgId">Идентификатор типа программы</param>
        /// <returns>Тип программы</returns>
        public virtual async Task<TypeProg> GetTypeProgByIdAsync(int typeProgId)
        {
            return await _typeProgRepository.GetByIdAsync(typeProgId, cache => default);
        }

        /// <summary>
        /// Запуск хранимой процедуры обработки и обновления телепрограммы
        /// </summary>
        /// <param name="webResourceId">Идентификатор веб-ресурса</param>
        /// <param name="tvProgXml">Xml-формат телепрограммы</param>
        /// <returns></returns>
//        public virtual async Task DelTwoWeekAgo(long irron)
//        {
//            await _dataProviderManager.DataProvider.QueryProcAsync<int>("sp_del_two_week_ago",
//               new DataParameter[] {
//                new DataParameter("@irron", irron, DataType.Int64)
//                });
//        }

        /// <summary>
        /// Обновление данных о пиктограмме
        /// </summary>
        /// <param name="channelId">Идентификатор канала</param>
        /// <param name="iconWebSrc">Адрес пиктограммы в интернете</param>
        /// <param name="channelIconName">Название пиктограммы</param>
        /// <param name="contentType">Тип содержимого (ContentType) пиктограммы</param>
        /// <param name="contentCoding">Кодировка пиктограммы</param>
        /// <param name="channelOrigIcon">Оригинальная пиктограмма</param>
        public virtual async Task UpdateIconAsync(int channelId, string iconWebSrc, string channelIconName, string contentType, string contentCoding
            , byte[] channelOrigIcon)
        {
            await _dataProviderManager.DataProvider.ExecuteNonQueryAsync("spUdtChannelImage",
                new DataParameter[] {
                    new DataParameter("@CID", channelId, DataType.Int32),
                    new DataParameter("@IconWebSrc", iconWebSrc, DataType.NVarChar),
                    new DataParameter("@ChannelIconName", channelIconName, DataType.NVarChar),
                    new DataParameter("@ContentType", contentType, DataType.NVarChar),
                    new DataParameter("@ContentCoding", contentCoding, DataType.NVarChar),
                    new DataParameter("@ChannelOrigIcon", channelOrigIcon, DataType.VarBinary),
                    new DataParameter("@IsSystem", 1, DataType.Boolean),
                    new DataParameter("@ErrCode", DataType.NVarChar){Direction = System.Data.ParameterDirection.Output}
                });
        }

        /// <summary>
        /// Обновление телеканалов программы передач
        /// </summary>
        public virtual async Task UpdateTvProgrammes()
        {
            _logger.Info(" =========== Начало обновления ============ ");
            Stopwatch sw = Stopwatch.StartNew();
            IList<WebResources> webResources = await GetAllWebResourcesAsync();
            IList<TypeProg> typeProgs = await GetAllTypeProgAsync();
            _logger.Trace("Ресурсы из базы успешно получены: {0}", string.Join(";", webResources.ToList().Select(
                   wr => string.Format("WRID = '{0}', FileName = '{1}', ResourceName = '{2}', ResourceUrl ='{3}'",
                   wr.Id, wr.FileName, wr.ResourceName, wr.ResourceUrl))));

            foreach (WebResources webResource in webResources)
            {
                if (webResource.TypeProgId == 2)
                    continue;
                _channelQty = 0;
                _newChannelQty = 0;
                try
                {
                    _logger.Info("Обновление для ID='{0}'", webResource.Id);
                    Stopwatch swLoc = Stopwatch.StartNew();
                    string fileName = webResource.FileName;
                    WebPart.GetWebTVProgramm(new Uri(webResource.ResourceUrl), ref fileName);
                    webResource.FileName = fileName;
                    XDocument xDoc = new XDocument();
                    var sourceType = (await GetTypeProgByIdAsync(webResource.TypeProgId))
                        .TypeName == "Формат XMLTV" ? Enums.TypeProg.XMLTV : Enums.TypeProg.InterTV;
                    if (sourceType == Enums.TypeProg.XMLTV)
                    {
                        if (webResource.Id == 6)
                        {
                            string tempFile = Path.GetTempFileName();

                            using (var streamReader = new StreamReader(webResource.FileName))
                            using (var streamWriter = new StreamWriter(tempFile))
                            {
                                string line;

                                while ((line = streamReader.ReadLine()) != null)
                                {
                                    if (!line.Contains("}{отт@бь)ч"))
                                        streamWriter.WriteLine(line);
                                }
                            }

                            File.Delete(webResource.FileName);
                            File.Move(tempFile, webResource.FileName);
                        }

                        xDoc = XDocument.Load(webResource.FileName, LoadOptions.SetLineInfo);
                        xDoc = WebPart.ReformatXDoc(xDoc);
                        if (webResource.Id == 1)
                        {
                            foreach (XElement xChan in xDoc.XPathSelectElements("tv/channel"))
                            {
                                string id = xChan.Attribute("id").Value;
                                _dictChanCodeOld[xChan.Element("display-name").Value] = id;
                            }
                        }
                        else if (webResource.Id == 2)
                        {
                            foreach (XElement xChan in xDoc.XPathSelectElements("tv/channel"))
                            {
                                string id = xChan.Attribute("id").Value;
                                _dictChanCodeNew[xChan.Element("display-name").Value] = xChan.Attribute("id").Value;
                            }
                        }
                    }                   

                    _logger.Info("Каналы и тв-программа ID='{0}' подготовлены к запуску обработки", webResource.Id);
                    // Сбор статистики, поиск и загрузка новых телеканалов:
                    var tsUpdateStart = DateTime.Now;
                    IEnumerable<XElement> docChannels = xDoc.XPathSelectElements("tv/channel");
                    _channelQty = docChannels.Count();
                    IEnumerable<XAttribute> internalIds = docChannels.Attributes("id");
                    IList<Channels> channels = await GetAllChannelsAsync();
                    IEnumerable<int> difference = internalIds.Select(attr => int.Parse(attr.Value))
                         .Except(channels.Where(q => q.Deleted is null).Select(x => x.InternalId ?? 1));
                    _newChannelQty = difference.Count();
                    int tvProgProviderId = typeProgs.FirstOrDefault(s => s.Id == webResource.TypeProgId).TvProgProviderId;
                    IList<Channels> channelsToInsert = new List<Channels>();
                    // Вставка новых телеканалов:
                    foreach (int newInternalId in difference)
                    {
                        XElement xChannel = xDoc.XPathSelectElements("tv/channel").FirstOrDefault(x => x.Attribute("id") != null && (int)x.Attribute("id") == newInternalId);

                        channelsToInsert.Add(new Channels
                        {
                            TvProgProviderId = tvProgProviderId,
                            InternalId = newInternalId,
                            IconId = null,
                            CreateDate = DateTime.Now,
                            TitleChannel = xChannel.Element("display-name").Value,
                            IconWebSrc = null,
                            Deleted = null,
                            SysOrderCol = 1000000000
                        });
                    }
                    if (channelsToInsert.Count > 0)
                    {
                        await _channelsRepository.InsertAsync(channelsToInsert);
                    }
                    // Сбор статистики и загрузка программы передач:
                    IEnumerable<XElement> docProgrammes = xDoc.XPathSelectElements("tv/programme");
                    _programmeQty = docProgrammes.Count();
                    IEnumerable<XAttribute> startAttribute = docProgrammes.Attributes("start");
                    IEnumerable<DateTime> startDateTime = startAttribute.Select(x => GetDateTimeValue(x.Value));
                    _minProgDate = startDateTime.Min();
                    _maxProgDate = startDateTime.Max();
                    IList<Programmes> programmes = await GetAllProgrammesAsync();
                    IList<Programmes> programmesToDelete = programmes.Where(x => x.TsStartMo >= _minProgDate && x.TsStartMo <= _maxProgDate).ToList();
                    // Удаление ранней из интервала загружаемой телепрограммы:
                    await _programmesRepository.DeleteAsync(programmesToDelete);
                    var typeProgId = typeProgs.FirstOrDefault(s => s.Id == webResource.TypeProgId).Id;
                    IList<Programmes> programmesToInsert = new List<Programmes>();
                    foreach (XElement newProgrammeElement in docProgrammes)
                    {
                        var channel = channels.FirstOrDefault(x => x.InternalId == int.Parse(newProgrammeElement.Attribute("channel").Value) &&
                                                                     x.Deleted is null);
                        if (channel is null)
                            { continue; }
                        
                        var channelId = channel.Id;
                        var internalChanId = int.Parse(newProgrammeElement.Attribute("channel").Value);
                        var tsStart = GetDateTimeValue(newProgrammeElement.Attribute("start").Value);
                        var tsStop = GetDateTimeValue(newProgrammeElement.Attribute("stop").Value);
                        var title = newProgrammeElement.Elements("title").Where(x => x.Attribute("lang") != null && x.Attribute("lang").Value == "ru")
                                                                         .FirstOrDefault()?.Value;
                        var descr = newProgrammeElement.Elements("desc").Where(x => x.Attribute("lang") != null && x.Attribute("lang").Value == "ru")
                                                                         .FirstOrDefault()?.Value;
                        var category = newProgrammeElement.Elements("category").Where(x => x.Attribute("lang") != null && x.Attribute("lang").Value == "ru")
                                                                         .FirstOrDefault()?.Value;
                        programmesToInsert.Add(
                            new Programmes
                            {
                                TypeProgId = typeProgId,
                                ChannelId = channelId,
                                InternalChanId = internalChanId,
                                TsStart = tsStart,
                                TsStop = tsStop,
                                TsStartMo = tsStart,
                                TsStopMo = tsStop,
                                Title = title,
                                Descr = descr,
                                Category = category
                            });
                    }

                    if (programmesToInsert.Count > 0)
                    {
                        // Вставка новой программы передач
                        await _programmesRepository.InsertAsync(programmesToInsert);
                    }

                    programmes = await GetAllProgrammesAsync();

                    // Удаление дубликатов:
                    var groupProgrammes = programmes.GroupBy(p => new { p.TypeProgId, p.InternalChanId, p.ChannelId, p.TsStartMo, p.TsStopMo })
                                                                                .SelectMany(g => g.Select((j, i) => new { j, rn = i + 1 }));
                    List<Programmes> dupleProgToDelete = await (from g in groupProgrammes
                                                          where g.rn > 1
                                                          select g.j).ToListAsync();

                    await _programmesRepository.DeleteAsync(dupleProgToDelete);

                    programmes = await GetAllProgrammesAsync();

                    // Удаление программы передач, которая превысила две недели от сегодняшнего дня:
                    List<Programmes> deleleTwoWeekFuture = await programmes.Where(x => x.TsStartMo >= DateTime.Now.AddDays(14)).ToListAsync();
                    await _programmesRepository.DeleteAsync(deleleTwoWeekFuture);

                    programmes = await GetAllProgrammesAsync();

                    // Удаление программы передач за три недели ранее последней передачи:
                    DateTime maxTsStartMo = programmes.Max(x => x.TsStartMo);
                    int[] days = [0, 1, 2, 3, 4, 5, 6];
                    List<Programmes> deleteTwoWeeksAgo = [];
                    foreach (var tsStartMonday in from day in
                                                      from int day in days
                                                      where maxTsStartMo.AddDays(-21).AddDays(day).DayOfWeek == DayOfWeek.Monday
                                                      select day
                                                  let tsStartMonday = maxTsStartMo.AddDays(-21).AddDays(day)
                                                  select tsStartMonday)
                    {
                        deleteTwoWeeksAgo = await programmes.Where(x => x.TsStartMo < tsStartMonday).ToListAsync();
                    }

                    await _programmesRepository.DeleteAsync(deleteTwoWeeksAgo);
                    
                    // Отчёт:
                    TimeSpan tsElapsed = DateTime.Now - tsUpdateStart;
                    await _updateProgLogRepository.InsertAsync(
                        new UpdateProgLog
                        {
                            WebResourceId = webResource.Id,
                            TsUpdateStart = tsUpdateStart,
                            TsUpdateEnd = DateTime.Now,
                            UdtElapsedSec = (int)tsElapsed.TotalSeconds,
                            MinProgDate = _minProgDate,
                            MaxProgDate = _maxProgDate,
                            QtyChans = _channelQty,
                            QtyProgrammes = _programmeQty,
                            QtyNewChans = _newChannelQty,
                            QtyNewProgrammes = _programmeQty,
                            IsSuccess = true,
                            ErrorMessage = null
                        });

                    if (webResource.Id == 2)
                    {
                        _logger.Info("Обработка каналов и тв-программы ID='{0}' завершена. Начало загрузки пиктограмм.", webResource.Id);
                        Stopwatch swPict = Stopwatch.StartNew();
                        foreach (XElement xChan in xDoc.XPathSelectElements("tv/channel"))
                        {
                            string id = xChan.Attribute("id").Value;
                            if (!string.IsNullOrWhiteSpace(id) && xChan.Element("icon") != null)
                            {
                                string src = xChan.Element("icon").Attribute("src").Value;
                                if (!string.IsNullOrWhiteSpace(src))
                                {
                                    byte[] channelIcons = WebPart.GetAndSaveIcon(id, src);
                                    await UpdateIconAsync(Convert.ToInt32(id), src, id + ".gif", "image/gif", "gzip",
                                        channelIcons);
                                }
                            }
                        }
                        swPict.Stop();
                        _logger.Info("Загрузка пиктограмм завершена. Потребовалось {0} сек.", swPict.ElapsedMilliseconds / 1000.0);
                    }
                    swLoc.Stop();
                    _logger.Info("Обновление для ID='{0}' успешно завершено. Затрачено '{1}' сек.", webResource.Id, swLoc.ElapsedMilliseconds / 1000.0);
                }
                catch (Exception ex)
                {
                    _logger.Error("Обновление тв-программы прошло неуспешно: ErrMessage='{0}', StackTrace='{1}'", ex.Message, ex.StackTrace);
                }
            }
            sw.Stop();
            _logger.Info(" ========== Обновление завершено ========== (Затрачено: {0} сек.)", sw.ElapsedMilliseconds / 1000.0);
        }
    }
    #endregion
}
